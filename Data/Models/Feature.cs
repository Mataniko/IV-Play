using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IV_Play.Data.Models
{
    public class Feature
    {
        [XmlAttribute]
        public string type { get; set; }

        [XmlAttribute]
        public string status { get; set; }

        [XmlAttribute]
        public string overall { get; set; }
    }
}
