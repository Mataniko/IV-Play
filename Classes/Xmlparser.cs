#region

using IV_Play.Data;
using IV_Play.Data.Models;
using IV_Play.Properties;
using System;
using System.Collections.Generic;
using System.Collections;
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
        private int _version;

        public Games Games
        {
            get
            {
                return _games;
            }
        }

        private MameInfo _mameInfo;
        public MameInfo MameInfo
        {
            get
            {
                return _mameInfo;
            }
        }

        /// <summary>
        /// Read basic game and clone info from MAME and create our initial gamelist & database.
        /// </summary>
        public void MakeQuickDat()
        {

            _mameInfo = CreateMameInfo();
            SettingsManager.MameCommands = _mameInfo.Commands;

            var machinesDictionary = new Dictionary<string, Machine>();
            var clonesDictionary = new Dictionary<string, List<string>>();
            var clonesHashtable = new Hashtable();
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

                    clonesHashtable.Add(clone, true);
                    if (clonesDictionary.ContainsKey(parent))
                        clonesDictionary[parent].Add(clone);
                    else
                        clonesDictionary.Add(parent, new List<string>() { clone });
                }
            }

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
                    machinesDictionary.Add(name, new Machine() { description = description, name = name });

                }
            }

            var sortedParents = machinesDictionary.Values.Where(x => !clonesHashtable.ContainsKey(x.name)).OrderBy(x => x.description);

            var results = new List<Machine>();
            foreach (var parent in sortedParents)
            {
                results.Add(parent);
                if (clonesDictionary.ContainsKey(parent.name))
                {
                    foreach (var clone in clonesDictionary[parent.name])
                    {
                        machinesDictionary[clone].cloneof = parent.name;
                        results.Add(machinesDictionary[clone]);
                    }
                }

            }


            DatabaseManager.SaveMachines(results);
            DatabaseManager.SaveMameInfo(_mameInfo);

            _games = CreateGamesFromMachines(results);
            _games.TotalGames = results.Count;
        }

        /// <summary>
        /// Querys MAME for the full ROM data and writes it to the database
        /// </summary>
        public void MakeDat(IProgress<int> progress)
        {
            try
            {
                XmlReaderSettings xmlReaderSettings;
                var xmlRootAttribute = _version < 162 ? "game" : "machine";
                var xmlSerializer = new XmlSerializer(typeof(Machine), new XmlRootAttribute(xmlRootAttribute));
                var mameCommand = ExecuteMameCommand("-listxml");
                var machines = new List<Machine>();
                //Setup the XML Reader/Writer options                
                xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;

                var counter = 0;

                using (StreamReader myOutput = mameCommand.StandardOutput)
                {
                    // Create a fast XML Reader
                    using (XmlReader xmlReader = XmlReader.Create(myOutput, xmlReaderSettings))
                    {
                        while (xmlReader.ReadToFollowing(xmlRootAttribute))
                        {
                            // MAME lists all of it's devices at the end, so we can just finish here.
                            if (xmlReader["isdevice"] == "yes") break;

                            var machine = (Machine)xmlSerializer.Deserialize(xmlReader.ReadSubtree());
                            counter++;
                            machines.Add(machine);

                            var game = new Game(machine);
                            if (machine.cloneof == null)
                            {
                                game.Children = _games[machine.name].Children;
                                _games[machine.name] = game;
                            }
                            else
                            {
                                _games[machine.cloneof].Children[machine.name] = game;
                            }

                            progress.Report(counter);
                        } // end while loop

                        progress.Report(-1);
                        DatabaseManager.UpdateMachines(machines);
                        DatabaseManager.SaveToDisk();                 
                    } // END XmlReader
                } // END Output Stream
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
            _version = mameFileInfo.FileMinorPart;
            var product = mameFileInfo.ProductName.ToUpper();
            var commands = new MameCommands(Settings.Default.MAME_EXE);

            return new MameInfo { Version = version, Commands = commands, Product = product };
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
                games.TryAdd(game.Name, game);
            }

            //Go through all the games and add clones to the parents.
            foreach (var machine in clones)
            {
                var game = new Game(machine);
                if (games.ContainsKey(game.ParentSet))
                {
                    games[game.ParentSet].Children.Add(game.Name, game);
                }                
            }

            return games;
        }

        public Game ReadGameByName(string name)
        {
            XmlReaderSettings xmlReaderSettings;
            var xmlRootAttribute = FileVersionInfo.GetVersionInfo(Settings.Default.MAME_EXE).FileMinorPart < 162 ? "game" : "machine";
            var xmlSerializer = new XmlSerializer(typeof(Machine), new XmlRootAttribute(xmlRootAttribute));
            var mameCommand = ExecuteMameCommand("-listxml " + name);
            var machines = new List<Machine>();
            //Setup the XML Reader/Writer options                
            xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;            

            using (StreamReader myOutput = mameCommand.StandardOutput)
            {
                // Create a fast XML Reader
                using (XmlReader xmlReader = XmlReader.Create(myOutput, xmlReaderSettings))
                {
                    while (xmlReader.ReadToFollowing(xmlRootAttribute))
                    {                        
                        return new Game((Machine)xmlSerializer.Deserialize(xmlReader.ReadSubtree()));                        
                    } // end while loop                    
                } // END XmlReader
            } // END Output Stream

            return null;
        }

        /// <summary>
        /// Reads the IV/Play Data file, technically should work with a compressed mame data file as well.
        /// </summary>
        public void ReadDat()
        {          
            _games = CreateGamesFromMachines(DatabaseManager.GetMachines());
            _mameInfo = DatabaseManager.GetMameInfo();
            SettingsManager.MameCommands = _mameInfo.Commands;
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