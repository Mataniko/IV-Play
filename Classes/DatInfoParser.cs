using System;
using System.IO;
using System.Text;

namespace IV_Play.Classes
{
  class DatInfoParser : InfoParser
  {
    public DatInfoParser(string datPath)
    {

      if (!File.Exists(datPath))
        return;

      try
      {
        using (StringReader stringReader = new StringReader(File.ReadAllText(datPath)))
        {
          StringBuilder stringBuilder = new StringBuilder();
          string[] keys = new string[0];
          string line;
          while ((line = stringReader.ReadLine()) != null)
          {
            // Skip everything until our first $info block
            if (!line.Contains("$info"))
            {
              continue;
            }

            // Line is $info
            stringBuilder = new StringBuilder();
            keys = line.Substring(6).Split(',');

            // Next line must be either $bio or $mame
            line = stringReader.ReadLine();
            if (!line.Equals("$bio") && !line.Equals("$mame"))
            {
              continue;
            }

            // Skip the first empty line that only occurs in history but not mameinfo
            if (line.Equals("$bio"))
            {
              line = stringReader.ReadLine();
            }


            // Read to the end of the block
            while (!(line = stringReader.ReadLine()).Equals("$end"))
            {
              stringBuilder.AppendLine(line);
            }

            // We're done parsing the entry
            string entry = stringBuilder.ToString();

            foreach (var key in keys)
            {
              if (key.Length == 0)
              {
                continue;
              }

              if (!_infoDictionary.ContainsKey(key))
              {
                _infoDictionary.Add(key, entry);
              }
            }

          }
        }
      }
      catch (Exception)
      {

      }
    }
  }
}
