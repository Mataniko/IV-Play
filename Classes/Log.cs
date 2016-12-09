#region

using System;
using System.IO;
using System.Text;

#endregion

namespace IVPlay
{

    /// <summary>
    /// Logging class, used for debugging.
    /// </summary>
    internal static class Logger
    {
        private static StringBuilder ErrorString = new StringBuilder();

        public static void WriteToLog(string message)
        {
            ErrorString.AppendLine(DateTime.Now + ": " + message);
            using (TextWriter tw = new StreamWriter("IV-Play.log"))
            {
                tw.WriteLine(ErrorString.ToString());
            }
        }

        public static void WriteToLog(Exception exception)
        {
            WriteToLog(exception.Message + " " + exception.StackTrace);
        }
    }
}