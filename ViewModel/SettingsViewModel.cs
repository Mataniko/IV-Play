using IVPlay.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IVPlay.ViewModel
{
    class SettingsViewModel : ViewModelBase
    {
        public ObservableCollection<string> ArtPaths { get; private set; }

        public SettingsViewModel()
        {
            ArtPaths = new ObservableCollection<string>(Settings.Default.art_view_folders.Split('|'));          
        }



        private RelayCommand _applyCommand;
        public ICommand ApplyCommand
        {
            get
            {
                if (_applyCommand == null)
                {
                    _applyCommand = new RelayCommand(
                        param => Settings.Default.Save(),
                        param => true
                        );
                }
                return _applyCommand;
            }
        }

        private void StartFiltering()
        {

        }
    }
}
