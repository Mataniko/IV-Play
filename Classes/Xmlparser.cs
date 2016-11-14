#region

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using IV_Play.Properties;
using IV_Play.Data.Models;
using System.Xml.Serialization;
using IV_Play.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
           
            if (File.Exists(Resources.DB_NAME))
            {
                var mameFileInfo = FileVersionInfo.GetVersionInfo(Settings.Default.MAME_EXE);
                Games.MameVersion = mameFileInfo.ProductVersion;
                using (var dbm = new DatabaseManager())
                {
                    foreach (var machine in dbm.GetMachines())
                    {
                        var game = new Game(machine);
                        if (!hiddenGames.ContainsKey(game.Name))
                        {
                            Games.Add(game.Name, game);
                        }
                    } 
                }
   
                Games.TotalGames = Games.Count;

                //Go through all the games and add clones to the parents.
                //We can't do it while reading the XML because the clones can come before a parent.
                foreach (Game game in Games.Values)
                {
                    if (!game.IsParent && Games.ContainsKey(game.ParentSet))
                        Games[game.ParentSet].Children.Add(game.Description, game);
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
        public static void MakeDat(IProgress<int> progress)
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

                var counter = 1;

                using (StreamReader myOutput = mameCommand.StandardOutput)
                {
                    // Create a fast XML Reader
                    using (XmlReader xmlReader = XmlReader.Create(myOutput, xmlReaderSettings))
                    {
                        using (var dbm = new DatabaseManager())
                        {
                             while (xmlReader.ReadToFollowing("machine"))
                            {
                                counter++;

                                if (!IsValidGame(xmlReader))
                                {
                                    continue;
                                }

                                var machine = (Machine)xmlSerializer.Deserialize(xmlReader.ReadSubtree());
                                machines.Add(machine);

                                var game = new Game(machine);
                                if (machine.cloneof == null)
                                {
                                    ParsedGames[machine.name] = game;
                                } else
                                {
                                    ParsedGames[machine.cloneof].Children[machine.name] = game;
                                }
                                
                                progress.Report(counter);
                            } // end while loop
                            dbm.UpdateMachines(machines);                            
                        }
                    } // END USING XML READER
                }
                //XmlParser.ReadDat();
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

        public static void MakeQuickDat()
        {
            if (File.Exists(Resources.DB_NAME)) File.Delete(Resources.DB_NAME);

            var machines = new Dictionary<string, Machine>();
            
            using (StreamReader listFull = ExecuteMameCommand("-listfull").StandardOutput)
            {
                // Read the header line.
                var line = listFull.ReadLine();
                var regex = new Regex(@"^(\S*)\s+""(.*)""$");
                while ((line = listFull.ReadLine()) != null)
                {
                    var match = regex.Match(line);
                    var name = match.Groups[1].Value;                    
                    var description = match.Groups[2].Value;
                    machines.Add(name, new Machine() { description = description, name = name });
                }                
            }
            
            using (StreamReader listClones = ExecuteMameCommand("-listclones").StandardOutput)
            {
                // Read the header line.
                var line = listClones.ReadLine();
                var regex = new Regex(@"^(\S+)\s+(\S+)\s*$");
                while ((line = listClones.ReadLine()) != null)
                {                   
                    var match = regex.Match(line);                   
                    var clone = match.Groups[1].Value;
                    var parent = match.Groups[2].Value;

                    machines[clone].cloneof = parent;
                }
            }

            using (var dbm = new DatabaseManager())
            {
                dbm.SaveMachines(machines.Values.ToList());
            }

            XmlParser.ReadDat();
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