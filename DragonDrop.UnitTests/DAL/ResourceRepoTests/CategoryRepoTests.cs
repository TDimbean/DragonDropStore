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
    public class CategoryRepoTests
    {
        private TestCategoryRepo _repo;
        private TestDb _db;

        public CategoryRepoTests()
        {
            _db = new TestDb();
            _repo = new TestCategoryRepo(_db);
        }

        [TestMethod]
        public void GetAll_HappyFlow_ShouldReturnAll()
        {
            var expected = _db.Categories;

            var result = _repo.GetAll();

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldReturnDesired()
        {
            // Arrange
            var target = _db.Categories.FirstOrDefault();

            // Act
            var result = _repo.Get(target.CategoryId);

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
                badId = new Random().Next(_db.Categories.Count() + 2, int.MaxValue);
                if (!_db.Categories.Any(m => m.CategoryId == badId)) break;
            }

            // Act
            var result = _repo.Get(badId);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetCategoryName_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var target = _db.Categories.FirstOrDefault();

            // Act
            var result = _repo.GetCategoryName(target.CategoryId);

            // Assert
            result.Should().BeEquivalentTo(target.Name);
        }

        [TestMethod]
        public void GetCategoryName_InexistentId_ShouldReturnNull()
        {
            // Arrange
            var badId = 0;
            while (true)
            {
                badId = new Random().Next(_db.Categories.Count() + 2, int.MaxValue);
                if (!_db.Categories.Any(m => m.CategoryId == badId)) break;
            }

            // Act
            var result = _repo.GetCategoryName(badId);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetCategoryId_HappyFlow_ShouldReturnDesired()
        {
            // Arrange
            var target = _db.Categories.FirstOrDefault();

            // Act
            var result = _repo.GetCategoryId(target.Name);

            // Assert
            result.Should().Be(target.CategoryId);
        }

        [TestMethod]
        public void GetCategoryId_InexistentName_ShouldReturnNull()
        {
            // Arrange
            var badName = string.Empty;
            while (true)
            {
                badName = Generators.GenRandomString(20);
                if (!_db.Categories.Any(m => m.Name == badName)) break;
            }

            // Act
            var result = _repo.GetCategoryId(badName);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.Categories.FirstOrDefault();
            var ogTarget = new Category
                {
                    CategoryId = target.CategoryId,
                    Name = target.Name
                };
            var newName = Generators.GenRandomString(20);

            // Act
            _repo.Update(new Category { CategoryId = target.CategoryId, Name = newName});
            var result = _repo.Get(target.CategoryId);

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
                badId = new Random().Next(_db.Categories.Count() + 1, int.MaxValue);
                break;
            }

            // Act
            _repo.Update(new Category { CategoryId = badId, Name = "FindMe" });
            var result = _repo.GetCategoryId("FindMe");

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void Add_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItemName = "New Status";
            var initCount = _db.Categories.Count();

            // Act
            _repo.Add(newItemName);
            var newCount = _db.Categories.Count();

            // Assert
            newCount.Should().Be(initCount + 1);
            _repo.GetCategoryId(newItemName).Should().NotBeNull();
        }
    }
}
