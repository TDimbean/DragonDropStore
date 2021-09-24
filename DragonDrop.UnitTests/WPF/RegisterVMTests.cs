using DragonDrop.BLL.DataServices;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.MainSubViews.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows;

namespace DragonDrop.UnitTests.WPF
{
    [TestClass]
    [Ignore]
    public class RegisterVMTests
    {
        private TestDb _db;
        private TestCustomerRepo _repo;
        private CustomerDataService _service;
        private PrivateObject _vm;

        string over50 = Generators.GenRandomString(51);
        string over100 = Generators.GenRandomString(101);
        string over200 = Generators.GenRandomString(201);

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestCustomerRepo(_db);
            _service = new CustomerDataService(_repo);
            _vm = new PrivateObject(typeof(RegisterViewModel), _service);
        }

        #region Entry Text Change Handlers

        //Valid entries

        [TestMethod]
        public void NameTextChanged_ToValid_ShouldShowCheck()
        {
            // Arrange
            _vm.SetProperty("NameBoxText", "Will Smith");

            // Act
            _vm.Invoke("NameTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("NameCheckVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("NameErrorVisibility").Should().Be(Visibility.Hidden);
        }

    //    [TestMethod]
    //    public void PhoneTextChanged_ToValid_ShouldShowCheck()
    //    {
    //        // Arrange
    //        var regVM = new RegisterViewModel(_service);
    //        ConstructorInfo changeCtor =
    //            typeof(TextChange)
    //            .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
    //null,
    //new[] { typeof(int) },
    //null);

    //        ConstructorInfo constructor =
    //            typeof(TextChangedEventArgs)
    //            .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
    //null,
    //new[] { typeof(TextChange) },
    //null);

    //        TextChange change =
    //            (TextChange)changeCtor.Invoke(new object[] { 1, 1, 0 });

    //        TextChangedEventArgs eventArgs =
    //            (TextChangedEventArgs)constructor.Invoke(new object[] { change });

    //        regVM.PhoneBoxText = "000-000-0000";
    //        //_vm.SetProperty("PhoneBoxText", "000-000-0000");

    //        // Act
    //        //_vm.Invoke("PhoneTextChangedCommandExecute", eventArgs);
    //        var lastRes = new RoutedEventArgs() as TextChangedEventArgs;
    //        regVM.PhoneTextChangedCommand.Execute(new RoutedEventArgs() as TextChangedEventArgs);

    //        // Assert
    //        //_vm.GetProperty("PhoneCheckVisibility").Should().Be(Visibility.Visible);
    //        //_vm.GetProperty("PhoneErrorVisibility").Should().Be(Visibility.Hidden);
    //        regVM.PhoneCheckVisibility.Should().Be(Visibility.Visible);
    //        regVM.PhoneErrorVisibility.Should().Be(Visibility.Hidden);
    //    }

        [TestMethod]
        public void EmailTextChanged_ToValid_ShouldShowCheck()
        {
            // Arrange
            _vm.SetProperty("EmailBoxText", "willieS@gmail.com");

            // Act
            _vm.Invoke("EmailTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("EmailCheckVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("EmailErrorVisibility").Should().Be(Visibility.Hidden);
        }

        [TestMethod]
        public void AdrTextChanged_ToValid_ShouldShowCheck()
        {
            // Arrange
            _vm.SetProperty("AdrBoxText", "145 Starstruck Avenue");

            // Act
            _vm.Invoke("AdrTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("AdrCheckVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("AdrErrorVisibility").Should().Be(Visibility.Hidden);
        }

        [TestMethod]
        public void CityTextChanged_ToValid_ShouldShowCheck()
        {
            // Arrange
            _vm.SetProperty("CityBoxText", "Phoenix");

            // Act
            _vm.Invoke("CityTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("CityCheckVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("CityErrorVisibility").Should().Be(Visibility.Hidden);
        }

        [TestMethod]
        public void StateTextChanged_ToValid_ShouldShowCheck()
        {
            // Arrange
            _vm.SetProperty("StateBoxText", "Arizona");

            // Act
            _vm.Invoke("StateTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("StateCheckVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("StateErrorVisibility").Should().Be(Visibility.Hidden);
        }

            //Bad Names

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void NameTextChanged_Blank_ShouldShowAppropriateError(string name)
        {
            // Arrange
            _vm.SetProperty("NameBoxText", name);

            // Act
            _vm.Invoke("NameTextChangedCommandExecute");

            // Assert

            _vm.GetProperty("NameCheckVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("NameErrorVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("NameErrorText").Should().Be("Name cannot be blank.");
        }

        [TestMethod]
        public void NameTextChanged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var name = Generators.GenRandomString(101);
            _vm.SetProperty("NameBoxText", name);

            // Act
            _vm.Invoke("NameTextChangedCommandExecute");

            // Assert

            _vm.GetProperty("NameCheckVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("NameErrorVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("NameErrorText").Should().Be("Name must not exceed 100 characters.");
        }

            //Bad Emails

        [TestMethod]
        public void EmailTextChaged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var email = (over50+"@"+over50).Substring(0,101);
            _vm.SetProperty("EmailBoxText", email);

            // Act
            _vm.Invoke("EmailTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("EmailCheckVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("EmailErrorVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("EmailErrorText").Should().Be("Email must not exceed 100 characters.");
        }

        [TestMethod]
        public void EmailTextChaged_WrongFormat_ShouldShowAppropriateError()
        {
            // Arrange
            var email = Generators.GenRandomString(20).Replace("@", string.Empty);
            _vm.SetProperty("EmailBoxText", email);

            // Act
            _vm.Invoke("EmailTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("EmailCheckVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("EmailErrorVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("EmailErrorText").Should().Be("Email must be in a valid format (account@provider).");
        }

        //Bad Phones

        //[DataTestMethod]
        //[DataRow("")]
        //[DataRow(null)]
        //public void PhoneTextChanged_Blank_ShouldShowAppropriateError(string phone)
        //{
        //    // Arrange
        //    _vm.SetProperty("PhoneBoxText", phone);

        //    // Act
        //    _vm.Invoke("PhoneTextChangedCommandExecute");

        //    // Assert
        //    _vm.GetProperty("PhoneCheckVisibility").Should().Be(Visibility.Hidden);
        //    _vm.GetProperty("PhoneErrorVisibility").Should().Be(Visibility.Visible);
        //    _vm.GetProperty("PhoneErrorText").Should().Be("Customer requires a phone number.");
        //}

        //[DataTestMethod]
        //[DataRow("1234567890")]
        //[DataRow("123-456-78910")]
        //public void PhoneTextChanged_LengthWrong_ShouldShowAppropriateError(string phone)
        //{
        //    // Arrange
        //    _vm.SetProperty("PhoneBoxText", phone);

        //    // Act
        //    _vm.Invoke("PhoneTextChangedCommandExecute");

        //    // Assert
        //    _vm.GetProperty("PhoneCheckVisibility").Should().Be(Visibility.Hidden);
        //    _vm.GetProperty("PhoneErrorVisibility").Should().Be(Visibility.Visible);
        //    _vm.GetProperty("PhoneErrorText").Should().Be("Phone Numbers must have precisely 10 characters.");
        //}

        //[TestMethod]
        //public void PhoneTextChanged_NoDashes_ShouldShowAppropriateError()
        //{
        //    // Arrange
        //    _vm.SetProperty("PhoneBoxText", "123+123+1234");

        //    // Act
        //    _vm.Invoke("PhoneTextChangedCommandExecute");

        //    // Assert
        //    _vm.GetProperty("PhoneCheckVisibility").Should().Be(Visibility.Hidden);
        //    _vm.GetProperty("PhoneErrorVisibility").Should().Be(Visibility.Visible);
        //    _vm.GetProperty("PhoneErrorText").Should().Be("Phone number must be in valid format: XXX-XXX-XXXX.");
        //}

        //[TestMethod]
        //public void PhoneTextChanged_NonDigits_ShouldShowAppropriateError()
        //{
        //    // Arrange
        //    _vm.SetProperty("PhoneBoxText", "123-123-123a");

        //    // Act
        //    _vm.Invoke("PhoneTextChangedCommandExecute");

        //    // Assert
        //    _vm.GetProperty("PhoneCheckVisibility").Should().Be(Visibility.Hidden);
        //    _vm.GetProperty("PhoneErrorVisibility").Should().Be(Visibility.Visible);
        //    _vm.GetProperty("PhoneErrorText").Should().Be("Phone number may only contain digits.");
        //}

        // Bad Addresses

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void AddressTextChanged_Blank_ShouldShowAppropriateError(string adr)
        {
            // Arrange
            _vm.SetProperty("AdrBoxText", adr);

            // Act
            _vm.Invoke("AdrTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("AdrCheckVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("AdrErrorVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("AdrErrorText").Should().Be("Address cannot be empty.");
        }

        [TestMethod]
        public void AddressTextChanged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var adr = Generators.GenRandomString(201);
            _vm.SetProperty("AdrBoxText", adr);

            // Act
            _vm.Invoke("AdrTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("AdrCheckVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("AdrErrorVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("AdrErrorText").Should().Be("Address may not exceed 200 characters.");
        }

            // Bad Cities

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void CityTextChanged_Blank_ShouldShowAppropriateError(string city)
        {
            // Arrange
            _vm.SetProperty("CityBoxText", city);

            // Act
            _vm.Invoke("CityTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("CityCheckVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("CityErrorVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("CityErrorText").Should().Be("City field cannot be blank.");
        }

        [TestMethod]
        public void CityTextChanged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var city = Generators.GenRandomString(101);
            _vm.SetProperty("CityBoxText", city);

            // Act
            _vm.Invoke("CityTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("CityCheckVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("CityErrorVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("CityErrorText").Should()
                .Be("City should not exceed 100 characters; If necessary, please use an abbreviation.");
        }

            // Bad State

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void StateTextChanged_Blank_ShouldShowAppropriateError(string state)
        {
            // Arrange
            _vm.SetProperty("StateBoxText", state);

            // Act
            _vm.Invoke("StateTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("StateCheckVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("StateErrorVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("StateErrorText").Should().Be("State/County cannot be blank.");
        }

        [TestMethod]
        public void StateTextChanged_TooLong_ShouldShowAppropriateError()
        {
            // Arrange
            var state = Generators.GenRandomString(51);
            _vm.SetProperty("StateBoxText", state);

            // Act
            _vm.Invoke("StateTextChangedCommandExecute");

            // Assert
            _vm.GetProperty("StateCheckVisibility").Should().Be(Visibility.Hidden);
            _vm.GetProperty("StateErrorVisibility").Should().Be(Visibility.Visible);
            _vm.GetProperty("StateErrorText").Should()
                .Be("State/County is limited to 50 characters; Where necessary, please use an abbreviation.");
        }

        #endregion

        [TestMethod]
        public void Submit_HappyFlow_ShouldRegisterCustomer()
        {
            // Arrange
            var cust = Generators.GenCustomer();
            _vm.SetProperty("NameBoxText", cust.Name);
            _vm.SetProperty("PhoneBoxText", cust.Phone);
            _vm.SetProperty("EmailBoxText", cust.Email);
            _vm.SetProperty("AdrBoxText", cust.Address);
            _vm.SetProperty("CityBoxText", cust.City);
            _vm.SetProperty("StateBoxText", cust.State);

            // Act
            _vm.Invoke("SubmitBtnClickCommandExecute", new RoutedEventArgs());
            var result = _service.GetAllFiltered(cust.Phone);

            // Assert
            result.Should().BeEquivalentTo(cust);

        }

        [DataTestMethod]
        #region Bad Data
        [DataRow("", "030-030-0440", "Wsmith@gmail.com","1 Gil Peak", "Eek", "Alsaska")]
        [DataRow(null, "030-030-0440", "Wsmith@gmail.com","1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smithrdquofbdpycnqfrorqtuxobsxvmtyjypgclmobszvdjjmdpqxphzueevwbmugrzugbygjvwgufjgyrnfrzxjonytlge",
            "030-030-0440", "Wsmith@gmail.com","1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmithgmail.com","1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", 
            "pmzdpgtodtoedxftodzlgzzxknaqemavmlnhszzogoflgjptqo@pmzdpgtodtoedxftodzlgzzxknaqemavmlnhszzogoflgjptqo",
            "1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith","", "Wsmith@gmail.com","1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", null, "Wsmith@gmail.com","1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "1234567890", "Wsmith@gmail.com","1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "123-456-78910", "Wsmith@gmail.com","1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "123+123+1234", "Wsmith@gmail.com","1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "123-123-123a", "Wsmith@gmail.com","1 Gil Peak", "Eek", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com","", "Eek", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com",null, "Eek", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com",
"emakejmvzckstiyibetqhlrplffpvktdmjrhtjhfhakatjmzlwbgyarftbtlhgbrtvubqhngzrytahddzrbmdjviyugqzobmewkortuzsqnukxhhllnluyodheufoffndvupajudapiwmygxzdnhvjtpmwiklipbhrqotmkofogpkzvracuskwuphindgdilumusrvali", "Eek", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com","1 Gil Peak", "", "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com","1 Gil Peak", null, "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com","1 Gil Peak",
           "mfmkxzqpeaqjfypbgzgxeraydbvuwshgxzbbrywgpmojkmaztqrgxkvcxgefyumqfnodqbnqpimtahiarnnavfdoasxljmxljlkxe"
            , "Alsaska")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com","1 Gil Peak", "Eek", "")]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com","1 Gil Peak", "Eek", null)]
        [DataRow("Will Smith", "030-030-0440", "Wsmith@gmail.com","1 Gil Peak", "Eek",
            "sicpihjuqzyxfkfaxwewmeovhqcqpgvhjsgfhcojagiaiyzqiay")]
        #endregion
        [TestMethod]
        public void Submit_BadData_ShouldNotRegisterCustomer
            (string name, string phone, string email, string adr, string city, string state)
        {
            // Arrange
            var cust = Generators.GenCustomer();
            _vm.SetProperty("NameBoxText", name);
            _vm.SetProperty("PhoneBoxText", phone);
            _vm.SetProperty("EmailBoxText", email);
            _vm.SetProperty("AdrBoxText", adr);
            _vm.SetProperty("CityBoxText", city);
            _vm.SetProperty("StateBoxText", state);

            var initCount = _service.GetAll().ToList().Count;

            // Act
            _vm.Invoke("SubmitBtnClickCommandExecute", new RoutedEventArgs());
            var newCount = _service.GetAll().ToList().Count;

            // Assert
            newCount.Should().Be(initCount);
        }
    }
}
