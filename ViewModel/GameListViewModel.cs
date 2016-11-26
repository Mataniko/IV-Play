using IV_Play.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IV_Play.ViewModel
{
    class GameListViewModel : ViewModelBase
    {
        private XmlParser _xmlParser;

        public ObservableCollection<Machine> Machines { get; set; }

        public GameListViewModel()
        {
            _xmlParser = new XmlParser();
            
        }

      
    }
}
