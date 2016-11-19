using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IV_Play.Data.Models
{
    public class Display
    {
        [XmlAttribute]
        public string refresh { get; set; }
        [XmlAttribute]
        public short rotate { get; set; }
        [XmlAttribute]
        public int width { get; set; }
        [XmlAttribute]
        public int height { get; set; }
        [XmlAttribute]
        public string type { get; set; }
        [XmlAttribute]
        public string rag { get; set; }

        public override string ToString()
        {
            var rotation = rotate == 0 || rotate == 180 ? 'H' : 'V';
            return string.Format("{0}x{1} ({2}) {3} Hz", width, height, rotation, refresh.TrimEnd('0').TrimEnd('.'));
        }
    }
}
