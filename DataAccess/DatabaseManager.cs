using IVPlay.Model;
using IVPlay.Properties;
using LiteDB;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace IVPlay.DataAccess
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

            if (File.Exists(Resources.Database_File_Name))
            {
                using (FileStream infileStream = File.Open(Resources.Database_File_Name, FileMode.OpenOrCreate))
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
            using (FileStream outFile = File.Open(Resources.Database_File_Name, FileMode.OpenOrCreate))
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

        public static void UpdateMachines(Dictionary<string, Machine> machines)
        {

            using (database.BeginTrans())
            {                
                foreach (var m in machines.Values)
                {
                    machinesCollection.Update(m);
                }
            }
        }

        public static IEnumerable<Machine> GetMachines()
        {
            return machinesCollection.FindAll();
        }

        public static Machine GetMachineByName(string name)
        {
            return machinesCollection.FindOne(m => m.Name == name);
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
