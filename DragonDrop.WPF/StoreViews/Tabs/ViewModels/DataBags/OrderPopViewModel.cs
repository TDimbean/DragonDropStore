using DragonDrop.BLL.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using Prism.Mvvm;
using System;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class OrderPopViewModel : BindableBase
    {
        private Order _ord;

        public OrderPopViewModel(Order ord)
        {
            _ord = ord;
        }

        public string CustomerDetail
        {
            get
            {
                var container = new UnityContainer();
                container.RegisterType<ICustomerRepository, CustomerRepository>();
                var custName = container.Resolve<CustomerDataService>().Get(_ord.CustomerId).Name;

                return "Placed by: " + custName + "(ID:" + _ord.CustomerId + ")";
            }
            set { }
        }

        public int OrderId { get => _ord.OrderId; set { } }
        public DateTime OrderDate { get => _ord.OrderDate; set { } }
        public DateTime? ShippingDate { get => _ord.ShippingDate; set { } }
        public int OrderStatusId { get => _ord.OrderStatusId; set { } }
        public int ShippingMethodId { get => _ord.ShippingMethodId; set { } }
        public int PaymentMethodId { get => _ord.PaymentMethodId; set { } }
    }
}
