#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace IV_Play
{
  /// <summary>
  /// Filter form.
  /// </summary>
  public partial class FilterDialog : Form
  {
    public FilterDialog()
    {
      InitializeComponent();
      Filter = "Enter Game / Year / Driver / Manufacturer";
      ClientSize = new Size(320, 150);
    }

    public string Filter { get; set; }

    public DialogResult ShowDialog(IWin32Window owner, string filter)
    {
      Filter = filter;
      return this.ShowDialog(owner);
    }
    private void _btnOK_Click(object sender, EventArgs e)
    {
      Filter = _txtFilter.Text;
    }

    private void FilterDialog_Load(object sender, EventArgs e)
    {
      _txtFilter.SelectAll();
      if (Filter == "")
        _txtFilter.Text = "Enter Game / Year / Driver / Manufacturer";
      else
      {
        _txtFilter.Text = Filter;
      }
    }

    private void FilterDialog_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Escape)
        Close();
    }
  }
}
