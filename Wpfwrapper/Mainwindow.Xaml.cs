#region

using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.MessageBox;

#endregion

namespace IV_Play
{
  /// <summary>
  /// Wrapper WPF window for our application, this enables us to use cool features like the JumpList
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      try
      {

        //Prevent multiple instances of the application from running.
        using (Mutex mutex = new Mutex(false, @"IV-Play MameUI"))
        {
          if (!mutex.WaitOne(0, false))
          {
            MessageBox.Show("An instance of IV-Play is already running.", "Warning:");
            Close();
            return;
          }

          //Enabled all the winforms visual styles to give it the Windows look.
          Application.EnableVisualStyles();
          Application.SetCompatibleTextRenderingDefault(false);

          //Load the background image for the main form.
          SettingsManager.GetBackgroundImage();

          //Display the form. While the status is retry it will just create new instances
          //This allows us to close the form and restart the application when refreshing
          //our data files.
          MainForm mainForm;
          DialogResult dialogResult = System.Windows.Forms.DialogResult.Retry;

          while (true)
          {
            if (dialogResult == System.Windows.Forms.DialogResult.Retry)
            {
              mainForm = new MainForm();
              mainForm.BringToFront();
              dialogResult = mainForm.ShowDialog();
            }
            else
            {
              break;
            }
          }
          Close();
        }
      }
      catch (Exception ex)
      {
        Logger.WriteToLog(ex);
        Close();
      }
    }
  }
}
