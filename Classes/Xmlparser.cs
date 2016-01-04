#region

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using IV_Play.Properties;

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

            String gameElementName = "game";
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

            XmlReaderSettings xs = new XmlReaderSettings();
            xs.DtdProcessing = DtdProcessing.Ignore;
            xs.ConformanceLevel = ConformanceLevel.Fragment;
            if (File.Exists("IV-Play.dat"))
            {
                using (FileStream infileStream = new FileStream(@"IV-Play.dat", FileMode.Open))
                {
                    using (GZipStream gZipStream = new GZipStream(infileStream, CompressionMode.Decompress))
                    {
                        using (StreamReader streamReader = new StreamReader(gZipStream, Encoding.ASCII))
                        {
                            using (XmlReader xmlReader = XmlReader.Create(streamReader, xs))
                            {

                                //Get the MAME version info
                                xmlReader.ReadToFollowing("mame");
                                Games.MameVersion = xmlReader["build"];

                                float floatMameVersion = float.Parse(Games.MameVersion.Substring(0,5), System.Globalization.CultureInfo.GetCultureInfo("en-US"));

                                if (floatMameVersion >= 0.162)
                                {
                                    gameElementName = "machine";
                                }

                                //Read Game elements
                                while (xmlReader.ReadToFollowing(gameElementName))
                                {
                                    //No bios sets should be in our XML, but check anyways
                                    if (!string.IsNullOrEmpty(xmlReader["isbios"]) &&
                                        xmlReader["isbios"].Equals("yes"))
                                        continue;
                                    string sIsMechanical = "";
                                    string sourcefile = "";
                                    string name = "";
                                    string parent = "";
                                    string description = "";
                                    string year = "";
                                    string manufacturer = "";
                                    bool status = true;
                                    string driver =
                                        "Status={0}, Emulation={1}, Color={2}, Sound={3}, Graphic={4}, Savestate={5}";
                                    string colors = "";
                                    string input = "";
                                    string display = "";
                                    StringBuilder sbRoms = new StringBuilder();
                                    StringBuilder sbAudio = new StringBuilder();
                                    StringBuilder sbCpu = new StringBuilder();

                                    name = xmlReader["name"];
                                    sourcefile = xmlReader["sourcefile"];
                                    parent = xmlReader["cloneof"];
                                    sIsMechanical = xmlReader["ismechanical"];


                                    while (xmlReader.Read())
                                    {
                                        //If we reached a game element here it means we must have reached the
                                        //end of the current game, so we break out of this loop to find the next game.
                                        if (xmlReader.Name.Equals(gameElementName))
                                            break;

                                        switch (xmlReader.Name)
                                        {
                                            case "description":
                                                description = xmlReader.ReadElementString();
                                                description = description.TrimStart('\'');
                                                break;
                                            case "year":
                                                year = xmlReader.ReadString();
                                                break;
                                            case "manufacturer":
                                                manufacturer = xmlReader.ReadString();
                                                break;
                                            case "rom":
                                            case "disk":
                                                sbRoms.AppendLine(xmlReader["region"] + "\t" + xmlReader["name"]);
                                                break;
                                            case "chip":
                                                if (xmlReader["type"] == "cpu")
                                                {
                                                    string clock = !string.IsNullOrEmpty(xmlReader["clock"])
                                                                       ? " " +
                                                                         (Convert.ToSingle(xmlReader["clock"])/1000000).
                                                                             ToString().TrimEnd('0').TrimEnd('.') +
                                                                         " MHz"
                                                                       : "";
                                                    sbCpu.AppendLine(xmlReader["name"] + clock);
                                                }

                                                if (xmlReader["type"] == "audio")
                                                {
                                                    string clock = !string.IsNullOrEmpty(xmlReader["clock"])
                                                                       ? " " +
                                                                         (Convert.ToSingle(xmlReader["clock"])/1000).
                                                                             ToString().TrimEnd('0').TrimEnd('.') +
                                                                         " kHz"
                                                                       : "";
                                                    sbAudio.AppendLine(xmlReader["name"] + clock);
                                                }
                                                break;
                                            case "driver":
                                                driver = string.Format(driver, xmlReader["status"],
                                                                       xmlReader["emulation"], xmlReader["color"],
                                                                       xmlReader["sound"], xmlReader["graphic"],
                                                                       xmlReader["savestate"]);

                                                //Colors is a separate element                       
                                                colors = xmlReader["palettesize"];

                                                //Cocktail and protection are not mandatory, so checking them.
                                                if (!string.IsNullOrEmpty(xmlReader["cocktail"]))
                                                    driver += string.Format(", Cocktail={0}", xmlReader["cocktail"]);
                                                if (!string.IsNullOrEmpty(xmlReader["protection"]))
                                                    driver += string.Format(", Protection={0}",
                                                                            xmlReader["protection"]);
                                                status = xmlReader["emulation"].Equals("good");
                                                break;
                                            case "display":

                                                string refresh = xmlReader["refresh"].TrimEnd('0').TrimEnd('.') + " Hz";

                                                char rotation = xmlReader["rotate"] == "0" ||
                                                                xmlReader["rotate"] == "180"
                                                                    ? 'H'
                                                                    : 'V';
                                                display = xmlReader["width"] + "x" + xmlReader["height"] + " (" +
                                                          rotation + ") " + refresh;
                                                break;
                                            case "input":
                                                if (xmlReader.IsStartElement())
                                                {
                                                    input = xmlReader["players"] + " Player(s) ";
                                                    input += xmlReader["buttons"] + " Button(s)";
                                                }
                                                break;
                                            case "control":
                                                input += " " + xmlReader["type"];
                                                break;
                                        }
                                    }
                                    Game game = new Game
                                                    {
                                                        CloneOf = string.IsNullOrEmpty(parent) ? name : parent,
                                                        CPU = sbCpu.ToString(),
                                                        Description = description,
                                                        SourceFile = sourcefile,
                                                        Name = name,
                                                        Manufacturer = manufacturer,
                                                        ParentSet = parent,
                                                        Screen = display.Trim(),
                                                        Sound = sbAudio.ToString(),
                                                        Working = status,
                                                        Year = year,
                                                        IconPath = Settings.Default.icons_directory + name + ".ico",
                                                        Driver = driver.Trim(),
                                                        Input = input.Trim(),
                                                        Display = display.Trim(),
                                                        Colors = colors,
                                                        Roms = sbRoms.ToString(),
                                                        IsMechanical = isMechanical(sIsMechanical)
                                                    };
                                    if (!hiddenGames.ContainsKey(game.Name))
                                    {
                                        Games.Add(game.Name, game);
                                    }                                    
                                } //while readto game
                            } //using xmlreader
                        } //streamreader
                    } //gzip
                } //filestream

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

        private static bool isMechanical(string value)
        {
            switch (value)
            {
                case "yes":
                    return true;
                case "no":
                    return false;
                default:
                    return false;                    
            }
        }

        /// <summary>
        /// Querys MAME for Rom data, and writes only the relevant data to IV/Play's XML
        /// </summary>
        public static void MakeDat()
        {
            try
            {
                XmlWriterSettings xmlWriterSettings;
                XmlReaderSettings xmlReaderSettings;
                ProcessStartInfo processStartInfo;


                //Launches the MAME process with -listxml            
                processStartInfo = new ProcessStartInfo(Settings.Default.MAME_EXE);
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.Arguments = "-listxml";
                Process proc = Process.Start(processStartInfo);


                //Setup the XML Reader/Writer options                
                xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;

                xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
                xmlWriterSettings.Indent = true;
                                
                using (StreamReader myOutput = proc.StandardOutput)
                {
                    // Create a fast XML Reader
                    using (XmlReader xmlReader = XmlReader.Create(myOutput, xmlReaderSettings))
                    {
                        //Save it to our Data file
                        using (FileStream outFile = File.Create("IV-Play.dat"))
                        {
                            //And zip it
                            using (GZipStream gZipStream = new GZipStream(outFile, CompressionMode.Compress))
                            {
                                //XmlWriter creates IV/Play's Data
                                using (XmlWriter xmlWriter = XmlWriter.Create(gZipStream, xmlWriterSettings))
                                {
                                    while (xmlReader.Read())
                                    {
                                        //Here because the XML is so big we have to flush at the cost of performance
                                        xmlWriter.Flush();

                                        //The Name represents the XML element name
                                        switch (xmlReader.Name)
                                        {
                                            case "mame":
                                                if (xmlReader.IsStartElement())
                                                {
                                                    xmlWriter.WriteStartElement(xmlReader.Name);
                                                    if (xmlReader.HasAttributes)
                                                    {
                                                        xmlWriter.WriteAttributes(xmlReader, true);
                                                    }
                                                }
                                                else
                                                {
                                                    xmlWriter.WriteEndElement();
                                                }
                                                break;
                                            case "machine":
                                            case "game":
                                                if (xmlReader.IsStartElement())
                                                {
                                                    //Is the game a bios set? if so, read to the next game.
                                                    while (!IsValidGame(xmlReader))
                                                    {
                                                        xmlReader.ReadToNextSibling(xmlReader.Name);
                                                    }


                                                    if (xmlReader.Name.Equals(xmlReader.Name))
                                                    {
                                                        xmlWriter.WriteStartElement(xmlReader.Name);
                                                        if (xmlReader.HasAttributes)
                                                        {
                                                            xmlWriter.WriteAttributes(xmlReader, true);
                                                        }                                                        
                                                    }
                                                            
                                                }
                                                else
                                                    xmlWriter.WriteEndElement();
                                                break;
                                            case "year":
                                            case "manufacturer":
                                                if (xmlReader.IsStartElement())
                                                    xmlWriter.WriteElementString(xmlReader.Name,
                                                                                    xmlReader.
                                                                                        ReadElementContentAsString());
                                                break;
                                            case "description":
                                                if (xmlReader.IsStartElement())
                                                {
                                                    string description = xmlReader.ReadElementContentAsString();
                                                    description = GetDescription(description);
                                                    xmlWriter.WriteElementString("description", description);
                                                }
                                                break;
                                            case "control":
                                            case "display":
                                            case "driver":
                                            case "chip":
                                            case "rom":
                                            case "input":
                                            case "disk":
                                                if (xmlReader.IsStartElement())
                                                {
                                                    xmlWriter.WriteStartElement(xmlReader.Name);
                                                    if (xmlReader.HasAttributes)
                                                    {
                                                        xmlWriter.WriteAttributes(xmlReader, true);
                                                    }
                                                    xmlWriter.WriteEndElement();
                                                }
                                                break;
                                        } //end switch statement
                                    } // end while loop
                                } // END USING XML WRITER
                            } // END USING GZIP
                        } // END USING FILE OUTPUT
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
            if (!string.IsNullOrEmpty(xmlReader["isbios"]) &&
                xmlReader["isbios"].Equals("yes"))
                return false;


            //Check that the game is runnable
            if (!string.IsNullOrEmpty(xmlReader["runnable"]) &&
               xmlReader["runnable"].Equals("no"))
                return false;

            return true;

        }

        /// <summary>
        /// Manipulates the Description to move The, An, A to the end of the string, but before the (SET INFO)
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        private static string GetDescription(string description)
        {
            //Get the insertion point (because the description might have a comment in it such as (Set 1)
            int insertionPoint = description.IndexOf('(');
            if (description.StartsWith("the ",
                                       StringComparison.
                                           InvariantCultureIgnoreCase))
            {
                description = description.Remove(0, 4);
                //If insertion point is -1, we can just stick it at the end of the string
                insertionPoint = insertionPoint == -1 ? description.Length : insertionPoint - 5;
                description = description.Insert(insertionPoint, ", The");
            }
            else if (description.StartsWith("a ",
                                            StringComparison.
                                                InvariantCultureIgnoreCase))
            {
                description = description.Remove(0, 2);
                insertionPoint = insertionPoint == -1 ? description.Length : insertionPoint - 3;
                description = description.Insert(insertionPoint, ", A");
            }
            else if (description.StartsWith("an ",
                                            StringComparison.
                                                InvariantCultureIgnoreCase))
            {
                description = description.Remove(0, 3);
                insertionPoint = insertionPoint == -1 ? description.Length : insertionPoint - 4;
                description = description.Insert(insertionPoint, ", An");
            }
            return description;
        }
    }
}