using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace IVPlay.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings {

        private readonly string _applicationPath = AppDomain.CurrentDomain.BaseDirectory;

        public Settings() {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //

            this.PropertyChanged += Settings_PropertyChanged;
            this.SettingsLoaded += Settings_SettingsLoaded;
        }

        private void Settings_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.MAME_EXE))
            {
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(_applicationPath);
                    foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                    {
                        if (fileInfo.Name.StartsWith("MAME", StringComparison.InvariantCultureIgnoreCase) &&
                            fileInfo.Extension.Equals(".exe", StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.MAME_EXE = fileInfo.FullName;
                            SetPaths(fileInfo.DirectoryName);
                        }
                    }
                }
                catch (Exception)
                {


                }

                if (!string.IsNullOrEmpty(this.MAME_EXE)) return;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "MAME Executable";
                openFileDialog.Filter = "MAME Executable|*.exe";
                if (openFileDialog.ShowDialog() == true)
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(openFileDialog.FileName);
                    if (!string.IsNullOrEmpty(fileVersionInfo.ProductName) &&
                        fileVersionInfo.ProductName.Contains("MAME"))
                    {
                        SetPaths(openFileDialog.FileName.Replace(openFileDialog.SafeFileName, ""));
                        this.MAME_EXE = openFileDialog.FileName;
                    }
                        
                    else
                    {
                        var messageBoxResult = MessageBox.Show(
                            openFileDialog.FileName + " Does not seem like a valid MAME executable, shutting down.",
                            "Error",
                            MessageBoxButton.OK);
                        Application.Current.Shutdown();

                    }
                }

            }
        }

        /// <summary>
        /// Sets the default MAME paths once an EXE has been found
        /// </summary>
        /// <param name="path">MAME path</param>
        private void SetPaths(string path)
        {
            var ArtPaths = new List<string>();

            if (!path.EndsWith("\\"))
                path = path + "\\";

            //Snap, Flyer, History, Cabinet, CPanel, Marquee, PCB, Title, MameInfo            
            Settings.Default.art_view_folders =
                string.Format("{0}{1}|{0}{2}|{0}{3}|{0}{4}|{0}{5}|{0}{6}|{0}{7}|{0}{8}|{0}{9}",
                              path.ToLower(), @"snap", @"flyers", @"history.dat", @"cabinets", @"cpanel", @"marquees",
                              @"pcb", @"titles", @"mameinfo.dat");

            //Art View Paths
            string Paths = "";
            ArtPaths.Add("None");
            foreach (var item in this.art_view_folders.Split('|'))
            {
                if (Directory.Exists(item) || File.Exists(item))
                    ArtPaths.Add(item);
            }
            Paths.TrimEnd('|');
            //Settings.Default.art_view_folders = Paths;

            this.icons_directory = Path.Combine(path, @"icons\");
            this.bkground_directory = Path.Combine(path, @"bkground\");
        }


        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Save();
        }

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }
    }
}
