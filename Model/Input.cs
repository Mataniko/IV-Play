using System.Xml.Serialization;

namespace IVPlay.Model
{
    public class Input
    {
        [XmlAttribute("players")]
        public short Players { get; set; }
        [XmlElement("control")]
        public Control[] Control { get; set; }

        public override string ToString()
        {
            if (Control == null || Control.Length == 0)
            {
                return string.Format("{0} Player(s)", Players);
            }

            return string.Format("{0} Player(s) {1} Button(s) {2}", Players, Control[0].Buttons, Control[0].Type);
        }
    }
}
