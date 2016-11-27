﻿using LiteDB;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.Xml.Serialization;

namespace IV_Play.Model
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
        public string description { get; set; }

    }
}


