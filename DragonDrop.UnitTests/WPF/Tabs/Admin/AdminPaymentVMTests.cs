using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Interfaces;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using System.Windows.Media;

namespace DragonDrop.UnitTests.WPF.Tabs
{
    [TestClass]
    public class AdminPaymentVMTests
    {
        private TestDb _db;
        private IPaymentRepository _repo;
        private IPaymentDataService _service;

        private AdminPaymentsTabViewModel _tabVM;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestPaymentRepo(_db);
            _service = new PaymentDataService(_repo);

            _tabVM = new AdminPaymentsTabViewModel(_service);
            _vm = new PrivateObject(typeof(AdminPaymentsTabViewModel), _service);
        }

        [TestMethod]
        public void SearchGotFocus_Placeholder_ShouldAdjustAppearanceAndRemoveText()
        {
            _tabVM.SearchGotFocusCommand.Execute();

            // Assert
            _tabVM.SearchColour.Should().Be(Brushes.Black);
            _tabVM.SearchFont.Should().Be("Adobe Heiti Std R");
            _tabVM.SearchStyle.Should().Be(FontStyles.Normal);

            _tabVM.SearchText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void SearchGotFocus_TextInBox_ShouldAdjustAppearanceAndRetainText()
        {
            // Arrange
            var text = "a";
            _tabVM.SearchText = text;

            // Act
            _tabVM.SearchGotFocusCommand.Execute();

            // Assert
            _tabVM.SearchColour.Should().Be(Brushes.Black);
            _tabVM.SearchFont.Should().Be("Adobe Heiti Std R");
            _tabVM.SearchStyle.Should().Be(FontStyles.Normal);

            _tabVM.SearchText.Should().Be(text);
        }

        [TestMethod]
        public void SerchLostFocus_EmptyBox_ShouldAdjustAppearanceAndApplyPlaceholder()
        {
            // Arrange
            _tabVM.SearchText = string.Empty;

            // Act
            _tabVM.SearchLostFocusCommand.Execute();

            // Assert
            _tabVM.SearchColour.Should().Be(Brushes.Gray);
            _tabVM.SearchFont.Should().Be("Segoe UI Historical");
            _tabVM.SearchStyle.Should().Be(FontStyles.Italic);

            _tabVM.SearchText.Should().Be(_vm.GetField("_searchPlaceholder").ToString());
        }

        [TestMethod]
        public void SerchLostFocus_TextInBox_ShouldAdjustAppearanceAndRetainText()
        {
            // Arrange
            var text = "a";
            _tabVM.SearchText = text;

            // Act
            _tabVM.SearchLostFocusCommand.Execute();

            // Assert
            _tabVM.SearchColour.Should().Be(Brushes.Gray);
            _tabVM.SearchFont.Should().Be("Segoe UI Historical");
            _tabVM.SearchStyle.Should().Be(FontStyles.Italic);

            _tabVM.SearchText.Should().Be(text);
        }

        [TestMethod]
        public void AdvCustTxtChanged_RightLength_ShouldLeaveUnchanged()
        {
            // Arrange
            var text = "12";
            _tabVM.AdvCustText = text;

            // Act
            _tabVM.AdvCustTextChangedCommand.Execute();

            // Assert
            _tabVM.AdvCustText.Should().Be(text);
        }

        [TestMethod]
        public void AdvCustTxtChanged_TooLong_ShouldTrimText()
        {
            // Arrange
            var text = "1234567890";
            _tabVM.AdvCustText = text;

            // Act
            _tabVM.AdvCustTextChangedCommand.Execute();

            // Assert
            _tabVM.AdvCustText.Should().Be(text.Substring(0,9));
        }

        [TestMethod]
        public void AdvAmtTxtChanged_RightLength_ShouldLeaveUnchanged()
        {
            // Arrange
            var text = "12";
            _tabVM.AdvAmountText = text;

            // Act
            _tabVM.AdvAmountTextChangedCommand.Execute();

            // Assert
            _tabVM.AdvAmountText.Should().Be(text);
        }

        [TestMethod]
        public void AdvAmountTxtChanged_TooLong_ShouldTrimText()
        {
            // Arrange
            var text = "1234567890123456";
            _tabVM.AdvAmountText = text;

            // Act
            _tabVM.AdvAmountTextChangedCommand.Execute();

            // Assert
            _tabVM.AdvAmountText.Should().Be(text.Substring(0, 15));
        }

        [DataTestMethod]
        [DataRow("1,500.01")]
        [DataRow("23.46")]
        public void AdvAmtLostFocus_Parses_ShouldBecomeCurrency(string amt)
        { 
            // Arrange
            _tabVM.AdvAmountText = amt;

            // Act
            _tabVM.AdvAmountLostFocusCommand.Execute();

            // Assert
            _tabVM.AdvAmountText.Should().Be(string.Format("{0:C}", decimal.Parse(amt)));
        }

        [TestMethod]
        public void AdvAmtLostFocus_DoesntParse_ShouldBecomeCurrency()
        {
            // Arrange
            var text = "ssd";
            _tabVM.AdvAmountText = text;

            // Act
            _tabVM.AdvAmountLostFocusCommand.Execute();

            // Assert
            _tabVM.AdvAmountText.Should().Be(text);
        }

        [TestMethod]
        public void AdvDateLostFocus_Parses_ShouldUpdateBothDates()
        {
            // Arrange
            var dateText = "12Jan2018";
            _tabVM.AdvDateText = dateText;
            _tabVM.AdvCalDate = new DateTime(2012, 12, 21);
            var date = DateTime.Parse(dateText);

            // Act
            _tabVM.AdvDateLostFocusCommand.Execute();

            // Assert
            _tabVM.AdvDateText.Should().Be(date.ToShortDateString());
            _tabVM.AdvCalDate.Should().Be(date);
        }

        [TestMethod]
        public void AdvDateLostFocus_DoesntParse_ShouldUpdateBothDates()
        {
            // Arrange
            var dateText = "mhg";
            _tabVM.AdvDateText = dateText;
            var calDate = new DateTime(2012, 12, 21);
            _tabVM.AdvCalDate = calDate;

            // Act
            _tabVM.AdvDateLostFocusCommand.Execute();

            // Assert
            _tabVM.AdvDateText.Should().Be(dateText);
            _tabVM.AdvCalDate.Should().Be(calDate);
        }

        [TestMethod]
        public void ChangeAdvDate_BothMatch_ShouldDoNothing()
        {
            // Arrange
            var date = new DateTime(2012, 12, 21);
            var dateText = date.ToShortDateString();
            _vm.SetProperty("AdvCalDate", date);
            _vm.SetProperty("AdvDateText", dateText);

            // Act
            _vm.Invoke("ChangeAdvDate", date);

            // Assert
            _vm.GetProperty("AdvCalDate").Should().Be(date);
            _vm.GetProperty("AdvDateText").Should().Be(dateText);
        }

        [TestMethod]
        public void ChangeAdvDate_TextMatches_ShouldChangeCal()
        {
            // Arrange
            var date = new DateTime(2012, 12, 21);
            var oldCalDate = new DateTime(2013, 12, 21);
            var dateText = date.ToShortDateString();
            _vm.SetProperty("AdvCalDate", oldCalDate);
            _vm.SetProperty("AdvDateText", dateText);

            // Act
            _vm.Invoke("ChangeAdvDate", date);

            // Assert
            _vm.GetProperty("AdvCalDate").Should().Be(date);
            _vm.GetProperty("AdvDateText").Should().Be(dateText);
        }

        [TestMethod]
        public void ChangeAdvDate_CalMatches_ShouldChangeText()
        {
            // Arrange
            var date = new DateTime(2012, 12, 21);
            var dateText = date.ToShortDateString();
            _vm.SetProperty("AdvCalDate", date);
            _vm.SetProperty("AdvDateText", "45");

            // Act
            _vm.Invoke("ChangeAdvDate", date);

            // Assert
            _vm.GetProperty("AdvCalDate").Should().Be(date);
            _vm.GetProperty("AdvDateText").Should().Be(dateText);
        }

        [TestMethod]
        public void ChangeAdvDate_NeitherMatch_ShouldChangeBoth()
        {
            // Arrange
            var date = new DateTime(2012, 12, 21);
            var dateText = date.ToShortDateString();
            _vm.SetProperty("AdvCalDate", date.AddYears(-1));
            _vm.SetProperty("AdvDateText", "67");

            // Act
            _vm.Invoke("ChangeAdvDate", date);

            // Assert
            _vm.GetProperty("AdvCalDate").Should().Be(date);
            _vm.GetProperty("AdvDateText").Should().Be(dateText);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void FilterSelChanged_HappyFlow_ShouldShowSelectedHideOthers(int selInd)
        {
            // Arrange
            _tabVM.SelFilterIndex = selInd;

            // Act
            _tabVM.FilterSelChangedCommand.Execute(null);

            // Assert
            switch (selInd)
            {
                case 1:
                    _tabVM.SearchFilterVisibility.Should().Be(Visibility.Collapsed);
                    _tabVM.AmountFilterVisibility.Should().Be(Visibility.Visible);
                    _tabVM.DateFilterVisibility.Should().Be(Visibility.Collapsed);
                    break;
                case 2:
                    _tabVM.SearchFilterVisibility.Should().Be(Visibility.Collapsed);
                    _tabVM.AmountFilterVisibility.Should().Be(Visibility.Collapsed);
                    _tabVM.DateFilterVisibility.Should().Be(Visibility.Visible);
                    break;
                default:
                    _tabVM.SearchFilterVisibility.Should().Be(Visibility.Visible);
                    _tabVM.AmountFilterVisibility.Should().Be(Visibility.Collapsed);
                    _tabVM.DateFilterVisibility.Should().Be(Visibility.Collapsed);
                    break;
            }
        }
    }
}
