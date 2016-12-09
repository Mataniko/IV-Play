using IVPlay.DataAccess;
using IVPlay.Properties;
using IVPlay.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shell;

namespace IVPlay.ViewModel
{
    partial class GameListViewModel
    {
        #region Machine Properties Command (CTRL+Enter)

        private RelayCommand _propertiesFormCommand;
        public ICommand PropertiesFormCommand
        {
            get
            {
                if (_propertiesFormCommand == null)
                {
                    _propertiesFormCommand = new RelayCommand(
                        param => this.OpenPropertiesForm(),
                        param => true
                        );
                }
                return _propertiesFormCommand;
            }
        }

        private void OpenPropertiesForm()
        {
            var props = new MachinePropertiesView(CurrentMachine);
            props.ShowDialog();
        }
        #endregion

        #region Navigation Commands (Left, Right)
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
            var parents = (from MachineViewModel mvm in _view where mvm.CloneOf == null select mvm);

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
            var parents = from MachineViewModel mvm in _view where mvm.CloneOf == null select mvm;

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
        #endregion

        #region Settings Command (F1)
        private RelayCommand _settingsCommand;
        public ICommand SettingsCommand
        {
            get
            {
                if (_settingsCommand == null)
                {
                    _settingsCommand = new RelayCommand(
                        param => this.OpenSettingsDialog(),
                        param => true
                        );
                }
                return _settingsCommand;
            }
        }

        private void OpenSettingsDialog()
        {
            var settingsDialog = new SettingsView();
            settingsDialog.ShowDialog();
        }
        #endregion

        #region Refresh Command (F4, F5)
        private RelayCommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(
                        param => this.RefreshGameList(),
                        param => true
                        );
                }
                return _refreshCommand;
            }
        }

        private bool _updating = false;
        private async void RefreshGameList()
        {
            if (!_updating)
            {
                _updating = true;
                var xmlParser = new XmlParser();
                xmlParser.MakeQuickDat();                
                var machineCollection = (from machine in DatabaseManager.GetMachines().Where(x => x.IsMechanical == "no") select new MachineViewModel(machine)).ToList();
                var favorites = LoadFavorites();

                foreach (MachineViewModel machine in machineCollection)
                {
                    machine.IsFavorite = favorites.Contains(machine.Name);
                    machine.PropertyChanged += this.Machine_PropertyChanged;
                }
                
                if (this.Machines == null)
                {
                    var favoritesMachines = machineCollection.Where(x => x.IsFavorite).OrderBy(y=>y.Description);
                    var normalMachines = machineCollection.Where(x => !x.IsFavorite).OrderBy(y => y.Id);
                    this.Machines = new ObservableCollection<MachineViewModel>(favoritesMachines.Concat(normalMachines));
                    this.Machines.CollectionChanged += this.Machines_CollectionChanged;
                } else
                {                                        
                    this.Machines.CollectionChanged -= this.Machines_CollectionChanged;
                    this.Machines.Clear();
                    foreach (var machine in machineCollection)
                    {
                        this.Machines.Add(machine);
                    }
                    this.Machines.CollectionChanged += this.Machines_CollectionChanged;
                }                
                
                _view = (CollectionView)CollectionViewSource.GetDefaultView(this.Machines);
                _view.Filter = UserFilter;
                _view.Refresh();
                UpdateTitle();

                BindingOperations.EnableCollectionSynchronization(this.Machines, _MachinesLock);
                App.Current.MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                var progress = new Progress<int>();
                progress.ProgressChanged += Progress_ProgressChanged;
                await Task.Factory.StartNew(() => xmlParser.MakeDat(progress, this.Machines));
                _view.Refresh();
                App.Current.MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
                _updating = false;
                UpdateTitle();
            }            
        }

        private HashSet<string> LoadFavorites()
        {
            if (!File.Exists(Settings.Default.favorites_ini))
                return new HashSet<string>();
            
            string[] favs = File.ReadAllLines(Settings.Default.favorites_ini);
            return new HashSet<string>(favs);                                   
        }
        #endregion

        #region Favorites Command (CTRL+D)
        private RelayCommand _addRemoveFavoriteCommand;
        public ICommand AddRemoveFavoriteCommand
        {
            get
            {
                if (_addRemoveFavoriteCommand == null)
                {
                    _addRemoveFavoriteCommand = new RelayCommand(
                        param => this.UpdateFavorites(),
                        param => true
                        );
                }
                return _addRemoveFavoriteCommand;
            }
        }     

        private void UpdateFavorites()
        {
            if (CurrentMachine == null || _updating) return;

            if (CurrentMachine.IsFavorite)
            {                             
                if (!File.Exists(Settings.Default.favorites_ini)) return;
                
                var favs = File.ReadAllLines(Settings.Default.favorites_ini).ToList();

                favs.Remove(CurrentMachine.Name);
                File.WriteAllLines(Settings.Default.favorites_ini, favs);
                CurrentMachine.IsFavorite = false;
            }
            else //Add game to favorites
            {
                CurrentMachine.IsFavorite = true;
                var favs = (from mvm in Machines where mvm.IsFavorite select mvm.Name).ToList();
                File.WriteAllLines(Settings.Default.favorites_ini, favs);                
            }
        }
        #endregion

        #region Filter Command (CTRL+F)
        private RelayCommand _filterCommand;
        public ICommand FilterCommand
        {
            get
            {
                if (_filterCommand == null)
                {
                    _filterCommand = new RelayCommand(
                        param => this.StartFiltering(),
                        param => true
                        );
                }
                return _filterCommand;
            }
        }

        private void StartFiltering()
        {
            
        }
        #endregion

        #region Start Command (Enter, LeftDoubleClick)
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
                
                Thread jumpThread = new Thread(() => _jumpList.AddTask(CurrentMachine));
                jumpThread.SetApartmentState(ApartmentState.STA);
                jumpThread.IsBackground = true;
                jumpThread.Start();

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
        #endregion
    }
}
