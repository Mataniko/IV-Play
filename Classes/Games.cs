using System.Collections.Concurrent;

namespace IV_Play
{
  /// <summary>
  /// Our Games list, Dictionary of romname/Game object
  /// </summary>
  internal class Games : ConcurrentDictionary<string, Game>
  {
    public int TotalGames { get; set; }
  }
}
