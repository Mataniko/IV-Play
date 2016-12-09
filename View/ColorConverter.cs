using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;

namespace IVPlay.View
{
    public class ColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorsRuntimeProperties = typeof(Colors).GetRuntimeProperties();

            return colorsRuntimeProperties.Where(x => ((Color)ColorConverter.ConvertFromString(x.Name)).ToString() == (value.ToString()) ).First();            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color returnValue = Colors.Green;
            if (value.GetType().Name == "RuntimePropertyInfo")
            {                
                returnValue = (Color)ColorConverter.ConvertFromString((value as PropertyInfo).Name);
            }
            return returnValue;
        }
    }
}
