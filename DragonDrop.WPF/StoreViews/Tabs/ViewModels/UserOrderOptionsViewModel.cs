using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.WPF.StoreViews.Tabs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class UserOrderOptionsViewModel
    {
        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);

        private IOrderItemDataService _itemService;
        private IProductDataService _prodService;
        private Order _ord;

        public UserOrderOptionsViewModel(Order ord, IOrderItemDataService itemService, IProductDataService prodService)
        {
            _ord = ord;
            _itemService = itemService;
            _prodService = prodService;

            ProductsList = new List<UserOrderProd>();

            var ordItems = _asyncCalls ? _itemService.GetAllByOrderIdAsync(ord.OrderId).Result :
                _itemService.GetAllByOrderId(ord.OrderId);
            foreach (var item in ordItems)
            {
                var prod = _asyncCalls ? _prodService.GetAsync(item.ProductId).Result : 
                    _prodService.Get(item.ProductId);

                ProductsList.Add(new UserOrderProd
                {
                    Name = prod.Name,
                    ProductId = prod.ProductId,
                    Price = prod.UnitPrice.GetValueOrDefault(),
                    Quantity = item.Quantity
                });
            }

        }

        public List<UserOrderProd> ProductsList { get; }

        public DateTime OrderDate
        {
            get => _ord.OrderDate;
        }

        public string ShippingDate
        {
            get => !_ord.ShippingDate.HasValue ? string.Empty:
                    _ord.ShippingDate.GetValueOrDefault().ToShortDateString();
        }

        public int PaymentMethodId { get => _ord.PaymentMethodId; }
        public int ShippingMethodId{get=>_ord.ShippingMethodId;   }
        public int OrderStatusId{get=>_ord.OrderStatusId;   }
    }
}
