#region

using IV_Play.Data.Models;
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
        }

        public Game(Machine machine)
        {
            Children = new SortedList<string, Game>();
            IsFavorite = false;
            CloneOf = string.IsNullOrEmpty(machine.cloneof) ? machine.name : machine.cloneof;
            CPU = machine.cpuinfo();
            Description = machine.description;
            SourceFile = machine.sourcefile;
            Name = machine.name;
            Manufacturer = machine.manufacturer;
            ParentSet = machine.cloneof;
            Screen = machine.displayinfo();
            Sound = machine.soundinfo();
            Working = machine.driver != null ? machine.driver.emulation == "good" : true;
            Year = machine.year;
            IconPath = Settings.Default.icons_directory + machine.name + ".ico";
            Driver = machine.driver != null ? machine.driver.ToString() : null;
            Input = machine.input != null ? machine.input.ToString() : null;
            Display = machine.displayinfo();
            //Colors = colors, Doesn't exist anymore?
            Roms = machine.rominfo();
            IsMechanical = machine.ismechanical == "yes";
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

        public Bitmap Icon { get; set; }

        public bool IsParent
        {
            get { return string.IsNullOrEmpty(ParentSet); }
        }

        public Game Copy()
        {
            return (Game) MemberwiseClone();
        }

        public override string ToString()
        {
            return Name;
        }
    }


 
}