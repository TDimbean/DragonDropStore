using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Interfaces;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System.Windows.Media;

namespace DragonDrop.UnitTests.WPF.Tabs
{
    [TestClass]
    public class AdminProductVMTests
    {
        private TestDb _db;
        private IProductRepository _repo;
        private IProductDataService _service;

        private AdminProductsTabViewModel _tabVM;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestProductRepo(_db);
            _service = new ProductDataService(_repo);

            _tabVM = new AdminProductsTabViewModel(_service);
            _vm = new PrivateObject(typeof(AdminProductsTabViewModel), _service);
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


        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void FilterSelChanged_HappyFlow_ShouldShowSelectedHideOthers(int selInd)
        {
            // Arrange
            _tabVM.SelFilterIndex = selInd;

            // Act
            _tabVM.FilterSelChangedCommand.Execute();

            // Assert
            switch (selInd)
            {
                case 1:
                    _tabVM.SearchFilterVisibility.Should().Be(Visibility.Collapsed);
                    _tabVM.PriceFilterVisibility.Should().Be(Visibility.Visible);
                    _tabVM.StockFilterVisibility.Should().Be(Visibility.Collapsed);
                    break;
                case 2:
                    _tabVM.SearchFilterVisibility.Should().Be(Visibility.Collapsed);
                    _tabVM.PriceFilterVisibility.Should().Be(Visibility.Collapsed);
                    _tabVM.StockFilterVisibility.Should().Be(Visibility.Visible);
                    break;
                default:
                    _tabVM.SearchFilterVisibility.Should().Be(Visibility.Visible);
                    _tabVM.PriceFilterVisibility.Should().Be(Visibility.Collapsed);
                    _tabVM.StockFilterVisibility.Should().Be(Visibility.Collapsed);
                    break;
            }
        }

        [TestMethod]
        public void AdvPriceTxtChanged_RightLength_ShouldLeaveUnchanged()
        {
            // Arrange
            var text = "12";
            _tabVM.AdvPriceText = text;

            // Act
            _tabVM.AdvPriceTextChangedCommand.Execute();

            // Assert
            _tabVM.AdvPriceText.Should().Be(text);
        }

        [TestMethod]
        public void AdvPriceTxtChanged_TooLong_ShouldTrimText()
        {
            // Arrange
            var text = "1234567890123456";
            _tabVM.AdvPriceText = text;

            // Act
            _tabVM.AdvPriceTextChangedCommand.Execute();

            // Assert
            _tabVM.AdvPriceText.Should().Be(text.Substring(0, 15));
        }

        [DataTestMethod]
        [DataRow("1,500.01")]
        [DataRow("23.46")]
        public void AdvPriceLostFocus_Parses_ShouldBecomeCurrency(string price)
        {
            // Arrange
            _tabVM.AdvPriceText = price;

            // Act
            _tabVM.AdvPriceLostFocusCommand.Execute();

            // Assert
            _tabVM.AdvPriceText.Should().Be(string.Format("{0:C}", decimal.Parse(price)));
        }

        [TestMethod]
        public void AdvPriceLostFocus_DoesntParse_ShouldBecomeCurrency()
        {
            // Arrange
            var text = "ssd";
            _tabVM.AdvPriceText = text;

            // Act
            _tabVM.AdvPriceLostFocusCommand.Execute();

            // Assert
            _tabVM.AdvPriceText.Should().Be(text);
        }

    }
}
