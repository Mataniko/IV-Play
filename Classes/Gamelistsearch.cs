#region

using System;
using System.Collections.Generic;

#endregion

//using System.Linq;

namespace IV_Play
{
  internal class GameListSearch
  {
    private const int SearchTimeout = 600; //end of incremental search timeot in msec

    private Game _currentNode;
    private GameList _gameList;
    private DateTime _lastKeyPressed = DateTime.Now;
    private string _searchString = "";
    private int iterations;

    public GameListSearch(GameList GameList)
    {
      _gameList = GameList;
    }

    public void Search(Char value)
    {
      if (Char.IsControl(value))
        return;

      Char ch = Char.ToLowerInvariant(value);
      DateTime dt = DateTime.Now;
      TimeSpan ts = dt - _lastKeyPressed;
      _lastKeyPressed = dt;

      if (ts.TotalMilliseconds < SearchTimeout)
        ContinuousSearch(ch);
      else
        FirstCharSearch(ch);

    }

    private void ContinuousSearch(Char value)
    {
      if (value == ' ' && String.IsNullOrEmpty(_searchString))
        return; //Ingnore leading space

      _searchString += value;
      DoContinuousSearch();
    }

    private void FirstCharSearch(Char value)
    {
      if (value == ' ')
        return;

      _searchString = value.ToString();
      Game node = null;
      if (_gameList.SelectedGame != null)
        node = _gameList.SelectedGame.NextGame; // ++next visible
      if (node == null)
        node = _gameList.NextVisibleGame;

      if (node != null)
        foreach (string label in IterateNodeLabels(node, SearchType.Description))
        {
          if (label.StartsWith(_searchString))
          {
            _gameList.SelectedGame = _currentNode;
            return;
          }
        }
    }

    public virtual void EndSearch()
    {
      _currentNode = null;
      _searchString = "";
    }

    public Game FindGameByName(string name, out int count)
    {
      int i = 0;

      if (!String.IsNullOrEmpty(name))
      {
        Game node = null;
        node = _gameList.FirstGame;

        foreach (string label in IterateNodeLabels(node, SearchType.Name))
        {
          i++;
          if (label.Equals(name))
          {
            break;
          }
          node = node.NextGame;
        }
      }
      count = i - 1;
      return _currentNode;
    }

    public Game FindInsertionPoint(string name)
    {
      Game lastNode = null;
      if (!String.IsNullOrEmpty(name))
      {
        Game node = null;

        node = _gameList.FirstGame;

        foreach (string label in IterateNodeLabels(node, SearchType.Description))
        {
          if (!node.IsFavorite)
            return lastNode;

          if (label.CompareTo(name) > 0)
          {
            break;
          }
          node = node.NextGame;
        }
      }

      return _currentNode;
    }

    public Game FindGameByRow(int row)
    {
      int i = 0;
      Game node = null;
      node = _gameList.FirstGame;

      foreach (string label in IterateNodeLabels(node, SearchType.Name))
      {
        if (i == row)
          break;
        i++;
        node = node.NextGame;
      }

      return _currentNode;
    }

    public Game FindLastFavorite()
    {
      Game node = null;

      node = _gameList.FirstGame;

      foreach (string label in IterateNodeLabels(node, SearchType.Name))
      {
        if (!node.IsFavorite)
          break;

        node = node.PreviousGame;
      }

      return node;
    }

    protected IEnumerable<string> IterateNodeLabels(Game start, SearchType type)
    {
      iterations = 0;
      _currentNode = start;
      while (_currentNode != null)
      {
        foreach (string label in GetNodeLabels(_currentNode, type))
        {
          iterations++;
          yield return label;
        }
        _currentNode = _currentNode.NextGame;
        if (_currentNode == null)
          _currentNode = _gameList.FirstGame;

        if (start == _currentNode)
          break;
      }
    }


    private IEnumerable<string> GetNodeLabels(Game node, SearchType type)
    {
      object obj = null;
      switch (type)
      {
        case SearchType.Name:
          obj = node.Name;
          break;
        case SearchType.Description:
          obj = node.Description;
          break;
        case SearchType.Driver:
          obj = node.SourceFile;
          break;
        case SearchType.Year:
          obj = node.Year;
          break;
      }

      if (obj != null)
        yield return obj.ToString().ToLowerInvariant();
    }

    private bool DoContinuousSearch()
    {
      bool found = false;
      if (!String.IsNullOrEmpty(_searchString))
      {
        Game node = null;
        if (_gameList.SelectedGame != null)
          node = _gameList.SelectedGame;
        if (node == null)
          node = _gameList.NextVisibleGame; // next visible

        if (!String.IsNullOrEmpty(_searchString))
        {
          foreach (string label in IterateNodeLabels(node, SearchType.Description))
          {
            if (label.StartsWith(_searchString))
            {
              found = true;
              _gameList.SelectedGame = _currentNode;
              break;
            }
          }
        }
      }
      return found;
    }

  }
}
