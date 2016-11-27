#region

using IV_Play.DataAccess;
using IV_Play.Model;
using IV_Play.Properties;
using IV_Play.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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

        private bool UserFilter(object item)
        {            
            var machine = (Machine)item;
            return machine.description.IndexOf("slug", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        
    }
}