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
    public class OrderDataServiceTests
    {
        private IOrderDataService _service;
        private IOrderRepository _repo;
        private TestDb _db;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestOrderRepo(_db);
            _service = new OrderDataService(_repo);
        }

        #region Creates

        [TestMethod]
        public void Create_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenOrder();

            // Act
            var result = _service.Create(newItem, true);
            var createdItem = _service.Get(newItem.OrderId);

            // Assert
            result.Should().BeNull();
            createdItem.Should().BeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Create_CustomerDoesntExist_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var badCustId = Generators.GenInexistentCustomerId();

            var newItem = Generators.GenOrder();
            newItem.CustomerId = badCustId;

            // Act
            var result = _service.Create(newItem, true);
            var createdOrder = _service.Get(0);

            // Assert
            result.Trim().Should().Be("No Customer with ID: " + badCustId +
                " found in Repo. Orders require an existing Customer to be associated with.");
            createdOrder.Should().BeNull();
        }

        [TestMethod]
        public void Create_OrderedAfterToday_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.OrderDate = DateTime.Now.AddDays(20);

            // Act
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdOrder = _service.Get(0);

            // Assert
            result.Should().Be("Order Date too far into the future; Must be no later than right now.");
            createdOrder.Should().BeNull();
        }

        [TestMethod]
        public void Create_ShippedBeforeOrdered_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.OrderDate = DateTime.Now.AddDays(-3);
            newItem.ShippingDate = newItem.OrderDate.AddDays(-20);

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdOrder = _service.Get(0);

            // Assert
            result.Should().Be("Orders cannot be Shipped before being placed.");
            createdOrder.Should().BeNull();
        }

        [TestMethod]
        public void Create_InexistentShippingMethod_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.ShippingMethodId = _db.ShippingMethods.Count() + 1;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdOrder = _service.Get(0);

            // Assert
            result.Should().Be("Orders must have a Shipping Method associated with them.");
            createdOrder.Should().BeNull();
        }

        [TestMethod]
        public void Create_InexistentPaymentMethod_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.PaymentMethodId = _db.PaymentMethods.Count() + 1;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdOrder = _service.Get(0);

            // Assert
            result.Should().Be("Orders must have a Payment Method associated with them.");
            createdOrder.Should().BeNull();
        }

        [TestMethod]
        public void Create_InexistentOrderStatus_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.OrderStatusId = _db.OrderStatuses.Count() + 1;

            // Act
            var result = _service.Create(newItem, true).Trim().GetUntilOrEmpty(".");
            var createdOrder = _service.Get(0);

            // Assert
            result.Should().Be("Orders must have a Status associated with them.");
            createdOrder.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void Create_ShippedOrDeliveredWithNoDate_ShouldNotCreateAndReturnAppropriateError(int ordStat)
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.ShippingDate = null;
            newItem.OrderStatusId = ordStat;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdOrder = _service.Get(0);

            // Assert
            result.Should().Be("An Order's Shipping Date cannot be missing if it has been Shipped or Delivered.");
            createdOrder.Should().BeNull();
        }

        [TestMethod]
        public void Create_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        {
            var newItem = Generators.GenOrder();
            newItem.OrderDate = DateTime.Now.AddDays(3);
            newItem.ShippingDate = newItem.OrderDate.AddDays(-4);

            //// Act
            var result = _service.Create(newItem, true);
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").Trim().GetUntilOrEmpty(".");
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();
            var createdOrder = _service.Get(0);

            // Assert
            err1.Should().Be("Order Date too far into the future; Must be no later than right now.");
            err2.Should().Be("Orders cannot be Shipped before being placed.");
            blank.Should().BeEmpty();
            createdOrder.Should().BeNull();
        }

        #endregion

        #region Updates

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var oldItem = new Order
            {
                OrderId = target.OrderId,
                CustomerId = target.CustomerId,
                ShippingDate=target.ShippingDate,
                ShippingMethodId=target.ShippingMethodId,
                OrderStatusId=target.OrderStatusId,
                OrderDate=target.OrderDate,
                PaymentMethodId=target.PaymentMethodId
            };
            var targetId = target.OrderId;
            var newItem = Generators.GenOrder();
            newItem.OrderId = targetId;

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
            var targetId = Generators.GenInexistentOrderId();
            var newItem = Generators.GenOrder();
            newItem.OrderId = targetId;
            var oldRepo = _service.GetAll();

            // Act
            var result = _service.Update(newItem, true).Trim();
            var newRepo = _service.GetAll();

            // Assert
            result.Should().Be("Order ID must exist within the Repo for an update to be performed;"+ 
                " Order Data Service could not find any Order with ID: " + targetId + " in Repo.");
            newRepo.Should().BeEquivalentTo(oldRepo);
        }

        [TestMethod]
        public void Update_InexistentCustomerId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.CustomerId = Generators.GenInexistentCustomerId();

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedOrder = _service.Get(target.OrderId);

            // Assert
            result.Should().Be("No Customer with ID: " + newItem.CustomerId +
                " found in Repo. Orders require an existing Customer to be associated with.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_OrderAfterToday_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.ShippingDate = null;
            newItem.OrderStatusId = 0;
            newItem.OrderDate = DateTime.Now.AddDays(3);

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.OrderId);

            // Assert
            result.Should().Be("Order Date too far into the future; Must be no later than right now.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_ShippedBeforeOrdered_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.ShippingDate = newItem.OrderDate.AddDays(-3);
            newItem.OrderStatusId = 2;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.OrderId);

            // Assert
            result.Should().Be("Orders cannot be Shipped before being placed.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_InexistentShippingMethod_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.ShippingMethodId = _db.ShippingMethods.Count() + 1;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.OrderId);

            // Assert
            result.Should().Be("Orders must have a Shipping Method associated with them.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_InexistentPaymentMethod_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.PaymentMethodId = _db.PaymentMethods.Count() + 1;

            // Act
            var result = _service.Update(newItem, true).Trim().GetUntilOrEmpty(".");
            var updatedCustomer = _service.Get(target.OrderId);

            // Assert
            result.Should().Be("Orders must have a Payment Method associated with them.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_InexistentOrderStatus_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.OrderStatusId = _db.OrderStatuses.Count()+1;

            // Act
            var result = _service.Update(newItem, true).Trim().GetUntilOrEmpty(".");
            var updatedCustomer = _service.Get(target.OrderId);

            // Assert
            result.Should().Be("Orders must have a Status associated with them.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void Update_ShippedOrDeliveredWithNoDate_ShouldNotUpdateAndReturnAppropriateError(int ordStat)
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.ShippingDate = null;
            newItem.OrderStatusId = ordStat;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.OrderId);

            // Assert
            result.Should().Be("An Order's Shipping Date cannot be missing if it has been Shipped or Delivered.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.OrderDate = DateTime.Now.AddDays(3);
            newItem.ShippingDate = newItem.OrderDate.AddDays(-4);

            // Act
            var result = _service.Update(newItem, true);
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();
            var updatedCustomer = _service.Get(target.OrderId);

            // Assert
            err1.Should().Be("Order Date too far into the future; Must be no later than right now.");
            err2.Should().Be("Orders cannot be Shipped before being placed.");
            blank.Should().BeEmpty();
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region Gets

        [TestMethod]
        public void Get_HappyFlow_ShouldReturnTarget()
        {
            var target = _db.Orders.FirstOrDefault();

            var result = _service.Get(target.OrderId);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNull()
        {
            var id = Generators.GenInexistentOrderId();

            var result = _service.Get(id);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetAll_HappyFlow_ShouldFetchAll()
        {
            var result = _service.GetAll();

            result.Should().BeEquivalentTo(_db.Orders);
        }

        [TestMethod]
        public void GetAllUnprocessed_HappyFlow_ShouldFetchAllUnprocessed()
        {
            var result = _service.GetAllUnprocessed();

            result.Should().BeEquivalentTo(_db.Orders.Where(o=>o.OrderStatusId==0));
            result.Any(o => o.OrderStatusId != 0).Should().BeFalse();
        }

        [TestMethod]
        public void GetAllProcessed_HappyFlow_ShouldFetchAllProcessed()
        {
            var result = _service.GetAllProcessed();

            result.Should().BeEquivalentTo(_db.Orders.Where(o => o.OrderStatusId == 1));
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

        #region Filtered

        [TestMethod]
        public void GetAllFiltered_CustomerNameMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = _service.GetAllFiltered(_db.Customers.SingleOrDefault(c => c.CustomerId == targetItem.CustomerId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_ShippingMethodMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = _service.GetAllFiltered(_db.ShippingMethods.SingleOrDefault(s => s.ShippingMethodId == targetItem.ShippingMethodId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_PaymentMethodMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = _service.GetAllFiltered(_db.PaymentMethods.SingleOrDefault(p => p.PaymentMethodId == targetItem.PaymentMethodId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_OrderStatusMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = _service.GetAllFiltered(_db.OrderStatuses.SingleOrDefault(os => os.OrderStatusId == targetItem.OrderStatusId).Name);

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

            _db.Orders.Add(targetItem);

            var expected = new List<Order> { targetItem };

            // Act
            var result = _service.GetAllFiltered(targetItem.OrderDate.ToShortDateString());

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

            _service.Create(targetItem);

            var expected = new List<Order> { targetItem };

            // Act
            var result = _service.GetAllFiltered(targetItem.ShippingDate.GetValueOrDefault().ToShortDateString());

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
            _db.BumpOrders();

            var expected = _db.Orders.Skip(1).Take(1).Append(_db.Orders.Skip(4).FirstOrDefault());

            // Act
            var result = _service.GetAllFiltered("processed delivered");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_HappyFlow_ShouldReturnMatch()
        {
            var search = _db.Orders.FirstOrDefault().OrderDate.ToShortDateString();
            var expected = new List<Order> { _service.Get(2) };

            var result = _service.GetAllFilteredAndPaged(search, 1, 2);

            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region ByCustomerId

        [TestMethod]
        public void GetAllByCustomerId_HappyFlow_ShouldRetrieve()
        {
            // Arrange
            var oldItem = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.CustomerId = oldItem.CustomerId;
            _service.Create(newItem);
            var expected = new List<Order> { oldItem, newItem };

            // Act
            var result = _service.GetAllByCustomerId(newItem.CustomerId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerId_InexistentId_ShouldReturnEmpty()
        {
            var custId = Generators.GenInexistentCustomerId();

            var result = _service.GetAllByCustomerId(custId);

            result.Should().BeEmpty();
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

            var result = _service.GetAllByCustomerIdAndPaged(custId, 1, 1);

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

            var result = _service.GetAllSorted(sortBy);

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

            var result = _service.GetAllSorted(sortBy);

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

            var result = _service.GetAllSorted(sortBy);

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

            var result = _service.GetAllSorted(sortBy);

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

            var result = _service.GetAllSorted(sortBy);

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

            var result = _service.GetAllSorted(sortBy);

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
            var sortBy = "Shipmethod";
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
            var sortBy = "orderdate";
            var pgIndex = 3;
            var pgSize = 1;

            var expected = new List<Order> { _db.Orders.LastOrDefault() };

            // Act
            var result = _service.GetAllSortedAndPaged(pgSize, pgIndex, sortBy);

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
            var result = _service.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, true);

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
            expected.AddRange(_db.Orders.Where(o => o.OrderId > 3).Reverse());

            // Act
            var result = _service.GetAllFilteredAndSorted(searchBy, sortBy);

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

            var expected = _service.GetAllFiltered(searchBy);

            // Act
            var result = _service.GetAllFilteredAndSorted(searchBy, sortBy, true);

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
            var result = _service.GetAllFilteredSortedAndPaged("cash", 2, 2, "ordDate");

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
            var result = _service.GetAllFilteredSortedAndPaged("cash", 2, 2, "ordDate", true);

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
            var result = _service.GetAllByCustomerIdAndSorted(2, "shipMethod");

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
            var result = _service.GetAllByCustomerIdAndSorted(2, "shipMethod", true);

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
            var result = _service.GetAllByCustomerIdSortedAndPaged(2, 2, 2, "ordDate");

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
            var result = _service.GetAllByCustomerIdSortedAndPaged(2, 2, 2, "ordDate", true);

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
            var result = _service.GetAllByCustomerIdAndFiltered(2, "UPS");

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
            var result = _service.GetAllByCustomerIdFilteredAndPaged(2, "Credit Card", 2, 2);

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
            var result = _service.GetAllByCustomerIdFilteredAndSorted(2, "Delivered", "ordDate");

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
            var result = _service.GetAllByCustomerIdFilteredAndSorted(2, "Credit Card", "stat", true);

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
            var result = _service.GetAllByCustomerIdFilteredSortedAndPaged(2, "delivered", 2, 1, "shipped");

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
            var result = _service.GetAllByCustomerIdFilteredSortedAndPaged(2, "Delivered", 2, 1, "shipped", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion Sorts

        #endregion

        #region StatusAdvancements

        [TestMethod]
        public void PromoteReceived_HappyFlow_ShouldAdvance()
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = 0;

            _service.Create(target);

            // Act
            _service.PromoteReceived(0);
            var result = _service.Get(0).OrderStatusId;

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
            target.ShippingDate = statId > 1 ? target.OrderDate.AddDays(2) : (DateTime?)null; ;
            target.OrderStatusId = statId;

            _service.Create(target);

            // Act
            _service.PromoteReceived(0);
            var result = _service.Get(0);

            // Assert
            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public void PromotedReceived_InexistentId_ShouldNotModify()
        {
            // Arrange
            var badId = _db.Orders.Count() + 1;

            // Act
            _service.PromoteReceived(badId);
            var result = _service.GetAll();

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

            _service.Create(target);

            var shipDate = target.OrderDate.AddDays(2);

            // Act
            _service.PromoteProcessed(0, shipDate);
            var result = _service.Get(0);

            // Assert
            result.OrderStatusId.Should().Be(2);
            result.ShippingDate.Should().Be(shipDate);
        }

        //TODO
        [TestMethod]
        public void PromoteProcessed_JustReceived_ShouldNotModify()
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = 0;

            _service.Create(target);

            var shipDate = target.OrderDate.AddDays(2);

            // Act
            _service.PromoteProcessed(0, shipDate);
            var result = _service.Get(0);

            // Assert
            result.OrderStatusId.Should().Be(0);
            result.ShippingDate.Should().BeNull();
        }


        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void PromoteProcessed_AlreadyShippedOrDelivered_ShouldNotModify(int statId)
        {
            // Arrange
            var target = Generators.GenOrder();
            var initDate = target.OrderDate.AddDays(1);
            target.ShippingDate = initDate;
            target.OrderStatusId = statId;

            _service.Create(target);

            var reqDate = target.OrderDate.AddDays(2);

            // Act
            _service.PromoteProcessed(0, reqDate);
            var result = _service.Get(0);

            // Assert
            result.OrderStatusId.Should().Be(statId);
            result.ShippingDate.Should().Be(initDate);
        }

        [TestMethod]
        public void PromoteProcessed_BadDate_ShouldNotModify()
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = 1;

            _service.Create(target);

            var shipDate = target.OrderDate.AddDays(-2);

            // Act
            _service.PromoteProcessed(0, shipDate);
            var result = _service.Get(0);

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
            _service.PromoteProcessed(badId, shipDate);
            var result = _service.GetAll();

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
            repo.BatchPromoteReceived(new List<int> { 4, 12, 13 });
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

        #region Validates

        #region WholeOrder Validation

        [TestMethod]
        public void ValidateOrder_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenOrder();

            // Act
            var result = _service.ValidateOrder(newItem);

            // Assert
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateOrder_CustomerDoesntExist_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var badCustId = Generators.GenInexistentCustomerId();

            var newItem = Generators.GenOrder();
            newItem.CustomerId = badCustId;

            // Act
            var result = _service.ValidateOrder(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should()
                .Be("No Customer with ID: " + badCustId +
                " found in Repo.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateOrder_OrderedAfterToday_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.OrderDate = DateTime.Now.AddDays(20);

            // Act
            var result = _service.ValidateOrder(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("Order Date too far into the future; Must be no later than right now.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateOrder_ShippedBeforeOrdered_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.OrderDate = DateTime.Now.AddDays(-3);
            newItem.ShippingDate = newItem.OrderDate.AddDays(-20);

            // Act
            var result = _service.ValidateOrder(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Orders cannot be Shipped before being placed.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateOrder_InexistentShippingMethod_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.ShippingMethodId = _db.ShippingMethods.Count() + 1;

            // Act
            var result = _service.ValidateOrder(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Orders must have a Shipping Method associated with them.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateOrder_InexistentPaymentMethod_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.PaymentMethodId = _db.PaymentMethods.Count() + 1;

            // Act
            var result = _service.ValidateOrder(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Orders must have a Payment Method associated with them.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateOrder_InexistentOrderStatus_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.OrderStatusId = _db.OrderStatuses.Count() + 1;

            // Act
            var result = _service.ValidateOrder(newItem);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("Orders must have a Status associated with them.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void ValidateOrder_ShippedOrDeliveredWithNoDate_ShouldReturnFalseAndAppropriateError(int ordStat)
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.ShippingDate = null;
            newItem.OrderStatusId = ordStat;

            // Act
            var result = _service.ValidateOrder(newItem);

            // Assert
            result.errorList.Trim().Should().Be("An Order's Shipping Date cannot be missing if it has been Shipped or Delivered.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateOrder_MultipleErrors_ShouldReturnFalseAndAppropriateErrors()
        {
            var newItem = Generators.GenOrder();
            newItem.OrderDate = DateTime.Now.AddDays(3);
            newItem.ShippingDate = newItem.OrderDate.AddDays(-4);

            //// Act
            var result = _service.ValidateOrder(newItem);
            var err1 = result.errorList.Trim().GetUntilOrEmpty(".");
            var err2 = result.errorList.Replace(err1, "").Trim();
            var blank = result.errorList.Replace(err1, "").Replace(err2, "").Trim();

            // Assert
            err1.Should().Be("Order Date too far into the future; Must be no later than right now.");
            err2.Should().Be("Orders cannot be Shipped before being placed.");
            blank.Should().BeEmpty();
            result.isValid.Should().BeFalse();
        }

        #endregion

        #region PartialOrder Validation

        [TestMethod]
        public void ValidateCustomerId_ItDoesntExist_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var badCustId = Generators.GenInexistentCustomerId();

            // Act
            var result = _service.ValidateCustomerId(badCustId);

            // Assert
            result.errorList.Trim().Should().Be("No Customer with ID: " + badCustId + " found in Repo. Orders require an existing Customer to be associated with.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateCustomerId_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var custId = _db.Customers.FirstOrDefault().CustomerId;

            // Act
            var result = _service.ValidateCustomerId(custId);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateOrderDate_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-20);

            // Act
            var result = _service.ValidateOrderDate(date);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateOrderDate_AfterToday_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var date = DateTime.Now.AddDays(20);

            // Act
            var result = _service.ValidateOrderDate(date);

            // Assert
            result.errorList.Trim().GetUntilOrEmpty(".").Should().Be("Order Date too far into the future; Must be no later than right now.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateShippingDate_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var ordDate = DateTime.Now.AddDays(-3);
            var shipDate = ordDate.AddDays(20);

            // Act
            var result = _service.ValidateShippingDate(shipDate, ordDate);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateShippingDate_BeforeOrdered_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var ordDate = DateTime.Now.AddDays(-3);
            var shipDate = ordDate.AddDays(-20);

            // Act
            var result = _service.ValidateShippingDate(shipDate, ordDate);

            // Assert
            result.errorList.Trim().Should().Be("Orders cannot be Shipped before being placed.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateShippingMethodId_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var shipId = _db.ShippingMethods.FirstOrDefault().ShippingMethodId;

            // Act
            var result = _service.ValidateShippingMethodId(shipId);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateShippingMethodId_Inexistent_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var shipId = _db.ShippingMethods.Count() + 1;

            // Act
            var result = _service.ValidateShippingMethodId(shipId);

            // Assert
            result.errorList.Trim().Should().Be("Orders must have a Shipping Method associated with them.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePaymentMethodId_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var payMethId = _db.PaymentMethods.FirstOrDefault().PaymentMethodId;

            // Act
            var result = _service.ValidatePaymentMethodId(payMethId);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidatePaymentMethodId_Inexistent_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var payMethId = _db.PaymentMethods.Count() + 1;

            // Act
            var result = _service.ValidatePaymentMethodId(payMethId);

            // Assert
            result.errorList.Trim().Should().Be("Orders must have a Payment Method associated with them.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateOrderStatusId_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var statId = _db.OrderStatuses.FirstOrDefault().OrderStatusId;

            // Act
            var result = _service.ValidateOrderStatusId(statId);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateOrderStatusId_Inexistent_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var statId = _db.OrderStatuses.Count() + 1;

            // Act
            var result = _service.ValidateOrderStatusId(statId);

            // Assert
            result.errorList.Trim().Should().Be("Orders must have a Status associated with them.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void ValidateShipWithStatus_ShippedOrDeliveredWithNoDate_ShouldReturnFalseAndAppropriateError(int ordStat)
        {
            // Arrange
            var shipDate = (DateTime?)null;
            var statId = ordStat;

            // Act
            var result = _service.ValidateShipWithStatus(shipDate, statId);

            // Assert
            result.errorList.Trim().Should().Be("An Order's Shipping Date cannot be missing if it has been Shipped or Delivered.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void ValidateShipWithStatus_HappyFlow_ShouldReturnTrueAndEmpty(int ordStat)
        {
            // Arrange
            var shipDate = DateTime.Now.AddDays(-20);
            var statId = ordStat;

            // Act
            var result = _service.ValidateShipWithStatus(shipDate, statId);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        #endregion

        #endregion

        #region LeftOvers

        //[DataTestMethod]
        ////[DataRow(null, 1, 1, 1, false)]
        ////[DataRow(1, null, 1, 1, false)]
        ////[DataRow(1, 1, null, 1, false)]
        //[DataRow(1, 1, 1, null, false)]
        //[DataRow(1, 1, 1, 1, true)]
        //public void Update_Nulls_ShouldNotUpdateAndReturnAppropriateError(int custId, int payId, int shipMethId, int ordStatId, bool dateNull)
        //{
        //    // Arrange
        //    var target = _db.Orders.FirstOrDefault();
        //    var newItem = new Order
        //    {
        //        CustomerId = custId,
        //        PaymentMethodId = payId,
        //        ShippingMethodId = shipMethId,
        //        OrderStatusId = ordStatId,
        //        OrderDate = (dateNull ? (DateTime?)DateTime.Now : null).GetAsyncValueOrDefault()
        //};
        //    newItem.OrderId = target.OrderId;

        //    // Act
        //    var result = _service.Update(newItem, true).Trim();
        //    var updatedCustomer = _service.GetAsync(target.OrderId);

        //    // Assert
        //    //result.Should().Be("Orders must have a Status associated with them.");
        //    updatedCustomer.Should().BeEquivalentTo(target);
        //    updatedCustomer.Should().NotBeEquivalentTo(newItem);
        //}
        #endregion

        #region Async

        #region Creates

        [TestMethod]
        public async Task CreateAsync_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenOrder();

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdItem = await _service.GetAsync(newItem.OrderId);

            // Assert
            result.Should().BeNull();
            createdItem.Should().BeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task CreateAsync_CustomerDoesntExist_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var badCustId = Generators.GenInexistentCustomerId();

            var newItem = Generators.GenOrder();
            newItem.CustomerId = badCustId;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdOrder = await _service.GetAsync(0);

            // Assert
            result.Trim().Should().Be("No Customer with ID: " + badCustId + 
                " found in Repo. Orders require an existing Customer to be associated with.");
            createdOrder.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_OrderedAfterToday_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.OrderDate = DateTime.Now.AddDays(20);

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdOrder = await _service.GetAsync(0);

            // Assert
            result.Trim().GetUntilOrEmpty(".").Should()
                .Be("Order Date too far into the future; Must be no later than right now.");
            createdOrder.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_ShippedBeforeOrderedAsync_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.OrderDate = DateTime.Now.AddDays(-3);
            newItem.ShippingDate = newItem.OrderDate.AddDays(-20);

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdOrder = await _service.GetAsync(0);

            // Assert
            result.Trim().Should().Be("Orders cannot be Shipped before being placed.");
            createdOrder.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_InexistentShippingMethod_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.ShippingMethodId = _db.ShippingMethods.Count() + 1;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdOrder = await _service.GetAsync(0);

            // Assert
            result.Trim().Should().Be("Orders must have a Shipping Method associated with them.");
            createdOrder.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_InexistentPaymentMethod_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.PaymentMethodId = _db.PaymentMethods.Count() + 1;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdOrder = await _service.GetAsync(0);

            // Assert
            result.Trim().GetUntilOrEmpty(".").Should().Be("Orders must have a Payment Method associated with them.");
            createdOrder.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_InexistentOrderStatus_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.OrderStatusId = _db.OrderStatuses.Count() + 1;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdOrder = await _service.GetAsync(0);

            // Assert
            result.Trim().GetUntilOrEmpty(".").Should().Be("Orders must have a Status associated with them.");
            createdOrder.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public async Task CreateAsync_ShippedOrDeliveredWithNoDate_ShouldNotCreateAndReturnAppropriateError(int ordStat)
        {
            // Arrange
            var newItem = Generators.GenOrder();
            newItem.ShippingDate = null;
            newItem.OrderStatusId = ordStat;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdOrder = await _service.GetAsync(0);

            // Assert
            result.Trim().Should()
                .Be("An Order's Shipping Date cannot be missing if it has been Shipped or Delivered.");
            createdOrder.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        {
            var newItem = Generators.GenOrder();
            newItem.OrderDate = DateTime.Now.AddDays(3);
            newItem.ShippingDate = newItem.OrderDate.AddDays(-4);

            //// Act
            var result = await _service.CreateAsync(newItem, true);
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").Trim().GetUntilOrEmpty(".");
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();
            var createdOrder = await _service.GetAsync(0);

            // Assert
            err1.Should().Be("Order Date too far into the future; Must be no later than right now.");
            err2.Should().Be("Orders cannot be Shipped before being placed.");
            blank.Should().BeEmpty();
            createdOrder.Should().BeNull();
        }

        #endregion

        #region Updates

        [TestMethod]
        public async Task UpdateAsync_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var oldItem = new Order
            {
                OrderId = target.OrderId,
                CustomerId = target.CustomerId,
                ShippingDate = target.ShippingDate,
                ShippingMethodId = target.ShippingMethodId,
                OrderStatusId = target.OrderStatusId,
                OrderDate = target.OrderDate,
                PaymentMethodId = target.PaymentMethodId
            };
            var targetId = target.OrderId;
            var newItem = Generators.GenOrder();
            newItem.OrderId = targetId;

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
            var targetId = Generators.GenInexistentOrderId();
            var newItem = Generators.GenOrder();
            newItem.OrderId = targetId;
            var oldRepo = await _service.GetAllAsync();

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var newRepo = await _service.GetAllAsync();

            // Assert
            result.Trim().Should().Be("Order ID must exist within the Repo for an update to be performed;"+
                " Order Data Service could not find any Order with ID: " + targetId + " in Repo.");
            newRepo.Should().BeEquivalentTo(oldRepo);
        }

        [TestMethod]
        public async Task UpdateAsync_InexistentCustomerId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.CustomerId = Generators.GenInexistentCustomerId();

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedOrder = await _service.GetAsync(target.OrderId);

            // Assert
            result.Trim().Should().Be("No Customer with ID: " + newItem.CustomerId + 
                " found in Repo. Orders require an existing Customer to be associated with.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_OrderAfterToday_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.ShippingDate = null;
            newItem.OrderStatusId = 0;
            newItem.OrderDate = DateTime.Now.AddDays(3);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.OrderId);

            // Assert
            result.Trim().Should().Be("Order Date too far into the future; Must be no later than right now.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_ShippedBeforeOrderedAsync_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.ShippingDate = newItem.OrderDate.AddDays(-3);
            newItem.OrderStatusId = 2;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.OrderId);

            // Assert
            result.Trim().Should().Be("Orders cannot be Shipped before being placed.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_InexistentShippingMethod_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.ShippingMethodId = _db.ShippingMethods.Count() + 1;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.OrderId);

            // Assert
            result.Trim().Should().Be("Orders must have a Shipping Method associated with them.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_InexistentPaymentMethod_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.PaymentMethodId = _db.PaymentMethods.Count() + 1;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.OrderId);

            // Assert
            result.Trim().Should().Be("Orders must have a Payment Method associated with them.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        //TODO
        [TestMethod]
        public async Task UpdateAsync_InexistentOrderStatus_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.OrderStatusId = _db.OrderStatuses.Count() + 1;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.OrderId);

            // Assert
            result.Trim().Should().Be("Orders must have a Status associated with them.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public async Task UpdateAsync_ShippedOrDeliveredWithNoDate_ShouldNotUpdateAndReturnAppropriateError(int ordStat)
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.ShippingDate = null;
            newItem.OrderStatusId = ordStat;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.OrderId);

            // Assert
            result.Trim().Should().Be("An Order's Shipping Date cannot be missing if it has been Shipped or Delivered.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var target = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.OrderDate = DateTime.Now.AddDays(3);
            newItem.ShippingDate = newItem.OrderDate.AddDays(-4);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();
            var updatedCustomer = await _service.GetAsync(target.OrderId);

            // Assert
            err1.Should().Be("Order Date too far into the future; Must be no later than right now.");
            err2.Should().Be("Orders cannot be Shipped before being placed.");
            blank.Should().BeEmpty();
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region Gets

        [TestMethod]
        public async Task GetAsync_HappyFlow_ShouldReturnTarget()
        {
            var target = _db.Orders.FirstOrDefault();

            var result = await _service.GetAsync(target.OrderId);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public async Task GetAsync_InexistentId_ShouldReturnNull()
        {
            var id = Generators.GenInexistentOrderId();

            var result = await _service.GetAsync(id);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAllAsync_HappyFlow_ShouldFetchAll()
        {
            var result = await _service.GetAllAsync();

            result.Should().BeEquivalentTo(_db.Orders);
        }

        [TestMethod]
        public async Task GetAllUnprocessedAsync_HappyFlow_ShouldFetchAllUnprocessedAsync()
        {
            var result = await _service.GetAllUnprocessedAsync();

            result.Should().BeEquivalentTo(_db.Orders.Where(o => o.OrderStatusId == 0));
            result.Any(o => o.OrderStatusId != 0).Should().BeFalse();
        }

        [TestMethod]
        public async Task GetAllProcessedAsync_HappyFlow_ShouldFetchAllProcessedAsync()
        {
            var result = await _service.GetAllProcessedAsync();

            result.Should().BeEquivalentTo(_db.Orders.Where(o => o.OrderStatusId == 1));
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
            var allRecords = await _service.GetAllAsync();
            var overIndex = allRecords.Count() + 10;

            var result = await _service.GetAllPaginatedAsync(1, overIndex);

            result.Should().BeEmpty();
        }


        #endregion

        #region Filtered

        [TestMethod]
        public async Task GetAllFilteredAsync_CustomerNameMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = await _service.GetAllFilteredAsync(_db.Customers.SingleOrDefault(c => c.CustomerId == targetItem.CustomerId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_ShippingMethodMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = await _service.GetAllFilteredAsync(_db.ShippingMethods.SingleOrDefault(s => s.ShippingMethodId == targetItem.ShippingMethodId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_PaymentMethodMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = await _service.GetAllFilteredAsync(_db.PaymentMethods.SingleOrDefault(p => p.PaymentMethodId == targetItem.PaymentMethodId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_OrderStatusMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Orders.FirstOrDefault();
            var expected = new List<Order> { targetItem };

            var result = await _service.GetAllFilteredAsync(_db.OrderStatuses.SingleOrDefault(os => os.OrderStatusId == targetItem.OrderStatusId).Name);

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

            _db.Orders.Add(targetItem);

            var expected = new List<Order> { targetItem };

            // Act
            var result = await _service.GetAllFilteredAsync(targetItem.OrderDate.ToShortDateString());

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

            await _service.CreateAsync(targetItem);

            var expected = new List<Order> { targetItem };

            // Act
            var result = await _service.GetAllFilteredAsync(targetItem.ShippingDate.GetValueOrDefault().ToShortDateString());

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
            _db.BumpOrders();

            var expected = _db.Orders.Skip(1).Take(1).Append(_db.Orders.Skip(4).FirstOrDefault());

            // Act
            var result = await _service.GetAllFilteredAsync("processed delivered");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_HappyFlow_ShouldReturnMatch()
        {
            var search = _db.Orders.FirstOrDefault().OrderDate.ToShortDateString();
            var expected = new List<Order> { await _service.GetAsync(2) };

            var result = await _service.GetAllFilteredAndPagedAsync(search, 1, 2);

            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region ByCustomerId

        [TestMethod]
        public async Task GetAllByCustomerIdAsync_HappyFlow_ShouldRetrieve()
        {
            // Arrange
            var oldItem = _db.Orders.FirstOrDefault();
            var newItem = Generators.GenOrder();
            newItem.CustomerId = oldItem.CustomerId;
            await _service.CreateAsync(newItem);
            var expected = new List<Order> { oldItem, newItem };

            // Act
            var result = await _service.GetAllByCustomerIdAsync(newItem.CustomerId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAsync_InexistentId_ShouldReturnEmpty()
        {
            var custId = Generators.GenInexistentCustomerId();

            var result = await _service.GetAllByCustomerIdAsync(custId);

            result.Should().BeEmpty();
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
            var firstSingleResult = await repo
                .GetAllByCustomerIdAndPagedAsync(newCustomer.CustomerId, 1, 1);
            var secondSingleResult = await repo
                .GetAllByCustomerIdAndPagedAsync(newCustomer.CustomerId, 1, 2);
            var thirdSingleResult = await repo
                .GetAllByCustomerIdAndPagedAsync(newCustomer.CustomerId, 1, 3);
            var firstDoubleResult = await repo
                .GetAllByCustomerIdAndPagedAsync(newCustomer.CustomerId, 2, 1);
            var lastDoubleResult = await repo
                .GetAllByCustomerIdAndPagedAsync(newCustomer.CustomerId, 2, 2);

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

            var result = await _service.GetAllByCustomerIdAndPagedAsync(custId, 1, 1);

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

            var result = await _service.GetAllSortedAsync(sortBy);

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

            var result = await _service.GetAllSortedAsync(sortBy);

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

            var result = await _service.GetAllSortedAsync(sortBy);

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

            var result = await _service.GetAllSortedAsync(sortBy);

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

            var result = await _service.GetAllSortedAsync(sortBy);

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

            var result = await _service.GetAllSortedAsync(sortBy);

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

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("asdfok")]
        public async Task GetAllSortedAsync_BadQuery_ShouldReturnAll(string sortBy)
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
            var sortBy = "Shipmethod";
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
            var sortBy = "orderdate";
            var pgIndex = 3;
            var pgSize = 1;

            var expected = new List<Order> { _db.Orders.LastOrDefault() };

            // Act
            var result = await _service.GetAllSortedAndPagedAsync(pgSize, pgIndex, sortBy);

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
            var result = await _service.GetAllSortedAndPagedAsync(pgSize, pgIndex, sortBy, true);

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
            var result = await _service.GetAllFilteredAndSortedAsync(searchBy, sortBy);

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

            var expected = await _service.GetAllFilteredAsync(searchBy);

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(searchBy, sortBy, true);

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
            var result = await _service.GetAllFilteredSortedAndPagedAsync("cash", 2, 2, "ordDate");

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
            var result = await _service.GetAllFilteredSortedAndPagedAsync("cash", 2, 2, "ordDate", true);

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
            var result = await _service.GetAllByCustomerIdAndSortedAsync(2, "shipMethod");

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
            var result = await _service.GetAllByCustomerIdAndSortedAsync(2, "shipMethod", true);

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
            var result = await _service.GetAllByCustomerIdSortedAndPagedAsync(2, 2, 2, "ordDate");

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
            var result = await _service.GetAllByCustomerIdSortedAndPagedAsync(2, 2, 2, "ordDate", true);

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
            var result = await _service.GetAllByCustomerIdAndFilteredAsync(2, "UPS");

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
            var result = await _service.GetAllByCustomerIdFilteredAndPagedAsync(2, "Credit Card", 2, 2);

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
            var result = await _service.GetAllByCustomerIdFilteredAndSortedAsync(2, "Delivered", "ordDate");

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
            var result = await _service.GetAllByCustomerIdFilteredAndSortedAsync(2, "Credit Card", "stat", true);

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
            var result = await _service.GetAllByCustomerIdFilteredSortedAndPagedAsync(2, "delivered", 2, 1, "shipped");

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
            var result = await _service.GetAllByCustomerIdFilteredSortedAndPagedAsync(2, "Delivered", 2, 1, "shipped", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion Sorts

        #endregion
        //TODO
        #region StatusAdvancements

        [TestMethod]
        public async Task PromoteReceivedAsync_HappyFlow_ShouldAdvance()
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = 0;

            await _service.CreateAsync(target);

            // Act
            await _service.PromoteReceivedAsync(0);
            var result = (await _service.GetAsync(0)).OrderStatusId;

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

            await _service.CreateAsync(target);

            // Act
            await _service.PromoteReceivedAsync(0);
            var result = await _service.GetAsync(0);

            // Assert
            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public async Task PromotedReceivedAsync_InexistentId_ShouldNotModify()
        {
            // Arrange
            var badId = _db.Orders.Count() + 1;

            // Act
            await _service.PromoteReceivedAsync(badId);
            var result = await _service.GetAllAsync();

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

            await _service.CreateAsync(target);

            var shipDate = target.OrderDate.AddDays(2);

            // Act
            await _service.PromoteProcessedAsync(0, shipDate);
            var result = await _service.GetAsync(0);

            // Assert
            result.OrderStatusId.Should().Be(2);
            result.ShippingDate.Should().Be(shipDate);
        }

        //TODO
        [TestMethod]
        public async Task PromoteProcessedAsync_JustReceived_ShouldNotModify()
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = 0;

            await _service.CreateAsync(target);

            var shipDate = target.OrderDate.AddDays(2);

            // Act
            await _service.PromoteProcessedAsync(0, shipDate);
            var result = await _service.GetAsync(0);

            // Assert
            result.OrderStatusId.Should().Be(0);
            result.ShippingDate.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public async Task PromoteProcessedAsync_AlreadyShippedOrDelivered_ShouldNotModify(int statId)
        {
            // Arrange
            var target = Generators.GenOrder();
            var initDate = target.OrderDate.AddDays(1);
            target.ShippingDate = initDate;
            target.OrderStatusId = statId;

            await _service.CreateAsync(target);

            var reqDate = target.OrderDate.AddDays(2);

            // Act
            await _service.PromoteProcessedAsync(0, reqDate);
            var result = await _service.GetAsync(0);

            // Assert
            result.OrderStatusId.Should().Be(statId);
            result.ShippingDate.Should().Be(initDate);
        }

        [TestMethod]
        public async Task PromoteProcessedAsync_BadDate_ShouldNotModify()
        {
            // Arrange
            var target = Generators.GenOrder();
            target.ShippingDate = null;
            target.OrderStatusId = 1;

            await _service.CreateAsync(target);

            var shipDate = target.OrderDate.AddDays(-2);

            // Act
            await _service.PromoteProcessedAsync(0, shipDate);
            var result = await _service.GetAsync(0);

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
            await _service.PromoteProcessedAsync(badId, shipDate);
            var result = await _service.GetAllAsync();

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
            var result1 = (await repo.GetAsync(4)).OrderStatusId;
            var result2 = (await repo.GetAsync(5)).OrderStatusId;
            var result3 = (await repo.GetAsync(6)).OrderStatusId;

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
            var result1 = (await repo.GetAsync(4)).OrderStatusId;
            var result2 = (await repo.GetAsync(5)).OrderStatusId;
            var result3 = (await repo.GetAsync(6)).OrderStatusId;

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
            var result1 = (await repo.GetAsync(4)).OrderStatusId;
            var result2 = (await repo.GetAsync(5)).OrderStatusId;
            var result3 = (await repo.GetAsync(6)).OrderStatusId;

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
            var result1 = (await repo.GetAsync(4)).OrderStatusId;
            var result2 = (await repo.GetAsync(5)).OrderStatusId;
            var result3 = (await repo.GetAsync(6)).OrderStatusId;

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
            var result = await repo.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(_db.Orders);
        }

        #endregion

        #endregion
    }
}
