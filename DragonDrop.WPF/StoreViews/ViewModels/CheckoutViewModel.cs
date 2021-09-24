using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.Infrastructure;
using DragonDrop.WPF.StoreViews.Tabs.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace DragonDrop.WPF.StoreViews.ViewModels

{
    public class CheckoutViewModel : BindableBase
    {
        //Fields
        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);

        private StoreViewModel _storeVM;

        private List<PaymentMethod> _payMethods;
        private List<ShippingMethod> _shipMethods;

        private IOrderDataService _ordService;
        private IOrderItemDataService _itemService;
        private IProductDataService _prodService;

        //Ctor
        public CheckoutViewModel(StoreViewModel storeVM, IOrderDataService ordService,
            IOrderItemDataService itemService, IProductDataService prodService)
        {
            _prodService = prodService;
            _itemService = itemService;
            _ordService = ordService;
            _storeVM = storeVM;

            var container = new UnityContainer();
            var payRepo = container.Resolve<PaymentMethodRepository>();
            var shipRepo = container.Resolve<ShippingMethodRepository>();

            PayMethods = payRepo.GetAll();
            ShipMethods = shipRepo.GetAll();
            CartList = storeVM.Cart.ToList();

            SubmitCommand = new DelegateCommand<RoutedEventArgs>(SubmitCommandExecute);
        }

        #region Properties

        public List<UserOrderProd> CartList { get; set; }

        public List<PaymentMethod> PayMethods
        {
            get => _payMethods;
            set => SetProperty(ref _payMethods, value);
        }
        public List<ShippingMethod> ShipMethods
        {
            get => _shipMethods;
            set => SetProperty(ref _shipMethods, value);
        }
        public PaymentMethod SelectedPayMethod { get; set; }
        public ShippingMethod SelectedShipMethod { get; set; }

        public DelegateCommand<RoutedEventArgs> SubmitCommand { get; }

        #endregion

        private void SubmitCommandExecute(RoutedEventArgs e)
        {
            //var win = ((e.Source as Button).Parent as StackPanel).Parent as CheckoutWindow;

            if (SelectedPayMethod == null || SelectedShipMethod == null)
            {
                var incompleteInfoAlert = "Incomplete Order Information. Please make sure your" +
                            " Payment and Shipping information are set.";
                try
                {
                    (((e.Source as Button).Parent as StackPanel).Parent as CheckoutWindow)
                        .Alert(incompleteInfoAlert);
                }
                catch(Exception ex)
                {
                    StaticLogger.LogError(GetType(), "Tried to notify user of Error, but couldn't find Checkout Window,/n" +
                        "Intended Message: " + incompleteInfoAlert + "\nDetails: " + ex.Message + "\nStackTrace: " +
                        ex.StackTrace);
                }
                return;
            }

            var ord = new Order
            {
                OrderDate = DateTime.Now,
                PaymentMethodId = SelectedPayMethod.PaymentMethodId,
                ShippingMethodId = SelectedShipMethod.ShippingMethodId,
                CustomerId = _storeVM.UserId,
                OrderStatusId = 0
            };

            _ordService.Create(ord);
            var ordId = _ordService.GetAllByCustomerId(_storeVM.UserId)
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefault().OrderId;

            foreach (var item in _storeVM.Cart)
            {
                _itemService.Create(new OrderItem { OrderId = ordId, ProductId = item.ProductId, Quantity = item.Quantity });
                var oldProd = _prodService.Get(item.ProductId);
                var updProd = new Product
                {
                    ProductId = oldProd.ProductId,
                    UnitPrice = oldProd.UnitPrice,
                    Name = oldProd.Name,
                    BarCode = oldProd.BarCode,
                    CategoryId = oldProd.CategoryId,
                    Description = oldProd.Description,
                    Stock = oldProd.Stock - item.Quantity
                };
                _prodService.Update(updProd);
            }

            _storeVM.ClearCartCommand.Execute();

            _storeVM.ResetTabs();

                try
                {
                    (((e.Source as Button).Parent as StackPanel).Parent as CheckoutWindow)
                        .Stop();
                }
                catch (Exception ex)
                {
                    StaticLogger.LogError(GetType(), "Tried to close Checkout Window, but couldn't find it./n" +
                        "\nDetails: " + ex.Message + "\nStackTrace: " + ex.StackTrace);
                }
        }
    }
}
