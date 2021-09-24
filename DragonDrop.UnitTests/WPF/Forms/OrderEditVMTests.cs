using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews;
using DragonDrop.WPF.StoreViews.Tabs.SubViews;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using DragonDrop.WPF.StoreViews.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DragonDrop.UnitTests.WPF.Forms
{
    [TestClass]
    [Ignore]

    public class OrderEditVMTests
    {
        private TestDb _db;
        private IOrderRepository _repo;
        private IOrderDataService _service;

        private Order _ogOrd;
        private OrderEditViewModel _editVM;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestOrderRepo(_db);
            _service = new OrderDataService(_repo);
            var prodRepo = new TestProductRepo(_db);
            var itemRepo = new TestOrderItemRepo(_db);
            var itemServ = new OrderItemDataService(itemRepo);
            var prodServ = new ProductDataService(prodRepo);

            _ogOrd = _db.Orders.FirstOrDefault();

            var payMethRepo = new TestPaymentMethodRepo(_db);
            var shipMethRepo = new TestShippingMethodRepo(_db);
            var ordStatRepo = new TestOrderStatusRepo(_db);

            var storeView = new PrivateObject(typeof(StoreView),false, null);
            List<Tab> tabs = (storeView.GetProperty("Tabs") as IEnumerable<Tab>).ToList();
            var realStore = new StoreView(false, null);

            var callingView = new OrderEditWindow(_ogOrd.OrderId, tabs[3] as IReloadableControl, realStore);

            var callingCul = callingView as IReloadableControl;

            _editVM = new OrderEditViewModel(_service, payMethRepo, shipMethRepo, ordStatRepo,_ogOrd.OrderId, prodServ, itemServ, callingView);
        }

        #region Focus Changes

        #region Order Date

        [TestMethod]
        public void OrdDateLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _editVM.OrdDateEntryText = string.Empty;

            // Act
            _editVM.OrdDateBoxLostFocusCommand.Execute();

            // Assert
            _editVM.OrdDateEntryText.Should().Be(_ogOrd.OrderDate.ToShortDateString());
        }

        [TestMethod]
        public void OrdDateLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.OrdDateEntryText = text;

            // Act
            _editVM.OrdDateBoxLostFocusCommand.Execute();

            // Assert
            _editVM.OrdDateEntryText.Should().Be(text);
        }

        [TestMethod]
        public void OrdDateReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.OrdDateEntryText = "parap";

            // Act
            _editVM.OrdDateResetCommand.Execute();

            // Assert
            _editVM.OrdDateEntryText.Should().Be(_ogOrd.OrderDate.ToShortDateString());
        }

        #endregion

        #region Shipping Date

        [TestMethod]
        public void ShipDateLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _editVM.ShipDateEntryText = string.Empty;

            // Act
            _editVM.ShipDateBoxLostFocusCommand.Execute();

            // Assert
            _editVM.ShipDateEntryText.Should().Be(_ogOrd.ShippingDate.HasValue?
                _ogOrd.ShippingDate.GetValueOrDefault().ToShortDateString():string.Empty);
        }

        [TestMethod]
        public void ShipDateLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.ShipDateEntryText = text;

            // Act
            _editVM.ShipDateBoxLostFocusCommand.Execute();

            // Assert
            _editVM.ShipDateEntryText.Should().Be(text);
        }

        [TestMethod]
        public void ShipDateReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            var initString = "parap";
            _editVM.ShipDateEntryText = initString;

            // Act
            _editVM.ShipDateResetCommand.Execute();

            // Assert
            _editVM.ShipDateEntryText.Should().Be(_ogOrd.ShippingDate.HasValue?
                _ogOrd.ShippingDate.GetValueOrDefault().ToShortDateString():initString);
        }

        #endregion

        #region Customer Id

        [TestMethod]
        public void CustLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _editVM.CustEntryText = string.Empty;

            // Act
            _editVM.CustBoxLostFocusCommand.Execute();

            // Assert
            _editVM.CustEntryText.Should().Be(_ogOrd.CustomerId.ToString());
        }

        [TestMethod]
        public void CustLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.CustEntryText = text;

            // Act
            _editVM.CustBoxLostFocusCommand.Execute();

            // Assert
            _editVM.CustEntryText.Should().Be(text);
        }

        [TestMethod]
        public void CustReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.CustEntryText = "pap";

            // Act
            _editVM.CustResetCommand.Execute();

            // Assert
            _editVM.CustEntryText.Should().Be(_ogOrd.CustomerId.ToString());
        }

        #endregion

        [TestMethod]
        public void PayMethReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.PayMethSelIndex = _db.PaymentMethods
                .Where(p => p.PaymentMethodId != _ogOrd.PaymentMethodId).FirstOrDefault()
                .PaymentMethodId;

            // Act
            _editVM.PayMethResetCommand.Execute();

            // Assert
            _editVM.PayMethSelIndex.Should().Be(_ogOrd.PaymentMethodId - 1);
        }

        [TestMethod]
        public void ShipMethReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.ShipMethSelIndex = _db.ShippingMethods
                .Where(s=>s.ShippingMethodId != _ogOrd.ShippingMethodId).FirstOrDefault()
                .ShippingMethodId;

            // Act
            _editVM.ShipMethResetCommand.Execute();

            // Assert
            _editVM.ShipMethSelIndex.Should().Be(_ogOrd.ShippingMethodId - 1);
        }

        [TestMethod]
        public void OrdStatReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.StatusSelIndex = _db.OrderStatuses
                .Where(s => s.OrderStatusId != _ogOrd.OrderStatusId).FirstOrDefault().OrderStatusId;

            // Act
            _editVM.StatResetCommand.Execute();

            // Assert
            _editVM.StatusSelIndex.Should().Be(_ogOrd.OrderStatusId);
        }

        #endregion

        #region Text Updates

        [TestMethod]
        public void OrdDateTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.OrdDateEntryText = _db.Orders.FirstOrDefault().OrderDate.ToString();

            // Act
            _editVM.OrdDateBoxTextChangedCommand.Execute();

            // Assert
            _editVM.OrdErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void ShipDateTxtChanged_ValidWithOrderAndRightStatus_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.OrdDateEntryText = DateTime.Now.AddDays(-10).ToShortDateString();
            _editVM.ShipDateEntryText = DateTime.Now.AddDays(-7).ToShortDateString();
            _editVM.SelectedStatus = _db.OrderStatuses.FirstOrDefault();

            // Act
            _editVM.ShipDateBoxTextChangedCommand.Execute();

            // Assert
            _editVM.ShipErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void CustTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.CustEntryText = "2";

            // Act
            _editVM.CustBoxTextChangedCommand.Execute();

            // Assert
            _editVM.CustErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        // Bad Data

        [TestMethod]
        public void OrderDateTxtChanged_AfterToday_ShouldShowAppropriateError()
        {
            // Arrange
            _editVM.OrdDateEntryText = DateTime.Now.AddDays(20).ToShortDateString();

            // Act
            _editVM.OrdDateBoxTextChangedCommand.Execute();

            // Assert
            _editVM.OrdErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.OrdErrorText.Should()
                .Be("Order Date too far into the future; Must be no later than right now.");
        }

        [TestMethod]
        public void ShipDateTxtChanged_BeforeOrdered_ShouldShowAppropriateError()
        {
            // Arrange
            _editVM.OrdDateEntryText = DateTime.Now.AddDays(-3).ToShortDateString();
            _editVM.ShipDateEntryText = DateTime.Now.AddDays(-5).ToShortDateString();
            _editVM.SelectedStatus = _db.OrderStatuses.FirstOrDefault();

            // Act
            _editVM.ShipDateBoxTextChangedCommand.Execute();

            // Assert
            _editVM.ShipErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.ShipErrorText.Should().Be("Orders cannot be Shipped before being placed.");
        }

        [TestMethod]
        public void CustTxtChanged_InvalidFormat_ShouldShowAppropriateError()
        {
            // Arrange
            _editVM.CustEntryText = "asdf";

            // Act
            _editVM.CustBoxTextChangedCommand.Execute();

            // Assert
            _editVM.CustErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.CustErrorText.Should().Be("Invalid Customer ID format");
        }

        [TestMethod]
        public void CustTxtChanged_InexistentID_ShouldShowAppropriateError()
        {
            // Arrange
            var badCustId = Generators.GenInexistentCustomerId().ToString();
            _editVM.CustEntryText = badCustId;

            // Act
            _editVM.CustBoxTextChangedCommand.Execute();

            // Assert
            _editVM.CustErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.CustErrorText.Should().Be("No Customer with ID: " + badCustId +
                " found in Repo.");
        }

        #endregion

        [TestMethod]
        public void Submit_HappyFlow_ShouldUpdateOrder()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();

            var oldOrd = new Order
            {
                ShippingDate = target.ShippingDate,
                ShippingMethodId = target.ShippingMethodId,
                OrderStatusId = target.OrderStatusId,
                CustomerId = target.CustomerId,
                OrderDate = target.OrderDate,
                OrderId = target.OrderId,
                PaymentMethodId = target.PaymentMethodId
            };

            var newOrd = Generators.GenOrder();


            _editVM.CustEntryText = newOrd.CustomerId.ToString();
            _editVM.SelectedOrdDate = newOrd.OrderDate;
            _editVM.SelectedShipDate = newOrd.ShippingDate.GetValueOrDefault();
            _editVM.SelectedStatus = _db.OrderStatuses.SingleOrDefault(s => s.OrderStatusId == newOrd.OrderStatusId);
            _editVM.SelectedPayMethod = _db.PaymentMethods.SingleOrDefault(p => p.PaymentMethodId == newOrd.PaymentMethodId);
            _editVM.SelectedShipMethod = _db.ShippingMethods.SingleOrDefault(s => s.ShippingMethodId == newOrd.ShippingMethodId);

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(target.OrderId);

            // Assert
            result.Should().BeEquivalentTo(target);
            result.Should().NotBeEquivalentTo(newOrd);
        }

        [DataTestMethod]
        [DataRow("1Jan2012", "3Jan2012", -1, 0)]
        [DataRow("1Jan2022", "3Jan2022", 1, 0)]
        [DataRow("4Jan2012", "3Jan2012", 1, 0)]
        [DataRow("1Jan2012", null, 1, 3)]
        public void Submit_BadData_ShouldNotUpdateOrder(string ordDateStr, string shipDateStr, int custId, int statId)
        {
            // Arrange
            var oldOrd = new Order
            {
                ShippingDate = _ogOrd.ShippingDate,
                ShippingMethodId = _ogOrd.ShippingMethodId,
                OrderStatusId = _ogOrd.OrderStatusId,
                CustomerId = _ogOrd.CustomerId,
                OrderDate = _ogOrd.OrderDate,
                OrderId = _ogOrd.OrderId,
                PaymentMethodId = _ogOrd.PaymentMethodId
            };

            var newOrd = new Order
            {
                OrderDate = DateTime.Parse(ordDateStr),
                ShippingDate = string.IsNullOrEmpty(shipDateStr) ? (DateTime?)null : DateTime.Parse(shipDateStr),
                CustomerId = custId,
                ShippingMethodId = _db.ShippingMethods.FirstOrDefault().ShippingMethodId,
                PaymentMethodId = _db.PaymentMethods.FirstOrDefault().PaymentMethodId,
                OrderStatusId = _db.OrderStatuses.SingleOrDefault(s => s.OrderStatusId == statId).OrderStatusId
            };

            _editVM.CustEntryText = newOrd.CustomerId.ToString();
            _editVM.SelectedOrdDate = newOrd.OrderDate;
            _editVM.SelectedShipDate = newOrd.ShippingDate.GetValueOrDefault();
            _editVM.SelectedStatus = _db.OrderStatuses
                .SingleOrDefault(s => s.OrderStatusId == newOrd.OrderStatusId);
            _editVM.SelectedPayMethod = _db.PaymentMethods
                .SingleOrDefault(p => p.PaymentMethodId == newOrd.PaymentMethodId);
            _editVM.SelectedShipMethod = _db.ShippingMethods
                .SingleOrDefault(s => s.ShippingMethodId == newOrd.ShippingMethodId);

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(_ogOrd.OrderId);

            // Assert
            result.Should().BeEquivalentTo(_ogOrd);
            result.Should().NotBeEquivalentTo(newOrd);
        }

    }
}
