using DragonDrop.DAL.Entities;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DragonDrop.UnitTests.DAL.ResourceRepoTests
{
    [TestClass]
    public class PaymentMethodRepoTests
    {
        private TestPaymentMethodRepo _repo;
        private TestDb _db;

        public PaymentMethodRepoTests()
        {
            _db = new TestDb();
            _repo = new TestPaymentMethodRepo(_db);
        }

        [TestMethod]
        public void GetAll_HappyFlow_ShouldReturnAll()
        {
            var expected = _db.PaymentMethods.ToList();

            var result = _repo.GetAll();

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldReturnDesired()
        {
            // Arrange
            var target = _db.PaymentMethods.FirstOrDefault();

            // Act
            var result = _repo.Get(target.PaymentMethodId);

            // Assert
            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNull()
        {
            // Arrange
            var badId = 0;
            while (true)
            {
                badId = new Random().Next(_db.PaymentMethods.Count() + 1, int.MaxValue);
                if (!_db.PaymentMethods.Any(m => m.PaymentMethodId == badId)) break;
            }

            // Act
            var result = _repo.Get(badId);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetMethodName_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var target = _db.PaymentMethods.FirstOrDefault();

            // Act
            var result = _repo.GetMethodName(target.PaymentMethodId);

            // Assert
            result.Should().BeEquivalentTo(target.Name);
        }

        [TestMethod]
        public void GetMethodName_InexistentId_ShouldReturnNull()
        {
            // Arrange
            var badId = 0;
            while (true)
            {
                badId = new Random().Next(_db.PaymentMethods.Count() + 1, int.MaxValue);
                if (!_db.PaymentMethods.Any(m => m.PaymentMethodId == badId)) break;
            }

            // Act
            var result = _repo.GetMethodName(badId);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetMethodId_HappyFlow_ShouldReturnDesired()
        {
            // Arrange
            var target = _db.PaymentMethods.FirstOrDefault();

            // Act
            var result = _repo.GetMethodId(target.Name);

            // Assert
            result.Should().Be(target.PaymentMethodId);
        }

        [TestMethod]
        public void GetMethodId_InexistentName_ShouldReturnNull()
        {
            // Arrange
            var badName = string.Empty;
            while (true)
            {
                badName = Generators.GenRandomString(20);
                if (!_db.PaymentMethods.Any(m => m.Name == badName)) break;
            }

            // Act
            var result = _repo.GetMethodId(badName);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.PaymentMethods.FirstOrDefault();
            var ogTarget = new PaymentMethod
                {
                    PaymentMethodId = target.PaymentMethodId,
                    Name = target.Name
                };
            var newName = Generators.GenRandomString(20);

            // Act
            _repo.Update(new PaymentMethod { PaymentMethodId = target.PaymentMethodId, Name = newName});
            var result = _repo.Get(target.PaymentMethodId);

            // Assert
            result.Should().NotBeEquivalentTo(ogTarget);
            result.Name.Should().Be(newName);
        }

        [TestMethod]
        public void Update_InexistentId_ShouldNotUpdate()
        {
            // Arrange
            var badId = 0;
            while (true)
            {
                badId = new Random().Next(_db.PaymentMethods.Count() + 1, int.MaxValue);
                break;
            }

            // Act
            _repo.Update(new PaymentMethod { PaymentMethodId = badId, Name = "FindMe" });
            var result = _repo.GetMethodId("FindMe");

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void Add_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItemName = "New Method";
            var initCount = _db.PaymentMethods.Count();

            // Act
            _repo.Add(newItemName);
            var newCount = _db.PaymentMethods.Count();

            // Assert
            newCount.Should().Be(initCount + 1);
            _repo.GetMethodId(newItemName).Should().NotBeNull();
        }
    }
}
