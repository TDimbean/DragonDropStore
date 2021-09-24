using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DragonDrop.WPF.StoreViews.Tabs.Converters
{
    public class IdToOrderStatusConverter : IValueConverter
    {
        IOrderStatusRepository _repo;

        public IdToOrderStatusConverter()
        {
            _repo = new OrderStatusRepository(new DragonDrop_DbContext());
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => _repo.GetStatusName(int.Parse(value.ToString()));

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => _repo.GetStatusId(value.ToString()).GetValueOrDefault();
    }
}
