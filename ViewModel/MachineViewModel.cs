using IVPlay.DataAccess;
using IVPlay.Model;
using IVPlay.Properties;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace IVPlay.ViewModel
{
    public class MachineViewModel : ViewModelBase
    {
        readonly Machine _machine;
        bool _isSelected;
        bool _isFocused;

        public MachineViewModel(Machine machine)
        {
            if (machine == null)
                throw new ArgumentNullException("machine");
            _machine = machine;
        }

        public int Id
        {
            get { return _machine.Id; }
        }

        public string Name
        {
            get { return _machine.Name; }
            set {
                _machine.Name = value;

                base.OnPropertyChanged("Name");
            }
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get { return _isFavorite; }
            set
            {
                _isFavorite = value;

                base.OnPropertyChanged("IsFavorite");
            }
        }

        public FontFamily Font
        {
            get { return Settings.Default.game_list_font; }
        }
        
        public Brush TextForeground {
            get
            {                
                if (IsFavorite)
                    return new SolidColorBrush(Settings.Default.favorites_color);

                if (IsMechanical)
                    return new SolidColorBrush(Colors.Brown);

                if (!IsWorking)
                    return new SolidColorBrush(Colors.Red);

                if (CloneOf == null)
                    return new SolidColorBrush(Settings.Default.game_list_clone_color);

                return new SolidColorBrush(Settings.Default.game_list_color);
            }            
        }

        public bool IsMechanical
        {
            get { return _machine.IsMechanical == "yes"; }
            set
            {
                _machine.IsMechanical = value == true ? "yes" : "no";

                base.OnPropertyChanged("IsMechanical");
            }
        }

        public string Description
        {
            get {
                return _machine.Description;
            }
            set
            {
                _machine.Description = value;
                base.OnPropertyChanged("Description");
            }
        }

        public string Year
        {
            get { return _machine.Year; }
            set
            {
                _machine.Year = value;

                base.OnPropertyChanged("Year");
            }
        }

        public string Manufacturer
        {
            get { return _machine.Manufacturer; }
            set
            {
                _machine.Manufacturer = value;

                base.OnPropertyChanged("Manufacturer");
            }
        }

        public string CloneOf
        {
            get { return _machine.CloneOf; }
            set
            {
                _machine.CloneOf = value;

                base.OnPropertyChanged("CloneOf");
            }
        }

        public string SourceFile
        {
            get { return _machine.Sourcefile; }
            set
            {
                _machine.Sourcefile = value;

                base.OnPropertyChanged("SourceFile");
            }
        }

        public Thickness Margin
        {
            get {
                if (_machine.CloneOf == null || IsFavorite) return new Thickness();

                return new Thickness(20, 0, 40, 0);
            }
        }

        public string Icon
        {
            get
            {
                var parentPath = Path.Combine(Settings.Default.icons_directory, string.Format("{0}.ico", _machine.Name));
                
                if (File.Exists(string.Format(parentPath)))
                    return parentPath;

                var clonePath = Path.Combine(Settings.Default.icons_directory, string.Format("{0}.ico", _machine.CloneOf));
                if (_machine.CloneOf != null && File.Exists(clonePath))
                    return clonePath;

                return Path.Combine(Settings.Default.icons_directory, string.Format("unknown.ico"));
            }
        }
        public string Snap
        {
            get
            {
                if (!IsSelected) return "";

                var snapPath = Settings.Default.art_view_folders.Split('|')[Settings.Default.art_type];
                
                if (snapPath.EndsWith(".dat"))
                {                    
                    return InfoParser.Instance.GetInfo(snapPath, _machine.Name);
                }
                else
                {
                    return Path.Combine(snapPath, string.Format("{0}.png", _machine.Name));
                }

                //var parentPath = Path.Combine(Settings.Default.art_view_folders.Split('|')[0], string.Format("{0}.png", _machine.Name));

                //var video = Path.Combine(Settings.Default.art_view_folders.Split('|')[0].Replace("snap","videosnaps"), string.Format("{0}.mp4", _machine.Name));

                //if (File.Exists(video))
                //    return video;

                //if (File.Exists(parentPath))
                //    return parentPath;

                //var clonePath = Path.Combine(Settings.Default.art_view_folders.Split('|')[0], string.Format("{0}.png", _machine.CloneOf));
                //if (_machine.CloneOf != null && File.Exists(clonePath))
                //    return clonePath;


                //return Path.Combine(Settings.Default.art_view_folders.Split('|')[0], string.Format("unknown.ico"));
            }
        }     

        public string Driver
        {
            get { return _machine.Driver.ToString(); }
        }

        public bool IsWorking
        {
            get { return _machine.Driver != null ? _machine.Driver.Emulation == "good" : true; }
            set
            {
                if (_machine.Driver == null) _machine.Driver = new Driver();

                if (value) _machine.Driver.Emulation = "good";
                else _machine.Driver.Emulation = "notgood!";

                base.OnPropertyChanged("IsWorking");
            }
        }

        public string CPU
        {
            get
            {
                if (_machine.Chip == null) return string.Empty;
                return GetStringFromArray(_machine.Chip.Where(x => x.Type == "cpu").ToArray());
            }
        }

        public string Roms
        {
            get
            {
                var roms = GetStringFromArray(_machine.Rom);
                var disks = GetStringFromArray(_machine.Disk);

                return roms + "\r\n" + disks;
            }
        }

        public string Display
        {
            get { return GetStringFromArray(_machine.Display); }
        }

        public string Input
        {
            get { return _machine.Input.ToString(); }
        }

        public string Sound
        {
            get
            {
                if (_machine.Chip == null || _machine.Sound == null) return string.Empty;
                var soundString = GetStringFromArray(_machine.Chip.Where(x => x.Type == "audio").ToArray());
                return string.Format("{0} Channel(s)\r\n{1}", _machine.Sound.channels, soundString);
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

        public string Label {
            get
            {
                string text = "";

                if (Settings.Default.GameListManufacturer && Settings.Default.GameListYear)
                    text = "{0} {1} {2}";
                else if (!Settings.Default.GameListManufacturer && Settings.Default.GameListYear)
                    text = "{0} {1}";
                else if (Settings.Default.GameListManufacturer && !Settings.Default.GameListYear)
                    text = "{0} {2}";
                else
                    text = "{0}";
                
                return string.Format(text, _machine.Description, _machine.Year, _machine.Manufacturer);
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected)
                    return;

                _isSelected = value;

                base.OnPropertyChanged("IsSelected");
            }
        }

        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                if (value == _isFocused)
                    return;

                _isFocused = value;

                base.OnPropertyChanged("IsFocused");
            }
        }

        public MachineViewModel Copy()
        {
            return new MachineViewModel(_machine);
        }
    }
}
