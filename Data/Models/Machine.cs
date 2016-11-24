using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IV_Play.Data.Models
{
    public class Machine
    {
        [BsonId]
        public int Id { get; set; }

        [XmlAttribute]
        [BsonIndex(unique: true)]        
        public string name { get; set; }
        [XmlAttribute]
        public string sourcefile { get; set; }
        [XmlAttribute]
        [BsonIgnore]
        public string isbios { get; set; } = "no";
        [XmlAttribute]
        [BsonIgnore]
        public string isdevice { get; set; } = "no";
        [XmlAttribute]
        public string ismechanical { get; set; } = "no";
        [XmlAttribute]
        [BsonIgnore]
        public string runnable { get; set; } = "yes";
        [XmlAttribute]
        public string cloneof { get; set; }
        [XmlAttribute]
        public string romof { get; set; }

        [XmlAttribute]
        [BsonIgnore]
        public string sampleof { get; set; }

        public string year { get; set; }
        public string manufacturer { get; set; }
        [BsonIgnore]
        public Input input { get; set; }

        public Driver driver { get; set; }

        [BsonIgnore]
        public Sound sound { get; set; }

        [XmlElement]
        [BsonIgnore]
        public Rom[] rom { get; set; }
        [XmlElement]
        [BsonIgnore]
        public Disk[] disk { get; set; }
        [XmlElement]
        [BsonIgnore]
        public Chip[] chip { get; set; }
        [XmlElement]
        [BsonIgnore]
        public Display[] display { get; set; }

        private string _description;
        public string description
        {
            get
            {
                var descriptionMatch = Regex.Match(_description, @"^(?<opening>(?:the|a|an))\s(?<content>[^\(]*)\s(?<info>\(.*)$", RegexOptions.IgnoreCase);

                if (!descriptionMatch.Success)
                    return _description.TrimStart('\'');

                return string.Format("{0}, {1} {2}", descriptionMatch.Groups[2], descriptionMatch.Groups[1], descriptionMatch.Groups[3]).TrimStart('\'');
            }
            set
            {
                _description = value;
            }
        }
        
        public string driverinfo
        {
            get
            {
                return driver.ToString();
            }
            set
            {
                return;
            }
        }

        [BsonIgnore]
        public string cpuinfo
        {
            get
            {
                if (chip == null) return string.Empty;
                return GetStringFromArray(chip.Where(x => x.type == "cpu").ToArray());
            }
            set
            {
                return;
            }
        }

        [BsonIgnore]
        public string rominfo
        {
            get
            {
                var roms = GetStringFromArray(rom);
                var disks = GetStringFromArray(disk);

                return roms + "\r\n" + disks;
            }
            set
            {
                return;
            }
        }

        [BsonIgnore]
        public string displayinfo
        {
            get
            {
                return GetStringFromArray(display);
            }
            set
            {
                return;
            }
        }

        [BsonIgnore]
        public string soundinfo { get
            {
                if (chip == null || sound == null) return string.Empty;
                var soundString = GetStringFromArray(chip.Where(x => x.type == "audio").ToArray());
                return string.Format("{0} Channel(s)\r\n{1}", sound.channels, soundString);
            }
            set
            {
                return;
            }
        }

        private string GetStringFromArray(object[] array)
        {
            if (array == null) return string.Empty;

            var sb = new StringBuilder();

            foreach (var obj in array)
            {
                sb.AppendLine(obj.ToString());
            }

            return sb.ToString();
        }

        [BsonIgnore]
        public string icon
        {
            get
            {
                if (File.Exists(string.Format(@"D:\Games\Emulators\MAME\icons\{0}.ico", this.name)))
                    return string.Format(@"D:\Games\Emulators\MAME\icons\{0}.ico", this.name);

                if (this.cloneof != null && File.Exists(string.Format(@"D:\Games\Emulators\MAME\icons\{0}.ico", this.cloneof)))
                    return string.Format(@"D:\Games\Emulators\MAME\icons\{0}.ico", this.cloneof);

                return @"D:\Games\Emulators\MAME\icons\unknown.ico";
            }
        }

        [BsonIgnore]
        public string ListText
        {
            get
            {         
                return string.Format("{0} {1} {2}", description, year, manufacturer).Trim();
            }

        }
    }
}


