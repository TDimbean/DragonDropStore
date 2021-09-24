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

    public class ItemEditVMTests
    {
        private TestDb _db;
        private IOrderItemRepository _repo;
        private IOrderRepository _ordRepo;
        private IProductRepository _prodRepo;
        private IOrderItemDataService _service;
        private IOrderDataService _ordService;
        private IProductDataService _prodService;

        private OrderItem _ogItem;
        private ItemEditViewModel _editVM;
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

            _ogItem = _db.OrderItems.FirstOrDefault();

            _editVM = new ItemEditViewModel(_ogItem, _service, _ordService, _prodService);
            _vm = new PrivateObject(typeof(ItemEditViewModel), _ogItem, _service, _ordService, _prodService);
        }

        #region Focus Changes

        #region Order ID

        [TestMethod]
        public void OrdLostFocus_EmptyBox_ShouldInsertOgValue()
        {
            // Arrange
            _editVM.OrdEntryText = string.Empty;

            // Act
            _editVM.OrdBoxLostFocusCommand.Execute();

            // Assert
            _editVM.OrdEntryText.Should().Be(_ogItem.OrderId.ToString());
        }

        [TestMethod]
        public void OrdLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.OrdEntryText = text;

            // Act
            _editVM.OrdBoxLostFocusCommand.Execute();

            // Assert
            _editVM.OrdEntryText.Should().Be(text);
        }

        [TestMethod]
        public void OrdReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.OrdEntryText = "939";

            // Act
            _editVM.OrdResetCommand.Execute();

            // Assert
            _editVM.OrdEntryText.Should().Be(_ogItem.OrderId.ToString());
        }

        #endregion

        #region Product ID

        [TestMethod]
        public void ProdLostFocus_EmptyBox_ShouldInsertOgValue()
        {
            // Arrange
            _editVM.ProdEntryText = string.Empty;

            // Act
            _editVM.ProdBoxLostFocusCommand.Execute();

            // Assert
            _editVM.ProdEntryText.Should().Be(_ogItem.ProductId.ToString());
        }

        [TestMethod]
        public void ProdLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.ProdEntryText = text;

            // Act
            _editVM.ProdBoxLostFocusCommand.Execute();

            // Assert
            _editVM.ProdEntryText.Should().Be(text);
        }

        [TestMethod]
        public void ProdReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.ProdEntryText = "939";

            // Act
            _editVM.ProdResetCommand.Execute();

            // Assert
            _editVM.ProdEntryText.Should().Be(_ogItem.ProductId.ToString());
        }

        #endregion

        #region Quantity

        [TestMethod]
        public void QtyLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _editVM.QtyEntryText = string.Empty;

            // Act
            _editVM.QtyBoxLostFocusCommand.Execute();

            // Assert
            _editVM.QtyEntryText.Should().Be(_ogItem.Quantity.ToString());
        }

        [TestMethod]
        public void QtyLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.QtyEntryText = text;

            // Act
            _editVM.QtyBoxLostFocusCommand.Execute();

            // Assert
            _editVM.QtyEntryText.Should().Be(text);
        }

        [TestMethod]
        public void QtyReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.QtyEntryText = "939";

            // Act
            _editVM.QtyResetCommand.Execute();

            // Assert
            _editVM.QtyEntryText.Should().Be(_ogItem.Quantity.ToString());
        }

        #endregion

        #endregion

        #region Text Updates

        [TestMethod]
        public void OrdTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.OrdEntryText = _db.Orders.FirstOrDefault().OrderId.ToString();

            // Act
            _editVM.OrdBoxTextChangedCommand.Execute();

            // Assert
            _editVM.OrdErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void ProdTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.OrdEntryText = _db.Products.FirstOrDefault().ProductId.ToString();

            // Act
            _editVM.OrdBoxTextChangedCommand.Execute();

            // Assert
            _editVM.OrdErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void QtyTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.QtyEntryText = "2";

            // Act
            _editVM.QtyBoxTextChangedCommand.Execute();

            // Assert
            _editVM.QtyErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        //Bad Order IDs

        [TestMethod]
        public void OrdTxtChanged_InvalidFormat_ShouldShowAppropriate()
        {
            // Arrange
            _editVM.OrdEntryText = "asdf";

            // Act
            _editVM.OrdBoxTextChangedCommand.Execute();

            // Assert
            _editVM.OrdErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.OrdErrorText.Should().Be("Invalid Order ID format");
        }

        [TestMethod]
        public void OrdTxtChanged_InexistentID_ShouldShowAppropriate()
        {
            // Arrange
            _editVM.OrdEntryText = Generators.GenInexistentOrderId().ToString();

            // Act
            _editVM.OrdBoxTextChangedCommand.Execute();

            // Assert
            _editVM.OrdErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.OrdErrorText.Should().Be("OrderItem needs to target a valid Order Id.");
        }

        //Bad Product IDs

        [TestMethod]
        public void ProdTxtChanged_InvalidFormat_ShouldShowAppropriate()
        {
            // Arrange
            _editVM.ProdEntryText = "asdf";

            // Act
            _editVM.ProdBoxTextChangedCommand.Execute();

            // Assert
            _editVM.ProdErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.ProdErrorText.Should().Be("Invalid Product ID format");
        }

        [TestMethod]
        public void ProdTxtChanged_InexistentID_ShouldShowAppropriate()
        {
            // Arrange
            _editVM.ProdEntryText = Generators.GenInexistentProductId().ToString();

            // Act
            _editVM.ProdBoxTextChangedCommand.Execute();

            // Assert
            _editVM.ProdErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.ProdErrorText.Should().Be("OrderItem needs to target a valid Product Id.");
        }

        //Bad Quantity

        [TestMethod]
        public void QtyTxtChanged_InvalidFormat_ShouldShowAppropriate()
        {
            // Arrange
            _editVM.QtyEntryText = "asdf";

            // Act
            _editVM.QtyBoxTextChangedCommand.Execute();

            // Assert
            _editVM.QtyErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.QtyErrorText.Should().Be("Quantity must be a positive whole number");
        }

        [TestMethod]
        public void QtyTxtChanged_NegativeNumber_ShouldShowAppropriate()
        {
            // Arrange
            _editVM.QtyEntryText = "-1";

            // Act
            _editVM.QtyBoxTextChangedCommand.Execute();

            // Assert
            _editVM.QtyErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.QtyErrorText.Should().Be("OrderItem must have a quantity greater than 0.");
        }

        #endregion

        #region Checks

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
            _vm.SetProperty("QtyEntryText", (prod.Stock + 1).ToString());

            // Act
            _vm.Invoke("CheckStock");

            // Assert
            _vm.GetProperty("ProdWarnVisibility").Should().Be(Visibility.Visible);
        }

        #endregion

        #region Submit

        [TestMethod]
        public void Submit_HappyFlow_ShouldRegisterItem()
        {
            // Arrange
            var targetItem = _db.OrderItems.FirstOrDefault();

            int newQty = targetItem.Quantity + 3;

            var oldItem = new OrderItem
            {
                Quantity = targetItem.Quantity,
                OrderId = targetItem.OrderId,
                ProductId = targetItem.ProductId
            };

            var expected = new OrderItem
            {
                Quantity = newQty,
                OrderId = targetItem.OrderId,
                ProductId = targetItem.ProductId
            };

            _editVM.OrdEntryText = targetItem.OrderId.ToString();
            _editVM.ProdEntryText = targetItem.ProductId.ToString();
            _editVM.QtyEntryText = newQty.ToString();

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(targetItem.OrderId, targetItem.ProductId);

            // Assert
            result.Should().BeEquivalentTo(expected);
            result.Should().NotBeEquivalentTo(oldItem);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void Submit_QuantityTooSmall_ShouldNotRegisterItem(int newQty)
        {
            // Arrange
            var targetItem = _db.OrderItems.FirstOrDefault();

            var oldItem = new OrderItem
            {
                Quantity = targetItem.Quantity,
                OrderId = targetItem.OrderId,
                ProductId = targetItem.ProductId
            };

            var expected = new OrderItem
            {
                Quantity = newQty,
                OrderId = targetItem.OrderId,
                ProductId = targetItem.ProductId
            };

            _editVM.OrdEntryText = targetItem.OrderId.ToString();
            _editVM.ProdEntryText = targetItem.ProductId.ToString();
            _editVM.QtyEntryText = newQty.ToString();

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(targetItem.OrderId, targetItem.ProductId);

            // Assert
            result.Should().BeEquivalentTo(oldItem);
            result.Should().NotBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Submit_InexistentOrderId_ShouldNotRegisterItem()
        {

            // Arrange
            var targetItem = _db.OrderItems.FirstOrDefault();

            int newQty = targetItem.Quantity + 3;
            var newOrdId = Generators.GenInexistentOrderId();

            var oldItem = new OrderItem
            {
                Quantity = targetItem.Quantity,
                OrderId = targetItem.OrderId,
                ProductId = targetItem.ProductId
            };

            var expected = new OrderItem
            {
                Quantity = newQty,
                OrderId = newOrdId,
                ProductId = targetItem.ProductId
            };

            _editVM.OrdEntryText = newOrdId.ToString();
            _editVM.ProdEntryText = targetItem.ProductId.ToString();
            _editVM.QtyEntryText = newQty.ToString();

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(targetItem.OrderId, targetItem.ProductId);

            // Assert
            result.Should().BeEquivalentTo(oldItem);
            result.Should().NotBeEquivalentTo(expected);
        }


        [TestMethod]
        public void Submit_InexistentProductId_ShouldNotRegisterItem()
        {
            // Arrange
            var targetItem = _db.OrderItems.FirstOrDefault();

            var newQty = targetItem.Quantity + 3;
            var newProdId = Generators.GenInexistentProductId();

            var oldItem = new OrderItem
            {
                Quantity = targetItem.Quantity,
                OrderId = targetItem.OrderId,
                ProductId = targetItem.ProductId
            };

            var expected = new OrderItem
            {
                Quantity = newQty,
                OrderId = targetItem.OrderId,
                ProductId = newProdId
            };

            _editVM.OrdEntryText = targetItem.OrderId.ToString();
            _editVM.ProdEntryText = newProdId.ToString();
            _editVM.QtyEntryText = newQty.ToString();

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(targetItem.OrderId, targetItem.ProductId);

            // Assert
            result.Should().BeEquivalentTo(oldItem);
            result.Should().NotBeEquivalentTo(expected);
        }

        #endregion
    }
}
