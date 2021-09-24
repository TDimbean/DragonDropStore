using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Windows;

namespace DragonDrop.UnitTests.WPF.Forms
{
    [TestClass]
    [Ignore]

    public class ItemAddVMTests
    {
        private TestDb _db;
        private IOrderItemRepository _repo;
        private IOrderRepository _ordRepo;
        private IProductRepository _prodRepo;
        private IOrderItemDataService _service;
        private IOrderDataService _ordService;
        private IProductDataService _prodService;

        private ItemAddViewModel _addVM;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestOrderItemRepo(_db);
            _ordRepo = new TestOrderRepo(_db);
            _prodRepo = new TestProductRepo(_db);
            _service = new OrderItemDataService(_repo);
            _ordService = new OrderDataService(_ordRepo);
            _prodService = new ProductDataService(_prodRepo);
            _addVM = new ItemAddViewModel(_service, _ordService, _prodService);
            _vm = new PrivateObject(typeof(ItemAddViewModel), _service, _ordService, _prodService);
        }

        [TestMethod]
        public void Submit_HappyFlow_ShouldRegisterItem()
        {
            // Arrange
            var newQty = 2;

            var newOrdId = _db.Orders.LastOrDefault().OrderId + 1;
            var newProdId = _db.Products.LastOrDefault().ProductId + 1;
            var newOrd = Generators.GenOrder();
            var newProd = Generators.GenProduct();
            newOrd.OrderId = newOrdId;
            newProd.ProductId = newProdId;

            _ordService.Create(newOrd);
            _prodService.Create(newProd);

            _addVM.OrdEntryText = newOrdId.ToString();
            _addVM.ProdEntryText = newProdId.ToString();
            _addVM.QtyEntryText = newQty.ToString();

            var expected = new OrderItem
            {
                OrderId = newOrdId,
                ProductId = newProdId,
                Quantity = newQty
            };

            // Act
            _addVM.SubmitCommand.Execute();
            var result = _service.Get(newOrdId, newProdId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void Submit_QuantityTooSmall_ShouldNotRegisterItem(int newQty)
        {
            // Arrange
            var newOrdId = _db.Orders.LastOrDefault().OrderId + 1;
            var newProdId = _db.Products.LastOrDefault().ProductId + 1;
            var newOrd = Generators.GenOrder();
            var newProd = Generators.GenProduct();
            newOrd.OrderId = newOrdId;
            newProd.ProductId = newProdId;

            _ordService.Create(newOrd);
            _prodService.Create(newProd);

            _addVM.OrdEntryText = newOrdId.ToString();
            _addVM.ProdEntryText = newProdId.ToString();
            _addVM.QtyEntryText = newQty.ToString();

            var initCount = _service.GetAll().Count();

            // Act
            _addVM.SubmitCommand.Execute();
            var result = _service.Get(newOrdId, newProdId);
            var newCount = _service.GetAll().Count();

            // Assert
            result.Should().BeNull();
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Submit_InexistentOrderId_ShouldNotRegisterItem()
        {
            // Arrange
            var newQty = 2;

            var badOrdId = Generators.GenInexistentOrderId();
            var newProdId = _db.Products.LastOrDefault().ProductId + 1;
            var newProd = Generators.GenProduct();
            newProd.ProductId = newProdId;

            _prodService.Create(newProd);

            _addVM.OrdEntryText = badOrdId.ToString();
            _addVM.ProdEntryText = newProdId.ToString();
            _addVM.QtyEntryText = newQty.ToString();

            var initCount = _service.GetAll().Count();

            // Act
            _addVM.SubmitCommand.Execute();
            var result = _service.Get(badOrdId, newProdId);
            var newCount = _service.GetAll().Count();

            // Assert
            result.Should().BeNull();
            newCount.Should().Be(initCount);
        }


        [TestMethod]
        public void Submit_InexistentProductId_ShouldNotRegisterItem()
        {
            // Arrange
            var newQty = 2;

            var newOrdId = _db.Orders.LastOrDefault().OrderId + 1;
            var badProdId = Generators.GenInexistentProductId();
            var newOrd = Generators.GenOrder();
            newOrd.OrderId = newOrdId;

            _ordService.Create(newOrd);

            _addVM.OrdEntryText = newOrdId.ToString();
            _addVM.ProdEntryText = badProdId.ToString();
            _addVM.QtyEntryText = newQty.ToString();

            var initCount = _service.GetAll().Count();

            // Act
            _addVM.SubmitCommand.Execute();
            var result = _service.Get(newOrdId, badProdId);
            var newCount = _service.GetAll().Count();

            // Assert
            result.Should().BeNull();
            newCount.Should().Be(initCount);
        }

        //TODO  https://stackoverflow.com/questions/1645815/how-can-i-programmatically-generate-keypress-events-in-c

        #region Focus Changes

        #region Order ID

        [TestMethod]
        public void OrdLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.OrdEntryText = string.Empty;

            // Act
            _addVM.OrdBoxLostFocusCommand.Execute();

            // Assert
            _addVM.OrdEntryText.Should().Be("Order ID");
        }

        [TestMethod]
        public void OrdLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.OrdEntryText = text;

            // Act
            _addVM.OrdBoxLostFocusCommand.Execute();

            // Assert
            _addVM.OrdEntryText.Should().Be(text);
        }

        [TestMethod]
        public void OrdGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.OrdEntryText = "Order ID";

            // Act
            _addVM.OrdBoxGotFocusCommand.Execute();

            // Assert
            _addVM.OrdEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void OrdGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.OrdEntryText = text;

            // Act
            _addVM.OrdBoxGotFocusCommand.Execute();

            // Assert
            _addVM.OrdEntryText.Should().Be(text);
        }

        #endregion

        #region Product ID

        [TestMethod]
        public void ProdLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.ProdEntryText = string.Empty;

            // Act
            _addVM.ProdBoxLostFocusCommand.Execute();

            // Assert
            _addVM.ProdEntryText.Should().Be("Product ID");
        }

        [TestMethod]
        public void ProdLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.ProdEntryText = text;

            // Act
            _addVM.ProdBoxLostFocusCommand.Execute();

            // Assert
            _addVM.ProdEntryText.Should().Be(text);
        }

        [TestMethod]
        public void ProdGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.ProdEntryText = "Product ID";

            // Act
            _addVM.ProdBoxGotFocusCommand.Execute();

            // Assert
            _addVM.ProdEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void ProdGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.ProdEntryText = text;

            // Act
            _addVM.ProdBoxGotFocusCommand.Execute();

            // Assert
            _addVM.ProdEntryText.Should().Be(text);
        }

        #endregion

        #region Quantity

        [TestMethod]
        public void QtyLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.QtyEntryText = string.Empty;

            // Act
            _addVM.QtyBoxLostFocusCommand.Execute();

            // Assert
            _addVM.QtyEntryText.Should().Be("Quantity");
        }

        [TestMethod]
        public void QtyLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.QtyEntryText = text;

            // Act
            _addVM.QtyBoxLostFocusCommand.Execute();

            // Assert
            _addVM.QtyEntryText.Should().Be(text);
        }

        [TestMethod]
        public void QtyGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.QtyEntryText = "Quantity";

            // Act
            _addVM.QtyBoxGotFocusCommand.Execute();

            // Assert
            _addVM.QtyEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void QtyGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.QtyEntryText = text;

            // Act
            _addVM.QtyBoxGotFocusCommand.Execute();

            // Assert
            _addVM.QtyEntryText.Should().Be(text);
        }

        #endregion

        #endregion

        #region Text Updates

        [TestMethod]
        public void OrdTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.OrdEntryText = _db.Orders.FirstOrDefault().OrderId.ToString();

            // Act
            _addVM.OrdBoxTextChangedCommand.Execute();

            // Assert
            _addVM.OrdErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void ProdTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.OrdEntryText = _db.Products.FirstOrDefault().ProductId.ToString();

            // Act
            _addVM.OrdBoxTextChangedCommand.Execute();

            // Assert
            _addVM.OrdErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void QtyTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.QtyEntryText = "2";

            // Act
            _addVM.QtyBoxTextChangedCommand.Execute();

            // Assert
            _addVM.QtyErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        //Bad Order IDs

        [TestMethod]
        public void OrdTxtChanged_InvalidFormat_ShouldShowAppropriate()
        {
            // Arrange
            _addVM.OrdEntryText = "asdf";

            // Act
            _addVM.OrdBoxTextChangedCommand.Execute();

            // Assert
            _addVM.OrdErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.OrdErrorText.Should().Be("Invalid Order ID format");
        }

        [TestMethod]
        public void OrdTxtChanged_InexistentID_ShouldShowAppropriate()
        {
            // Arrange
            _addVM.OrdEntryText = Generators.GenInexistentOrderId().ToString();

            // Act
            _addVM.OrdBoxTextChangedCommand.Execute();

            // Assert
            _addVM.OrdErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.OrdErrorText.Should().Be("OrderItem needs to target a valid Order Id.");
        }

        //Bad Product IDs

        [TestMethod]
        public void ProdTxtChanged_InvalidFormat_ShouldShowAppropriate()
        {
            // Arrange
            _addVM.ProdEntryText = "asdf";

            // Act
            _addVM.ProdBoxTextChangedCommand.Execute();

            // Assert
            _addVM.ProdErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.ProdErrorText.Should().Be("Invalid Product ID format");
        }

        [TestMethod]
        public void ProdTxtChanged_InexistentID_ShouldShowAppropriate()
        {
            // Arrange
            _addVM.ProdEntryText = Generators.GenInexistentProductId().ToString();

            // Act
            _addVM.ProdBoxTextChangedCommand.Execute();

            // Assert
            _addVM.ProdErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.ProdErrorText.Should().Be("OrderItem needs to target a valid Product Id.");
        }

        //Bad Quantity

        [TestMethod]
        public void QtyTxtChanged_InvalidFormat_ShouldShowAppropriate()
        {
            // Arrange
            _addVM.QtyEntryText = "asdf";

            // Act
            _addVM.QtyBoxTextChangedCommand.Execute();

            // Assert
            _addVM.QtyErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.QtyErrorText.Should().Be("Quantity must be a positive whole number");
        }

        [TestMethod]
        public void QtyTxtChanged_NegativeNumber_ShouldShowAppropriate()
        {
            // Arrange
            _addVM.QtyEntryText = "-1";

            // Act
            _addVM.QtyBoxTextChangedCommand.Execute();

            // Assert
            _addVM.QtyErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.QtyErrorText.Should().Be("OrderItem must have a quantity greater than 0.");
        }

        #endregion

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void CheckStatus_OrderShippedOrDelivered_ShouldShowWarning(int statId)
        {
            // Arrange
            var targOrd = Generators.GenOrder();
            targOrd.OrderId = _db.Orders.LastOrDefault().OrderId + 1;
            targOrd.OrderStatusId = statId;
            targOrd.OrderDate = DateTime.Now.AddDays(-10);
            targOrd.ShippingDate = DateTime.Now.AddDays(-2);

            _ordService.Create(targOrd);
            _vm.SetProperty("OrdEntryText", targOrd.OrderId.ToString());

            // Act
            _vm.Invoke("CheckStatus");

            // Assert
            _vm.GetProperty("OrdWarnVisibility").Should().Be(Visibility.Visible);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void CheckStatus_OrderReceivedOrProcesed_ShouldHideWarning(int statId)
        {
            // Arrange
            var targOrd = Generators.GenOrder();
            targOrd.OrderId = _db.Orders.LastOrDefault().OrderId + 1;
            targOrd.OrderStatusId = statId;

            _ordService.Create(targOrd);
            _vm.SetProperty("OrdEntryText", targOrd.OrderId.ToString());

            // Act
            _vm.Invoke("CheckStatus");

            // Assert
            _vm.GetProperty("OrdWarnVisibility").Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void CheckStock_SufficientStock_ShouldHideWarning()
        {
            // Arrange
            var prod = _db.Products.Where(p => p.Stock >= 1).FirstOrDefault();
            _vm.SetProperty("ProdEntryText", prod.ProductId.ToString());
            _vm.SetProperty("QtyEntryText", prod.Stock.ToString());

            // Act
            _vm.Invoke("CheckStock");

            // Assert
            _vm.GetProperty("ProdWarnVisibility").Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void CheckStock_InsufficientStock_ShouldShowWarning()
        {
            // Arrange
            var prod = _db.Products.Where(p => p.Stock >= 1).FirstOrDefault();
            _vm.SetProperty("ProdEntryText", prod.ProductId.ToString());
            _vm.SetProperty("QtyEntryText", (prod.Stock+1).ToString());

            // Act
            _vm.Invoke("CheckStock");

            // Assert
            _vm.GetProperty("ProdWarnVisibility").Should().Be(Visibility.Visible);
        }
    }
}
