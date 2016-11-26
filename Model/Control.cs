using System.Xml.Serialization;

namespace IV_Play.Model
{
    public class Control
    {
        [XmlAttribute]
        public string type { get; set; }

        [XmlAttribute]
        public short buttons { get; set; }

        [XmlAttribute]
        public short player { get; set; }
    }
}
