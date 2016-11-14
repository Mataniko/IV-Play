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
    class DatabaseManager : IDisposable
    {
        private MemoryStream dbMemoryStream;        
        private LiteDatabase database;
        private LiteCollection<Machine> machinesCollection;
                        
        public DatabaseManager()
        {
            Open();
            machinesCollection = database.GetCollection<Machine>("machines");
        }

        public LiteDatabase Database { 
            get
            {
                return database;
            }
        }

        private void Open()
        {
            using (FileStream infileStream = File.Open(Resources.DB_NAME, FileMode.OpenOrCreate))
            {
                using (GZipStream gZipStream = new GZipStream(infileStream, CompressionMode.Decompress))
                {
                    dbMemoryStream = new MemoryStream();
                    gZipStream.CopyTo(dbMemoryStream);
                }
            }

            database = new LiteDatabase(dbMemoryStream);           
        }

        private void Close()
        {
            using (dbMemoryStream)
            {
                using (FileStream outFile = File.Open(Resources.DB_NAME, FileMode.OpenOrCreate))
                {
                    using (GZipStream gZipStream = new GZipStream(outFile, CompressionMode.Compress))
                    {
                        dbMemoryStream.Position = 0;
                        dbMemoryStream.CopyTo(gZipStream);
                    }
                }
            }
        }

        public void SaveMachines(List<Machine> machines)
        {

            using (database.BeginTrans())
            {
                machines.ForEach(x => machinesCollection.Insert(x));
            }
        }

        public void UpdateMachines(List<Machine> machines)
        {

            using (database.BeginTrans())
            {
                machines.ForEach(x => machinesCollection.Update(x));
            }
        }

        public List<Machine> GetMachines()
        {
            var machineCol = database.GetCollection<Machine>("machines");            
            return machineCol.FindAll().ToList();
        }

        public Machine GetMachineByName(string name)
        {
            return machinesCollection.FindOne(m => m.name == name);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
