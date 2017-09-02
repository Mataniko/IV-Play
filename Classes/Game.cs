#region

using IV_Play.Data.Models;
using IV_Play.Properties;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

#endregion

namespace IV_Play
{
    /// <summary>
    /// Instance of a MAME game
    /// </summary>
    [Serializable]
    public class Game : GameListItem
    {
        public Game()
        {
            Children = new SortedList<string, Game>();
            IsFavorite = false;
            ShowAsParent = false;
        }

        public Game(Machine machine)
        {
            Children = new SortedList<string, Game>();
            IsFavorite = false;
            CloneOf = string.IsNullOrEmpty(machine.cloneof) ? machine.name : machine.cloneof;
            CPU = ParseCpu(machine.chip);
            Description = machine.description;
            SourceFile = machine.sourcefile;
            Name = machine.name;
            Manufacturer = machine.manufacturer;
            ParentSet = machine.cloneof;
            Screen = ParseDisplay(machine.display);
            Sound = ParseSound(machine.chip, machine.sound);
            Working = machine.driver != null ? machine.driver.emulation == "good" : true;
            Year = machine.year;
            IconPath = Settings.Default.icons_directory + machine.name + ".ico";
            Driver = machine.driver != null ? machine.driver.ToString() : null;          
            Input = machine.input != null ? machine.input.ToString() : null;
            Features = ParseFeatures(machine.feature);
            Display = ParseDisplay(machine.display);
            //Colors = colors, Doesn't exist anymore?
            Roms = ParseRom(machine.rom, machine.disk);
            IsMechanical = machine.ismechanical == "yes";
            ShowAsParent = false;
        }

        public string Roms { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SourceFile { get; set; }
        public string ParentSet { get; set; }
        public string Manufacturer { get; set; }
        public string Year { get; set; }
        public string CPU { get; set; }
        public string Sound { get; set; }
        public string CloneOf { get; set; }
        public string Screen { get; set; }
        public bool Working { get; set; }
        public bool IsFavorite { get; set; }
        public SortedList<string, Game> Children { get; set; }
        public string IconPath { get; set; }
        public string Driver { get; set; }
        public string Display { get; set; }
        public string Input { get; set; }
        public string Colors { get; set; }
        public string History { get; set; }
        public string Info { get; set; }
        public string Features { get; set; }
        public bool HasOverlay { get; set; }
        public bool IsMechanical { get; set; }
        public bool ShowAsParent { get; set; }

        public Bitmap Icon { get; set; }

        public bool IsParent
        {
            get { return string.IsNullOrEmpty(ParentSet); }
        }

        public Game Copy()
        {
            return (Game)MemberwiseClone();
        }

        public override string ToString()
        {
            return Name;
        }

        private string ParseFeatures(Feature[] feature)
        {
            if (feature == null) return "";

            var sb = new StringBuilder();            
            foreach (var obj in feature)
            {
                var status = "";
                if (!String.IsNullOrEmpty(obj.status) && !String.IsNullOrEmpty(obj.overall))
                    status = string.Format("{0};{1}", obj.status, obj.overall);
                else if (String.IsNullOrEmpty(obj.status))
                    status = obj.overall;
                else
                    status = obj.status;
                sb.Append(string.Format("{0}={1}, ", System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(obj.type), status));
            }

            return sb.ToString().TrimEnd(new char[2] { ',', ' '});
        }
        private string ParseCpu(Chip[] chip)
        {
            if (chip == null) return string.Empty;
            return GetStringFromArray(chip.Where(x => x.type == "cpu").ToArray());
        }

        private string ParseRom(Rom[] rom, Disk[] disk)
        {
            var roms = GetStringFromArray(rom);
            var disks = GetStringFromArray(disk);

            return roms + "\r\n" + disks;
        }

        private string ParseDisplay(Display[] display)
        {
            return GetStringFromArray(display);
        }

        private string ParseSound(Chip[] chip, Sound sound)
        {
            if (chip == null || sound == null) return string.Empty;
            var soundString = GetStringFromArray(chip.Where(x => x.type == "audio").ToArray());
            return string.Format("{0} Channel(s)\r\n{1}", sound.channels, soundString);
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
    }



}