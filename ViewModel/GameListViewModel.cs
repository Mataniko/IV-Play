using IV_Play.DataAccess;
using IV_Play.Model;
using IV_Play.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace IV_Play.ViewModel
{
    class GameListViewModel : ViewModelBase
    {        
        private MameInfo _mameInfo;
        private MachineViewModel _machine;
        private readonly object _MachinesLock = new object();
        private CollectionView _view;

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

        public ObservableCollection<MachineViewModel> Machines { get; private set; }

        public GameListViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;
            if (Settings.Default.MAME_EXE == "")
                SettingsManager.GetMamePath(true, true);

            LoadMachines();
            SettingsManager.GetBackgroundImage();
        }

        private async void LoadMachines()
        {            
            var machineCollection = (from machine in DatabaseManager.GetMachines().Where(x => x.ismechanical == "no") select new MachineViewModel(machine)).ToList();

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
            } 
        }

        private void Progress_ProgressChanged(object sender, int e)
        {
            //if (e % 300 == 0)
            //    Application.Current.Dispatcher.BeginInvoke(new Action(() => _view.Refresh()));
        }

        private bool UserFilter(object item)
        {
            //if ((item as MachineViewModel).IsMechanical)
            //    Console.WriteLine((item as MachineViewModel).Description);
            var mvm = (item as MachineViewModel);
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

            if (e.PropertyName == "IsMechanical" && (sender as MachineViewModel).IsMechanical)
            {
                this.OnPropertyChanged("IsMechanical");                
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

        private RelayCommand _leftCommand;
        public ICommand LeftCommand
        {
            get
            {
                if (_leftCommand == null)
                {
                    _leftCommand = new RelayCommand(
                        param => this.GoToPreviousLetter(),
                        param => true
                        );
                }
                return _leftCommand;
            }
        }

        private RelayCommand _rightCommand;
        public ICommand RightCommand
        {
            get
            {
                if (_rightCommand == null)
                {
                    _rightCommand = new RelayCommand(
                        param => this.GoToNextLetter(),
                        param => true
                        );
                }
                return _rightCommand;
            }
        }

        internal void GoToPreviousLetter()
        {         
            if (CurrentMachine == null)
                return;

            MachineViewModel parent = CurrentMachine.CloneOf == null ? CurrentMachine : (from m in Machines where m.Name == CurrentMachine.CloneOf select m).Single();

            char nextKey;
            char key = Char.ToLower(parent.Description[0]);

            nextKey = key == 'a' ? '9' : Char.ToLower((char)(key - 1));
            var parents = (from mvm in Machines where mvm.CloneOf == null select mvm);

            while (true)
            {
                var machines = (from m in parents where char.ToLower(m.Description[0]) == nextKey select m);
                if (machines.Count() > 0)
                {
                    var newMachine = machines.First();
                    CurrentMachine = newMachine;                 
                    return;
                }

                if (nextKey == '0' - 1)
                {
                    nextKey = '(';
                }
                else if (nextKey == '(' - 1)
                {
                    nextKey = 'z';
                }
                else
                    nextKey--;
            }
        }

        internal void GoToNextLetter()
        {

            if (CurrentMachine == null)
                return;

            MachineViewModel parent = CurrentMachine.CloneOf == null ? CurrentMachine : (from m in Machines where m.Name == CurrentMachine.CloneOf select m).Single();

            char nextKey;
            char key = Char.ToLower(parent.Description[0]);

            nextKey = key == '9' ? 'a' : Char.ToLower((char)(key + 1));
            var parents = from mvm in Machines where mvm.CloneOf == null select mvm;

            while (true)
            {
                var machines = (from m in parents where char.ToLower(m.Description[0]) == nextKey select m);
                if (machines.Count() > 0)
                {
                    var newMachine = machines.First();
                    CurrentMachine = newMachine;
                    return;
                }

                if (nextKey == '(' + 1)
                {
                    nextKey = '0';
                }
                else if (nextKey == 'z' + 1)
                {
                    nextKey = '(';
                }
                else
                    nextKey++;
            }
        }

        private RelayCommand _startCommand;
        public ICommand StartCommand
        {
            get
            {
                if (_startCommand == null)
                {
                    _startCommand = new RelayCommand(
                        param => this.StartGame(),
                        param => true
                        );
                }
                return _startCommand;
            }
        }

        private void StartGame()
        {
            try
            {
                if (CurrentMachine == null) return;

                Console.WriteLine(Settings.Default.MAME_EXE);
                ProcessStartInfo psi = new ProcessStartInfo(Settings.Default.MAME_EXE);
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.Arguments = Settings.Default.command_line_switches + " " + CurrentMachine.Name.Replace("fav_", "");
                psi.WorkingDirectory = Path.GetDirectoryName(Settings.Default.MAME_EXE);
                Process proc = Process.Start(psi);

                StreamReader streamReader = proc.StandardError;

                var window = App.Current.MainWindow;
                window.WindowState = WindowState.Minimized;

                //Thread jumpThread = new Thread(AddGameToJumpList);
                //jumpThread.SetApartmentState(ApartmentState.STA);
                //jumpThread.IsBackground = true;
                //jumpThread.Start();

                proc.WaitForExit();

                using (StringReader stringReader = new StringReader(streamReader.ReadToEnd()))
                {
                    string s = stringReader.ReadToEnd();
                    if (s != null)
                        if (s.Contains("error", StringComparison.InvariantCultureIgnoreCase) && Settings.Default.show_error) // Check is MAME returned an error and display it.
                        {
                            MessageBox.Show(s);
                        }
                }
                window.WindowState = WindowState.Normal;                
            }
            catch
            {
                MessageBox.Show("Error loading MAME, please check that MAME hasn't been moved.");
            }
        }
    }
}
