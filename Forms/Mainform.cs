#region

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


using IV_Play.Properties;
using System.Threading.Tasks;
using IV_Play.Data.Models;

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
    private MameInfo _mameInfo;
    private bool updating = false;

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

      if (Settings.Default.full_screen)
      {
        TopMost = true;
        FormBorderStyle = FormBorderStyle.None;
        Bounds = Screen.PrimaryScreen.Bounds;
      }

      //If we don't have a dat file, we need to create one. The progress form is responsible for that.
      if (Settings.Default.MAME_EXE == "")
        SettingsManager.GetMamePath(true, true);

      RefreshGames();

    }

    public async void RefreshGames()
    {
      try
      {
        var xmlParser = new XmlParser();

        if (!File.Exists(Resources.DB_NAME) && !string.IsNullOrEmpty(Settings.Default.MAME_EXE))
        {
          updating = true;
          xmlParser.MakeQuickDat();
          _mameInfo = xmlParser.MameInfo;
          SettingsManager.MameCommands = _mameInfo.Commands;
          updateList(xmlParser.Games);
          var progress = new Progress<int>();
          progress.ProgressChanged += Progress_ProgressChanged;
          await Task.Factory.StartNew(() => xmlParser.MakeDat(progress));
          updateList(xmlParser.Games);
          UpdateTitleBar();
          updating = false;

        }
        else
        {
          xmlParser.ReadDat();
          _mameInfo = xmlParser.MameInfo;
          SettingsManager.MameCommands = _mameInfo.Commands;
          updateList(xmlParser.Games);
        }
      }
      catch (Exception ex)
      {
        Logger.WriteToLog(ex);
      }

      //Now that we know where MAME is we can load the default art assets
      _gameList.LoadDefaultArtAssets();

      UpdateTitleBar();
    }

    private void Progress_ProgressChanged(object sender, int e)
    {
      if (e % 300 == 0 || e == -1)
      {
        UpdateTitleBar(e);
      }
    }

    private void updateList(Games games)
    {
      _gameList.LoadGames(games);
      _gameList.LoadSettings();
      _gameList.Filter = _gameList.Filter;
    }
    private void GameList_GameListChanged(object sender, EventArgs e)
    {
      if (!updating)
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
        Text = string.Format("IV/Play - {0} {1} {2} Games", _mameInfo.Product, _mameInfo.Version,
                             _gameList.Count - _gameList.CountFavorites);
        if (_gameList.CountFavorites > 0)
          Text += string.Format(@" / {0} Favorites", _gameList.CountFavorites);
        if (!string.IsNullOrEmpty(_gameList.Filter))
          Text += string.Format(" - Current Filter: {0}", _gameList.Filter);

        if (progress > -1)
        {
          var progressPercentage = (int)(((float)progress / (float)_gameList.ProgressCount) * 100);
          Text += string.Format(" - Updating {0}%", progressPercentage);
        }
      }
      catch (Exception ex)
      {
        Logger.WriteToLog(ex);
        Text = "IV/Play";
      }
    }
  }
}
