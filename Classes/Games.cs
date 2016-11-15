using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IV_Play
{
    /// <summary>
    /// Our Games list, Dictionary of romname/Game object
    /// </summary>
    internal class Games : Dictionary<string, Game>
    {
        public int TotalGames { get; set; }
        public string MameVersion { get; set; }
        public string MAME { get; set; }
    }
}
