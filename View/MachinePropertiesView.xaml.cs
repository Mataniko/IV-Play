using IV_Play.DataAccess;
using IV_Play.ViewModel;
using System;
using System.Windows;

namespace IV_Play.View
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    public partial class MachinePropertiesView : Window
    {

        public MachineViewModel Machine { get; set; } 
       
        public MachinePropertiesView(MachineViewModel machine)
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
