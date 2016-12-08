#region
using IV_Play.Model;
using IV_Play.Properties;
using IV_Play.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

#endregion

namespace IV_Play.DataAccess
{
    /// <summary>
    /// This Class handles the creation and parsing of Mame Machine data.
    /// </summary>
    internal class XmlParser
    {
        /// <summary>
        /// Read basic game and clone info from MAME and create our initial gamelist & database.
        /// </summary>
        public void MakeQuickDat()
        {
            //_mameInfo = CreateMameInfo();
            //SettingsManager.MameCommands = _mameInfo.Commands;

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
            //DatabaseManager.SaveMameInfo(_mameInfo);
        }

        /// <summary>
        /// Querys MAME for the full ROM data and writes it to the database
        /// </summary>
        public void MakeDat(IProgress<int> progress, ObservableCollection<MachineViewModel> Machines)
        {
            try
            {
                XmlReaderSettings xmlReaderSettings;
                var mameFileInfo = FileVersionInfo.GetVersionInfo(Settings.Default.MAME_EXE);
                var xmlRootAttribute = mameFileInfo.FileMinorPart < 162 ? "game" : "machine";
                var xmlSerializer = new XmlSerializer(typeof(Machine), new XmlRootAttribute(xmlRootAttribute));                                
                
                //Setup the XML Reader/Writer options                
                xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;

                var machinesDatabase = DatabaseManager.GetMachines().ToDictionary(x => x.name);
                
                var counter = 0;

                var machinesIndexDictionary = Machines.ToDictionary(x => x.Name, y => Machines.IndexOf(y));
                using (StreamReader myOutput = MameWrapper.StartMameProcess("-listxml").StandardOutput)
                {                    
                    using (XmlReader xmlReader = XmlReader.Create(myOutput, xmlReaderSettings))
                    {
                        while (xmlReader.ReadToFollowing(xmlRootAttribute))
                        {
                            // MAME lists all of its devices at the end, so we can just finish here.
                            if (xmlReader["isdevice"] == "yes") break;

                            var machine = (Machine)xmlSerializer.Deserialize(xmlReader.ReadSubtree());
                            if (machinesDatabase.ContainsKey(machine.name))
                            {
                                machine.Id = machinesDatabase[machine.name].Id;
                                machinesDatabase[machine.name] = machine;
                                var gameListMachine = Machines[machinesIndexDictionary[machine.name]];

                                gameListMachine.IsMechanical = machine.ismechanical == "yes";
                                gameListMachine.SourceFile = machine.sourcefile;
                                gameListMachine.Year = machine.year;
                                gameListMachine.Manufacturer = machine.manufacturer;
                                gameListMachine.IsWorking = machine.driver != null ? machine.driver.emulation == "good" : true;                                
                            }
                            counter++;                            
                            progress.Report(counter);
                        } // end while loop

                        DatabaseManager.UpdateMachines(machinesDatabase);
                        DatabaseManager.SaveToDisk();

                        progress.Report(-1);                                      
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
            var product = mameFileInfo.ProductName.ToUpper();
            var commands = new MameCommands(Settings.Default.MAME_EXE);

            return new MameInfo { Version = version, Commands = commands, Product = product };
        }

        public Machine ReadMachineByName(string name)
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
                        return (Machine)xmlSerializer.Deserialize(xmlReader.ReadSubtree());
                    } // end while loop                    
                } // END XmlReader
            } // END Output Stream

            return null;
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