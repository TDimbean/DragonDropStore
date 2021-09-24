using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DragonDrop.WPF.StoreViews.Tabs.Converters
{
    public class IdToPaymentMethodConverter : IValueConverter
    {
        IPaymentMethodRepository _repo;

        public IdToPaymentMethodConverter()=>
            _repo = new PaymentMethodRepository(new DragonDrop_DbContext());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            =>_repo.GetMethodName(int.Parse(value.ToString()));

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            =>_repo.GetMethodId(value.ToString()).GetValueOrDefault();
    }
}
