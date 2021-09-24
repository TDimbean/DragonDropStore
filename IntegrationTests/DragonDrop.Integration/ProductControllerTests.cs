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
    public class ProductControllerTests
    {
        private string _routePrefix;
        private HttpClient _client;
        private JsonMediaTypeFormatter _formatter;

        [TestInitialize]
        public void Init()
        {
            _routePrefix = "http://localhost:10957/api/products";

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
            var fetchedItems = result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            fetchedItems.Should().NotBeEmpty();
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldReturnOkAndRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .FirstOrDefault();

            // Act
            var result = _client.GetAsync(_routePrefix + "?prodId=" + expected.ProductId).Result
                .Content.ReadAsAsync<Product>().Result;

            // Assert
            //result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNotFound()
        {
            // Arrange
            var id = 0;
            while (true)
            {
                if (!_client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.Any(p => p.ProductId == id)) break;
                id = (new Random()).Next(0, int.MaxValue);
            }

            // Act
            var result = _client.GetAsync(_routePrefix + "?prodId=" + id).Result.StatusCode;

            // Assert
            result.Should().Be(HttpStatusCode.NotFound);
        }

        #region Creates

        [TestMethod]
        public void Create_HappyFlow_ShouldReturnOk()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();

            // Act
            var result = _client.PostAsync(_routePrefix, newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.LastOrDefault();
            newItem.ProductId = fetchedItem.ProductId;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("\"Product successfully registered.\"");
            fetchedItem.Should().BeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Create_InexistentCategoryId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.CategoryId = 6576;

            // Act
            var result = _client.PostAsync(_routePrefix+"?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Products require a valid Category to be placed under\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
        }

        [TestMethod]
        public void Create_NullCategoryId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.CategoryId = null;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Products require a valid Category to be placed under\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
        }

        [TestMethod]
        public void Create_NameTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.Name = StaticGenerator.GenRandomString(51);

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Product Name limited to 50 characters.");
        }

        [TestMethod]
        public void Create_DescriptionTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.Description = StaticGenerator.GenRandomString(301);

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Product Description is limited to 300 characters.");
        }

        [TestMethod]
        public void Create_NegativeStock_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.Stock = -3;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Product Stock may not be negative.");
        }

        [TestMethod]
        public void Create_NegativeUnitPrice_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.UnitPrice = -3m;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
        }

        [TestMethod]
        public void Create_UnitPriceZero_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.UnitPrice = 0m;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
        }

        [TestMethod]
        public void Create_NullUnitPrice_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.UnitPrice = null;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
        }

        [TestMethod]
        public void Create_NullBarCode_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.BarCode = null;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Products require a Barcode.");
        }

        [TestMethod]
        public void Create_DuplicateBarCode_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.BarCode = _client.GetAsync(_routePrefix + "?prodId=1").Result.Content
                .ReadAsAsync<Product>().Result.BarCode;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Products must have a unique Bar Code.");
        }

        [DataTestMethod]
        [DataRow("12345665432")]
        [DataRow("1234566543210")]
        public void Create_WrongLengthBarCode_ShouldNotCreateAndReturnAppropriateError(string barCode)
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.BarCode = barCode;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Product Barcode must be precisely 12 characters long.");
        }

        [TestMethod]
        public void Create_BarCodeHasLetters_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.BarCode = "12345665432a";

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Product Barcode must be made up entirely of digits.");
        }

        [TestMethod]
        public void Create_BarCodeDoesNotCheckOut_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.BarCode = StaticGenerator.GenBarCode(null, false);

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
        }

        [TestMethod]
        public void Create_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        {
            // Arrange
            var newItem = StaticGenerator.GenProduct();
            newItem.Name = null;
            newItem.Description = StaticGenerator.GenRandomString(301);

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var err1 = result.GetUntilOrEmpty(".").Trim();
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").GetUntilOrEmpty(".").Trim();

            // Assert
            err1.Should().Be("Products require a name.");
            err2.Should().Be("Product Description is limited to 300 characters.");
            blank.Should().BeEmpty();
        }

        [TestMethod]
        public void Create_BadFlowNoErrorRequest_ShouldReturnBadRequestWithStandardError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.State = null;
            newItem.City = null;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix, newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Product registration failed.");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            newCount.Should().Be(initCount);
        }

        #endregion

        #region Updates

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var savedItem = new Product
            {
                Stock = targetItem.Stock,
                BarCode = targetItem.BarCode,
                CategoryId = targetItem.CategoryId,
                Description = targetItem.Description,
                Name = targetItem.Name,
                UnitPrice = targetItem.UnitPrice
            };
            var targetId = targetItem.ProductId;
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = targetId;

            // Act
            var result = _client.PutAsync(_routePrefix+"?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + targetItem.ProductId).Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("\"Product successfully updated.\"");
            fetchedItem.Should().BeEquivalentTo(newItem);
            fetchedItem.Should().NotBeEquivalentTo(targetItem);

            // CleanUp
            var repair = _client.PutAsync(_routePrefix, targetItem, _formatter).Result;
            repair.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public void Update_InexistentId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var targetId = 0;
            while (true)
            {
                if (_client.GetAsync(_routePrefix + "?prodId=" + targetId).Result.StatusCode == HttpStatusCode.OK)
                    targetId = (new Random()).Next(1, int.MaxValue);
                else break;
            }

            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = targetId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true",newItem,_formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Product Data Service could not find any Product with ID:\t" + targetId + " in Repo.");
        }

        [TestMethod]
        public void Update_InexistentCategoryId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.CategoryId = 4378;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix+"?prodId="+target.ProductId).Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.Should().Be("Products require a valid Category to be placed under\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NullCategoryId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.CategoryId = null;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId).Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.Should().Be("Products require a valid Category to be placed under\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void Update_NoName_ShouldNotUpdateAndReturnAppropriateError(string name)
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Name = name;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;


            // Assert
            result.Should().Be("Products require a name.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NameTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Name = StaticGenerator.GenRandomString(51);

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;


            // Assert
            result.Should().Be("Product Name limited to 50 characters.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_DescriptionTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Description = StaticGenerator.GenRandomString(301);

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.Should().Be("Product Description is limited to 300 characters.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NegativeStock_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Stock = -3;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.Should().Be("Product Stock may not be negative.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NegativeUnitPrice_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.UnitPrice = -3m;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_UnitPriceZero_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.UnitPrice = 0m;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NullUnitPrice_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.UnitPrice = null;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NullBarCode_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = null;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.Should().Be("Products require a Barcode.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_DuplicateBarCode_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = _client.GetAsync(_routePrefix + "?prodId=5").Result.Content
                .ReadAsAsync<Product>().Result.BarCode;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.Should().Be("Products must have a unique Bar Code.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("12345665432")]
        [DataRow("1234566543210")]
        public void Update_WrongLengthBarCode_ShouldNotUpdateAndReturnAppropriateError(string barCode)
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = barCode;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.Should().Be("Product Barcode must be precisely 12 characters long.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_BarCodeHasLetters_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = "12345665432a";

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.Should().Be("Product Barcode must be made up entirely of digits.");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_BarCodeDoesNotCheckOut_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = StaticGenerator.GenBarCode(null, false);

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            result.Should().Be("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var target = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.FirstOrDefault();
            var newItem = StaticGenerator.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Name = null;
            newItem.Description = StaticGenerator.GenRandomString(301);

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var err1 = result.GetUntilOrEmpty(".").Trim();
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").GetUntilOrEmpty(".").Trim();
            var fetchedItem = _client.GetAsync(_routePrefix + "?prodId=" + target.ProductId)
                .Result.Content.ReadAsAsync<Product>().Result;

            // Assert
            err1.Should().Be("Products require a name.");
            err2.Should().Be("Product Description is limited to 300 characters.");
            blank.Should().BeEmpty();
            fetchedItem.Should().BeEquivalentTo(target);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region QueriedGets

        #region Paginated

        [TestMethod]
        public void GetAllPaginated_HappyFlow_ShouldOkAndRightPage()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=2&pgIndex=2").Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsAsync<IEnumerable<Product>>().Result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_FullPage_ShouldReturnOkAndRightSize()
        {
            var result = _client.GetAsync(_routePrefix + "?pgSize=10&pgIndex=1").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.Count();

            result.Should().Be(10);
        }

        [TestMethod]
        public void GetAllPaginated_PartialPage_ShouldReturnOkAndRightSize()
        {
            // Arrange
            var recordsCount = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.Count();
            var pgSize = 1;
            var pgIndex = 0;
            var expected = 0;

            while (true)
            {
                expected = recordsCount % pgSize;
                if (expected == 0)
                {
                    pgSize++;
                    break;
                }
            }

            pgIndex = recordsCount / pgSize + 1;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=" + pgIndex)
                .Result.Content.ReadAsAsync<IEnumerable<Product>>().Result.Count();

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void GetAllPaginated_SizeTooBig_ShouldReturnOkAndAll()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;
            var pgSize = expected.Count() + 4;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=1").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_IndexTooHigh_ShouldReturnOkAndEmpty()
        {
            // Arrange
            var pgIndex = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.Count() + 10;

            // Act
            var result = _client.GetAsync(_routePrefix+"?pgSize=1&pgIndex="+pgIndex).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllPaginated_SizeZero_ShouldReturnOkAndAll()
        {
            var result = _client.GetAsync(_routePrefix + "?pgSize=0&pgIndex=12").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllPaginated_IndexZero_ShouldReturnOkAndAll()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=12&pgIndex=0").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(-1, 2)]
        [DataRow(1, -2)]
        public void GetAllPaginated_NegativeNumbers_ShouldConvertToPositive(int pgSize, int pgIndex)
        {
            var expected = _client.GetAsync(_routePrefix + "?pgSize=" + Math.Abs(pgSize) + "&pgIndex=" + Math.Abs(pgIndex)).Result
                .Content.ReadAsAsync<IEnumerable<Product>>().Result;

            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=" + pgIndex).Result
                .Content.ReadAsAsync<IEnumerable<Product>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region Filtered

        [TestMethod]
        public void GetAllFiltered_NameMatch_ShouldReturnMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.Skip(27).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=mill").Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_DescriptionMatch_ShouldReturnMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.Skip(37).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=oak").Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_BarCodeMatch_ShouldReturnMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy="+expected.FirstOrDefault().BarCode)
                .Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_ManufacturerMatch_ShouldReturnMatches()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var manufacturer = allRecords.FirstOrDefault().Manufacturer;
            var expected = allRecords.Where(p => p.Manufacturer == manufacturer);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy="+manufacturer).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_StockMatch_ShouldReturnMatch()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var stock = allRecords.Skip(2).FirstOrDefault().Stock;
            var expected = allRecords.Where(p => p.Stock == stock);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + stock).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_PriceMatch_ShouldReturnMatch()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var price = allRecords.FirstOrDefault().UnitPrice;
            var expected = allRecords.Where(p => p.UnitPrice == price);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + price.ToString()).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_Category_ShouldReturnMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.Where(p=>p.CategoryId == 1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + "traditional").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_InexistentTerm_ShouldReturnEmpty()
        {
            // Arrange
            var searchBy = Guid.NewGuid().ToString();

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + searchBy).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllFiltered_DifTermsForDifResults_ShoudReturnBothMatches()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var expected = allRecords.Take(1).Append(allRecords.Skip(10).FirstOrDefault());
            var searchBy = expected.FirstOrDefault().BarCode.ToString() + " " +
                            expected.LastOrDefault().BarCode.ToString();

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + searchBy).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_OverStock_ShouldReturnMatches()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var expected = allRecords.Where(p => p.Stock > 100);

            // Act
            var result = _client.GetAsync(_routePrefix + "?stock=100&over=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_UnderStock_ShouldReturnMatches()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var expected = allRecords.Where(p => p.Stock < 20);

            // Act
            var result = _client.GetAsync(_routePrefix + "?stock=20&over=false").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_OverPrice_ShouldReturnMatches()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var expected = allRecords.Where(p => p.UnitPrice > 30m);

            // Act
            var result = _client.GetAsync(_routePrefix + "?price=30&over=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_UnderPrice_ShouldReturnMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.UnitPrice < 20m);

            // Act
            var result = _client.GetAsync(_routePrefix + "?price=20&over=false").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_StocksAndPricesOutOfBounds_ShouldReturnEmpty()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var lowStock = allRecords.OrderBy(p => p.Stock).FirstOrDefault().Stock;
            var highStock = allRecords.OrderByDescending(p => p.Stock).FirstOrDefault().Stock;
            var lowPrice = allRecords.OrderBy(p => p.UnitPrice).FirstOrDefault().UnitPrice.ToString();
            var highPrice = allRecords.OrderByDescending(p => p.UnitPrice).FirstOrDefault().UnitPrice.ToString();

            // Act
            var result = _client.GetAsync(_routePrefix + "?price="+highPrice+"&over=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.ToList();
            result.AddRange(_client.GetAsync(_routePrefix + "?price="+lowPrice+"&over=false").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result);
            result.AddRange(_client.GetAsync(_routePrefix + "?stock=" + highStock + "&over=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result);
            result.AddRange(_client.GetAsync(_routePrefix + "?stock=" + lowStock + "&over=false").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region FilteredAndPaged

        [TestMethod]
        public void GetAllFilteredAndPaged_HappyFlow_ShouldReturnMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.Skip(15).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=acrylic&pgSize=2&pgIndex=3").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_OverStock_ShouldReturnMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.Where(p => p.Stock < 13).Skip(4).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?stock=13&over=false&pgSize=2&pgIndex=3").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_UnderStock_ShouldReturnMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.Stock > 140).Skip(4).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?stock=140&over=true&pgSize=2&pgIndex=3").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_OverPrice_ShouldReturnMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.Skip(67).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?price=40&over=true&pgSize=2&pgIndex=3").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_UnderPrice_ShouldReturnMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.Where(p => p.UnitPrice < 5m).Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?price=5&over=false&pgSize=2&pgIndex=2").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region Sorts

        [DataTestMethod]
        [DataRow("ID")]
        [DataRow("iD")]
        [DataRow("PRODUCT")]
        [DataRow("proDuct")]
        [DataRow("PROD")]
        [DataRow("prOd")]
        [DataRow("PRODUCTID")]
        [DataRow("productId")]
        [DataRow("PRODID")]
        [DataRow("prodId")]
        [DataRow("PRODUCT_ID")]
        [DataRow("product_id")]
        [DataRow("PROD_ID")]
        [DataRow("prod_id")]
        public void GetAllSorted_ById_ShouldReturnSorted(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            var result = _client.GetAsync(_routePrefix+"?sortBy="+sortBy).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("NAME")]
        [DataRow("name")]
        [DataRow("PRODUCTNAME")]
        [DataRow("productName")]
        [DataRow("PRODNAME")]
        [DataRow("prodName")]
        [DataRow("PRODUCT_NAME")]
        [DataRow("product_name")]
        [DataRow("PROD_NAME")]
        [DataRow("prod_name")]
        public void GetAllSorted_ByName_ShouldReturnSorted(string sortBy)
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.OrderBy(p=>p.Name);

            // Act
            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("MANUFACTURER")]
        [DataRow("maNuFacTurer")]
        [DataRow("MAN")]
        [DataRow("man")]
        [DataRow("MADEBY")]
        [DataRow("madeBy")]
        [DataRow("MADE_BY")]
        [DataRow("made_by")]
        public void GetAllSorted_ByManufacturer_ShouldReturnSorted(string sortBy)
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.OrderBy(p => p.Manufacturer);

            // Act
            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("BARCODE")]
        [DataRow("barCode")]
        [DataRow("BAR_CODE")]
        [DataRow("bar_code")]
        [DataRow("CODE")]
        [DataRow("coDe")]
        [DataRow("UPC")]
        [DataRow("upc")]
        [DataRow("UPCCODE")]
        [DataRow("upcCode")]
        [DataRow("UPC_CODE")]
        [DataRow("upc_Code")]
        public void GetAllSorted_ByBarCode_ShouldReturnSorted(string sortBy)
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.OrderBy(p => p.BarCode);

            // Act
            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("STOCK")]
        [DataRow("stOck")]
        [DataRow("STK")]
        [DataRow("sTk")]
        public void GetAllSorted_ByStock_ShouldReturnSorted(string sortBy)
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.OrderBy(p => p.Stock);

            // Act
            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("PRICE")]
        [DataRow("pRice")]
        [DataRow("COST")]
        [DataRow("CoSt")]
        [DataRow("UNITPRICE")]
        [DataRow("unitPrice")]
        [DataRow("UNIT_PRICE")]
        [DataRow("unit_price")]
        public void GetAllSorted_ByPrice_ShouldReturnSorted(string sortBy)
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.OrderBy(p => p.UnitPrice);

            // Act
            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("CATEGORY")]
        [DataRow("CaTegOry")]
        [DataRow("CAT")]
        [DataRow("Cat")]
        public void GetAllSorted_ByCategory_ShouldReturnSorted(string sortBy)
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.OrderBy(p => p.CategoryId);

            // Act
            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("DESCRIPTION")]
        [DataRow("DecRiptIon")]
        [DataRow("DESC")]
        [DataRow("dEsc")]
        public void GetAllSorted_ByDescription_ShouldReturnSorted(string sortBy)
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.OrderBy(p => p.Description.Length);

            // Act
            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("sdfhushjb")]
        [DataRow(null)]
        public void GetAllSorted_BadQuery_ShouldReturnSorted(string sortBy)
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Act
            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescending_HappyFlow_ShouldReturnSortedDescending()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.OrderByDescending(p => p.Name);

            // Act
            var result = _client.GetAsync(_routePrefix + "?sortBy=name&desc=true").Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.OrderBy(p => p.BarCode).Skip(4).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=2&pgIndex=3&sortBy=barcode")
                .Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.Skip(9).Take(2).Reverse();

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=2&pgIndex=3&sortBy=name&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.Name.Contains("Yuxin")).OrderBy(p => p.UnitPrice);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=yuxin&sortBy=price").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.Name.Contains("Yuxin")).OrderByDescending(p => p.UnitPrice);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=yuxin&sortBy=price&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverStockAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.Stock>140).OrderBy(p => p.UnitPrice);

            // Act
            var result = _client.GetAsync(_routePrefix + "?stock=140&over=true&sortBy=price").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverStockAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.Stock > 140).OrderByDescending(p => p.UnitPrice);

            // Act
            var result = _client.GetAsync(_routePrefix + "?stock=140&over=true&sortBy=price&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderStockAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.Stock < 20).OrderBy(p => p.UnitPrice);

            // Act
            var result = _client.GetAsync(_routePrefix + "?stock=20&over=false&sortBy=price").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderStockAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.Stock < 20).OrderByDescending(p => p.UnitPrice);

            // Act
            var result = _client.GetAsync(_routePrefix + "?stock=20&over=false&sortBy=price&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverPriceAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.UnitPrice >30).OrderBy(p => p.Stock);

            // Act
            var result = _client.GetAsync(_routePrefix + "?price=30&over=true&sortBy=stock").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverPriceAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.UnitPrice > 30).OrderByDescending(p => p.Stock);

            // Act
            var result = _client.GetAsync(_routePrefix + "?price=30&over=true&sortBy=stock&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderPriceAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.UnitPrice <20).OrderBy(p => p.Stock);

            // Act
            var result = _client.GetAsync(_routePrefix + "?price=20&over=false&sortBy=stock").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderPriceAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.UnitPrice < 20).OrderByDescending(p => p.Stock);

            // Act
            var result = _client.GetAsync(_routePrefix + "?price=20&over=false&sortBy=stock&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.CategoryId == 2 || 
                p.Description.ToUpper().Contains("TABLE") ||
                p.Name.ToUpper().Contains("TABLE"))
                .OrderBy(p => p.UnitPrice).Skip(10).Take(5);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=table&pgSize=5&pgIndex=3&sortBy=price").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result
                .Where(p => p.CategoryId == 2 ||
                p.Description.ToUpper().Contains("TABLE") ||
                p.Name.ToUpper().Contains("TABLE"))
                .OrderByDescending(p => p.UnitPrice).Skip(10).Take(5);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=table&pgSize=5&pgIndex=3&sortBy=price&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverStockSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var expected = allRecords.Skip(19).Take(1).Append(allRecords.Skip(21).FirstOrDefault());

            // Act
            var result = _client.GetAsync(_routePrefix + "?stock=100&over=true&pgSize=2&pgIndex=2&sortBy=price").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverStockSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var expected = allRecords.Skip(4).Take(1).Append(allRecords.Skip(34).FirstOrDefault());

            // Act
            var result = _client.GetAsync(_routePrefix + "?stock=100&over=true&pgSize=2&pgIndex=2&sortBy=price&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderStockSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.Skip(28).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?stock=10&over=false&pgSize=2&pgIndex=3&sortBy=price").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderStockSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>()
                .Result.Skip(22).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?stock=10&over=false&pgSize=2&pgIndex=3&sortBy=price&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverPriceSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var expected = allRecords.Skip(3).Take(1).Append(allRecords.Skip(67).FirstOrDefault());

            // Act
            var result = _client.GetAsync(_routePrefix + "?price=30&over=true&pgSize=2&pgIndex=2&sortBy=stock").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverPriceSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result.Skip(33).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?price=30&over=true&pgSize=2&pgIndex=2&sortBy=stock&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderPriceSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var expected = allRecords.Skip(68).Take(1).Append(allRecords.Skip(24).FirstOrDefault());

            // Act
            var result = _client.GetAsync(_routePrefix + "?price=10&over=false&pgSize=2&pgIndex=2&sortBy=stock").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderPriceSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            var expected = allRecords.Skip(19).Take(1).Append(allRecords.Skip(39).FirstOrDefault());

            // Act
            var result = _client.GetAsync(_routePrefix + "?price=10&over=false&pgSize=2&pgIndex=2&sortBy=stock&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Product>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #endregion

    }
}
