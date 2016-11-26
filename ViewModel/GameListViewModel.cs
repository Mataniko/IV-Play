using IV_Play.DataAccess;
using IV_Play.Model;
using IV_Play.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IV_Play.ViewModel
{
    class GameListViewModel : ViewModelBase
    {        
        private MameInfo _mameInfo;

        public ObservableCollection<MachineViewModel> Machines { get; private set; }

        public GameListViewModel()
        {
            LoadMachines();
        }

        private async void LoadMachines()
        {
            Console.WriteLine(Settings.Default.MAME_EXE);
            if (!File.Exists(Properties.Resources.DB_NAME) && !string.IsNullOrEmpty(Settings.Default.MAME_EXE))
            {
                //var xmlParser = new XmlParser();

                //xmlParser.MakeQuickDat();
                //this.Machines = new ObservableCollection<MachineViewModel>(DatabaseManager.GetMachines());
                //this.Machines.CollectionChanged += Machines_CollectionChanged;

                //this._mameInfo = xmlParser.MameInfo;
                //SettingsManager.MameCommands = _mameInfo.Commands;
                //var progress = new Progress<int>();
                //await Task.Factory.StartNew(() => xmlParser.MakeDat(progress));
                //this.Machines = new ObservableCollection<MachineViewModel>(DatabaseManager.GetMachines().Where(x => x.ismechanical == "no"));
                //this.Machines.CollectionChanged += Machines_CollectionChanged;
            }
            else
            {
                this._mameInfo = DatabaseManager.GetMameInfo();
                
                this.Machines = new ObservableCollection<MachineViewModel>();
                var machineCollection = DatabaseManager.GetMachines().Where(x => x.ismechanical == "no");
                this.Machines.CollectionChanged += Machines_CollectionChanged;

                foreach (var machine in machineCollection)
                {
                    var mvm = new MachineViewModel(machine);                    
                    this.Machines.Add(mvm);
                }
                
            }
        }

        private void Machines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
