#region

using IV_Play.DataAccess;
using System;
using System.Windows.Forms;

#endregion

namespace IV_Play
{
    /// <summary>
    /// Rom Properties view
    /// </summary>
    public partial class PropertiesView : Form
    {
        private Game _game;

        public PropertiesView(Game game)
        {
            InitializeComponent();            
            try
            {
                //_game = new XmlParser().ReadGameByName(game.Name);
            }
            catch
            {
                _game = game;
            }            
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
            base.OnKeyUp(e);
        }

        private void PopulateForm()
        {

            _layoutPanel.BackgroundImage = SettingsManager.BackgroundImage;

            Text = string.Format("IV/Play {0} Properties", _game.Description);
            _txtDesc.Text = _game.Description;
            _txtManu.Text = _game.Manufacturer;
            _txtName.Text = _game.Name.Replace("fav_", "");
            _txtYear.Text = _game.Year;
            _txtColors.Text = _game.Colors;
            _txtStatus.Text = _game.Driver;
            _txtCPU.Text = _game.CPU;

            _txtRoms.Text = _game.Roms;

            _txtClone.Text = _game.CloneOf;
            _txtSource.Text = _game.SourceFile;
            _txtInput.Text = _game.Input;
            _txtSound.Text = _game.Sound;
            _txtScreen.Text = _game.Display;
        }

        private void PropertiesView_Load(object sender, EventArgs e)
        {
            PopulateForm();
        }

        private void _btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}