using System;
using System.Globalization;
using System.Windows.Data;

namespace DragonDrop.WPF.StoreViews.Tabs.Converters
{
    public class RawCodeToUpcConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valStr = value.ToString();
            return valStr[0] + " " + valStr.Substring(1, 5) + " " + valStr.Substring(5, 5) + " " + valStr[11];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
           => value.ToString().Replace(" ", string.Empty);
    }
}
