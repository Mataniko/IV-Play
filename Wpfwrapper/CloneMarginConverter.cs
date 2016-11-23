using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IV_Play.WPFWrapper
{
    class CloneMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new Thickness();

            return new Thickness(20, 0, 40, 0);


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine("converting from");
            return new Thickness();
            //throw new NotImplementedException();
        }
    }
}
