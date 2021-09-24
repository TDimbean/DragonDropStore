using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Interfaces;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonDrop.UnitTests.WPF.Tabs.User
{
    [TestClass]
    public class UserPaymentsTabVMTests
    {
        private TestDb _db;
        private IPaymentRepository _repo;
        private IPaymentDataService _service;
        private int _custId;
        private UserPaymentsTabViewModel _tabVM;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestPaymentRepo(_db);
            _service = new PaymentDataService(_repo);
            _custId = _db.Customers.FirstOrDefault().CustomerId;
            _tabVM = new UserPaymentsTabViewModel(_service, _custId);
            _vm = new PrivateObject(typeof(UserPaymentsTabViewModel), _service, _custId);
        }

        [TestMethod]
        public void SearchGotFocus_HasText_ShouldKeepIt()
        {
            // Arrange
            var text = "A";
            _tabVM.SearchText = text;

            // Act
            _tabVM.SearchGotFocusCommand.Execute();

            // Assert
            _tabVM.SearchText.Should().Be(text);
        }

        [TestMethod]
        public void SearchGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _tabVM.SearchText = _vm.GetField("_searchPlaceholder").ToString();

            // Act
            _tabVM.SearchGotFocusCommand.Execute();

            // Assert
            _tabVM.SearchText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void SearchLostFocus_HasText_ShouldKeepIt()
        {
            // Arrange
            var text = "A";
            _tabVM.SearchText = text;

            // Act
            _tabVM.SearchLostFocusCommand.Execute();

            // Assert
            _tabVM.SearchText.Should().Be(text);
        }

        [TestMethod]
        public void SearchLostFocus_Empty_ShouldKeepIt()
        {
            // Arrange
            _tabVM.SearchText = string.Empty;

            // Act
            _tabVM.SearchLostFocusCommand.Execute();

            // Assert
            _tabVM.SearchText.Should().Be(_vm.GetField("_searchPlaceholder").ToString());
        }

        [TestMethod]
        public void AmountTxtChanged_CharCountNormal_ShouldLeaveBe()
        {
            // Arrange
            var text = "123456789012345";
            _tabVM.AdvAmountText = text;

            // Act
            _tabVM.AdvAmountTextChangedCommand.Execute();

            // Assert
            _tabVM.AdvAmountText.Should().Be(text);
        }

        [TestMethod]
        public void AmountTxtChanged_TooManyChars_ShouldTrim()
        {
            // Arrange
            var text = "1234567890123456";
            _tabVM.AdvAmountText = text;

            // Act
            _tabVM.AdvAmountTextChangedCommand.Execute();

            // Assert
            _tabVM.AdvAmountText.Should().Be(text.Substring(0,15));
        }

        [TestMethod]
        public void AmountLostFocus_Gibberish_ShouldLeaveBe()
        {
            // Arrange
            var text = "asdf";
            _tabVM.AdvAmountText = text;

            // Act
            _tabVM.AdvAmountLostFocusCommand.Execute();

            // Assert
            _tabVM.AdvAmountText.Should().Be(text);
        }

        [TestMethod]
        public void AmountLostFocus_ContainsAmount_ShouldFormat()
        {
            // Arrange
            var text = "12.345";
            _tabVM.AdvAmountText = text;

            // Act
            _tabVM.AdvAmountLostFocusCommand.Execute();

            // Assert
            _tabVM.AdvAmountText.Should().Be(decimal.Parse(text).ToString("C2"));
        }

        [TestMethod]
        public void DateLostFocus_Gibberish_ShouldLeaveBe()
        {
            // Arrange
            var text = "asdf";
            _tabVM.AdvDateText = text;

            // Act
            _tabVM.AdvDateLostFocusCommand.Execute();

            // Assert
            _tabVM.AdvDateText.Should().Be(text);
        }

        [TestMethod]
        public void DateLostFocus_ContainsDate_ShouldFormat()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-3);
            _tabVM.AdvDateText = date.ToString();

            // Act
            _tabVM.AdvDateLostFocusCommand.Execute();

            // Assert
            _tabVM.AdvDateText.Should().Be(date.ToShortDateString());
        }
    }
}
