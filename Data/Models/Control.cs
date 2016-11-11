using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IV_Play.Data.Models
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
