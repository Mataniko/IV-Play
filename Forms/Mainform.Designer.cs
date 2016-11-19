namespace IV_Play
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._gameList = new IV_Play.GameList();
            this.SuspendLayout();
            // 
            // _gameList
            // 
            this._gameList.ArtDisplayMode = IV_Play.ArtDisplayMode.normal;
            this._gameList.ArtTransperancy = 0;
            this._gameList.ArtType = 0;
            this._gameList.BackColor = System.Drawing.Color.Transparent;
            this._gameList.BorderColor = System.Drawing.Color.Empty;
            this._gameList.BorderWidth = 0;
            this._gameList.CloneColor = System.Drawing.Color.Empty;
            this._gameList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gameList.FavoriteCloneColor = System.Drawing.Color.Empty;
            this._gameList.FavoriteColor = System.Drawing.Color.Empty;
            this._gameList.FavoritesMode = IV_Play.FavoritesMode.Games;
            this._gameList.Filter = "";
            this._gameList.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this._gameList.Location = new System.Drawing.Point(0, 0);
            this._gameList.Name = "_gameList";
            this._gameList.ParentColor = System.Drawing.Color.Empty;
            this._gameList.Rows = 0;
            this._gameList.SelectedGame = null;
            this._gameList.SelectedRow = 0;
            this._gameList.ShowManufacturer = false;
            this._gameList.ShowYear = false;
            this._gameList.Size = new System.Drawing.Size(1117, 462);
            this._gameList.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Green;
            this.BackgroundImage = global::IV_Play.Properties.Resources.Default_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1117, 462);
            this.Controls.Add(this._gameList);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "IV/Play";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private GameList _gameList;

    }
}