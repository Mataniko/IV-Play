using IV_Play.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IV_Play.ViewModel
{
    public class MachineViewModel : ViewModelBase
    {
        readonly Machine _machine;
        bool _isSelected;
        bool _isFocused;

        public MachineViewModel(Machine machine)
        {
            if (machine == null)
                throw new ArgumentNullException("Can't create MachineViewModel");            
            _machine = machine;
        }

        public string Name
        {
            get { return _machine.name; }
            set {
                _machine.name = value;

                base.OnPropertyChanged("Name");
            }
        }

        public string Description
        {
            get {
                var descriptionMatch = Regex.Match(_machine.description, @"^(?<opening>(?:the|a|an))\s(?<content>[^\(]*)\s(?<info>\(.*)$", RegexOptions.IgnoreCase);

                if (!descriptionMatch.Success)
                    return _machine.description.TrimStart('\'');

                return string.Format("{0}, {1} {2}", descriptionMatch.Groups[2], descriptionMatch.Groups[1], descriptionMatch.Groups[3]).TrimStart('\'');
            }
            set
            {
                _machine.description = value;
                base.OnPropertyChanged("Description");
            }
        }

        public string Year
        {
            get { return _machine.year; }
            set
            {
                _machine.year = value;

                base.OnPropertyChanged("Year");
            }
        }

        public string Manufacturer
        {
            get { return _machine.manufacturer; }
            set
            {
                _machine.manufacturer = value;

                base.OnPropertyChanged("Manufacturer");
            }
        }

        public string CloneOf
        {
            get { return _machine.cloneof; }
            set
            {
                _machine.cloneof = value;

                base.OnPropertyChanged("CloneOf");
            }
        }

        public string SourceFile
        {
            get { return _machine.sourcefile; }
            set
            {
                _machine.sourcefile = value;

                base.OnPropertyChanged("SourceFile");
            }
        }

        public Thickness Margin
        {
            get {
                if (_machine.cloneof == null) return new Thickness();

                return new Thickness(20, 0, 40, 0);
            }
            set { }
        }

        public string Icon
        {
            get
            {
                if (File.Exists(string.Format(@"D:\Games\Emulators\MAME\icons\{0}.ico", _machine.name)))
                    return string.Format(@"D:\Games\Emulators\MAME\icons\{0}.ico", _machine.name);

                if (_machine.cloneof != null && File.Exists(string.Format(@"D:\Games\Emulators\MAME\icons\{0}.ico", _machine.cloneof)))
                    return string.Format(@"D:\Games\Emulators\MAME\icons\{0}.ico", _machine.cloneof);

                return @"D:\Games\Emulators\MAME\icons\unknown.ico";
            }
        }
        public string Snap
        {
            get
            {
                if (!IsSelected) return "";
                if (File.Exists(string.Format(@"D:\Games\Emulators\MAME\snap\{0}.png", _machine.name)))
                    return string.Format(@"D:\Games\Emulators\MAME\snap\{0}.png", _machine.name);
                else if (_machine.cloneof != null && File.Exists(string.Format(@"D:\Games\Emulators\MAME\snap\{0}.png", _machine.cloneof)))
                    return string.Format(@"D:\Games\Emulators\MAME\snap\{0}.png", _machine.cloneof);
                else
                    return @"D:\Games\Emulators\MAME\snap\005.png";
            }
        }     

        public string Driver
        {
            get
            {
                return _machine.driver.ToString();
            }
            set { }
        }

        public string CPU
        {
            get
            {
                if (_machine.chip == null) return string.Empty;
                return GetStringFromArray(_machine.chip.Where(x => x.type == "cpu").ToArray());
            }
            set { }
        }

        public string Roms
        {
            get
            {
                var roms = GetStringFromArray(_machine.rom);
                var disks = GetStringFromArray(_machine.disk);

                return roms + "\r\n" + disks;
            }
            set { }
        }

        public string Display
        {
            get
            {
                return GetStringFromArray(_machine.display);
            }
            set { }
        }

        public string Input
        {
            get
            {
                return _machine.input.ToString();
            }
            set { }
        }

        public string Sound
        {
            get
            {
                if (_machine.chip == null || _machine.sound == null) return string.Empty;
                var soundString = GetStringFromArray(_machine.chip.Where(x => x.type == "audio").ToArray());
                return string.Format("{0} Channel(s)\r\n{1}", _machine.sound.channels, soundString);
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
    }
}
