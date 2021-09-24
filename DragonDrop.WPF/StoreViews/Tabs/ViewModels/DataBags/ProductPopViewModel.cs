using DragonDrop.DAL.Entities;
using Prism.Mvvm;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    //public class ProductPopViewModel : BindableBase
    public class ProductPopViewModel : BindableBase
    {
        private Product _prod;

        public ProductPopViewModel(Product prod) => _prod = prod;

        public int ProductId { get => _prod.ProductId; set { } }
        public int? CategoryId { get => _prod.CategoryId; set { } }
        public string Name { get => _prod.Name; set { } }
        public string Description { get => _prod.Description; set { } }
        public int Stock { get => _prod.Stock; set { } }
        public decimal? UnitPrice { get => _prod.UnitPrice.GetValueOrDefault(); set { } }
        public string BarCode { get => _prod.BarCode; set { } }
        public string Manufacturer { get => _prod.Manufacturer; set { } }

    }
}
