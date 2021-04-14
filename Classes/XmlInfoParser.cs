using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace IV_Play.Classes
{
  class XmlInfoParser : InfoParser
  {
    public XmlInfoParser(string datPath)
    {

      if (!File.Exists(datPath))
        return;

      try
      {
        using (var fileReader = File.OpenRead(datPath))
        {
          using (XmlReader xmlReader = XmlReader.Create(fileReader))
          {
            StringBuilder stringBuilder = new StringBuilder();
            List<string> keys = new List<string>();
            while (xmlReader.ReadToFollowing("entry"))
            {
              var innerReader = xmlReader.ReadSubtree();

              if (innerReader.ReadToDescendant("systems"))
              {
                var systemsReader = innerReader.ReadSubtree();

                keys.Clear();
                while (systemsReader.ReadToFollowing("system"))
                {
                  keys.Add(systemsReader.GetAttribute("name"));
                }

                if (innerReader.ReadToFollowing("text"))
                {
                  foreach (var key in keys)
                  {
                    if (!_infoDictionary.ContainsKey(key))
                      _infoDictionary.Add(key, innerReader.ReadElementContentAsString());
                  }
                }
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
