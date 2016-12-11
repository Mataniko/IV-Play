using IVPlay.DataAccess;
using IVPlay.Model;
using IVPlay.Properties;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shell;

namespace IVPlay.ViewModel
{
    partial class GameListViewModel : ViewModelBase
    {        
        private MameInfo _mameInfo;
        private MachineViewModel _machine;
        private readonly object _MachinesLock = new object();
        private CollectionView _view;
        private JumpListClass _jumpList;
        private FavoritesMode _favoritesMode;

        public MachineViewModel CurrentMachine
        {
            get
            {
                return _machine;
            }
            private set
            {
                if (_machine == value) return;
          
                _machine = value;
                base.OnPropertyChanged("CurrentMachine");
            }
        }

        private string _filter = "";
        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                _view.Refresh();
                UpdateTitle();
                base.OnPropertyChanged("Filter");
            }
        }

        private string _title;
        
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;

                OnPropertyChanged("Title");
            }
        }
        public ObservableCollection<MachineViewModel> Machines { get; private set; }

        public GameListViewModel()
        {
            //SettingsManager.GetBackgroundImage();
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;
            //if (Settings.Default.MAME_EXE == "")
            //    SettingsManager.GetMamePath(true, true);
            
            _favoritesMode = (FavoritesMode)Settings.Default.favorites_mode;
            _jumpList = new JumpListClass();            
            LoadMachines();
           // SettingsManager.GetBackgroundImage();
            
        }

        private void LoadMachines()
        {            
            var machineCollection = (from machine in DatabaseManager.GetMachines() select new MachineViewModel(machine)).ToList();           

            if (!machineCollection.Any())
            {
                RefreshGameList();
                UpdateTitle();
            } else
            {
                var favorites = LoadFavorites();

                foreach (MachineViewModel machine in machineCollection)
                {                    
                    machine.PropertyChanged += this.Machine_PropertyChanged;
                    machine.IsFavorite = favorites.Contains(machine.Name);
                }

                var favoritesMachines = machineCollection.Where(x => x.IsFavorite).OrderBy(y => y.Description);
                var normalMachines = machineCollection.Where(x => !x.IsFavorite).OrderBy(y => y.Id);
                this.Machines = new ObservableCollection<MachineViewModel>(favoritesMachines.Concat(normalMachines));                
                this.Machines.CollectionChanged += this.Machines_CollectionChanged;
                _view = (CollectionView)CollectionViewSource.GetDefaultView(this.Machines);
                _view.Filter = UserFilter;
            }
            
            UpdateTitle();                        
        }

        private void UpdateTitle()
        {

            if (_mameInfo == null)
                _mameInfo = DatabaseManager.GetMameInfo();

            if (this.Machines == null)
            {
                Title = "IV/Play";
                return;
            }                

            var titleSB = "";
            titleSB += string.Format("IV/Play - {0} {1}", _mameInfo.Product, _mameInfo.Version);

            // Favorites handling
            if (_favoritesMode == FavoritesMode.Games)
                titleSB += string.Format(" {0} Games", _view.Count);
            else
            {
                var favorites = (from m in Machines where m.IsFavorite select m).Count();                
                var count = _favoritesMode == FavoritesMode.FavoritesAndGames ? _view.Count - favorites : _view.Count;
                if (favorites > 0)
                    titleSB += string.Format(@" {0} Games / {1} Favorites", count, favorites);
            }
            
            

            
            

            if (!string.IsNullOrEmpty(Filter))
                titleSB += string.Format(" - Current Filter: {0}", Filter);       

            Title = titleSB;
        }

        private void Progress_ProgressChanged(object sender, int e)
        {
            var percentage = (((float)e / (float)Machines.Count));
            if (Title.IndexOf("Updating") < 0)
                Title += string.Format(" - Updating {0} of {1} Games", e, Machines.Count);
            else
            {
                var currentTitle = Title.Replace((e-1)+ " of", e+ " of");
                Title = currentTitle;
            }
            
            if (App.Current.MainWindow != null)            
                App.Current.MainWindow.TaskbarItemInfo.ProgressValue = percentage;            

        }

        private bool UserFilter(object item)
        {
            var mvm = (item as MachineViewModel);

            if (_favoritesMode == FavoritesMode.Favorites && !mvm.IsFavorite) return false;
            if (_favoritesMode == FavoritesMode.Games && mvm.IsFavorite) return false;

            if (!mvm.IsFavorite)
            {
                if (Settings.Default.hide_mechanical_games && mvm.IsMechanical) return false;
                if (Settings.Default.hide_nonworking && !mvm.IsWorking) return false;
                if (Settings.Default.hide_clones && mvm.CloneOf != null) return false;
            }            

            return mvm.Name.Contains(_filter, StringComparison.InvariantCultureIgnoreCase) ||
                                      mvm.Manufacturer.Contains(_filter,
                                                                        StringComparison.InvariantCultureIgnoreCase) ||
                                      mvm.Year.Contains(_filter,
                                                                StringComparison.InvariantCultureIgnoreCase) ||
                                      mvm.SourceFile.Contains(_filter,
                                                                      StringComparison.InvariantCultureIgnoreCase) ||
                                      mvm.Description.Contains(_filter,
                                                                       StringComparison.InvariantCultureIgnoreCase);           
        }

        private void Machine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string IsSelected = "IsSelected";
            var senderMachineViewModel = (sender as MachineViewModel);
            // Make sure that the property name we're referencing is valid.
            // This is a debugging technique, and does not execute in a Release build.
            senderMachineViewModel.VerifyPropertyName(IsSelected);

            // When a customer is selected or unselected, we must let the
            // world know that the TotalSelectedSales property has changed,
            // so that it will be queried again for a new value.
            if (e.PropertyName == IsSelected)
            {
                this.OnPropertyChanged("MachineSelected");
                CurrentMachine = sender as MachineViewModel;                
            }

            if (e.PropertyName == "IsFavorite")
            {
                if (this.Machines == null) return;

                if (senderMachineViewModel.IsFavorite) // Reorder favorites when inserting
                {
                    var favorites = (from m in this.Machines where m.IsFavorite orderby m.Description descending select m);

                    foreach (var machine in favorites)
                    {
                        var indexOfMachine = this.Machines.IndexOf(machine);
                        this.Machines.Move(indexOfMachine, 0);
                        if (indexOfMachine == 0) // Force a refresh if we're not moving an item
                            _view.Refresh();                                                                                
                    }
                }        
            }

            if (e.PropertyName == "IsMechanical" && (sender as MachineViewModel).IsMechanical)
            {
                this.OnPropertyChanged("IsMechanical"); 
            } 
                
        }

        private void Machines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (MachineViewModel machineVM in e.NewItems)
                    machineVM.PropertyChanged += this.Machine_PropertyChanged;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (MachineViewModel machineVM in e.OldItems)
                    machineVM.PropertyChanged -= this.Machine_PropertyChanged;

            UpdateTitle();
        }

        

       

        
    }
}
