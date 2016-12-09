#region

using IV_Play.Model;
using IV_Play.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;

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
            CloneOf = string.IsNullOrEmpty(machine.CloneOf) ? machine.Name : machine.CloneOf;
            //CPU = machine.cpuinfo;
            Description = machine.Description;
            SourceFile = machine.Sourcefile;
            Name = machine.Name;
            Manufacturer = machine.Manufacturer;
            ParentSet = machine.CloneOf;
            //Screen = machine.displayinfo;
            //Sound = machine.soundinfo;
            Working = machine.Driver != null ? machine.Driver.Emulation == "good" : true;
            Year = machine.Year;
            IconPath = Settings.Default.icons_directory + machine.Name + ".ico";
            Driver = machine.Driver != null ? machine.Driver.ToString() : null;
            Input = machine.Input != null ? machine.Input.ToString() : null;
            //Display = machine.displayinfo;
            //Colors = colors, Doesn't exist anymore?
            //Roms = machine.rominfo;
            IsMechanical = machine.IsMechanical == "yes";
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
    }



}