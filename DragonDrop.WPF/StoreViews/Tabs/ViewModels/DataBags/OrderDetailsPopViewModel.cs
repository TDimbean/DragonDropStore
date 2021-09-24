using DragonDrop.DAL.Entities;
using Prism.Mvvm;
using System;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class OrderDetailsPopViewModel : BindableBase
    {
        private Order _ord;

        public OrderDetailsPopViewModel(Order ord) => _ord = ord;

        public int OrderId { get => _ord.OrderId; set { } }
        public int PaymentMethodId { get => _ord.PaymentMethodId; set { } }
        public int ShippingMethodId { get => _ord.ShippingMethodId; set { } }
        public int OrderStatusId { get => _ord.OrderStatusId; set { } }
        public int CustomerId { get => _ord.CustomerId; set { } }
        public DateTime OrderDate { get => _ord.OrderDate; set { } }
        public DateTime? ShippingDate { get => _ord.ShippingDate; set { } }
    }
}
