using IV_Play.Model;
using IV_Play.ViewModel;
using System;
using System.Windows;

namespace IV_Play.View
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    public partial class Properties : Window
    {

        public MachineViewModel Machine { get; set; } 
       
        public Properties(MachineViewModel machine)
        {
            try
            {
                Machine = new MachineViewModel(new XmlParser().ReadMachineByName(machine.Name));
            }
            catch (Exception ex)
            {
                Machine = machine;
            }

            InitializeComponent();


        }
    }
}
