using DragonDrop.Infrastructure.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DragonDrop.WPF.StoreViews.Tabs.Converters
{
    public class CodeOrProdToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value.ToString().IsDigitsOnly() ? 36 : 20;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
