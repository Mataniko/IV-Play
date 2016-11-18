#region

using IV_Play.Data;
using IV_Play.Data.Models;
using IV_Play.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

#endregion

namespace IV_Play
{    
    /// <summary>
    /// This Class handles the creation and parsing of Mame and IV/Play data.
    /// Our DAT file is basically a trimmed down MAME xml compressed to save space.
    /// </summary>
    internal class XmlParser
    {
        private Games _games;
        public Games Games { get
            {
                return _games;
            }
        }        

        /// <summary>
        /// Read basic game and clone info from MAME and create our initial gamelist & database.
        /// </summary>
        public void MakeQuickDat()
        {
            var machines = new Dictionary<string, Machine>();
            var mameInfo = CreateMameInfo();

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
                dbm.SaveMameInfo(mameInfo);
            }
            _games = CreateGamesFromMachines(machines.Values.ToList());
            _games.MameVersion = mameInfo.Version;
            SettingsManager.MameCommands = mameInfo.Commands;
        }

        /// <summary>
        /// Querys MAME for the full ROM data and writes it to the database
        /// </summary>
        public void MakeDat(IProgress<int> progress)
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
                                // MAME lists all of it's devices at the end, so we can just finish here.
                                if (xmlReader["isdevice"] == "yes") break;
                                                                                       
                                if (xmlReader["isbios"] == "yes" || xmlReader["runnable"] == "no")
                                {
                                    _games.Remove(xmlReader["name"]);                                   
                                    continue;
                                }

                                var machine = (Machine)xmlSerializer.Deserialize(xmlReader.ReadSubtree());
                                counter++;
                                machines.Add(machine);

                                var game = new Game(machine);
                                if (machine.cloneof == null)
                                {
                                    _games[machine.name] = game;
                                }
                                else
                                {
                                    _games[machine.cloneof].Children[machine.name] = game;
                                }

                                progress.Report(counter);
                            } // end while loop

                            progress.Report(-1);
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
                Logger.WriteToLog(ex.Message);
                Settings.Default.MAME_EXE = "";
                Application.Exit();
            }
        }

        private MameInfo CreateMameInfo()
        {
            var mameFileInfo = FileVersionInfo.GetVersionInfo(Settings.Default.MAME_EXE);
            var version = mameFileInfo.ProductVersion;
            var commands = new MameCommands(Settings.Default.MAME_EXE);
            
            return new MameInfo { Version = version, Commands = commands };
        }

        private Games CreateGamesFromMachines(List<Machine> machines)
        {
            var games = new Games();
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

            var parents = (from m in machines where m.cloneof == null && !hiddenGames.ContainsKey(m.name) select m);
            var clones = (from m in machines where m.cloneof != null && !hiddenGames.ContainsKey(m.name) select m);
            foreach (var machine in parents)
            {
                var game = new Game(machine);               
                games.Add(game.Name, game);                
            }

            games.TotalGames = games.Count;

            //Go through all the games and add clones to the parents.
            foreach (var machine in clones)
            {
                var game = new Game(machine);
                games[game.ParentSet].Children.Add(game.Description, game);
            }
           
            return games;
        }

        /// <summary>
        /// Reads the IV/Play Data file, technically should work with a compressed mame data file as well.
        /// </summary>
        public Games ReadDat()
        {
            var dbm = new DatabaseManager();
                
            var games = CreateGamesFromMachines(dbm.GetMachines());
            var mameInfo = dbm.GetMameInfo();
            games.MameVersion = mameInfo.Version;
            SettingsManager.MameCommands = mameInfo.Commands;            
            return games;
        }

        public Process ExecuteMameCommand(string argument)
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