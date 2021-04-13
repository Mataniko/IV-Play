using System;
using System.Windows.Forms;

using IV_Play.Properties;
using System.Threading.Tasks;

namespace IV_Play
{
  public partial class MainForm
  {

    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Alt) return;

      switch (e.KeyCode)
      {
        case Keys.F5:
          _tsRefresh_Click();
          break;
        case Keys.F4:
          _tsPairing_Click();
          break;
        case Keys.F1:
          _tsHelpForm_Click();
          break;
      }
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
      if (keyData == (Keys.Control | Keys.E) || keyData == (Keys.Control | Keys.F))
      {
        _gameList.ControlKeyPressed = false;
        filterToolStripMenuItem_Click();
        return true;
      }
      if (keyData == (Keys.Alt | Keys.Enter))
      {
        _tsProperties_Click();
        return true;
      }
      else if (keyData == (Keys.P | Keys.Alt))
      {
        _tsPictureMode_Click();
        return true;
      }
      else if (keyData == (Keys.D | Keys.Alt))
      {
        _tsFavorites_Click();
        return true;
      }
      else if (keyData == (Keys.D | Keys.Control))
      {
        _tsFavs_Click();
        return true;
      }
      return base.ProcessDialogKey(keyData);
    }

    private async void RefreshGameList()
    {
      if (!updating)
      {
        updating = true;
        var xmlParser = new XmlParser();
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
    }

    private void filterToolStripMenuItem_Click()
    {
      if (filterDialog.ShowDialog(this, _gameList.Filter) == DialogResult.OK)
      {
        _gameList.Filter = filterDialog.Filter;
      }
    }

    private void _tsFavorites_Click()
    {
      if (Settings.Default.favorites_mode == 2)
        Settings.Default.favorites_mode = 0;
      else
        Settings.Default.favorites_mode++;
      _gameList.FavoritesMode = (FavoritesMode)Settings.Default.favorites_mode;
      _gameList.FilterGames();
    }

    private void _tsPairing_Click()
    {
      if (SettingsManager.GetMamePath(false, false))
        RefreshGameList();
    }

    private void _tsFavs_Click()
    {
      _gameList.AddRemoveFavorites(_gameList.SelectedGame);
    }

    private void _tsProperties_Click()
    {
      PropertiesView propertiesView = new PropertiesView(_gameList.SelectedGame);
      propertiesView.ShowDialog(this);
    }

    private void _tsPictureMode_Click()
    {
      if (Settings.Default.art_area == (int)ArtDisplayMode.superlarge)
        Settings.Default.art_area = 0;
      else
        Settings.Default.art_area++;
      _gameList.ArtDisplayMode = (ArtDisplayMode)Settings.Default.art_area;
      Invalidate();
    }

    private void _tsHelpForm_Click()
    {
      using (ConfigForm configForm = new ConfigForm())
      {
        configForm.ShowDialog(this);
      }
    }

    private void _tsRefresh_Click()
    {
      RefreshGameList();
    }
  }
}
