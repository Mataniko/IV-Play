namespace IV_Play
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this._lblArtView = new System.Windows.Forms.Label();
            this._lblArtType = new System.Windows.Forms.Label();
            this._lblOpacity = new System.Windows.Forms.Label();
            this._chkRotateBackground = new System.Windows.Forms.CheckBox();
            this._chkManu = new System.Windows.Forms.CheckBox();
            this._chkItemZoom = new System.Windows.Forms.CheckBox();
            this._chkYear = new System.Windows.Forms.CheckBox();
            this._cmbArtType = new System.Windows.Forms.ComboBox();
            this._cmbArtView = new System.Windows.Forms.ComboBox();
            this._fontDialog = new System.Windows.Forms.FontDialog();
            this._groupGameList = new System.Windows.Forms.GroupBox();
            this._chkArrowKeyPaging = new System.Windows.Forms.CheckBox();
            this._chkFilterSearch = new System.Windows.Forms.CheckBox();
            this._chkBackgroundRepeat = new System.Windows.Forms.CheckBox();
            this._chkArtNonWorking = new System.Windows.Forms.CheckBox();
            this._chkAllowGrouping = new System.Windows.Forms.CheckBox();
            this._chkFullscreen = new System.Windows.Forms.CheckBox();
            this._chkShowError = new System.Windows.Forms.CheckBox();
            this._chkInputFilter = new System.Windows.Forms.CheckBox();
            this._cmdFavMode = new System.Windows.Forms.ComboBox();
            this._lblFavMode = new System.Windows.Forms.Label();
            this._lblFavColor = new System.Windows.Forms.Label();
            this._lblColorGame = new System.Windows.Forms.Label();
            this._btnGameFont = new System.Windows.Forms.Button();
            this._lblClonesColor = new System.Windows.Forms.Label();
            this._lblParentsColor = new System.Windows.Forms.Label();
            this._chkHideNonWorking = new System.Windows.Forms.CheckBox();
            this._chkAuditGames = new System.Windows.Forms.CheckBox();
            this._chkHideNonWorkingMechanical = new System.Windows.Forms.CheckBox();
            this._groupArt = new System.Windows.Forms.GroupBox();
            this._lblOpacityPercentage = new System.Windows.Forms.Label();
            this._picBG = new System.Windows.Forms.PictureBox();
            this._borderWidth = new System.Windows.Forms.NumericUpDown();
            this._lblBackground = new System.Windows.Forms.Label();
            this._lblBorderColor = new System.Windows.Forms.Label();
            this._lblBorderWidth = new System.Windows.Forms.Label();
            this._trackBarOpacity = new System.Windows.Forms.TrackBar();
            this._btnApply = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnOK = new System.Windows.Forms.Button();
            this._groupArtTypes = new System.Windows.Forms.GroupBox();
            this._btnDatFont = new System.Windows.Forms.Button();
            this._btnLoadDat = new System.Windows.Forms.Button();
            this._btnMoveDown = new System.Windows.Forms.Button();
            this._btnMoveUp = new System.Windows.Forms.Button();
            this._btnRemoveView = new System.Windows.Forms.Button();
            this._btnAddArtView = new System.Windows.Forms.Button();
            this._listArtViews = new System.Windows.Forms.ListBox();
            this._folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this._fontDialog2 = new System.Windows.Forms.FontDialog();
            this._btnReset = new System.Windows.Forms.Button();
            this._groupMame = new System.Windows.Forms.GroupBox();
            this._lblCommandLine = new System.Windows.Forms.Label();
            this._groupFilters = new System.Windows.Forms.GroupBox();
            this._chkHideClones = new System.Windows.Forms.CheckBox();
            this.autoCompleteTextBox1 = new IV_Play.AutoCompleteTextBox();
            this._colorPickerBorder = new IV_Play.ColorPicker();
            this._colorPickerFav = new IV_Play.ColorPicker();
            this._colorPickerParent = new IV_Play.ColorPicker();
            this._colorPickerClone = new IV_Play.ColorPicker();
            this._groupGameList.SuspendLayout();
            this._groupArt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._picBG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._borderWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._trackBarOpacity)).BeginInit();
            this._groupArtTypes.SuspendLayout();
            this._groupMame.SuspendLayout();
            this._groupFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lblArtView
            // 
            this._lblArtView.AutoSize = true;
            this._lblArtView.Location = new System.Drawing.Point(185, 16);
            this._lblArtView.Name = "_lblArtView";
            this._lblArtView.Size = new System.Drawing.Size(49, 13);
            this._lblArtView.TabIndex = 15;
            this._lblArtView.Text = "Art View:";
            // 
            // _lblArtType
            // 
            this._lblArtType.AutoSize = true;
            this._lblArtType.Location = new System.Drawing.Point(6, 16);
            this._lblArtType.Name = "_lblArtType";
            this._lblArtType.Size = new System.Drawing.Size(50, 13);
            this._lblArtType.TabIndex = 14;
            this._lblArtType.Text = "Art Type:";
            // 
            // _lblOpacity
            // 
            this._lblOpacity.AutoSize = true;
            this._lblOpacity.Location = new System.Drawing.Point(6, 62);
            this._lblOpacity.Name = "_lblOpacity";
            this._lblOpacity.Size = new System.Drawing.Size(62, 13);
            this._lblOpacity.TabIndex = 10;
            this._lblOpacity.Text = "Art Opacity:";
            // 
            // _chkRotateBackground
            // 
            this._chkRotateBackground.AutoSize = true;
            this._chkRotateBackground.Location = new System.Drawing.Point(184, 118);
            this._chkRotateBackground.Name = "_chkRotateBackground";
            this._chkRotateBackground.Size = new System.Drawing.Size(151, 17);
            this._chkRotateBackground.TabIndex = 7;
            this._chkRotateBackground.Text = "Rotate Background Image";
            this._chkRotateBackground.UseVisualStyleBackColor = true;
            // 
            // _chkManu
            // 
            this._chkManu.AutoSize = true;
            this._chkManu.Location = new System.Drawing.Point(10, 118);
            this._chkManu.Name = "_chkManu";
            this._chkManu.Size = new System.Drawing.Size(173, 17);
            this._chkManu.TabIndex = 6;
            this._chkManu.Text = "Show Manufacturer in Gamelist";
            this._chkManu.UseVisualStyleBackColor = true;
            // 
            // _chkItemZoom
            // 
            this._chkItemZoom.AutoSize = true;
            this._chkItemZoom.Location = new System.Drawing.Point(184, 96);
            this._chkItemZoom.Name = "_chkItemZoom";
            this._chkItemZoom.Size = new System.Drawing.Size(99, 17);
            this._chkItemZoom.TabIndex = 5;
            this._chkItemZoom.Text = "Use Icon Zoom";
            this._chkItemZoom.UseVisualStyleBackColor = true;
            // 
            // _chkYear
            // 
            this._chkYear.AutoSize = true;
            this._chkYear.Location = new System.Drawing.Point(10, 96);
            this._chkYear.Name = "_chkYear";
            this._chkYear.Size = new System.Drawing.Size(132, 17);
            this._chkYear.TabIndex = 4;
            this._chkYear.Text = "Show Year in Gamelist";
            this._chkYear.UseVisualStyleBackColor = true;
            // 
            // _cmbArtType
            // 
            this._cmbArtType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbArtType.FormattingEnabled = true;
            this._cmbArtType.Items.AddRange(new object[] {
            "None"});
            this._cmbArtType.Location = new System.Drawing.Point(9, 32);
            this._cmbArtType.Name = "_cmbArtType";
            this._cmbArtType.Size = new System.Drawing.Size(143, 21);
            this._cmbArtType.TabIndex = 0;
            // 
            // _cmbArtView
            // 
            this._cmbArtView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbArtView.FormattingEnabled = true;
            this._cmbArtView.Items.AddRange(new object[] {
            "Normal",
            "Vertical Stretch",
            "Super Large"});
            this._cmbArtView.Location = new System.Drawing.Point(188, 32);
            this._cmbArtView.Name = "_cmbArtView";
            this._cmbArtView.Size = new System.Drawing.Size(143, 21);
            this._cmbArtView.TabIndex = 1;
            // 
            // _fontDialog
            // 
            this._fontDialog.AllowScriptChange = false;
            // 
            // _groupGameList
            // 
            this._groupGameList.Controls.Add(this._chkArrowKeyPaging);
            this._groupGameList.Controls.Add(this._chkFilterSearch);
            this._groupGameList.Controls.Add(this._chkBackgroundRepeat);
            this._groupGameList.Controls.Add(this._chkArtNonWorking);
            this._groupGameList.Controls.Add(this._chkAllowGrouping);
            this._groupGameList.Controls.Add(this._chkFullscreen);
            this._groupGameList.Controls.Add(this._chkShowError);
            this._groupGameList.Controls.Add(this._chkInputFilter);
            this._groupGameList.Controls.Add(this._cmdFavMode);
            this._groupGameList.Controls.Add(this._lblFavMode);
            this._groupGameList.Controls.Add(this._lblFavColor);
            this._groupGameList.Controls.Add(this._lblColorGame);
            this._groupGameList.Controls.Add(this._chkManu);
            this._groupGameList.Controls.Add(this._chkRotateBackground);
            this._groupGameList.Controls.Add(this._colorPickerFav);
            this._groupGameList.Controls.Add(this._chkItemZoom);
            this._groupGameList.Controls.Add(this._btnGameFont);
            this._groupGameList.Controls.Add(this._chkYear);
            this._groupGameList.Controls.Add(this._colorPickerParent);
            this._groupGameList.Controls.Add(this._colorPickerClone);
            this._groupGameList.Controls.Add(this._lblClonesColor);
            this._groupGameList.Controls.Add(this._lblParentsColor);
            this._groupGameList.Location = new System.Drawing.Point(12, 12);
            this._groupGameList.Name = "_groupGameList";
            this._groupGameList.Size = new System.Drawing.Size(342, 285);
            this._groupGameList.TabIndex = 0;
            this._groupGameList.TabStop = false;
            this._groupGameList.Text = "Game List:";
            // 
            // _chkArrowKeyPaging
            // 
            this._chkArrowKeyPaging.AutoSize = true;
            this._chkArrowKeyPaging.Location = new System.Drawing.Point(184, 210);
            this._chkArrowKeyPaging.Name = "_chkArrowKeyPaging";
            this._chkArrowKeyPaging.Size = new System.Drawing.Size(140, 17);
            this._chkArrowKeyPaging.TabIndex = 44;
            this._chkArrowKeyPaging.Text = "Left/Right Arrow Paging";
            this._chkArrowKeyPaging.UseVisualStyleBackColor = true;
            // 
            // _chkFilterSearch
            // 
            this._chkFilterSearch.AutoSize = true;
            this._chkFilterSearch.Location = new System.Drawing.Point(10, 186);
            this._chkFilterSearch.Name = "_chkFilterSearch";
            this._chkFilterSearch.Size = new System.Drawing.Size(121, 17);
            this._chkFilterSearch.TabIndex = 45;
            this._chkFilterSearch.Text = "Enable Filter Search";
            this._chkFilterSearch.UseVisualStyleBackColor = true;
            // 
            // _chkBackgroundRepeat
            // 
            this._chkBackgroundRepeat.AutoSize = true;
            this._chkBackgroundRepeat.Location = new System.Drawing.Point(10, 164);
            this._chkBackgroundRepeat.Name = "_chkBackgroundRepeat";
            this._chkBackgroundRepeat.Size = new System.Drawing.Size(136, 17);
            this._chkBackgroundRepeat.TabIndex = 43;
            this._chkBackgroundRepeat.Text = "Tile Background Image";
            this._chkBackgroundRepeat.UseVisualStyleBackColor = true;
            // 
            // _chkArtNonWorking
            // 
            this._chkArtNonWorking.AutoSize = true;
            this._chkArtNonWorking.Location = new System.Drawing.Point(10, 209);
            this._chkArtNonWorking.Name = "_chkArtNonWorking";
            this._chkArtNonWorking.Size = new System.Drawing.Size(156, 17);
            this._chkArtNonWorking.TabIndex = 36;
            this._chkArtNonWorking.Text = "Draw Non-Working Overlay";
            this._chkArtNonWorking.UseVisualStyleBackColor = true;
            // 
            // _chkAllowGrouping
            // 
            this._chkAllowGrouping.AutoSize = true;
            this._chkAllowGrouping.Location = new System.Drawing.Point(184, 187);
            this._chkAllowGrouping.Name = "_chkAllowGrouping";
            this._chkAllowGrouping.Size = new System.Drawing.Size(147, 17);
            this._chkAllowGrouping.TabIndex = 42;
            this._chkAllowGrouping.Text = "Group Parent/Child Roms";
            this._chkAllowGrouping.UseVisualStyleBackColor = true;
            // 
            // _chkFullscreen
            // 
            this._chkFullscreen.AutoSize = true;
            this._chkFullscreen.Location = new System.Drawing.Point(184, 164);
            this._chkFullscreen.Name = "_chkFullscreen";
            this._chkFullscreen.Size = new System.Drawing.Size(116, 17);
            this._chkFullscreen.TabIndex = 41;
            this._chkFullscreen.Text = "Fullscreen Window";
            this._chkFullscreen.UseVisualStyleBackColor = true;
            // 
            // _chkShowError
            // 
            this._chkShowError.AutoSize = true;
            this._chkShowError.Location = new System.Drawing.Point(10, 141);
            this._chkShowError.Name = "_chkShowError";
            this._chkShowError.Size = new System.Drawing.Size(83, 17);
            this._chkShowError.TabIndex = 40;
            this._chkShowError.Text = "Show Errors";
            this._chkShowError.UseVisualStyleBackColor = true;
            // 
            // _chkInputFilter
            // 
            this._chkInputFilter.AutoSize = true;
            this._chkInputFilter.Location = new System.Drawing.Point(184, 141);
            this._chkInputFilter.Name = "_chkInputFilter";
            this._chkInputFilter.Size = new System.Drawing.Size(90, 17);
            this._chkInputFilter.TabIndex = 38;
            this._chkInputFilter.Text = "Filter on Input";
            this._chkInputFilter.UseVisualStyleBackColor = true;
            // 
            // _cmdFavMode
            // 
            this._cmdFavMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmdFavMode.FormattingEnabled = true;
            this._cmdFavMode.Items.AddRange(new object[] {
            "No Favorites",
            "Favorites and Games",
            "Only Favorites"});
            this._cmdFavMode.Location = new System.Drawing.Point(7, 254);
            this._cmdFavMode.Name = "_cmdFavMode";
            this._cmdFavMode.Size = new System.Drawing.Size(328, 21);
            this._cmdFavMode.TabIndex = 8;
            // 
            // _lblFavMode
            // 
            this._lblFavMode.AutoSize = true;
            this._lblFavMode.Location = new System.Drawing.Point(7, 238);
            this._lblFavMode.Name = "_lblFavMode";
            this._lblFavMode.Size = new System.Drawing.Size(83, 13);
            this._lblFavMode.TabIndex = 35;
            this._lblFavMode.Text = "Favorites Mode:";
            // 
            // _lblFavColor
            // 
            this._lblFavColor.AutoSize = true;
            this._lblFavColor.Location = new System.Drawing.Point(261, 59);
            this._lblFavColor.Name = "_lblFavColor";
            this._lblFavColor.Size = new System.Drawing.Size(53, 13);
            this._lblFavColor.TabIndex = 31;
            this._lblFavColor.Text = "Favorites:";
            // 
            // _lblColorGame
            // 
            this._lblColorGame.AutoSize = true;
            this._lblColorGame.Location = new System.Drawing.Point(166, 39);
            this._lblColorGame.Name = "_lblColorGame";
            this._lblColorGame.Size = new System.Drawing.Size(89, 13);
            this._lblColorGame.TabIndex = 30;
            this._lblColorGame.Text = "Game List Colors:";
            // 
            // _btnGameFont
            // 
            this._btnGameFont.Location = new System.Drawing.Point(6, 19);
            this._btnGameFont.Name = "_btnGameFont";
            this._btnGameFont.Size = new System.Drawing.Size(135, 56);
            this._btnGameFont.TabIndex = 0;
            this._btnGameFont.Text = "Game List Font";
            this._btnGameFont.UseVisualStyleBackColor = true;
            this._btnGameFont.Click += new System.EventHandler(this._btnFont_Click);
            // 
            // _lblClonesColor
            // 
            this._lblClonesColor.AutoSize = true;
            this._lblClonesColor.Location = new System.Drawing.Point(261, 40);
            this._lblClonesColor.Name = "_lblClonesColor";
            this._lblClonesColor.Size = new System.Drawing.Size(42, 13);
            this._lblClonesColor.TabIndex = 2;
            this._lblClonesColor.Text = "Clones:";
            // 
            // _lblParentsColor
            // 
            this._lblParentsColor.AutoSize = true;
            this._lblParentsColor.Location = new System.Drawing.Point(261, 21);
            this._lblParentsColor.Name = "_lblParentsColor";
            this._lblParentsColor.Size = new System.Drawing.Size(46, 13);
            this._lblParentsColor.TabIndex = 30;
            this._lblParentsColor.Text = "Parents:";
            // 
            // _chkHideNonWorking
            // 
            this._chkHideNonWorking.Location = new System.Drawing.Point(9, 42);
            this._chkHideNonWorking.Name = "_chkHideNonWorking";
            this._chkHideNonWorking.Size = new System.Drawing.Size(156, 24);
            this._chkHideNonWorking.TabIndex = 0;
            this._chkHideNonWorking.Text = "Hide Non-Working Games";
            // 
            // _chkAuditGames
            // 
            this._chkAuditGames.AutoSize = true;
            this._chkAuditGames.Location = new System.Drawing.Point(177, 42);
            this._chkAuditGames.Name = "_chkAuditGames";
            this._chkAuditGames.Size = new System.Drawing.Size(166, 17);
            this._chkAuditGames.TabIndex = 39;
            this._chkAuditGames.Text = "Audit Game List (MAME_g.ini)";
            this._chkAuditGames.UseVisualStyleBackColor = true;
            // 
            // _chkHideNonWorkingMechanical
            // 
            this._chkHideNonWorkingMechanical.AutoSize = true;
            this._chkHideNonWorkingMechanical.Location = new System.Drawing.Point(9, 19);
            this._chkHideNonWorkingMechanical.Name = "_chkHideNonWorkingMechanical";
            this._chkHideNonWorkingMechanical.Size = new System.Drawing.Size(142, 17);
            this._chkHideNonWorkingMechanical.TabIndex = 37;
            this._chkHideNonWorkingMechanical.Text = "Hide Mechanical Games";
            this._chkHideNonWorkingMechanical.UseVisualStyleBackColor = true;
            // 
            // _groupArt
            // 
            this._groupArt.Controls.Add(this._lblOpacityPercentage);
            this._groupArt.Controls.Add(this._picBG);
            this._groupArt.Controls.Add(this._borderWidth);
            this._groupArt.Controls.Add(this._colorPickerBorder);
            this._groupArt.Controls.Add(this._lblBackground);
            this._groupArt.Controls.Add(this._lblBorderColor);
            this._groupArt.Controls.Add(this._lblBorderWidth);
            this._groupArt.Controls.Add(this._trackBarOpacity);
            this._groupArt.Controls.Add(this._cmbArtView);
            this._groupArt.Controls.Add(this._cmbArtType);
            this._groupArt.Controls.Add(this._lblArtView);
            this._groupArt.Controls.Add(this._lblOpacity);
            this._groupArt.Controls.Add(this._lblArtType);
            this._groupArt.Location = new System.Drawing.Point(12, 383);
            this._groupArt.Name = "_groupArt";
            this._groupArt.Size = new System.Drawing.Size(342, 202);
            this._groupArt.TabIndex = 1;
            this._groupArt.TabStop = false;
            this._groupArt.Text = "Art Area:";
            // 
            // _lblOpacityPercentage
            // 
            this._lblOpacityPercentage.AutoSize = true;
            this._lblOpacityPercentage.Location = new System.Drawing.Point(160, 102);
            this._lblOpacityPercentage.Name = "_lblOpacityPercentage";
            this._lblOpacityPercentage.Size = new System.Drawing.Size(27, 13);
            this._lblOpacityPercentage.TabIndex = 33;
            this._lblOpacityPercentage.Text = "75%";
            // 
            // _picBG
            // 
            this._picBG.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._picBG.Image = global::IV_Play.Properties.Resources.Default_Background_800x432;
            this._picBG.Location = new System.Drawing.Point(188, 140);
            this._picBG.Name = "_picBG";
            this._picBG.Size = new System.Drawing.Size(143, 56);
            this._picBG.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._picBG.TabIndex = 32;
            this._picBG.TabStop = false;
            this._picBG.Click += new System.EventHandler(this._picBG_Click);
            // 
            // _borderWidth
            // 
            this._borderWidth.Location = new System.Drawing.Point(9, 176);
            this._borderWidth.Name = "_borderWidth";
            this._borderWidth.Size = new System.Drawing.Size(57, 20);
            this._borderWidth.TabIndex = 4;
            this._borderWidth.Value = new decimal(new int[] {
            11,
            0,
            0,
            0});
            // 
            // _lblBackground
            // 
            this._lblBackground.AutoSize = true;
            this._lblBackground.Location = new System.Drawing.Point(185, 124);
            this._lblBackground.Name = "_lblBackground";
            this._lblBackground.Size = new System.Drawing.Size(68, 13);
            this._lblBackground.TabIndex = 29;
            this._lblBackground.Text = "Background:";
            // 
            // _lblBorderColor
            // 
            this._lblBorderColor.AutoSize = true;
            this._lblBorderColor.Location = new System.Drawing.Point(6, 124);
            this._lblBorderColor.Name = "_lblBorderColor";
            this._lblBorderColor.Size = new System.Drawing.Size(68, 13);
            this._lblBorderColor.TabIndex = 27;
            this._lblBorderColor.Text = "Border Color:";
            // 
            // _lblBorderWidth
            // 
            this._lblBorderWidth.AutoSize = true;
            this._lblBorderWidth.Location = new System.Drawing.Point(6, 160);
            this._lblBorderWidth.Name = "_lblBorderWidth";
            this._lblBorderWidth.Size = new System.Drawing.Size(72, 13);
            this._lblBorderWidth.TabIndex = 28;
            this._lblBorderWidth.Text = "Border Width:";
            // 
            // _trackBarOpacity
            // 
            this._trackBarOpacity.LargeChange = 3;
            this._trackBarOpacity.Location = new System.Drawing.Point(6, 76);
            this._trackBarOpacity.Maximum = 100;
            this._trackBarOpacity.Name = "_trackBarOpacity";
            this._trackBarOpacity.Size = new System.Drawing.Size(328, 45);
            this._trackBarOpacity.TabIndex = 2;
            this._trackBarOpacity.TickStyle = System.Windows.Forms.TickStyle.None;
            this._trackBarOpacity.Value = 50;
            this._trackBarOpacity.Scroll += new System.EventHandler(this._trackBarOpacity_Scroll);
            // 
            // _btnApply
            // 
            this._btnApply.Location = new System.Drawing.Point(719, 596);
            this._btnApply.Name = "_btnApply";
            this._btnApply.Size = new System.Drawing.Size(75, 23);
            this._btnApply.TabIndex = 5;
            this._btnApply.Text = "Apply";
            this._btnApply.UseVisualStyleBackColor = true;
            this._btnApply.Click += new System.EventHandler(this._btnApply_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(638, 596);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 4;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
            // 
            // _btnOK
            // 
            this._btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOK.Location = new System.Drawing.Point(557, 596);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 3;
            this._btnOK.Text = "OK";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _groupArtTypes
            // 
            this._groupArtTypes.Controls.Add(this._btnDatFont);
            this._groupArtTypes.Controls.Add(this._btnLoadDat);
            this._groupArtTypes.Controls.Add(this._btnMoveDown);
            this._groupArtTypes.Controls.Add(this._btnMoveUp);
            this._groupArtTypes.Controls.Add(this._btnRemoveView);
            this._groupArtTypes.Controls.Add(this._btnAddArtView);
            this._groupArtTypes.Controls.Add(this._listArtViews);
            this._groupArtTypes.Location = new System.Drawing.Point(360, 71);
            this._groupArtTypes.Name = "_groupArtTypes";
            this._groupArtTypes.Size = new System.Drawing.Size(440, 514);
            this._groupArtTypes.TabIndex = 2;
            this._groupArtTypes.TabStop = false;
            this._groupArtTypes.Text = "Art Types:";
            // 
            // _btnDatFont
            // 
            this._btnDatFont.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._btnDatFont.AutoSize = true;
            this._btnDatFont.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btnDatFont.Location = new System.Drawing.Point(197, 485);
            this._btnDatFont.Name = "_btnDatFont";
            this._btnDatFont.Size = new System.Drawing.Size(87, 23);
            this._btnDatFont.TabIndex = 4;
            this._btnDatFont.Text = "DAT Text Font";
            this._btnDatFont.UseVisualStyleBackColor = true;
            this._btnDatFont.Click += new System.EventHandler(this._btnDatFont_Click);
            // 
            // _btnLoadDat
            // 
            this._btnLoadDat.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._btnLoadDat.AutoSize = true;
            this._btnLoadDat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btnLoadDat.Location = new System.Drawing.Point(111, 485);
            this._btnLoadDat.Name = "_btnLoadDat";
            this._btnLoadDat.Size = new System.Drawing.Size(80, 23);
            this._btnLoadDat.TabIndex = 3;
            this._btnLoadDat.Text = "Add DAT File";
            this._btnLoadDat.UseVisualStyleBackColor = true;
            this._btnLoadDat.Click += new System.EventHandler(this._btnLoadDat_Click);
            // 
            // _btnMoveDown
            // 
            this._btnMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._btnMoveDown.AutoSize = true;
            this._btnMoveDown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btnMoveDown.Location = new System.Drawing.Point(361, 485);
            this._btnMoveDown.Name = "_btnMoveDown";
            this._btnMoveDown.Size = new System.Drawing.Size(75, 23);
            this._btnMoveDown.TabIndex = 6;
            this._btnMoveDown.Text = "Move Down";
            this._btnMoveDown.UseVisualStyleBackColor = true;
            this._btnMoveDown.Click += new System.EventHandler(this._btnMoveDown_Click);
            // 
            // _btnMoveUp
            // 
            this._btnMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._btnMoveUp.AutoSize = true;
            this._btnMoveUp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btnMoveUp.Location = new System.Drawing.Point(294, 485);
            this._btnMoveUp.Name = "_btnMoveUp";
            this._btnMoveUp.Size = new System.Drawing.Size(61, 23);
            this._btnMoveUp.TabIndex = 5;
            this._btnMoveUp.Text = "Move Up";
            this._btnMoveUp.UseVisualStyleBackColor = true;
            this._btnMoveUp.Click += new System.EventHandler(this._btnMoveUp_Click);
            // 
            // _btnRemoveView
            // 
            this._btnRemoveView.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._btnRemoveView.AutoSize = true;
            this._btnRemoveView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btnRemoveView.Location = new System.Drawing.Point(48, 485);
            this._btnRemoveView.Name = "_btnRemoveView";
            this._btnRemoveView.Size = new System.Drawing.Size(57, 23);
            this._btnRemoveView.TabIndex = 2;
            this._btnRemoveView.Text = "Remove";
            this._btnRemoveView.UseVisualStyleBackColor = true;
            this._btnRemoveView.Click += new System.EventHandler(this._btnRemoveView_Click);
            // 
            // _btnAddArtView
            // 
            this._btnAddArtView.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._btnAddArtView.AutoSize = true;
            this._btnAddArtView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btnAddArtView.Location = new System.Drawing.Point(6, 485);
            this._btnAddArtView.Name = "_btnAddArtView";
            this._btnAddArtView.Size = new System.Drawing.Size(36, 23);
            this._btnAddArtView.TabIndex = 1;
            this._btnAddArtView.Text = "Add";
            this._btnAddArtView.UseVisualStyleBackColor = true;
            this._btnAddArtView.Click += new System.EventHandler(this._btnAddArtView_Click);
            // 
            // _listArtViews
            // 
            this._listArtViews.AllowDrop = true;
            this._listArtViews.FormattingEnabled = true;
            this._listArtViews.Location = new System.Drawing.Point(6, 19);
            this._listArtViews.Name = "_listArtViews";
            this._listArtViews.Size = new System.Drawing.Size(430, 459);
            this._listArtViews.TabIndex = 0;
            this._listArtViews.DragDrop += new System.Windows.Forms.DragEventHandler(this._listArtViews_DragDrop);
            this._listArtViews.DragEnter += new System.Windows.Forms.DragEventHandler(this._listArtViews_DragEnter);
            this._listArtViews.KeyUp += new System.Windows.Forms.KeyEventHandler(this._listArtViews_KeyUp);
            // 
            // _fontDialog2
            // 
            this._fontDialog2.AllowScriptChange = false;
            this._fontDialog2.ShowColor = true;
            // 
            // _btnReset
            // 
            this._btnReset.Location = new System.Drawing.Point(12, 596);
            this._btnReset.Name = "_btnReset";
            this._btnReset.Size = new System.Drawing.Size(101, 23);
            this._btnReset.TabIndex = 6;
            this._btnReset.Text = "Reset To Defaults";
            this._btnReset.UseVisualStyleBackColor = true;
            this._btnReset.Click += new System.EventHandler(this._btnReset_Click);
            // 
            // _groupMame
            // 
            this._groupMame.Controls.Add(this.autoCompleteTextBox1);
            this._groupMame.Controls.Add(this._lblCommandLine);
            this._groupMame.Location = new System.Drawing.Point(360, 12);
            this._groupMame.Name = "_groupMame";
            this._groupMame.Size = new System.Drawing.Size(440, 55);
            this._groupMame.TabIndex = 7;
            this._groupMame.TabStop = false;
            this._groupMame.Text = "Mame Settings:";
            // 
            // _lblCommandLine
            // 
            this._lblCommandLine.AutoSize = true;
            this._lblCommandLine.Location = new System.Drawing.Point(3, 16);
            this._lblCommandLine.Name = "_lblCommandLine";
            this._lblCommandLine.Size = new System.Drawing.Size(126, 13);
            this._lblCommandLine.TabIndex = 0;
            this._lblCommandLine.Text = "Command Line Switches:";
            // 
            // _groupFilters
            // 
            this._groupFilters.Controls.Add(this._chkHideNonWorking);
            this._groupFilters.Controls.Add(this._chkHideClones);
            this._groupFilters.Controls.Add(this._chkHideNonWorkingMechanical);
            this._groupFilters.Controls.Add(this._chkAuditGames);
            this._groupFilters.Location = new System.Drawing.Point(12, 303);
            this._groupFilters.Name = "_groupFilters";
            this._groupFilters.Size = new System.Drawing.Size(342, 74);
            this._groupFilters.TabIndex = 8;
            this._groupFilters.TabStop = false;
            this._groupFilters.Text = "Filters";
            // 
            // _chkHideClones
            // 
            this._chkHideClones.AutoSize = true;
            this._chkHideClones.Location = new System.Drawing.Point(177, 19);
            this._chkHideClones.Name = "_chkHideClones";
            this._chkHideClones.Size = new System.Drawing.Size(83, 17);
            this._chkHideClones.TabIndex = 38;
            this._chkHideClones.Text = "Hide Clones";
            this._chkHideClones.UseVisualStyleBackColor = true;
            // 
            // autoCompleteTextBox1
            // 
            this.autoCompleteTextBox1.Items = ((System.Collections.Generic.SortedDictionary<string, string>)(resources.GetObject("autoCompleteTextBox1.Items")));
            this.autoCompleteTextBox1.Location = new System.Drawing.Point(6, 32);
            this.autoCompleteTextBox1.Name = "autoCompleteTextBox1";
            this.autoCompleteTextBox1.Size = new System.Drawing.Size(428, 20);
            this.autoCompleteTextBox1.TabIndex = 1;
            // 
            // _colorPickerBorder
            // 
            this._colorPickerBorder.BackColor = System.Drawing.SystemColors.Control;
            this._colorPickerBorder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._colorPickerBorder.Color = System.Drawing.SystemColors.Control;
            this._colorPickerBorder.Location = new System.Drawing.Point(9, 141);
            this._colorPickerBorder.Name = "_colorPickerBorder";
            this._colorPickerBorder.Size = new System.Drawing.Size(16, 16);
            this._colorPickerBorder.TabIndex = 3;
            this._colorPickerBorder.TabStop = false;
            // 
            // _colorPickerFav
            // 
            this._colorPickerFav.BackColor = System.Drawing.SystemColors.Control;
            this._colorPickerFav.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._colorPickerFav.Color = System.Drawing.SystemColors.Control;
            this._colorPickerFav.Location = new System.Drawing.Point(318, 59);
            this._colorPickerFav.Name = "_colorPickerFav";
            this._colorPickerFav.Size = new System.Drawing.Size(16, 16);
            this._colorPickerFav.TabIndex = 3;
            this._colorPickerFav.TabStop = false;
            // 
            // _colorPickerParent
            // 
            this._colorPickerParent.BackColor = System.Drawing.SystemColors.Control;
            this._colorPickerParent.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._colorPickerParent.Color = System.Drawing.SystemColors.Control;
            this._colorPickerParent.Location = new System.Drawing.Point(318, 19);
            this._colorPickerParent.Name = "_colorPickerParent";
            this._colorPickerParent.Size = new System.Drawing.Size(16, 16);
            this._colorPickerParent.TabIndex = 1;
            this._colorPickerParent.TabStop = false;
            // 
            // _colorPickerClone
            // 
            this._colorPickerClone.BackColor = System.Drawing.SystemColors.Control;
            this._colorPickerClone.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._colorPickerClone.Color = System.Drawing.SystemColors.Control;
            this._colorPickerClone.Location = new System.Drawing.Point(318, 39);
            this._colorPickerClone.Name = "_colorPickerClone";
            this._colorPickerClone.Size = new System.Drawing.Size(16, 16);
            this._colorPickerClone.TabIndex = 2;
            this._colorPickerClone.TabStop = false;
            // 
            // ConfigForm
            // 
            this.AcceptButton = this._btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(806, 631);
            this.Controls.Add(this._groupFilters);
            this.Controls.Add(this._groupMame);
            this.Controls.Add(this._btnReset);
            this.Controls.Add(this._groupArtTypes);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnApply);
            this.Controls.Add(this._groupArt);
            this.Controls.Add(this._groupGameList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "IV/Play Config";
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ConfigForm_KeyDown);
            this._groupGameList.ResumeLayout(false);
            this._groupGameList.PerformLayout();
            this._groupArt.ResumeLayout(false);
            this._groupArt.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._picBG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._borderWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._trackBarOpacity)).EndInit();
            this._groupArtTypes.ResumeLayout(false);
            this._groupArtTypes.PerformLayout();
            this._groupMame.ResumeLayout(false);
            this._groupMame.PerformLayout();
            this._groupFilters.ResumeLayout(false);
            this._groupFilters.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _lblArtView;
        private System.Windows.Forms.Label _lblArtType;
        private System.Windows.Forms.Label _lblOpacity;
        private System.Windows.Forms.CheckBox _chkRotateBackground;
        private System.Windows.Forms.CheckBox _chkManu;
        private System.Windows.Forms.CheckBox _chkItemZoom;
        private System.Windows.Forms.CheckBox _chkYear;
        private System.Windows.Forms.ComboBox _cmbArtType;
        private System.Windows.Forms.ComboBox _cmbArtView;
        private System.Windows.Forms.FontDialog _fontDialog;
        private ColorPicker _colorPickerFav;
        private System.Windows.Forms.GroupBox _groupGameList;
        private System.Windows.Forms.Button _btnGameFont;
        private ColorPicker _colorPickerParent;
        private ColorPicker _colorPickerClone;
        private System.Windows.Forms.Label _lblClonesColor;
        private System.Windows.Forms.Label _lblParentsColor;
        private System.Windows.Forms.GroupBox _groupArt;
        private System.Windows.Forms.TrackBar _trackBarOpacity;
        private System.Windows.Forms.NumericUpDown _borderWidth;
        private ColorPicker _colorPickerBorder;
        private System.Windows.Forms.Label _lblBackground;
        private System.Windows.Forms.Label _lblBorderWidth;
        private System.Windows.Forms.Label _lblBorderColor;
        private System.Windows.Forms.Label _lblFavColor;
        private System.Windows.Forms.Label _lblColorGame;
        private System.Windows.Forms.Button _btnApply;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.PictureBox _picBG;
        private System.Windows.Forms.Label _lblOpacityPercentage;
        private System.Windows.Forms.GroupBox _groupArtTypes;
        private System.Windows.Forms.Button _btnMoveDown;
        private System.Windows.Forms.Button _btnMoveUp;
        private System.Windows.Forms.Button _btnRemoveView;
        private System.Windows.Forms.Button _btnAddArtView;
        private System.Windows.Forms.FolderBrowserDialog _folderDialog;
        private System.Windows.Forms.Button _btnLoadDat;
        private System.Windows.Forms.ComboBox _cmdFavMode;
        private System.Windows.Forms.Label _lblFavMode;
        private System.Windows.Forms.FontDialog _fontDialog2;
        private System.Windows.Forms.Button _btnDatFont;
        private System.Windows.Forms.Button _btnReset;
        private System.Windows.Forms.CheckBox _chkArtNonWorking;
        private System.Windows.Forms.ListBox _listArtViews;
        private System.Windows.Forms.GroupBox _groupMame;
        private System.Windows.Forms.Label _lblCommandLine;
        private AutoCompleteTextBox autoCompleteTextBox1;
        private System.Windows.Forms.CheckBox _chkHideNonWorkingMechanical;
        private System.Windows.Forms.CheckBox _chkInputFilter;
        private System.Windows.Forms.GroupBox _groupFilters;
        private System.Windows.Forms.CheckBox _chkFilterSearch;
        private System.Windows.Forms.CheckBox _chkArrowKeyPaging;
        private System.Windows.Forms.CheckBox _chkBackgroundRepeat;
        private System.Windows.Forms.CheckBox _chkAllowGrouping;
        private System.Windows.Forms.CheckBox _chkFullscreen;
        private System.Windows.Forms.CheckBox _chkShowError;
        private System.Windows.Forms.CheckBox _chkHideNonWorking;
        private System.Windows.Forms.CheckBox _chkAuditGames;
        private System.Windows.Forms.CheckBox _chkHideClones;
    }
}