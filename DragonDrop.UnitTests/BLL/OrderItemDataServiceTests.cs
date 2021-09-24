using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.Infrastructure.Helpers;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonDrop.UnitTests.BLL
{
    [TestClass]
    public class OrderItemDataServiceTests
    {
        private IOrderItemDataService _service;
        private IOrderItemRepository _repo;
        private TestDb _db;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestOrderItemRepo(_db);
            _service = new OrderItemDataService(_repo);
        }

        
        [TestMethod]
        public void Get_HappyFlow_ShouldReturnTarget()
        {
            var target = _db.OrderItems.FirstOrDefault();

            var result = _service.Get(target.OrderId, target.ProductId);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNull()
        {
            var id1 = Generators.GenInexistentOrderId();
            var id2 = Generators.GenInexistentProductId();

            var result = _service.Get(id1, id2);

            result.Should().BeNull();
        }

        #region Creates

        [TestMethod]
        public void Create_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();

            // Act
            var result = _service.Create(newItem, true);
            var createdItem = _service.Get(newItem.OrderId, newItem.ProductId);

            // Assert
            result.Should().BeNull();
            createdItem.Should().BeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(null)]
        public void Create_QuantityZero_ShouldNotCreateAndReturnAppropriateError(int qty)
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            newItem.Quantity = qty;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdItem = _service.Get(newItem.OrderId, newItem.ProductId);

            // Assert
            result.Should().Be("OrderItem must have a quantity greater than 0.");
            createdItem.Should().BeNull();
        }

        [TestMethod]
        public void Create_InexistentOrderId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            newItem.OrderId = Generators.GenInexistentOrderId();

            // Act
            var result = _service.Create(newItem, true).GetUntilOrEmpty(".").Trim();
            var createdItem = _service.Get(newItem.OrderId, newItem.ProductId);

            // Assert
            result.Should().Be("OrderItem needs to target a valid Order Id.");
            createdItem.Should().BeNull();
        }

        [TestMethod]
        public void Create_InexistentProductId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            newItem.ProductId = Generators.GenInexistentProductId();

            // Act
            var result = _service.Create(newItem, true).GetUntilOrEmpty(".").Trim();
            var createdItem = _service.Get(newItem.OrderId, newItem.ProductId);

            // Assert
            result.Should().Be("OrderItem needs to target a valid Product Id.");
            createdItem.Should().BeNull();
        }

        [TestMethod]
        public void Create_Duplicate_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var oldItem = _db.OrderItems.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = oldItem.OrderId,
                ProductId = oldItem.ProductId,
                Quantity = oldItem.Quantity + 1
            };

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdItem = _service.Get(newItem.OrderId, newItem.ProductId);

            // Assert
            result.Should().Be("OrderItem already exists. If you wish to change the Quantity, please use the Update.");
            createdItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Create_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            newItem.OrderId = Generators.GenInexistentOrderId();
            newItem.ProductId = Generators.GenInexistentProductId();

            // Act
            var result = _service.Create(newItem, true);
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();
            var createdItem = _service.Get(newItem.OrderId, newItem.ProductId);

            // Assert
            err1.Should().Be("OrderItem needs to target a valid Order Id.");
            err2.Should().Be("OrderItem needs to target a valid Product Id.");
            blank.Should().BeEmpty();
            createdItem.Should().BeNull();
        }

        #endregion

        #region Updates

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.OrderItems.FirstOrDefault();
            var oldItem = new OrderItem
            {
                OrderId = target.OrderId,
                ProductId = target.ProductId,
                Quantity = target.Quantity
            };
            var targetOrdId = target.OrderId;
            var targetProdId = target.ProductId;
            var newItem = new OrderItem
            {
                OrderId = targetOrdId,
                ProductId = targetProdId,
                Quantity = target.Quantity + 1
            };

            // Act
            var result = _service.Update(newItem, true);
            var updatedItem = _service.Get(targetOrdId, targetProdId);

            // Assert
            result.Should().BeNull();
            updatedItem.Should().BeEquivalentTo(newItem);
            updatedItem.Should().NotBeEquivalentTo(oldItem);
        }

        [DataTestMethod]
        public void Update_InexistentEntry_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            var oldRepo = _service.GetAll();

            // Act
            var result = _service.Update(newItem, true).Trim();
            var newRepo = _service.GetAll();

            // Assert
            result.Should().Be("Order Item with Order/Product IDs: " + newItem.OrderId + "/" + newItem.ProductId + " was not found.");
            newRepo.Should().BeEquivalentTo(oldRepo);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(null)]
        public void Update_QuantityZero_ShouldNotUpdateAndReturnAppropriateError(int qty)
        {
            // Arrange
            var target = _db.OrderItems.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = target.OrderId,
                ProductId = target.ProductId,
                Quantity = qty
            };

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedItem = _service.Get(target.OrderId, target.ProductId);

            // Assert
            result.Should().Be("OrderItem must have a quantity greater than 0.");
            updatedItem.Should().BeEquivalentTo(target);
            updatedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_InexistentOrderId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.OrderItems.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = Generators.GenInexistentOrderId(),
                ProductId = target.ProductId,
                Quantity = target.Quantity + 1
            };

            // Act
            var result = _service.Update(newItem, true).GetUntilOrEmpty(".").Trim();
            var updatedItem = _service.Get(target.OrderId, target.ProductId);

            // Assert
            result.Should().Be("OrderItem needs to target a valid Order Id.");
            updatedItem.Should().BeEquivalentTo(target);
            updatedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_InexistentProductId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.OrderItems.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = target.OrderId,
                ProductId = Generators.GenInexistentProductId(),
                Quantity = target.Quantity + 1
            };

            // Act
            var result = _service.Update(newItem, true).GetUntilOrEmpty(".").Trim();
            var updatedItem = _service.Get(target.OrderId, target.ProductId);

            // Assert
            result.Should().Be("OrderItem needs to target a valid Product Id.");
            updatedItem.Should().BeEquivalentTo(target);
            updatedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var target = _db.OrderItems.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = Generators.GenInexistentOrderId(),
                ProductId = Generators.GenInexistentProductId(),
                Quantity = target.Quantity + 1
            };

            // Act
            var result = _service.Update(newItem, true).Trim();
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").Trim().GetUntilOrEmpty(".");
            var err3 = result.Replace(err1, "").Replace(err2, "").Trim().GetUntilOrEmpty(".");
            var blank = result.Replace(err1, "").Replace(err2, "").Replace(err3, "").Trim();
            var updatedItem = _service.Get(target.OrderId, target.ProductId);

            // Assert
            err1.Should().Be("OrderItem needs to target a valid Order Id.");
            err2.Should().Be("OrderItem needs to target a valid Product Id.");
            err3.Should().Be("Order Item with Order/Product IDs: " + newItem.OrderId + "/" + newItem.ProductId + " was not found.");
            blank.Should().BeEmpty();
            updatedItem.Should().BeEquivalentTo(target);
            updatedItem.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region Gets

        [TestMethod]
        public void GetAll_HappyFlow_ShouldFetchAll()
        {
            var result = _service.GetAll();

            result.Should().BeEquivalentTo(_db.OrderItems);
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
            var expected = _service.GetAll();
            var oversizedPage = expected.Count() + 10;

            var result = _service.GetAllPaginated(oversizedPage, 1);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_IndexTooHigh_ShouldReturnEmpty()
        {
            var overIndex = _service.GetAll().Count() + 10;

            var result = _service.GetAllPaginated(1, overIndex);

            result.Should().BeEmpty();
        }

        #endregion

        #region ByIds

        [TestMethod]
        public void GetAllByOrderId_HappyFlow_ShouldReturnMatches()
        {
            // Arrange
            var targetOrdId = _db.Orders.FirstOrDefault().OrderId;
            var expected = _db.OrderItems.Where(i => i.OrderId == targetOrdId);

            // Act
            var result = _service.GetAllByOrderId(targetOrdId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByOrderId_InexistentId_ShouldReturnNull()
        {
            var ordId = Generators.GenInexistentOrderId();

            var result = _service.GetAllByOrderId(ordId);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetAllByProduct_HappyFlow_ShouldReturnMatches()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            _service.Create(newItem);

            // Act
            var result = _service.GetAllByProductId(newItem.ProductId);

            // Assert
            result.Count().Should().Be(2);
            result.Should().ContainEquivalentOf(newItem);
        }

        [TestMethod]
        public void GetAllByProduct_InexistentId_ShouldReturnNull()
        {
            var prodId = Generators.GenInexistentProductId();

            var result = _service.GetAllByProductId(prodId);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetAllByOrderAndPaged_HappyFlow_ShouldReturnMatch()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            _service.Create(newItem);
            var expected = new List<OrderItem> { newItem };

            // Act
            var result = _service.GetAllByOrderIdAndPaged(newItem.OrderId, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByProductAndPaged_HappyFlow_ShouldReturnMatch()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            _service.Create(newItem);
            var expected = new List<OrderItem> { newItem };

            // Act
            var result = _service.GetAllByProductIdAndPaged(newItem.ProductId, 1, 2);

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

            var result = _service.GetAllSorted(sortBy);

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
            var result = _service.GetAllSorted(sortBy);

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
            var result = _service.GetAllSorted(sortBy);

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
            var result = _service.GetAllSorted(sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAlllSorted_Descending_ShoudReturnRequested()
        {
            var expected = _db.OrderItems;

            var result = _service.GetAllSorted("prodId", true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedAndPaged_HappyFlow_ShoudReturnRequested()
        {
            // Arrange
            var expected = new List<OrderItem> { _db.OrderItems.LastOrDefault() };

            // Act
            var result = _service.GetAllSortedAndPaged(2, 2, "qty");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescendingAndPaged_HappyFlow_ShoudReturnRequested()
        {
            // Arrange
            var expected = new List<OrderItem> { _db.OrderItems.Skip(2).FirstOrDefault() };

            // Act
            var result = _service.GetAllSortedAndPaged(2, 2, "prodId", true);

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
            var result = _service.GetAllByOrderIdAndSorted(2, "prodId");

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
            var result = _service.GetAllByOrderIdAndSorted(2, "qty", true);

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
            var result = _service.GetAllByProductIdAndSorted(2, "qty");

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
            var result = _service.GetAllByProductIdAndSorted(1, "ordId", true);

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
            var result = _service.GetAllByOrderIdSortedAndPaged(2, 1, 2, "qty");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByOrderSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem> { _db.OrderItems.Skip(2).FirstOrDefault() };

            // Act
            var result = _service.GetAllByOrderIdSortedAndPaged(3, 1, 2, "qty", true);

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
            var result = _service.GetAllByProductIdSortedAndPaged(2, 2, 2, "qty");

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
            var result = _service.GetAllByProductIdSortedAndPaged(2, 2, 2, "qty", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #endregion

        #region Validates

        [TestMethod]
        public void ValidateOrderItem_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();

            // Act
            var result = _service.ValidateOrderItem(newItem);

            // Assert
            result.errorList.Trim().Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(null)]
        public void ValidateOrderItem_QuantityZero_ShouldReturnFalseAndAppropriateError(int qty)
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            newItem.Quantity = qty;

            // Act
            var result = _service.ValidateOrderItem(newItem);

            // Assert
            result.errorList.Trim().Should().Be("OrderItem must have a quantity greater than 0.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateOrderItem_InexistentOrderId_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            newItem.OrderId = Generators.GenInexistentOrderId();

            // Act
            var result = _service.ValidateOrderItem(newItem);

            // Assert
            result.errorList.GetUntilOrEmpty(".").Trim().Should().Be("OrderItem needs to target a valid Order Id.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateOrderItem_InexistentProductId_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            newItem.ProductId = Generators.GenInexistentProductId();

            // Act
            var result = _service.ValidateOrderItem(newItem);

            // Assert
            result.errorList.GetUntilOrEmpty(".").Trim().Should().Be("OrderItem needs to target a valid Product Id.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateOrderItem_MultipleErrors_ShouldReturnFalseAndAppropriateErrors()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            newItem.OrderId = Generators.GenInexistentOrderId();
            newItem.ProductId = Generators.GenInexistentProductId();

            // Act
            var result = _service.ValidateOrderItem(newItem);
            var err1 = result.errorList.Trim().GetUntilOrEmpty(".");
            var err2 = result.errorList.Replace(err1, "").Trim();
            var blank = result.errorList.Replace(err1, "").Replace(err2, "").Trim();

            // Assert
            err1.Should().Be("OrderItem needs to target a valid Order Id.");
            err2.Should().Be("OrderItem needs to target a valid Product Id.");
            blank.Should().BeEmpty();
            result.isValid.Should().BeFalse();
        }
        
        [TestMethod]
        public void ValidateQuantity_Zero_ShouldReturnFalseAndAppropriateError()
        {
            var result = _service.ValidateQuantity(0);

            // Assert
            result.error.Should().Be("OrderItem must have a quantity greater than 0.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateOrderId_Inexistent_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var ordId = Generators.GenInexistentOrderId();

            // Act
            var result = _service.ValidateOrderId(ordId);

            // Assert
            result.error.Should().Be("OrderItem needs to target a valid Order Id.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProductId_Inexistent_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var prodId = Generators.GenInexistentProductId();

            // Act
            var result = _service.ValidateProductId(prodId);

            // Assert
            result.error.Should().Be("OrderItem needs to target a valid Product Id.");
            result.isValid.Should().BeFalse();
        }


        [TestMethod]
        public void ValidateQuantity_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            var result = _service.ValidateQuantity(1);

            // Assert
            result.error.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateOrderId_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var ordId = _db.Orders.FirstOrDefault().OrderId;

            // Act
            var result = _service.ValidateOrderId(ordId);

            // Assert
            result.error.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateProductId_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var prodId = _db.Products.FirstOrDefault().ProductId;

            // Act
            var result = _service.ValidateProductId(prodId);

            // Assert
            result.error.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }
        #endregion

        #region Async

        [TestMethod]
        public async Task GetAsync_HappyFlow_ShouldReturnTarget()
        {
            var target = _db.OrderItems.FirstOrDefault();

            var result = await _service.GetAsync(target.OrderId, target.ProductId);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public async Task GetAsync_InexistentId_ShouldReturnNull()
        {
            var id1 = Generators.GenInexistentOrderId();
            var id2 = Generators.GenInexistentProductId();

            var result = await _service.GetAsync(id1, id2);

            result.Should().BeNull();
        }

        #region Creates

        [TestMethod]
        public async Task CreateAsync_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdItem = await _service.GetAsync(newItem.OrderId, newItem.ProductId);

            // Assert
            result.Should().BeNull();
            createdItem.Should().BeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(null)]
        public async Task CreateAsync_QuantityZero_ShouldNotCreateAndReturnAppropriateError(int qty)
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            newItem.Quantity = qty;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdItem = await _service.GetAsync(newItem.OrderId, newItem.ProductId);

            // Assert
            result.Should().Be("OrderItem must have a quantity greater than 0.");
            createdItem.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_InexistentOrderId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            newItem.OrderId = Generators.GenInexistentOrderId();

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdItem = await _service.GetAsync(newItem.OrderId, newItem.ProductId);

            // Assert
            result.Should().Be("OrderItem needs to target a valid Order Id.");
            createdItem.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_InexistentProductId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            newItem.ProductId = Generators.GenInexistentProductId();

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdItem = await _service.GetAsync(newItem.OrderId, newItem.ProductId);

            // Assert
            result.Should().Be("OrderItem needs to target a valid Product Id.");
            createdItem.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_Duplicate_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var oldItem = _db.OrderItems.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = oldItem.OrderId,
                ProductId = oldItem.ProductId,
                Quantity = oldItem.Quantity + 1
            };

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdItem = await _service.GetAsync(newItem.OrderId, newItem.ProductId);

            // Assert
            result.Should().Be("OrderItem already exists. If you wish to change the Quantity, please use the Update.");
            createdItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task CreateAsync_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            newItem.OrderId = Generators.GenInexistentOrderId();
            newItem.ProductId = Generators.GenInexistentProductId();

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".");
            var blank = result.Replace(err1, "").Replace(err2, "");
            var createdItem = await _service.GetAsync(newItem.OrderId, newItem.ProductId);

            // Assert
            err1.Should().Be("OrderItem needs to target a valid Order Id.");
            err2.Should().Be("OrderItem needs to target a valid Product Id.");
            blank.Should().BeEmpty();
            createdItem.Should().BeNull();
        }

        #endregion

        #region Updates

        [TestMethod]
        public async Task UpdateAsync_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.OrderItems.FirstOrDefault();
            var oldItem = new OrderItem
            {
                OrderId = target.OrderId,
                ProductId = target.ProductId,
                Quantity = target.Quantity
            };
            var targetOrdId = target.OrderId;
            var targetProdId = target.ProductId;
            var newItem = new OrderItem
            {
                OrderId = targetOrdId,
                ProductId = targetProdId,
                Quantity = target.Quantity + 1
            };

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedItem = await _service.GetAsync(targetOrdId, targetProdId);

            // Assert
            result.Should().BeNull();
            updatedItem.Should().BeEquivalentTo(newItem);
            updatedItem.Should().NotBeEquivalentTo(oldItem);
        }

        [DataTestMethod]
        public async Task UpdateAsync_InexistentEntry_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            var oldRepo = await _service.GetAllAsync();

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var newRepo = await _service.GetAllAsync();

            // Assert
            result.Trim().Should().Be("Order Item with Order/Product IDs: " +
                newItem.OrderId + "/" + newItem.ProductId + " was not found.");
            newRepo.Should().BeEquivalentTo(oldRepo);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(null)]
        public async Task UpdateAsync_QuantityZero_ShouldNotUpdateAndReturnAppropriateError(int qty)
        {
            // Arrange
            var target = _db.OrderItems.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = target.OrderId,
                ProductId = target.ProductId,
                Quantity = qty
            };

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedItem = await _service.GetAsync(target.OrderId, target.ProductId);

            // Assert
            result.Should().Be("OrderItem must have a quantity greater than 0.");
            updatedItem.Should().BeEquivalentTo(target);
            updatedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_InexistentOrderId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.OrderItems.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = Generators.GenInexistentOrderId(),
                ProductId = target.ProductId,
                Quantity = target.Quantity + 1
            };

            // Act
            var result = (await _service.UpdateAsync(newItem, true)).GetUntilOrEmpty(".");
            var updatedItem = await _service.GetAsync(target.OrderId, target.ProductId);

            // Assert
            result.Should().Be("OrderItem needs to target a valid Order Id.");
            updatedItem.Should().BeEquivalentTo(target);
            updatedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_InexistentProductId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.OrderItems.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = target.OrderId,
                ProductId = Generators.GenInexistentProductId(),
                Quantity = target.Quantity + 1
            };

            // Act
            var result = (await _service.UpdateAsync(newItem, true)).GetUntilOrEmpty(".");
            var updatedItem = await _service.GetAsync(target.OrderId, target.ProductId);

            // Assert
            result.Should().Be("OrderItem needs to target a valid Product Id.");
            updatedItem.Should().BeEquivalentTo(target);
            updatedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var target = _db.OrderItems.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = Generators.GenInexistentOrderId(),
                ProductId = Generators.GenInexistentProductId(),
                Quantity = target.Quantity + 1
            };

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".").Trim();
            var err3 = result.Replace(err1, "").Replace(err2, "").GetUntilOrEmpty(".").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").Replace(err3, "").Trim();
            var updatedItem = await _service.GetAsync(target.OrderId, target.ProductId);

            // Assert
            err1.Should().Be("OrderItem needs to target a valid Order Id.");
            err2.Should().Be("OrderItem needs to target a valid Product Id.");
            err3.Should().Be("Order Item with Order/Product IDs: " + newItem.OrderId + "/" + newItem.ProductId + " was not found.");
            blank.Should().BeEmpty();
            updatedItem.Should().BeEquivalentTo(target);
            updatedItem.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region Gets

        [TestMethod]
        public async Task GetAllAsync_HappyFlow_ShouldFetchAll()
        {
            var result = await _service.GetAllAsync();

            result.Should().BeEquivalentTo(_db.OrderItems);
        }

        #region Paginated

        [TestMethod]
        public async Task GetAllPaginatedAsync_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            int pgSize = 1;
            int pgIndex = 3;
            var expected = _db.OrderItems.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();

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
        public async Task GetAllPaginatedAsync_SizeTooBig_ShouldReturnAll()
        {
            var expected = await _service.GetAllAsync();
            var oversizedPage = expected.Count() + 10;

            var result = await _service.GetAllPaginatedAsync(oversizedPage, 1);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllPaginatedAsync_IndexTooHigh_ShouldReturnEmpty()
        {
            var overIndex = (await _service.GetAllAsync()).Count() + 10;

            var result = await _service.GetAllPaginatedAsync(1, overIndex);

            result.Should().BeEmpty();
        }

        #endregion

        #region ByIds

        [TestMethod]
        public async Task GetAllByOrderIdAsync_HappyFlow_ShouldReturnMatches()
        {
            // Arrange
            var targetOrdId = _db.Orders.FirstOrDefault().OrderId;
            var expected = _db.OrderItems.Where(i => i.OrderId == targetOrdId);

            // Act
            var result = await _service.GetAllByOrderIdAsync(targetOrdId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByOrderIdAsync_InexistentId_ShouldReturnNull()
        {
            var ordId = Generators.GenInexistentOrderId();

            var result = await _service.GetAllByOrderIdAsync(ordId);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAllByProductAsync_HappyFlow_ShouldReturnMatches()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            await _service.CreateAsync(newItem);

            // Act
            var result = await _service.GetAllByProductIdAsync(newItem.ProductId);

            // Assert
            result.Count().Should().Be(2);
            result.Should().ContainEquivalentOf(newItem);
        }

        [TestMethod]
        public async Task GetAllByProductAsync_InexistentId_ShouldReturnNull()
        {
            var prodId = Generators.GenInexistentProductId();

            var result = await _service.GetAllByProductIdAsync(prodId);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAllByOrderAndPagedAsync_HappyFlow_ShouldReturnMatch()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            await _service.CreateAsync(newItem);
            var expected = new List<OrderItem> { newItem };

            // Act
            var result = await _service.GetAllByOrderIdAndPagedAsync(newItem.OrderId, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByProductAndPagedAsync_HappyFlow_ShouldReturnMatch()
        {
            // Arrange
            var newItem = Generators.GenOrderItem();
            await _service.CreateAsync(newItem);
            var expected = new List<OrderItem> { newItem };

            // Act
            var result = await _service.GetAllByProductIdAndPagedAsync(newItem.ProductId, 1, 2);

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
        public async Task GetAllSortedAsync_ByOrderId_ShouldReturnRequestedAsync(string sortBy)
        {
            var expected = _db.OrderItems;

            var result = await _service.GetAllSortedAsync(sortBy);

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
        [DataRow("ProductAsync_ID")]
        [DataRow("ProductAsync_id")]
        [DataRow("PROD_ID")]
        [DataRow("prOd_id")]
        public async Task GetAllSortedAsync_ByProductId_ShouldReturnRequestedAsync(string sortBy)
        {
            // Arrange
            var expected = _db.OrderItems;
            expected.Reverse();

            // Act
            var result = await _service.GetAllSortedAsync(sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("QUANTITY")]
        [DataRow("quantITy")]
        [DataRow("QTY")]
        [DataRow("qty")]
        public async Task GetAllSortedAsync_ByQuantity_ShouldReturnRequestedAsync(string sortBy)
        {
            // Arrange
            var expected = _db.OrderItems.OrderBy(i => i.Quantity);

            // Act
            var result = await _service.GetAllSortedAsync(sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("asdf")]
        [DataRow(null)]
        public async Task GetAllSortedAsync_BadQuery_ShouldReturnAll(string sortBy)
        {
            // Arrange
            var expected = _db.OrderItems;

            // Act
            var result = await _service.GetAllSortedAsync(sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAlllSortedAsync_DescendingAsync_ShoudReturnRequested()
        {
            var expected = _db.OrderItems;

            var result = await _service.GetAllSortedAsync("prodId", true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedAndPagedAsync_HappyFlow_ShoudReturnRequested()
        {
            // Arrange
            var expected = new List<OrderItem> { _db.OrderItems.LastOrDefault() };

            // Act
            var result = await _service.GetAllSortedAndPagedAsync(2, 2, "qty");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedDescendingAndPagedAsync_HappyFlow_ShoudReturnRequested()
        {
            // Arrange
            var expected = new List<OrderItem> { _db.OrderItems.Skip(2).FirstOrDefault() };

            // Act
            var result = await _service.GetAllSortedAndPagedAsync(2, 2, "prodId", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByOrderIdAndSortedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem>
                {
                    _db.OrderItems.Skip(4).FirstOrDefault(),
                    _db.OrderItems.Skip(1).FirstOrDefault()
                };

            // Act
            var result = await _service.GetAllByOrderIdAndSortedAsync(2, "prodId");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByOrderIdAndSortedDescendingAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem>
                {
                    _db.OrderItems.Skip(4).FirstOrDefault(),
                    _db.OrderItems.Skip(1).FirstOrDefault()
                };

            // Act
            var result = await _service.GetAllByOrderIdAndSortedAsync(2, "qty", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByProductIdAndSortedAsync_HappyFlow_ShouldReturnRequested()
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
            var result = await _service.GetAllByProductIdAndSortedAsync(2, "qty");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByProductIdAndSortedDescendingAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem>
                {
                    _db.OrderItems.Skip(2).FirstOrDefault(),
                    _db.OrderItems.Skip(4).FirstOrDefault()
                };

            // Act
            var result = await _service.GetAllByProductIdAndSortedAsync(1, "ordId", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByOrderIdSortedAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem> { _db.OrderItems.Skip(4).FirstOrDefault() };

            // Act
            var result = await _service.GetAllByOrderIdSortedAndPagedAsync(2, 1, 2, "qty");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByOrderSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem> { _db.OrderItems.Skip(2).FirstOrDefault() };

            // Act
            var result = await _service.GetAllByOrderIdSortedAndPagedAsync(3, 1, 2, "qty", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByProductIdSortedAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem> { _db.OrderItems.LastOrDefault() };

            // Act
            var result = await _service.GetAllByProductIdSortedAndPagedAsync(2, 2, 2, "qty");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByProductIdSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrderItems();

            var expected = new List<OrderItem> { _db.OrderItems.Skip(1).FirstOrDefault() };

            // Act
            var result = await _service.GetAllByProductIdSortedAndPagedAsync(2, 2, 2, "qty", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #endregion

        #endregion
    }
}

