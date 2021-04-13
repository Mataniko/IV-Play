using System.Collections.Generic;
using IV_Play.Classes;

namespace IV_Play
{
  public class InfoParser
  {
    protected Dictionary<string, Info> _infoDictionary = new Dictionary<string, Info>();

    public Info this[string game]
    {
      get { return _infoDictionary.ContainsKey(game) ? _infoDictionary[game] : new Info(); }
    }

    public bool Contains(string game)
    {
      return _infoDictionary.ContainsKey(game);
    }

    protected InfoParser()
    {
    }

    protected Info CreateInfo(string text)
    {
      Info info = new Info
      {
        Text = text
      };

      return info;
    }

    public static InfoParser Create(string datFile)
    {
      if (datFile.Contains(".xml"))
      {
        return new XmlInfoParser(datFile);
      }
      else
      {
        return new DatInfoParser(datFile);
      }

    }
  }
}
