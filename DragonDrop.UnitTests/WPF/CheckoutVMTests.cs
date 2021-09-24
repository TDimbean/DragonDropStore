using DragonDrop.BLL.DataServices;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.StoreViews;
using DragonDrop.WPF.StoreViews.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows;

namespace DragonDrop.UnitTests.WPF
{
    [TestClass]
    [Ignore]
    public class CheckoutVMTests
    {
        private TestDb _db;
        private TestOrderRepo _ordRepo;
        private TestOrderItemRepo _itemRepo;
        private TestProductRepo _prodRepo;
        private ProductDataService _prodService;
        private OrderDataService _ordService;
        private OrderItemDataService _itemService;

        private StoreView _storeView;
        private StoreViewModel _storeVM;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            if (Application.ResourceAssembly == null)
                Application.ResourceAssembly = typeof(StoreView).Assembly;

            _db = new TestDb();
            _ordRepo = new TestOrderRepo(_db);
            _itemRepo = new TestOrderItemRepo(_db);
            _prodRepo = new TestProductRepo(_db);
            _ordService = new OrderDataService(_ordRepo);
            _itemService = new OrderItemDataService(_itemRepo);
            _prodService = new ProductDataService(_prodRepo);

            var cust = _db.Customers.FirstOrDefault();

            _storeView = new StoreView(false, cust);
            _storeVM = new StoreViewModel(_storeView, _prodService, false, cust);
            _vm = new PrivateObject(typeof(CheckoutViewModel), _storeVM, _ordService, _itemService, _prodService);
        }

        [TestMethod]
        public void Submit_HappyFlow_ShouldPlaceOrder()
        {
            // Arrange
            _vm.SetProperty("SelectedPayMethod", _db.PaymentMethods.FirstOrDefault());
            _vm.SetProperty("SelectedShipMethod", _db.ShippingMethods.FirstOrDefault());

            _storeVM.AddToCartCommand.Execute(_db.Products.FirstOrDefault().ProductId);

            var initProdStock = _db.Products.FirstOrDefault().Stock;
            var initOrdCount = _db.Orders.Count;
            var initItemsCount = _db.OrderItems.Count;

            // Act
            _vm.Invoke("SubmitCommandExecute", new RoutedEventArgs());
            var newProdStock = _db.Products.FirstOrDefault().Stock;
            var newOrdCount = _db.Orders.Count;
            var newItemsCount = _db.OrderItems.Count;

            // Assert
            newProdStock.Should().BeLessThan(initProdStock);
            newOrdCount.Should().BeGreaterThan(initOrdCount);
            newItemsCount.Should().BeGreaterThan(initItemsCount);
            _storeVM.Cart.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow(true,false)]
        [DataRow(false,true)]
        public void Submit_MethodsNotSelected_ShouldPlaceOrder(bool hasPayMeth, bool hasShipMeth)
        {
            // Arrange
            if (hasPayMeth)  _vm.SetProperty("SelectedPayMethod", _db.PaymentMethods.FirstOrDefault());
            if (hasShipMeth) _vm.SetProperty("SelectedShipMethod", _db.ShippingMethods.FirstOrDefault());

            _storeVM.AddToCartCommand.Execute(_db.Products.FirstOrDefault().ProductId);

            var initProdStock = _db.Products.FirstOrDefault().Stock;
            var initOrdCount = _db.Orders.Count;
            var initItemsCount = _db.OrderItems.Count;

            // Act
            _vm.Invoke("SubmitCommandExecute", new RoutedEventArgs());
            var newProdStock = _db.Products.FirstOrDefault().Stock;
            var newOrdCount = _db.Orders.Count;
            var newItemsCount = _db.OrderItems.Count;

            // Assert
            newProdStock.Should().Be(initProdStock);
            newOrdCount.Should().Be(initOrdCount);
            newItemsCount.Should().Be(initItemsCount);
            _storeVM.Cart.Should().NotBeEmpty();
        }
    }
}
