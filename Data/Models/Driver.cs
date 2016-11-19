using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IV_Play.Data.Models
{
    public class Driver
    {
        [XmlAttribute]
        public string status { get; set; }
        [XmlAttribute]
        public string color { get; set; }
        [XmlAttribute]
        public string sound { get; set; }
        [XmlAttribute]
        public string graphic { get; set; }
        [XmlAttribute]
        public string cocktail { get; set; }
        [XmlAttribute]
        public string protection { get; set; }
        [XmlAttribute]
        public string savestate { get; set; }
        [XmlAttribute]
        public string emulation { get; set; }

        public override string ToString()
        {
            var returnString = string.Format("Status={0}, Emulation={1}, Color={2}, Sound={3}, Graphic={4}, Savestate={5}",
                this.status,
                this.emulation,
                this.color,
                this.sound,
                this.graphic,
                this.savestate
                );
            if (!string.IsNullOrEmpty(this.cocktail))
            {
                returnString += string.Format(" Cocktail={0}", this.cocktail);
            }

            if (!string.IsNullOrEmpty(this.protection))
            {
                returnString += string.Format(" Protection={0}", this.protection);
            }

            return returnString;
        }
    }


}