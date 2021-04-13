using System.Xml.Serialization;

namespace IV_Play.Data.Models
{
  public class Sound
  {
    [XmlAttribute]
    public string channels { get; set; }
  }
}
