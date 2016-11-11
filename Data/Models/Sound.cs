using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IV_Play.Data.Models
{
    public class Sound
    {
        [XmlAttribute]
        public string channels { get; set; }
    }
}
