#region

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using IV_Play.Properties;
using IV_Play.Data.Models;
using System.Xml.Serialization;
using IV_Play.Data;
using System.Collections.Generic;

#endregion

namespace IV_Play
{
    /// <summary>
    /// This Class handles the creation and parsing of Mame and IV/Play data.
    /// Our DAT file is basically a trimmed down MAME xml compressed to save space.
    /// </summary>
    internal static class XmlParser
    {
        public static Games ParsedGames { get; set; }
        public static Games Games { get; set; }

        /// <summary>
        /// Reads the IV/Play Data file, technically should work with a compressed mame data file as well.
        /// </summary>
        public static void ReadDat()
        {

            //Get the mame commands this seems like the best place to do it
            SettingsManager.MameCommands = new MameCommands(Settings.Default.MAME_EXE);

            Games = new Games();
            var hiddenGames = new System.Collections.Hashtable();            

            if (File.Exists("Hidden.ini"))
            {
                foreach (var item in File.ReadAllLines("Hidden.ini"))
                {
                    if (!hiddenGames.ContainsKey(item))
                    {
                        hiddenGames.Add(item, true);
                    }                    
                }
            }
            
            var xmlSerializer = new XmlSerializer(typeof(Machine), new XmlRootAttribute("machine"));            
            XmlReaderSettings xs = new XmlReaderSettings();
            xs.DtdProcessing = DtdProcessing.Ignore;
            xs.ConformanceLevel = ConformanceLevel.Fragment;
            if (File.Exists(@"IV-Play.db"))
            {
                
                Games.MameVersion = FileVersionInfo.GetVersionInfo(Settings.Default.MAME_EXE).FileVersion;
                using (var dbm = new DatabaseManager())
                { 
                    foreach (var machine in dbm.GetMachines())
                    {                        
                        Game game = new Game
                        {
                            CloneOf = string.IsNullOrEmpty(machine.cloneof) ? machine.name : machine.cloneof,
                            CPU = machine.cpuinfo(),
                            Description = machine.description,
                            SourceFile = machine.sourcefile,
                            Name = machine.name,
                            Manufacturer = machine.manufacturer,
                            ParentSet = machine.cloneof,
                            Screen = machine.displayinfo(),
                            Sound = machine.soundinfo(),
                            Working = machine.driver.emulation == "good",
                            Year = machine.year,
                            IconPath = Settings.Default.icons_directory + machine.name + ".ico",
                            Driver = machine.driver.ToString(),
                            Input = machine.input.ToString(),
                            Display = machine.displayinfo(),
                            //Colors = colors, Doesn't exist anymore?
                            Roms = machine.rominfo(),
                            IsMechanical = machine.ismechanical == "yes"
                        };
                        if (!hiddenGames.ContainsKey(game.Name))
                        {
                            Games.Add(game.Name, game);
                        }
                    } 
                }
   
                Games.TotalGames = Games.Count;

                //Go through all the games and add clones to the parents.
                //We can't do it while reading the XML because the clones can come before a parent.
                foreach (var game in Games)
                {
                    if (!game.Value.IsParent && Games.ContainsKey(game.Value.ParentSet))
                        Games[game.Value.ParentSet].Children.Add(game.Value.Description, game.Value);
                }

                //Create a new, and final list of games of just the parents, who now have clones in them.
                ParsedGames = new Games();
                foreach (var game in Games)
                {
                    if (game.Value.IsParent) //parent set, goes in
                        ParsedGames.Add(game.Value.Name, game.Value);
                }

                //Store this information for the titlebar later
                ParsedGames.TotalGames = Games.TotalGames;
                ParsedGames.MameVersion = Games.MameVersion;

                //games = null; //No need for this anymore, will be collected by the GC
            }
        }

        /// <summary>
        /// Querys MAME for Rom data, and writes only the relevant data to IV/Play's XML
        /// </summary>
        public static void MakeDat()
        {
            try
            {
                XmlReaderSettings xmlReaderSettings;
                var xmlSerializer = new XmlSerializer(typeof(Machine), new XmlRootAttribute("machine"));
                var mameCommand = ExecuteMameCommand("-listxml");
                var machines = new List<Machine>();
                //Setup the XML Reader/Writer options                
                xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;


                using (StreamReader myOutput = mameCommand.StandardOutput)
                {
                    // Create a fast XML Reader
                    using (XmlReader xmlReader = XmlReader.Create(myOutput, xmlReaderSettings))
                    {
                        using (var dbm = new DatabaseManager())
                        {
                                while (xmlReader.ReadToFollowing("machine"))
                                {
                                    if (!IsValidGame(xmlReader))
                                    {
                                        continue;
                                    }

                                    machines.Add((Machine)xmlSerializer.Deserialize(xmlReader.ReadSubtree()));

                                } // end while loop
                            dbm.SaveMachines(machines);
                        }
                    } // END USING XML READER
                }
            }
            catch (Exception ex)
            {
                //Generate an error and delete our Data file, maybe the mame xml format changed?
                MessageBox.Show("Unable to create database from mame.exe.\r\nIV/Play will now exit.", "Error");
                File.Delete("IV-Play.dat");
                Settings.Default.MAME_EXE = "";
                Application.Exit();
            }
        }

        private static bool IsValidGame(XmlReader xmlReader)
        {
            return !(xmlReader["isbios"] == "yes" || xmlReader["isdevice"] == "yes" || xmlReader["runnable"] == "no");
        }

        public static Process ExecuteMameCommand(string argument)
        {
            ProcessStartInfo processStartInfo;
            processStartInfo = new ProcessStartInfo(Settings.Default.MAME_EXE);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.Arguments = argument;
            return Process.Start(processStartInfo);
        }        
    }
}