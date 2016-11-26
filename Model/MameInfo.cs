
namespace IV_Play.Model
{
    public class MameInfo
    {
        public string Version { get; set; }
        public string Product { get; set; }
        public MameCommands Commands { get; set; }

        public MameInfo() { }
    }
}
