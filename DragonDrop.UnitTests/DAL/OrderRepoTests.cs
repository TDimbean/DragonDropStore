using DragonDrop.DAL.Entities;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonDrop.UnitTests.DAL
{
    [TestClass]
    public class OrderRepoTests
    {
        private TestOrderRepo _repo;
        private TestDb _db;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestOrderRepo(_db);
        }

        [TestMethod]
        public void Create_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenOrder();
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
            var completeItem = Generators.GenOrder();
            var newItem = new Order
            {
                OrderDate = completeItem.OrderDate,
                ShippingDate = completeItem.ShippingDate,
                ShippingMethodId = completeItem.ShippingMethodId,
                OrderStatusId = completeItem.OrderStatusId,
                PaymentMethodId = completeItem.PaymentMethodId,
                CustomerId = -3
            };

            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Create(newItem);
            var newCount = _repo.GetAll().Count();

            // Assert
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var item = _db.Orders.FirstOrDefault();
            var oldItem = new Order
            {
                OrderId = item.OrderId,
                ShippingMethodId = item.ShippingMethodId,
                CustomerId = item.CustomerId,
                ShippingDate = item.ShippingDate,
                OrderStatusId = item.OrderStatusId,
                OrderDate = item.OrderDate,
                PaymentMethodId = item.PaymentMethodId
            };
            var updItem = Generators.GenOrder();
            updItem.OrderId = item.OrderId;

            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Update(updItem);
            var result = _repo.Get(oldItem.OrderId);
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
            var item = _db.Orders.FirstOrDefault();
            var oldItem = new Order
            {
                OrderId = item.OrderId,
                ShippingDate = item.ShippingDate,
                ShippingMethodId = item.ShippingMethodId,
                OrderStatusId = item.OrderStatusId,
                CustomerId = item.CustomerId,
                OrderDate = item.OrderDate,
                PaymentMethodId = item.PaymentMethodId
            };
            var updItem = Generators.GenOrder();
            updItem.OrderId = Generators.GenInexistentOrderId();
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Update(updItem);
            var result = _repo.Get(oldItem.OrderId);
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
        public void Get_HappyFlow_ShouldRetrieveMatch()
        {
            var expected = _db.Orders.FirstOrDefault();

            var result = _repo.Get(expected.OrderId);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNull()
        {
            var inexistentId = Generators.GenInexistentOrderId();

            var result = _repo.Get(inexistentId);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetAll_HappyFlow_ShouldRetrieveAllRecords()
        {
            var result = _repo.GetAll();

            result.Should().BeEquivalentTo(_db.Orders.ToList());
        }

        [TestMethod]
        public void GetAllUnprocessed_HappyFlow_ShouldRetrieveAllUnprocessedRecords()
        {
            var result = _repo.GetAllUnprocessed();

            result.Should().BeEquivalentTo(_db.Orders.Where(o => o.OrderStatusId == 0).ToList());
            result.Any(o => o.OrderStatusId != 0).Should().BeFalse();
        }

        [TestMethod]
        public void GetAllProcessed_HappyFlow_ShouldRetrieveAllProcessedRecords()
        {
            var result = _repo.GetAllProcessed();

            result.Should().BeEquivalentTo(_db.Orders.Where(o => o.OrderStatusId == 1).ToList());
            result.Any(o => o.OrderStatusId != 1).Should().BeFalse();
        }

        #region Paginated

        [TestMethod]
        public void GetAllPaginated_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            int pgSize = 1;
            int pgIndex = 3;
            var expected = _db.Orders.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();

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

        #region Filtered

        [TestMethod]
        public void GetAllFiltered_CustomerNameMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = _repo.GetAllFiltered(_db.Customers.SingleOrDefault(c => c.CustomerId == targetItem.CustomerId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_ShippingMethodMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = _repo.GetAllFiltered(_db.ShippingMethods.SingleOrDefault(s => s.ShippingMethodId == targetItem.ShippingMethodId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_PaymentMethodMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = _repo.GetAllFiltered(_db.PaymentMethods.SingleOrDefault(p => p.PaymentMethodId == targetItem.PaymentMethodId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_OrderStatusMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = _repo.GetAllFiltered(_db.OrderStatuses.SingleOrDefault(os => os.OrderStatusId == targetItem.OrderStatusId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_OrderDateMatch_ShouldReturnMatch()
        {
            // Arrange
            var targetItem = Generators.GenOrder();

            var uniDate = Generators.GenUnusedDate();
            targetItem.OrderDate = uniDate;
            targetItem.ShippingDate = null;

            _repo.Create(targetItem);

            var expected = new List<Order> { targetItem };

            // Act
            var result = _repo.GetAllFiltered(targetItem.OrderDate.ToShortDateString());

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_ShippingDateMatch_ShouldReturnMatch()
        {
            // Arrange
            var targetItem = Generators.GenOrder();

            var uniDate = Generators.GenUnusedDate();
            targetItem.OrderDate = uniDate;
            targetItem.ShippingDate = uniDate.AddDays(2);

            _repo.Create(targetItem);

            var expected = new List<Order> { targetItem };

            // Act
            var result = _repo.GetAllFiltered(targetItem.ShippingDate.GetValueOrDefault().ToShortDateString());

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
            var target1 = Generators.GenOrder();
            var target2 = Generators.GenOrder();
            var uniDate1 = Generators.GenUnusedDate();
            var uniDate2 = Generators.GenUnusedDate();
            target1.OrderDate = uniDate1;
            target1.ShippingDate = null;
            target2.OrderDate = uniDate2;
            target2.ShippingDate = null;
            var search = target1.OrderDate.ToShortDateString() + " " +
                         target2.OrderDate.ToShortDateString();
            var expected = new List<Order> { target1, target2 };
            _repo.Create(target1);
            _repo.Create(target2);

            // Act
            var result = _repo.GetAllFiltered(search);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        [TestMethod]
        public void GetAllFilteredAndPaged_HappyFlow_ShouldReturnMatch()
        {
            var search = _db.Orders.FirstOrDefault().OrderDate.ToShortDateString();
            var expected = new List<Order> { _repo.Get(2) };

            var result = _repo.GetAllFilteredAndPaged(search, 1, 2);

            result.Should().BeEquivalentTo(expected);
        }

        #region ByCustomerId

        [TestMethod]
        public void GetAllByCustomerId_HappyFlow_ShouldRetrieve()
        {
            // Arrange
            var oldItem = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.CustomerId = oldItem.CustomerId;
            _repo.Create(newItem);
            var expected = new List<Order> { oldItem, newItem };

            // Act
            var result = _repo.GetAllByCustomerId(newItem.CustomerId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomeId_InexistentId_ShouldReturnEmpty()
        {
            var custId = Generators.GenInexistentCustomerId();

            var result = _repo.GetAllByCustomerId(custId);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetAllByCustomerIdAndPaged_HappyFlow_ShouldReturnDesired()
        {
            // Arrange
            var context = new TestDb();
            var custRepo = new TestCustomerRepo(context);
            var repo = new TestOrderRepo(context);

            var newCustomer = new Customer
            {
                Name = "Will Smith",
                Phone = Generators.GenPhoneNumber(),
                Email = "wSmith.contacti@gmail.com",
                Address = "138 Dew Lane",
                City = "Fresno",
                State = "California"
            };

            custRepo.Create(newCustomer);
            newCustomer = context.Customers.SingleOrDefault(c => c.Phone == newCustomer.Phone);

            var target1 = Generators.GenOrder();
            var target2 = Generators.GenOrder();
            var target3 = Generators.GenOrder();

            target1.CustomerId = newCustomer.CustomerId;
            target2.CustomerId = newCustomer.CustomerId;
            target3.CustomerId = newCustomer.CustomerId;

            repo.Create(target1);
            repo.Create(target2);
            repo.Create(target3);

            // Act
            var firstSingleResult = repo.GetAllByCustomerIdAndPaged(newCustomer.CustomerId, 1, 1);
            var secondSingleResult = repo.GetAllByCustomerIdAndPaged(newCustomer.CustomerId, 1, 2);
            var thirdSingleResult = repo.GetAllByCustomerIdAndPaged(newCustomer.CustomerId, 1, 3);
            var firstDoubleResult = repo.GetAllByCustomerIdAndPaged(newCustomer.CustomerId, 2, 1);
            var lastDoubleResult = repo.GetAllByCustomerIdAndPaged(newCustomer.CustomerId, 2, 2);

            // Assert
            firstSingleResult.Should().BeEquivalentTo(new List<Order> { target1 });
            secondSingleResult.Should().BeEquivalentTo(new List<Order> { target2 });
            thirdSingleResult.Should().BeEquivalentTo(new List<Order> { target3 });
            firstDoubleResult.Should().BeEquivalentTo(new List<Order> { target1, target2 });
            lastDoubleResult.Should().BeEquivalentTo(new List<Order> { target3 });
        }

        [TestMethod]
        public void GetAllByCustomerIdAndPaged_InexistentCustomerId_ShouldReturnNull()
        {
            var custId = Generators.GenInexistentCustomerId();

            var result = _repo.GetAllByCustomerIdAndPaged(custId, 1, 1);

            result.Should().BeNull();
        }

        #endregion

        #region Sorts

        [DataTestMethod]
        [DataRow("ID")]
        [DataRow("id")]
        [DataRow("ORDERID")]
        [DataRow("ORDID")]
        [DataRow("OrdId")]
        [DataRow("orderId")]
        [DataRow("ORD_ID")]
        [DataRow("ord_id")]
        public void GetAllSorted_ById_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.OrderId);

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("CUST")]
        [DataRow("cust")]
        [DataRow("CUSTID")]
        [DataRow("custId")]
        [DataRow("CUSTOMER")]
        [DataRow("cuStoMer")]
        [DataRow("CUSTOMERID")]
        [DataRow("CustomerId")]
        public void GetAllSorted_ByCustomerId_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.CustomerId);

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("SHIP")]
        [DataRow("ShIp")]
        [DataRow("SHIPPING")]
        [DataRow("shipping")]
        [DataRow("SHIPPINGMETHOD")]
        [DataRow("shiPpingMethod")]
        [DataRow("SHIPMETHOD")]
        [DataRow("shIpMetHod")]
        [DataRow("DELIVERY")]
        [DataRow("deliVery")]
        [DataRow("DELIVERYMETHOD")]
        [DataRow("deLiVERyMethOd")]
        [DataRow("SHIPMETH")]
        [DataRow("ShipMetH")]
        [DataRow("SHIP_METH")]
        [DataRow("shIp_mEth")]
        [DataRow("SHIPPING_METHOD")]
        [DataRow("Shipping_method")]
        [DataRow("SHIP_METHOD")]
        [DataRow("sHip_meThod")]
        public void GetAllSorted_ByShippingMethodId_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.ShippingMethodId);

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("PAY")]
        [DataRow("pay")]
        [DataRow("PAYMENT")]
        [DataRow("paYmeNt")]
        [DataRow("PAYMETHOD")]
        [DataRow("PaYMethod")]
        [DataRow("PAYMENTMETHOD")]
        [DataRow("PaYMentMethod")]
        public void GetAllSorted_ByPaymentMethodId_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.PaymentMethodId);

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("ORDERDATE")]
        [DataRow("OrdErDate")]
        [DataRow("ORDER_DATE")]
        [DataRow("OrdeR_Date")]
        [DataRow("PLACED")]
        [DataRow("Placed")]
        public void GetAllSorted_ByOrderDate_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.OrderDate);

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("SHIPPINGDATE")]
        [DataRow("shippingDate")]
        [DataRow("SHIPPING_DATE")]
        [DataRow("shIPping_daTE")]
        [DataRow("SHIPDATE")]
        [DataRow("shIpDate")]
        [DataRow("SHIP_DATE")]
        [DataRow("ship_date")]
        [DataRow("SHIPPED")]
        [DataRow("shIpped")]
        public void GetAllSorted_ByShippingDate_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.ShippingDate);

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("STATUS")]
        [DataRow("StaTus")]
        [DataRow("STAT")]
        [DataRow("stat")]
        [DataRow("ORDERSTATUS")]
        [DataRow("OrderStatus")]
        [DataRow("ORDSTATUS")]
        [DataRow("ordStatus")]
        [DataRow("ORDER_STATUS")]
        [DataRow("order_status")]
        [DataRow("ORD_STARTUS")]
        [DataRow("ord_status")]
        [DataRow("ORDSTAT")]
        [DataRow("ordStat")]
        [DataRow("ORD_STAT")]
        [DataRow("ord_stat")]
        public void GetAllSorted_ByOrderStatusId_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.OrderStatusId);

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
            var sortBy = "Shipmethod";
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
            var sortBy = "orderdate";
            var pgIndex = 3;
            var pgSize = 1;

            var expected = new List<Order> { _db.Orders.LastOrDefault() };

            // Act
            var result = _repo.GetAllSortedAndPaged(pgSize, pgIndex, sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var sortBy = "shippingMethod";
            var pgIndex = 3;
            var pgSize = 1;

            var expected = new List<Order> { _db.Orders.FirstOrDefault() };

            // Act
            var result = _repo.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            //// Arrange
            _db.BumpOrders();
            var searchBy = "cash";
            var sortBy = "ordDate";

            var expected = new List<Order> { _db.Orders.SingleOrDefault(o => o.OrderId == 2) };
            expected.AddRange(_db.Orders.Where(o=>o.OrderId>3).Reverse());

            // Act
            var result = _repo.GetAllFilteredAndSorted(searchBy, sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            //// Arrange

            _db.BumpOrders();

            // Regine is Cust with ID 2
            var searchBy = "Regine";
            // In regular Order: 2, 2, 1. Sorted Asc would reverse them, descending should leave them just as if it were a simple filter
            var sortBy = "shippingMethod";

            var expected = _repo.GetAllFiltered(searchBy);

            // Act
            var result = _repo.GetAllFilteredAndSorted(searchBy, sortBy, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrders();

            var repo = new TestCustomerRepo(_db);
            var expected = _db.Orders.Skip(3).Take(2).Reverse();

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged("cash", 2, 2, "ordDate");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrders();

            var expected = new List<Order> { _db.Orders.SingleOrDefault(o => o.OrderId == 6), _db.Orders.SingleOrDefault(o => o.OrderId == 2) };

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged("cash", 2, 2, "ordDate", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrders();

            var expected = new List<Order>
            {
                _db.Orders.SingleOrDefault(o=>o.OrderId == 6),
                _db.Orders.SingleOrDefault(o=>o.OrderId == 2),
                _db.Orders.SingleOrDefault(o=>o.OrderId == 4)
            };

            // Act
            var result = _repo.GetAllByCustomerIdAndSorted(2, "shipMethod");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrders();

            var expected = new List<Order>
            {
                _db.Orders.SingleOrDefault(o=>o.OrderId == 4),
                _db.Orders.SingleOrDefault(o=>o.OrderId == 2),
                _db.Orders.SingleOrDefault(o=>o.OrderId == 6)
            };

            // Act
            var result = _repo.GetAllByCustomerIdAndSorted(2, "shipMethod", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrders();

            var expected = new List<Order> { _db.Orders.SingleOrDefault(o => o.OrderId == 4) };

            // Act
            var result = _repo.GetAllByCustomerIdSortedAndPaged(2, 2, 2, "ordDate");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrders();

            var expected = new List<Order> { _db.Orders.SingleOrDefault(o => o.OrderId == 2) };

            // Act
            var result = _repo.GetAllByCustomerIdSortedAndPaged(2, 2, 2, "ordDate", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndFiltered_HappyFlow_ShoulReturnequested()
        {
            // Arrange
            _db.BumpOrders();

            var expected = new List<Order> { _db.Orders.SingleOrDefault(o => o.OrderId == 4) };

            // Act
            var result = _repo.GetAllByCustomerIdAndFiltered(2, "UPS");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrdersDouble();

            var expected = _db.Orders.SingleOrDefault(o => o.OrderId == 9);

            // Act
            var result = _repo.GetAllByCustomerIdFilteredAndPaged(2, "Credit Card", 2, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrdersDouble();

            var expected = new List<Order>
                {
                    _db.Orders.SingleOrDefault(o=>o.OrderId == 7),
                    _db.Orders.SingleOrDefault(o=>o.OrderId == 8),
                    _db.Orders.SingleOrDefault(o=>o.OrderId == 2)
                };

            // Act
            var result = _repo.GetAllByCustomerIdFilteredAndSorted(2, "Delivered", "ordDate");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpOrdersDouble();

            var expected = new List<Order>
                {
                    _db.Orders.SingleOrDefault(o=>o.OrderId == 9),
                    _db.Orders.SingleOrDefault(o=>o.OrderId == 7),
                    _db.Orders.SingleOrDefault(o=>o.OrderId == 8)
                };

            // Act
            var result = _repo.GetAllByCustomerIdFilteredAndSorted(2, "Credit Card", "stat", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredSortedAndPaged()
        {
            // Arrange
            _db.BumpOrdersDouble();

            var expected = new List<Order>
            {
                _db.Orders.SingleOrDefault(o=>o.OrderId == 7),
                _db.Orders.SingleOrDefault(o=>o.OrderId == 8)
            };

            // Act
            var result = _repo.GetAllByCustomerIdFilteredSortedAndPaged(2, "delivered", 2, 1, "shipped");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredSortedDescendingAndPaged()
        {
            // Arrange
            _db.BumpOrdersDouble();

            var expected = new List<Order>
            {
                _db.Orders.SingleOrDefault(o=>o.OrderId == 2),
                _db.Orders.SingleOrDefault(o=>o.OrderId == 8)
            };

            // Act
            var result = _repo.GetAllByCustomerIdFilteredSortedAndPaged(2, "Delivered", 2, 1, "shipped", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion Sorts

        #endregion

        #region IdVerifiers

        [TestMethod]
        public void CustomerIdExists_ItDoes_ShouldReturnTrue()
        {
            var id = _db.Customers.FirstOrDefault().CustomerId;

            var result = _repo.CustomerIdExists(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CustomerIdExists_ItDoesNot_ShouldReturnFalse()
        {
            var id = Generators.GenInexistentCustomerId();

            var result = _repo.CustomerIdExists(id);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void PaymentMethodIdExists_ItDoes_ShouldReturnTrue()
        {
            var id = _db.PaymentMethods.FirstOrDefault().PaymentMethodId;

            var result = _repo.PaymentMethodIdExists(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void PaymentMethodIdExists_ItDoesNot_ShouldReturnFalse()
        {
            var id = _db.PaymentMethods.Count() + 1;

            var result = _repo.PaymentMethodIdExists(id);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void ShippingMethodIdExists_ItDoes_ShouldReturnTrue()
        {
            var id = _db.ShippingMethods.FirstOrDefault().ShippingMethodId;

            var result = _repo.ShippingMethodIdExists(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void ShippingMethodIdExists_ItDoesNot_ShouldReturnFalse()
        {
            var id = _db.ShippingMethods.Count() + 1;

            var result = _repo.ShippingMethodIdExists(id);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void OrderStatusIdExists_ItDoes_ShouldReturnTrue()
        {
            var id = _db.OrderStatuses.FirstOrDefault().OrderStatusId;

            var result = _repo.OrderStatusIdExists(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void OrderStatusIdExists_ItDoesNot_ShouldReturnFalse()
        {
            var id = _db.OrderStatuses.Count() + 1;

            var result = _repo.OrderStatusIdExists(id);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void OrderIdExists_ItDoes_ShouldReturnTrue()
        {
            var id = _db.Orders.FirstOrDefault().OrderId;

            var result = _repo.OrderIdExists(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void OrderIdExists_ItDoesNot_ShouldReturnFalse()
        {
            var id = Generators.GenInexistentOrderId();

            var result = _repo.OrderIdExists(id);

            result.Should().BeFalse();
        }

        #endregion

        #region StatusAdvancements

        [TestMethod]
        public void PromoteReceived_HappyFlow_ShouldAdvance()
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = 0;

            _repo.Create(target);

            // Act
            _repo.PromoteReceived(0);
            var result = _repo.Get(0).OrderStatusId;

            // Assert
            result.Should().Be(1);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void PromoteReceived_WrongStatus_ShouldNotModify(int statId)
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = statId > 1 ? target.OrderDate.AddDays(2) :(DateTime?) null; ;
            target.OrderStatusId = statId;

            _repo.Create(target);

            // Act
            _repo.PromoteReceived(0);
            var result = _repo.Get(0);

            // Assert
            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public void PromotedReceived_InexistentId_ShouldNotModify()
        {
            // Arrange
            var badId = _db.Orders.Count() + 1;

            // Act
            _repo.PromoteReceived(badId);
            var result = _repo.GetAll();

            // Assert
            result.Should().BeEquivalentTo(_db.Orders);
        }

        [TestMethod]
        public void PromoteProcessed_HappyFlow_ShouldAdvanceAndSetShipDate()
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = 1;

            _repo.Create(target);

            var shipDate = target.OrderDate.AddDays(2);

            // Act
            _repo.PromoteProcessed(0, shipDate);
            var result = _repo.Get(0);

            // Assert
            result.OrderStatusId.Should().Be(2);
            result.ShippingDate.Should().Be(shipDate);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(2)]
        [DataRow(3)]
        public void PromoteProcessed_WrongStatus_ShouldNotModify(int statId)
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = statId;

            _repo.Create(target);

            var shipDate = target.OrderDate.AddDays(2);

            // Act
            _repo.PromoteProcessed(0, shipDate);
            var result = _repo.Get(0);

            // Assert
            result.OrderStatusId.Should().Be(statId);
            result.ShippingDate.Should().BeNull();
        }

        [TestMethod]
        public void PromoteProcessed_BadDate_ShouldNotModify()
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = 1;

            _repo.Create(target);

            var shipDate = target.OrderDate.AddDays(-2);

            // Act
            _repo.PromoteProcessed(0, shipDate);
            var result = _repo.Get(0);

            // Assert
            result.OrderStatusId.Should().Be(1);
            result.ShippingDate.Should().Be(null);
        }

        [TestMethod]
        public void PromoteProcessed_InexistentId_ShouldNotModify()
        {
            // Arrange
            var shipDate = DateTime.Today;
            var badId = _db.Orders.Count() + 1;

            // Act
            _repo.PromoteProcessed(badId, shipDate);
            var result = _repo.GetAll();

            // Assert
            result.Should().BeEquivalentTo(_db.Orders);
        }

        [TestMethod]
        public void BatchPromoteReceived_HappyFlow_ShouldAdvanceAll()
        {
            // Arrange
            _db.BumpReceivedOrders();
            var repo = new TestOrderRepo(_db);

            // Act
            repo.BatchPromoteReceived(new List<int> { 4, 5, 6 });
            var result1 = repo.Get(4).OrderStatusId;
            var result2 = repo.Get(5).OrderStatusId;
            var result3 = repo.Get(6).OrderStatusId;

            // Assert
            result1.Should().Be(1);
            result2.Should().Be(1);
            result3.Should().Be(1);
        }

        [TestMethod]
        public void BatchPromoteReceived_PartiallyWrong_ShouldOnlyAdvanceReceived()
        {
            // Arrange
            _db.BumpReceivedOrders();
            var corrupts = _db.Orders.Where(o => o.OrderId > 4);
            foreach (var ord in corrupts)
                ord.OrderStatusId = 2;
            var repo = new TestOrderRepo(_db);

            // Act
            repo.BatchPromoteReceived(new List<int> { 4, 5, 6 });
            var result1 = repo.Get(4).OrderStatusId;
            var result2 = repo.Get(5).OrderStatusId;
            var result3 = repo.Get(6).OrderStatusId;

            // Assert
            result1.Should().Be(1);
            result2.Should().Be(2);
            result3.Should().Be(2);
        }

        [TestMethod]
        public void BatchPromoteReceived_AllWrong_ShouldOnlyNotModify()
        {
            // Arrange
            _db.BumpReceivedOrders();
            var corrupts = _db.Orders.Where(o => o.OrderId > 3);
            foreach (var ord in corrupts)
                ord.OrderStatusId = 2;
            var repo = new TestOrderRepo(_db);

            // Act
            repo.BatchPromoteReceived(new List<int> { 4, 5, 6 });
            var result1 = repo.Get(4).OrderStatusId;
            var result2 = repo.Get(5).OrderStatusId;
            var result3 = repo.Get(6).OrderStatusId;

            // Assert
            result1.Should().Be(2);
            result2.Should().Be(2);
            result3.Should().Be(2);
        }

        [TestMethod]
        public void BatchPromoteReceived_PartiallyWrongId_ShouldOnlyAdvanceExistent()
        {
            // Arrange
            _db.BumpReceivedOrders();
            var repo = new TestOrderRepo(_db);

            // Act
            repo.BatchPromoteReceived(new List<int> { 4, 12,13 });
            var result1 = repo.Get(4).OrderStatusId;
            var result2 = repo.Get(5).OrderStatusId;
            var result3 = repo.Get(6).OrderStatusId;

            // Assert
            result1.Should().Be(1);
            result2.Should().Be(0);
            result3.Should().Be(0);
        }

        [TestMethod]
        public void BatchPromoteReceived_IdsAllWrong_ShouldNotModify()
        {
            // Arrange
            _db.BumpReceivedOrders();
            var repo = new TestOrderRepo(_db);

            // Act
            repo.BatchPromoteReceived(new List<int> { 11, 12, 13 });
            var result = repo.GetAll();

            // Assert
            result.Should().BeEquivalentTo(_db.Orders);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task CreateAsync_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            var initCount = (await _repo.GetAllAsync()).Count();

            // Act
            await _repo.CreateAsync(newItem);
            var result = await _repo.GetAllAsync();
            var newCount = result.Count();

            // Assert
            result.Should().ContainEquivalentOf(newItem);
            newCount.Should().Be(initCount + 1);
        }

        [TestMethod]
        public async Task CreateAsync_IncompleteRecord_ShouldNotCreate()
        {
            // Arrange
            var completeItem = Generators.GenOrder();
            var newItem = new Order
            {
                OrderDate = completeItem.OrderDate,
                ShippingDate = completeItem.ShippingDate,
                ShippingMethodId = completeItem.ShippingMethodId,
                OrderStatusId = completeItem.OrderStatusId,
                PaymentMethodId = completeItem.PaymentMethodId,
                CustomerId = -3
            };

            var initCount = (await _repo.GetAllAsync()).Count();

            // Act
            await _repo.CreateAsync(newItem);
            var newCount = (await _repo.GetAllAsync()).Count();

            // Assert
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public async Task UpdateAsync_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var item = _db.Orders.FirstOrDefault();
            var oldItem = new Order
            {
                OrderId = item.OrderId,
                ShippingMethodId = item.ShippingMethodId,
                CustomerId = item.CustomerId,
                ShippingDate = item.ShippingDate,
                OrderStatusId = item.OrderStatusId,
                OrderDate = item.OrderDate,
                PaymentMethodId = item.PaymentMethodId
            };
            var updItem = Generators.GenOrder();
            updItem.OrderId = item.OrderId;

            var initCount = (await _repo.GetAllAsync()).Count();

            // Act
            await _repo.UpdateAsync(updItem);
            var result = await _repo.GetAsync(oldItem.OrderId);
            var newRepo = await _repo.GetAllAsync();
            var newCount = newRepo.Count();

            // Assert
            newCount.Should().Be(initCount);
            newRepo.Should().ContainEquivalentOf(updItem);
            result.Should().BeEquivalentTo(updItem);
            result.Should().NotBeEquivalentTo(oldItem);
        }

        [TestMethod]
        public async Task UpdateAsync_InexistentId_ShouldNotUpdate()
        {
            // Arrange
            var item = _db.Orders.FirstOrDefault();
            var oldItem = new Order
            {
                OrderId = item.OrderId,
                ShippingDate = item.ShippingDate,
                ShippingMethodId = item.ShippingMethodId,
                OrderStatusId = item.OrderStatusId,
                CustomerId = item.CustomerId,
                OrderDate = item.OrderDate,
                PaymentMethodId = item.PaymentMethodId
            };
            var updItem = Generators.GenOrder();
            updItem.OrderId = Generators.GenInexistentOrderId();
            var initCount = (await _repo.GetAllAsync()).Count();

            // Act
            await _repo.UpdateAsync(updItem);
            var result = await _repo.GetAsync(oldItem.OrderId);
            var newRepo = await _repo.GetAllAsync();
            var newCount = newRepo.Count();

            // Assert
            newCount.Should().Be(initCount);
            newRepo.Should().ContainEquivalentOf(oldItem);
            result.Should().BeEquivalentTo(oldItem);
            result.Should().NotBeEquivalentTo(updItem);
        }

        #region Gets

        [TestMethod]
        public async Task GetAsync_HappyFlow_ShouldRetrieveMatch()
        {
            var expected = _db.Orders.FirstOrDefault();

            var result = await _repo.GetAsync(expected.OrderId);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAsync_InexistentId_ShouldReturnNull()
        {
            var inexistentId = Generators.GenInexistentOrderId();

            var result = await _repo.GetAsync(inexistentId);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAllAsync_HappyFlow_ShouldRetrieveAllRecords()
        {
            var result = await _repo.GetAllAsync();

            result.Should().BeEquivalentTo(_db.Orders.ToList());
        }

        [TestMethod]
        public async Task GetAllUnprocessedAsync_HappyFlow_ShouldRetrieveAllUnprocessedRecords()
        {
            var result = await _repo.GetAllUnprocessedAsync();

            result.Should().BeEquivalentTo(_db.Orders.Where(o => o.OrderStatusId == 0).ToList());
            result.Any(o => o.OrderStatusId != 0).Should().BeFalse();
        }

        [TestMethod]
        public async Task GetAllProcessedAsync_HappyFlow_ShouldRetrieveAllProcessedRecords()
        {
            var result = await _repo.GetAllProcessedAsync();

            result.Should().BeEquivalentTo(_db.Orders.Where(o => o.OrderStatusId == 1).ToList());
            result.Any(o => o.OrderStatusId != 1).Should().BeFalse();
        }

        #region Paginated

        [TestMethod]
        public async Task GetAllPaginatedAsync_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            int pgSize = 1;
            int pgIndex = 3;
            var expected = _db.Orders.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();

            // Act
            var result = await _repo.GetAllPaginatedAsync(pgSize, pgIndex);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllPaginatedAsync_FullPage_ShouldReturnRightSize()
        {
            var result = await _repo.GetAllPaginatedAsync(2, 1);

            result.Count().Should().Be(2);
        }

        [TestMethod]
        public async Task GetAllPaginatedAsync_PartialPage_ShouldReturnRightSize()
        {
            var result = await _repo.GetAllPaginatedAsync(2, 2);

            result.Count().Should().Be(1);
        }

        [TestMethod]
        public async Task GetAllPaginatedAsync_SizeTooBig_ShouldReturnAll()
        {
            var expected = await _repo.GetAllAsync();
            var oversizedPage = expected.Count() + 10;

            var result = await _repo.GetAllPaginatedAsync(oversizedPage, 1);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllPaginatedAsync_IndexTooHigh_ShouldReturnEmpty()
        {
            var overIndex = (await _repo.GetAllAsync()).Count() + 10;

            var result = await _repo.GetAllPaginatedAsync(1, overIndex);

            result.Should().BeEmpty();
        }

        #endregion

        #region Filtered

        [TestMethod]
        public async Task GetAllFilteredAsync_CustomerNameMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = await _repo.GetAllFilteredAsync(_db.Customers.SingleOrDefault(c => c.CustomerId == targetItem.CustomerId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_ShippingMethodMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = await _repo.GetAllFilteredAsync(_db.ShippingMethods.SingleOrDefault(s => s.ShippingMethodId == targetItem.ShippingMethodId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_PaymentMethodMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = await _repo.GetAllFilteredAsync(_db.PaymentMethods.SingleOrDefault(p => p.PaymentMethodId == targetItem.PaymentMethodId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_OrderStatusMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = await _repo.GetAllFilteredAsync(_db.OrderStatuses.SingleOrDefault(os => os.OrderStatusId == targetItem.OrderStatusId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_OrderDateMatch_ShouldReturnMatch()
        {
            // Arrange
            var targetItem = Generators.GenOrder();

            var uniDate = Generators.GenUnusedDate();
            targetItem.OrderDate = uniDate;
            targetItem.ShippingDate = null;

            await _repo.CreateAsync(targetItem);

            var expected = new List<Order> { targetItem };

            // Act
            var result = await _repo.GetAllFilteredAsync(targetItem.OrderDate.ToShortDateString());

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_ShippingDateMatch_ShouldReturnMatch()
        {
            // Arrange
            var targetItem = Generators.GenOrder();

            var uniDate = Generators.GenUnusedDate();
            targetItem.OrderDate = uniDate;
            targetItem.ShippingDate = uniDate.AddDays(2);

            await _repo.CreateAsync(targetItem);

            var expected = new List<Order> { targetItem };

            // Act
            var result = await _repo.GetAllFilteredAsync(targetItem.ShippingDate.GetValueOrDefault().ToShortDateString());

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_InexistentTerm_ShouldReturnEmpty()
        {
            var search = Guid.NewGuid().ToString();

            var result = await _repo.GetAllFilteredAsync(search);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_DifTermsForDifResults_ShoudReturnBothMatches()
        {
            // Arrange
            var target1 = Generators.GenOrder();
            var target2 = Generators.GenOrder();
            var uniDate1 = Generators.GenUnusedDate();
            var uniDate2 = Generators.GenUnusedDate();
            target1.OrderDate = uniDate1;
            target1.ShippingDate = null;
            target2.OrderDate = uniDate2;
            target2.ShippingDate = null;
            var search = target1.OrderDate.ToShortDateString() + " " +
                         target2.OrderDate.ToShortDateString();
            var expected = new List<Order> { target1, target2 };
            await _repo.CreateAsync(target1);
            await _repo.CreateAsync(target2);

            // Act
            var result = await _repo.GetAllFilteredAsync(search);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_HappyFlow_ShouldReturnMatch()
        {
            var search = _db.Orders.FirstOrDefault().OrderDate.ToShortDateString();
            var expected = new List<Order> { await _repo.GetAsync(2) };

            var result = await _repo.GetAllFilteredAndPagedAsync(search, 1, 2);

            result.Should().BeEquivalentTo(expected);
        }

        #region ByCustomerId

        [TestMethod]
        public async Task GetAllByCustomerIdAsync_HappyFlow_ShouldRetrieve()
        {
            // Arrange
            var oldItem = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.CustomerId = oldItem.CustomerId;
            await _repo.CreateAsync(newItem);
            var expected = new List<Order> { oldItem, newItem };

            // Act
            var result = await _repo.GetAllByCustomerIdAsync(newItem.CustomerId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomeIdAsync_InexistentId_ShouldReturnEmpty()
        {
            var custId = Generators.GenInexistentCustomerId();

            var result = await _repo.GetAllByCustomerIdAsync(custId);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndPagedAsync_HappyFlow_ShouldReturnDesiredAsync()
        {
            // Arrange
            var context = new TestDb();
            var custRepo = new TestCustomerRepo(context);
            var repo = new TestOrderRepo(context);

            var newCustomer = new Customer
            {
                Name = "Will Smith",
                Phone = Generators.GenPhoneNumber(),
                Email = "wSmith.contacti@gmail.com",
                Address = "138 Dew Lane",
                City = "Fresno",
                State = "California"
            };

            await custRepo.CreateAsync(newCustomer);
            newCustomer = context.Customers.SingleOrDefault(c => c.Phone == newCustomer.Phone);

            var target1 = Generators.GenOrder();
            var target2 = Generators.GenOrder();
            var target3 = Generators.GenOrder();

            target1.CustomerId = newCustomer.CustomerId;
            target2.CustomerId = newCustomer.CustomerId;
            target3.CustomerId = newCustomer.CustomerId;

           await repo.CreateAsync(target1);
           await repo.CreateAsync(target2);
           await repo.CreateAsync(target3);

            // Act
            var firstSingleResult =await repo.GetAllByCustomerIdAndPagedAsync(newCustomer.CustomerId, 1, 1);
            var secondSingleResult =await repo.GetAllByCustomerIdAndPagedAsync(newCustomer.CustomerId, 1, 2);
            var thirdSingleResult =await repo.GetAllByCustomerIdAndPagedAsync(newCustomer.CustomerId, 1, 3);
            var firstDoubleResult =await repo.GetAllByCustomerIdAndPagedAsync(newCustomer.CustomerId, 2, 1);
            var lastDoubleResult =await repo.GetAllByCustomerIdAndPagedAsync(newCustomer.CustomerId, 2, 2);

            // Assert
            firstSingleResult.Should().BeEquivalentTo(new List<Order> { target1 });
            secondSingleResult.Should().BeEquivalentTo(new List<Order> { target2 });
            thirdSingleResult.Should().BeEquivalentTo(new List<Order> { target3 });
            firstDoubleResult.Should().BeEquivalentTo(new List<Order> { target1, target2 });
            lastDoubleResult.Should().BeEquivalentTo(new List<Order> { target3 });
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndPagedAsync_InexistentCustomerId_ShouldReturnNull()
        {
            var custId = Generators.GenInexistentCustomerId();

            var result = await _repo.GetAllByCustomerIdAndPagedAsync(custId, 1, 1);

            result.Should().BeNull();
        }

        #endregion

        #region Sorts

        [DataTestMethod]
        [DataRow("ID")]
        [DataRow("id")]
        [DataRow("ORDERID")]
        [DataRow("ORDID")]
        [DataRow("OrdId")]
        [DataRow("orderId")]
        [DataRow("ORD_ID")]
        [DataRow("ord_id")]
        public async Task GetAllSortedAsync_ById_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.OrderId);

            var result = await _repo.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("CUST")]
        [DataRow("cust")]
        [DataRow("CUSTID")]
        [DataRow("custId")]
        [DataRow("CUSTOMER")]
        [DataRow("cuStoMer")]
        [DataRow("CUSTOMERID")]
        [DataRow("CustomerId")]
        public async Task GetAllSortedAsync_ByCustomerId_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.CustomerId);

            var result = await _repo.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("SHIP")]
        [DataRow("ShIp")]
        [DataRow("SHIPPING")]
        [DataRow("shipping")]
        [DataRow("SHIPPINGMETHOD")]
        [DataRow("shiPpingMethod")]
        [DataRow("SHIPMETHOD")]
        [DataRow("shIpMetHod")]
        [DataRow("DELIVERY")]
        [DataRow("deliVery")]
        [DataRow("DELIVERYMETHOD")]
        [DataRow("deLiVERyMethOd")]
        [DataRow("SHIPMETH")]
        [DataRow("ShipMetH")]
        [DataRow("SHIP_METH")]
        [DataRow("shIp_mEth")]
        [DataRow("SHIPPING_METHOD")]
        [DataRow("Shipping_method")]
        [DataRow("SHIP_METHOD")]
        [DataRow("sHip_meThod")]
        public async Task GetAllSortedAsync_ByShippingMethodId_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.ShippingMethodId);

            var result = await _repo.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("PAY")]
        [DataRow("pay")]
        [DataRow("PAYMENT")]
        [DataRow("paYmeNt")]
        [DataRow("PAYMETHOD")]
        [DataRow("PaYMethod")]
        [DataRow("PAYMENTMETHOD")]
        [DataRow("PaYMentMethod")]
        public async Task GetAllSortedAsync_ByPaymentMethodId_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.PaymentMethodId);

            var result = await _repo.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("ORDERDATE")]
        [DataRow("OrdErDate")]
        [DataRow("ORDER_DATE")]
        [DataRow("OrdeR_Date")]
        [DataRow("PLACED")]
        [DataRow("Placed")]
        public async Task GetAllSortedAsync_ByOrderDate_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.OrderDate);

            var result = await _repo.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("SHIPPINGDATE")]
        [DataRow("shippingDate")]
        [DataRow("SHIPPING_DATE")]
        [DataRow("shIPping_daTE")]
        [DataRow("SHIPDATE")]
        [DataRow("shIpDate")]
        [DataRow("SHIP_DATE")]
        [DataRow("ship_date")]
        [DataRow("SHIPPED")]
        [DataRow("shIpped")]
        public async Task GetAllSortedAsync_ByShippingDate_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.ShippingDate);

            var result = await _repo.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("STATUS")]
        [DataRow("StaTus")]
        [DataRow("STAT")]
        [DataRow("stat")]
        [DataRow("ORDERSTATUS")]
        [DataRow("OrderStatus")]
        [DataRow("ORDSTATUS")]
        [DataRow("ordStatus")]
        [DataRow("ORDER_STATUS")]
        [DataRow("order_status")]
        [DataRow("ORD_STARTUS")]
        [DataRow("ord_status")]
        [DataRow("ORDSTAT")]
        [DataRow("ordStat")]
        [DataRow("ORD_STAT")]
        [DataRow("ord_stat")]
        public async Task GetAllSortedAsync_ByOrderStatusId_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Orders.OrderBy(c => c.OrderStatusId);

            var result = await _repo.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("asdfok")]
        public async Task GetAllSortedAsync_BadQuery_ShouldReturnAll(string sortBy)
        {
            var expected = await _repo.GetAllAsync();

            var resultAsc = await _repo.GetAllSortedAsync(sortBy);
            var resultDes = await _repo.GetAllSortedAsync(sortBy, true);

            resultAsc.Should().BeEquivalentTo(expected);
            resultDes.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedAsync_Descending_ShouldSortDescending()
        {
            // Arrange
            var sortBy = "Shipmethod";
            var ascending = await _repo.GetAllSortedAsync(sortBy);
            var expected = ascending.Reverse();

            // Act
            var result = await _repo.GetAllSortedAsync(sortBy, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedAndPagedAsync_ShouldReturnRequestedAsync()
        {
            // Arrange
            var sortBy = "orderdate";
            var pgIndex = 3;
            var pgSize = 1;

            var expected = new List<Order> { _db.Orders.LastOrDefault() };

            // Act
            var result = await _repo.GetAllSortedAndPagedAsync(pgSize, pgIndex, sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            var sortBy = "shippingMethod";
            var pgIndex = 3;
            var pgSize = 1;

            var expected = new List<Order> { _db.Orders.FirstOrDefault() };

            // Act
            var result = await _repo.GetAllSortedAndPagedAsync(pgSize, pgIndex, sortBy, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            //// Arrange
            _db.BumpOrders();
            var searchBy = "cash";
            var sortBy = "ordDate";

            var expected = new List<Order> { _db.Orders.SingleOrDefault(o => o.OrderId == 2) };
            expected.AddRange(_db.Orders.Where(o => o.OrderId > 3).Reverse());

            // Act
            var result = await _repo.GetAllFilteredAndSortedAsync(searchBy, sortBy);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            //// Arrange

            _db.BumpOrders();

            // Regine is Cust with ID 2
            var searchBy = "Regine";
            // In regular Order: 2, 2, 1. Sorted Asc would reverse them, descending should leave them just as if it were a simple filter
            var sortBy = "shippingMethod";

            var expected = await _repo.GetAllFilteredAsync(searchBy);

            // Act
            var result = await _repo.GetAllFilteredAndSortedAsync(searchBy, sortBy, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpOrders();

            var repo = new TestCustomerRepo(_db);
            var expected = _db.Orders.Skip(3).Take(2).Reverse();

            // Act
            var result = await _repo.GetAllFilteredSortedAndPagedAsync("cash", 2, 2, "ordDate");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpOrders();

            var expected = new List<Order> { _db.Orders.SingleOrDefault(o => o.OrderId == 6), _db.Orders.SingleOrDefault(o => o.OrderId == 2) };

            // Act
            var result = await _repo.GetAllFilteredSortedAndPagedAsync("cash", 2, 2, "ordDate", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpOrders();

            var expected = new List<Order>
            {
                _db.Orders.SingleOrDefault(o=>o.OrderId == 6),
                _db.Orders.SingleOrDefault(o=>o.OrderId == 2),
                _db.Orders.SingleOrDefault(o=>o.OrderId == 4)
            };

            // Act
            var result = await _repo.GetAllByCustomerIdAndSortedAsync(2, "shipMethod");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpOrders();

            var expected = new List<Order>
            {
                _db.Orders.SingleOrDefault(o=>o.OrderId == 4),
                _db.Orders.SingleOrDefault(o=>o.OrderId == 2),
                _db.Orders.SingleOrDefault(o=>o.OrderId == 6)
            };

            // Act
            var result = await _repo.GetAllByCustomerIdAndSortedAsync(2, "shipMethod", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpOrders();

            var expected = new List<Order> { _db.Orders.SingleOrDefault(o => o.OrderId == 4) };

            // Act
            var result = await _repo.GetAllByCustomerIdSortedAndPagedAsync(2, 2, 2, "ordDate");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpOrders();

            var expected = new List<Order> { _db.Orders.SingleOrDefault(o => o.OrderId == 2) };

            // Act
            var result = await _repo.GetAllByCustomerIdSortedAndPagedAsync(2, 2, 2, "ordDate", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndFilteredAsync_HappyFlow_ShoulReturnequestedAsync()
        {
            // Arrange
            _db.BumpOrders();

            var expected = new List<Order> { _db.Orders.SingleOrDefault(o => o.OrderId == 4) };

            // Act
            var result = await _repo.GetAllByCustomerIdAndFilteredAsync(2, "UPS");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdFilteredAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpOrdersDouble();

            var expected = _db.Orders.SingleOrDefault(o => o.OrderId == 9);

            // Act
            var result = await _repo.GetAllByCustomerIdFilteredAndPagedAsync(2, "Credit Card", 2, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdFilteredAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpOrdersDouble();

            var expected = new List<Order>
                {
                    _db.Orders.SingleOrDefault(o=>o.OrderId == 7),
                    _db.Orders.SingleOrDefault(o=>o.OrderId == 8),
                    _db.Orders.SingleOrDefault(o=>o.OrderId == 2)
                };

            // Act
            var result = await _repo.GetAllByCustomerIdFilteredAndSortedAsync(2, "Delivered", "ordDate");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdFilteredAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpOrdersDouble();

            var expected = new List<Order>
                {
                    _db.Orders.SingleOrDefault(o=>o.OrderId == 9),
                    _db.Orders.SingleOrDefault(o=>o.OrderId == 7),
                    _db.Orders.SingleOrDefault(o=>o.OrderId == 8)
                };

            // Act
            var result = await _repo.GetAllByCustomerIdFilteredAndSortedAsync(2, "Credit Card", "stat", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdFilteredSortedAndPagedAsync()
        {
            // Arrange
            _db.BumpOrdersDouble();

            var expected = new List<Order>
            {
                _db.Orders.SingleOrDefault(o=>o.OrderId == 7),
                _db.Orders.SingleOrDefault(o=>o.OrderId == 8)
            };

            // Act
            var result = await _repo.GetAllByCustomerIdFilteredSortedAndPagedAsync(2, "delivered", 2, 1, "shipped");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdFilteredSortedDescendingAndPagedAsync()
        {
            // Arrange
            _db.BumpOrdersDouble();

            var expected = new List<Order>
            {
                _db.Orders.SingleOrDefault(o=>o.OrderId == 2),
                _db.Orders.SingleOrDefault(o=>o.OrderId == 8)
            };

            // Act
            var result = await _repo.GetAllByCustomerIdFilteredSortedAndPagedAsync(2, "Delivered", 2, 1, "shipped", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion Sorts

        #endregion

        #region IdVerifiers

        [TestMethod]
        public async Task CustomerIdExistsAsync_ItDoes_ShouldReturnTrue()
        {
            var id = _db.Customers.FirstOrDefault().CustomerId;

            var result = await _repo.CustomerIdExistsAsync(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task CustomerIdExistsAsync_ItDoesNot_ShouldReturnFalse()
        {
            var id = Generators.GenInexistentCustomerId();

            var result = await _repo.CustomerIdExistsAsync(id);

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task PaymentMethodIdExistsAsync_ItDoes_ShouldReturnTrue()
        {
            var id = _db.PaymentMethods.FirstOrDefault().PaymentMethodId;

            var result = await _repo.PaymentMethodIdExistsAsync(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task PaymentMethodIdExistsAsync_ItDoesNot_ShouldReturnFalse()
        {
            var id = _db.PaymentMethods.Count() + 1;

            var result = await _repo.PaymentMethodIdExistsAsync(id);

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task ShippingMethodIdExistsAsync_ItDoes_ShouldReturnTrue()
        {
            var id = _db.ShippingMethods.FirstOrDefault().ShippingMethodId;

            var result = await _repo.ShippingMethodIdExistsAsync(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task ShippingMethodIdExistsAsync_ItDoesNot_ShouldReturnFalse()
        {
            var id = _db.ShippingMethods.Count() + 1;

            var result = await _repo.ShippingMethodIdExistsAsync(id);

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task OrderStatusIdExistsAsync_ItDoes_ShouldReturnTrue()
        {
            var id = _db.OrderStatuses.FirstOrDefault().OrderStatusId;

            var result = await _repo.OrderStatusIdExistsAsync(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task OrderStatusIdExistsAsync_ItDoesNot_ShouldReturnFalse()
        {
            var id = _db.OrderStatuses.Count() + 1;

            var result = await _repo.OrderStatusIdExistsAsync(id);

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task OrderIdExistsAsync_ItDoes_ShouldReturnTrue()
        {
            var id = _db.Orders.FirstOrDefault().OrderId;

            var result = await _repo.OrderIdExistsAsync(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task OrderIdExistsAsync_ItDoesNot_ShouldReturnFalse()
        {
            var id = Generators.GenInexistentOrderId();

            var result = await _repo.OrderIdExistsAsync(id);

            result.Should().BeFalse();
        }

        #endregion

        #region StatusAdvancements

        [TestMethod]
        public async Task PromoteReceivedAsync_HappyFlow_ShouldAdvance()
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = 0;

            await _repo.CreateAsync(target);

            // Act
            await _repo.PromoteReceivedAsync(0);
            var result = (await _repo.GetAsync(0)).OrderStatusId;

            // Assert
            result.Should().Be(1);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public async Task PromoteReceivedAsync_WrongStatus_ShouldNotModify(int statId)
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = statId > 1 ? target.OrderDate.AddDays(2) : (DateTime?)null; ;
            target.OrderStatusId = statId;

            await _repo.CreateAsync(target);

            // Act
            await _repo.PromoteReceivedAsync(0);
            var result = await _repo.GetAsync(0);

            // Assert
            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public async Task PromotedReceivedAsync_InexistentId_ShouldNotModify()
        {
            // Arrange
            var badId = _db.Orders.Count() + 1;

            // Act
            await _repo.PromoteReceivedAsync(badId);
            var result = await _repo.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(_db.Orders);
        }

        [TestMethod]
        public async Task PromoteProcessedAsync_HappyFlow_ShouldAdvanceAndSetShipDate()
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = 1;

            await _repo.CreateAsync(target);

            var shipDate = target.OrderDate.AddDays(2);

            // Act
            await _repo.PromoteProcessedAsync(0, shipDate);
            var result = await _repo.GetAsync(0);

            // Assert
            result.OrderStatusId.Should().Be(2);
            result.ShippingDate.Should().Be(shipDate);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(2)]
        [DataRow(3)]
        public async Task PromoteProcessedAsync_WrongStatus_ShouldNotModify(int statId)
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = statId;

            await _repo.CreateAsync(target);

            var shipDate = target.OrderDate.AddDays(2);

            // Act
            await _repo.PromoteProcessedAsync(0, shipDate);
            var result = await _repo.GetAsync(0);

            // Assert
            result.OrderStatusId.Should().Be(statId);
            result.ShippingDate.Should().BeNull();
        }

        [TestMethod]
        public async Task PromoteProcessedAsync_BadDate_ShouldNotModify()
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = 1;

            await _repo.CreateAsync(target);

            var shipDate = target.OrderDate.AddDays(-2);

            // Act
            await _repo.PromoteProcessedAsync(0, shipDate);
            var result = await _repo.GetAsync(0);

            // Assert
            result.OrderStatusId.Should().Be(1);
            result.ShippingDate.Should().Be(null);
        }

        [TestMethod]
        public async Task PromoteProcessedAsync_InexistentId_ShouldNotModify()
        {
            // Arrange
            var shipDate = DateTime.Today;
            var badId = _db.Orders.Count() + 1;

            // Act
            await _repo.PromoteProcessedAsync(badId, shipDate);
            var result = await _repo.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(_db.Orders);
        }

        [TestMethod]
        public async Task BatchPromoteReceivedAsync_HappyFlow_ShouldAdvanceAll()
        {
            // Arrange
            _db.BumpReceivedOrders();
            var repo = new TestOrderRepo(_db);

            // Act
           await repo.BatchPromoteReceivedAsync(new List<int> { 4, 5, 6 });
            var result1 =(await repo.GetAsync(4)).OrderStatusId;
            var result2 =(await repo.GetAsync(5)).OrderStatusId;
            var result3 =(await repo.GetAsync(6)).OrderStatusId;

            // Assert
            result1.Should().Be(1);
            result2.Should().Be(1);
            result3.Should().Be(1);
        }

        [TestMethod]
        public async Task BatchPromoteReceivedAsync_PartiallyWrong_ShouldOnlyAdvanceReceivedAsync()
        {
            // Arrange
            _db.BumpReceivedOrders();
            var corrupts = _db.Orders.Where(o => o.OrderId > 4);
            foreach (var ord in corrupts)
                ord.OrderStatusId = 2;
            var repo = new TestOrderRepo(_db);

            // Act
           await repo.BatchPromoteReceivedAsync(new List<int> { 4, 5, 6 });
            var result1 =(await repo.GetAsync(4)).OrderStatusId;
            var result2 =(await repo.GetAsync(5)).OrderStatusId;
            var result3 =(await repo.GetAsync(6)).OrderStatusId;

            // Assert
            result1.Should().Be(1);
            result2.Should().Be(2);
            result3.Should().Be(2);
        }

        [TestMethod]
        public async Task BatchPromoteReceivedAsync_AllWrong_ShouldOnlyNotModify()
        {
            // Arrange
            _db.BumpReceivedOrders();
            var corrupts = _db.Orders.Where(o => o.OrderId > 3);
            foreach (var ord in corrupts)
                ord.OrderStatusId = 2;
            var repo = new TestOrderRepo(_db);

            // Act
           await repo.BatchPromoteReceivedAsync(new List<int> { 4, 5, 6 });
            var result1 =(await repo.GetAsync(4)).OrderStatusId;
            var result2 =(await repo.GetAsync(5)).OrderStatusId;
            var result3 =(await repo.GetAsync(6)).OrderStatusId;

            // Assert
            result1.Should().Be(2);
            result2.Should().Be(2);
            result3.Should().Be(2);
        }

        [TestMethod]
        public async Task BatchPromoteReceivedAsync_PartiallyWrongId_ShouldOnlyAdvanceExistent()
        {
            // Arrange
            _db.BumpReceivedOrders();
            var repo = new TestOrderRepo(_db);

            // Act
           await repo.BatchPromoteReceivedAsync(new List<int> { 4, 12, 13 });
            var result1 =(await repo.GetAsync(4)).OrderStatusId;
            var result2 =(await repo.GetAsync(5)).OrderStatusId;
            var result3 =(await repo.GetAsync(6)).OrderStatusId;

            // Assert
            result1.Should().Be(1);
            result2.Should().Be(0);
            result3.Should().Be(0);
        }

        [TestMethod]
        public async Task BatchPromoteReceivedAsync_IdsAllWrong_ShouldNotModify()
        {
            // Arrange
            _db.BumpReceivedOrders();
            var repo = new TestOrderRepo(_db);

            // Act
           await repo.BatchPromoteReceivedAsync(new List<int> { 11, 12, 13 });
            var result =await repo.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(_db.Orders);
        }

        #endregion

        #endregion
    }
}
