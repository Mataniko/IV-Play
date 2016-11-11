using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IV_Play.Data.Models
{
    public class Rom
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string region { get; set; }
        [XmlAttribute]
        public string status { get; set; } = "good";
        [XmlAttribute]
        public string size { get; set; }

        public override string ToString()
        {
            return string.Format("{0,-8} {1,-8} {2, -16}", region, status, name);
        }
    }
}