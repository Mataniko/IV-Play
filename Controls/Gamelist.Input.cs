#region

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using IV_Play.Properties;

#endregion

namespace IV_Play
{
    internal partial class GameList
    {
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            SuspendLayout();
            Invalidate();
            try
            {
                if (SettingsManager.ArtPaths[ArtType].Contains(".dat", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ControlKeyPressed || _imageArea.Contains(e.Location))
                    {
                        if (e.Delta > 0)
                            ScrollInfoText(-5, InfoScrollMode.Line);
                        else
                            ScrollInfoText(5, InfoScrollMode.Line);
                    }
                    else
                    {
                        base.OnMouseWheel(e);
                    }
                    ResumeLayout();
                    return;
                }
                
                base.OnMouseWheel(e);
                ResumeLayout();
            }
            catch
            {
                ResumeLayout();          
            }

            
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (Count == 0)
                return;

            try
            {
                if (_imageArea.Contains(e.Location))
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            if (ArtType == SettingsManager.ArtPaths.Count - 1)
                                Settings.Default.art_type = 0;
                            else
                                Settings.Default.art_type++;

                            ArtType = Settings.Default.art_type;
                            break;
                        case MouseButtons.Right:
                            if (
                                !SettingsManager.ArtPaths[ArtType].Contains(".dat",
                                                                            StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (Settings.Default.art_area == (int) ArtDisplayMode.superlarge)
                                    Settings.Default.art_area = (int) ArtDisplayMode.normal;
                                else
                                    Settings.Default.art_area++;
                                ArtDisplayMode = (ArtDisplayMode) Settings.Default.art_area;
                            }
                            break;
                    }
                    RefreshImage();
                    Invalidate();
                }
                else
                {
                    int row = (e.Y + VerticalScroll.Value)/RowHeight;
                    Game game = GetNodeAtRow(row);
                    if (game == null)
                        return;

                    Graphics g = CreateGraphics();
                    SizeF strLen = g.MeasureString(GetRowText(game), Font);
                    int offset = game.IsParent ? SmallIconSize.Width + OffsetParent : SmallIconSize.Width + OffsetChild;
                    if (e.X <= strLen.Width + offset)
                    {
                        SelectedGame = game;
                    }
                    else
                    {
                        SelectedGame = null;
                        _bgImage = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex);
            }


            base.OnMouseClick(e);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (Count == 0)
                return;
            try
            {
                if (SelectedGame == null)
                    base.OnMouseDoubleClick(e);
                else
                {
                    int row = (e.Y + VerticalScroll.Value)/RowHeight;
                    if (row == SelectedRow && e.Button == MouseButtons.Left)
                    {
                        Graphics g = CreateGraphics();
                        SizeF strLen = g.MeasureString(GetRowText(SelectedGame), Font);
                        int offset = SelectedGame.IsParent
                                         ? SmallIconSize.Width + OffsetParent
                                         : SmallIconSize.Width + OffsetChild;
                        if (e.X <= strLen.Width + offset)
                            StartGame();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Control)
                ControlKeyPressed = false;

            if (e.Handled) return;

            if (!Settings.Default.arrow_key_paging)
            {
                if (e.KeyCode == Keys.Left)
                {
                    GoToPreviousCharacter();
                    e.Handled = true;
                }

                if (e.KeyCode == Keys.Right)
                {
                    GoToNextLetter();
                    e.Handled = true;
                }
            }
        }

        private void ScrollInfoText(int rows, InfoScrollMode scrollMode)
        {
            SuspendLayout();
            if (_currentInfo == null)
                return;


            switch (scrollMode)
            {
                case InfoScrollMode.Line:
                    if (rows > 0 && _currentInfo.TotalRows <= _currentInfo.VisibleRows)
                        return;

                    infoRow += rows;
                    break;
                case InfoScrollMode.Page:
                    rows = _currentInfo.VisibleRows*rows;
                    if (rows > 0 && _currentInfo.TotalRows <= _currentInfo.VisibleRows)
                        return;

                    infoRow += rows;
                    break;
                case InfoScrollMode.All:
                    if (rows < 0)
                    {
                        infoRow = 0;
                    }
                    else
                    {
                        int i = _currentInfo.TotalRows;
                        while (i > _currentInfo.VisibleRows)
                        {
                            infoRow += _currentInfo.VisibleRows;
                            i = GetMaxRows(GetShortInfoText());
                        }
                    }
                    break;
            }
            ResumeLayout();
            infoRow = Math.Max(infoRow, 0);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control)
                ControlKeyPressed = true;
            Invalidate();


            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.R:
                    if (e.Control)
                    {
                        Random rand = new Random(DateTime.Now.Millisecond);
                        int iRand = rand.Next(0, Games.Count);
                        SelectedGame = GetNodeAtRow(iRand);
                    }                
                    break;
                case Keys.Back:
                        if (e.Control && Filter.Length != 0)
                            Filter="";
                    break;
                case Keys.Down:
                    if (e.Control)
                        ScrollInfoText(3, InfoScrollMode.Line);
                    else
                        NavigateForward(1);
                    break;
                case Keys.Up:
                    if (e.Control)
                        ScrollInfoText(-3, InfoScrollMode.Line);
                    else
                        NavigateBackward(1);
                    break;
                case Keys.PageDown:
                case Keys.Right:
                    if (e.KeyCode == Keys.PageDown || (e.KeyCode == Keys.Right && Settings.Default.arrow_key_paging))
                    {
                        if (e.Control)
                            ScrollInfoText(1, InfoScrollMode.Page);
                        else
                            NavigateForward(ClientRectangle.Height / RowHeight);
                    }
                    break;
                case Keys.PageUp:
                case Keys.Left:
                    if (e.KeyCode == Keys.PageUp || (e.KeyCode == Keys.Left && Settings.Default.arrow_key_paging))
                    {
                        if (e.Control)
                            ScrollInfoText(-1, InfoScrollMode.Page);
                        else
                            NavigateBackward(ClientRectangle.Height / RowHeight);
                    }
                    break;
                case Keys.Home:
                    if (e.Control)
                        ScrollInfoText(-1, InfoScrollMode.All);
                    else if (SelectedGame != null && FavoritesMode == FavoritesMode.FavoritesAndGames)
                    {
                        Game lastFavorite = GetNodeAtRow(CountFavorites);
                        if (SelectedGame.IsFavorite || SelectedGame == lastFavorite)
                            SelectedGame = Games.First().Value;
                        else
                            SelectedGame = lastFavorite;
                    }
                    else
                    {
                        SelectedGame = Games.First().Value;
                    }
                    break;
                case Keys.End:
                    if (e.Control)
                        ScrollInfoText(1, InfoScrollMode.All);
                    else if (SelectedGame != null && FavoritesMode == FavoritesMode.FavoritesAndGames)
                    {
                        Game lastFavorite = GetNodeAtRow(CountFavorites - 1);
                        if (!SelectedGame.IsFavorite || SelectedGame == lastFavorite)
                            SelectedGame = Games.Last().Value;
                        else
                            SelectedGame = lastFavorite;
                    }
                    else
                        SelectedGame = Games.Last().Value;
                    break;

                case Keys.Enter:
                case Keys.D1:
                    if (Focused)
                        StartGame();
                    break;

                case Keys.Escape:
                    Application.Exit();
                    break;

                case Keys.D0:
                //case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    if (e.Alt)
                    {
                        int artType = e.KeyValue - (int) Keys.D0;
                        if (artType < SettingsManager.ArtPaths.Count)
                        {
                            Settings.Default.art_type = artType;
                            ArtType = Settings.Default.art_type;
                            RefreshImage();
                        }
                        e.SuppressKeyPress = true;
                    }
                    break;
            }
        }


        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            try
            {
                if (e.Handled || !Settings.Default.filter_search) return;

                if (Settings.Default.filter_on_input)
                {
                    if (e.KeyChar == '\b' && !ControlKeyPressed)
                    {                        
                        if (Filter.Length != 0)
                            Filter = Filter.Remove(Filter.Length - 1);
                    }
                    else if (!char.IsControl(e.KeyChar))
                        Filter += e.KeyChar;
                }
                else
                    _search.Search(e.KeyChar);
            }
            catch
            {
            }
        }
    }
}