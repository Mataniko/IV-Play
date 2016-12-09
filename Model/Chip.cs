using System.Xml.Serialization;

namespace IV_Play.Model
{
    public class Chip
    {
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("clock")]
        public float Clock { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        public override string ToString()
        {
            if (this.Clock != 0)
            {
                switch (Type)
                {
                    case "cpu":
                        return string.Format("{0} {1} MHz", Name, Clock / 1000000);
                    case "audio":
                        return string.Format("{0} {1} kHz", Name, Clock / 1000);
                }
            }

            return Name;
        }
    }


}
