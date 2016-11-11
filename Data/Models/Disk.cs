using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IV_Play.Data.Models
{
    public class Disk
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string region { get; set; }
        [XmlAttribute]
        public string status { get; set; } = "good";

        public override string ToString()
        {
            return string.Format("{0,8} {1,7} {2}", region, status, name);
        }
    }
}