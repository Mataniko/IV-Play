using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IV_Play.Data.Models
{
    public class Chip
    {
        [XmlAttribute]
        public string type { get; set; }
        [XmlAttribute]
        public float clock { get; set; }

        [XmlAttribute]
        public string name { get; set; }

        public override string ToString()
        {
            if (this.clock != 0)
            {
                switch (type)
                {
                    case "cpu":
                        return string.Format("{0} {1} MHz", name, clock / 1000000);
                    case "audio":
                        return string.Format("{0} {1} kHz", name, clock / 1000);
                }
            }

            return name;
        }
    }


}
