using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Interfaces;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows;

namespace DragonDrop.UnitTests.WPF.Tabs
{
    [TestClass]
    public class AdminItemVMTests
    {
        private TestDb _db;
        private IOrderItemRepository _itemRepo;
        private IOrderRepository _ordRepo;
        private IProductRepository _prodRepo;
        private IOrderItemDataService _itemService;
        private IOrderDataService _ordService;
        private IProductDataService _prodService;

        private AdminItemsTabViewModel _tabVM;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _itemRepo = new TestOrderItemRepo(_db);
            _ordRepo = new TestOrderRepo(_db);
            _prodRepo = new TestProductRepo(_db);
            _itemService = new OrderItemDataService(_itemRepo);
            _ordService = new OrderDataService(_ordRepo);
            _prodService = new ProductDataService(_prodRepo);

            _tabVM = new AdminItemsTabViewModel(_itemService, _ordService, _prodService);
            _vm = new PrivateObject(typeof(AdminItemsTabViewModel), _itemService, _ordService, _prodService);
        }

        [TestMethod]
        public void IdTxtChanged_RightLength_ShouldChangeNothing()
        {
            // Arrange
            var idText = "46";
            _tabVM.IdText = idText;

            // Act
            _tabVM.IdTextChangedCommand.Execute();

            // Assert
            _tabVM.IdText.Should().Be(idText);
        }

        [TestMethod]
        public void IdTxtChanged_TooLong_ShouldLimitText()
        {
            // Arrange
            var idText = "1234567890";
            _tabVM.IdText = idText;

            // Act
            _tabVM.IdTextChangedCommand.Execute();

            // Assert
            _tabVM.IdText.Should().Be(idText.Substring(0,9));
        }

        [TestMethod]
        public void UpdateDetails_SelectionEmpty_ShouldDisplayDefaults()
        {
            // Arrange
            _tabVM.SelItem = null;

            // Act
            _tabVM.UpdateDetailsCommand.Execute();

            // Assert
            _tabVM.OrderIdLabel.Should().Be(string.Empty);
            _tabVM.ProductIdLabel.Should().Be(string.Empty);
            
            _tabVM.OrderDateLbl.Should().Be(string.Empty);
            _tabVM.ShippingDateLbl.Should().Be(string.Empty);
            _tabVM.OrderStatusIdLbl.Should().Be(0);
            _tabVM.ShippingMethodIdLbl.Should().Be(0);
            _tabVM.PaymentMethodIdLbl.Should().Be(0);
            
            _tabVM.ProdNameLbl.Should().Be(string.Empty);
            _tabVM.CategoryIdLbl.Should().Be(0);
            _tabVM.UnitPriceLbl.Should().Be(string.Empty);
            _tabVM.StockLbl.Should().Be(0);
            _tabVM.DescLbl.Should().Be(string.Empty);
            _tabVM.BarCodeLbl.Should().Be(string.Empty);
            _tabVM.ManufacturerLbl.Should().Be(string.Empty);
            _tabVM.ShipTimeVis.Should().Be(Visibility.Hidden);
        }

        [TestMethod]
        public void UpdateDetails_ItemSelected_ShouldPopulateDetails()
        {
            // Arrange
            var item = _db.OrderItems.FirstOrDefault();
            var ord = _ordService.Get(item.OrderId);
            var prod = _prodService.Get(item.ProductId);
            _tabVM.SelItem = item;

            // Act
            _tabVM.UpdateDetailsCommand.Execute();

            // Assert
            _tabVM.OrderIdLabel.Should().Be(ord.OrderId.ToString());
            _tabVM.ProductIdLabel.Should().Be(prod.ProductId.ToString());

            _tabVM.OrderDateLbl.Should().Be(ord.OrderDate.ToShortDateString());
            _tabVM.ShippingDateLbl.Should().Be(ord.ShippingDate.GetValueOrDefault().ToShortDateString());
            _tabVM.OrderStatusIdLbl.Should().Be(ord.OrderStatusId);
            _tabVM.ShippingMethodIdLbl.Should().Be(ord.ShippingMethodId);
            _tabVM.PaymentMethodIdLbl.Should().Be(ord.PaymentMethodId);

            _tabVM.ProdNameLbl.Should().Be(prod.Name);
            _tabVM.CategoryIdLbl.Should().Be(prod.CategoryId.GetValueOrDefault());
            _tabVM.UnitPriceLbl.Should().Be(string.Format("{0:C}", prod.UnitPrice.GetValueOrDefault()));
            _tabVM.StockLbl.Should().Be(prod.Stock);
            _tabVM.DescLbl.Should().Be(prod.Description);
            _tabVM.BarCodeLbl.Should().Be(prod.BarCode.Substring(0, 1) + " " +
                        prod.BarCode.Substring(1, 5) + " " +
                        prod.BarCode.Substring(6, 5) + " " +
                        prod.BarCode.Substring(11, 1));
            _tabVM.ManufacturerLbl.Should().Be(prod.Manufacturer);
            if (_tabVM.ShippingDateLbl == "1/1/0001")
                _tabVM.ShipTimeVis.Should().Be(Visibility.Hidden);
            else
                _tabVM.ShipTimeVis.Should().Be(Visibility.Visible);
        }
    }
}
