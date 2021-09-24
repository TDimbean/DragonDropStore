using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.Infrastructure.Helpers;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonDrop.UnitTests.BLL
{
    [TestClass]
    public class CustomerDataServiceTests
    {
        private ICustomerDataService _service;
        private ICustomerRepository _repo;
        private TestDb _db;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestCustomerRepo(_db);
            _service = new CustomerDataService(_repo);
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldReturnTarget()
        {
            var target = _db.Customers.FirstOrDefault();

            var result = _service.Get(target.CustomerId);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNull()
        {
            var id = Generators.GenInexistentCustomerId();

            var result = _service.Get(id);

            result.Should().BeNull();
        }

        [TestMethod]
        public void EmailExists_ItDoes_ShouldReturnTrue()
        {
            var email = _db.Customers.FirstOrDefault().Email;

            var result = _service.EmailExists(email);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void EmailExists_ItDoesnt_ShouldReturnFalse()
        {
            var email = Generators.GenEmailAddress();

            var result = _service.EmailExists(email);

            result.Should().BeFalse();
        }

        public void PhoneExists_ItDoes_ShouldReturnTrue()
        {
            var phone = _db.Customers.FirstOrDefault().Phone;

            var result = _service.PhoneExists(phone);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void PhoneExists_ItDoesnt_ShouldReturnFalse()
        {
            var phone = Generators.GenPhoneNumber();

            var result = _service.PhoneExists(phone);

            result.Should().BeFalse();
        }


        #region Creates

        [TestMethod]
        public void Create_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = new Customer
            {
                CustomerId = 4,
                Name = "Will Smith",
                Phone = Generators.GenPhoneNumber(),
                Email = null,
                Address = "104 Tropic Point",
                City = "Guerneville",
                State = "California"
            };

            // Act
            var result = _service.Create(newItem, true);
            var createdItem = _service.Get(newItem.CustomerId);

            // Assert
            result.Should().BeNull();
            createdItem.Should().BeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Create_NoName_ShouldNotCreateAndReturnAppropriateError(string name)
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Name = name;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("Name cannot be blank.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public void Create_NameTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Name = Generators.GenRandomString(101);

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("Name must not exceed 100 characters.");
            createdCustomer.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Create_NoPhone_ShouldNotCreateAndReturnAppropriateError(string phone)
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Phone = phone;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("Customer requires a phone number.");
            createdCustomer.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("1234567890")]
        [DataRow("123-456-78910")]
        public void Create_PhoneLengthWrong_ShouldNotCreateAndReturnAppropriateError(string phone)
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Phone = phone;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("Phone Numbers must have precisely 10 characters.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public void Create_PhoneNoDashes_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Phone = "123+123+1234";

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("Phone number must be in valid format: XXX-XXX-XXXX.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public void Create_PhoneNonDigits_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Phone = "123-123-123a";

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("Phone number may only contain digits.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public void Create_EmailTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.Email = Generators.GenRandomString(101);

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("Email must not exceed 100 characters.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public void Create_EmailWrongFormat_ShouldNotCreateAndReturnAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.Email = Generators.GenRandomString(40).Replace("@","");

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("Email must be in a valid format (account@provider).");
            createdCustomer.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Create_NoAddress_ShouldNotCreateAndReturnAppropriateError(string adr)
        {
            var newItem = Generators.GenCustomer();
            newItem.Address = adr;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("Address cannot be empty.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public void Create_AddressTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.Address = Generators.GenRandomString(201);

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("Address may not exceed 200 characters.");
            createdCustomer.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Create_NoCity_ShouldNotCreateAndReturnAppropriateError(string city)
        {
            var newItem = Generators.GenCustomer();
            newItem.City = city;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("City field cannot be blank.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public void Create_CityTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.City = Generators.GenRandomString(101);

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("City should not exceed 100 characters; If necessary, please use an abbreviation.");
            createdCustomer.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Create_NoState_ShouldNotCreateAndReturnAppropriateError(string state)
        {
            var newItem = Generators.GenCustomer();
            newItem.State = state;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("State/County cannot be blank.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public void Create_StateTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.State = Generators.GenRandomString(51);

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdCustomer = _service.Get(0);

            // Assert
            result.Should().Be("State/County is limited to 50 characters; Where necessary, please use an abbreviation.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public void Create_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        {
            var newItem = Generators.GenCustomer();
            newItem.State = null;
            newItem.City = null;

            //// Act
            var result = _service.Create(newItem, true);
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();
            var createdCustomer = _service.Get(0);

            // Assert
            //result.Should().Be("City field cannot be blank.\nState/County cannot be blank.\n");
            err1.Should().Be("City field cannot be blank.");
            err2.Should().Be("State/County cannot be blank.");
            blank.Should().BeEmpty();
            createdCustomer.Should().BeNull();
        }

        #endregion

        #region Updates

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var oldItem = new Customer
            {
                CustomerId = target.CustomerId,
                Address = target.Address,
                City = target.City,
                Email = target.Email,
                Name = target.Name,
                Phone = target.Phone,
                State = target.State
            };
            var targetId = target.CustomerId;
            var newItem = new Customer
            {
                CustomerId = targetId,
                Name = "Will Smith",
                Phone = Generators.GenPhoneNumber(),
                Email = null,
                Address = "104 Tropic Point",
                City = "Guerneville",
                State = "California"
            };

            // Act
            var result = _service.Update(newItem, true);
            var updatedItem = _service.Get(targetId);

            // Assert
            result.Should().BeNull();
            updatedItem.Should().BeEquivalentTo(newItem);
            updatedItem.Should().NotBeEquivalentTo(oldItem);
        }

        [TestMethod]
        public void Update_InexistentId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var targetId = Generators.GenInexistentCustomerId();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = targetId;
            var oldRepo = _service.GetAll();

            // Act
            var result = _service.Update(newItem, true);
            var newRepo = _service.GetAll();

            // Assert
            result.Should().Be("Customer ID must exist within the Repo for an update to be performed.");
            newRepo.Should().BeEquivalentTo(oldRepo);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Update_NoName_ShouldNotUpdateAndReturnAppropriateError(string name)
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Name = name;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("Name cannot be blank.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NameTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Name = Generators.GenRandomString(101);

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("Name must not exceed 100 characters.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Update_NoPhone_ShouldNotUpdateAndReturnAppropriateError(string phone)
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Phone = phone;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("Customer requires a phone number.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("1234567890")]
        [DataRow("123-456-78910")]
        public void Update_PhoneLengthWrong_ShouldNotUpdateAndReturnAppropriateError(string phone)
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Phone = phone;

            //// Act
            //Only returns the first error by loooking for the first occurance of [.]. The alternative was to use an Array or List of string.
            //Wasn't too keen on passing a memory intensive data structure between layers just to display 1 or 2 sentences.
            var result = _service.Update(newItem, true).Trim().GetUntilOrEmpty(".");
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("Phone Numbers must have precisely 10 characters.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_PhoneNoDashes_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Phone = "123212311234";

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("Phone number must be in valid format: XXX-XXX-XXXX.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_PhoneNonDigits_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Phone = "123-123-123a";

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("Phone number may only contain digits.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_EmailTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Email = Generators.GenRandomString(101);

            // Act
            var result = _service.Update(newItem, true).Trim().GetUntilOrEmpty(".");
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("Email must not exceed 100 characters.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_EmailWrongFormat_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Email = Generators.GenRandomString(20).Replace("@","");

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("Email must be in a valid format (account@provider).");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Update_NoAddress_ShouldNotUpdateAndReturnAppropriateError(string adr)
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Address = adr;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("Address cannot be empty.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_AddressTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Address = Generators.GenRandomString(201);

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("Address may not exceed 200 characters.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Update_NoCity_ShouldNotUpdateAndReturnAppropriateError(string city)
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.City = city;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("City field cannot be blank.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_CityTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.City = Generators.GenRandomString(101);

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("City should not exceed 100 characters; If necessary, please use an abbreviation.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Update_NoState_ShouldNotUpdateAndReturnAppropriateError(string state)
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.State = state;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("State/County cannot be blank.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_StateTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.State = Generators.GenRandomString(51);

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            result.Should().Be("State/County is limited to 50 characters; Where necessary, please use an abbreviation.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.State = null;
            newItem.City = null;

            //// Act
            var result = _service.Update(newItem, true);
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();
            var updatedCustomer = _service.Get(target.CustomerId);

            // Assert
            err1.Should().Be("City field cannot be blank.");
            err2.Should().Be("State/County cannot be blank.");
            blank.Should().BeEmpty();
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region Gets

        [TestMethod]
        public void GetAll_HappyFlow_ShouldFetchAll()
        {
            var result = _service.GetAll();

            result.Should().BeEquivalentTo(_db.Customers);
        }

        #region Paginated

        [TestMethod]
        public void GetAllPaginated_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            int pgSize = 1;
            int pgIndex = 3;
            var expected = _db.Customers.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();

            // Act
            var result = _service.GetAllPaginated(pgSize, pgIndex);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_FullPage_ShouldReturnRightSize()
        {
            var result = _service.GetAllPaginated(2, 1);

            result.Count().Should().Be(2);
        }

        [TestMethod]
        public void GetAllPaginated_PartialPage_ShouldReturnRightSize()
        {
            var result = _service.GetAllPaginated(2, 2);

            result.Count().Should().Be(1);
        }

        [TestMethod]
        public void GetAllPaginated_SizeTooBig_ShouldReturnAll()
        {
            // Arrange
            var expected = _db.Customers;
            var oversizedPage = expected.Count() + 10;

            // Act
            var result = _service.GetAllPaginated(oversizedPage, 1);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_IndexTooHigh_ShouldReturnEmpty()
        {
            var overIndex = _db.Customers.Count() + 10;

            var result = _service.GetAllPaginated(1, overIndex);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllPaginated_SizeZero_ShouldReturnAll()
        {
            var result = _service.GetAllPaginated(0, 2);

            result.Should().BeEquivalentTo(_db.Customers);
        }

        [TestMethod]
        public void GetAllPaginated_IndexZero_ShouldReturnAll()
        {
            var result = _service.GetAllPaginated(2, 0);

            result.Should().BeEquivalentTo(_db.Customers);
        }

        [DataTestMethod]
        [DataRow(-1,2)]
        [DataRow(1,-2)]
        public void GetAllPaginated_NegativeNumbers_ShouldConvertToPositive(int pgSize, int pgIndex)
        {
            var expected = _service.GetAllPaginated(1, 2);

            var result = _service.GetAllPaginated(pgSize, pgIndex);

            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region Filtered

        [TestMethod]
        public void GetAllFiltered_NameMatch_ShouldReturnMatch()
        {
            var target = _db.Customers.FirstOrDefault();
            var expected = new List<Customer> { target };

            var result = _service.GetAllFiltered(target.Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_EmailMatch_ShouldReturnMatch()
        {
            var target = _db.Customers.Where(c => c.Email != null).ToList().FirstOrDefault();
            var expected = new List<Customer> { target };

            var result = _service.GetAllFiltered(target.Email);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_AddressMatch_ShouldReturnMatch()
        {
            var target = _db.Customers.ToList().FirstOrDefault();
            var expected = new List<Customer> { target };

            var result = _service.GetAllFiltered(target.Email);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_CityMatch_ShouldReturnMatch()
        {
            var target = _db.Customers.ToList().FirstOrDefault();
            var expected = new List<Customer> { target };

            var result = _service.GetAllFiltered(target.City);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_StateMatch_ShouldReturnMatch()
        {
            //Need to use LastOrDefault, because first record hails from District Of Columbia, the "of" being a match both on the expected State field from the target record and with the "of" in "Micros[of]t" from the second record's Email field. Could make the Email search require a perfect match, but then you couldn't search Customers based on mail provider"
            var target = _db.Customers.ToList().LastOrDefault();
            var expected = new List<Customer> { target };

            var result = _service.GetAllFiltered(target.State);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_StateAbbreviationMatch_ShouldReturnMatch()
        {
            // Arrange
            var target = new Customer
            {
                Name = "George Claudius",
                Phone = Generators.GenPhoneNumber(),
                Email = null,
                Address = "15 Pine Road",
                City = "Lost Springs",
                State = "Wyoming"
            };
            _service.Create(target);
            var expected = new List<Customer> { target };

            // Act
            var result = _service.GetAllFiltered("WY");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_InexistentTerm_ShouldReturnEmpty()
        {
            var search = Guid.NewGuid().ToString();

            var result = _service.GetAllFiltered(search);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllFiltered_DifTermsForDifResults_ShoudReturnBothMatches()
        {
            // Arrange
            var target1 = _db.Customers.FirstOrDefault();
            var target2 = _db.Customers.Skip(1).FirstOrDefault();
            var search = target1.Name.Split(' ').FirstOrDefault() + " " +
                         target2.Name.Split(' ').FirstOrDefault();
            var expected = new List<Customer> { target1, target2 };

            // Act
            var result = _service.GetAllFiltered(search);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_HappyFlow_ShouldReturnMatch()
        {
            var expected = new List<Customer> { _service.Get(2) };

            var result = _service.GetAllFilteredAndPaged("of", 1, 2);

            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region Sorts

        [DataTestMethod]
        [DataRow("ID")]
        [DataRow("CUSTOMERID")]
        [DataRow("id")]
        [DataRow("customerId")]
        public void GetAllSorted_ById_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.CustomerId);

            var result = _service.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("NaME")]
        [DataRow("NAME")]
        public void GetAllSorted_ByName_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.Name);

            var result = _service.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("ADR")]
        [DataRow("ADDRESS")]
        [DataRow("address")]
        [DataRow("adr")]
        public void GetAllSorted_ByAddress_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.Address);

            var result = _service.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("PHONE")]
        [DataRow("phone")]
        [DataRow("PHONENUMBER")]
        [DataRow("PhoneNumber")]
        [DataRow("NUMBER")]
        [DataRow("NumbER")]
        [DataRow("PHONENO")]
        [DataRow("PhoneNO")]
        [DataRow("PhoneNO")]
        [DataRow("PHONE_NO")]
        [DataRow("phone_no")]
        [DataRow("PHONE_NUMBER")]
        [DataRow("PhONe_NumbEr")]
        [DataRow("TELEPHONE")]
        [DataRow("telephone")]
        public void GetAllSorted_ByPhone_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.Phone);

            var result = _service.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("EMAIL")]
        [DataRow("email")]
        [DataRow("E-MAIL")]
        [DataRow("e-mail")]
        [DataRow("MAIL")]
        [DataRow("mAiL")]
        public void GetAllSorted_ByEmail_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.Email);

            var result = _service.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("City")]
        [DataRow("CITY")]
        [DataRow("TOWN")]
        [DataRow("ToWn")]
        public void GetAllSorted_ByCity_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.City);

            var result = _service.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("STATE")]
        [DataRow("sTaTe")]
        [DataRow("COUNTY")]
        [DataRow("County")]
        public void GetAllSorted_ByState_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.State);

            var result = _service.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("asdfok")]
        public void GetAllSorted_BadQuery_ShouldReturnAll(string sortBy)
        {
            var expected = _service.GetAll();

            var resultAsc = _service.GetAllSorted(sortBy);
            var resultDes = _service.GetAllSorted(sortBy, true);

            resultAsc.Should().BeEquivalentTo(expected);
            resultDes.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSorted_Descending_ShouldSortDescending()
        {
            // Arrange
            var sortBy = "Name";
            var ascending = _service.GetAllSorted(sortBy);
            var expected = ascending.Reverse();

            // Act
            var result = _service.GetAllSorted(sortBy, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedAndPaged_ShouldReturnRequested()
        {
            // Arrange
            var sortBy = "Name";
            var pgIndex = 3;
            var pgSize = 1;

            var expected = new List<Customer> { _db.Customers.SingleOrDefault(c => c.CustomerId == 1) };

            // Act
            var result = _service.GetAllSortedAndPaged(pgSize, pgIndex, sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescendingAndPaged_ShouldReturnRequested()
        {
            // Arrange
            var sortBy = "Name";
            var pgIndex = 3;
            var pgSize = 1;

            var expected = new List<Customer> { _db.Customers.SingleOrDefault(c => c.CustomerId == 3) };

            // Act
            var result = _service.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            //// Arrange
            // Should return first 2 entries by State: District OF Colombia and Email: [...]@microsOFt.com respectively
            var searchBy = "of";
            // Should return the 2-item list in reverse order
            var sortBy = "email";

            var expected = _db.Customers.Take(2).Reverse();

            // Act
            var result = _service.GetAllFilteredAndSorted(searchBy, sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            //// Arrange
            // Should return first 2 entries by State: District OF Colombia and Email: [...]@microsOFt.com respectively
            var searchBy = "of";
            // Should return the 2-item list in reverse order
            var sortBy = "email";

            var expected = _db.Customers.Take(2);

            // Act
            var result = _service.GetAllFilteredAndSorted(searchBy, sortBy, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpCustomers();

            var expected = _db.Customers.Skip(4);

            // Act
            var result = _service.GetAllFilteredSortedAndPaged("CA", 2, 2, "name");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpCustomers();

            var repo = new TestCustomerRepo(_db);
            var expected = _db.Customers.Skip(2).Take(2);

            // Act
            var result = _service.GetAllFilteredSortedAndPaged("CA", 2, 2, "name", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }


        #endregion

        #endregion

        #region MainWindow Methods

        [TestMethod]
        public void OneCustWithEmailExists_TheyDo_ShouldReturnTrue()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();

            // Act
            var result = _service.OneCustWithEmailExists(target.Email);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void OneCustWithEmailExists_TheyDont_ShouldReturnFalse()
        {
            // Arrange
            var email = string.Empty;
            while (true)
            {
                email = Generators.GenRandomString(20) + "@gmail.com";
                if (!_db.Customers.Any(c => c.Email == email)) break;
            }

            // Act
            var result = _service.OneCustWithEmailExists(email);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void OneCustWithEmailExists_Duplicate_ShouldReturnFalse()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = new Customer
            {
                CustomerId = Generators.GenInexistentCustomerId(),
                Name = "William Wallace",
                Phone = Generators.GenPhoneNumber(),
                Address = "013 Nobody Owens Drive",
                State = target.State,
                City = target.City,
                Email = target.Email
            };
            _service.Create(newItem);


            // Act
            var result = _service.OneCustWithEmailExists(target.Email);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void FindIdByEmail_HappyFlow_ShouldReturnRightId()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();

            // Act
            var result = _service.FindIdByEmail(target.Email);

            // Assert
            result.Should().Be(target.CustomerId);
        }

        [TestMethod]
        public void FindIdByEmail_InexistentEmail_ShouldReturnNull()
        {
            // Arrange
            var email = string.Empty;
            while (true)
            {
                email = Generators.GenRandomString(20) + "@gmail.com";
                if (!_db.Customers.Any(c => c.Email == email)) break;
            }

            // Act
            var result = _service.FindIdByEmail(email);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void FindIdByEmail_Duplicate_ShouldReturnNull()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = new Customer
            {
                CustomerId = Generators.GenInexistentCustomerId(),
                Name = "William Wallace",
                Phone = Generators.GenPhoneNumber(),
                Address = "013 Nobody Owens Drive",
                State = target.State,
                City = target.City,
                Email = target.Email
            };
            _service.Create(newItem);


            // Act
            var result = _service.FindIdByEmail(target.Email);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void PassMatchesEmail_ItDoes_ShouldReturnTrue()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();

            // Act
            var result = _service.PassMatchesEmail(target.Email, target.Phone.Substring(0, 3));

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void PassMatchesEmail_ItDoesNot_ShouldReturnFalse()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var truePass = target.Phone.Substring(0, 3);
            var pass = string.Empty;
            while (true)
            {
                pass = Generators.GenRandomString(3);
                if (pass != truePass) break;
            }

            // Act
            var result = _service.PassMatchesEmail(target.Email, pass);

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("schaman0@hexun.com", null)]
        [DataRow(null, "202")]
        public void PassMatchesEmail_NullValue_ShouldReturnFalse(string email, string pass)
        {
            var result = _service.PassMatchesEmail(email, pass);

            result.Should().BeFalse();
        }

        #endregion

        #region Validates

        #region Whole Customer Validation

        [TestMethod]
        public void ValidateCustomer_HappyFlow_ShouldReturnTrueAndNull()
        {
            // Arrange
            var newItem = new Customer
            {
                CustomerId = 4,
                Name = "Will Smith",
                Phone = Generators.GenPhoneNumber(),
                Email = null,
                Address = "104 Tropic Point",
                City = "Guerneville",
                State = "California"
            };

            // Act
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.isValid.Should().BeTrue();
            result.errorList.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ValidateCustomer_NoName_ShouldReturnFalseAndAppropriateError(string name)
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Name = name;

            // Act
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Name cannot be blank.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateCustomer_NameTooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Name = Generators.GenRandomString(101);

            // Act
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Name must not exceed 100 characters.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ValidateCustomer_NoPhone_ShouldReturnFalseAndAppropriateError(string phone)
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Phone = phone;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("Customer requires a phone number.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("1234567890")]
        [DataRow("123-456-78910")]
        public void ValidateCustomer_PhoneLengthWrong_ShouldReturnFalseAndAppropriateError(string phone)
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Phone = phone;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.ValidateCustomer(newItem);
            var createdCustomer = _service.Get(0);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("Phone Numbers must have precisely 10 characters.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateCustomer_PhoneNoDashes_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Phone = "123+123+1234";

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("Phone number must be in valid format: XXX-XXX-XXXX.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateCustomer_PhoneNonDigits_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Phone = "123-123-123a";

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("Phone number may only contain digits.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateCustomer_EmailTooLong_ShouldReturnFalseAndAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.Email = Generators.GenRandomString(101);

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("Email must not exceed 100 characters.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ValidateCustomer_NoAddress_ShouldReturnFalseAndAppropriateError(string adr)
        {
            var newItem = Generators.GenCustomer();
            newItem.Address = adr;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("Address cannot be empty.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateCustomer_AddressTooLong_ShouldReturnFalseAndAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.Address = Generators.GenRandomString(201);

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("Address may not exceed 200 characters.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ValidateCustomer_NoCity_ShouldReturnFalseAndAppropriateError(string city)
        {
            var newItem = Generators.GenCustomer();
            newItem.City = city;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("City field cannot be blank.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateCustomer_CityTooLong_ShouldReturnFalseAndAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.City = Generators.GenRandomString(101);

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should()
                .Be("City should not exceed 100 characters; If necessary, please use an abbreviation.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ValidateCustomer_NoState_ShouldReturnFalseAndAppropriateError(string state)
        {
            var newItem = Generators.GenCustomer();
            newItem.State = state;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("State/County cannot be blank.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateCustomer_StateTooLong_ShouldReturnFalseAndAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.State = Generators.GenRandomString(51);

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = _service.ValidateCustomer(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should()
                .Be("State/County is limited to 50 characters; Where necessary, please use an abbreviation.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateCustomer_MultipleErrors_ShouldReturnFalseAndAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.State = null;
            newItem.City = null;

            //// Act
            var result = _service.ValidateCustomer(newItem);
            var err1 = result.errorList.Trim().GetUntilOrEmpty(".");
            var err2 = result.errorList.Replace(err1, "").Trim();
            var blank = result.errorList.Replace(err1, "").Replace(err2, "").Trim();

            // Assert
            err1.Should().Be("City field cannot be blank.");
            err2.Should().Be("State/County cannot be blank.");
            blank.Should().BeEmpty();
            result.isValid.Should().BeFalse();
        }

        #endregion

        #region Partial Validation

        #region Name

        [TestMethod]
        public void ValidateName_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var name = Generators.GenRandomString(new Random().Next(1, 101));

            // Act
            var result = _service.ValidateName(name);

            // Assert
            result.isValid.Should().BeTrue();
            result.errorList.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ValidateName_Blank_ShouldReturnFalseAndAppropriateError(string name)
        {
            var result = _service.ValidateName(name);

            // Assert
            result.errorList.Trim().Should().Be("Name cannot be blank.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateName_TooLong_ShouldReturnFalseAndAppropriateError()
        {
            //Assert
            var name = Generators.GenRandomString(101);

            // Act
            var result = _service.ValidateName(name);

            // Assert
            result.errorList.Trim().Should().Be("Name must not exceed 100 characters.");
            result.isValid.Should().BeFalse();
        }


        #endregion

        #region E-Mail

        [TestMethod]
        public void ValidateEmail_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var rnd = Generators.GenRandomString(new Random().Next(1, 50));
            var email = rnd + "@" + rnd;

            // Act
            var result = _service.ValidateEmail(email);

            // Assert
            result.isValid.Should().BeTrue();
            result.errorList.Should().BeEmpty();
        }

        [TestMethod]
        public void ValidateEmail_TooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var email = Generators.GenRandomString(101);

            // Act
            var result = _service.ValidateEmail(email);

            // Assert
            result.errorList.GetUntilOrEmpty(".").Should().Be("Email must not exceed 100 characters.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateEmail_WrongFormat_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var email = "bed.time_yahoo.com";

            // Act
            var result = _service.ValidateEmail(email);

            // Assert
            result.errorList.GetUntilOrEmpty(".").Should().Be("Email must be in a valid format (account@provider).");
            result.isValid.Should().BeFalse();
        }

        #endregion

        #region Phone

        [TestMethod]
        public void ValidatePhone_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var phone = Generators.GenPhoneNumber();

            // Act
            var result = _service.ValidatePhone(phone);

            // Assert
            result.isValid.Should().BeTrue();
            result.errorList.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ValidatePhone_Blank_ShouldReturnFalseAndAppropriateError(string phone)
        {
            var result = _service.ValidatePhone(phone);

            result.errorList.Trim().Should().Be("Customer requires a phone number.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("1234567890")]
        [DataRow("123-456-78910")]
        public void ValidatePhone_LengthWrong_ShouldReturnFalseAndAppropriateError(string phone)
        {
            var result = _service.ValidatePhone(phone);

            result.errorList.GetUntilOrEmpty(".").Trim().Should().Be("Phone Numbers must have precisely 10 characters.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePhone_NoDashes_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var phone = "123+123+1234";

            // Act
            var result = _service.ValidatePhone(phone);

            // Assert
            result.errorList.GetUntilOrEmpty(".").Trim().Should().Be("Phone number must be in valid format: XXX-XXX-XXXX.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePhone_NonDigits_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var phone = "123-123-123a";

            // Act
            var result = _service.ValidatePhone(phone);

            // Assert
            result.errorList.Trim().Should().Be("Phone number may only contain digits.");
            result.isValid.Should().BeFalse();
        }

        #endregion

        #region Address

        [TestMethod]
        public void ValidateAddress_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var adr = Generators.GenRandomString(new Random().Next(1, 201));

            // Act
            var result = _service.ValidateAddress(adr);

            // Assert
            result.isValid.Should().BeTrue();
            result.errorList.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ValidateAddress_Blank_ShouldReturnFalseAndAppropriateError(string adr)
        {
            var result = _service.ValidateAddress(adr);

            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("Address cannot be empty.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateAddress_TooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var adr = Generators.GenRandomString(201);

            // Act
            var result = _service.ValidateAddress(adr);

            // Assert
            result.errorList.Trim().Should().Be("Address may not exceed 200 characters.");
            result.isValid.Should().BeFalse();
        }

        #endregion

        #region City

        [TestMethod]
        public void ValidateCity_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var city = Generators.GenRandomString(new Random().Next(1, 101));

            // Act
            var result = _service.ValidateCity(city);

            // Assert
            result.isValid.Should().BeTrue();
            result.errorList.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ValidateCity_Blank_ShouldReturnFalseAndAppropriateError(string city)
        {
            var result = _service.ValidateCity(city);

            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("City field cannot be blank.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateCity_TooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var city = Generators.GenRandomString(101);

            // Act
            var result = _service.ValidateCity(city);

            // Assert
            result.errorList.Trim().Should()
                .Be("City should not exceed 100 characters; If necessary, please use an abbreviation.");
            result.isValid.Should().BeFalse();
        }

        #endregion

        #region State

        [TestMethod]
        public void ValidateState_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var state = Generators.GenRandomString(new Random().Next(1, 51));

            // Act
            var result = _service.ValidateState(state);

            // Assert
            result.isValid.Should().BeTrue();
            result.errorList.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ValidateState_Blank_ShouldReturnFalseAndAppropriateError(string state)
        {
            var result = _service.ValidateState(state);

            // Assert
            result.errorList.Trim().Should().Be("State/County cannot be blank.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateState_TooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var state = Generators.GenRandomString(51);

            // Act
            var result = _service.ValidateState(state);

            // Assert
            result.errorList.Trim().Should()
                .Be("State/County is limited to 50 characters; Where necessary, please use an abbreviation.");
            result.isValid.Should().BeFalse();
        }

        #endregion

        #endregion

        #endregion

        #region Async

        [TestMethod]
        public async Task GetAsync_HappyFlow_ShouldReturnTarget()
        {
            var target = _db.Customers.FirstOrDefault();

            var result = await _service.GetAsync(target.CustomerId);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public async Task GetAsync_InexistentId_ShouldReturnNull()
        {
            var id = Generators.GenInexistentCustomerId();

            var result = await _service.GetAsync(id);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task EmailExistsAsync_ItDoes_ShouldReturnTrue()
        {
            var email = _db.Customers.FirstOrDefault().Email;

            var result = await _service.EmailExistsAsync(email);

            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task EmailExistsAsync_ItDoesnt_ShouldReturnFalse()
        {
            var email = Generators.GenEmailAddress();

            var result = await _service.EmailExistsAsync(email);

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task PhoneExistsAsync_ItDoes_ShouldReturnTrue()
        {
            var phone = _db.Customers.FirstOrDefault().Phone;

            var result = await _service.PhoneExistsAsync(phone);

            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task PhoneExistsAsync_ItDoesnt_ShouldReturnFalse()
        {
            var phone = Generators.GenPhoneNumber();

            var result = await _service.PhoneExistsAsync(phone);

            result.Should().BeFalse();
        }


        #region Creates

        [TestMethod]
        public async Task CreateAsync_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = new Customer
            {
                CustomerId = 4,
                Name = "Will Smith",
                Phone = Generators.GenPhoneNumber(),
                Email = null,
                Address = "104 Tropic Point",
                City = "Guerneville",
                State = "California"
            };

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdItem = await _service.GetAsync(newItem.CustomerId);

            // Assert
            result.Should().BeNull();
            createdItem.Should().BeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task CreateAsync_NoName_ShouldNotCreateAndReturnAppropriateError(string name)
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Name = name;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.Should().Be("Name cannot be blank.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_NameTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Name = Generators.GenRandomString(101);

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.Should().Be("Name must not exceed 100 characters.");
            createdCustomer.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task CreateAsync_NoPhone_ShouldNotCreateAndReturnAppropriateError(string phone)
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Phone = phone;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.Should().Be("Customer requires a phone number.");
            createdCustomer.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("1234567890")]
        [DataRow("123-456-78910")]
        public async Task CreateAsync_PhoneLengthWrong_ShouldNotCreateAndReturnAppropriateError(string phone)
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Phone = phone;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.GetUntilOrEmpty(".").Should().Be("Phone Numbers must have precisely 10 characters.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_PhoneNoDashes_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Phone = "123+123+1234";

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.GetUntilOrEmpty(".").Should().Be("Phone number must be in valid format: XXX-XXX-XXXX.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_PhoneNonDigits_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenCustomer();
            newItem.Phone = "123-123-123a";

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.Should().Be("Phone number may only contain digits.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_EmailTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.Email = Generators.GenRandomString(101);

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.GetUntilOrEmpty(".").Should().Be("Email must not exceed 100 characters.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_EmailWrongFormat_ShouldNotCreateAndReturnAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.Email = Generators.GenRandomString(40).Replace("@", "");

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.Should().Be("Email must be in a valid format (account@provider).");
            createdCustomer.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task CreateAsync_NoAddress_ShouldNotCreateAndReturnAppropriateError(string adr)
        {
            var newItem = Generators.GenCustomer();
            newItem.Address = adr;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.Should().Be("Address cannot be empty.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_AddressTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.Address = Generators.GenRandomString(201);

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.Should().Be("Address may not exceed 200 characters.");
            createdCustomer.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task CreateAsync_NoCity_ShouldNotCreateAndReturnAppropriateError(string city)
        {
            var newItem = Generators.GenCustomer();
            newItem.City = city;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.Should().Be("City field cannot be blank.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_CityTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.City = Generators.GenRandomString(101);

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.Should().Be("City should not exceed 100 characters; If necessary, please use an abbreviation.");
            createdCustomer.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task CreateAsync_NoState_ShouldNotCreateAndReturnAppropriateError(string state)
        {
            var newItem = Generators.GenCustomer();
            newItem.State = state;

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.Should().Be("State/County cannot be blank.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_StateTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            var newItem = Generators.GenCustomer();
            newItem.State = Generators.GenRandomString(51);

            //// Act
            // Get Until Or Empty will fetch the first error, ending int PERIOD [.] 
            // Alternative would've been using a List or Array of String for the errors
            var result = await _service.CreateAsync(newItem, true);
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            result.Should().Be("State/County is limited to 50 characters; Where necessary, please use an abbreviation.");
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        {
            var newItem = Generators.GenCustomer();
            newItem.State = null;
            newItem.City = null;

            //// Act
            var result = await _service.CreateAsync(newItem, true);
            var err1 = result.GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".");
            var blank = result.Replace(err1, "").Replace(err2, "");
            var createdCustomer = await _service.GetAsync(0);

            // Assert
            //result.Should().Be("City field cannot be blank.\nState/County cannot be blank.\n");
            err1.Should().Be("City field cannot be blank.");
            err2.Should().Be("State/County cannot be blank.");
            blank.Should().BeEmpty();
            createdCustomer.Should().BeNull();
        }

        #endregion

        #region Updates

        [TestMethod]
        public async Task UpdateAsync_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var oldItem = new Customer
            {
                CustomerId = target.CustomerId,
                Address = target.Address,
                City = target.City,
                Email = target.Email,
                Name = target.Name,
                Phone = target.Phone,
                State = target.State
            };
            var targetId = target.CustomerId;
            var newItem = new Customer
            {
                CustomerId = targetId,
                Name = "Will Smith",
                Phone = Generators.GenPhoneNumber(),
                Email = null,
                Address = "104 Tropic Point",
                City = "Guerneville",
                State = "California"
            };

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedItem = await _service.GetAsync(targetId);

            // Assert
            result.Should().BeNull();
            updatedItem.Should().BeEquivalentTo(newItem);
            updatedItem.Should().NotBeEquivalentTo(oldItem);
        }

        [TestMethod]
        public async Task UpdateAsync_InexistentId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var targetId = Generators.GenInexistentCustomerId();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = targetId;
            var oldRepo = await _service.GetAllAsync();

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var newRepo = await _service.GetAllAsync();

            // Assert
            result.Should().Be("Customer ID must exist within the Repo for an update to be performed.");
            newRepo.Should().BeEquivalentTo(oldRepo);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task UpdateAsync_NoName_ShouldNotUpdateAndReturnAppropriateError(string name)
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Name = name;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.Should().Be("Name cannot be blank.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_NameTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Name = Generators.GenRandomString(101);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.Should().Be("Name must not exceed 100 characters.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task UpdateAsync_NoPhone_ShouldNotUpdateAndReturnAppropriateError(string phone)
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Phone = phone;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.Should().Be("Customer requires a phone number.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("1234567890")]
        [DataRow("123-456-78910")]
        public async Task UpdateAsync_PhoneLengthWrong_ShouldNotUpdateAndReturnAppropriateError(string phone)
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Phone = phone;

            //// Act
            //Only returns the first error by loooking for the first occurance of [.]. The alternative was to use an Array or List of string.
            //Wasn't too keen on passing a memory intensive data structure between layers just to display 1 or 2 sentences.
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.GetUntilOrEmpty(".").Should().Be("Phone Numbers must have precisely 10 characters.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_PhoneNoDashes_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Phone = "123212311234";

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.Should().Be("Phone number must be in valid format: XXX-XXX-XXXX.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_PhoneNonDigits_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Phone = "123-123-123a";

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.Should().Be("Phone number may only contain digits.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_EmailTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Email = Generators.GenRandomString(101);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.GetUntilOrEmpty(".").Should().Be("Email must not exceed 100 characters.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_EmailWrongFormat_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Email = Generators.GenRandomString(20).Replace("@", "");

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.Should().Be("Email must be in a valid format (account@provider).");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task UpdateAsync_NoAddress_ShouldNotUpdateAndReturnAppropriateError(string adr)
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Address = adr;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.Should().Be("Address cannot be empty.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_AddressTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.Address = Generators.GenRandomString(201);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.Should().Be("Address may not exceed 200 characters.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task UpdateAsync_NoCity_ShouldNotUpdateAndReturnAppropriateError(string city)
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.City = city;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.Should().Be("City field cannot be blank.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_CityTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.City = Generators.GenRandomString(101);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.Should().Be("City should not exceed 100 characters; If necessary, please use an abbreviation.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task UpdateAsync_NoState_ShouldNotUpdateAndReturnAppropriateError(string state)
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.State = state;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.Should().Be("State/County cannot be blank.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_StateTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.State = Generators.GenRandomString(51);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            result.Should().Be("State/County is limited to 50 characters; Where necessary, please use an abbreviation.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = Generators.GenCustomer();
            newItem.CustomerId = target.CustomerId;
            newItem.State = null;
            newItem.City = null;

            //// Act
            var result = await _service.UpdateAsync(newItem, true);
            var err1 = result.GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".");
            var blank = result.Replace(err1, "").Replace(err2, "");
            var updatedCustomer = await _service.GetAsync(target.CustomerId);

            // Assert
            err1.Should().Be("City field cannot be blank.");
            err2.Should().Be("State/County cannot be blank.");
            blank.Should().BeEmpty();
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region Gets

        [TestMethod]
        public async Task GetAll_HappyFlow_ShouldFetchAllAsync()
        {
            var result = await _service.GetAllAsync();

            result.Should().BeEquivalentTo(_db.Customers);
        }

        #region Paginated

        [TestMethod]
        public async Task GetAllPaginatedAsync_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            int pgSize = 1;
            int pgIndex = 3;
            var expected = _db.Customers.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();

            // Act
            var result = await _service.GetAllPaginatedAsync(pgSize, pgIndex);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllPaginatedAsync_FullPage_ShouldReturnRightSize()
        {
            var result = await _service.GetAllPaginatedAsync(2, 1);

            result.Count().Should().Be(2);
        }

        [TestMethod]
        public async Task GetAllPaginatedAsync_PartialPage_ShouldReturnRightSize()
        {
            var result = await _service.GetAllPaginatedAsync(2, 2);

            result.Count().Should().Be(1);
        }

        [TestMethod]
        public async Task GetAllPaginatedAsync_SizeTooBig_ShouldReturnAllAsync()
        {
            // Arrange
            var expected = _db.Customers;
            var oversizedPage = expected.Count() + 10;

            // Act
            var result = await _service.GetAllPaginatedAsync(oversizedPage, 1);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllPaginatedAsync_IndexTooHigh_ShouldReturnEmpty()
        {
            var overIndex = _db.Customers.Count() + 10;

            var result = await _service.GetAllPaginatedAsync(1, overIndex);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetAllPaginatedAsync_SizeZero_ShouldReturnAllAsync()
        {
            var result = await _service.GetAllPaginatedAsync(0, 2);

            result.Should().BeEquivalentTo(_db.Customers);
        }

        [TestMethod]
        public async Task GetAllPaginatedAsync_IndexZero_ShouldReturnAllAsync()
        {
            var result = await _service.GetAllPaginatedAsync(2, 0);

            result.Should().BeEquivalentTo(_db.Customers);
        }

        [DataTestMethod]
        [DataRow(-1, 2)]
        [DataRow(1, -2)]
        public async Task GetAllPaginatedAsync_NegativeNumbers_ShouldConvertToPositive(int pgSize, int pgIndex)
        {
            var expected = await _service.GetAllPaginatedAsync(1, 2);

            var result = await _service.GetAllPaginatedAsync(pgSize, pgIndex);

            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region Filtered

        [TestMethod]
        public async Task GetAllFilteredAsync_NameMatch_ShouldReturnMatch()
        {
            var target = _db.Customers.FirstOrDefault();
            var expected = new List<Customer> { target };

            var result = await _service.GetAllFilteredAsync(target.Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_EmailMatch_ShouldReturnMatch()
        {
            var target = _db.Customers.Where(c => c.Email != null).ToList().FirstOrDefault();
            var expected = new List<Customer> { target };

            var result = await _service.GetAllFilteredAsync(target.Email);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_AddressMatch_ShouldReturnMatch()
        {
            var target = _db.Customers.ToList().FirstOrDefault();
            var expected = new List<Customer> { target };

            var result = await _service.GetAllFilteredAsync(target.Email);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_CityMatch_ShouldReturnMatch()
        {
            var target = _db.Customers.ToList().FirstOrDefault();
            var expected = new List<Customer> { target };

            var result = await _service.GetAllFilteredAsync(target.City);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_StateMatch_ShouldReturnMatch()
        {
            //Need to use LastOrDefault, because first record hails from District Of Columbia, the "of" being a match both on the expected State field from the target record and with the "of" in "Micros[of]t" from the second record's Email field. Could make the Email search require a perfect match, but then you couldn't search Customers based on mail provider"
            var target = _db.Customers.ToList().LastOrDefault();
            var expected = new List<Customer> { target };

            var result = await _service.GetAllFilteredAsync(target.State);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_StateAbbreviationMatch_ShouldReturnMatch()
        {
            // Arrange
            var target = new Customer
            {
                Name = "George Claudius",
                Phone = Generators.GenPhoneNumber(),
                Email = null,
                Address = "15 Pine Road",
                City = "Lost Springs",
                State = "Wyoming"
            };
            await _service.CreateAsync(target);
            var expected = new List<Customer> { target };

            // Act
            var result = await _service.GetAllFilteredAsync("WY");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_InexistentTerm_ShouldReturnEmpty()
        {
            var search = Guid.NewGuid().ToString();

            var result = await _service.GetAllFilteredAsync(search);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_DifTermsForDifResults_ShoudReturnBothMatches()
        {
            // Arrange
            var target1 = _db.Customers.FirstOrDefault();
            var target2 = _db.Customers.Skip(1).FirstOrDefault();
            var search = target1.Name.Split(' ').FirstOrDefault() + " " +
                         target2.Name.Split(' ').FirstOrDefault();
            var expected = new List<Customer> { target1, target2 };

            // Act
            var result = await _service.GetAllFilteredAsync(search);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_HappyFlow_ShouldReturnMatch()
        {
            var expected = new List<Customer> { await _service.GetAsync(2) };

            var result = await _service.GetAllFilteredAndPagedAsync("of", 1, 2);

            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region Sorts

        [DataTestMethod]
        [DataRow("ID")]
        [DataRow("CUSTOMERID")]
        [DataRow("id")]
        [DataRow("customerId")]
        public async Task GetAllSortedAsync_ById_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.CustomerId);

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("NaME")]
        [DataRow("NAME")]
        public async Task GetAllSortedAsync_ByName_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.Name);

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("ADR")]
        [DataRow("ADDRESS")]
        [DataRow("address")]
        [DataRow("adr")]
        public async Task GetAllSortedAsync_ByAddress_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.Address);

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("PHONE")]
        [DataRow("phone")]
        [DataRow("PHONENUMBER")]
        [DataRow("PhoneNumber")]
        [DataRow("NUMBER")]
        [DataRow("NumbER")]
        [DataRow("PHONENO")]
        [DataRow("PhoneNO")]
        [DataRow("PhoneNO")]
        [DataRow("PHONE_NO")]
        [DataRow("phone_no")]
        [DataRow("PHONE_NUMBER")]
        [DataRow("PhONe_NumbEr")]
        [DataRow("TELEPHONE")]
        [DataRow("telephone")]
        public async Task GetAllSortedAsync_ByPhone_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.Phone);

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("EMAIL")]
        [DataRow("email")]
        [DataRow("E-MAIL")]
        [DataRow("e-mail")]
        [DataRow("MAIL")]
        [DataRow("mAiL")]
        public async Task GetAllSortedAsync_ByEmail_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.Email);

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("City")]
        [DataRow("CITY")]
        [DataRow("TOWN")]
        [DataRow("ToWn")]
        public async Task GetAllSortedAsync_ByCity_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.City);

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("STATE")]
        [DataRow("sTaTe")]
        [DataRow("COUNTY")]
        [DataRow("County")]
        public async Task GetAllSortedAsync_ByState_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.State);

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("asdfok")]
        public async Task GetAllSortedAsync_BadQuery_ShouldReturnAllAsync(string sortBy)
        {
            var expected = await _service.GetAllAsync();

            var resultAsc = await _service.GetAllSortedAsync(sortBy);
            var resultDes = await _service.GetAllSortedAsync(sortBy, true);

            resultAsc.Should().BeEquivalentTo(expected);
            resultDes.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedAsync_Descending_ShouldSortDescending()
        {
            // Arrange
            var sortBy = "Name";
            var ascending = await _service.GetAllSortedAsync(sortBy);
            var expected = ascending.Reverse();

            // Act
            var result = await _service.GetAllSortedAsync(sortBy, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedAndPagedAsync_ShouldReturnRequestedAsync()
        {
            // Arrange
            var sortBy = "Name";
            var pgIndex = 3;
            var pgSize = 1;

            var expected = new List<Customer> { _db.Customers.SingleOrDefault(c => c.CustomerId == 1) };

            // Act
            var result = await _service.GetAllSortedAndPagedAsync(pgSize, pgIndex, sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedDescendingAndPagedAsync_ShouldReturnRequestedAsync()
        {
            // Arrange
            var sortBy = "Name";
            var pgIndex = 3;
            var pgSize = 1;

            var expected = new List<Customer> { _db.Customers.SingleOrDefault(c => c.CustomerId == 3) };

            // Act
            var result = await _service.GetAllSortedAndPagedAsync(pgSize, pgIndex, sortBy, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            //// Arrange
            // Should return first 2 entries by State: District OF Colombia and Email: [...]@microsOFt.com respectively
            var searchBy = "of";
            // Should return the 2-item list in reverse order
            var sortBy = "email";

            var expected = _db.Customers.Take(2).Reverse();

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(searchBy, sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            //// Arrange
            // Should return first 2 entries by State: District OF Colombia and Email: [...]@microsOFt.com respectively
            var searchBy = "of";
            // Should return the 2-item list in reverse order
            var sortBy = "email";

            var expected = _db.Customers.Take(2);

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(searchBy, sortBy, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpCustomers();

            var expected = _db.Customers.Skip(4);

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync("CA", 2, 2, "name");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpCustomers();

            var repo = new TestCustomerRepo(_db);
            var expected = _db.Customers.Skip(2).Take(2);

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync("CA", 2, 2, "name", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }


        #endregion

        #endregion

        #region MainWindow Methods

        [TestMethod]
        public async Task OneCustWithEmailExistsAsync_TheyDo_ShouldReturnTrue()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();

            // Act
            var result = await _service.OneCustWithEmailExistsAsync(target.Email);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task OneCustWithEmailExistsAsync_TheyDont_ShouldReturnFalse()
        {
            // Arrange
            var email = string.Empty;
            while (true)
            {
                email = Generators.GenRandomString(20) + "@gmail.com";
                if (!_db.Customers.Any(c => c.Email == email)) break;
            }

            // Act
            var result = await _service.OneCustWithEmailExistsAsync(email);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task OneCustWithEmailExistsAsync_Duplicate_ShouldReturnFalse()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = new Customer
            {
                CustomerId = Generators.GenInexistentCustomerId(),
                Name = "William Wallace",
                Phone = Generators.GenPhoneNumber(),
                Address = "013 Nobody Owens Drive",
                State = target.State,
                City = target.City,
                Email = target.Email
            };
            await _service.CreateAsync(newItem);


            // Act
            var result = await _service.OneCustWithEmailExistsAsync(target.Email);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task FindIdByEmailAsync_HappyFlow_ShouldReturnRightId()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();

            // Act
            var result = await _service.FindIdByEmailAsync(target.Email);

            // Assert
            result.Should().Be(target.CustomerId);
        }

        [TestMethod]
        public async Task FindIdByEmailAsync_InexistentEmail_ShouldReturnNull()
        {
            // Arrange
            var email = string.Empty;
            while (true)
            {
                email = Generators.GenRandomString(20) + "@gmail.com";
                if (!_db.Customers.Any(c => c.Email == email)) break;
            }

            // Act
            var result = await _service.FindIdByEmailAsync(email);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task FindIdByEmailAsync_Duplicate_ShouldReturnNull()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = new Customer
            {
                CustomerId = Generators.GenInexistentCustomerId(),
                Name = "William Wallace",
                Phone = Generators.GenPhoneNumber(),
                Address = "013 Nobody Owens Drive",
                State = target.State,
                City = target.City,
                Email = target.Email
            };
            await _service.CreateAsync(newItem);


            // Act
            var result = await _service.FindIdByEmailAsync(target.Email);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task PassMatchesEmailAsync_ItDoes_ShouldReturnTrue()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();

            // Act
            var result = await _service.PassMatchesEmailAsync(target.Email, target.Phone.Substring(0, 3));

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task PassMatchesEmailAsync_ItDoesNot_ShouldReturnFalse()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var truePass = target.Phone.Substring(0, 3);
            var pass = string.Empty;
            while (true)
            {
                pass = Generators.GenRandomString(3);
                if (pass != truePass) break;
            }

            // Act
            var result = await _service.PassMatchesEmailAsync(target.Email, pass);

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("schaman0@hexun.com", null)]
        [DataRow(null, "202")]
        public async Task PassMatchesEmailAsync_NullValue_ShouldReturnFalse(string email, string pass)
        {
            var result = await _service.PassMatchesEmailAsync(email, pass);

            result.Should().BeFalse();
        }

        #endregion

        #endregion
    }
}
