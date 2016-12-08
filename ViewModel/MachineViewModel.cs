using IV_Play.Model;
using IV_Play.Properties;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace IV_Play.ViewModel
{
    public class MachineViewModel : ViewModelBase, IEditableObject
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

        public bool IsMechanical
        {
            get { return _machine.ismechanical == "yes"; }
            set
            {
                _machine.ismechanical = value == true ? "yes" : "no";

                base.OnPropertyChanged("IsMechanical");
            }
        }

        public string Description
        {
            get {
                return _machine.description;
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
                var s = typeof(Colors).GetProperties();
                

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

        public bool IsWorking
        {
            get { return _machine.driver != null ? _machine.driver.emulation == "good" : true; }
            set
            {
                if (_machine.driver == null) _machine.driver = new Driver();

                if (value) _machine.driver.emulation = "good";
                else _machine.driver.emulation = "notgood!";

                base.OnPropertyChanged("IsWorking");
            }
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
                
                return string.Format(text, _machine.description, _machine.year, _machine.manufacturer);
            }
            set { }
        }

        public void BeginEdit()
        {
           // throw new NotImplementedException();
        }

        public void EndEdit()
        {
            //throw new NotImplementedException();
        }

        public void CancelEdit()
        {
           // throw new NotImplementedException();
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
