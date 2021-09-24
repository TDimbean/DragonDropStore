using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.StoreViews.Tabs;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DragonDrop.UnitTests.WPF.Tabs.User
{
    [TestClass]
    public class UserAccTabVMTests
    {
        private TestDb _db;
        private ICustomerRepository _repo;
        private ICustomerDataService _service;
        private UserAccountTabViewModel _tabVM;
        private UserAccountTab _tab;
        private Customer _ogCust;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestCustomerRepo(_db);
            _service = new CustomerDataService(_repo);
            _ogCust = _db.Customers.FirstOrDefault();
            _tab = new UserAccountTab(_service, _ogCust);
            _tabVM = new UserAccountTabViewModel(_ogCust, _service, _tab);
            _vm = new PrivateObject(typeof(UserAccountTabViewModel), _ogCust, _service, _tab);
        }

        [TestMethod]
        public void Submit_HappyFlow_ShouldUpdateCustomer()
        {
            // Arrange
            var newCust = Generators.GenCustomer();
            newCust.CustomerId = _ogCust.CustomerId;
            var oldCust = new Customer
            {
                State = _ogCust.State,
                Address = _ogCust.Address,
                City = _ogCust.City,
                CustomerId = _ogCust.CustomerId,
                Email = _ogCust.Email,
                Name = _ogCust.Name,
                Phone = _ogCust.Phone
            };

            _tabVM.Fields[0].Text = newCust.Name;
            _tabVM.Fields[1].Text = newCust.Email;
            _tabVM.Fields[2].Text = newCust.Phone;
            _tabVM.Fields[3].Text = newCust.Address;
            _tabVM.Fields[4].Text = newCust.City;
            _tabVM.Fields[5].Text = newCust.State;

            // Act
            _tabVM.SubmitClickCommand.Execute();
            var result = _service.Get(_ogCust.CustomerId);

            // Assert
            result.Should().BeEquivalentTo(newCust);
            result.Should().NotBeEquivalentTo(oldCust);
            _tabVM.Fields[0].OgText.Should().Be(newCust.Name);
            _tabVM.Fields[1].OgText.Should().Be(newCust.Email);
            _tabVM.Fields[2].OgText.Should().Be(newCust.Phone);
            _tabVM.Fields[3].OgText.Should().Be(newCust.Address);
            _tabVM.Fields[4].OgText.Should().Be(newCust.City);
            _tabVM.Fields[5].OgText.Should().Be(newCust.State);
            _tabVM.UserName.Should().Be(newCust.Name);
            _tabVM.Fields[0].OgText.Should().NotBe(oldCust.Name);
            _tabVM.Fields[1].OgText.Should().NotBe(oldCust.Email);
            _tabVM.Fields[2].OgText.Should().NotBe(oldCust.Phone);
            _tabVM.Fields[3].OgText.Should().NotBe(oldCust.Address);
            _tabVM.Fields[4].OgText.Should().NotBe(oldCust.City);
            _tabVM.Fields[5].OgText.Should().NotBe(oldCust.State);
            _tabVM.UserName.Should().NotBe(oldCust.Name);
        }

        [DataTestMethod]
        #region BadData
        [DataRow("", "wsmith@gmail.com", "070-555-2337", "1, Starlit Avenue", "Phoenix", "Arizona")]
        [DataRow("Will Smith12345678901234567891123456789212345678931hmxgitffouukkixwoqxbpkcxaqkrfezhetztmuvwkvicutfgsp", "wsmith@gmail.com", "070-555-2337", "1, Starlit Avenue", "Phoenix", "Arizona")]
        [DataRow("Will Smith", "wsmithrtudzirvzmsuxtstkeavilijdcyceoycrvopnxkfkqmqvqtqkphmiememsqdfowhnwvvupywkskngbsmrmfjs@gmail.com", "070-555-2337", "1, Starlit Avenue", "Phoenix", "Arizona")]
        [DataRow("Will Smith", "wsmithgmail.com", "070-555-2337", "1, Starlit Avenue", "Phoenix", "Arizona")]
        [DataRow("Will Smith", "wsmith@gmail.com", "07-555-2337", "1, Starlit Avenue", "Phoenix", "Arizona")]
        [DataRow("Will Smith", "wsmith@gmail.com", "0700-555-2337", "1, Starlit Avenue", "Phoenix", "Arizona")]
        [DataRow("Will Smith", "wsmith@gmail.com", "A70-555-2337", "1, Starlit Avenue", "Phoenix", "Arizona")]
        [DataRow("Will Smith", "wsmith@gmail.com", "070+555+2337", "1, Starlit Avenue", "Phoenix", "Arizona")]
        [DataRow("Will Smith", "wsmith@gmail.com", "070-555-2337", "", "Phoenix", "Arizona")]
        [DataRow("Will Smith", "wsmith@gmail.com", "070-555-2337", "lhqtpswstlfnetwdgiopmofrqtfqjydcgiorqopoyalthcdsejzhzlguzlvvbhsqttvntpcamruljysvismalqgfmwbvvkhpcplmypsfsnrddaodnrldnizdtjqkrvputgouqokxyvormxvhdqdcqcrmagooywtoequxjgczyrftlawqeojjtytndukdniohuzgigwfbb", "Phoenix", "Arizona")]
        [DataRow("Will Smith", "wsmith@gmail.com", "070-555-2337", "1, Starlit Avenue", "", "Arizona")]
        [DataRow("Will Smith", "wsmith@gmail.com", "070-555-2337", "1, Starlit Avenue", "Phoenix", "")]
        [DataRow("Will Smith", "wsmith@gmail.com", "070-555-2337", "1, Starlit Avenue", "zgpesaxtxlmmxouupqqwesxxbkrovobzjoatcbdztokmwgynnkalnnexkgospdxpmzpbvndginrrflumvwpskddkuslmiizjcvxtp", "Arizona")]
        [DataRow("Will Smith", "wsmith@gmail.com", "070-555-2337", "1, Starlit Avenue", "Phoenix", "zgpesaxtxlmmxouupqqwesxxbkrovobzjoatcbdztokmwgynnkalnnexkgospdxpmzpbvndginrrflumvwpskddkuslmiizjcvxtp")]
        #endregion
        public void Submit_BadData_ShouldNotUpdateCustomer
            (string name, string email, string phone, string adr, string city, string state)
        {
            // Arrange
            var oldCust = new Customer
            {
                State = _ogCust.State,
                Address = _ogCust.Address,
                City = _ogCust.City,
                CustomerId = _ogCust.CustomerId,
                Email = _ogCust.Email,
                Name = _ogCust.Name,
                Phone = _ogCust.Phone
            };

            _tabVM.Fields[0].Text = name;
            _tabVM.Fields[1].Text = email;
            _tabVM.Fields[2].Text = phone;
            _tabVM.Fields[3].Text = adr;
            _tabVM.Fields[4].Text = city;
            _tabVM.Fields[5].Text = state;

            // Act
            _tabVM.SubmitClickCommand.Execute();
            var result = _service.Get(_ogCust.CustomerId);

            // Assert
            result.Should().BeEquivalentTo(oldCust);
            _tabVM.Fields[0].OgText.Should().Be(oldCust.Name);
            _tabVM.Fields[1].OgText.Should().Be(oldCust.Email);
            _tabVM.Fields[2].OgText.Should().Be(oldCust.Phone);
            _tabVM.Fields[3].OgText.Should().Be(oldCust.Address);
            _tabVM.Fields[4].OgText.Should().Be(oldCust.City);
            _tabVM.Fields[5].OgText.Should().Be(oldCust.State);
            _tabVM.UserName.Should().Be(_vm.Invoke("GetAccTabHeader", oldCust.Name).ToString());
            _tabVM.Fields[0].OgText.Should().NotBe(name);
            _tabVM.Fields[1].OgText.Should().NotBe(email);
            _tabVM.Fields[2].OgText.Should().NotBe(phone);
            _tabVM.Fields[3].OgText.Should().NotBe(adr);
            _tabVM.Fields[4].OgText.Should().NotBe(city);
            _tabVM.Fields[5].OgText.Should().NotBe(state);
            _tabVM.UserName.Should().NotBe(name);
        }
    }
}
