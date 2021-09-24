using DragonDrop.DAL.Entities;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonDrop.UnitTests.DAL
{
    [TestClass]
    public class CustomerRepoTests
    {
        private TestCustomerRepo _repo;
        private TestDb _db;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestCustomerRepo(_db);
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldRetrieveMatch()
        {
            var expected = _db.Customers.FirstOrDefault();

            var result = _repo.Get(expected.CustomerId);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNull()
        {
            var inexistentId = Generators.GenInexistentCustomerId();

            var result = _repo.Get(inexistentId);

            result.Should().BeNull();
        }

        [TestMethod]
        public void Create_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = new Customer
            {
                Name = "Will Smith",
                Address = "12 Joester Avenue",
                Phone = Generators.GenPhoneNumber(),
                Email = "freshP.contact@gmail.com",
                City = "Los Angeles",
                State = "California"
            };
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Create(newItem);
            var result = _repo.GetAll();
            var newCount = result.Count();

            // Assert
            result.Should().ContainEquivalentOf(newItem);
            newCount.Should().Be(initCount + 1);
        }

        [TestMethod]
        public void Create_IncompleteRecord_ShouldNotCreate()
        {
            // Arrange
            var unmAdr = Generators.GenUnmistakableAddress();
            var newItem = new Customer
            {
                Name = null,
                Address = unmAdr,
                Phone = Generators.GenPhoneNumber(),
                Email = "freshP.contact@gmail.com",
                City = "Los Angeles",
                State = "California"
            };
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Create(newItem);
            var createdItem = _repo.GetAll().SingleOrDefault(c => c.Address == unmAdr);
            var result = _repo.GetAll();
            var newCount = result.Count();

            // Assert
            newCount.Should().Be(initCount);
            createdItem.Should().BeNull();
        }

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var item = _db.Customers.FirstOrDefault();
            var oldItem = new Customer
            {
                CustomerId = item.CustomerId,
                Name = item.Name,
                Phone = item.Phone,
                Email = item.Email,
                Address = item.Address,
                City = item.City,
                State = item.State
            };
            var updItem = new Customer
            {
                CustomerId = oldItem.CustomerId,
                Name = "George Newman",
                Phone = Generators.GenPhoneNumber(),
                Email = "gnewman.17@gmail.com",
                Address = "14 Uptown Road",
                City = "New York City",
                State = "New York"
            };
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Update(updItem);
            var result = _repo.Get(oldItem.CustomerId);
            var newRepo = _repo.GetAll();
            var newCount = newRepo.Count();

            // Assert
            newCount.Should().Be(initCount);
            newRepo.Should().ContainEquivalentOf(updItem);
            result.Should().BeEquivalentTo(updItem);
            result.Should().NotBeEquivalentTo(oldItem); 
        }

        [TestMethod]
        public void Update_InexistentId_ShouldNotUpdate()
        {
            // Arrange
            var item = _db.Customers.FirstOrDefault();
            var oldItem = new Customer
            {
                CustomerId = item.CustomerId,
                Name = item.Name,
                Phone = item.Phone,
                Email = item.Email,
                Address = item.Address,
                City = item.City,
                State = item.State
            };
            var updItem = new Customer
            {
                CustomerId = Generators.GenInexistentCustomerId(),
                Name = "George Newman",
                Phone = Generators.GenPhoneNumber(),
                Email = "gnewman.17@gmail.com",
                Address = "14 Uptown Road",
                City = "New York City",
                State = "New York"
            };
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Update(updItem);
            var result = _repo.Get(oldItem.CustomerId);
            var newRepo = _repo.GetAll();
            var newCount = newRepo.Count();

            // Assert
            newCount.Should().Be(initCount);
            newRepo.Should().ContainEquivalentOf(oldItem);
            result.Should().BeEquivalentTo(oldItem);
            result.Should().NotBeEquivalentTo(updItem);
        }

        #region Gets

        [TestMethod]
        public void GetAll_HappyFlow_ShouldRetrieveAllRecords()
        {
            var result = _repo.GetAll();

            result.Should().BeEquivalentTo(_db.Customers.ToList());
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
            var result = _repo.GetAllPaginated(pgSize, pgIndex);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_FullPage_ShouldReturnRightSize()
        {
            var result = _repo.GetAllPaginated(2, 1);

            result.Count().Should().Be(2);
        }

        [TestMethod]
        public void GetAllPaginated_PartialPage_ShouldReturnRightSize()
        {
            var result = _repo.GetAllPaginated(2, 2);

            result.Count().Should().Be(1);
        }

        [TestMethod]
        public void GetAllPaginated_SizeTooBig_ShouldReturnAll()
        {
            var expected = _repo.GetAll();
            var oversizedPage = expected.Count() + 10;

            var result = _repo.GetAllPaginated(oversizedPage, 1);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_IndexTooHigh_ShouldReturnEmpty()
        {
            var overIndex = _repo.GetAll().Count() + 10;

            var result = _repo.GetAllPaginated(1, overIndex);

            result.Should().BeEmpty();
        }

        #endregion

        #region Filters

        [TestMethod]
        public void GetAllFiltered_NameMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Customers.FirstOrDefault();
            var expected = new List<Customer> { targetItem };

            var result = _repo.GetAllFiltered(targetItem.Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_EmailMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Customers.Where(c=>c.Email!=null).ToList().FirstOrDefault();
            var expected = new List<Customer> { targetItem };

            var result = _repo.GetAllFiltered(targetItem.Email);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_AddressMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Customers.ToList().FirstOrDefault();
            var expected = new List<Customer> { targetItem };

            var result = _repo.GetAllFiltered(targetItem.Email);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_CityMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Customers.ToList().FirstOrDefault();
            var expected = new List<Customer> { targetItem};

            var result = _repo.GetAllFiltered(targetItem.City);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_StateMatch_ShouldReturnMatch()
        {
            //Need to use LastOrDefault, because first record hails from District Of Columbia, the "of" being a match both on the expected State field from the target record and with the "of" in "Micros[of]t" from the second record's Email field. Could make the Email search require a perfect match, but then you couldn't search Customers based on mail provider"
            var targetItem = _db.Customers.ToList().LastOrDefault();
            var expected = new List<Customer> { targetItem };

            var result = _repo.GetAllFiltered(targetItem.State);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_StateAbbreviationMatch_ShouldReturnMatch()
        {
            // Arrange
            var targetItem = new Customer
            {
                Name = "George Claudius",
                Phone = Generators.GenPhoneNumber(),
                Email = null,
                Address = "15 Pine Road",
                City = "Lost Springs",
                State = "Wyoming"
            };
            _repo.Create(targetItem);
            var expected = new List<Customer> { targetItem };

            // Act
            var result = _repo.GetAllFiltered("WY");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_InexistentTerm_ShouldReturnEmpty()
        {
            var search = Guid.NewGuid().ToString();

            var result = _repo.GetAllFiltered(search);

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
            var result = _repo.GetAllFiltered(search);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_HappyFlow_ShouldReturnMatch()
        {
            var expected = new List<Customer> { _repo.Get(2) };

            var result = _repo.GetAllFilteredAndPaged("of", 1, 2);

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

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("NaME")]
        [DataRow("NAME")]
        public void GetAllSorted_ByName_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Customers.OrderBy(c => c.Name);

            var result = _repo.GetAllSorted(sortBy);

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

            var result = _repo.GetAllSorted(sortBy);

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

            var result = _repo.GetAllSorted(sortBy);

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

            var result = _repo.GetAllSorted(sortBy);

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

            var result = _repo.GetAllSorted(sortBy);

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

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("asdfok")]
        public void GetAllSorted_BadQuery_ShouldReturnAll(string sortBy)
        {
            var expected = _repo.GetAll();

            var resultAsc = _repo.GetAllSorted(sortBy);
            var resultDes = _repo.GetAllSorted(sortBy, true);

            resultAsc.Should().BeEquivalentTo(expected);
            resultDes.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSorted_Descending_ShouldSortDescending()
        {
            // Arrange
            var sortBy = "Name";
            var ascending = _repo.GetAllSorted(sortBy);
            var expected = ascending.Reverse();

            // Act
            var result = _repo.GetAllSorted(sortBy, true);

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
            var result = _repo.GetAllSortedAndPaged(pgSize, pgIndex, sortBy);

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
            var result = _repo.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, true);

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
            var result = _repo.GetAllFilteredAndSorted(searchBy, sortBy);

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
            var result = _repo.GetAllFilteredAndSorted(searchBy, sortBy, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpCustomers();

            var repo = new TestCustomerRepo(_db);
            var expected = _db.Customers.Skip(4);

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged("CA", 2, 2, "name");

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
            var result = _repo.GetAllFilteredSortedAndPaged("CA", 2, 2, "name", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion Sorts

        #endregion

        [TestMethod]
        public void CustomerIdExists_ItDoes_ShouldReturnTrue()
        {
            var id = _db.Customers.FirstOrDefault().CustomerId;

            var result = _repo.CustomerIdExists(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CustomerIdExists_ItDoesnt_ShouldReturnFalse()
        {
            var id = Generators.GenInexistentCustomerId();

            var result = _repo.CustomerIdExists(id);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void EmailExists_ItDoes_ShouldReturnTrue()
        {
            var email = _db.Customers.FirstOrDefault().Email;

            var result = _repo.EmailExists(email);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void EmailExists_ItDoesnt_ShouldReturnFalse()
        {
            var email = Generators.GenEmailAddress();

            var result = _repo.EmailExists(email);

            result.Should().BeFalse();
        }

        #region Main Window Methods

        [TestMethod]
        public void FindIdByEmail_HappyFlow_ShouldReturnExpected()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var expected = target.CustomerId;

            // Act
            var result = _repo.FindIdByEmail(target.Email);

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void FindIdByEmail_InexistentEmail_ShouldReturnNull()
        {
            // Arrange
            var email = string.Empty;
            while (true)
            {
                email = Generators.GenRandomString(30);
                if (!_db.Customers.Any(c => c.Email != null && c.Email == email)) break;
            }

            // Act
            var result = _repo.FindIdByEmail(email);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void FindIdByEmail_DuplicateEmail_ShouldReturnNull()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var newItem = new Customer
            {
                CustomerId = target.CustomerId + 10,
                Name = "Reggie Ulric",
                State = target.State,
                City = target.City,
                Phone = Generators.GenPhoneNumber(),
                Address = "012 Lovellace Avenue",
                Email = target.Email
            };
            _repo.Create(newItem);

            // Act
            var result = _repo.FindIdByEmail(target.Email);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetPassByCustomerId_HappyFlow_ShouldReturnPasswordFromPhone()
        {
            // Arrange
            var target = _db.Customers.FirstOrDefault();
            var expected = _db.Customers.FirstOrDefault().Phone.Substring(0, 3);

            // Act
            var result = _repo.GetPassByCustomerId(target.CustomerId);

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void GetPassByCustomerId_InexistentId_ShouldReturnNull()
        {
            // Arrange
            var custId = Generators.GenInexistentCustomerId();

            // Act
            var result = _repo.GetPassByCustomerId(custId);

            // Assert
            result.Should().BeNull();
        }

        #endregion
    }
}
