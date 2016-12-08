using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;

namespace IV_Play.View
{
    public class ColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var s = typeof(Colors).GetRuntimeProperties();
            


            var a = s.Where(x => ((Color)ColorConverter.ConvertFromString(x.Name)).ToString() == (value.ToString()) ).First();
            return a;
            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return value;

            Color returnValue = Colors.Green;
            if (value.GetType().Name == "RuntimePropertyInfo")
            {                
                returnValue = (Color)ColorConverter.ConvertFromString((value as PropertyInfo).Name);
            }
            return returnValue;
            //throw new NotImplementedException();
        }
    }
}
