using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IV_Play.Data.Models
{
    public class Input
    {
        [XmlAttribute]
        public short players { get; set; }        
        [XmlElement]
        public Control[] control { get; set; }

        public override string ToString()
        {
            if (control == null || control.Length == 0)
            {
                return string.Format("{0} Player(s)", players);
            }

            return string.Format("{0} Player(s) {1} Button(s) {2}", players, control[0].buttons, control[0].type);
        }
    }
}
