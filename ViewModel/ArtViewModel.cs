using IVPlay.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IVPlay.ViewModel
{
    class ArtViewModel : ViewModelBase
    {
        public Brush TextForeground
        {
            get
            {
                return new SolidColorBrush(Settings.Default.info_font_color);
            }
        }
    }
}
