using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;


namespace IV_Play
{
    public partial class AutoCompleteTextBox : TextBox
    {
        private ListBox _listBox = new ListBox();
        private ToolTip tooltip = new ToolTip();
        private bool _listBoxShow = false;
        public bool ListBoxOn { get { return _listBoxShow; } }

        public SortedDictionary<string, string> Items { get { return _items; } set { _items = value; } }
        private SortedDictionary<string, string> _items = new SortedDictionary<string, string>();

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.AcceptsReturn = false;
            this.Multiline = false;
            _listBox.SelectedIndexChanged += new EventHandler(_listBox_SelectedIndexChanged);
            _listBox.MouseDoubleClick += new MouseEventHandler(_listBox_MouseDoubleClick);
        }

        void _listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            InsertWord();
            Text += " ";
            this.SelectionStart = Text.Length;
        }

        void _listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listBox.SelectedIndex > -1)
            {
                // tooltip.Hide(_listBox);
                string title = _listBox.SelectedItem.ToString();
                string text = _items[title];


                tooltip.ToolTipTitle = title;

                tooltip.Show(text, _listBox);

            }
            else
            {
                tooltip.Hide(_listBox);
            }
        }

        private string GetLastWord()
        {
            string[] words = Text.Split(' ');
            try
            {
                for (int i = words.Count() - 1; i >= 0; i--)
                {
                    if (words[i] == "")
                        return "";

                    if (words[i].StartsWith("-") && i == words.Count() - 1)
                        return words[i];

                }
            }
            catch (Exception)
            {

                return "";
            }
            return "";
        }
        private void PopulateListBox()
        {
            _listBox.Items.Clear();
            string lastWord = GetLastWord();

            if (lastWord == "")
                return;

            foreach (var item in _items)
            {
                bool insert = true;
                foreach (var word in Text.Split(' '))
                {
                    if (item.Key == word)
                        insert = false;

                }


                if (item.Key.StartsWith(lastWord) && !item.Key.Equals(lastWord) && insert)
                    _listBox.Items.Add(item.Key);
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Down)
            {
                e.SuppressKeyPress = true;
                //  if (_listBox.Items.Count > 0)
                //      _listBox.SelectedIndex = Math.Min(_listBox.Items.Count - 1, _listBox.SelectedIndex + 1);
            }

            if (e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                // if (_listBox.Items.Count > 0)
                //      _listBox.SelectedIndex = Math.Max(_listBox.SelectedIndex - 1, 0);
            }

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                if (_listBoxShow && _listBox.SelectedIndex >= 0)
                {
                    InsertWord();

                    if (e.KeyCode == Keys.Enter)
                    {
                        Text += " ";
                        this.SelectionStart = Text.Length;
                    }
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }

            if (e.KeyCode == Keys.OemMinus)
            {
                base.OnKeyDown(e);
                ShowListBox();
            }

            if (e.KeyCode == Keys.Space)
            {
                HideListBox();
            }

            if (_listBoxShow)
            {
                if (e.KeyCode == Keys.Down)
                {
                    if (_listBox.Items.Count > 0)
                        _listBox.SelectedIndex = Math.Min(_listBox.Items.Count - 1, _listBox.SelectedIndex + 1);
                }

                if (e.KeyCode == Keys.Up)
                {
                    if (_listBox.Items.Count > 0)
                        _listBox.SelectedIndex = Math.Max(_listBox.SelectedIndex - 1, 0);
                }
            }
            //else
            base.OnKeyDown(e);


        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter && ListBoxOn)
                return false;

            return base.ProcessDialogKey(keyData);
        }

        private void InsertWord()
        {
            if (_listBox.SelectedIndex < 0 || _listBox.Items.Count == 0)
                return;

            string addtext = _listBox.SelectedItem.ToString();
            Text = Text.Remove(Text.LastIndexOf('-'));
            Text += addtext;
            this.SelectionStart = Text.Length;
            HideListBox();

        }

        private void HideListBox()
        {
            try
            {
                this.Parent.Parent.Controls.Remove(_listBox);
                _listBoxShow = false;
                tooltip.Hide(_listBox);
                this.Select();
                this.Focus();
            }
            catch (Exception)
            {


            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            string word = GetLastWord();

            // if (_listBoxShow)
            PopulateListBox();

            if (string.IsNullOrEmpty(Text))
                HideListBox();



            if (_listBox.FindString(word + '\t') != -1 || _listBox.Items.Count == 0)
                HideListBox();
            else if (word.StartsWith("-"))
                //PopulateListBox(items);
                ShowListBox();


            if (_listBox.Items.Count == 1)
                _listBox.SelectedIndex = 0;

            base.OnTextChanged(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Down)
            {
                e.SuppressKeyPress = true;
                //  if (_listBox.Items.Count > 0)
                //      _listBox.SelectedIndex = Math.Min(_listBox.Items.Count - 1, _listBox.SelectedIndex + 1);
            }

            if (e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                // if (_listBox.Items.Count > 0)
                //      _listBox.SelectedIndex = Math.Max(_listBox.SelectedIndex - 1, 0);
            }
            //  }
            //  else

            if (e.KeyCode == Keys.OemMinus)
            {
                base.OnKeyDown(e);
                ShowListBox();
            }


            base.OnKeyUp(e);


        }


        private void ShowListBox()
        {

            this.Parent.Parent.Controls.Add(_listBox);
            Point loc = this.Parent.PointToScreen(new Point(this.Left, this.Bottom));
            Point parentLoc = this.Parent.Parent.PointToClient(loc);
            _listBox.Location = parentLoc;
            _listBox.Width = this.Width;
            _listBox.BringToFront();
            _listBox.Show();
            _listBoxShow = true;


        }

    }
}
