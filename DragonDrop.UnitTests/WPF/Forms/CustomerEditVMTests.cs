using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows;

namespace DragonDrop.UnitTests.WPF.Forms
{
    [TestClass]
    [Ignore]
    public class CustomerEditVMTests
    {
        private TestDb _db;
        private ICustomerRepository _repo;
        private ICustomerDataService _service;

        private Customer _ogCust;
        private CustomerEditViewModel _editVM;

        private string over50 = Generators.GenRandomString(51);

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _ogCust = _db.Customers.FirstOrDefault();
            _repo = new TestCustomerRepo(_db);
            _service = new CustomerDataService(_repo);
            _editVM = new CustomerEditViewModel(_service, _ogCust.CustomerId);
        }

        #region Text Box Resets

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("Hank Row")]
        public void NameReset_HappyFlow_ShouldReset(string name)
        {
            // Arrange
            if (name != null) _editVM.NameEntryText = name;

            // Act
            _editVM.NameBoxResetCommand.Execute();

            // Assert
            _editVM.NameEntryText.Should().Be(_ogCust.Name);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("023-123-4322")]
        public void PhoneReset_HappyFlow_ShouldReset(string phone)
        {
            // Arrange
            if (phone != null) _editVM.PhoneEntryText = phone;

            // Act
            _editVM.PhoneBoxResetCommand.Execute();

            // Assert
            _editVM.PhoneEntryText.Should().Be(_ogCust.Phone);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("hrow83@yahoo.com")]
        public void EmailReset_HappyFlow_ShouldReset(string email)
        {
            // Arrange
            if (email != null) _editVM.EmailEntryText = email;

            // Act
            _editVM.EmailBoxResetCommand.Execute();

            // Assert
            _editVM.EmailEntryText.Should().Be(_ogCust.Email);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("13 Scorn Street")]
        public void AdrReset_HappyFlow_ShouldReset(string adr)
        {
            // Arrange
            if (adr != null) _editVM.AdrEntryText = adr;

            // Act
            _editVM.AdrBoxResetCommand.Execute();

            // Assert
            _editVM.AdrEntryText.Should().Be(_ogCust.Address);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("Portland")]
        public void CityReset_HappyFlow_ShouldReset(string city)
        {
            // Arrange
            if (city != null) _editVM.NameEntryText = city;

            // Act
            _editVM.CityBoxResetCommand.Execute();

            // Assert
            _editVM.CityEntryText.Should().Be(_ogCust.City);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("Oregon")]
        public void StateReset_HappyFlow_ShouldReset(string state)
        {
            // Arrange
            if (state != null) _editVM.StateEntryText = state;

            // Act
            _editVM.StateBoxResetCommand.Execute();

            // Assert
            _editVM.StateEntryText.Should().Be(_ogCust.State);
        }

        #endregion

        #region Focus Changes

        #region Name

        [TestMethod]
        public void NameLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _editVM.NameEntryText = string.Empty;

            // Act
            _editVM.NameBoxLostFocusCommand.Execute();

            // Assert
            _editVM.NameEntryText.Should().Be(_ogCust.Name);
        }

        [TestMethod]
        public void NameLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.NameEntryText = text;

            // Act
            _editVM.NameBoxLostFocusCommand.Execute();

            // Assert
            _editVM.NameEntryText.Should().Be(text);
        }

        #endregion

        #region Phone

        [TestMethod]
        public void PhoneLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _editVM.PhoneEntryText = string.Empty;

            // Act
            _editVM.PhoneBoxLostFocusCommand.Execute();

            // Assert
            _editVM.PhoneEntryText.Should().Be(_ogCust.Phone);
        }

        [TestMethod]
        public void PhoneLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.PhoneEntryText = text;

            // Act
            _editVM.PhoneBoxLostFocusCommand.Execute();

            // Assert
            _editVM.PhoneEntryText.Should().Be(text);
        }

        #endregion

        #region Email

        [TestMethod]
        public void EmailLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _editVM.EmailEntryText = string.Empty;

            // Act
            _editVM.EmailBoxLostFocusCommand.Execute();

            // Assert
            _editVM.EmailEntryText.Should().Be(_ogCust.Email);
        }

        [TestMethod]
        public void EmailLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.EmailEntryText = text;

            // Act
            _editVM.EmailBoxLostFocusCommand.Execute();

            // Assert
            _editVM.EmailEntryText.Should().Be(text);
        }

        #endregion

        #region Address

        [TestMethod]
        public void AdrLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _editVM.AdrEntryText = string.Empty;

            // Act
            _editVM.AdrBoxLostFocusCommand.Execute();

            // Assert
            _editVM.AdrEntryText.Should().Be(_ogCust.Address);
        }

        [TestMethod]
        public void AdrLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.AdrEntryText = text;

            // Act
            _editVM.AdrBoxLostFocusCommand.Execute();

            // Assert
            _editVM.AdrEntryText.Should().Be(text);
        }

        #endregion

        #region City

        [TestMethod]
        public void CityLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _editVM.CityEntryText = string.Empty;

            // Act
            _editVM.CityBoxLostFocusCommand.Execute();

            // Assert
            _editVM.CityEntryText.Should().Be(_ogCust.City);
        }

        [TestMethod]
        public void CityLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.CityEntryText = text;

            // Act
            _editVM.CityBoxLostFocusCommand.Execute();

            // Assert
            _editVM.CityEntryText.Should().Be(text);
        }

        #endregion

        #region State

        [TestMethod]
        public void StateLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _editVM.StateEntryText = string.Empty;

            // Act
            _editVM.StateBoxLostFocusCommand.Execute();

            // Assert
            _editVM.StateEntryText.Should().Be(_ogCust.State);
        }

        [TestMethod]
        public void StateLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.StateEntryText = text;

            // Act
            _editVM.StateBoxLostFocusCommand.Execute();

            // Assert
            _editVM.StateEntryText.Should().Be(text);
        }

        #endregion

        #endregion

        #region Text Updates

        [TestMethod]
        public void NameTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.NameEntryText = "Will Smith";

            // Act
            _editVM.NameBoxTextChangedCommand.Execute();

            // Assert
            _editVM.NameErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void EmailTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.EmailEntryText = "wsmith@gmail.com";

            // Act
            _editVM.EmailBoxTextChangedCommand.Execute();

            // Assert
            _editVM.EmailErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void AddressTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.AdrEntryText = "12 Startstruck Boulevard";

            // Act
            _editVM.AdrBoxTextChangedCommand.Execute();

            // Assert
            _editVM.AdrErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void CityTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.CityEntryText = "Phoenix";

            // Act
            _editVM.CityBoxTextChangedCommand.Execute();

            // Assert
            _editVM.CityErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void StateTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.StateEntryText = "Arizona";

            // Act
            _editVM.StateBoxTextChangedCommand.Execute();

            // Assert
            _editVM.StateErrorVisibility.Should().Be(Visibility.Collapsed);
        }

            //Bad Names

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void NameTextChanged_Blank_ShouldShowAppropriateError(string name)
        {
            // Arrange
            _editVM.NameEntryText = name;

            // Act
            _editVM.NameBoxTextChangedCommand.Execute();

            // Assert
            _editVM.NameErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.NameErrorText.Should().Be("Name cannot be blank.");
        }

        [TestMethod]
        public void NameTextChanged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var name = Generators.GenRandomString(101);
            _editVM.NameEntryText = name;

            // Act
            _editVM.NameBoxTextChangedCommand.Execute();

            // Assert

            _editVM.NameErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.NameErrorText.Should().Be("Name must not exceed 100 characters.");
        }

            //Bad Emails

        [TestMethod]
        public void EmailTextChaged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var email = (over50 + "@" + over50).Substring(0, 101);
            _editVM.EmailEntryText = email;

            // Act
            _editVM.EmailBoxTextChangedCommand.Execute();

            // Assert
            _editVM.EmailErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.EmailErrorText.Should().Be("Email must not exceed 100 characters.");
        }

        [TestMethod]
        public void EmailTextChaged_WrongFormat_ShouldShowAppropriateError()
        {
            // Arrange
            var email = Generators.GenRandomString(20).Replace("@", string.Empty);
            _editVM.EmailEntryText = email;

            // Act
            _editVM.EmailBoxTextChangedCommand.Execute();

            // Assert
            _editVM.EmailErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.EmailErrorText.Should().Be("Email must be in a valid format (account@provider).");
        }

        //Bad Phones

        //[DataTestMethod]
        //[DataRow("")]
        //[DataRow(null)]
        //public void PhoneTextChanged_Blank_ShouldShowAppropriateError(string phone)
        //{
        //    // Arrange
        //    _editVM.PhoneEntryText=phone);

        //    // Act
        //    _editVM.PhoneBoxTextChangedCommand.Execute();

        //    // Assert
        //    _editVM.PhoneCheckVisibility.Should().Be(Visibility.Hidden);
        //    _editVM.PhoneErrorVisibility.Should().Be(Visibility.Visible);
        //    _editVM.PhoneErrorText.Should().Be("Customer requires a phone number.");
        //}

        //[DataTestMethod]
        //[DataRow("1234567890")]
        //[DataRow("123-456-78910")]
        //public void PhoneTextChanged_LengthWrong_ShouldShowAppropriateError(string phone)
        //{
        //    // Arrange
        //    _editVM.PhoneEntryText=phone);

        //    // Act
        //    _editVM.PhoneBoxTextChangedCommand.Execute();

        //    // Assert
        //    _editVM.PhoneCheckVisibility.Should().Be(Visibility.Hidden);
        //    _editVM.PhoneErrorVisibility.Should().Be(Visibility.Visible);
        //    _editVM.PhoneErrorText.Should().Be("Phone Numbers must have precisely 10 characters.");
        //}

        //[TestMethod]
        //public void PhoneTextChanged_NoDashes_ShouldShowAppropriateError()
        //{
        //    // Arrange
        //    _editVM.PhoneEntryText="123+123+1234;

        //    // Act
        //    _editVM.PhoneBoxTextChangedCommand.Execute();

        //    // Assert
        //    _editVM.PhoneCheckVisibility.Should().Be(Visibility.Hidden);
        //    _editVM.PhoneErrorVisibility.Should().Be(Visibility.Visible);
        //    _editVM.PhoneErrorText.Should().Be("Phone number must be in valid format: XXX-XXX-XXXX.");
        //}

        //[TestMethod]
        //public void PhoneTextChanged_NonDigits_ShouldShowAppropriateError()
        //{
        //    // Arrange
        //    _editVM.PhoneEntryText="123-123-123a;

        //    // Act
        //    _editVM.PhoneBoxTextChangedCommand.Execute();

        //    // Assert
        //    _editVM.PhoneCheckVisibility.Should().Be(Visibility.Hidden);
        //    _editVM.PhoneErrorVisibility.Should().Be(Visibility.Visible);
        //    _editVM.PhoneErrorText.Should().Be("Phone number may only contain digits.");
        //}

        // Bad Addresses

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void AddressTextChanged_Blank_ShouldShowAppropriateError(string adr)
        {
            // Arrange
            _editVM.AdrEntryText = adr;

            // Act
            _editVM.AdrBoxTextChangedCommand.Execute();

            // Assert
            _editVM.AdrErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.AdrErrorText.Should().Be("Address cannot be empty.");
        }

        [TestMethod]
        public void AddressTextChanged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var adr = Generators.GenRandomString(201);
            _editVM.AdrEntryText = adr;

            // Act
            _editVM.AdrBoxTextChangedCommand.Execute();

            // Assert
            _editVM.AdrErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.AdrErrorText.Should().Be("Address may not exceed 200 characters.");
        }

          // Bad Cities

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void CityTextChanged_Blank_ShouldShowAppropriateError(string city)
        {
            // Arrange
            _editVM.CityEntryText = city;

            // Act
            _editVM.CityBoxTextChangedCommand.Execute();

            // Assert
            _editVM.CityErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.CityErrorText.Should().Be("City field cannot be blank.");
        }

        [TestMethod]
        public void CityTextChanged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var city = Generators.GenRandomString(101);
            _editVM.CityEntryText = city;

            // Act
            _editVM.CityBoxTextChangedCommand.Execute();

            // Assert
            _editVM.CityErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.CityErrorText.Should()
                .Be("City should not exceed 100 characters; If necessary, please use an abbreviation.");
        }

           // Bad State

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void StateTextChanged_Blank_ShouldShowAppropriateError(string state)
        {
            // Arrange
            _editVM.StateEntryText = state;

            // Act
            _editVM.StateBoxTextChangedCommand.Execute();

            // Assert
            _editVM.StateErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.StateErrorText.Should().Be("State/County cannot be blank.");
        }

        [TestMethod]
        public void StateTextChanged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var state = Generators.GenRandomString(51);
            _editVM.StateEntryText = state;

            // Act
            _editVM.StateBoxTextChangedCommand.Execute();

            // Assert
            _editVM.StateErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.StateErrorText.Should()
                .Be("State/County is limited to 50 characters; Where necessary, please use an abbreviation.");
        }

        #endregion

        [TestMethod]
        public void Submit_HappyFlow_ShouldRegisterCustomer()
        {
            // Arrange
            var initCust = new Customer
            {
                CustomerId = _ogCust.CustomerId,
                State = _ogCust.State,
                Address = _ogCust.Address,
                City = _ogCust.City,
                Email = _ogCust.Email,
                Name = _ogCust.Name,
                Phone = _ogCust.Phone
            };

            var cust = Generators.GenCustomer();
            _editVM.NameEntryText = cust.Name;
            _editVM.PhoneEntryText = cust.Phone;
            _editVM.EmailEntryText = cust.Email;
            _editVM.AdrEntryText = cust.Address;
            _editVM.CityEntryText = cust.City;
            _editVM.StateEntryText = cust.State;
            cust.CustomerId = _ogCust.CustomerId;

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(_ogCust.CustomerId);

            // Assert
            result.Should().BeEquivalentTo(cust);
            result.Should().NotBeEquivalentTo(initCust);
        }

        [DataTestMethod]
        #region Bad Data
        [DataRow("", "030-030-0440", "Wsmith@gmail.com", "1 Gil Peak", "Eek", "Alsaska")]
        [DataRow(null, "030-030-0440", "Wsmith@gmail.com", "1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smithrdquofbdpycnqfrorqtuxobsxvmtyjypgclmobszvdjjmdpqxphzueevwbmugrzugbygjvwgufjgyrnfrzxjonytlge",
           "030-030-0440", "Wsmith@gmail.com", "1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmithgmail.com", "1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440",
           "pmzdpgtodtoedxftodzlgzzxknaqemavmlnhszzogoflgjptqo@pmzdpgtodtoedxftodzlgzzxknaqemavmlnhszzogoflgjptqo",
           "1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "", "Wsmith@gmail.com", "1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", null, "Wsmith@gmail.com", "1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "1234567890", "Wsmith@gmail.com", "1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "123-456-78910", "Wsmith@gmail.com", "1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "123+123+1234", "Wsmith@gmail.com", "1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "123-123-123a", "Wsmith@gmail.com", "1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com", "", "Eek", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com", null, "Eek", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com",
"emakejmvzckstiyibetqhlrplffpvktdmjrhtjhfhakatjmzlwbgyarftbtlhgbrtvubqhngzrytahddzrbmdjviyugqzobmewkortuzsqnukxhhllnluyodheufoffndvupajudapiwmygxzdnhvjtpmwiklipbhrqotmkofogpkzvracuskwuphindgdilumusrvali", "Eek", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com", "1 Gil Peak", "", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com", "1 Gil Peak", null, "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com", "1 Gil Peak",
          "mfmkxzqpeaqjfypbgzgxeraydbvuwshgxzbbrywgpmojkmaztqrgxkvcxgefyumqfnodqbnqpimtahiarnnavfdoasxljmxljlkxe"
           , "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com", "1 Gil Peak", "Eek", "")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com", "1 Gil Peak", "Eek", null)]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com", "1 Gil Peak", "Eek",
           "sicpihjuqzyxfkfaxwewmeovhqcqpgvhjsgfhcojagiaiyzqiay")]
        #endregion
        [TestMethod]
        public void Submit_BadData_ShouldNotRegisterCustomer
           (string name, string phone, string email, string adr, string city, string state)
        {
            // Arrange
            var cust = Generators.GenCustomer();
            _editVM.NameEntryText = name;
            _editVM.PhoneEntryText = phone;
            _editVM.EmailEntryText = email;
            _editVM.AdrEntryText = adr;
            _editVM.CityEntryText = city;
            _editVM.StateEntryText = state;
            cust.CustomerId = _ogCust.CustomerId;

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(_ogCust.CustomerId);

            // Assert
            result.Should().BeEquivalentTo(_ogCust);
            result.Should().NotBeEquivalentTo(cust);
        }
    }
}
