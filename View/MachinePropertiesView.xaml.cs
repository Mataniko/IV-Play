using IVPlay.DataAccess;
using IVPlay.ViewModel;
using System;
using System.Windows;

namespace IVPlay.View
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
                Logger.WriteToLog(ex);
                Machine = machine;
            }

            InitializeComponent();


        }
    }
}
