using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DragonDrop.WPF.StoreViews.Tabs.Converters
{
    public class IdToCategoryConverter : IValueConverter
    {
        ICategoryRepository _repo;

        public IdToCategoryConverter()=>_repo = new CategoryRepository(new DragonDrop_DbContext());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            =>_repo.GetCategoryName(int.Parse(value.ToString()));

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
           => _repo.GetCategoryId(value.ToString()).GetValueOrDefault();
    }
}
