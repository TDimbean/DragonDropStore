using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DragonDrop.WPF.StoreViews.Tabs.Converters
{
    public class IdToShippingMethodConverter : IValueConverter
    {
        IShippingMethodRepository _repo;

        public IdToShippingMethodConverter()
        {
            _repo = new ShippingMethodRepository(new DragonDrop_DbContext());
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _repo.GetMethodName(int.Parse(value.ToString()));
            //try
            //{
            //    switch (int.Parse(value.ToString()))
            //    {
            //        case 1:
            //            return "Credit Card";
            //        case 2:
            //            return "Cash";
            //        case 3:
            //            return "PayPal";
            //        case 4:
            //            return "Wire Transfer";
            //        default:
            //            return "ERROR";
            //    }
            //}

            //catch (NullReferenceException ex)
            //{
            //    StaticLogger.LogError(GetType(), "Payment Method ID to String converter failed. The integer ID value given was, in fact, not an int. Details: " + ex.Message + "\n Stack Trace: " + ex.StackTrace);
            //}
            //catch (Exception ex)
            //{
            //    StaticLogger.LogError(GetType(), "Payment Method ID to String converter encountered and unexpected error. Details: " + ex.Message + "\n Stack Trace: " + ex.StackTrace);
            //}
            return "ERROR";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _repo.GetMethodId(value.ToString()).GetValueOrDefault();

            //if (value is string)
            //{
            //    switch (value.ToString().ToUpper())
            //    {
            //        case "CREDIT CARD":
            //            return 1;
            //        case "CASH":
            //            return 2;
            //        case "PAYPAL":
            //            return 3;
            //        case "WIRE TRANSFER":
            //            return 4;
            //        default:
            //            return -1;
            //    }
            //}
            //return -1;
        }
    }
}
