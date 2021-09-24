using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Interfaces;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonDrop.UnitTests.WPF.Tabs.User
{
    [TestClass]
    public class UserStoreTabVMTests
    {
        private TestDb _db;
        private IProductRepository _repo;
        private IProductDataService _service;
        private UserStoreTabViewModel _tabVM;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestProductRepo(_db);
            _service = new ProductDataService(_repo);
            _tabVM = new UserStoreTabViewModel(_service);
            _vm = new PrivateObject(typeof(UserStoreTabViewModel), _service);
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
        public void PriceTxtChanged_CharCountNormal_ShouldLeaveBe()
        {
            // Arrange
            var text = "123456789012345";
            _tabVM.AdvPriceText = text;

            // Act
            _tabVM.AdvPriceTextChangedCommand.Execute();

            // Assert
            _tabVM.AdvPriceText.Should().Be(text);
        }

        [TestMethod]
        public void PriceTxtChanged_TooManyChars_ShouldTrim()
        {
            // Arrange
            var text = "1234567890123456";
            _tabVM.AdvPriceText = text;

            // Act
            _tabVM.AdvPriceTextChangedCommand.Execute();

            // Assert
            _tabVM.AdvPriceText.Should().Be(text.Substring(0, 15));
        }

        [TestMethod]
        public void PriceLostFocus_Gibberish_ShouldLeaveBe()
        {
            // Arrange
            var text = "asdf";
            _tabVM.AdvPriceText = text;

            // Act
            _tabVM.AdvPriceLostFocusCommand.Execute();

            // Assert
            _tabVM.AdvPriceText.Should().Be(text);
        }

        [TestMethod]
        public void PriceLostFocus_ContainsPrice_ShouldFormat()
        {
            // Arrange
            var text = "12.345";
            _tabVM.AdvPriceText = text;

            // Act
            _tabVM.AdvPriceLostFocusCommand.Execute();

            // Assert
            _tabVM.AdvPriceText.Should().Be(decimal.Parse(text).ToString("C2"));
        }

    }
}
