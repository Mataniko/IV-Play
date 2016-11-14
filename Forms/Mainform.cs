#region

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


using IV_Play.Properties;
using System.Threading.Tasks;

#endregion

//using System.Linq;

namespace IV_Play
{
    /// <summary>
    /// Our main form. Handles all of the key strokes through a hidden menu.
    /// (It's much easier to manage than managing the onkey events of the control)
    /// </summary>
    public partial class MainForm : Form
    {
        private FilterDialog filterDialog = new FilterDialog();

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This helps drawing the form faster, do not remove.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            BringToFront();

            GameList.GameListChanged += GameList_GameListChanged;

            _gameList.BackgroundImage = SettingsManager.BackgroundImage;
            Location = new Point(Settings.Default.Window_x, Settings.Default.Window_y);
            Size = new Size(Settings.Default.window_width, Settings.Default.window_height);

            //If we don't have a dat file, we need to create one. The progress form is responsible for that.
            if (Settings.Default.MAME_EXE == "")
                SettingsManager.GetMamePath(true, true);

            try
            {
                if (!File.Exists(Resources.DB_NAME) && !string.IsNullOrEmpty(Settings.Default.MAME_EXE))
                {
                    XmlParser.MakeQuickDat();
                    var progress = new Progress<int>();
                    progress.ProgressChanged += Progress_ProgressChanged;
                    var task = new Task(() => XmlParser.MakeDat(progress));
                    Task.Factory.ContinueWhenAll(new Task[] { task }, (Action) => updateList());
                    task.Start();
                }
                else
                {
                    XmlParser.ReadDat();
                }
            }
            catch (Exception)
            {
            }

            //Now that we know where MAME is we can load the default art assets
            _gameList.LoadDefaultArtAssets();

            //Load our games. Setting a filter is important because it also populates the list
            //a blank string will return everything.
            updateList();

            UpdateTitleBar();

            //InfoParser infoParser = new InfoParser(@"D:\Games\Emulators\MAME\command.dat");
        }

        private void Progress_ProgressChanged(object sender, int e)
        {
            if (e % 300 == 0) {
                UpdateTitleBar(e);                
            } else if (e == _gameList.Count)
            {
                UpdateTitleBar();
            }
            
        }

        private void updateList()
        {
            _gameList.LoadGames(XmlParser.ParsedGames);
            _gameList.LoadSettings();
            _gameList.Filter = "";
        }
        private void GameList_GameListChanged(object sender, EventArgs e)
        {
            UpdateTitleBar();
        }         

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            Settings.Default.window_width = Width;
            Settings.Default.window_height = Height;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Window_x = Location.X;
            Settings.Default.Window_y = Location.Y;
            if (_gameList.SelectedGame != null)
                Settings.Default.last_game = _gameList.SelectedGame.Name;
            SettingsManager.WriteSettingsToFile();
        }
        
        /// <summary>
        /// Updates the game count in the titlebar.
        /// </summary>
        private void UpdateTitleBar(int progress = -1)
        {
            try
            {
                Text = string.Format("IV/Play - {0} {1} {2} Games", GetMameType(), XmlParser.ParsedGames.MameVersion,
                                     _gameList.Count - _gameList.CountFavorites);
                if (_gameList.CountFavorites > 0)
                    Text += string.Format(@" / {0} Favorites", _gameList.CountFavorites);
                if (!string.IsNullOrEmpty(_gameList.Filter))
                    Text += string.Format(" - Current Filter: {0}", _gameList.Filter);

                if (progress > -1)
                {
                    var progressPercentage = (int)(((float)progress / (float)_gameList.Count) * 100);
                    Text += string.Format(" - Updating {0}%", progressPercentage);            
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex);
                Text = "IV/Play";
            }
        }

        /// <summary>
        /// Returns the MAME product name for the title bar.
        /// </summary>
        /// <returns></returns>
        private string GetMameType()
        {
            try
            {
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Settings.Default.MAME_EXE);
                return fileVersionInfo.ProductName.ToUpper();
            }
            catch (Exception)
            {
                return "";
            }
        }      

        

    }
}