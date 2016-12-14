using IVPlay.Properties;
using System.Windows;

namespace IVPlay.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {        
        public MainWindowViewModel()
        {
            base.DisplayName = "IV/Play";

            Application.Current.MainWindow.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
