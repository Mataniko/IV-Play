using IV_Play.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IV_Play.DataAccess
{
    static class MameWrapper
    {
        public static void GetMameVersion()
        {

        }

        public static SortedDictionary<string, string> GetMameUsageArguments()
        {
            var commands = new SortedDictionary<string, string>();
            using (StreamReader myOutput = StartMameProcess("-showusage").StandardOutput)
            {
                // Read the actual output from MAME -showusage                    
                var line = "";
                while ((line = myOutput.ReadLine()) != null)
                {
                    if (line.StartsWith("-")) //found a command, hurray.
                    {
                        string command = line.Substring(0, line.IndexOf(' '));
                        string description = line.Substring(line.IndexOf(' ')).Trim();

                        commands.Add(command, description);
                    }
                }                
            }
            return commands;
        }

        public static Process StartMameProcess(string arguments)
        {
            ProcessStartInfo processStartInfo;
            processStartInfo = new ProcessStartInfo(Settings.Default.MAME_EXE);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.Arguments = arguments;
            return Process.Start(processStartInfo);
        }
    }
}
