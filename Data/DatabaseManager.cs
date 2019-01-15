using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDB;
using System.IO;
using System.IO.Compression;
using IV_Play.Data.Models;
using IV_Play.Properties;

namespace IV_Play.Data
{
    static class DatabaseManager
    {
        private static MemoryStream dbMemoryStream;
        private static LiteDatabase database;
        private static LiteCollection<Machine> machinesCollection;
        private static LiteCollection<MameInfo> mameInfoCollection;
        
        static DatabaseManager()
        {
            Open();                      
        }

        private static void Open()
        {
            dbMemoryStream = new MemoryStream();

            if (File.Exists(Resources.DB_NAME))
            {
                using (FileStream infileStream = File.Open(Resources.DB_NAME, System.IO.FileMode.OpenOrCreate))
                {
                    using (GZipStream gZipStream = new GZipStream(infileStream, CompressionMode.Decompress))
                    {
                        gZipStream.CopyTo(dbMemoryStream);
                    }
                }
            }

            database = new LiteDatabase(dbMemoryStream);

            machinesCollection = database.GetCollection<Machine>("machines");
            mameInfoCollection = database.GetCollection<MameInfo>("mameinfo");

            try
            {
                mameInfoCollection.EnsureIndex("Version");
                machinesCollection.EnsureIndex("name");
            }
            catch (LiteException ex)
            {
                if (ex.Message == "Invalid database version: 6")
                {
                    Upgrade();
                }
            }
        }

        private static void Upgrade()
        {
            database.Dispose();
            dbMemoryStream.Seek(0, SeekOrigin.Begin);
            using (var tempFile = File.Create("upgrade.db"))
            {
                dbMemoryStream.CopyTo(tempFile);
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

        public static void SaveToDisk()
        {
            using (FileStream outFile = File.Open(Resources.DB_NAME, System.IO.FileMode.OpenOrCreate))
            {
                using (GZipStream gZipStream = new GZipStream(outFile, CompressionMode.Compress))
                {
                    dbMemoryStream.WriteTo(gZipStream);                    
                }
            }
        }

        public static void SaveMachines(List<Machine> machines)
        {
            machinesCollection.Delete(Query.All());            
            machinesCollection.Insert(machines);            
        }

        public static void UpdateMachines(List<Machine> machines)
        {
            var indexes = machinesCollection.FindAll().ToDictionary(x => x.name);
            machines.ForEach(x => {
                x.Id = indexes[x.name].Id;                    
            });                
            machinesCollection.Upsert(machines);                

            // Delete any machines that we didn't update, which are devices.
            // We can tell which devices these are because they don't have a year.
            var devices = machinesCollection.FindAll().Where(x => x.year == null);
            devices.ToList().ForEach(x => { machinesCollection.Delete(x.Id); });
        }

        public static List<Machine> GetMachines()
        {
            return machinesCollection.FindAll().ToList();
        }

        public static Machine GetMachineByName(string name)
        {
            return machinesCollection.FindOne(m => m.name == name);
        }

        public static void SaveMameInfo(MameInfo mameInfo)
        {
            mameInfoCollection.Delete(Query.All());
            mameInfoCollection.Insert(mameInfo);
        }

        public static MameInfo GetMameInfo()
        {
            return mameInfoCollection.FindOne(Query.All());
        }
    }
}
