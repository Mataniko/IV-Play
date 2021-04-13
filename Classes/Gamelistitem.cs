#region

using System;
using System.Drawing;

#endregion

namespace IV_Play
{
  [Serializable]
  public class GameListItem
  {
    public int Index { get; set; }
    public bool IsSelected { get; set; }
    public Game NextGame { get; set; }
    public Game PreviousGame { get; set; }
    public Image Snap { get; set; }
    public Rectangle RowRectangle { get; set; }
  }
}
