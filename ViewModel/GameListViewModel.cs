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
using System.Windows.Input;

namespace IV_Play.ViewModel
{
    class GameListViewModel : ViewModelBase
    {        
        private MameInfo _mameInfo;
        private MachineViewModel _machine;
        public MachineViewModel CurrentMachine { get
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
                var machineCollection = (from machine in DatabaseManager.GetMachines().Where(x => x.ismechanical == "no") select new MachineViewModel(machine)).ToList();                

                foreach (MachineViewModel machine in machineCollection)                
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
                
                App.Current.MainWindow.WindowState = WindowState.Minimized;

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
                App.Current.MainWindow.WindowState = WindowState.Normal;                
            }
            catch
            {
                MessageBox.Show("Error loading MAME, please check that MAME hasn't been moved.");
            }
        }
    }
}
