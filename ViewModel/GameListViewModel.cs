using IV_Play.DataAccess;
using IV_Play.Model;
using IV_Play.Properties;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IV_Play.ViewModel
{
    partial class GameListViewModel : ViewModelBase
    {        
        private MameInfo _mameInfo;
        private MachineViewModel _machine;
        private readonly object _MachinesLock = new object();
        private CollectionView _view;
        private JumpListClass _jumpList;

        public MachineViewModel CurrentMachine
        {
            get
            {
                return _machine;
            }
            private set
            {
                if (_machine == value) return;
          
                _machine = value;
                base.OnPropertyChanged("CurrentMachine");
            }
        }

        private string _filter = "";
        public string Filter
        {
            get { return _filter; }
            set
            {                
                _filter = value;

                _view.Refresh();
                base.OnPropertyChanged("Filter");
            }
        }

        private string _title;
        
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;

                OnPropertyChanged("Title");
            }
        }
        public ObservableCollection<MachineViewModel> Machines { get; private set; }

        public GameListViewModel()
        {
            SettingsManager.GetBackgroundImage();
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;
            if (Settings.Default.MAME_EXE == "")
                SettingsManager.GetMamePath(true, true);

            _jumpList = new JumpListClass();
            LoadMachines();
            SettingsManager.GetBackgroundImage();
            
        }

        private async void LoadMachines()
        {            
            var machineCollection = (from machine in DatabaseManager.GetMachines() select new MachineViewModel(machine));

            if (!machineCollection.Any())
            {
                var xmlParser = new XmlParser();
                xmlParser.MakeQuickDat();
                machineCollection = (from machine in DatabaseManager.GetMachines().Where(x => x.ismechanical == "no") select new MachineViewModel(machine)).ToList();

                foreach (MachineViewModel machine in machineCollection)
                    machine.PropertyChanged += this.Machine_PropertyChanged;

                this.Machines = new ObservableCollection<MachineViewModel>(machineCollection);
                this.Machines.CollectionChanged += this.Machines_CollectionChanged;

                _view = (CollectionView)CollectionViewSource.GetDefaultView(this.Machines);
                _view.Filter = UserFilter;

                BindingOperations.EnableCollectionSynchronization(this.Machines, _MachinesLock);
                var progress = new Progress<int>();
                progress.ProgressChanged += Progress_ProgressChanged;
                await Task.Factory.StartNew(() => xmlParser.MakeDat(progress, this.Machines));
                _view.Refresh();
            } else
            {
                
                this.Machines = new ObservableCollection<MachineViewModel>(machineCollection);
                this.Machines.CollectionChanged += this.Machines_CollectionChanged;
                _view = (CollectionView)CollectionViewSource.GetDefaultView(this.Machines);
                _view.Filter = UserFilter;
            }
        }

        private void Progress_ProgressChanged(object sender, int e)
        {
            //if (e % 300 == 0)
            //    Application.Current.Dispatcher.BeginInvoke(new Action(() => _view.Refresh()));
        }

        private bool UserFilter(object item)
        {
            var mvm = (item as MachineViewModel);
            if (Settings.Default.hide_mechanical_games && mvm.IsMechanical) return false;
            if (Settings.Default.hide_nonworking && !mvm.IsWorking) return false;
            return mvm.Name.Contains(_filter, StringComparison.InvariantCultureIgnoreCase) ||
                                      mvm.Manufacturer.Contains(_filter,
                                                                        StringComparison.InvariantCultureIgnoreCase) ||
                                      mvm.Year.Contains(_filter,
                                                                StringComparison.InvariantCultureIgnoreCase) ||
                                      mvm.SourceFile.Contains(_filter,
                                                                      StringComparison.InvariantCultureIgnoreCase) ||
                                      mvm.Description.Contains(_filter,
                                                                       StringComparison.InvariantCultureIgnoreCase);           
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

            if (e.PropertyName == "IsMechanical" && (sender as MachineViewModel).IsMechanical)
            {
                this.OnPropertyChanged("IsMechanical");
                
                //if ((sender as MachineViewModel).CloneOf == null)
                //    Application.Current.Dispatcher.BeginInvoke(new Action(()=> _view.Refresh()));               
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
