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

    public class PaymentAddVMTests
    {
        private TestDb _db;
        private IPaymentRepository _repo;
        private IPaymentDataService _service;

        private PaymentAddViewModel _addVM;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestPaymentRepo(_db);
            var methRepo = new TestPaymentMethodRepo(_db);
            _service = new PaymentDataService(_repo);

            _addVM = new PaymentAddViewModel(_service, methRepo);
        }

        #region Focus Changes

        #region Customer ID

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

        #region Amount

        [TestMethod]
        public void AmountLostFocus_EmptyBox_ShouldInsertPlaceHolder()
        {
            // Arrange
            _addVM.AmountEntryText = string.Empty;

            // Act
            _addVM.AmountBoxLostFocusCommand.Execute();

            // Assert
            _addVM.AmountEntryText.Should().Be("Amount");
        }

        [TestMethod]
        public void AmountLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.AmountEntryText = text;

            // Act
            _addVM.AmountBoxLostFocusCommand.Execute();

            // Assert
            _addVM.AmountEntryText.Should().Be(text);
        }

        [TestMethod]
        public void AmountGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.AmountEntryText = "Amount";

            // Act
            _addVM.AmountBoxGotFocusCommand.Execute();

            // Assert
            _addVM.AmountEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void AmountGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.AmountEntryText = text;

            // Act
            _addVM.AmountBoxGotFocusCommand.Execute();

            // Assert
            _addVM.AmountEntryText.Should().Be(text);
        }

        #endregion

        #region Date

        [TestMethod]
        public void DateLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.DateEntryText = string.Empty;

            // Act
            _addVM.DateBoxLostFocusCommand.Execute();

            // Assert
            _addVM.DateEntryText.Should().Be("Date");
        }

        [TestMethod]
        public void DateLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.DateEntryText = text;

            // Act
            _addVM.DateBoxLostFocusCommand.Execute();

            // Assert
            _addVM.DateEntryText.Should().Be(text);
        }

        [TestMethod]
        public void DateLostFocus_ContainsValidDate_ShouldFormatAllDates()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-3);
            var text = date.ToShortDateString();
            // we parse it instead of using the original to remove the time from the dateTime
            date = DateTime.Parse(text);
            _addVM.DateEntryText = text;
            _addVM.SelectedDate = new DateTime(1500, 1, 1);
            _addVM.DispDate = new DateTime(1600, 1, 1);

            // Act
            _addVM.DateBoxLostFocusCommand.Execute();

            // Assert
            _addVM.DateEntryText.Should().Be(text);
            _addVM.SelectedDate.Should().Be(date);
            _addVM.DispDate.Should().Be(date);
        }

        [TestMethod]
        public void DateGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.DateEntryText = "Date";

            // Act
            _addVM.DateBoxGotFocusCommand.Execute();

            // Assert
            _addVM.DateEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void DateGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.DateEntryText = text;

            // Act
            _addVM.DateBoxLostFocusCommand.Execute();

            // Assert
            _addVM.DateEntryText.Should().Be(text);
        }

        #endregion

        #endregion

        #region Text Updates

        [TestMethod]
        public void CustTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.CustEntryText = _db.Customers.FirstOrDefault().CustomerId.ToString();

            // Act
            _addVM.CustBoxTextChangedCommand.Execute();

            // Assert
            _addVM.CustErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void AmountTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.AmountEntryText = "12.87";

            // Act
            _addVM.AmountBoxTextChangedCommand.Execute();

            // Assert
            _addVM.AmountErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void DateTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.DateEntryText = DateTime.Now.AddDays(-7).ToShortDateString();

            // Act
            _addVM.DateBoxTextChangedCommand.Execute();

            // Assert
            _addVM.DateErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        //Bad Data

        [TestMethod]
        public void CustTxtChanged_Inexistent_ShouldShowAppropriateError()
        {
            // Arrange
            _addVM.CustEntryText = Generators.GenInexistentCustomerId().ToString();

            // Act
            _addVM.CustBoxTextChangedCommand.Execute();

            // Assert
            _addVM.CustErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.CustErrorText.Should().Be("Payments require a valid CustomerId.");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("-3")]
        public void AmountTxtChanged_ZeroOrLess_ShouldShowAppropriateError(string amount)
        {
            // Arrange
            _addVM.AmountEntryText = amount;

            // Act
            _addVM.AmountBoxTextChangedCommand.Execute();

            // Assert
            _addVM.AmountErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.AmountErrorText.Should().Be("Amount must be greater than 0.");
        }

        [TestMethod]
        public void DateTxtChanged_AfterToday_ShouldShowAppropriateError()
        {
            // Arrange
            _addVM.DateEntryText = DateTime.Now.AddDays(2).ToShortDateString();

            // Act
            _addVM.DateBoxTextChangedCommand.Execute();

            // Assert
            _addVM.DateErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.DateErrorText.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
        }

        #endregion

        #region Submit

        [TestMethod]
        public void Submit_HappyFlow_ShouldRegisterPayment()
        {
            // Arrange
            var pay = Generators.GenPayment();

            var dateStr = pay.Date.GetValueOrDefault().ToShortDateString();

            _addVM.AmountEntryText = pay.Amount.GetValueOrDefault().ToString();
            _addVM.AmountBoxLostFocusCommand.Execute();
            _addVM.CustEntryText = pay.CustomerId.ToString();
            _addVM.DateEntryText = dateStr;
            _addVM.DateBoxLostFocusCommand.Execute();
            _addVM.SelectedMethod = _db.PaymentMethods
                .SingleOrDefault(m => m.PaymentMethodId == pay.PaymentMethodId);

            // Act
            _addVM.SubmitCommand.Execute();
            var result = _service.GetAllByCustomerIdAndFiltered(pay.CustomerId, dateStr).SingleOrDefault();

            // Assert
            result.Should().BeEquivalentTo(pay);
        }

        [TestMethod]
        public void Submit_InexistentCustomerID_ShouldNotUpdatePayment()
        {
            // Arrange
            var pay = Generators.GenPayment();

            _addVM.AmountEntryText = pay.Amount.GetValueOrDefault().ToString();
            _addVM.AmountBoxLostFocusCommand.Execute();
            _addVM.CustEntryText = Generators.GenInexistentCustomerId().ToString();
            _addVM.DateEntryText = pay.Date.GetValueOrDefault().ToShortDateString();
            _addVM.DateBoxLostFocusCommand.Execute();
            _addVM.SelectedMethod = _db.PaymentMethods
                .SingleOrDefault(m => m.PaymentMethodId == pay.PaymentMethodId);
            var initCount = _service.GetAll().Count();


            // Act
            _addVM.SubmitCommand.Execute();
            var newCount = _service.GetAll().Count();

            // Assert
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Submit_DateAfterToday_ShouldNotUpdatePayment()
        {
            // Arrange
            var pay = Generators.GenPayment();

            _addVM.AmountEntryText = pay.Amount.GetValueOrDefault().ToString();
            _addVM.AmountBoxLostFocusCommand.Execute();
            _addVM.CustEntryText = pay.CustomerId.ToString();
            _addVM.DateEntryText = DateTime.Now.AddDays(3).ToShortDateString();
            _addVM.DateBoxLostFocusCommand.Execute();
            _addVM.SelectedMethod = _db.PaymentMethods
                .SingleOrDefault(m => m.PaymentMethodId == pay.PaymentMethodId);
            var initCount = _service.GetAll().Count();

            // Act
            _addVM.SubmitCommand.Execute();
            var newCount = _service.GetAll().Count();

            // Assert
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Submit_AmountTooSmall_ShouldNotUpdatePayment()
        {
            // Arrange
            var pay = Generators.GenPayment();

            _addVM.AmountEntryText = "0";
            _addVM.AmountBoxLostFocusCommand.Execute();
            _addVM.CustEntryText = pay.CustomerId.ToString();
            _addVM.DateEntryText = pay.Date.GetValueOrDefault().ToShortDateString();
            _addVM.DateBoxLostFocusCommand.Execute();
            _addVM.SelectedMethod = _db.PaymentMethods
                .SingleOrDefault(m => m.PaymentMethodId == pay.PaymentMethodId);
            var initCount = _service.GetAll().Count();

            // Act
            _addVM.SubmitCommand.Execute();
            var newCount = _service.GetAll().Count();

            // Assert
            newCount.Should().Be(initCount);
        }

        #endregion
    }
}
