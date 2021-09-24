using System;
using System.Globalization;
using System.Windows.Data;

namespace DragonDrop.WPF.StoreViews.Tabs.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        //This will break so hard if it is fed any object other than a bool. Use cautiously.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => !(bool)value;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Convert(value, targetType, parameter, culture);
    }
}
