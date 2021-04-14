using System.Collections.Generic;
using System.Linq;
using LiteDB;
using System.IO;
using System.IO.Compression;
using IV_Play.Data.Models;
using IV_Play.Properties;
using System;

namespace IV_Play.Data
{
  class DatabaseManager : IDisposable
  {
    private MemoryStream _dbMemoryStream;
    private LiteDatabase _database;
    private LiteCollection<Machine> _machinesCollection;
    private LiteCollection<MameInfo> _mameInfoCollection;
    private static bool _upgradeCheck = false;
    private bool _saveChanges = false;

    public static DatabaseManager Create()
    {
      return new DatabaseManager();
    }
    private DatabaseManager()
    {
      Open();
    }

    private void Open()
    {
      _dbMemoryStream = new MemoryStream();

      if (File.Exists(Resources.DB_NAME))
      {
        using (FileStream infileStream = File.Open(Resources.DB_NAME, System.IO.FileMode.OpenOrCreate))
        {
          using (GZipStream gZipStream = new GZipStream(infileStream, CompressionMode.Decompress))
          {
            gZipStream.CopyTo(_dbMemoryStream);
          }
        }
      }

      _database = new LiteDatabase(_dbMemoryStream);
      _machinesCollection = _database.GetCollection<Machine>("machines");
      _mameInfoCollection = _database.GetCollection<MameInfo>("mameinfo");

      if (!_upgradeCheck)
      {
        try
        {
          _mameInfoCollection.EnsureIndex("Version");
          _machinesCollection.EnsureIndex("name");
          _upgradeCheck = true;
        }
        catch (LiteException ex)
        {
          if (ex.ErrorCode == LiteException.INVALID_DATABASE_VERSION)
          {
            Upgrade();
            _upgradeCheck = true;
          }
        }
      }
    }

    private void Upgrade()
    {
      _database.Dispose();
      _dbMemoryStream.Seek(0, SeekOrigin.Begin);
      using (var tempFile = File.Create("upgrade.db"))
      {
        _dbMemoryStream.CopyTo(tempFile);
      }

      var tempDb = new LiteDatabase("filename=upgrade.db;upgrade=true");

      tempDb.Dispose();

      using (var upgradedDb = File.Open("upgrade.db", System.IO.FileMode.Open))
      {
        using (FileStream outFile = File.Open(Resources.DB_NAME, System.IO.FileMode.OpenOrCreate))
        {
          using (GZipStream gZipStream = new GZipStream(outFile, CompressionMode.Compress))
          {
            upgradedDb.CopyTo(gZipStream);
          }
        }
      }

      File.Delete("upgrade.db");
      File.Delete("upgrade-bkp.db");
      Open();
    }

    public void SaveToDisk()
    {
      // No need to save if we didn't update the database
      if (!_saveChanges)
      {
        return;
      }

      using (FileStream outFile = File.Open(Resources.DB_NAME, System.IO.FileMode.OpenOrCreate))
      {
        using (GZipStream gZipStream = new GZipStream(outFile, CompressionMode.Compress))
        {
          _dbMemoryStream.WriteTo(gZipStream);
        }
      }
    }

    public void SaveMachines(List<Machine> machines)
    {
      _machinesCollection.Delete(Query.All());
      _machinesCollection.Insert(machines);
      _saveChanges = true;
    }

    public void UpdateMachines(List<Machine> machines)
    {
      var indexes = _machinesCollection.FindAll().ToDictionary(x => x.name);
      machines.ForEach(x =>
      {
        x.Id = indexes[x.name].Id;
      });
      _machinesCollection.Upsert(machines);

      // Delete any machines that we didn't update, which are devices.
      // We can tell which devices these are because they don't have a year.
      var devices = _machinesCollection.FindAll().Where(x => x.year == null);
      devices.ToList().ForEach(x => { _machinesCollection.Delete(x.Id); });
      _saveChanges = true;
    }

    public List<Machine> GetMachines()
    {
      return _machinesCollection.FindAll().ToList();
    }

    public Machine GetMachineByName(string name)
    {
      return _machinesCollection.FindOne(m => m.name == name);
    }

    public void SaveMameInfo(MameInfo mameInfo)
    {
      _mameInfoCollection.Delete(Query.All());
      _mameInfoCollection.Insert(mameInfo);
      _saveChanges = true;
    }

    public MameInfo GetMameInfo()
    {
      return _mameInfoCollection.FindOne(Query.All());
    }

    public void Dispose()
    {
      SaveToDisk();      
      _database.Dispose();
      _dbMemoryStream.Close();
    }
  }
}
