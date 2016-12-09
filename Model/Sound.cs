using System.Xml.Serialization;

namespace IVPlay.Model
{
    public class Sound
    {
        [XmlAttribute]
        public string channels { get; set; }
    }
}
