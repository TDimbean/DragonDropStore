using System;
using System.Globalization;
using System.Windows.Data;

namespace DragonDrop.WPF.StoreViews.Tabs.Converters
{
    public class CurrencyFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var decVal = 0m;
            var parses = decimal.TryParse(value.ToString(), out decVal);

            return string.Format("{0:C}", decVal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
