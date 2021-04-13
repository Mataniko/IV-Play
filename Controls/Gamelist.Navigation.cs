#region

using System;
using System.Linq;

using IV_Play.Properties;

#endregion

namespace IV_Play
{
  internal partial class GameList
  {
    public void GoToLastGame()
    {
      try
      {
        int i = 0;
        if (FavoritesMode == FavoritesMode.Favorites)
        {
          Game game = _search.FindGameByName(Settings.Default.last_game, out i);
          if (game != null && game.IsFavorite)
            SelectedGame = game;
        }
        else
        {
          Game game = _search.FindGameByName(Settings.Default.last_game, out i);
          SelectedGame = game ?? Games.First().Value;
        }
      }
      catch (Exception)
      {
        SelectedGame = null;
      }
    }

    private void GoToPreviousCharacter()
    {
      Game game;

      game = GetNodeParent(SelectedGame ?? GetNodeAtRow(VerticalScroll.Value / RowHeight));

      if (game == null)
        return;

      char nextKey;
      char key = Char.ToLower(game.Description.ToCharArray()[0]);

      nextKey = key == 'a' ? '9' : Char.ToLower((char)(key - 1));

      bool favorite = game.IsFavorite;
      while (!GoToLetter(nextKey, favorite))
      {
        if (nextKey == key)
          return;

        if (nextKey == '(' - 1)
        {
          nextKey = 'z';
          favorite = CountFavorites > 0 ? !game.IsFavorite : game.IsFavorite;
        }
        else
          nextKey--;
      }
    }

    private void ScrollTo(int inRow)
    {
      try
      {
        int row = inRow;
        row = row * RowHeight;

        if (row > VerticalScroll.Value)
          row += ClientRectangle.Height % RowHeight;
        int topBounds = VerticalScroll.Value + ClientRectangle.Height;
        if (!(row > VerticalScroll.Value && row < VerticalScroll.Value + ClientRectangle.Height))
          if (row >= topBounds)
            VerticalScroll.Value += row - topBounds + RowHeight;
          else
            VerticalScroll.Value = row;
      }
      catch (Exception ex)
      {
        Logger.WriteToLog(ex);
      }
      _prevScrollValue = Math.Min(VerticalScroll.Value, VerticalScroll.Maximum);
    }

    private bool GoToLetter(char key, bool favorite)
    {
      bool found = false;
      key = Char.ToLower(key);

      if (Games.Count > 1)
      {
        foreach (Game game in Games.Values)
        {
          if (!string.IsNullOrEmpty(game.Description) && (game.IsParent || game.IsFavorite) &&
              Char.ToLower(game.Description.ToCharArray()[0]) == key && game.IsFavorite == favorite)
          {
            found = true;
            SelectedGame = game;
            break;
          }
        }
      }
      else
        return true;
      return found;
    }

    private void GoToNextLetter()
    {

      Game game;

      game = GetNodeParent(SelectedGame ?? GetNodeAtRow(VerticalScroll.Value / RowHeight));

      if (game == null)
        return;

      //Game game;
      //if (SelectedGame == null)
      //{
      //    game = GetNodeParent(GetNodeAtRow(VerticalScroll.Value/RowHeight));
      //    _selectedGame = game;
      //}
      //else
      //    game = GetNodeParent(SelectedGame);


      char nextKey;
      char key = Char.ToLower(game.Description.ToCharArray()[0]);

      nextKey = key == '9' ? 'a' : Char.ToLower((char)(key + 1));

      bool favorite = game.IsFavorite;
      while (!GoToLetter(nextKey, favorite))
      {
        if (nextKey == key)
          return;

        if (nextKey == 'z' + 1)
        {
          nextKey = '(';
          favorite = CountFavorites > 0 ? !game.IsFavorite : game.IsFavorite;
        }
        else
          nextKey++;
      }
    }

    private void NavigateBackward(int n)
    {
      int row = Math.Max(SelectedRow - n, 0);
      if (row != SelectedRow)
        SelectedGame = GetNodeAtRow(row);
    }

    private void NavigateForward(int n)
    {
      int row = Math.Min(SelectedRow + n, Games.Count - 1);
      if (row != SelectedRow)
        SelectedGame = GetNodeAtRow(row);
    }
  }
}
