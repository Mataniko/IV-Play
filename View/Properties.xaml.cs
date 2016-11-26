using IV_Play.Model;
using IV_Play.ViewModel;
using System.Windows;

namespace IV_Play.View
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    public partial class Properties : Window
    {

        public Machine Machine { get; set; } 
        public Properties(Machine machine)
        {
            try
            {
                Machine = new XmlParser().ReadMachineByName(machine.name);
            }
            catch
            {
                Machine = machine;
            }

            InitializeComponent();

           
        }

        public Properties(MachineViewModel machine)
        {
            try
            {
                Machine = new XmlParser().ReadMachineByName(machine.Name);
            }
            catch
            {
                Machine = new Machine();
            }

            InitializeComponent();


        }
    }
}
