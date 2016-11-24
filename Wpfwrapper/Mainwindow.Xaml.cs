#region

using IV_Play.Data;
using IV_Play.Data.Models;
using IV_Play.Properties;
using IV_Play.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.MessageBox;

#endregion

namespace IV_Play
{
    /// <summary>
    /// Wrapper WPF window for our application, this enables us to use cool features like the JumpList
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool updating = false;
        private MameInfo _mameInfo;
        private List<Machine> machines;
        private Navigation _navigation;
        public MainWindow()
        {
            try
            {

                //Prevent multiple instances of the application from running.
                using (Mutex mutex = new Mutex(false, @"IV-Play MameUI"))
                {
                    //if (!mutex.WaitOne(0, false))
                    //{
                    //    MessageBox.Show("An instance of IV-Play is already running.", "Warning:");
                    //    Close();
                    //    return;
                    //}

                    //Enabled all the winforms visual styles to give it the Windows look.
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    
                    //Load the background image for the main form.
                    SettingsManager.GetBackgroundImage();

                    //Display the form. While the status is retry it will just create new instances
                    //This allows us to close the form and restart the application when refreshing
                    //our data files.
                    //MainForm mainForm;
                    //DialogResult dialogResult = System.Windows.Forms.DialogResult.Retry;


                    LoadGames();

                    //while (true)
                    //{
                    //if (dialogResult == System.Windows.Forms.DialogResult.Retry)
                    //{
                    //    mainForm = new MainForm();
                    //    mainForm.BringToFront();
                    //    mainForm.Show();
                    //}
                    //else
                    //{
                    //    break;
                    //}
                    //}
                    //Close();
                }
                }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex);
                Close();
            }
        }

        private async void LoadGames()
        {
            Console.WriteLine(Settings.Default.MAME_EXE);
            if (!File.Exists(Properties.Resources.DB_NAME) && !string.IsNullOrEmpty(Settings.Default.MAME_EXE))
            {
                var xmlParser = new XmlParser();
                updating = true;
                xmlParser.MakeQuickDat();
                machines = DatabaseManager.GetMachines();                
                _mameInfo = xmlParser.MameInfo;
                SettingsManager.MameCommands = _mameInfo.Commands;
                var progress = new Progress<int>();                                
                await Task.Factory.StartNew(() => xmlParser.MakeDat(progress));
                machines = DatabaseManager.GetMachines().Where(x => x.ismechanical == "no").ToList();
                gameList.ItemsSource = machines;
                updating = false;
            } else
            {
                _mameInfo = DatabaseManager.GetMameInfo();
                machines = DatabaseManager.GetMachines().Where(x => x.ismechanical == "no").ToList();
            }
        }

        private bool UserFilter(object item)
        {            
            var machine = (Machine)item;
            return machine.description.IndexOf("slug", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            gameList.ItemsSource = machines;
            _navigation = new Navigation(gameList);
        }

        private void gameList_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

           if (e.Key == Key.A)
            {
                if (gameList.SelectedItem == null) return;

                var prop = new IV_Play.View.Properties((Machine)gameList.SelectedItem);
                prop.ShowDialog();
            }

            if (e.Key == Key.S)
            {
               CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(gameList.ItemsSource);                
               view.Filter = UserFilter;
            }

            if (e.Key == Key.Left)
            {
                e.Handled = true;
                _navigation.GoToPreviousCharacter();
            }

            if (e.Key == Key.Right)
            {
                e.Handled = true;
                _navigation.GoToNextLetter();                                
            }

           
        }

        private void gameList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var currentMachine = ((Machine)gameList.SelectedItem);

            if (File.Exists(string.Format(@"D:\Games\Emulators\MAME\snap\{0}.png", currentMachine.name)))
                previewImage.Source = new ImageSourceConverter().ConvertFromString(string.Format(@"D:\Games\Emulators\MAME\snap\{0}.png", currentMachine.name)) as ImageSource;
            else if ((File.Exists(string.Format(@"D:\Games\Emulators\MAME\snap\{0}.png", currentMachine.cloneof))))
                previewImage.Source = new ImageSourceConverter().ConvertFromString(string.Format(@"D:\Games\Emulators\MAME\snap\{0}.png", currentMachine.cloneof)) as ImageSource;
            else
                previewImage.Source = null;


        }               

        private void gameList_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
           
        }

        private void gameList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            try
            {
                var machine = (Machine)gameList.SelectedItem;
                if (machine != null)
                {
                    ProcessStartInfo psi = new ProcessStartInfo(Settings.Default.MAME_EXE);
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;
                    psi.Arguments = Settings.Default.command_line_switches + " " + machine.name.Replace("fav_", "");
                    psi.WorkingDirectory = Path.GetDirectoryName(Settings.Default.MAME_EXE);
                    Process proc = Process.Start(psi);

                    StreamReader streamReader = proc.StandardError;

                    this.WindowState = WindowState.Minimized;

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
                    this.WindowState = WindowState.Normal;
                }
            }
            catch
            {
                MessageBox.Show("Error loading MAME, please check that MAME hasn't been moved.");
            }
        }
    }
}