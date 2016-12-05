#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

using IV_Play.Properties;
using System.Collections;
using IV_Play.DataAccess;

#endregion

namespace IV_Play
{
    /// <summary>
    /// The Game List Control.
    /// </summary>
    internal partial class GameList : UserControl
    {
        private Image _bgImage;
        private Dictionary<string, Game> Games = new Dictionary<string, Game>();
        private Games _games = new Games();
        private Games _gamesFavorites = new Games();
        private Rectangle _imageArea;
        private int _rowHeight;
        private GameListSearch _search;
        private Game _selectedGame;
        private Bitmap _defaultCloneIcon = Resources.default_clone.ToBitmap();
        private Bitmap _defaultIcon = Resources.default_parent.ToBitmap();
        private Bitmap _defaultNonWorkingIcon = Resources.default_nonworking.ToBitmap();
        private Image _defaultNonWorkingSnap = Resources.nonworking;
        private JumpListClass jumpListClass = new JumpListClass();
        private int _prevScrollValue;
        private const int ArtOffset = 11;
        private InfoParser MameInfo;
        private InfoParser History;
        public bool ControlKeyPressed { get; set; }
        private Info _currentInfo = null;
        private string _currentInfoText = "";
        private int infoRow = 0;        

        public static event GameListChanged GameListChanged;

        #region Properties

        private int _countFavorites;
        private string _filter = "";

        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                FilterGames();
            }
        }

        public int CountFavorites
        {
            get { return FavoritesMode != FavoritesMode.Games ? _countFavorites : 0; }
        }

        public FavoritesMode FavoritesMode { get; set; }

        [Browsable(true)]
        public ArtDisplayMode ArtDisplayMode { get; set; }

        public int ArtType { get; set; }

        [DefaultValue(typeof(Size), "16, 16")]
        public Size SmallIconSize { get; set; }

        [DefaultValue(typeof(Size), "32, 32")]
        public Size LargeIconSize { get; set; }

        [Browsable(false)]
        public Game SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                if (value == null && _selectedGame != null)
                    Settings.Default.last_game = _selectedGame.Name;

                _selectedGame = value;

                if (_selectedGame != null)
                {
                    SelectedRow = _selectedGame.Index;
                    ScrollTo(SelectedRow);
                    RefreshImage();
                }
                else
                {
                    SelectedRow = -1;
                }
                Invalidate();
            }
        }

        [Browsable(false)]
        public int SelectedRow { get; set; }

        [Browsable(false)]
        public int Rows { get; set; }

        [Browsable(false)]
        public Game NextVisibleGame
        {
            get { return GetNodeAtRow(VerticalScroll.Value / RowHeight); }
        }

        [Browsable(false)]
        public int NextVisibleRow
        {
            get { return VerticalScroll.Value / RowHeight; }
        }

        [Browsable(false)]
        public Game FirstGame
        {
            get { return Games.Count == 0 ? null : Games.First().Value; }
        }

        [DefaultValue(18), Browsable(false)]
        public int RowHeight
        {
            get { return !DesignMode ? Math.Max(_rowHeight, Font.Height) : 18; }
            set { _rowHeight = value; }
        }

        [DefaultValue(18)]
        public int OffsetParent { get; set; }

        [DefaultValue(37)]
        public int OffsetChild { get; set; }

        [DefaultValue(typeof(Color), "White")]
        public Color ParentColor { get; set; }

        [DefaultValue(typeof(Color), "255,255,128")]
        public Color CloneColor { get; set; }

        [DefaultValue(typeof(Color), "Aqua")]
        public Color FavoriteColor { get; set; }

        [DefaultValue(typeof(Color), "Green")]
        public Color FavoriteCloneColor { get; set; }

        [DefaultValue(11)]
        public int BorderWidth { get; set; }

        [DefaultValue(typeof(Color), "White")]
        public Color BorderColor { get; set; }

        [DefaultValue(75)]
        public int ArtTransperancy { get; set; }

        [DefaultValue(true)]
        public bool ShowYear { get; set; }

        [DefaultValue(true)]
        public bool ShowManufacturer { get; set; }

        [Browsable(false)]
        public int Count
        {
            get { return Games.Count; }
        }

        [Browsable(false)]
        public int ProgressCount
        {
            get { return _games.TotalGames; }
        }

        #endregion

        #region Constructor

        public GameList()
        {
            InitializeComponent();
            ConfigForm.ConfigSaved += ConfigForm_ConfigSaved;
            //These ensure fast drawing
            SetStyle(ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.UserPaint
                     | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.ResizeRedraw
                     | ControlStyles.Selectable
                     | ControlStyles.SupportsTransparentBackColor
                     , true);

            //Register for the settings changed event.
            Settings.Default.SettingChanging += Default_SettingChanging;

            if (!DesignMode)
                LoadSettings();

            //These are our default settings
            OffsetChild = 37;
            OffsetParent = 18;
            RowHeight = 18;
            LargeIconSize = new Size(32, 32);
            SmallIconSize = new Size(16, 16);
        }

        #endregion

        #region Events

        private void ConfigForm_ConfigSaved(object sender, EventArgs e)
        {
            LoadSettings();
            FilterGames();
        }

        private void Default_SettingChanging(object sender, SettingChangingEventArgs e)
        {
            if (!DesignMode)
                LoadSettings();
        }

        private void GameList_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        #endregion

        #region Private Methods

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // GameList
            // 
            Name = "GameList";
            Load += GameList_Load;
            ResumeLayout(false);
        }

        /// <summary>
        /// This background worker manages the JumpList for us so the user can resume using the application.
        /// </summary>
        private void AddGameToJumpList()
        {
            //jumpListClass.AddTask(SelectedGame);
            Dispatcher.Run();
        }

        private DisplayModeEnum DisplayMode
        {
            get
            {
                if (ShowManufacturer && ShowYear)
                    return DisplayModeEnum.DescriptionYearAndManufacturer;

                if (!ShowManufacturer && ShowYear)
                    return DisplayModeEnum.DescriptionAndYear;

                if (ShowManufacturer && !ShowYear)
                    return DisplayModeEnum.DescriptionAndManufacturer;

                return DisplayModeEnum.Description;
            }
        }

        private void StartGame()
        {
            try
            {
                if (SelectedGame != null && Parent.Visible)
                {
                    ProcessStartInfo psi = new ProcessStartInfo(Settings.Default.MAME_EXE);
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;
                    psi.Arguments = Settings.Default.command_line_switches + " " + SelectedGame.Name.Replace("fav_", "");
                    psi.WorkingDirectory = Path.GetDirectoryName(Settings.Default.MAME_EXE);
                    Process proc = Process.Start(psi);

                    StreamReader streamReader = proc.StandardError;

                    Form parent = ((Form)Parent);
                    var windowState = parent.WindowState;
                    parent.WindowState = FormWindowState.Minimized;

                    Thread jumpThread = new Thread(AddGameToJumpList);
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
                    parent.WindowState = windowState;
                }
            }
            catch
            {
                MessageBox.Show("Error loading MAME, please check that MAME hasn't been moved.");
            }
        }

        /// <summary>
        /// Loads our favorites from favorites.ini
        /// </summary>
        /// <returns>How many favorites have been added already to our internal list.</returns>
        private int LoadFavorites()
        {
            _gamesFavorites = new Games();
            _countFavorites = 0;
            if (File.Exists(Settings.Default.favorites_ini))
            {
                string[] favs = File.ReadAllLines(Settings.Default.favorites_ini);
                foreach (string s in favs) //Some of the lines are not favorites, just ignores them.
                {
                    if (s == "") continue;
                    try
                    {
                        Game game = new Game(DatabaseManager.GetMachineByName(s));
                        if (game != null)
                        {
                            Game g = game.Copy();
                            g.Name = "fav_" + g.Name;
                            g.IsFavorite = true;
                            _gamesFavorites.TryAdd(g.Name, g);
                        }
                    }
                    catch (Exception)
                    {
                        //Not a game, do nothing.                       
                    }
                }

                //Sorts out the list by description alphabetically and filters it according to what the user set.
                var sortedDict = (from entry in _gamesFavorites
                                  where
                                      entry.Value.Name.Contains(_filter, StringComparison.InvariantCultureIgnoreCase) ||
                                      entry.Value.Manufacturer.Contains(_filter,
                                                                        StringComparison.InvariantCultureIgnoreCase) ||
                                      entry.Value.Year.Contains(_filter,
                                                                StringComparison.InvariantCultureIgnoreCase) ||
                                      entry.Value.SourceFile.Contains(_filter,
                                                                      StringComparison.InvariantCultureIgnoreCase) ||
                                      entry.Value.Description.Contains(_filter,
                                                                       StringComparison.InvariantCultureIgnoreCase)
                                  orderby entry.Value.Description.ToLower() ascending
                                  select entry);


                //Add the filtered favorites to our game list.
                Game prevGame = null;
                int i = 0;
                foreach (var fGame in sortedDict)
                {
                    fGame.Value.Index = i++;
                    Games.Add(fGame.Key, fGame.Value);
                    _countFavorites++;
                    if (prevGame != null)
                    {
                        prevGame.NextGame = fGame.Value;
                    }
                    fGame.Value.PreviousGame = prevGame;
                    prevGame = fGame.Value;
                }
                return i;
            }
            return 0;
        }

        /// <summary>
        /// This parses our gamelist using the current filters to what were actually displaying.
        /// </summary>
        private void LoadGamesAndFavorites()
        {

            var sortedDict = (from entry in _games
                              where ((!Settings.Default.hide_mechanical_games || (Settings.Default.hide_mechanical_games && !entry.Value.IsMechanical)))
                              orderby entry.Value.IsParent, entry.Value.Description.ToLower() ascending
                              select entry);

            int i = FavoritesMode != FavoritesMode.Games ? LoadFavorites() : 0;

            Game prevGame;
            if (Games.Count == 0)
                prevGame = null;
            else
            {
                prevGame = Games.Last().Value;
            }

            if (FavoritesMode == FavoritesMode.Favorites)
            {
                if (SelectedGame != null && !SelectedGame.IsFavorite)
                    SelectedGame = null;
                Invalidate();
                return;
            }

            var nonWorkingParents = new Hashtable();
           
            foreach (var fGame in sortedDict)
            {
                // Check for non-working children
                if ((!fGame.Value.Working && Settings.Default.hide_nonworking))
                {
                    if (fGame.Value.Children.Where(x => x.Value.Working).Count() == 0)
                        continue;
                    else
                    {
                        nonWorkingParents.Add(fGame.Value.Name, true);                        
                    }                        
                }

                if ((fGame.Value.Name.Contains(_filter, StringComparison.InvariantCultureIgnoreCase) ||
                    fGame.Value.Manufacturer.Contains(_filter, StringComparison.InvariantCultureIgnoreCase) ||
                    fGame.Value.SourceFile.Contains(_filter, StringComparison.InvariantCultureIgnoreCase) ||
                    fGame.Value.Year.Contains(_filter, StringComparison.InvariantCultureIgnoreCase) ||
                    fGame.Value.Description.Contains(_filter, StringComparison.InvariantCultureIgnoreCase)) && !nonWorkingParents.ContainsKey(fGame.Value.Name))
                {                
                    fGame.Value.Index = i++;                  
                    Games.Add(fGame.Key, fGame.Value);
                    if (prevGame != null)
                    {
                        prevGame.NextGame = fGame.Value;
                    }
                    fGame.Value.PreviousGame = prevGame;
                    prevGame = fGame.Value;
                }
                if (!Settings.Default.hide_clones)
                {
                    foreach (var child in fGame.Value.Children)
                    {
                        if ((!child.Value.Working && Settings.Default.hide_nonworking)) continue;
                                               
                        if (nonWorkingParents.ContainsKey(child.Value.ParentSet))
                        {
                            child.Value.ShowAsParent = true;
                            nonWorkingParents.Remove(child.Value.ParentSet);
                        }

                        if (child.Value.Name.Contains(_filter, StringComparison.InvariantCultureIgnoreCase) ||
                            child.Value.Manufacturer.Contains(_filter, StringComparison.InvariantCultureIgnoreCase) ||
                            child.Value.SourceFile.Contains(_filter, StringComparison.InvariantCultureIgnoreCase) ||
                            child.Value.Year.Contains(_filter, StringComparison.InvariantCultureIgnoreCase) ||
                            child.Value.Description.Contains(_filter, StringComparison.InvariantCultureIgnoreCase))
                        {                            
                            child.Value.Index = i++;
                            Games.Add(child.Key, child.Value);
                            if (prevGame != null)
                            {
                                prevGame.NextGame = child.Value;
                                fGame.Value.PreviousGame = prevGame;
                            }
                            prevGame = child.Value;                         
                        }
                    }
                }
            }
        }

        private Game GetNodeParent(Game node)
        {
            if (node == null)
                return null;

            if (node.IsFavorite)
                return node;

            return !node.IsParent && Games.ContainsKey(node.ParentSet) ? Games[node.ParentSet] : node;
        }

        private Game GetNodeAtRow(int row)
        {
            try
            {
                if (row >= Games.Count)
                    return null;
                return Games.ElementAt(row).Value;
            }
            catch (Exception)
            {

                return null;
            }

        }

        /// <summary>
        /// Find the art path depending on the current Art Type
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns></returns>
        private string GetArtPath(string gameName)
        {
            string path = "";
            try
            {
                path = Path.Combine(SettingsManager.ArtPaths[Settings.Default.art_type], gameName);
            }
            catch (Exception)
            {
                return "";
            }
            return path;
        }

        /// <summary>
        /// Caches the icon from disk when drawing the game row for the first time
        /// </summary>
        /// <param name="game"></param>
        private void FetchIcon(Game game)
        {
            if (game.Icon == null || (!game.Working && !game.HasOverlay && Settings.Default.non_working_overlay))
            {
                if (File.Exists(game.IconPath))
                {
                    try
                    {
                        game.Icon = new Icon(game.IconPath).ToBitmap();

                        if (!game.Working && Settings.Default.non_working_overlay)
                            DrawNonWorkingOverlay(game);

                    }
                    catch (Exception ex)
                    {
                        Logger.WriteToLog(ex);
                        if (game.Working)
                            game.Icon = game.IsParent ? _defaultIcon : _defaultCloneIcon;
                        else
                        {
                            game.Icon = _defaultNonWorkingIcon;
                            game.HasOverlay = true;
                        }
                    }

                }
                else
                {
                    try
                    {

                        if (game.IsParent)
                        {
                            if (game.Working)
                                game.Icon = _defaultIcon;
                            else
                            {
                                game.Icon = _defaultNonWorkingIcon;
                                game.HasOverlay = true;
                            }
                        }
                        else
                        {
                            if (game.Working && File.Exists(Path.Combine(Settings.Default.icons_directory, game.ParentSet + ".ico")))
                            {
                                game.Icon =
                                    new Icon(Path.Combine(Settings.Default.icons_directory, game.ParentSet + ".ico")).
                                        ToBitmap();

                                //if (!game.Working && Settings.Default.non_working_overlay)
                                //      DrawNonWorkingOverlay(game);

                            }
                            else
                                if (game.Working)
                                game.Icon = _defaultCloneIcon;
                            else
                            {
                                game.Icon = _defaultNonWorkingIcon;
                                game.HasOverlay = true;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteToLog(ex);
                        if (game.Working)
                            game.Icon = game.IsParent ? _defaultIcon : _defaultCloneIcon;
                        else
                            game.Icon = _defaultNonWorkingIcon;
                    }
                }
            }
            else
            {
                if (!game.Working && game.HasOverlay && !Settings.Default.non_working_overlay)
                {
                    game.Icon = null;
                    game.HasOverlay = false;
                    FetchIcon(game);
                }
            }


        }

        private void DrawNonWorkingOverlay(Game game)
        {
            Graphics g = Graphics.FromImage(game.Icon);
            g.DrawImageUnscaled(Properties.Resources.nonworkingoverlay, 16, 16);
            game.HasOverlay = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the control properties.
        /// </summary>
        public void LoadSettings()
        {
            if (Process.GetCurrentProcess().ProcessName != "devenv")
            {
                Font = Settings.Default.game_list_font;
                ParentColor = Settings.Default.game_list_color;
                CloneColor = Settings.Default.game_list_clone_color;
                FavoriteColor = Settings.Default.favorites_color;
                ShowManufacturer = Settings.Default.GameListManufacturer;
                ShowYear = Settings.Default.GameListYear;
                ArtTransperancy = Settings.Default.art_opacity;
                BorderColor = Settings.Default.art_border_color;
                BorderWidth = Settings.Default.art_border_width;
                ArtType = Settings.Default.art_type;
                ArtDisplayMode = (ArtDisplayMode)Settings.Default.art_area;
                FavoritesMode = (FavoritesMode)Settings.Default.favorites_mode;

                try
                {
                    string path = Path.Combine(Settings.Default.bkground_directory, Settings.Default.bkground_image);
                    BackgroundImage = File.Exists(path) ? Image.FromFile(path) : Resources.Default_Background;
                }
                catch
                {
                    BackgroundImage = Resources.Default_Background;
                }
            }
        }
        /// <summary>
        /// Filters the game list, this gets called whenever the Filter Property is set and should not be called directly
        /// </summary>
        internal void FilterGames()
        {
            if (SelectedGame != null)
            {
                Settings.Default.last_game = SelectedGame.Name;
                SelectedGame = null;
            }

            // Clear out the game collection before filtering.                        
            Games.Clear();
            if (_games != null && _games.Count > 0)
            {
                SuspendLayout();

                LoadGamesAndFavorites();

                if (Games.Count > 0)
                    Games.Last().Value.NextGame = null;
                //Sets up our scrolling 

                AutoScroll = true;
                AutoScrollMinSize = new Size(ClientRectangle.Width, Games.Count * RowHeight);

                VerticalScroll.Value = AutoScrollMinSize.Height < _prevScrollValue ? 0 : _prevScrollValue;
                VerticalScroll.SmallChange = RowHeight;

                Refresh();

                //Initalizes the list searcher and locates the last game.
                _search = new GameListSearch(this);
                ResumeLayout();


                GoToLastGame();

                //Raise the Gamelist event
                if (GameListChanged != null)
                    GameListChanged(null, null);
            }
        }

        /// <summary>
        /// Adds or removes games to the favorites, this has to refresh the list.
        /// </summary>
        /// <param name="game"></param>
        public void AddRemoveFavorites(Game game)
        {
            if (game == null)
                return;

            if (game.IsFavorite)
            {
                //remove game from favorites file
                string[] favs = new string[1];
                try
                {
                    if (File.Exists(Settings.Default.favorites_ini))
                        favs = File.ReadAllLines(Settings.Default.favorites_ini);
                }
                catch (Exception ex)
                {
                    Logger.WriteToLog(ex);
                }

                string name = game.Name.Replace("fav_", "");

                try
                {
                    List<string> favList = favs.ToList();
                    favList.Remove(name);
                    favList.Sort();
                    File.WriteAllLines(Settings.Default.favorites_ini, favList, Encoding.ASCII);
                }
                catch (Exception ex)
                {
                    Logger.WriteToLog(ex);
                }
                SelectedGame = SelectedGame.NextGame;
                if (FavoritesMode != FavoritesMode.Games)
                    Filter = _filter;
            }
            else //Add game to favorites
            {
                //check that no favorite exists
                if (!Games.ContainsKey("fav_" + game.Name))
                {
                    string[] favs = new string[1];

                    try
                    {
                        if (File.Exists(Settings.Default.favorites_ini))
                            favs = File.ReadAllLines(Settings.Default.favorites_ini);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteToLog(ex);
                    }

                    try
                    {
                        List<string> favList = favs.ToList();
                        favList.Add(game.Name);
                        favList.Sort();
                        File.WriteAllLines(Settings.Default.favorites_ini, favList, Encoding.ASCII);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteToLog(ex);
                    }

                    if (FavoritesMode != FavoritesMode.Games)
                        Filter = _filter;
                }
            }
        }

        /// <summary>
        /// Loads the game art from disk.
        /// </summary>
        public void RefreshImage()
        {

            //Reset the info row for scrolling
            infoRow = 0;

            if (SelectedGame != null)
            {
                //Load the nonworking image                
                try
                {
                    string gameName = SelectedGame.Name;
                    gameName = gameName.Replace("fav_", "");
                    gameName += ".png";
                    string path = GetArtPath(gameName);
                    string parentPath = GetArtPath(SelectedGame.ParentSet + ".png");

                    if (SettingsManager.ArtPaths[ArtType].Contains(".dat"))
                    {
                        _bgImage = null;
                        return;
                    }

                    if (File.Exists(path))
                        _bgImage = Image.FromFile(path);
                    else if (SelectedGame.Working && File.Exists(parentPath))
                        _bgImage = Image.FromFile(parentPath);
                    else if (!SelectedGame.Working)
                        _bgImage = _defaultNonWorkingSnap;
                    else
                        _bgImage = null;
                }
                catch (Exception ex)
                {
                    Logger.WriteToLog(ex);
                    _bgImage = null;
                }
            }
        }

        /// <summary>
        /// Looks if the user has his own default icons/pngs
        /// </summary>
        internal void LoadDefaultArtAssets()
        {
            //Load the default icons
            try
            {
                if (File.Exists(Path.Combine(Settings.Default.icons_directory, "working.ico")))
                    _defaultIcon = new Icon(Path.Combine(Settings.Default.icons_directory, "working.ico")).ToBitmap();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex);
            }
            try
            {
                if (File.Exists(Path.Combine(Settings.Default.icons_directory, "clone.ico")))
                    _defaultCloneIcon = new Icon(Path.Combine(Settings.Default.icons_directory, "clone.ico")).ToBitmap();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex);
            }

            try
            {
                if (File.Exists(Path.Combine(Settings.Default.icons_directory, "nonworking.ico")))
                    _defaultNonWorkingIcon =
                        new Icon(Path.Combine(Settings.Default.icons_directory, "nonworking.ico")).ToBitmap();
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex);
            }

            //Load the default nonworking image
            try
            {
                string snapPath = SettingsManager.ArtPaths.Find(s => s.Contains("snap"));
                if (snapPath != null && File.Exists(Path.Combine(snapPath, "nonworking.png")))
                    _defaultNonWorkingSnap =
                        Image.FromFile(Path.Combine(snapPath, "nonworking.png"));
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex);
            }
        }

        internal void LoadGames(Games games)
        {
            _games = games;           
        }

        #endregion
    }
}