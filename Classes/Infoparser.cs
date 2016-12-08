using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IV_Play
{
    public class InfoParser
    {
        public Info this[string game]
        {
            get { return _infoDictionary.ContainsKey(game) ? _infoDictionary[game] : new Info(); }
        }

        private Dictionary<string, Info> _infoDictionary = new Dictionary<string, Info>();

        public bool Contains(string game)
        {
            return _infoDictionary.ContainsKey(game);
        }

        public InfoParser(string datPath)
        {
            if (!File.Exists(datPath))
                return;

            try
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(datPath)))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    string[] keys = new string[0];
                    while (true)
                    {
                        string line = stringReader.ReadLine();

                        if (line == null)
                            return;

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
                                if (!_infoDictionary.ContainsKey(key))
                                    _infoDictionary.Add(key, CreateInfo(entry));
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
            }
            catch (Exception)
            {


            }
        }

        private Info CreateInfo(string text)
        {
            Info info = new Info();
            info.Text = text;

            //int count = 1;
            //int start = 0;
            //while ((start = text.IndexOf('\n', start)) != -1)
            //{
            //    count++;
            //    start++;
            //}
            //info.Rows = count;
            return info;
        }
    }
}
