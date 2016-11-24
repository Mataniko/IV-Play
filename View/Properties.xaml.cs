using IV_Play.Data;
using IV_Play.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
    }
}
