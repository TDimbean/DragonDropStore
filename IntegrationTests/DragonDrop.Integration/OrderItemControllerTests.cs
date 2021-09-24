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
    public class OrderItemControllerTests
    {
        private string _routePrefix;
        private HttpClient _client;
        private JsonMediaTypeFormatter _formatter;

        [TestInitialize]
        public void Init()
        {
            _routePrefix = "http://localhost:10957/api/orderitems";

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
            var fetchedItems = result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            fetchedItems.Should().NotBeEmpty();
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldReturnOkAndRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.FirstOrDefault();

            // Act
            var res = _client.GetAsync(_routePrefix + "?ordId=" + expected.OrderId + "&prodId=" + expected.ProductId).Result;
            var result = res.Content.ReadAsAsync<OrderItem>().Result;

            // Assert
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNotFound()
        {
            // Arrange
            (int ordId, int prodId) ids = (0,0) ;
            while (true)
            {
                if (!_client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                    .Any(i => i.OrderId == ids.ordId && i.ProductId == ids.prodId)) break;
                ids = ((new Random()).Next(0, int.MaxValue), (new Random()).Next(0, int.MaxValue));
            }

            // Act
            var result = _client.GetAsync(_routePrefix + "?ordId=" + ids.ordId + "&prodId=" + ids.prodId).Result.StatusCode;

            // Assert
            result.Should().Be(HttpStatusCode.NotFound);
        }

        #region Creates

        [TestMethod]
        public void Create_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrderItem();
            while (true)
            {
                if (_client.GetAsync(_routePrefix + "?ordId=" + newItem.OrderId + "&prodId=" + newItem.ProductId).Result.StatusCode == HttpStatusCode.OK)
                    newItem = StaticGenerator.GenOrderItem();
                else break;
            }

            // Act
            var result = _client.PostAsync(_routePrefix, newItem, _formatter).Result.StatusCode;
            var fetchedItem = _client.GetAsync(_routePrefix+"?ordId="+newItem.OrderId+"&prodId="+newItem.ProductId).Result.Content
                .ReadAsAsync<OrderItem>().Result;

            // Assert
            result.Should().Be(HttpStatusCode.OK);
            fetchedItem.Should().BeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(null)]
        public void Create_QuantityZero_ShouldNotCreateAndReturnAppropriateError(int qty)
        {
            // Arrange
            var newItem = StaticGenerator.GenOrderItem();
            newItem.Quantity = qty;

            // Act
            var result = _client.PostAsync(_routePrefix+"?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("OrderItem must have a quantity greater than 0.");
        }

        [TestMethod]
        public void Create_InexistentOrderId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrderItem();
            newItem.OrderId = 4537;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("OrderItem needs to target a valid Order Id.");
        }

        [TestMethod]
        public void Create_InexistentProductId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrderItem();
            newItem.ProductId = 9875;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("OrderItem needs to target a valid Product Id.");
        }

        [TestMethod]
        public void Create_Duplicate_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var oldItem = _client.GetAsync(_routePrefix + "?returnErrors=true").Result.Content
                .ReadAsAsync<IEnumerable<OrderItem>>().Result.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = oldItem.OrderId,
                ProductId = oldItem.ProductId,
                Quantity = oldItem.Quantity + 1
            };

            // Act
            var result = _client.PostAsync(_routePrefix+"?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("OrderItem already exists. If you wish to change the Quantity, please use the Update.");
        }

        [TestMethod]
        public void Create_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrderItem();
            newItem.OrderId = 67565;
            newItem.ProductId = 978654;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();

            // Assert
            err1.Should().Be("OrderItem needs to target a valid Order Id.");
            err2.Should().Be("OrderItem needs to target a valid Product Id.");
            blank.Should().BeEmpty();
        }

        #endregion

        #region Updates

        [TestMethod]
        public void Update_HappyFlow_ShouldReturnOk()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrderItem();
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.LastOrDefault();
            var savedItem = new OrderItem
            {
                OrderId = targetItem.OrderId,
                ProductId = targetItem.ProductId,
                Quantity = targetItem.Quantity
            };

            newItem.OrderId = targetItem.OrderId;
            newItem.ProductId = targetItem.ProductId;

            // Act
            var result = _client.PutAsync(_routePrefix, newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + targetItem.OrderId 
                + "&prodId="+targetItem.ProductId).Result.Content.ReadAsAsync<OrderItem>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("\"OrderItem successfully updated.\"");
            fetchedItem.Should().BeEquivalentTo(newItem);
            fetchedItem.Should().NotBeEquivalentTo(targetItem);

            // CleanUp
            var repair = _client.PutAsync(_routePrefix, targetItem, _formatter).Result;
            repair.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [DataTestMethod]
        public void Update_InexistentEntry_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenOrderItem();
            while (true)
            {
                if (_client.GetAsync(_routePrefix + "?ordId=" + newItem.OrderId + "&prodId=" + newItem.ProductId).Result.StatusCode == HttpStatusCode.OK)
                    newItem = StaticGenerator.GenOrderItem();
                else break;
            }
            var oldRepo = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Act
            var result = _client.PutAsync(_routePrefix+"?returnErrors=true", newItem, _formatter).Result.Content
                .ReadAsAsync<HttpError>().Result.Message;
            var newRepo = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().Be("\nOrder Item with Order/Product IDs: " + newItem.OrderId + "/" + newItem.ProductId + " was not found.");
            newRepo.Should().BeEquivalentTo(oldRepo);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(null)]
        public void Update_QuantityZero_ShouldNotUpdateAndReturnAppropriateError(int qty)
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = target.OrderId,
                ProductId = target.ProductId,
                Quantity = qty
            };

            // Act
            var result = _client.PutAsync(_routePrefix+"?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + target.OrderId + "&prodId=" + target.ProductId).Result.Content
                .ReadAsAsync<OrderItem>().Result;
                
            // Assert
            result.Should().Be("OrderItem must have a quantity greater than 0.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_InexistentOrderId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = 9879,
                ProductId = target.ProductId,
                Quantity = target.Quantity+1
            };

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content
                .ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".");
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + target.OrderId + "&prodId=" + target.ProductId).Result.Content
                .ReadAsAsync<OrderItem>().Result;

            // Assert
            result.Should().Be("OrderItem needs to target a valid Order Id.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_InexistentProductId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = target.OrderId,
                ProductId = 84357,
                Quantity = target.Quantity + 1
            };

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".");
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + target.OrderId + "&prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<OrderItem>().Result;


            // Assert
            result.Should().Be("OrderItem needs to target a valid Product Id.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.FirstOrDefault();
            var newItem = new OrderItem
            {
                OrderId = 54678,
                ProductId = 84357,
                Quantity = target.Quantity + 1
            };

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").Trim().GetUntilOrEmpty(".");
            var err3 = result.Replace(err1, "").Replace(err2, "").Trim().GetUntilOrEmpty(".");
            var blank = result.Replace(err1, "").Replace(err2, "").Replace(err3, "").Trim();
            var fetchedItem = _client.GetAsync(_routePrefix + "?ordId=" + target.OrderId + "&prodId=" + target.ProductId).Result.Content
                .ReadAsAsync<OrderItem>().Result;

            // Assert
            err1.Should().Be("OrderItem needs to target a valid Order Id.");
            err2.Should().Be("OrderItem needs to target a valid Product Id.");
            err3.Should().Be("Order Item with Order/Product IDs: " + newItem.OrderId + "/" + newItem.ProductId + " was not found.");
            blank.Should().BeEmpty();
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region Gets

        #region Paginated

        [TestMethod]
        public void GetAllPaginated_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=2&pgIndex=2").Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_FullPage_ShouldReturnRightSize()
        {
            var result = _client.GetAsync(_routePrefix + "?pgSize=7&pgIndex=2").Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            result.Count().Should().Be(7);
        }

        [TestMethod]
        public void GetAllPaginated_PartialPage_ShouldReturnRightSize()
        {
            // Arrange
            var recordsCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.Count();
            var pgSize = 1;
            var pgIndex = 0;
            var expected = 0;

            while (true)
            {
                expected = recordsCount % pgSize;
                if (expected == 0) pgSize++;
                else  break;
            }

            pgIndex = recordsCount / pgSize + 1;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=" + pgIndex)
                .Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.Count();

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void GetAllPaginated_SizeTooBig_ShouldReturnAll()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;
            var oversizedPage = expected.Count() + 10;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=" + oversizedPage + "&pgIndex=1").Result.Content
                .ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_IndexTooHigh_ShouldReturnEmpty()
        {
            var overIndex = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.Count() + 10;

            var result = _client.GetAsync(_routePrefix + "?pgSize=1&pgIndex=" + overIndex).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllPaginated_SizeZero_ShouldReturnOkAndAll()
        {
            var result = _client.GetAsync(_routePrefix + "?pgSize=0&pgIndex=12").Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllPaginated_IndexZero_ShouldReturnOkAndAll()
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            var result = _client.GetAsync(_routePrefix + "?pgSize=12&pgIndex=0").Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(-1, 2)]
        [DataRow(1, -2)]
        public void GetAllPaginated_NegativeNumbers_ShouldConvertToPositive(int pgSize, int pgIndex)
        {
            var expected = _client.GetAsync(_routePrefix + "?pgSize=" + Math.Abs(pgSize) + "&pgIndex=" + Math.Abs(pgIndex)).Result
                .Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=" + pgIndex).Result
                .Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region ByIds

        [TestMethod]
        public void GetAllByOrderId_HappyFlow_ShouldReturnMatches()
        {
            // Arrange
            var targetOrderId = _client.GetAsync(_routePrefix.Replace("orderitems", "orders")).Result.Content.ReadAsAsync<IEnumerable<Order>>()
                .Result.FirstOrDefault().OrderId;
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Where(i => i.OrderId == targetOrderId);

            // Act
            var result = _client.GetAsync(_routePrefix + "?ordId=" + targetOrderId).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByOrderId_InexistentId_ShouldReturnNull()
        {
            // Arrange
            var ordId = 4765767;
            while (true)
            {
                if (_client.GetAsync(_routePrefix.Replace("orderitems", "orders") + "?ordId=" + ordId).Result.StatusCode == HttpStatusCode.OK)
                    ordId = (new Random()).Next(1, int.MaxValue);
                else break;
            }

            // Act
            var result = _client.GetAsync(_routePrefix + "?ordId=" + ordId).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetAllByProductId_HappyFlow_ShouldReturnMatches()
        {
            // Arrange
            var targetProductId = _client.GetAsync(_routePrefix.Replace("orderitems", "products")).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.FirstOrDefault().ProductId;
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Where(i => i.ProductId == targetProductId);

            // Act
            var result = _client.GetAsync(_routePrefix + "?prodId=" + targetProductId).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByProduct_InexistentId_ShouldReturnNull()
        {
            // Arrange
            var prodId = 4765767;
            while (true)
            {
                if (_client.GetAsync(_routePrefix.Replace("orderitems", "products") + "?prodId=" + prodId).Result.StatusCode == HttpStatusCode.OK)
                    prodId = (new Random()).Next(1, int.MaxValue);
                else break;
            }

            // Act
            var result = _client.GetAsync(_routePrefix + "?prodId=" + prodId).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetAllByOrderIdAndPaged_HappyFlow_ShouldReturnMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Skip(4).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix+"?ordId=1&pgSize=2&pgIndex=3").Result.Content
                .ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByProductIdAndPaged_HappyFlow_ShouldReturnMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Where(i => i.ProductId == 58).Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?prodId=58&pgSize=2&pgIndex=2").Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

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
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.OrderBy(i => i.OrderId);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

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
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.OrderBy(i => i.ProductId);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("QUANTITY")]
        [DataRow("quantITy")]
        [DataRow("QTY")]
        [DataRow("qty")]
        public void GetAllSorted_ByQuantity_ShouldReturnRequested(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result.OrderBy(i => i.Quantity);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("asdf")]
        [DataRow(null)]
        public void GetAllSorted_BadQuery_ShouldReturnAll(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSorted_Descending_ShoudReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>()
                .Result.OrderByDescending(i => i.Quantity);

            // Act
            var result = _client.GetAsync(_routePrefix + "?sortBy=ordId&desc=true").Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedAndPaged_HappyFlow_ShoudReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Skip(6).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=2&pgIndex=4&sortBy=ordId").Result.Content
                .ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescendingAndPaged_HappyFlow_ShoudReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .OrderByDescending(i => i.ProductId).Skip(6).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=2&pgIndex=4&sortBy=prodId&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByOrderIdAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Where(i => i.OrderId == 1).OrderBy(i => i.Quantity);

            // Act
            var result = _client.GetAsync(_routePrefix + "?ordId=1&sortBy=qty").Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByOrderIdAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Where(i => i.OrderId == 1).OrderByDescending(i => i.Quantity);

            // Act
            var result = _client.GetAsync(_routePrefix + "?ordId=1&sortBy=qty&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByProductIdAndSorted_HappyFlow_ShouldReturnRequested()
        { 
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Where(i => i.ProductId == 58).OrderBy(i => i.Quantity);

            // Act
            var result = _client.GetAsync(_routePrefix + "?prodId=58&sortBy=qty").Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByProductIdAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Where(i => i.ProductId == 58).OrderByDescending(i => i.Quantity);

            // Act
            var result = _client.GetAsync(_routePrefix + "?prodId=58&sortBy=qty&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByOrderIdSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Where(i => i.OrderId == 1).OrderBy(i => i.Quantity).Skip(4).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?ordId=1&pgSize=2&pgIndex=3&sortBy=qty").Result.Content
                .ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByOrderIdSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Where(i => i.OrderId == 1).OrderByDescending(i => i.Quantity).Skip(4).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?ordId=1&pgSize=2&pgIndex=3&sortBy=qty&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByProductIdSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Where(i => i.ProductId==58).OrderBy(i => i.Quantity).Skip(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?prodId=58&pgSize=2&pgIndex=2&sortBy=qty").Result.Content
                .ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByProductIdSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<OrderItem>>().Result
                .Where(i => i.ProductId == 58).OrderByDescending(i => i.Quantity).Skip(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?prodId=58&pgSize=2&pgIndex=2&sortBy=qty&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<OrderItem>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #endregion

    }
}
