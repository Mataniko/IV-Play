using System.Xml.Serialization;

namespace IV_Play.Data.Models
{
  public class Feature
  {
    [XmlAttribute]
    public string type { get; set; }

    [XmlAttribute]
    public string status { get; set; }

    [XmlAttribute]
    public string overall { get; set; }
  }
}
