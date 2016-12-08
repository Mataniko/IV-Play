using IV_Play.Properties;
using IV_Play.View;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace IV_Play.ViewModel
{
    partial class GameListViewModel
    {
        #region Machine Properties Command (CTRL+Enter)

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
