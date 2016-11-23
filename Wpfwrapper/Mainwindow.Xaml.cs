#region

using IV_Play.Data;
using IV_Play.Data.Models;
using IV_Play.Properties;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
        public MainWindow()
        {
            try
            {

                //Prevent multiple instances of the application from running.
                using (Mutex mutex = new Mutex(false, @"IV-Play MameUI"))
                {
                    if (!mutex.WaitOne(0, false))
                    {
                        MessageBox.Show("An instance of IV-Play is already running.", "Warning:");
                        Close();
                        return;
                    }

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
                var machines = DatabaseManager.GetMachines();
                gameList.ItemsSource = machines;
                _mameInfo = xmlParser.MameInfo;
                SettingsManager.MameCommands = _mameInfo.Commands;
                var progress = new Progress<int>();
                //progress.ProgressChanged += Progress_ProgressChanged;
                await Task.Factory.StartNew(() => xmlParser.MakeDat(progress));
                machines = DatabaseManager.GetMachines().Where(x => x.ismechanical == "no").ToList();
                gameList.ItemsSource = machines;
                //updateList(xmlParser.Games);
                updating = false;
                //UpdateTitleBar();
            } else
            {
                gameList.ItemsSource = DatabaseManager.GetMachines().Where(x => x.ismechanical == "no").ToList();
            }
        }

        private bool UserFilter(object item)
        {            
            var machine = (Machine)item;
            return machine.description.IndexOf("slug", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGames();
            //gameList.ItemsSource = from machine in DatabaseManager.GetMachines() where machine.ismechanical == "no" select machine;
        }

        private void gameList_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.S)
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(gameList.ItemsSource);


               view.Filter = UserFilter;
            }
            if (e.Key == Key.Right)
            {
                e.Handled = true;
                var currentLetter = (int)((Machine)gameList.SelectedItem).description.ToLower()[0];
                var index = gameList.SelectedIndex;
                while (true)
                {
                    var nextItem = (Machine)gameList.Items[++index];

                    if (nextItem.cloneof == null && (int)(nextItem.description.ToLower()[0]) > currentLetter)
                    {
                        gameList.SelectedIndex = index;
                        gameList.ScrollIntoView(gameList.Items[index]);
                        break;
                    }
                        
                }
            }

            if (e.Key == Key.Left)
            {
                e.Handled = true;
                var currentLetter = (int)((Machine)gameList.SelectedItem).description.ToLower()[0];
                var index = gameList.SelectedIndex;
                while (true)
                {
                    var nextItem = (Machine)gameList.Items[--index];

                    if (nextItem.cloneof == null && (int)(nextItem.description.ToLower()[0])+1 < currentLetter)
                    {
                        gameList.SelectedIndex = index;
                        gameList.ScrollIntoView(gameList.Items[index]);
                        break;
                    }

                }
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
    }
}