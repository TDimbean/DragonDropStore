using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
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

    public class CustomerAddVMTests
    {
        private TestDb _db;
        private ICustomerRepository _repo;
        private ICustomerDataService _service;

        private CustomerAddViewModel _addVM;

        private string over50 = Generators.GenRandomString(51);

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestCustomerRepo(_db);
            _service = new CustomerDataService(_repo);
            _addVM = new CustomerAddViewModel(_service);
        }

        #region Focus Changes

        #region Name

        [TestMethod]
        public void NameLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.NameEntryText = string.Empty;

            // Act
            _addVM.NameBoxLostFocusCommand.Execute();

            // Assert
            _addVM.NameEntryText.Should().Be("Name");
        }

        [TestMethod]
        public void NameLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.NameEntryText = text;

            // Act
            _addVM.NameBoxLostFocusCommand.Execute();

            // Assert
            _addVM.NameEntryText.Should().Be(text);
        }

        [TestMethod]
        public void NameGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.NameEntryText = "Name";

            // Act
            _addVM.NameBoxGotFocusCommand.Execute();

            // Assert
            _addVM.NameEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void NameGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.NameEntryText = text;

            // Act
            _addVM.NameBoxGotFocusCommand.Execute();

            // Assert
            _addVM.NameEntryText.Should().Be(text);
        }

        #endregion

        #region Phone

        [TestMethod]
        public void PhoneLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.PhoneEntryText = string.Empty;

            // Act
            _addVM.PhoneBoxLostFocusCommand.Execute();

            // Assert
            _addVM.PhoneEntryText.Should().Be("Phone");
        }

        [TestMethod]
        public void PhoneLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.PhoneEntryText = text;

            // Act
            _addVM.PhoneBoxLostFocusCommand.Execute();

            // Assert
            _addVM.PhoneEntryText.Should().Be(text);
        }

        [TestMethod]
        public void PhoneGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.PhoneEntryText = "Phone";

            // Act
            _addVM.PhoneBoxGotFocusCommand.Execute();

            // Assert
            _addVM.PhoneEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void PhoneGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.PhoneEntryText = text;

            // Act
            _addVM.PhoneBoxGotFocusCommand.Execute();

            // Assert
            _addVM.PhoneEntryText.Should().Be(text);
        }

        #endregion
  
        #region Email

        [TestMethod]
        public void EmailLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.EmailEntryText = string.Empty;

            // Act
            _addVM.EmailBoxLostFocusCommand.Execute();

            // Assert
            _addVM.EmailEntryText.Should().Be("Email");
        }

        [TestMethod]
        public void EmailLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.EmailEntryText = text;

            // Act
            _addVM.EmailBoxLostFocusCommand.Execute();

            // Assert
            _addVM.EmailEntryText.Should().Be(text);
        }

        [TestMethod]
        public void EmailGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.EmailEntryText = "Email";

            // Act
            _addVM.EmailBoxGotFocusCommand.Execute();

            // Assert
            _addVM.EmailEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void EmailGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.EmailEntryText = text;

            // Act
            _addVM.EmailBoxGotFocusCommand.Execute();

            // Assert
            _addVM.EmailEntryText.Should().Be(text);
        }

        #endregion
       
        #region Adr

        [TestMethod]
        public void AdrLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.AdrEntryText = string.Empty;

            // Act
            _addVM.AdrBoxLostFocusCommand.Execute();

            // Assert
            _addVM.AdrEntryText.Should().Be("Address");
        }

        [TestMethod]
        public void AdrLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.AdrEntryText = text;

            // Act
            _addVM.AdrBoxLostFocusCommand.Execute();

            // Assert
            _addVM.AdrEntryText.Should().Be(text);
        }

        [TestMethod]
        public void AdrGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.AdrEntryText = "Address";

            // Act
            _addVM.AdrBoxGotFocusCommand.Execute();

            // Assert
            _addVM.AdrEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void AdrGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.AdrEntryText = text;

            // Act
            _addVM.AdrBoxGotFocusCommand.Execute();

            // Assert
            _addVM.AdrEntryText.Should().Be(text);
        }

        #endregion

        #region City

        [TestMethod]
        public void CityLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.CityEntryText = string.Empty;

            // Act
            _addVM.CityBoxLostFocusCommand.Execute();

            // Assert
            _addVM.CityEntryText.Should().Be("City");
        }

        [TestMethod]
        public void CityLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.CityEntryText = text;

            // Act
            _addVM.CityBoxLostFocusCommand.Execute();

            // Assert
            _addVM.CityEntryText.Should().Be(text);
        }

        [TestMethod]
        public void CityGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.CityEntryText = "City";

            // Act
            _addVM.CityBoxGotFocusCommand.Execute();

            // Assert
            _addVM.CityEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void CityGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.CityEntryText = text;

            // Act
            _addVM.CityBoxGotFocusCommand.Execute();

            // Assert
            _addVM.CityEntryText.Should().Be(text);
        }

        #endregion

        #region State

        [TestMethod]
        public void StateLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.StateEntryText = string.Empty;

            // Act
            _addVM.StateBoxLostFocusCommand.Execute();

            // Assert
            _addVM.StateEntryText.Should().Be("State");
        }

        [TestMethod]
        public void StateLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.StateEntryText = text;

            // Act
            _addVM.StateBoxLostFocusCommand.Execute();

            // Assert
            _addVM.StateEntryText.Should().Be(text);
        }

        [TestMethod]
        public void StateGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.StateEntryText = "State";

            // Act
            _addVM.StateBoxGotFocusCommand.Execute();

            // Assert
            _addVM.StateEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void StateGotFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.StateEntryText = text;

            // Act
            _addVM.StateBoxGotFocusCommand.Execute();

            // Assert
            _addVM.StateEntryText.Should().Be(text);
        }

        #endregion

        #endregion

        #region Text Updates

        [TestMethod]
        public void NameTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.NameEntryText="Will Smith";

            // Act
            _addVM.NameBoxTextChangedCommand.Execute();

            // Assert
            _addVM.NameErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void EmailTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.EmailEntryText = "wsmith@gmail.com";

            // Act
            _addVM.EmailBoxTextChangedCommand.Execute();

            // Assert
            _addVM.EmailErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void AddressTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.AdrEntryText = "12 Startstruck Boulevard";

            // Act
            _addVM.AdrBoxTextChangedCommand.Execute();

            // Assert
            _addVM.AdrErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void CityTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.CityEntryText = "Phoenix";

            // Act
            _addVM.CityBoxTextChangedCommand.Execute();

            // Assert
            _addVM.CityErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void StateTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.StateEntryText = "Arizona";

            // Act
            _addVM.StateBoxTextChangedCommand.Execute();

            // Assert
            _addVM.StateErrorVisibility.Should().Be(Visibility.Collapsed);
        }

            //Bad Names

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void NameTextChanged_Blank_ShouldShowAppropriateError(string name)
        {
            // Arrange
            _addVM.NameEntryText = name;

            // Act
            _addVM.NameBoxTextChangedCommand.Execute();

            // Assert
            _addVM.NameErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.NameErrorText.Should().Be("Name cannot be blank.");
        }

        [TestMethod]
        public void NameTextChanged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var name = Generators.GenRandomString(101);
            _addVM.NameEntryText=name;

            // Act
            _addVM.NameBoxTextChangedCommand.Execute();

            // Assert

            _addVM.NameErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.NameErrorText.Should().Be("Name must not exceed 100 characters.");
        }

            //Bad Emails

        [TestMethod]
        public void EmailTextChaged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var email = (over50 + "@" + over50).Substring(0, 101);
            _addVM.EmailEntryText=email;

            // Act
            _addVM.EmailBoxTextChangedCommand.Execute();

            // Assert
            _addVM.EmailErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.EmailErrorText.Should().Be("Email must not exceed 100 characters.");
        }

        [TestMethod]
        public void EmailTextChaged_WrongFormat_ShouldShowAppropriateError()
        {
            // Arrange
            var email = Generators.GenRandomString(20).Replace("@",string.Empty);
            _addVM.EmailEntryText=email;

            // Act
            _addVM.EmailBoxTextChangedCommand.Execute();

            // Assert
            _addVM.EmailErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.EmailErrorText.Should().Be("Email must be in a valid format (account@provider).");
        }

        //Bad Phones

        //[DataTestMethod]
        //[DataRow("")]
        //[DataRow(null)]
        //public void PhoneTextChanged_Blank_ShouldShowAppropriateError(string phone)
        //{
        //    // Arrange
        //    _addVM.PhoneEntryText=phone);

        //    // Act
        //    _addVM.PhoneBoxTextChangedCommand.Execute();

        //    // Assert
        //    _addVM.PhoneCheckVisibility.Should().Be(Visibility.Hidden);
        //    _addVM.PhoneErrorVisibility.Should().Be(Visibility.Visible);
        //    _addVM.PhoneErrorText.Should().Be("Customer requires a phone number.");
        //}

        //[DataTestMethod]
        //[DataRow("1234567890")]
        //[DataRow("123-456-78910")]
        //public void PhoneTextChanged_LengthWrong_ShouldShowAppropriateError(string phone)
        //{
        //    // Arrange
        //    _addVM.PhoneEntryText=phone);

        //    // Act
        //    _addVM.PhoneBoxTextChangedCommand.Execute();

        //    // Assert
        //    _addVM.PhoneCheckVisibility.Should().Be(Visibility.Hidden);
        //    _addVM.PhoneErrorVisibility.Should().Be(Visibility.Visible);
        //    _addVM.PhoneErrorText.Should().Be("Phone Numbers must have precisely 10 characters.");
        //}

        //[TestMethod]
        //public void PhoneTextChanged_NoDashes_ShouldShowAppropriateError()
        //{
        //    // Arrange
        //    _addVM.PhoneEntryText="123+123+1234;

        //    // Act
        //    _addVM.PhoneBoxTextChangedCommand.Execute();

        //    // Assert
        //    _addVM.PhoneCheckVisibility.Should().Be(Visibility.Hidden);
        //    _addVM.PhoneErrorVisibility.Should().Be(Visibility.Visible);
        //    _addVM.PhoneErrorText.Should().Be("Phone number must be in valid format: XXX-XXX-XXXX.");
        //}

        //[TestMethod]
        //public void PhoneTextChanged_NonDigits_ShouldShowAppropriateError()
        //{
        //    // Arrange
        //    _addVM.PhoneEntryText="123-123-123a;

        //    // Act
        //    _addVM.PhoneBoxTextChangedCommand.Execute();

        //    // Assert
        //    _addVM.PhoneCheckVisibility.Should().Be(Visibility.Hidden);
        //    _addVM.PhoneErrorVisibility.Should().Be(Visibility.Visible);
        //    _addVM.PhoneErrorText.Should().Be("Phone number may only contain digits.");
        //}

        // Bad Addresses

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void AddressTextChanged_Blank_ShouldShowAppropriateError(string adr)
        {
            // Arrange
            _addVM.AdrEntryText=adr;

            // Act
            _addVM.AdrBoxTextChangedCommand.Execute();

            // Assert
            _addVM.AdrErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.AdrErrorText.Should().Be("Address cannot be empty.");
        }

        [TestMethod]
        public void AddressTextChanged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var adr = Generators.GenRandomString(201);
            _addVM.AdrEntryText=adr;

            // Act
            _addVM.AdrBoxTextChangedCommand.Execute();

            // Assert
            _addVM.AdrErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.AdrErrorText.Should().Be("Address may not exceed 200 characters.");
        }

            // Bad Cities

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void CityTextChanged_Blank_ShouldShowAppropriateError(string city)
        {
            // Arrange
            _addVM.CityEntryText=city;

            // Act
            _addVM.CityBoxTextChangedCommand.Execute();

            // Assert
            _addVM.CityErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.CityErrorText.Should().Be("City field cannot be blank.");
        }

        [TestMethod]
        public void CityTextChanged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var city = Generators.GenRandomString(101);
            _addVM.CityEntryText=city;

            // Act
            _addVM.CityBoxTextChangedCommand.Execute();

            // Assert
            _addVM.CityErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.CityErrorText.Should()
                .Be("City should not exceed 100 characters; If necessary, please use an abbreviation.");
        }

            // Bad State

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void StateTextChanged_Blank_ShouldShowAppropriateError(string state)
        {
            // Arrange
            _addVM.StateEntryText=state;

            // Act
            _addVM.StateBoxTextChangedCommand.Execute();

            // Assert
            _addVM.StateErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.StateErrorText.Should().Be("State/County cannot be blank.");
        }

        [TestMethod]
        public void StateTextChanged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var state = Generators.GenRandomString(51);
            _addVM.StateEntryText=state;

            // Act
            _addVM.StateBoxTextChangedCommand.Execute();

            // Assert
            _addVM.StateErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.StateErrorText.Should()
                .Be("State/County is limited to 50 characters; Where necessary, please use an abbreviation.");
        }

        #endregion

        [TestMethod]
        public void Submit_HappyFlow_ShouldRegisterCustomer()
        {
            // Arrange
            var cust = Generators.GenCustomer();
            _addVM.NameEntryText = cust.Name;
            _addVM.PhoneEntryText = cust.Phone;
            _addVM.EmailEntryText = cust.Email;
            _addVM.AdrEntryText = cust.Address;
            _addVM.CityEntryText = cust.City;
            _addVM.StateEntryText = cust.State;

            // Act
            _addVM.SubmitCommand.Execute();
            var result = _service.GetAllFiltered(cust.Phone).SingleOrDefault();

            // Assert
            result.Should().BeEquivalentTo(cust);
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
            _addVM.NameEntryText = name;
            _addVM.PhoneEntryText = phone;
            _addVM.EmailEntryText = email;
            _addVM.AdrEntryText = adr;
            _addVM.CityEntryText = city;
            _addVM.StateEntryText = state;

            var initCount = _service.GetAll().ToList().Count;

            // Act
            _addVM.SubmitCommand.Execute();
            var newCount = _service.GetAll().ToList().Count;

            // Assert
            newCount.Should().Be(initCount);
        }
    }
}
