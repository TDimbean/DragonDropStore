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

    public class PaymentEditVMTests
    {
        private TestDb _db;
        private IPaymentRepository _repo;
        private IPaymentDataService _service;

        private Payment _ogPay;
        private PaymentEditViewModel _editVM;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestPaymentRepo(_db);
            var methRepo = new TestPaymentMethodRepo(_db);
            _service = new PaymentDataService(_repo);

            _ogPay = _db.Payments.FirstOrDefault();

            _editVM = new PaymentEditViewModel(_service, methRepo, _ogPay.PaymentId);
        }

        #region Focus Changes

        #region Customer ID

        [TestMethod]
        public void CustLostFocus_EmptyBox_ShouldInsertOgValue()
        {
            // Arrange
            _editVM.CustEntryText = string.Empty;

            // Act
            _editVM.CustBoxLostFocusCommand.Execute();

            // Assert
            _editVM.CustEntryText.Should().Be(_ogPay.CustomerId.ToString());
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
            _editVM.CustEntryText = "939";

            // Act
            _editVM.CustResetCommand.Execute();

            // Assert
            _editVM.CustEntryText.Should().Be(_ogPay.CustomerId.ToString());
        }

        #endregion

        #region Amount

        [TestMethod]
        public void AmountLostFocus_EmptyBox_ShouldInsertOgValue()
        {
            // Arrange
            _editVM.AmountEntryText = string.Empty;

            // Act
            _editVM.AmountBoxLostFocusCommand.Execute();

            // Assert
            _editVM.AmountEntryText.Should().Be(_ogPay.Amount.GetValueOrDefault().ToString("C2"));
        }

        [TestMethod]
        public void AmountLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.AmountEntryText = text;

            // Act
            _editVM.AmountBoxLostFocusCommand.Execute();

            // Assert
            _editVM.AmountEntryText.Should().Be(text);
        }

        [TestMethod]
        public void AmountReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.AmountEntryText = "939";

            // Act
            _editVM.AmountResetCommand.Execute();

            // Assert
            _editVM.AmountEntryText.Should().Be(_ogPay.Amount.GetValueOrDefault().ToString("C2"));
        }

        #endregion

        #region Date

        [TestMethod]
        public void DateLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _editVM.DateEntryText = string.Empty;
            _editVM.DispDate = new DateTime(1500, 01, 01);
            _editVM.SelectedDate = new DateTime(1600, 01, 01);

            var expectedDate = _ogPay.Date.GetValueOrDefault();

            // Act
            _editVM.DateBoxLostFocusCommand.Execute();

            // Assert
            _editVM.DateEntryText.Should().Be(expectedDate.ToShortDateString());
            _editVM.DispDate.Should().Be(expectedDate);
            _editVM.SelectedDate.Should().Be(expectedDate);
        }

        [TestMethod]
        public void DateLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.DateEntryText = text;

            // Act
            _editVM.DateBoxLostFocusCommand.Execute();

            // Assert
            _editVM.DateEntryText.Should().Be(text);
        }

        [TestMethod]
        public void DateLostFocus_ContainsValidDate_ShouldFormatAllDates()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-3);
            var text = date.ToShortDateString();
            date = DateTime.Parse(text);
            _editVM.DateEntryText = text;
            _editVM.SelectedDate = new DateTime(1500, 1, 1);
            _editVM.DispDate = new DateTime(1600, 1, 1);

            // Act
            _editVM.DateBoxLostFocusCommand.Execute();

            // Assert
            _editVM.DateEntryText.Should().Be(text);
            _editVM.SelectedDate.Should().Be(date);
            _editVM.DispDate.Should().Be(date);
        }


        [TestMethod]
        public void DateReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.DateEntryText = "939";

            // Act
            _editVM.DateResetCommand.Execute();

            // Assert
            _editVM.DateEntryText.Should().Be(_ogPay.Date.GetValueOrDefault().ToShortDateString());
        }

        #endregion

        #endregion

        #region Text Updates

        [TestMethod]
        public void CustTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.CustEntryText = _db.Customers.FirstOrDefault().CustomerId.ToString();

            // Act
            _editVM.CustBoxTextChangedCommand.Execute();

            // Assert
            _editVM.CustErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void AmountTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.AmountEntryText = "12.87";

            // Act
            _editVM.AmountBoxTextChangedCommand.Execute();

            // Assert
            _editVM.AmountErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void DateTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.DateEntryText = DateTime.Now.AddDays(-7).ToShortDateString();

            // Act
            _editVM.DateBoxTextChangedCommand.Execute();

            // Assert
            _editVM.DateErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        //Bad Data

        [TestMethod]
        public void CustTxtChanged_Inexistent_ShouldShowAppropriateError()
        {
            // Arrange
            _editVM.CustEntryText = Generators.GenInexistentCustomerId().ToString();

            // Act
            _editVM.CustBoxTextChangedCommand.Execute();

            // Assert
            _editVM.CustErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.CustErrorText.Should().Be("Payments require a valid CustomerId.");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("-3")]
        public void AmountTxtChanged_ZeroOrLess_ShouldShowAppropriateError(string amount)
        {
            // Arrange
            _editVM.AmountEntryText = amount;

            // Act
            _editVM.AmountBoxTextChangedCommand.Execute();

            // Assert
            _editVM.AmountErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.AmountErrorText.Should().Be("Amount must be greater than 0.");
        }

        [TestMethod]
        public void DateTxtChanged_AfterToday_ShouldShowAppropriateError()
        {
            // Arrange
            _editVM.DateEntryText = DateTime.Now.AddDays(2).ToShortDateString();

            // Act
            _editVM.DateBoxTextChangedCommand.Execute();

            // Assert
            _editVM.DateErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.DateErrorText.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
        }

        #endregion
        
        #region Submit

        [TestMethod]
        public void Submit_HappyFlow_ShouldUpdatePayment()
        {
            // Arrange
            var pay = Generators.GenPayment();
            pay.PaymentId = _ogPay.PaymentId;

            var oldPay = new Payment
            {
                Amount = _ogPay.Amount,
                CustomerId = _ogPay.CustomerId,
                Date = _ogPay.Date.GetValueOrDefault(),
                PaymentMethodId = _ogPay.PaymentMethodId,
                PaymentId=_ogPay.PaymentId
            };

            _editVM.AmountEntryText = pay.Amount.GetValueOrDefault().ToString();
            _editVM.AmountBoxLostFocusCommand.Execute();
            _editVM.CustEntryText = pay.CustomerId.ToString();
            _editVM.DateEntryText = pay.Date.GetValueOrDefault().ToShortDateString();
            _editVM.DateBoxLostFocusCommand.Execute();
            _editVM.SelectedMethod = _db.PaymentMethods
                .SingleOrDefault(m => m.PaymentMethodId == pay.PaymentMethodId);

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(_ogPay.PaymentId);

            // Assert
            result.Should().BeEquivalentTo(pay);
            result.Should().NotBeEquivalentTo(oldPay);
        }

        [TestMethod]
        public void Submit_InexistentCustomerID_ShouldNotUpdatePayment()
        {
        // Arrange
        var pay = Generators.GenPayment();
        pay.PaymentId = _ogPay.PaymentId;

            var oldPay = new Payment
            {
                Amount = _ogPay.Amount,
                CustomerId = _ogPay.CustomerId,
                Date = _ogPay.Date.GetValueOrDefault(),
                PaymentMethodId = _ogPay.PaymentMethodId,
                PaymentId = _ogPay.PaymentId
            };

        _editVM.AmountEntryText = pay.Amount.GetValueOrDefault().ToString();
        _editVM.AmountBoxLostFocusCommand.Execute();
            _editVM.CustEntryText = Generators.GenInexistentCustomerId().ToString();
            _editVM.DateEntryText = pay.Date.GetValueOrDefault().ToShortDateString();
        _editVM.DateBoxLostFocusCommand.Execute();
            _editVM.SelectedMethod = _db.PaymentMethods
                .SingleOrDefault(m => m.PaymentMethodId == pay.PaymentMethodId);

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(_ogPay.PaymentId);

        // Assert
        result.Should().BeEquivalentTo(oldPay);
        result.Should().NotBeEquivalentTo(pay);
    }

        [TestMethod]
        public void Submit_DateAfterToday_ShouldNotUpdatePayment()
        {
            // Arrange
            var pay = Generators.GenPayment();
            pay.PaymentId = _ogPay.PaymentId;

            var oldPay = new Payment
            {
                Amount = _ogPay.Amount,
                CustomerId = _ogPay.CustomerId,
                Date = _ogPay.Date.GetValueOrDefault(),
                PaymentMethodId = _ogPay.PaymentMethodId,
                PaymentId = _ogPay.PaymentId
            };

            _editVM.AmountEntryText = pay.Amount.GetValueOrDefault().ToString();
            _editVM.AmountBoxLostFocusCommand.Execute();
            _editVM.CustEntryText = pay.CustomerId.ToString();
            _editVM.DateEntryText = DateTime.Now.AddDays(3).ToShortDateString();
            _editVM.DateBoxLostFocusCommand.Execute();
            _editVM.SelectedMethod = _db.PaymentMethods
                .SingleOrDefault(m => m.PaymentMethodId == pay.PaymentMethodId);

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(_ogPay.PaymentId);

            // Assert
            result.Should().BeEquivalentTo(oldPay);
            result.Should().NotBeEquivalentTo(pay);
        }

        [TestMethod]
        public void Submit_AmountTooSmall_ShouldNotUpdatePayment()
        {
            // Arrange
            var pay = Generators.GenPayment();
            pay.PaymentId = _ogPay.PaymentId;

            var oldPay = new Payment
            {
                Amount = _ogPay.Amount,
                CustomerId = _ogPay.CustomerId,
                Date = _ogPay.Date.GetValueOrDefault(),
                PaymentMethodId = _ogPay.PaymentMethodId,
                PaymentId = _ogPay.PaymentId
            };

            _editVM.AmountEntryText = "0";
            _editVM.AmountBoxLostFocusCommand.Execute();
            _editVM.CustEntryText = pay.CustomerId.ToString();
            _editVM.DateEntryText = pay.Date.GetValueOrDefault().ToShortDateString();
            _editVM.DateBoxLostFocusCommand.Execute();
            _editVM.SelectedMethod = _db.PaymentMethods
                .SingleOrDefault(m => m.PaymentMethodId == pay.PaymentMethodId);

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(_ogPay.PaymentId);

            // Assert
            result.Should().BeEquivalentTo(oldPay);
            result.Should().NotBeEquivalentTo(pay);
        }

        #endregion
    }
}
