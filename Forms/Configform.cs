#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using IV_Play.Properties;

#endregion

//using System.Linq;

namespace IV_Play
{
    /// <summary>
    /// IV/Play configuration form
    /// </summary>
    public partial class ConfigForm : Form
    {
        private string _backgroundImage;
        private string _backgroundPath;
        private bool _bgImageChanged;
        private bool _initialBackgroundValue;
        private Font gameFont;
        private Font infoFont;
        private Color infoColor;
        private string lastDir = Environment.CurrentDirectory;
        private int[] _customColors = new int[16];

        public ConfigForm()
        {
            InitializeComponent();
        }

        public static event ConfigFormSaved ConfigSaved;


        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter && autoCompleteTextBox1.ListBoxOn)
                return true;

            return base.ProcessDialogKey(keyData);
        }
        private void _btnFont_Click(object sender, EventArgs e)
        {
            _fontDialog.Font = gameFont;

            if (_fontDialog.ShowDialog() == DialogResult.OK)
            {
                gameFont = _fontDialog.Font;
            }
            _btnOK.Select();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            if (SettingsManager.MameCommands != null)
            {
                autoCompleteTextBox1.Items = SettingsManager.MameCommands.Commands;
            }
            LoadSettings();
            _btnOK.Select();

        }

        private void LoadSettings()
        {
            _initialBackgroundValue = Settings.Default.rotate_background;
            _listArtViews.Items.Clear();

            //Art Views           
            foreach (var s in SettingsManager.ArtPaths)
            {
                if (s == "None")
                    continue;

                _listArtViews.Items.Add(s);
            }

            UpdateArtTypeDropDown();

            //Font
            gameFont = Settings.Default.game_list_font;
            infoFont = Settings.Default.info_font;
            infoColor = Settings.Default.info_font_color;
            _fontDialog.Font = Settings.Default.game_list_font;
            _fontDialog2.Font = Settings.Default.info_font;
            _fontDialog2.Color = Settings.Default.info_font_color;

            //Command line
            autoCompleteTextBox1.Text = Settings.Default.command_line_switches;

            //Colors
            _colorPickerParent.Color = Settings.Default.game_list_color;
            //_colorPickerParent.CustomColors = _customColors;
            _colorPickerClone.Color = Settings.Default.game_list_clone_color;
            //_colorPickerClone.CustomColors = _customColors;
            _colorPickerFav.Color = Settings.Default.favorites_color;
            //_colorPickerFav.CustomColors = _customColors;
            _colorPickerBorder.Color = Settings.Default.art_border_color;
            //_colorPickerBorder.CustomColors = _customColors;

            //Enums
            _cmbArtView.SelectedIndex = Settings.Default.art_area;
            _cmbArtType.SelectedIndex = Settings.Default.art_type;

            //Checkboxes
            _chkManu.Checked = Settings.Default.GameListManufacturer;
            _chkYear.Checked = Settings.Default.GameListYear;
            _chkRotateBackground.Checked = Settings.Default.rotate_background;
            _chkItemZoom.Checked = Settings.Default.large_icon;
            _chkArtNonWorking.Checked = Settings.Default.non_working_overlay;
            _chkHideNonWorkingMechanical.Checked = Settings.Default.hide_mechanical_games;
            _chkHideNonWorking.Checked = Settings.Default.hide_nonworking;
            _chkHideClones.Checked = Settings.Default.hide_clones;
            _chkInputFilter.Checked = Settings.Default.filter_on_input;
            _chkShowError.Checked = Settings.Default.show_error;
            _chkFilterSearch.Checked = Settings.Default.filter_search;
            _chkAuditGames.Checked = Settings.Default.audit_games;
            _chkAllowGrouping.Checked = Settings.Default.allow_grouping;
            _chkFullscreen.Checked = Settings.Default.full_screen;
            _chkBackgroundRepeat.Checked = Settings.Default.background_repeat;
            _chkArrowKeyPaging.Checked = Settings.Default.arrow_key_paging;

            //Favorites
            _cmdFavMode.SelectedIndex = Settings.Default.favorites_mode;

            //BorderUpDown
            _borderWidth.Value = Settings.Default.art_border_width;

            //Opacity
            _trackBarOpacity.Value = Settings.Default.art_opacity;
            _lblOpacityPercentage.Text = _trackBarOpacity.Value + "%";

            //Background
            _backgroundImage = Settings.Default.bkground_image;
            _backgroundPath = Settings.Default.bkground_directory;
            try
            {
                _picBG.Image =
                    Image.FromFile(Path.Combine(Settings.Default.bkground_directory, Settings.Default.bkground_image));
            }
            catch (Exception)
            {
                _picBG.Image = Resources.Default_Background;
            }
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            bool reloadGames = _chkAuditGames.Checked != Settings.Default.audit_games || _chkAllowGrouping.Checked != Settings.Default.allow_grouping;
            bool reloadAudit = _chkAuditGames.Checked && _chkAuditGames.Checked != Settings.Default.audit_games;

            SaveSettings();
            UpdateWindow(reloadGames, reloadAudit);

            Close();
        }

        private void _btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _btnApply_Click(object sender, EventArgs e)
        {
            bool reloadGames = _chkAuditGames.Checked != Settings.Default.audit_games || _chkAllowGrouping.Checked != Settings.Default.allow_grouping;
            bool reloadAudit = _chkAuditGames.Checked && _chkAuditGames.Checked != Settings.Default.audit_games;

            SaveSettings();
            UpdateWindow(reloadGames, reloadAudit);
        }

        private void _picBG_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Images|*.png;*.bmp;*.gif;*.jpg";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        _picBG.Image = Image.FromFile(openFileDialog.FileName);
                        _backgroundImage = openFileDialog.SafeFileName;
                        _backgroundPath = openFileDialog.FileName.Replace(openFileDialog.SafeFileName, "");
                        SettingsManager.BackgroundImage = _picBG.Image;
                        _bgImageChanged = true;
                    }
                }
                _btnOK.Select();
            }
            catch
            {
                MessageBox.Show("Invalid image selected");
            }
        }

        private void _trackBarOpacity_Scroll(object sender, EventArgs e)
        {
            _lblOpacityPercentage.Text = _trackBarOpacity.Value + "%";
        }

        private void SaveSettings()
        {
            //Enums
            Settings.Default.art_area = _cmbArtView.SelectedIndex;
            Settings.Default.art_type = _cmbArtType.SelectedIndex;

            //Checkboxes
            Settings.Default.GameListManufacturer = _chkManu.Checked;
            Settings.Default.GameListYear = _chkYear.Checked;
            Settings.Default.rotate_background = _chkRotateBackground.Checked;
            Settings.Default.large_icon = _chkItemZoom.Checked;
            Settings.Default.non_working_overlay = _chkArtNonWorking.Checked;
            Settings.Default.hide_mechanical_games = _chkHideNonWorkingMechanical.Checked;
            Settings.Default.hide_nonworking = _chkHideNonWorking.Checked;
            Settings.Default.hide_clones = _chkHideClones.Checked;
            Settings.Default.filter_on_input = _chkInputFilter.Checked;
            Settings.Default.show_error = _chkShowError.Checked;
            Settings.Default.filter_search = _chkFilterSearch.Checked;
            Settings.Default.audit_games = _chkAuditGames.Checked;
            Settings.Default.allow_grouping = _chkAllowGrouping.Checked;
            Settings.Default.full_screen = _chkFullscreen.Checked;
            Settings.Default.background_repeat = _chkBackgroundRepeat.Checked;
            Settings.Default.arrow_key_paging = _chkArrowKeyPaging.Checked;

            //Favorites
            Settings.Default.favorites_mode = _cmdFavMode.SelectedIndex;

            //Font
            Settings.Default.game_list_font = gameFont;
            Settings.Default.info_font = infoFont;

            //Command line
            Settings.Default.command_line_switches = autoCompleteTextBox1.Text;

            //Colors
            Settings.Default.info_font_color = infoColor;
            Settings.Default.game_list_color = _colorPickerParent.Color;
            Settings.Default.game_list_clone_color = _colorPickerClone.Color;
            Settings.Default.favorites_color = _colorPickerFav.Color;
            Settings.Default.art_border_color = _colorPickerBorder.Color;

            //BorderUpDown
            Settings.Default.art_border_width = Convert.ToInt32(_borderWidth.Value);

            //Opacity
            Settings.Default.art_opacity = _trackBarOpacity.Value;

            //Background
            if (_bgImageChanged)
            {
                Settings.Default.bkground_image = _backgroundImage;
                Settings.Default.bkground_directory = _backgroundPath;
            }

            //Logic to change background
            //1. Rotate initally off, turned on, but the background image wasn't changed. Change BG now
            if (!_initialBackgroundValue && !_bgImageChanged && _chkRotateBackground.Checked)
            {
                SettingsManager.GetBackgroundImage();
                _picBG.Image = SettingsManager.BackgroundImage;
                _initialBackgroundValue = true;
            }
            //2. Checkback was initially off, but user pressed on and off again several times WITHOUT
            // changing the background image - as that takes precedence.
            else if ((_initialBackgroundValue && !_bgImageChanged && !_chkRotateBackground.Checked))
                _initialBackgroundValue = false;


            //Art View Paths
            string Paths = "";
            SettingsManager.ArtPaths = new List<string>();
            SettingsManager.ArtPaths.Add("None");

            foreach (var item in _listArtViews.Items)
            {
                SettingsManager.ArtPaths.Add(item.ToString());
                Paths += item.ToString() + '|';
            }
            Paths.TrimEnd('|');
            Settings.Default.art_view_folders = Paths;

            SettingsManager.WriteSettingsToFile();

            if (ConfigSaved != null)
                ConfigSaved(null, null);
        }

        private void _btnMoveUp_Click(object sender, EventArgs e)
        {
            if (_listArtViews.SelectedItem == null)
                return;

            string tmpFolder = _listArtViews.SelectedItem.ToString();
            int index = Math.Max(0, _listArtViews.SelectedIndex - 1);
            _listArtViews.Items[_listArtViews.SelectedIndex] = _listArtViews.Items[index];
            _listArtViews.Items[index] = tmpFolder;
            _listArtViews.SelectedIndex = index;

            UpdateArtTypeDropDown();
        }

        private void _btnMoveDown_Click(object sender, EventArgs e)
        {
            if (_listArtViews.SelectedItem == null)
                return;

            string tmpFolder = _listArtViews.SelectedItem.ToString();
            int index = Math.Min(_listArtViews.Items.Count - 1, _listArtViews.SelectedIndex + 1);
            _listArtViews.Items[_listArtViews.SelectedIndex] = _listArtViews.Items[index];
            _listArtViews.Items[index] = tmpFolder;
            _listArtViews.SelectedIndex = index;

            UpdateArtTypeDropDown();
        }

        private void _btnRemoveView_Click(object sender, EventArgs e)
        {
            if (_listArtViews.SelectedItem == null)
                return;

            if (_listArtViews.SelectedIndex == _cmbArtType.SelectedIndex - 1)
                _cmbArtType.SelectedIndex = 0;

            int index = Math.Max(0, _listArtViews.SelectedIndex - 1);
            _listArtViews.Items.RemoveAt(_listArtViews.SelectedIndex);
            if (_listArtViews.Items.Count > 0)
                _listArtViews.SelectedIndex = index;

            UpdateArtTypeDropDown();
        }

        private void _btnAddArtView_Click(object sender, EventArgs e)
        {
            try
            {


                _folderDialog.SelectedPath = lastDir;
                if (_folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    lastDir = _folderDialog.SelectedPath;
                    if (!_listArtViews.Items.Contains(lastDir.ToLower()))
                        _listArtViews.Items.Add(lastDir.ToLower());
                }
                UpdateArtTypeDropDown();
                _btnOK.Select();
            }
            catch (Exception)
            {


            }
        }


        private void UpdateArtTypeDropDown()
        {
            object prevArtType = _cmbArtType.SelectedItem;
            _cmbArtType.Items.Clear();
            _cmbArtType.Items.Add("None");
            foreach (var item in _listArtViews.Items)
            {
                string itemName = Path.GetFileNameWithoutExtension(item.ToString()).ToLower();
                itemName = itemName.Insert(1, Char.ToUpper(itemName[0]).ToString());
                itemName = itemName.Remove(0, 1);
                _cmbArtType.Items.Add(itemName);
            }
            _cmbArtType.SelectedItem = prevArtType;
        }
        private void _listArtViews_DragOver(object sender, DragEventArgs e)
        {
            //e.Effect = DragDropEffects.Copy;

        }

        private void _listArtViews_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void _listArtViews_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileDrop = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var path in fileDrop)
                {
                    // get the file attributes for file or directory
                    FileAttributes attr = File.GetAttributes(path);

                    //detect whether its a directory or file   
                    string filePath;

                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory || (Path.GetExtension(path) == ".dat"))
                    {
                        filePath = path.ToLower();
                    }
                    else
                        filePath = Path.GetDirectoryName(path);

                    if (!_listArtViews.Items.Contains(filePath.ToLower()))
                    {
                        _listArtViews.Items.Add(filePath.ToLower());
                    }
                }
                UpdateArtTypeDropDown();
            }
        }

        private void _btnLoadDat_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "MAME Information DAT (History, Info, Command)|*.dat";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!_listArtViews.Items.Contains(openFileDialog.FileName.ToLower()))
                        _listArtViews.Items.Add(openFileDialog.FileName.ToLower());
                }
            }
            _btnOK.Select();
        }

        private void _btnDatFont_Click(object sender, EventArgs e)
        {
            try
            {
                _fontDialog2.Font = infoFont;
                _fontDialog2.Color = infoColor;

                if (_fontDialog2.ShowDialog() == DialogResult.OK)
                {
                    infoFont = _fontDialog2.Font;
                    infoColor = _fontDialog2.Color;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex);
            }
            _btnOK.Select();
        }

        private void _btnReset_Click(object sender, EventArgs e)
        {
            SettingsManager.ResetSettings();
            LoadSettings();
        }

        private void _listArtViews_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && _listArtViews.SelectedIndex > 0)
                _btnRemoveView.PerformClick();

        }

        private void ConfigForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && autoCompleteTextBox1.ListBoxOn)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void UpdateWindow(bool reloadGames = false, bool reloadAudit = false)
        {
            Cursor.Current = Cursors.WaitCursor;

            // Toggle full-screen.
            if (Settings.Default.full_screen)
            {
                Owner.TopMost = true;
                Owner.FormBorderStyle = FormBorderStyle.None;
                Owner.Bounds = Screen.PrimaryScreen.Bounds;
            }
            else
            {
                Owner.TopMost = false;
                Owner.FormBorderStyle = FormBorderStyle.Sizable;
                Owner.Location = new Point(Settings.Default.Window_x, Settings.Default.Window_y);
                Owner.Size = new Size(Settings.Default.window_width, Settings.Default.window_height);
            }

            if (reloadAudit)
            {
                if (File.Exists("MAME_g.ini"))
                {
                    SettingsManager.ReadAuditFromFile("MAME_g.ini");
                }
                else
                {
                    MessageBox.Show("The game audit file MAME_g.ini was not found. This file can be generated by MameUI64 during a game audit.\n\nRun MameUI64.\nSelect File->Audit all games.\nCopy MAME_g.ini into the same folder as IV-Play.exe");
                }
            }

            Cursor.Current = Cursors.Default;
        }
    }
}
