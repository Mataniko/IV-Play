using LiteDB;
using System.Text.RegularExpressions;

using System.Xml.Serialization;

namespace IVPlay.Model
{
    public class Machine
    {
        [BsonId]
        public int Id { get; set; }

        [XmlAttribute("name")]
        [BsonIndex(unique: true)]        
        public string Name { get; set; }
        [XmlAttribute("sourcefile")]
        public string Sourcefile { get; set; }
        [XmlAttribute("isbios")]
        [BsonIgnore]
        public string isbios { get; set; } = "no";
        [XmlAttribute("isdevice")]
        [BsonIgnore]
        public string IsDevice { get; set; } = "no";
        [XmlAttribute("ismechanical")]
        public string IsMechanical { get; set; } = "no";
        [XmlAttribute("Runnable")]
        [BsonIgnore]
        public string Runnable { get; set; } = "yes";
        [XmlAttribute("cloneof")]
        public string CloneOf { get; set; }
        [XmlAttribute("romof")]
        public string RomOf { get; set; }

        [XmlAttribute("sampleof")]
        [BsonIgnore]
        public string SampleOf { get; set; }

        [XmlElement("year")]
        public string Year { get; set; }
        [XmlElement("manufacturer")]
        public string Manufacturer { get; set; }

        [BsonIgnore]
        [XmlElement("input")]
        public Input Input { get; set; }

        [XmlElement("driver")]
        public Driver Driver { get; set; }

        [BsonIgnore]
        [XmlElement("sound")]
        public Sound Sound { get; set; }

        [XmlElement("rom")]
        [BsonIgnore]
        public Rom[] Rom { get; set; }
        [XmlElement("disk")]
        [BsonIgnore]
        public Disk[] Disk { get; set; }
        [XmlElement("chip")]
        [BsonIgnore]
        public Chip[] Chip { get; set; }
        [XmlElement("display")]
        [BsonIgnore]
        public Display[] Display { get; set; }

        private string _description;
        [XmlElement("description")]
        public string Description
        {
            get
            {
                var descriptionMatch = Regex.Match(_description, @"^(?<opening>(?:the|a|an))\s(?<content>[^\(]*)\s(?<info>\(.*)$", RegexOptions.IgnoreCase);

                if (!descriptionMatch.Success)
                    return _description.TrimStart('\'');

                return string.Format("{0}, {1} {2}", descriptionMatch.Groups[2], descriptionMatch.Groups[1], descriptionMatch.Groups[3]).TrimStart('\'');
            }
            set
            {
                _description = value;
            }
        }

    }
}


