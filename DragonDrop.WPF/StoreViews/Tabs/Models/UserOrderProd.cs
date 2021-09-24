using Prism.Mvvm;

namespace DragonDrop.WPF.StoreViews.Tabs.Models
{
    public class UserOrderProd:BindableBase
    {
        private int _productId;
        private int _quantity;
        private string _name;
        private decimal _price;

        public int ProductId { get=>_productId; set=>SetProperty(ref _productId, value); }
        public int Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }
        public string Name { get=>_name; set=>SetProperty(ref _name, value); }
        public decimal Price { get=>_price; set=>SetProperty(ref _price,value); }
    }
}
