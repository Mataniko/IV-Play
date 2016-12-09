using System.Xml.Serialization;

namespace IV_Play.Model
{
    public class Disk
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("region")]
        public string Region { get; set; }
        [XmlAttribute("status")]
        public string Status { get; set; } = "good";

        public override string ToString()
        {
            return string.Format("{0,8} {1,7} {2}", Region, Status, Name);
        }
    }
}