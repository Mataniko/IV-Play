#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using IV_Play.Properties;
using System.ComponentModel;

#endregion

namespace IV_Play
{

    /// <summary>
    /// Responsible for reading and saving our settings to file.
    /// We are storing the actual settings in the builtin application Settings.
    /// </summary>
    [Serializable]
    public static class SettingsManager
    {
        private static string cfgPath = Application.StartupPath + @"\IV-Play.cfg";

        public static Image BackgroundImage;
        public static List<string> ArtPaths = new List<string>();
        public static MameCommands MameCommands;
        public static int[] CustomColors = new int[16];
        public static List<string> WorkingRoms = new List<string>();

        static SettingsManager()
        {
            if (File.Exists(cfgPath))
                ReadSettingsFromFile(cfgPath);
            else
                SetDefaultSettings();

            if (File.Exists("MAME_g.ini"))
            {
                ReadAuditFromFile("MAME_g.ini");
            }
        }

        public static void ReadAuditFromFile(string fileName)
        {
            WorkingRoms.Clear();

            string[] lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                string query = " [012],";
                Regex regEx = new Regex(query);
                Match match = regEx.Match(line);
                if (match.Success)
                {
                    // Valid and working rom.
                    int end = line.IndexOf(' ');
                    if (end != -1)
                    {
                        WorkingRoms.Add(line.Substring(0, end));
                    }
                }
            }
        }

        public static void ResetSettings()
        {
            string mameExe = Settings.Default.MAME_EXE;
            string jumpList = Settings.Default.jumplist;

            string[] strings = File.ReadAllLines(cfgPath);
            try
            {
                foreach (var s in strings)
                {
                    string param = s.Split('=')[0];
                    Type type = Settings.Default[param].GetType();
                    Settings.Default[param] = CastHelper(type, Settings.Default.Properties[param].DefaultValue.ToString());
                }

            }
            catch (Exception)
            {

            }
            Settings.Default.MAME_EXE = mameExe;

            SetPaths(Path.GetDirectoryName(mameExe) + "\\");
            //WriteSettingsToFile();

        }

        /// <summary>
        /// Reads a key/value pair seperated by a '=' character
        /// </summary>
        /// <param name="path"></param>
        private static void ReadSettingsFromFile(string path)
        {
            string[] settingsArray = File.ReadAllLines(path, Encoding.ASCII);
            foreach (string s in settingsArray)
            {
                string param;
                string value;
                try
                {
                    string[] sSplit = s.Split('=');
                    param = sSplit[0];
                    value = sSplit[1];
                    for (int i = 2; i < sSplit.Length; i++)
                    {
                        value += "=" + sSplit[i];
                    }
                }
                catch (Exception)
                {
                    param = "";
                    value = "";
                }

                Type type = null;
                try
                {
                    type = Settings.Default[param].GetType();
                    Settings.Default[param] = CastHelper(type, value);
                }
                catch (Exception)
                {

                    //If an exception happened, the item is probably not a setting and will be ignored.
                    try
                    {
                        if (param != "")
                            Settings.Default[param] = CastHelper(type,
                                                                 Settings.Default.Properties[param].DefaultValue.
                                                                     ToString());
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteToLog(ex);
                    }
                }
            }

            ArtPaths.Add("None");
            foreach (var artpath in Settings.Default.art_view_folders.Split('|'))
            {
                if (!string.IsNullOrEmpty(artpath))
                    ArtPaths.Add(artpath);
            }
        }

        internal static int[] ReadCustomColors()
        {
            int[] ints = new int[16];
            string[] strings;
            try
            {
                strings = Settings.Default.custom_colors.Split(',');
            }
            catch (Exception)
            {
                strings = Settings.Default.Properties["custom_colors"].DefaultValue.ToString().Split(',');
            }

            for (int i = 0; i < strings.Length; i++)
            {
                try
                {
                    ints[i] = Convert.ToInt32(strings[i]);
                }
                catch (Exception)
                {

                    ints[i] = 0;
                }

            }
            return ints;
        }

        /// <summary>
        /// Translate a text value to its setting type
        /// </summary>
        /// <param name="type">Type to convert to</param>
        /// <param name="value">string to cast</param>
        /// <returns></returns>
        private static object CastHelper(Type type, string value)
        {
            switch (type.Name)
            {
                case "Int32":
                    return Convert.ToInt32(value);

                case "Byte":
                    return Convert.ToByte(value);

                case "Boolean":
                    return Convert.ToBoolean(value);

                case "Color":
                    {
                        try
                        {
                            string[] rgb = value.Split(',');
                            return Color.FromArgb(Convert.ToInt32(rgb[0]), Convert.ToInt32(rgb[1]), Convert.ToInt32(rgb[2]));
                        }
                        catch (Exception)
                        {
                            return Color.FromName(value);
                        }

                    }
                case "Font":
                    return (new FontConverter().ConvertFromString(value));

                default:
                    return value;
            }
        }


        /// <summary>
        /// Load the background image and rotate it if needed
        /// Else, load the builtin background.
        /// </summary>
        public static void GetBackgroundImage()
        {
            try
            {
                if (Settings.Default.rotate_background)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Settings.Default.bkground_directory);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    var images =
                        (from entry in fileInfos
                         where entry.Extension.In(".png", ".bmp", ".gif", "jpg")
                         select entry);
                    Random rand = new Random();
                    int iRand = rand.Next(0, images.Count());
                    Settings.Default.bkground_image = images.ElementAt(iRand).Name;
                    string bkimg = images.ElementAt(iRand).FullName;
                    BackgroundImage = Image.FromFile(bkimg);
                    return;
                }

                string path = Settings.Default.bkground_directory + Settings.Default.bkground_image;
                if (File.Exists(path))
                {
                    BackgroundImage = Image.FromFile(path);
                    return;
                }
            }
            catch
            {
                BackgroundImage = Resources.Default_Background;
            }

            BackgroundImage = Resources.Default_Background;
        }

        /// <summary>
        /// Sets and validates a MAME exe, however the user can load any exe should he wish.       
        /// </summary>
        /// <param name="TryFindMAME">Try to find MAME in the current folder, used for first start.</param>
        /// <returns>True if MAME was found</returns>
        public static bool GetMamePath(bool TryFindMAME, bool setPaths)
        {
            if (TryFindMAME)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(Application.StartupPath);
                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    if (fileInfo.Name.StartsWith("MAME", StringComparison.InvariantCultureIgnoreCase) &&
                        fileInfo.Extension.Equals(".exe", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Settings.Default.MAME_EXE = fileInfo.FullName;
                        if (setPaths)
                            SetPaths(fileInfo.DirectoryName);
                        return true;
                    }
                }
            }


            OpenFileDialog openFileDialog = new OpenFileDialog();
            using (openFileDialog)
            {
                openFileDialog.Title = "MAME Executable";
                openFileDialog.Filter = "MAME Executable|*.exe";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(openFileDialog.FileName);
                    if (!string.IsNullOrEmpty(fileVersionInfo.ProductName) &&
                        fileVersionInfo.ProductName.Contains("MAME"))
                        Settings.Default.MAME_EXE = openFileDialog.FileName;
                    else
                    {
                        DialogResult error =
                            MessageBox.Show(
                                openFileDialog.FileName + " Does not seem like a valid MAME executable, use anyway?",
                                "Error",
                                MessageBoxButtons.YesNo);
                        if (error == DialogResult.No)
                            return false;

                        Settings.Default.MAME_EXE = openFileDialog.FileName;
                    }
                    if (setPaths)
                        SetPaths(openFileDialog.FileName.Replace(openFileDialog.SafeFileName, ""));
                    Settings.Default.MAME_EXE = openFileDialog.FileName;
                    return true;
                }
            }


            return false;
        }

        /// <summary>
        /// Sets the default MAME paths once an EXE has been found
        /// </summary>
        /// <param name="path">MAME path</param>
        private static void SetPaths(string path)
        {
            ArtPaths = new List<string>();

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
            foreach (var item in Settings.Default.art_view_folders.Split('|'))
            {
                if (Directory.Exists(item) || File.Exists(item))
                    ArtPaths.Add(item);
            }
            Paths.TrimEnd('|');
            //Settings.Default.art_view_folders = Paths;

            Settings.Default.icons_directory = Path.Combine(path, @"icons\");
            Settings.Default.bkground_directory = Path.Combine(path, @"bkground\");

        }

        /// <summary>
        /// Create the initial IV/Play.cfg
        /// </summary>
        private static void SetDefaultSettings()
        {
            WriteSettingsToFile();
        }

        /// <summary>
        /// Dump the context of Settings to file.
        /// </summary>
        public static void WriteSettingsToFile()
        {
            List<string> listSettings = new List<string>();

            foreach (SettingsProperty setting in Settings.Default.Properties)
            {
                if (setting.PropertyType.Name == "Color")
                {
                    Color color = (Color)Settings.Default[setting.Name];
                    listSettings.Add(setting.Name + "=" + string.Format("{0},{1},{2}", color.R, color.G, color.B));
                }
                else if (setting.PropertyType.Name == "Font")
                    listSettings.Add(setting.Name + "=" +
                                     (new FontConverter().ConvertToString(Settings.Default[setting.Name])));
                else
                    listSettings.Add(setting.Name + "=" + Settings.Default[setting.Name]);
            }
            try
            {
                bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
                if (!designMode)
                {
                    File.WriteAllLines(cfgPath, listSettings, Encoding.ASCII);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }



        }

        public static void WriteCustomColors(int[] colors)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var color in colors)
            {
                stringBuilder.AppendFormat("{0},", color);
            }
            string sColors = stringBuilder.ToString().TrimEnd(',');
            Settings.Default.custom_colors = sColors;
        }

        /// <summary>
        /// Limits the jumplist to 20 items and makes sure we don't have duplicate games in it.
        /// </summary>
        /// <param name="GameName"></param>
        public static void AddGameToJumpList(string GameName)
        {
            string[] jumpItems = Settings.Default.jumplist.Split(',');
            string jumpList = "";
            if (jumpItems.Length >= 20)
            {
                for (int i = 1; i < 20; i++)
                {
                    jumpList += jumpItems[i] + ",";
                }
                Settings.Default.jumplist = jumpList.TrimEnd(',');
            }

            Settings.Default.jumplist += string.IsNullOrEmpty(Settings.Default.jumplist)
                                             ? GameName
                                             : "," + GameName;

            WriteSettingsToFile();
        }
    }
}