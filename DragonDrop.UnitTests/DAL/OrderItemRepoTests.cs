using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure;
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
    public class OrderItemRepoTests
    {
        private TestOrderItemRepo _repo;
        private TestDb _db;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestOrderItemRepo(_db);
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldRetrieveMatch()
        {
            var target = _db.OrderItems.FirstOrDefault();

            var result = _repo.Get(target.OrderId, target.ProductId);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public void Get_InexistentOrderId_ShouldReturnNull()
        {
            var inOrdId = Generators.GenInexistentOrderId();

            var result = _repo.Get(inOrdId, 1);

            result.Should().BeNull();
        }

        [TestMethod]
        public void Get_InexistentProductId_ShouldReturnNull()
        {
            var inProdId = Generators.GenInexistentProductId();

            var result = _repo.Get(1, inProdId);

            result.Should().BeNull();
        }

        [TestMethod]
        public void Create_HappyFlow_SholdRegisterRecord()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            if (newItem == null)
            {
                StaticLogger.LogWarn(GetType(), "Every single Order-Product permutation is already registered as an OrderItem. This is highly unlikely, yet it either is the case or the RNG has generated 1000 already existing combinations in arow. Please create an alternative test that adds a new Item or Product to the Repo first, then generates an OrderItem based on it. Just to be clear, everything is working properly, nothing broke down, the OrderItem records have simply used up every available combination and need to be fed fresh Products and/or Orders to make room.");
                return;
            }
            var initCount = _db.Products.Count();

            // Act
            _repo.Create(newItem);
            var addedItem = _repo.Get(newItem.OrderId, newItem.ProductId);
            var newRepo = _repo.GetAll();
            var newCount = newRepo.Count();

            // Assert
            newCount.Should().Be(initCount + 1);
            newRepo.Should().ContainEquivalentOf(newItem);
            addedItem.Should().BeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Create_ExistingCombination_ShouldNotCreate()
        {
            // Arrange
            var existingItem = _db.OrderItems.FirstOrDefault();
            var newItem = Generators.GenOrderItem();
            newItem.OrderId = existingItem.OrderId;
            newItem.ProductId = existingItem.ProductId;
            var initCount = _db.Products.Count();
            var oldRepo = _repo.GetAll();

            // Act
            _repo.Create(newItem);
            var addedItem = _repo.Get(newItem.OrderId, newItem.ProductId);
            var newRepo = _repo.GetAll();
            var newCount = newRepo.Count();

            // Assert
            newCount.Should().Be(initCount);
            newRepo.Should().NotContain(newItem);
            newRepo.Should().BeEquivalentTo(oldRepo);
            addedItem.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow(null,1,1)]
        [DataRow(1, null, 1)]
        [DataRow(1,1,null)]
        public void Create_NullValues_ShouldNotCreate(int ordId, int prodId, int qty)
        {
            // Arrange
            var newItem = new OrderItem { OrderId = ordId, ProductId = prodId, Quantity = qty };
            var initRepo = _repo.GetAll();
            var initCount = initRepo.Count();

            // Act
            _repo.Create(newItem);
            var newRepo = _repo.GetAll();
            var newCount = newRepo.Count();

            // Assert
            newRepo.Should().BeEquivalentTo(initRepo);
            newCount.Should().Be(initCount);
        }

        #region Updates

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            //// Arrange
            var targetItem = _db.OrderItems.FirstOrDefault();

            //Find a new Quantity
            var qty = 0;
            var rnd = new Random();
            var same = true;
            while(same)
            {
                qty = rnd.Next(1, _db.Products.SingleOrDefault(p => p.ProductId == targetItem.ProductId).Stock + 1);
                if (qty != targetItem.Quantity) same = false;
            }

            //Form the Update Object
            var newItem = new OrderItem
            {
                OrderId = targetItem.OrderId,
                ProductId = targetItem.ProductId,
                Quantity = qty
            };

            //// Act
            _repo.Update(newItem);
            var allItems = _repo.GetAll();
            var fetchedItem = _repo.Get(targetItem.OrderId, targetItem.ProductId);

            // Assert
            allItems.Should().ContainEquivalentOf(newItem);
            fetchedItem.Quantity.Should().Be(newItem.Quantity);
        }

        [TestMethod]
        public void Update_BadQuantity_ShouldNotUpdate()
        {
            // Arrange
            var targetItem = _db.OrderItems.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = targetItem.OrderId,
                ProductId = targetItem.ProductId,
                Quantity = -3
            };

            // Act
            _repo.Update(newItem);
            var fetchedItem = _repo.Get(targetItem.OrderId, targetItem.ProductId);

            // Assert
            fetchedItem.Quantity.Should().NotBe(newItem.Quantity);
            fetchedItem.Should().BeEquivalentTo(targetItem);
        }

        [TestMethod]
        public void Update_InexistentOrderId_ShouldNotUpdate()
        {
            // Arrange
            var updItem = new OrderItem
            {
                OrderId = Generators.GenInexistentOrderId(),
                ProductId = _db.Products.FirstOrDefault().ProductId,
                Quantity = 1
            };
            var initCount = _db.OrderItems.Count();

            // Act
            _repo.Update(updItem);
            var result = _repo.Get(updItem.OrderId, updItem.ProductId);
            var newCount = _repo.GetAll().Count();

            // Assert
            newCount.Should().Be(initCount);
            result.Should().BeNull();
        }

        [TestMethod]
        public void Update_InexistentProductId_ShouldNotUpdate()
        {
            // Arrange
            var updItem = new OrderItem
            {
                OrderId = _db.Orders.FirstOrDefault().OrderId,
                ProductId = Generators.GenInexistentProductId(),
                Quantity = 1
            };
            var initCount = _db.OrderItems.Count();

            // Act
            _repo.Update(updItem);
            var result = _repo.Get(updItem.OrderId, updItem.ProductId);
            var newCount = _repo.GetAll().Count();

            // Assert
            newCount.Should().Be(initCount);
            result.Should().BeNull();
        }

        #endregion

        #region Gets

        [TestMethod]
        public void GetAll_HappyFlow_ShouldFetchAll()
        {
            var expected = _db.OrderItems.ToList();

            var result = _repo.GetAll();

            result.Should().BeEquivalentTo(expected);
        }

        #region Paginated

        [TestMethod]
        public void GetAllPaginated_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            int pgSize = 1;
            int pgIndex = 3;
            var expected = _db.OrderItems.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();

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

        #region ByIds

        [TestMethod]
        public void GetAllByOrder_HappyFlow_ShouldReturnMatches()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            _repo.Create(newItem);

            // Act
            var result = _repo.GetAllByOrderId(newItem.OrderId);

            // Assert
            result.Count().Should().Be(2);
            result.Should().ContainEquivalentOf(newItem);
        }

        [TestMethod]
        public void GetAllByOrder_InexistentId_ShouldReturnEmpty()
        {
            var ordId = Generators.GenInexistentOrderId();

            var result = _repo.GetAllByOrderId(ordId);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllByProduct_HappyFlow_ShouldReturnMatches()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            _repo.Create(newItem);

            // Act
            var result = _repo.GetAllByProductId(newItem.ProductId);

            // Assert
            result.Count().Should().Be(2);
            result.Should().ContainEquivalentOf(newItem);
        }

        [TestMethod]
        public void GetAllByProduct_InexistentId_ShouldReturnMatches()
        {
            var prodId = Generators.GenInexistentProductId();

            var result = _repo.GetAllByProductId(prodId);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllByOrderAndPaged_HappyFlow_ShouldReturnMatch()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            _repo.Create(newItem);
            var expected = new List<OrderItem> { newItem };

            // Act
            var result = _repo.GetAllByOrderIdAndPaged(newItem.OrderId, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByProductAndPaged_HappyFlow_ShouldReturnMatch()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            _repo.Create(newItem);
            var expected = new List<OrderItem> { newItem };

            // Act
            var result = _repo.GetAllByProductIdAndPaged(newItem.ProductId, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region Sorts

        [DataTestMethod]
        [DataRow("ORDER")]
        [DataRow("orDeR")]
        [DataRow("ORD")]
        [DataRow("ord")]
        [DataRow("ORDERID")]
        [DataRow("orderId")]
        [DataRow("ORDID")]
        [DataRow("ordId")]
        [DataRow("ORDER_ID")]
        [DataRow("OrDeR_id")]
        [DataRow("ORD_ID")]
        [DataRow("Ord_id")]
        public void GetAllSorted_ByOrderId_ShouldReturnRequested(string sortBy)
        {
            var expected = _db.OrderItems;

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("PRODUCT")]
        [DataRow("prOdUct")]
        [DataRow("PROD")]
        [DataRow("proD")]
        [DataRow("PRODUCTID")]
        [DataRow("ProductId")]
        [DataRow("PRODID")]
        [DataRow("prodId")]
        [DataRow("PRODUCT_ID")]
        [DataRow("prOduCt_id")]
        [DataRow("PROD_ID")]
        [DataRow("prOd_id")]
        public void GetAllSorted_ByProductId_ShouldReturnRequested(string sortBy)
        {
            // Arrange
            var expected = _db.OrderItems;
            expected.Reverse();

            // Act
            var result = _repo.GetAllSorted(sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("QUANTITY")]
        [DataRow("quantITy")]
        [DataRow("QTY")]
        [DataRow("qty")]
        public void GetAllSorted_ByQuantity_ShouldReturnRequested(string sortBy)
        {
            // Arrange
            var expected = _db.OrderItems.OrderBy(i => i.Quantity);

            // Act
            var result = _repo.GetAllSorted(sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("asdf")]
        [DataRow(null)]
        public void GetAllSorted_BadQuery_ShouldReturnAll(string sortBy)
        {
            // Arrange
            var expected = _db.OrderItems;

            // Act
            var result = _repo.GetAllSorted(sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAlllSorted_Descending_ShoudReturnRequested()
        {
            var expected = _db.OrderItems;

            var result = _repo.GetAllSorted("prodId", true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedAndPaged_HappyFlow_ShoudReturnRequested()
        {
            // Arrange
            var expected = new List<OrderItem> { _db.OrderItems.LastOrDefault() };

            // Act
            var result = _repo.GetAllSortedAndPaged(2, 2, "qty");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescendingAndPaged_HappyFlow_ShoudReturnRequested()
        {
            // Arrange
            var expected = new List<OrderItem> { _db.OrderItems.Skip(2).FirstOrDefault() };

            // Act
            var result = _repo.GetAllSortedAndPaged(2, 2, "prodId", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByOrderIdAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem>
                {
                    _db.OrderItems.Skip(4).FirstOrDefault(),
                    _db.OrderItems.Skip(1).FirstOrDefault()
                };

            // Act
            var result = _repo.GetAllByOrderIdAndSorted(2, "prodId");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByOrderIdAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem>
                {
                    _db.OrderItems.Skip(4).FirstOrDefault(),
                    _db.OrderItems.Skip(1).FirstOrDefault()
                };

            // Act
            var result = _repo.GetAllByOrderIdAndSorted(2, "qty", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByProductIdAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem>
                {
                    _db.OrderItems.Skip(1).FirstOrDefault(),
                    _db.OrderItems.Skip(3).FirstOrDefault(),
                    _db.OrderItems.Skip(5).FirstOrDefault()
                };

            // Act
            var result = _repo.GetAllByProductIdAndSorted(2, "qty");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByProductIdAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem>
                {
                    _db.OrderItems.Skip(2).FirstOrDefault(),
                    _db.OrderItems.Skip(4).FirstOrDefault()
                };

            // Act
            var result = _repo.GetAllByProductIdAndSorted(1, "ordId", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByOrderIdSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem> { _db.OrderItems.Skip(4).FirstOrDefault() };

            // Act
            var result = _repo.GetAllByOrderIdSortedAndPaged(2, 1, 2, "qty");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByOrderSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem> { _db.OrderItems.Skip(2).FirstOrDefault()};

            // Act
            var result = _repo.GetAllByOrderIdSortedAndPaged(3, 1, 2, "qty", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByProductIdSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem> { _db.OrderItems.LastOrDefault() };

            // Act
            var result = _repo.GetAllByProductIdSortedAndPaged(2, 2, 2, "qty");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByProductIdSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem> { _db.OrderItems.Skip(1).FirstOrDefault() };

            // Act
            var result = _repo.GetAllByProductIdSortedAndPaged(2, 2, 2, "qty", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion
        
        #endregion

        #region IdVerifiers

        [TestMethod]
        public void OrderIdExists_ItDoes_ShouldReturnTrue()
        {
            var id = _db.Orders.FirstOrDefault().OrderId;

            var result = _repo.OrderIdExists(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void OrderIdExists_ItDoesnt_ShouldReturnFalse()
        {
            var id = Generators.GenInexistentOrderId();

            var result = _repo.OrderIdExists(id);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void ProductIdExists_ItDoes_ShouldReturnTrue()
        {
            var id = _db.Products.FirstOrDefault().ProductId;

            var result = _repo.ProductIdExists(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void ProductIdExists_ItDoesnt_ShouldReturnFalse()
        {
            var id = Generators.GenInexistentProductId();

            var result = _repo.ProductIdExists(id);

            result.Should().BeFalse();
        }

        #endregion

    }
}
