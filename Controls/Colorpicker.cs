#region

using System.Drawing;
using System.Windows.Forms;

#endregion

//using System.Linq;

namespace IV_Play
{

  /// <summary>
  /// A simple control that we use as a color picker in our config form.
  /// </summary>
  public partial class ColorPicker : UserControl
  {

    public ColorPicker()
    {
      InitializeComponent();
      CustomColors = SettingsManager.ReadCustomColors();
    }

    public static int[] CustomColors { get; set; }

    public Color Color
    {
      get { return BackColor; }
      set { BackColor = value; }
    }

    private void panel1_MouseClick(object sender, MouseEventArgs e)
    {

      colorDialog1.Color = BackColor;
      colorDialog1.CustomColors = CustomColors;
      DialogResult dialogResult = colorDialog1.ShowDialog(this);
      if (dialogResult == DialogResult.OK)
        BackColor = colorDialog1.Color;
      CustomColors = colorDialog1.CustomColors;
      SettingsManager.WriteCustomColors(CustomColors);
    }


  }
}
