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
    public class AdminOrderVMTests
    {
        private TestDb _db;
        private IOrderRepository _repo;
        private IOrderDataService _service;

        private AdminOrdersTabViewModel _tabVM;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestOrderRepo(_db);
            _service = new OrderDataService(_repo);

            _tabVM = new AdminOrdersTabViewModel(_service);
            _vm = new PrivateObject(typeof(AdminOrdersTabViewModel), _service);
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

    }
}
