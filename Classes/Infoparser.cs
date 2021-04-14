using System.Collections.Generic;
using IV_Play.Classes;

namespace IV_Play
{
  public class InfoParser
  {
    protected Dictionary<string, string> _infoDictionary = new Dictionary<string, string>();

    public string this[string game]
    {
      get { return _infoDictionary.ContainsKey(game) ? _infoDictionary[game] : ""; }
    }

    public bool Contains(string game)
    {
      return _infoDictionary.ContainsKey(game);
    }

    protected InfoParser()
    {
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
