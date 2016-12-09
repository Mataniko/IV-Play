#region

using IV_Play.Model;
using System;
using System.Threading;
using System.Windows;

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
                    //if (!mutex.WaitOne(0, false))
                    //{
                    //    MessageBox.Show("An instance of IV-Play is already running.", "Warning:");
                    //    Close();
                    //    return;
                    //}
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