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
            machinesCollection = database.GetCollection<Machine>("machines");
            mameInfoCollection = database.GetCollection<MameInfo>("mameinfo");            
        }

        private static void Open()
        {
            dbMemoryStream = new MemoryStream();

            if (File.Exists(Resources.DB_NAME))
            {
                using (FileStream infileStream = File.Open(Resources.DB_NAME, FileMode.OpenOrCreate))
                {
                    using (GZipStream gZipStream = new GZipStream(infileStream, CompressionMode.Decompress))
                    {
                        gZipStream.CopyTo(dbMemoryStream);
                    }
                }
            }

            database = new LiteDatabase(dbMemoryStream);
        }

        public static void SaveToDisk()
        {
            using (FileStream outFile = File.Open(Resources.DB_NAME, FileMode.OpenOrCreate))
            {
                using (GZipStream gZipStream = new GZipStream(outFile, CompressionMode.Compress))
                {
                    dbMemoryStream.WriteTo(gZipStream);                    
                }
            }
        }

        public static void SaveMachines(Dictionary<string, Machine> machines)
        {
            machinesCollection.Delete(Query.All());
            using (database.BeginTrans())
            {
                foreach (var m in machines)
                    machinesCollection.Insert(m.Value);
            }
        }

        public static void UpdateMachines(List<Machine> machines)
        {

            using (database.BeginTrans())
            {
                machines.ForEach(x => machinesCollection.Update(x));
            }
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
