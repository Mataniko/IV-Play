#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

using IV_Play.Properties;

#endregion

namespace IV_Play
{
    internal partial class GameList
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Games.Count > 0)
            {
                AutoScroll = true;
                AutoScrollMinSize = new Size(ClientRectangle.Width, Games.Count * RowHeight);

                Graphics g = e.Graphics;

                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);

                DrawHighlight(g);
                int startX = Math.Abs(AutoScrollPosition.Y);

                int endX = (startX + ClientSize.Height);
                endX = Math.Min(endX + RowHeight, Games.Count * RowHeight);
                startX = startX / RowHeight;
                endX = endX / RowHeight;

                bool paintingSelected = SelectedGame != null ? true : false;

                for (int j = startX; j <= endX; j++)
                {
                    if (j >= Games.Count) continue;

                    Game game = GetNodeAtRow(j);
                    if (j != SelectedRow)
                    {
                        if (game.IsParent || game.IsFavorite || (!game.IsParent && !Settings.Default.allow_grouping))
                            DrawRow(j, OffsetParent, game, g);
                        else
                            DrawRow(j, OffsetChild, game, g);
                    }
                    else
                    {
                        paintingSelected = true;
                    }
                }
                if (paintingSelected && SelectedGame != null)
                {
                    if (SelectedGame.IsParent || SelectedGame.IsFavorite)
                        DrawRow(SelectedRow, OffsetParent, SelectedGame, g);
                    else
                        DrawRow(SelectedRow, OffsetChild, SelectedGame, g);
                }
            }
            else
                base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!DesignMode)
            {
                Graphics formGraphics = e.Graphics;
                if (BackgroundImage == null)
                    BackgroundImage = Resources.Default_Background;
                else
                {
                    using (TextureBrush texture = new TextureBrush(BackgroundImage) { WrapMode = Settings.Default.background_repeat ? WrapMode.Tile : WrapMode.Clamp })
                    {
                        formGraphics.CompositingQuality = CompositingQuality.HighSpeed;

                        formGraphics.FillRectangle(texture, e.ClipRectangle);
                    }
                }
                //if (!_keyIsDown)
                DrawBackground(formGraphics);
            }
            else
                base.OnPaintBackground(e);
        }

        private void DrawHighlight(Graphics g)
        {
            try
            {
                if (SelectedGame != null)
                {
                    int row = SelectedRow;
                    Game game = GetNodeAtRow(row);

                    if (game == null)
                        return;

                    string rowText = GetRowText(game);

                    int offset = game.IsParent || game.IsFavorite ? OffsetParent : OffsetChild;
                    rowText = ValidateRowLength(rowText, offset);

                    SizeF sizeF = g.MeasureString(rowText, Font);
                    using (Pen borderPen = new Pen(Color.Blue) { DashStyle = DashStyle.Dot })
                    {
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, Color.LightBlue)))
                        {
                            if (game.IsParent || game.IsFavorite)
                            {
                                g.DrawRectangle(borderPen, OffsetParent + SmallIconSize.Width + 3,
                                                row * Math.Max(Font.Height, RowHeight), sizeF.Width, sizeF.Height);
                                g.FillRectangle(brush, OffsetParent + SmallIconSize.Width + 3,
                                                row * Math.Max(Font.Height, RowHeight), sizeF.Width, sizeF.Height);
                            }
                            else
                            {
                                g.DrawRectangle(borderPen, OffsetChild + SmallIconSize.Width + 3,
                                                row * Math.Max(Font.Height, RowHeight), sizeF.Width, sizeF.Height);
                                g.FillRectangle(brush, OffsetChild + SmallIconSize.Width + 3,
                                                row * Math.Max(Font.Height, RowHeight), sizeF.Width, sizeF.Height);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void DrawRow(int row, int offset, Game game, Graphics g)
        {
            //Draw Icon
            FetchIcon(game);
            try
            {
                if (row == SelectedRow && Settings.Default.large_icon && SelectedGame != null)
                {
                    int lastrow = VerticalScroll.Maximum / RowHeight == row ? LargeIconSize.Height / 2 : 0;
                    g.DrawImage(game.Icon, offset - LargeIconSize.Width / 2,
                                RowHeight * row - lastrow, LargeIconSize.Width,
                                LargeIconSize.Height);
                }
                else
                    g.DrawImage(game.Icon, offset, RowHeight * row, SmallIconSize.Width,
                                SmallIconSize.Height);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex);
            }

            //Draw Text
            string RowText = GetRowText(game);
            RowText = ValidateRowLength(RowText, offset);

            game.RowRectangle = new Rectangle(0, RowHeight * row - RowHeight, ClientRectangle.Width,
                                              RowHeight * 2);

            if (game.IsFavorite)
                g.DrawString(RowText, Font, new SolidBrush(FavoriteColor), offset + SmallIconSize.Width + 1,
                             RowHeight * row);
            else if (game.IsParent)
                g.DrawString(RowText, Font, new SolidBrush(ParentColor), offset + SmallIconSize.Width + 1,
                             RowHeight * row);
            else
            {
                g.DrawString(RowText, Font, new SolidBrush(CloneColor), offset + SmallIconSize.Width + 1,
                             RowHeight * row);
            }
        }

        private void DrawTextInfo(Graphics g)
        {
            if (SelectedGame == null)
                return;

            string gameName = SelectedGame.IsParent ? SelectedGame.Name : SelectedGame.ParentSet;

            int maxRows = 0;
            int chars;
            Info info = new Info();
            string text = "";
            StringFormat stringFormat = new StringFormat();


            //Calculate the Area to draw the text in.
            SizeF size = new SizeF((float)ClientRectangle.Width / 2, 100000);

            RectangleF rectangleF = new RectangleF(((float)ClientRectangle.Width / 2), ArtOffset,
                                                   ((float)ClientRectangle.Width / 2) - ArtOffset,
                                                   ClientRectangle.Height - ArtOffset * 2);


            //Find out if we are reading MameInfo or History
            string textType = Path.GetFileNameWithoutExtension(SettingsManager.ArtPaths[ArtType]);
            if (textType.Equals("mameinfo", StringComparison.InvariantCultureIgnoreCase))
            {
                if (MameInfo == null)
                    MameInfo = new InfoParser(SettingsManager.ArtPaths[ArtType]);

                if (!MameInfo.Contains(gameName.Replace("fav_", "")))
                    return;

                info = MameInfo[gameName.Replace("fav_", "")];
            }
            else if (textType.Equals("history", StringComparison.InvariantCultureIgnoreCase))
            {
                if (History == null)
                    History = new InfoParser(SettingsManager.ArtPaths[ArtType]);

                if (!History.Contains(gameName.Replace("fav_", "")))
                    return;

                info = History[gameName.Replace("fav_", "")];
            }
            else
                return;

            _currentInfo = info;


            text = GetShortInfoText();
            _currentInfoText = text;


            g.MeasureString(text, Settings.Default.info_font, size, stringFormat, out chars, out maxRows);


            g.DrawString(text, Settings.Default.info_font, new SolidBrush(Settings.Default.info_font_color), rectangleF,
                         stringFormat);

            _imageArea = new Rectangle((int)rectangleF.X, (int)rectangleF.Y, (int)rectangleF.Width,
                                       (int)rectangleF.Height);

            info.TotalRows = maxRows;
            info.VisibleRows = GetVisibleRows(_currentInfoText);
        }

        private void DrawBackground(Graphics g)
        {
            if (_bgImage != null)
            {
                _currentInfo = null;
                //Sets the transparency for our art
                ImageAttributes ia = CreateImageAttributes();
                g.CompositingMode = CompositingMode.SourceOver;

                int width;
                int height;
                int border = BorderWidth;
                Rectangle borderRectangle = new Rectangle();

                using (Pen borderPen = new Pen(Color.FromArgb(255, BorderColor), border))
                {
                    SetDisplayModeByImageSize();

                    using (Bitmap bmp = new Bitmap(1, 1))
                    {
                        int halfBorder = border / 2 + border % 2;

                        switch (ArtDisplayMode)
                        {
                            case ArtDisplayMode.normal:
                                borderRectangle =
                                    new Rectangle(ClientRectangle.Width - (_bgImage.Width + ArtOffset + border * 2 + halfBorder),
                                                  ArtOffset + halfBorder, _bgImage.Width + border * 2, _bgImage.Height + border * 2);
                                break;
                            case ArtDisplayMode.vertical:
                                //First we get the height and width of the new image
                                height = ClientRectangle.Height - ArtOffset * 2 - border * 2;
                                width = Convert.ToInt32((height / (float)_bgImage.Height) * _bgImage.Width);
                                borderRectangle =
                                    new Rectangle(ClientRectangle.Width - (width + ArtOffset + border + halfBorder),
                                                  ArtOffset + halfBorder, width + border, height + border);
                                break;
                            case ArtDisplayMode.superlarge:
                                // Here we have to calculate the aspect ratio
                                float aspectRatio = _bgImage.Width / (float)_bgImage.Height;
                                width = Convert.ToInt32((ClientRectangle.Width) * 0.66);
                                height = Convert.ToInt32(width / aspectRatio);
                                borderRectangle = new Rectangle(Convert.ToInt32(ClientRectangle.Width - width - ArtOffset),
                                                                ArtOffset,
                                                                width, height);
                                break;
                        }
                    }

                    // Now that we know where to put our image and how wide the border is, we can draw the image in the middle.
                    if (borderRectangle.Width > 0 && borderRectangle.Height > 0)
                    {
                        using (Bitmap bmp = new Bitmap(borderRectangle.Width, borderRectangle.Height))
                        {
                            Graphics bg = Graphics.FromImage(bmp);
                            bg.Clear(borderPen.Color);
                            bg.DrawImage(_bgImage, border, border, borderRectangle.Width - border * 2,
                                         borderRectangle.Height - border * 2);

                            //Don't interpolate in normal mode, this makes things look nice.
                            if (ArtDisplayMode == ArtDisplayMode.normal)
                            {
                                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                                bg.InterpolationMode = InterpolationMode.NearestNeighbor;
                            }

                            g.DrawImage(bmp, borderRectangle, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);


                            //reset the ArtDisplayMode
                            ArtDisplayMode = (ArtDisplayMode)Settings.Default.art_area;

                            // We use the image area to detect where the mouse was clicked.
                            _imageArea = borderRectangle;
                        }
                    }
                }
            }
            else if (SettingsManager.ArtPaths.Count > 0) //Draw Text.
            {
                try
                {
                    if (SettingsManager.ArtPaths[Math.Min(ArtType, SettingsManager.ArtPaths.Count - 1)].Contains(
                        ".dat", StringComparison.InvariantCultureIgnoreCase))
                        DrawTextInfo(g);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Adjusts the ArtDisplayMode so that images will always fit, even if they are not of standard size.
        /// </summary>
        private void SetDisplayModeByImageSize()
        {
            //If the image is too tall, draw it in vertical stretch mode
            if (_bgImage.Height + BorderWidth * 2 > ClientRectangle.Height && ArtDisplayMode == ArtDisplayMode.normal)
                ArtDisplayMode = ArtDisplayMode.vertical;

            //If the image is too wide, draw it in superlarge mode.
            if (ArtDisplayMode == ArtDisplayMode.vertical)
            {
                int width = Convert.ToInt32(((ClientRectangle.Height - ArtOffset * 2 - BorderWidth * 2) / (float)_bgImage.Height) *
                                            _bgImage.Width);
                if (width > Convert.ToInt32((ClientRectangle.Width) * 0.66))
                    ArtDisplayMode = ArtDisplayMode.superlarge;
            }
        }

        /// <summary>
        /// Calculate the transperancy
        /// </summary>
        /// <returns>ImageAttributes with the correct transparency values.</returns>
        private ImageAttributes CreateImageAttributes()
        {
            ColorMatrix cm = new ColorMatrix { Matrix33 = (float)ArtTransperancy / 100 };
            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(cm);
            return ia;
        }


        private string GetShortInfoText()
        {
            StringReader stringReader = new StringReader(_currentInfo.Text);

            for (int i = 0; i < infoRow; i++)
            {
                stringReader.ReadLine();
            }

            return stringReader.ReadToEnd();
        }

        private int GetVisibleRows(string text)
        {
            Graphics g = CreateGraphics();
            int chars = 0;
            int visibleRows = 0;
            g.MeasureString(text, Settings.Default.info_font, _imageArea.Size, new StringFormat(), out chars,
                            out visibleRows);
            return visibleRows;
        }

        private int GetMaxRows(string text)
        {
            Graphics g = CreateGraphics();
            int chars = 0;
            int visibleRows = 0;
            g.MeasureString(text, Settings.Default.info_font, new SizeF(_imageArea.Width, 100000), new StringFormat(),
                            out chars, out visibleRows);
            return visibleRows;
        }


        /// <summary>
        /// Returns what text we should draw depending on the display mode.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private string GetRowText(Game game)
        {
            string RowText = "";
            switch (DisplayMode)
            {
                case DisplayModeEnum.Description:
                    RowText = game.Description;
                    break;
                case DisplayModeEnum.DescriptionAndYear:
                    RowText = game.Description + " " + game.Year;
                    break;
                case DisplayModeEnum.DescriptionAndManufacturer:
                    RowText = game.Description + " " + game.Manufacturer;
                    break;
                case DisplayModeEnum.DescriptionYearAndManufacturer:
                    RowText = game.Description + " " + game.Year + " " + game.Manufacturer;
                    break;
            }

            return RowText;
        }


        private string ValidateRowLength(string text, int offset)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            Graphics g = CreateGraphics();
            float textWidth = g.MeasureString(text, Font).Width;
            bool trimmed = false;
            while (textWidth + offset + ArtOffset > ((float)ClientRectangle.Width / 2))
            {
                trimmed = true;
                text = text.Remove(text.Length - 1, 1);
                textWidth = g.MeasureString(text, Font).Width;
            }

            if (trimmed && text.Length > 3)
            {
                text = text.Remove(text.Length - 3, 3);
                text = text + "...";
            }
            return text;
        }
    }
}