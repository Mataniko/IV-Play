using IV_Play.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IV_Play.ViewModel
{
    class MachineViewModel : ViewModelBase
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
