using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IVPlay.DataAccess
{
    public class InfoParser
    {
        private static InfoParser _instance;

        private Dictionary<string, Dictionary<string, string>> _infoDictionary = new Dictionary<string, Dictionary<string, string>>();

        private InfoParser() { }

        public static InfoParser Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new InfoParser();

                return _instance;
            }
        }

        public string GetInfo(string DatFile, string MachineName)
        {
            if (!_infoDictionary.ContainsKey(DatFile))
                _infoDictionary.Add(DatFile, ParseDat(DatFile));

            if (!_infoDictionary[DatFile].ContainsKey(MachineName)) return "";

            return _infoDictionary[DatFile][MachineName];
        }
        
        private Dictionary<string, string> ParseDat(string path)
        {
            if (!File.Exists(path))
                return new Dictionary<string, string>();

            try
            {

                var info = new Dictionary<string, string>();
                using (StringReader stringReader = new StringReader(File.ReadAllText(path)))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    string[] keys = new string[0];
                    while (true)
                    {
                        string line = stringReader.ReadLine();

                        if (line == null)
                            break;

                        if (line.StartsWith("#"))
                            continue;
                        else if (line.StartsWith("$info="))
                        {
                            stringBuilder = new StringBuilder();
                            keys = line.Split('=')[1].TrimEnd(',').Split(',');
                        }
                        else if (line.StartsWith("$end"))
                        {
                            string entry = stringBuilder.ToString().Replace("\r\n\r\n", "\r\n");
                            entry = entry.TrimStart('\r', '\n');
                            entry = entry.TrimEnd('\r', '\n');

                            foreach (var key in keys)
                            {
                                if (!info.ContainsKey(key))
                                    info.Add(key, entry);
                            }

                            continue;
                        }
                        else if (line.StartsWith("$"))
                            continue;
                        else
                        {
                            if (line.Length == 0)
                            {
                                stringBuilder.AppendLine("\r\n");
                            }
                            else
                            {
                                stringBuilder.AppendLine(line);
                            }
                        }
                    }
                }

                return info;
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();

            }
        }
    }
}
