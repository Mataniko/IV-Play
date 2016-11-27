using IV_Play.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IV_Play.ViewModel
{
    public class MachineViewModel : ViewModelBase
    {
        readonly Machine _machine;
        bool _isSelected;

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
            get { return _machine.description; }
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
                    return @"D:\Games\Emulators\MAME\snap\unknown.png";
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
    }
}
