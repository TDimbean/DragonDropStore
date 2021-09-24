using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Interfaces;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DragonDrop.UnitTests.WPF.Tabs.User
{
    [TestClass]
    public class UserOrdersTabVMTests
    {
        private TestDb _db;
        private IOrderRepository _repo;
        private IOrderDataService _service;
        private UserOrdersTabViewModel _tabVM;
        private PrivateObject _vm;
        
        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();

            var custId = _db.Customers.FirstOrDefault().CustomerId;
            _repo = new TestOrderRepo(_db);
            _service = new OrderDataService(_repo);
            _tabVM = new UserOrdersTabViewModel(_service, custId);
            _vm = new PrivateObject(typeof(UserOrdersTabViewModel), _service, custId);
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

    }
}
