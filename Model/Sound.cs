using System.Xml.Serialization;

namespace IV_Play.Model
{
    public class Sound
    {
        [XmlAttribute]
        public string channels { get; set; }
    }
}
