using System.Xml.Serialization;

namespace IVPlay.Model
{
    public class Driver
    {
        [XmlAttribute("status")]
        public string Status { get; set; }
        [XmlAttribute("color")]
        public string Color { get; set; }
        [XmlAttribute("sound")]
        public string Sound { get; set; }
        [XmlAttribute("graphic")]
        public string Graphic { get; set; }
        [XmlAttribute("cocktail")]
        public string Cocktail { get; set; }
        [XmlAttribute("protection")]
        public string Protection { get; set; }
        [XmlAttribute("savestate")]
        public string Savestate { get; set; }
        [XmlAttribute("emulation")]
        public string Emulation { get; set; }

        public override string ToString()
        {
            var returnString = string.Format("Status={0}, Emulation={1}, Color={2}, Sound={3}, Graphic={4}, Savestate={5}",
                this.Status,
                this.Emulation,
                this.Color,
                this.Sound,
                this.Graphic,
                this.Savestate
                );
            if (!string.IsNullOrEmpty(this.Cocktail))
            {
                returnString += string.Format(" Cocktail={0}", this.Cocktail);
            }

            if (!string.IsNullOrEmpty(this.Protection))
            {
                returnString += string.Format(" Protection={0}", this.Protection);
            }

            return returnString;
        }
    }


}