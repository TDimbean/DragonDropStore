using DragonDrop.Integration.Entities;
using DragonDrop.Integration.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace DragonDrop.Integration
{
    [TestClass]
    public class OrderControllerTests
    {
        private string _routePrefix;
        private HttpClient _client;
        private JsonMediaTypeFormatter _formatter;

        [TestInitialize]
        public void Init()
        {
            _routePrefix = "http://localhost:10957/api/orders";

            _formatter = new JsonMediaTypeFormatter();
            _formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            _formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            _client = new HttpClient();
        }

        [TestMethod]
        public void GetAll_HappyFlow_ShouldReturnAllRecords()
        {
            var result = _client.GetAsync(_routePrefix).Result;
            var fetchedItems = result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            fetchedItems.Should().NotBeEmpty();
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldReturnOkAndRequested()
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.FirstOrDefault();

            var result = _client.GetAsync(_routePrefix + "?ordId=" + expected.OrderId).Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsAsync<Order>().Result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNotFound()
        {
            // Arrange
            var id = 0;
            while (true)
            {
                if (!_client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Any(c => c.OrderId == id)) break;
                id = (new Random()).Next(0, int.MaxValue);
            }

            // Act
            var result = _client.GetAsync(_routePrefix + "?ordId=" + id).Result.StatusCode;

            // Assert
            result.Should().Be(HttpStatusCode.NotFound);
        }
     
        #region Creates

        [TestMethod]
        public void Create_HappyFlow_ShouldReturnOk()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrder();
            var idlessNew = new
            {
                newItem.OrderDate,
                newItem.CustomerId,
                newItem.OrderStatusId,
                newItem.PaymentMethodId,
                newItem.ShippingDate,
                newItem.ShippingMethodId
            };

            // Act
            var result = _client.PostAsync(_routePrefix, newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.LastOrDefault();
            var idlessFetch = new
            {
                fetchedItem.OrderDate,
                fetchedItem.CustomerId,
                fetchedItem.OrderStatusId,
                fetchedItem.PaymentMethodId,
                fetchedItem.ShippingDate,
                fetchedItem.ShippingMethodId
            };

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("\"Order successfully registered.\"");
            idlessFetch.Should().BeEquivalentTo(idlessNew);
        }

        [TestMethod]
        public void Create_CustomerDoesntExist_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var badCustId = 0;
            while (true)
            {
                if (_client.GetAsync("http://localhost:10957/api/customers?custId=" + badCustId).Result.StatusCode == HttpStatusCode.OK)
                    badCustId = (new Random()).Next(1, int.MaxValue);
                else break;
            }

            var newItem = StaticGenerator.GenOrder();
            newItem.CustomerId = badCustId;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("No Customer with ID: " + badCustId + " found in Repo. Orders require an existing Customer to be associated with.");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_OrderedAfterToday_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrder();
            newItem.OrderDate = DateTime.Now.AddDays(20);

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").
                Should().Be("Order Date too far into the future; Must be no later than right now.");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_ShippedBeforeOrdered_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrder();
            newItem.OrderDate = DateTime.Now.AddDays(-3);
            newItem.ShippingDate = newItem.OrderDate.AddDays(-20);


            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Orders cannot be Shipped before being placed.");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_InexistentShippingMethod_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrder();
            newItem.ShippingMethodId = 10;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Orders must have a Shipping Method associated with them.");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_InexistentPaymentMethod_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrder();
            newItem.PaymentMethodId = 10;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Orders must have a Payment Method associated with them.");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_InexistentOrderStatus_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrder();
            newItem.OrderStatusId = 2135;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Orders must have a Status associated with them.");
            newCount.Should().Be(initCount);
        }

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void Create_ShippedOrDeliveredWithNoDate_ShouldReturnBadRequestAndAppropriateError(int ordStat)
        {
            // Arrange
            var newItem = StaticGenerator.GenOrder();
            newItem.ShippingDate = null;
            newItem.OrderStatusId = ordStat;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("An Order's Shipping Date cannot be missing if it has been Shipped or Delivered.");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_MultipleErrors_ShouldReturnBadRequestAndAppropriateErrors()
        {
            //Arrange
            var newItem = StaticGenerator.GenOrder();
            newItem.OrderDate = DateTime.Now.AddDays(3);
            newItem.ShippingDate = newItem.OrderDate.AddDays(-4);

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var resMsg = result.Content.ReadAsAsync<HttpError>().Result.Message;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();
            var err1 = resMsg.GetUntilOrEmpty(".");
            var err2 = resMsg.Replace(err1, "").Trim().GetUntilOrEmpty(".");
            var blank = resMsg.Replace(err1, "").Replace(err2, "").Trim();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            err1.Should().Be("Order Date too far into the future; Must be no later than right now.");
            err2.Should().Be("Orders cannot be Shipped before being placed.");
            blank.Should().BeEmpty();
            newCount.Should().Be(initCount);
        }

        #endregion

        #region Updates

        [TestMethod]
        public void Update_HappyFlow_ShouldReturnOkAndUpdate()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrder();
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.LastOrDefault();
            var savedItem = new Order
            {
                OrderId = targetItem.OrderId,
                CustomerId = targetItem.CustomerId,
                ShippingDate = targetItem.ShippingDate,
                ShippingMethodId = targetItem.ShippingMethodId,
                OrderStatusId = targetItem.OrderStatusId,
                OrderDate = targetItem.OrderDate,
                PaymentMethodId = targetItem.PaymentMethodId
            };

            newItem.OrderId = targetItem.OrderId;

            // Act
            var result = _client.PutAsync(_routePrefix, newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + targetItem.OrderId).Result.Content.ReadAsAsync<Order>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("\"Order successfully updated.\"");
            fetchedItem.Should().BeEquivalentTo(newItem);
            fetchedItem.Should().NotBeEquivalentTo(targetItem);

            // CleanUp
            var repair = _client.PutAsync(_routePrefix, targetItem, _formatter).Result;
            repair.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public void Update_InexistentId_ShouldReturnBadRequest()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrder();
            var id = 0;
            while (true)
            {
                if (_client.GetAsync(_routePrefix + "?ordId=" + id).Result.StatusCode == HttpStatusCode.OK) id = (new Random()).Next(0, int.MaxValue);
                break;
            }
            newItem.OrderId = id;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + newItem.OrderId)
                .Result.Content.ReadAsAsync<Order>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message
                .GetUntilOrEmpty(".").Trim().Should().Be("Order ID must exist within the Repo for an update to be performed; Order Data Service could not find any Order with ID: " + id + " in Repo.");
            fetchedItem.Should().BeNull();
        }

        [TestMethod]
        public void Update_InexistentCustomerId_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenOrder();
            newItem.OrderId = target.OrderId;

            var badCustId = 0;
            while (true)
            {
                if (_client.GetAsync("http://localhost:10957/api/customers?custId=" + badCustId).Result.StatusCode == HttpStatusCode.OK) badCustId = (new Random()).Next(0, int.MaxValue);
                break;
            }
            newItem.CustomerId = badCustId;

            // Act
            var result = _client.PutAsync(_routePrefix+"?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + newItem.OrderId)
                .Result.Content.ReadAsAsync<Order>().Result;

            // Assert
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("No Customer with ID: " + newItem.CustomerId 
                + " found in Repo. Orders require an existing Customer to be associated with.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_OrderAfterToday_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.ShippingDate = null;
            newItem.OrderStatusId = 0;
            newItem.OrderDate = DateTime.Now.AddDays(3);

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + target.OrderId)
                .Result.Content.ReadAsAsync<Order>().Result;

            // Assert
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Order Date too far into the future; Must be no later than right now.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_ShippedBeforeOrdered_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.ShippingDate = newItem.OrderDate.AddDays(-3);
            newItem.OrderStatusId = 2;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + target.OrderId)
                .Result.Content.ReadAsAsync<Order>().Result;

            // Assert
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Orders cannot be Shipped before being placed.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_InexistentShippingMethod_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.ShippingMethodId = 10;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + target.OrderId)
                .Result.Content.ReadAsAsync<Order>().Result;

            // Assert
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Orders must have a Shipping Method associated with them.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_InexistentPaymentMethod_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.PaymentMethodId = 3456;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + target.OrderId)
                .Result.Content.ReadAsAsync<Order>().Result;

            // Assert
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Orders must have a Payment Method associated with them.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_InexistentOrderStatus_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.OrderStatusId = 10;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + target.OrderId)
                .Result.Content.ReadAsAsync<Order>().Result;

            // Assert
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Orders must have a Status associated with them.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void Update_ShippedOrDeliveredWithNoDate_ShouldReturnBadRequestAndAppropriateError(int ordStat)
        {
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.ShippingDate = null;
            newItem.OrderStatusId = ordStat;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + target.OrderId)
                .Result.Content.ReadAsAsync<Order>().Result;

            // Assert
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("An Order's Shipping Date cannot be missing if it has been Shipped or Delivered.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_MultipleErrors_ShouldReturnBadRequestAndAppropriateErrors()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenOrder();
            newItem.OrderId = target.OrderId;
            newItem.OrderDate = DateTime.Now.AddDays(3);
            newItem.ShippingDate = newItem.OrderDate.AddDays(-4);


            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + target.OrderId)
                .Result.Content.ReadAsAsync<Order>().Result;

            // Assert
            err1.Should().Be("Order Date too far into the future; Must be no later than right now.");
            err2.Should().Be("Orders cannot be Shipped before being placed.");
            blank.Should().BeEmpty();
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region QueriedGets

        #region ByCustomerId

        [TestMethod]
        public void GetAllByCustomerId_HappyFlow_ShouldRetrieve()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            var targetCustId = allRecords.FirstOrDefault().CustomerId;
            var expected = allRecords.Where(x => x.CustomerId == targetCustId);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=" + targetCustId).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerId_InexistentId_ShouldReturnEmpty()
        {
            // Arrange
            var custId = 0;
            while (true)
            {
                if (_client.GetAsync("http://localhost:10957/api/customers?custId=" + custId).Result.StatusCode == HttpStatusCode.OK)
                    custId = (new Random()).Next(1, int.MaxValue);
                else break;
            }

            // Act
            var result = _client.GetAsync(_routePrefix+"?custId="+custId).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllByCustomerIdAndPaged_HappyFlow_ShouldReturnDesired()
        {
            // Arrange
            var targets = _client.GetAsync(_routePrefix + "?custId=32").Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            var target1 = targets.Take(1);
            var target2 = targets.Skip(1).Take(1);
            var target3 = targets.Skip(2).Take(2);

            // Act
            var firstSingleResult = _client.GetAsync(_routePrefix + "?custId=32&pgSize=1&pgIndex=1")
                .Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            var secondSingleResult = _client.GetAsync(_routePrefix + "?custId=32&pgSize=1&pgIndex=2")
                .Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            
            var firstDoubleResult = _client.GetAsync(_routePrefix + "?custId=32&pgSize=2&pgIndex=1")
                .Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            var lastDoubleResult = _client.GetAsync(_routePrefix + "?custId=32&pgSize=2&pgIndex=2")
                .Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            firstSingleResult.Should().BeEquivalentTo( target1 );
            secondSingleResult.Should().BeEquivalentTo( target2 );
            firstDoubleResult.Should().BeEquivalentTo(target1.Concat(target2));
            lastDoubleResult.Should().BeEquivalentTo(target3 );
        }

        #endregion

        #region Filtered

        [TestMethod]
        public void GetAllFiltered_CustomerNameMatch_ShouldReturnMatch()
        {
            // Arrange
            var expected= _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Take(1);
            var name = _client.GetAsync("http://localhost:10957/api/customers?custId=" 
                + expected.SingleOrDefault().CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result.Name;

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + name).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_ShippingMethodMatch_ShouldReturnMatch()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            var expected = allRecords.Where(x => x.ShippingMethodId == 2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=DHL").Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);

        }

        [TestMethod]
        public void GetAllFiltered_PaymentMethodMatch_ShouldReturnMatch()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            var expected = allRecords.Where(x => x.PaymentMethodId == 2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=Cash").Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_OrderStatusMatch_ShouldReturnMatch()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            var expected = allRecords.Where(x => x.OrderStatusId == 2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=shipped").Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_OrderDateMatch_ShouldReturnMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + expected.SingleOrDefault().OrderDate.ToShortDateString())
                .Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_ShippingDateMatch_ShouldReturnMatch()
        {
            // Arrange
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x=>x.ShippingDate!=null).FirstOrDefault();
            var expected = new List<Order> { targetItem };

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + targetItem.ShippingDate.GetValueOrDefault().ToShortDateString())
                .Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_InexistentTerm_ShouldReturnEmpty()
        {
            var searchBy = Guid.NewGuid().ToString();

            var result = _client.GetAsync(_routePrefix + "?searchBy=" + searchBy).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllFiltered_DifTermsForDifResults_ShoudReturnBothMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" +
                expected.FirstOrDefault().OrderDate.ToShortDateString() + " " +
                expected.LastOrDefault().OrderDate.ToShortDateString())
                .Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_HappyFlow_ShouldReturnMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x => x.OrderStatusId == 0).Skip(4).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=received&pgSize=2&pgIndex=3").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
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
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

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
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.OrderBy(x=>x.CustomerId);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

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
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.OrderBy(x => x.ShippingMethodId);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

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
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.OrderBy(x => x.PaymentMethodId);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

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
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.OrderBy(x => x.OrderDate);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

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
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.OrderBy(x => x.ShippingDate);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

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
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.OrderBy(x => x.OrderStatusId);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("asdfok")]
        public void GetAllSorted_BadQuery_ShouldReturnAll(string sortBy)
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Act
            var resultAsc = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            var resultDes = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy+"&desc=true").Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            resultAsc.Should().BeEquivalentTo(expected);
            resultDes.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSorted_Descending_ShouldSortDescending()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>()
                .Result.OrderByDescending(x => x.ShippingMethodId);

            // Act
            var result = _client.GetAsync(_routePrefix + "?sortBy=shipMeth&desc=true").Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedAndPaged_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>()
                .Result.Skip(4).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=2&pgIndex=3&sortBy=payMeth").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var allEntries = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            var expected = allEntries.Skip(11).Take(1).Append(allEntries.Skip(13).FirstOrDefault());

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=2&pgIndex=2&sortBy=ordStat&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x=>x.OrderStatusId==0).OrderBy(x=>x.OrderDate);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=received&sortBy=placed").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x => x.OrderStatusId == 0).OrderByDescending(x => x.OrderDate);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=received&sortBy=placed&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var allEntries = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            var expected = allEntries.Skip(1).Take(1).Append(allEntries.Skip(3).FirstOrDefault());

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=fedEx&pgSize=2&pgIndex=1&sortBy=ordStat")
                .Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>()
                .Result.Skip(15).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=delivered&pgSize=2&pgIndex=1&sortBy=shipMeth&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x => x.CustomerId == 32).Reverse();

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=32&sortBy=ordStat").Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
        
        [TestMethod]
        public void GetAllByCustomerIdAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x => x.CustomerId == 32).Reverse();

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=32&sortby=placed&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x => x.CustomerId == 32).LastOrDefault();

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=32&pgSize=1&pgIndex=1&sortby=ordStat").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x => x.CustomerId == 32).LastOrDefault();

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=32&pgSize=3&pgIndex=2&sortby=shipDate&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndFiltered_HappyFlow_ShoulReturnequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x => x.OrderId == 1001 || x.OrderId == 3002);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=32&searchBy=DHL").Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x=>x.CustomerId==1&&x.PaymentMethodId==3).Skip(2).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=1&searchBy=payPal&pgSize=2&pgIndex=2").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x => x.CustomerId == 1 && x.PaymentMethodId == 3).Reverse();

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=1&searchBy=payPal&sortBy=ordStat").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x => x.CustomerId==1 && x.PaymentMethodId == 3).OrderByDescending(x=>x.OrderStatusId);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=1&searchBy=payPal&sortBy=ordStat&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredSortedAndPaged()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x => x.CustomerId == 1 && x.PaymentMethodId == 3).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=1&searchBy=payPal&pgSize=2&pgIndex=2&sortBy=ordStat").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredSortedDescendingAndPaged()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result
                .Where(x => x.CustomerId == 1 && x.PaymentMethodId == 3).Skip(2).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=1&searchBy=payPal&pgSize=2&pgIndex=2&sortBy=ordStat&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region Paginated

        [TestMethod]
        public void GetAllPaginated_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=2&pgIndex=2").Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_FullPage_ShouldReturnRightSize()
        {
            var pgSize = 7;

            var result = _client.GetAsync(_routePrefix + "?pgSize="+pgSize+"&pgIndex=3").Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            result.Count().Should().Be(pgSize);
        }

        [TestMethod]
        public void GetAllPaginated_PartialPage_ShouldReturnRightSize()
        {
            // Arrange
            var recordsCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();
            var pgSize = 1;
            var pgIndex = 0;
            var expected = 0;

            while (true)
            {
                expected = recordsCount % pgSize;
                if (expected == 0) pgSize++;
                else break;
            }

            pgIndex = recordsCount / pgSize + 1;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=" + pgIndex)
                .Result.Content.ReadAsAsync<IEnumerable<Order>>().Result.Count();

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void GetAllPaginated_SizeTooBig_ShouldReturnOkAndAll()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;
            var pgSize = expected.Count() + 4;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=1").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_IndexTooHigh_ShouldReturnOkAndEmpty()
        {
            // Arrange
            var pgIndex = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result.Count()+10;

            // Act
            var result = _client.GetAsync(_routePrefix+"?pgSize=1&pgIndex="+pgIndex).Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllPaginated_SizeZero_ShouldReturnOkAndEmpty()
        {
            var result = _client.GetAsync(_routePrefix + "?pgSize=0&pgIndex=12").Result.Content.ReadAsAsync<IEnumerable<Order>>().Result;

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllPaginated_IndexZero_ShouldReturnOkAndAll()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=12&pgIndex=0").Result.Content
                .ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(-1, 2)]
        [DataRow(1, -2)]
        public void GetAllPaginated_NegativeNumbers_ShouldConvertToPositive(int pgSize, int pgIndex)
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix + "?pgSize=" + Math.Abs(pgSize) + "&pgIndex=" + Math.Abs(pgIndex)).Result
                .Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=" + pgIndex).Result
                .Content.ReadAsAsync<IEnumerable<Order>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #endregion
    }
}

