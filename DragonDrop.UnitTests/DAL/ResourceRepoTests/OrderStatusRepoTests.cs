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
    public class OrderStatusRepoTests
    {
        private TestOrderStatusRepo _repo;
        private TestDb _db;

        public OrderStatusRepoTests()
        {
            _db = new TestDb();
            _repo = new TestOrderStatusRepo(_db);
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldReturnDesired()
        {
            // Arrange
            var target = _db.OrderStatuses.FirstOrDefault();

            // Act
            var result = _repo.Get(target.OrderStatusId);

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
                badId = new Random().Next(_db.OrderStatuses.Count() + 2, int.MaxValue);
                if (!_db.OrderStatuses.Any(m => m.OrderStatusId == badId)) break;
            }

            // Act
            var result = _repo.Get(badId);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetStatusName_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var target = _db.OrderStatuses.FirstOrDefault();

            // Act
            var result = _repo.GetStatusName(target.OrderStatusId);

            // Assert
            result.Should().BeEquivalentTo(target.Name);
        }

        [TestMethod]
        public void GetStatusName_InexistentId_ShouldReturnNull()
        {
            // Arrange
            var badId = 0;
            while (true)
            {
                badId = new Random().Next(_db.OrderStatuses.Count() + 2, int.MaxValue);
                if (!_db.OrderStatuses.Any(m => m.OrderStatusId == badId)) break;
            }

            // Act
            var result = _repo.GetStatusName(badId);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetStatusId_HappyFlow_ShouldReturnDesired()
        {
            // Arrange
            var target = _db.OrderStatuses.FirstOrDefault();

            // Act
            var result = _repo.GetStatusId(target.Name);

            // Assert
            result.Should().Be(target.OrderStatusId);
        }

        [TestMethod]
        public void GetStatusId_InexistentName_ShouldReturnNull()
        {
            // Arrange
            var badName = string.Empty;
            while (true)
            {
                badName = Generators.GenRandomString(20);
                if (!_db.OrderStatuses.Any(m => m.Name == badName)) break;
            }

            // Act
            var result = _repo.GetStatusId(badName);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.OrderStatuses.FirstOrDefault();
            var ogTarget = new OrderStatus
                {
                    OrderStatusId = target.OrderStatusId,
                    Name = target.Name
                };
            var newName = Generators.GenRandomString(20);

            // Act
            _repo.Update(new OrderStatus { OrderStatusId = target.OrderStatusId, Name = newName});
            var result = _repo.Get(target.OrderStatusId);

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
                badId = new Random().Next(_db.OrderStatuses.Count() + 1, int.MaxValue);
                break;
            }

            // Act
            _repo.Update(new OrderStatus { OrderStatusId = badId, Name = "FindMe" });
            var result = _repo.GetStatusId("FindMe");

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void Add_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItemName = "New Status";
            var initCount = _db.OrderStatuses.Count();

            // Act
            _repo.Add(newItemName);
            var newCount = _db.OrderStatuses.Count();

            // Assert
            newCount.Should().Be(initCount + 1);
            _repo.GetStatusId(newItemName).Should().NotBeNull();
        }
    }
}
