using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
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

    public class OrderAddVMTests
    {
        private TestDb _db;
        private IOrderRepository _repo;
        private IOrderDataService _service;

        private OrderAddViewModel _addVM;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestOrderRepo(_db);
            _service = new OrderDataService(_repo);

            var payMethRepo = new TestPaymentMethodRepo(_db);
            var shipMethRepo = new TestShippingMethodRepo(_db);
            var ordStatRepo = new TestOrderStatusRepo(_db);

            _addVM = new OrderAddViewModel(_service, payMethRepo, shipMethRepo, ordStatRepo);
            _vm = new PrivateObject(typeof(OrderAddViewModel), _service, payMethRepo, shipMethRepo, ordStatRepo);
        }

        #region Focus Changes

        #region Order Date

        [TestMethod]
        public void OrdDateLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.OrdDateEntryText = string.Empty;

            // Act
            _addVM.OrdDateBoxLostFocusCommand.Execute();

            // Assert
            _addVM.OrdDateEntryText.Should().Be("Placed On");
        }

        [TestMethod]
        public void OrdDateLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.OrdDateEntryText = text;

            // Act
            _addVM.OrdDateBoxLostFocusCommand.Execute();

            // Assert
            _addVM.OrdDateEntryText.Should().Be(text);
        }

        [TestMethod]
        public void OrdDateGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.OrdDateEntryText = "Placed On";

            // Act
            _addVM.OrdDateBoxGotFocusCommand.Execute();

            // Assert
            _addVM.OrdDateEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void OrdDateGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.OrdDateEntryText = text;

            // Act
            _addVM.OrdDateBoxGotFocusCommand.Execute();

            // Assert
            _addVM.OrdDateEntryText.Should().Be(text);
        }

        #endregion

        #region Shipping Date

        [TestMethod]
        public void ShipDateLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.ShipDateEntryText = string.Empty;

            // Act
            _addVM.ShipDateBoxLostFocusCommand.Execute();

            // Assert
            _addVM.ShipDateEntryText.Should().Be("Shipped");
        }

        [TestMethod]
        public void ShipDateLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.ShipDateEntryText = text;

            // Act
            _addVM.ShipDateBoxLostFocusCommand.Execute();

            // Assert
            _addVM.ShipDateEntryText.Should().Be(text);
        }

        [TestMethod]
        public void ShipDateGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.ShipDateEntryText = "Shipped";

            // Act
            _addVM.ShipDateBoxGotFocusCommand.Execute();

            // Assert
            _addVM.ShipDateEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void ShipDateGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.ShipDateEntryText = text;

            // Act
            _addVM.ShipDateBoxGotFocusCommand.Execute();

            // Assert
            _addVM.ShipDateEntryText.Should().Be(text);
        }

        #endregion

        #region Customer Id

        [TestMethod]
        public void CustLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.CustEntryText = string.Empty;

            // Act
            _addVM.CustBoxLostFocusCommand.Execute();

            // Assert
            _addVM.CustEntryText.Should().Be("Customer ID");
        }

        [TestMethod]
        public void CustLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.CustEntryText = text;

            // Act
            _addVM.CustBoxLostFocusCommand.Execute();

            // Assert
            _addVM.CustEntryText.Should().Be(text);
        }

        [TestMethod]
        public void CustGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.CustEntryText = "Customer ID";

            // Act
            _addVM.CustBoxGotFocusCommand.Execute();

            // Assert
            _addVM.CustEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void CustGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.CustEntryText = text;

            // Act
            _addVM.CustBoxGotFocusCommand.Execute();

            // Assert
            _addVM.CustEntryText.Should().Be(text);
        }

        #endregion

        #endregion

        #region Text Updates

        [TestMethod]
        public void OrdDateTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.OrdDateEntryText = _db.Orders.FirstOrDefault().OrderDate.ToString();

            // Act
            _addVM.OrdDateBoxTextChangedCommand.Execute();

            // Assert
            _addVM.OrdDateErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void ShipDateTxtChanged_ValidWithOrderAndRightStatus_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.OrdDateEntryText = DateTime.Now.AddDays(-10).ToShortDateString();
            _addVM.ShipDateEntryText = DateTime.Now.AddDays(-7).ToShortDateString();
            _addVM.SelectedStatus = _db.OrderStatuses.FirstOrDefault();

            // Act
            _addVM.ShipDateBoxTextChangedCommand.Execute();

            // Assert
            _addVM.ShipDateErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void CustTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.CustEntryText = "2";

            // Act
            _addVM.CustBoxTextChangedCommand.Execute();

            // Assert
            _addVM.CustErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        // Bad Data

        [TestMethod]
        public void OrderDateTxtChanged_AfterToday_ShouldShowAppropriateError()
        {
            // Arrange
            _addVM.OrdDateEntryText = DateTime.Now.AddDays(20).ToShortDateString();

            // Act
            _addVM.OrdDateBoxTextChangedCommand.Execute();

            // Assert
            _addVM.OrdDateErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.OrdErrorText.Should()
                .Be("Order Date too far into the future; Must be no later than right now.");
        }

        [TestMethod]
        public void ShipDateTxtChanged_BeforeOrdered_ShouldShowAppropriateError()
        {
            // Arrange
            _addVM.OrdDateEntryText = DateTime.Now.AddDays(-3).ToShortDateString();
            _addVM.ShipDateEntryText = DateTime.Now.AddDays(-5).ToShortDateString();
            _addVM.SelectedStatus = _db.OrderStatuses.FirstOrDefault();

            // Act
            _addVM.ShipDateBoxTextChangedCommand.Execute();

            // Assert
            _addVM.ShipDateErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.ShipErrorText.Should().Be("Orders cannot be Shipped before being placed.");
        }

        [TestMethod]
        public void CustTxtChanged_InvalidFormat_ShouldShowAppropriateError()
        {
            // Arrange
            _addVM.CustEntryText = "asdf";

            // Act
            _addVM.CustBoxTextChangedCommand.Execute();

            // Assert
            _addVM.CustErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.CustErrorText.Should().Be("Invalid Customer ID format");
        }

        [TestMethod]
        public void CustTxtChanged_InexistentID_ShouldShowAppropriateError()
        {
            // Arrange
            var badCustId = Generators.GenInexistentCustomerId().ToString();
            _addVM.CustEntryText = badCustId;

            // Act
            _addVM.CustBoxTextChangedCommand.Execute();

            // Assert
            _addVM.CustErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.CustErrorText.Should().Be("No Customer with ID: " + badCustId + 
                " found in Repo.");
        }

        #endregion

        [TestMethod]
        public void Submit_HappyFlow_ShouldRegisterOrder()
        {
            // Arrange
            var newOrd = Generators.GenOrder();

            _addVM.CustEntryText = newOrd.CustomerId.ToString();
            _addVM.SelectedOrdDate = newOrd.OrderDate;
            _addVM.SelectedShipDate = newOrd.ShippingDate.GetValueOrDefault();
            _addVM.SelectedStatus = _db.OrderStatuses.SingleOrDefault(s => s.OrderStatusId == newOrd.OrderStatusId);
            _addVM.SelectedPayMethod = _db.PaymentMethods.SingleOrDefault(p => p.PaymentMethodId == newOrd.PaymentMethodId);
            _addVM.SelectedShipMethod = _db.ShippingMethods.SingleOrDefault(s => s.ShippingMethodId == newOrd.ShippingMethodId);

            // Act
            _addVM.SubmitCommand.Execute();
            var result = _service.GetAllFiltered(newOrd.OrderDate.ToShortDateString()).SingleOrDefault();

            // Assert
            result.Should().BeEquivalentTo(newOrd);
        }

        [DataTestMethod]
        [DataRow("1Jan2012", "3Jan2012", -1, 0)]
        [DataRow("1Jan2022", "3Jan2022", 1, 0)]
        [DataRow("4Jan2012", "3Jan2012", 1, 0)]
        [DataRow("1Jan2012", null, 1,3)]
        public void Submit_BadData_ShouldNotRegisterOrder(string ordDateStr, string shipDateStr, int custId, int statId)
        {
            // Arrange
            var ordDate = DateTime.Parse(ordDateStr);
            var shipDate = string.IsNullOrEmpty(shipDateStr) ? (DateTime?)null : DateTime.Parse(shipDateStr);

            _addVM.CustEntryText = custId.ToString();
            _addVM.SelectedOrdDate = ordDate;
            if(shipDate!=null) _addVM.SelectedShipDate = shipDate.GetValueOrDefault();
            _addVM.SelectedStatus = _db.OrderStatuses.SingleOrDefault(s=>s.OrderStatusId==statId);
            _addVM.SelectedPayMethod = _db.PaymentMethods.FirstOrDefault();
            _addVM.SelectedShipMethod = _db.ShippingMethods.FirstOrDefault();

            var initCount = _service.GetAll().Count();

            // Act
            _addVM.SubmitCommand.Execute();
            var newCount = _service.GetAll().Count();

            // Assert
            newCount.Should().Be(initCount);
        }

    }
}
