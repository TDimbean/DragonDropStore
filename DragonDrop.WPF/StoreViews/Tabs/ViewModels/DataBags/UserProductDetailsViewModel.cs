using DragonDrop.BLL.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using Prism.Mvvm;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class UserProductDetailsViewModel : BindableBase
    {
        private Product _prod;

        public UserProductDetailsViewModel(int prodId)
        {
            var container = new UnityContainer();
            container.RegisterType<IProductRepository, ProductRepository>();

            _prod = container.Resolve<ProductDataService>().Get(prodId);
        }

        
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
