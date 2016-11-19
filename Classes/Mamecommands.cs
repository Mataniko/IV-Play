using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using IV_Play.Properties;

namespace IV_Play
{
    public class MameCommands
    {
        private SortedDictionary<string, string> _commansdAndDescriptions = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> Commands { get { return _commansdAndDescriptions; } }

        public MameCommands() { }
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

                            _commansdAndDescriptions.Add(command, description);
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
