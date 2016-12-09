using System.Xml.Serialization;

namespace IV_Play.Model
{
    public class Control
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("buttons")]
        public short Buttons { get; set; }

        [XmlAttribute("player")]
        public short Player { get; set; }
    }
}
