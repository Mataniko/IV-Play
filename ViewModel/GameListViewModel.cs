using IV_Play.DataAccess;
using IV_Play.Model;
using IV_Play.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IV_Play.ViewModel
{
    class GameListViewModel : ViewModelBase
    {        
        private MameInfo _mameInfo;
        public MachineViewModel CurrentMachine { get; private set; }

        public ObservableCollection<MachineViewModel> Machines { get; private set; }

        public GameListViewModel()
        {
            LoadMachines();
        }

        private async void LoadMachines()
        {
            Console.WriteLine(Settings.Default.MAME_EXE);
            if (!File.Exists(Properties.Resources.DB_NAME) && !string.IsNullOrEmpty(Settings.Default.MAME_EXE))
            {
                //var xmlParser = new XmlParser();

                //xmlParser.MakeQuickDat();
                //this.Machines = new ObservableCollection<MachineViewModel>(DatabaseManager.GetMachines());
                //this.Machines.CollectionChanged += Machines_CollectionChanged;

                //this._mameInfo = xmlParser.MameInfo;
                //SettingsManager.MameCommands = _mameInfo.Commands;
                //var progress = new Progress<int>();
                //await Task.Factory.StartNew(() => xmlParser.MakeDat(progress));
                //this.Machines = new ObservableCollection<MachineViewModel>(DatabaseManager.GetMachines().Where(x => x.ismechanical == "no"));
                //this.Machines.CollectionChanged += Machines_CollectionChanged;
            }
            else
            {
                this._mameInfo = DatabaseManager.GetMameInfo();                                
                var machineCollection = (from machine in DatabaseManager.GetMachines().Where(x => x.ismechanical == "no") select new MachineViewModel(machine));                

                foreach (var machine in machineCollection)                
                    machine.PropertyChanged += this.Machine_PropertyChanged;

                this.Machines = new ObservableCollection<MachineViewModel>(machineCollection);
                this.Machines.CollectionChanged += this.Machines_CollectionChanged;
            }
        }

        private RelayCommand _propertiesCommand;
        public ICommand GetProperties
        {
            get
            {
                if (_propertiesCommand == null)
                {
                    _propertiesCommand = new RelayCommand(
                        param => this.OpenPropertiesForm(),
                        param => true
                        );
                }
                return _propertiesCommand;
            }
        }

        private void OpenPropertiesForm()
        {
            var props = new IV_Play.View.Properties(CurrentMachine);
            props.ShowDialog();
        }

        private void Machine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string IsSelected = "IsSelected";

            // Make sure that the property name we're referencing is valid.
            // This is a debugging technique, and does not execute in a Release build.
            (sender as MachineViewModel).VerifyPropertyName(IsSelected);

            // When a customer is selected or unselected, we must let the
            // world know that the TotalSelectedSales property has changed,
            // so that it will be queried again for a new value.
            if (e.PropertyName == IsSelected)
            {
                this.OnPropertyChanged("MachineSelected");
                CurrentMachine = sender as MachineViewModel;
            }
                
        }

        private void Machines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (MachineViewModel machineVM in e.NewItems)
                    machineVM.PropertyChanged += this.Machine_PropertyChanged;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (MachineViewModel machineVM in e.OldItems)
                    machineVM.PropertyChanged -= this.Machine_PropertyChanged;
        }
    }
}
