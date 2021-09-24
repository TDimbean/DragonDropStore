using DragonDrop.BLL.DataServices;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows;

namespace DragonDrop.UnitTests.WPF
{
    [TestClass]
    public class MainWindowVMTests
    {
        private TestDb _db;
        private TestCustomerRepo _repo;
        private CustomerDataService _service;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestCustomerRepo(_db);
            _service = new CustomerDataService(_repo);
            _vm = new PrivateObject(typeof(MainWindowViewModel), _service);
        }

        [TestMethod]
        public void DoesInitializeWork()
        {
            var vis =_vm.GetProperty("PassApprovedVisibility");

            vis.Should().Be(Visibility.Hidden);
        }

        [TestMethod]
        public void InputUpdate_BothEmpty_AllShouldBeEmpty()
        {
            // Arrange
            _vm.SetProperty("AdrBoxText", string.Empty);
            _vm.SetProperty("PassBoxText", string.Empty);

            // Act
            _vm.Invoke("InputUpdate");

            // Assert
            _vm.GetProperty("PassApprovedVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("PassErrorVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("AdrApprovedVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("AdrErrorVisibility").Should().Be(Visibility.Hidden);
        }

        [TestMethod]
        public void InputUpdate_BothRight_BothShouldBeValid()
        {
            // Arrange
            var cust = _db.Customers.FirstOrDefault();
            _vm.SetProperty("AdrBoxText", cust.Email);
            _vm.SetProperty("PassBoxText", cust.Phone.Substring(0,3));

            // Act
            _vm.Invoke("InputUpdate");

            // Assert
            _vm.GetProperty("PassApprovedVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("PassErrorVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("AdrApprovedVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("AdrErrorVisibility").Should().Be(Visibility.Hidden);
        }

        [TestMethod]
        public void InputUpdate_BothGibberish_AdrShouldBeXPassShouldBeEmpty()
        {
            // Arrange
            var badAdr = _db.Customers.FirstOrDefault().Email;
            while (_db.Customers.Any(c => c.Email == badAdr))
                badAdr = Generators.GenRandomString(30);

            _vm.SetProperty("AdrBoxText", badAdr);
            _vm.SetProperty("PassBoxText", "0000");

            // Act
            _vm.Invoke("InputUpdate");

            // Assert
            _vm.GetProperty("PassApprovedVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("PassErrorVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("AdrApprovedVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("AdrErrorVisibility").Should().Be(Visibility.Visible);
        }

        [TestMethod]
        public void InputUpdate_AdrGibberishPassEmpty_AdrShouldBeXPassShouldBeEmpty()
        {
            // Arrange
            var badAdr = _db.Customers.FirstOrDefault().Email;
            while(_db.Customers.Any(c=>c.Email==badAdr))
                badAdr = Generators.GenRandomString(30);

            _vm.SetProperty("AdrBoxText", badAdr);
            _vm.SetProperty("PassBoxText", string.Empty);

            // Act
            _vm.Invoke("InputUpdate");

            // Assert
            _vm.GetProperty("PassApprovedVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("PassErrorVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("AdrApprovedVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("AdrErrorVisibility").Should().Be(Visibility.Visible);
        }

        [TestMethod]
        public void InputUpdate_AdrRightPassisGibberish_AdrShouldBeValidPassShouldBeX()
        {
            // Arrange
            _vm.SetProperty("AdrBoxText", _db.Customers.FirstOrDefault().Email);
            _vm.SetProperty("PassBoxText", "0000"/*this is a bad pass, bcuz in this version, psses are all 3 digits*/);

            // Act
            _vm.Invoke("InputUpdate");

            // Assert
            _vm.GetProperty("PassApprovedVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("PassErrorVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("AdrApprovedVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("AdrErrorVisibility").Should().Be(Visibility.Hidden);
        }

        [TestMethod]
        public void InputUpdate_AdrRightPassisEmpty_AdrShouldBeValidPassShouldBeX()

        {
            // Arrange
            _vm.SetProperty("AdrBoxText", _db.Customers.FirstOrDefault().Email);
            _vm.SetProperty("PassBoxText", string.Empty);

            // Act
            _vm.Invoke("InputUpdate");

            // Assert
            _vm.GetProperty("PassApprovedVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("PassErrorVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("AdrApprovedVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("AdrErrorVisibility").Should().Be(Visibility.Hidden);
        }

        [TestMethod]
        public void InputUpdate_AdrEmptyPassGibberish_BothShouldBeEmpty()
        {
            // Arrange
            _vm.SetProperty("AdrBoxText", string.Empty);
            _vm.SetProperty("PassBoxText", "0000");

            // Act
            _vm.Invoke("InputUpdate");

            // Assert
            _vm.GetProperty("PassApprovedVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("PassErrorVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("AdrApprovedVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("AdrErrorVisibility").Should().Be(Visibility.Hidden);
        }

    }
}
