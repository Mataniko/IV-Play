using System.Xml.Serialization;

namespace IVPlay.Model
{
    public class Display
    {
        [XmlAttribute("refresh")]
        public string Refresh { get; set; }
        [XmlAttribute("rotate")]
        public short Rotate { get; set; }
        [XmlAttribute("width")]
        public int Width { get; set; }
        [XmlAttribute("height")]
        public int Height { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("tag")]
        public string Tag { get; set; }

        public override string ToString()
        {
            var rotation = Rotate == 0 || Rotate == 180 ? 'H' : 'V';
            return string.Format("{0}x{1} ({2}) {3} Hz", Width, Height, rotation, Refresh.TrimEnd('0').TrimEnd('.'));
        }
    }
}
