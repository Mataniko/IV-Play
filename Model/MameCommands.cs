using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace IV_Play.Model
{
    public class MameCommands
    {
        public SortedDictionary<string, string> Commands { get; set; }

        public MameCommands() {
            Commands = new SortedDictionary<string, string>();
        }

        public MameCommands(string mamePath)
        {
            //Launches the MAME process with -showusage     
            ProcessStartInfo processStartInfo;
            processStartInfo = new ProcessStartInfo(mamePath);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.Arguments = "-showusage";
            Process proc = Process.Start(processStartInfo);

            Commands = new SortedDictionary<string, string>();

            //Setup the XML Reader/Writer options                
            using (StreamReader myOutput = proc.StandardOutput)
            {
                // Read the actual output from MAME -showusage
                while (!myOutput.EndOfStream)
                {
                    try
                    {
                        string line = myOutput.ReadLine();
                        if (line.StartsWith("-")) //found a command, hurray.
                        {
                            string command = line.Substring(0, line.IndexOf(' '));
                            string description = line.Substring(line.IndexOf(' ')).Trim();

                            Commands.Add(command, description);
                        }
                    }
                    catch (Exception)
                    {
                        //not a line, whatever.

                    }
                }
            }
        }
    }
}
