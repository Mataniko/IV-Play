using System.Xml.Serialization;

namespace IVPlay.Model
{
    public class Rom
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("region")]
        public string Region { get; set; }
        [XmlAttribute("status")]
        public string Status { get; set; } = "good";
        [XmlAttribute("size")]
        public string Size { get; set; }

        public override string ToString()
        {
            return string.Format("{0,-8} {1,-8} {2, -16}", Region, Status, Name);
        }
    }
}