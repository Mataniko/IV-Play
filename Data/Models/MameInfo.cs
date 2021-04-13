using LiteDB;

namespace IV_Play.Data.Models
{
  public class MameInfo
  {
    public ObjectId Id { get; set; }
    public string Version { get; set; }
    public string Product { get; set; }
    public MameCommands Commands { get; set; }

    public MameInfo() { }
  }
}
