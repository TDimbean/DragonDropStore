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
    public class CustomerControllerTests
    {
        private string _routePrefix;
        private HttpClient _client;
        private JsonMediaTypeFormatter _formatter;

        [TestInitialize]
        public void Init()
        {
            _routePrefix = "http://localhost:10957/api/customers";

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
            var fetchedItems = result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            fetchedItems.Should().NotBeEmpty();
        }


        // For some reason GET returns a collection of desired Object, not a single Object
        [TestMethod]
        public void Get_HappyFlow_ShouldReturnOkAndRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.FirstOrDefault();

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=" + expected.CustomerId).Result.Content
                .ReadAsAsync<Customer>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnBadRequest()
        {
            // Arrange
            var id = 0;
            while (true)
            {
                if (!_client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>()
                    .Result.Any(c => c.CustomerId == id)) break;
                id = new Random().Next(0, int.MaxValue);
            }

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=" + id).Result.StatusCode;

            // Assert
            result.Should().Be(HttpStatusCode.NotFound);
        }

        #region Creates

        [TestMethod]
        public void Create_HappyFlow_ShouldReturnOk()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            var idlessNew = new
            {
                newItem.Name,
                newItem.Address,
                newItem.Phone,
                newItem.Email,
                newItem.City,
                newItem.State
            };

            // Act
            var result = _client.PostAsync(_routePrefix, newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            var idlessFetched = new
            {
                fetchedItem.Name,
                fetchedItem.Address,
                fetchedItem.Phone,
                fetchedItem.Email,
                fetchedItem.City,
                fetchedItem.State
            };

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("\"Customer successfully registered.\"");
            idlessFetched.Should().BeEquivalentTo(idlessNew);
        }

        #region EmptyFields

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Create_NoName_ShouldReturnBadRequestAndAppropriateError(string name)
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Name = name;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix+"?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Name cannot be blank.");
            newCount.Should().Be(initCount);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Create_NoPhone_ShouldReturnBadRequestAndAppropriateError(string phone)
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Phone = phone;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Customer requires a phone number.");
            newCount.Should().Be(initCount);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Create_NoAddress_ShouldReturnBadRequestAndAppropriateError(string adr)
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Address = adr;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Address cannot be empty.");
            newCount.Should().Be(initCount);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Create_NoCity_ShouldReturnBadRequestAndAppropriateError(string city)
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.City = city;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("City field cannot be blank.");
            newCount.Should().Be(initCount);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Create_NoState_ShouldReturnBadRequestAndAppropriateError(string state)
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.State = state;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("State/County cannot be blank.");
            newCount.Should().Be(initCount);
        }

        #endregion

        #region StringsTooLong

        [TestMethod]
        public void Create_NameTooLong_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Name = StaticGenerator.GenRandomString(101);

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Name must not exceed 100 characters.");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_EmailTooLong_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Email  = StaticGenerator.GenRandomString(101);

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Email must not exceed 100 characters.");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_AddressTooLong_ShouldReturnBadRequestAndAppropriateError()
        {
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });

            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            var client = new HttpClient();

            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Address = StaticGenerator.GenRandomString(201);

            var initCount = client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = client.PostAsync(_routePrefix + "?returnErrors=true", newItem, formatter).Result;
            var newCount = client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Address may not exceed 200 characters.");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_CityTooLong_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.City = StaticGenerator.GenRandomString(201);

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("City should not exceed 100 characters; If necessary, please use an abbreviation.");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_StateTooLong_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.State = StaticGenerator.GenRandomString(51);

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("State/County is limited to 50 characters; Where necessary, please use an abbreviation.");
            newCount.Should().Be(initCount);
        }

        #endregion

        #region PhoneWrongFormat

        [DataTestMethod]
        [DataRow("1234567890")]
        [DataRow("123-456-78910")]
        public void Create_PhoneLengthWrong_ShouldReturnBadRequestAndAppropriateError(string phone)
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Phone = phone;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Phone Numbers must have precisely 10 characters.");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_PhoneNoDashes_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Phone = "123+123+1234";

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Phone number must be in valid format: XXX-XXX-XXXX.");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_PhoneNonDigits_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Phone = "123-123-123a";

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Phone number may only contain digits.");
            newCount.Should().Be(initCount);
        }

        #endregion

        [TestMethod]
        public void Create_EmailWrongFormat_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Email = StaticGenerator.GenRandomString(20).Replace("@","");

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".")
                .Should().Be("Email must be in a valid format (account@provider).");
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_MultipleErrors_ShouldReturnBadRequestAndAppropriateErrors()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.State = null;
            newItem.City = null;

            var initCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result
                .Content.ReadAsAsync<HttpError>().Result.Message;
            var err1 = result.Trim().GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "");
            var newCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            err1.Should().Be("City field cannot be blank.");
            err2.Should().Be("State/County cannot be blank.");
            blank.Should().BeEmpty();
            newCount.Should().Be(initCount);
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
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Customer registration failed.");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            newCount.Should().Be(initCount);
        }

        #endregion

        #region Updates

        [TestMethod]
        public void Update_HappyFlow_ShouldReturnOk()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            var savedItem = new Customer
            {
                Name = targetItem.Name,
                State = targetItem.State,
                Address = targetItem.Address,
                City = targetItem.City,
                Email = targetItem.Email,
                Phone = targetItem.Phone
            };

            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix, newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Should().Be("\"Customer successfully updated.\"");
            fetchedItem.Should().BeEquivalentTo(newItem);
            fetchedItem.Should().NotBeEquivalentTo(targetItem);

            // CleanUp
            var repair = _client.PutAsync(_routePrefix, targetItem, _formatter).Result;
            repair.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #region EmptyFields

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Update_NoName_ShouldReturnBadRequestAndAppropriateError(string name)
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;
            newItem.Name = name;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Name cannot be blank.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Update_NoPhone_ShouldReturnBadRequestAndAppropriateError(string phone)
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Phone = phone;
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Customer requires a phone number.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Update_NoAddress_ShouldReturnBadRequestAndAppropriateError(string adr)
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Address = adr;
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Address cannot be empty.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Update_NoCity_ShouldReturnBadRequestAndAppropriateError(string city)
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.City = city;
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("City field cannot be blank.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Update_NoState_ShouldReturnBadRequestAndAppropriateError(string state)
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.State = state;
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("State/County cannot be blank.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region StringsTooLong

        [TestMethod]
        public void Update_NameTooLong_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Name = StaticGenerator.GenRandomString(101);
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Name must not exceed 100 characters.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_EmailTooLong_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Email = StaticGenerator.GenRandomString(101);
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId).Result
                .Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Email must not exceed 100 characters.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_AddressTooLong_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Address = StaticGenerator.GenRandomString(201);
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId).Result.Content
                .ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Address may not exceed 200 characters.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_CityTooLong_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.City = StaticGenerator.GenRandomString(201);
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("City should not exceed 100 characters; If necessary, please use an abbreviation.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_StateTooLong_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.State = StaticGenerator.GenRandomString(51);
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("State/County is limited to 50 characters; Where necessary, please use an abbreviation.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region PhoneWrongFormat

        [DataTestMethod]
        [DataRow("1234567890")]
        [DataRow("123-456-78910")]
        public void Update_PhoneLengthWrong_ShouldReturnBadRequestAndAppropriateError(string phone)
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Phone = phone;
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Phone Numbers must have precisely 10 characters.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_PhoneNoDashes_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Phone = "123+123+1234";
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Phone number must be in valid format: XXX-XXX-XXXX.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_PhoneNonDigits_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Phone = "123-123-123a";
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Phone number may only contain digits.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        [TestMethod]
        public void Update_EmailWrongFormat_ShouldReturnBadRequestAndAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.Email = StaticGenerator.GenRandomString(20).Replace("@","");
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Email must be in a valid format (account@provider).");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_MultipleErrors_ShouldReturnBadRequestAndAppropriateErrors()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.State = null;
            newItem.City = null;
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var resMsg =result.Content.ReadAsAsync<HttpError>().Result.Message;
            var status = result.StatusCode;
            var err1 = resMsg.Trim().GetUntilOrEmpty(".");
            var err2 = resMsg.Replace(err1, "").Trim();
            var blank = resMsg.Replace(err1, "").Replace(err2, "").Trim();
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            err1.Should().Be("City field cannot be blank.");
            err2.Should().Be("State/County cannot be blank.");
            blank.Should().BeEmpty();
            status.Should().Be(HttpStatusCode.BadRequest);
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_BadFlowNoErrorRequest_ShouldReturnBadRequestWithStandardError()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            newItem.State = null;
            newItem.City = null;
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.LastOrDefault();
            newItem.CustomerId = targetItem.CustomerId;

            // Act
            var result = _client.PutAsync(_routePrefix, newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + targetItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.Should().Be("Customer update failed.");
            fetchedItem.Should().BeEquivalentTo(targetItem);
            fetchedItem.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_InexistentId_ShouldReturnBadRequest()
        {
            // Arrange
            var newItem = StaticGenerator.GenCustomer();
            var id = 0;
            while(true)
            {
                if (_client.GetAsync(_routePrefix + "?custId=" + id).Result.StatusCode == HttpStatusCode.OK) id = (new Random()).Next(0, int.MaxValue);
                break;
            }
            newItem.CustomerId = id;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?custId=" + newItem.CustomerId)
                .Result.Content.ReadAsAsync<Customer>().Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Content.ReadAsAsync<HttpError>().Result.Message.GetUntilOrEmpty(".").Should().Be("Customer ID must exist within the Repo for an update to be performed.");
            fetchedItem.Should().BeNull();
        }

        #endregion

        #region QueriedGets

        #region Paginated

        [TestMethod]
        public void GetAllPaginated_HappyFlow_ShouldOkAndRightPage()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=2&pgIndex=2").Result;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_FullPage_ShouldReturnOkAndRightSize()
        {
            var result = _client.GetAsync(_routePrefix + "?pgSize=10&pgIndex=1").Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            result.Should().Be(10);
        }
        
        [TestMethod]
        public void GetAllPaginated_PartialPage_ShouldReturnOkAndRightSize()
        {
            // Arrange
            var recordsCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();
            var pgSize = 1;
            var pgIndex = 0;
            var expected = 0;

            while (true)
            {
                expected = recordsCount % pgSize;
                if (expected == 0)
                    pgSize++;
                  else  break;
            }
            pgIndex = recordsCount / pgSize + 1;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=" + pgIndex)
                .Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Count();

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void GetAllPaginated_SizeTooBig_ShouldReturnOkAndAll()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;
            var pgSize = expected.Count() + 4;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=1").Result.Content
                .ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_IndexTooHigh_ShouldReturnOkAndEmpty()
        {
            // Arrange
            var pgIndex = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>()
                .Result.Count() + 10;

            // Act
            var result = _client.GetAsync(_routePrefix+"?pgSize=1&pgIndex="+pgIndex).Result.Content
                .ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllPaginated_SizeZero_ShouldReturnOkAndAll()
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            var result = _client.GetAsync(_routePrefix + "?pgSize=0&pgIndex=12").Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_IndexZero_ShouldReturnOkAndAll()
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            var result = _client.GetAsync(_routePrefix + "?pgSize=12&pgIndex=0").Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(-1, 2)]
        [DataRow(1, -2)]
        public void GetAllPaginated_NegativeNumbers_ShouldConvertToPositive(int pgSize, int pgIndex)
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix + "?pgSize=" + Math.Abs(pgSize) + "&pgIndex=" + Math.Abs(pgIndex)).Result
                .Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=" + pgIndex).Result
                .Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region Filtered

        [TestMethod]
        public void GetAllFiltered_NameMatch_ShouldReturnOkAndMatch()
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Take(1);

            var result = _client.GetAsync(_routePrefix + "?searchBy=" + expected.FirstOrDefault().Name)
                .Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_EmailMatch_ShouldReturnOkAndMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>()
                .Result.Where(c => c.Email != null).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + expected.FirstOrDefault().Email)
                .Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_AddressMatch_ShouldReturnOkAndMatch()
        {
            // Arrange
            var expected = new List<Customer>
            { _client.GetAsync(_routePrefix + "?custId=1").Result.Content.ReadAsAsync<Customer>().Result };

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=holy")
               .Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_CityMatch_ShouldReturnOkAndMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>()
                .Result.Skip(26).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + expected.FirstOrDefault().City)
                .Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_StateMatch_ShouldReturnOkAndMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>()
                .Result.Skip(4).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + expected.FirstOrDefault().State)
                .Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_StateAbbreviationMatch_ShouldReturnOkAndMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>()
                 .Result.Where(x => x.State == "New Hampshire");

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=NH").Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_InexistentTerm_ShouldReturnOkAndEmpty()
        {
            var search = Guid.NewGuid().ToString();

            var result = _client.GetAsync(_routePrefix+"?searchBy="+search).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllFiltered_DifTermsForDifResults_ShoudReturnOkAndBothMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Skip(19).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=Nuys Utica").Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_HappyFlow_ShouldReturnOkAndMatch()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.Skip(33).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=Texas&pgSize=2&pgIndex=2").Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region Sorts

        [DataTestMethod]
        [DataRow("ID")]
        [DataRow("CUSTOMERID")]
        [DataRow("id")]
        [DataRow("customerId")]
        public void GetAllSorted_ById_ShouldReturnSorted(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            var result = _client.GetAsync(_routePrefix + "?sortby=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("NaME")]
        [DataRow("NAME")]
        public void GetAllSorted_ByName_ShouldReturnSorted(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.OrderBy(x=>x.Name);

            var result = _client.GetAsync(_routePrefix + "?sortby=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("ADR")]
        [DataRow("ADDRESS")]
        [DataRow("address")]
        [DataRow("adr")]
        public void GetAllSorted_ByAddress_ShouldReturnSorted(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.OrderBy(x => x.Address);

            var result = _client.GetAsync(_routePrefix + "?sortby=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("PHONE")]
        [DataRow("phone")]
        [DataRow("PHONENUMBER")]
        [DataRow("PhoneNumber")]
        [DataRow("NUMBER")]
        [DataRow("NumbER")]
        [DataRow("PHONENO")]
        [DataRow("PhoneNO")]
        [DataRow("PhoneNO")]
        [DataRow("PHONE_NO")]
        [DataRow("phone_no")]
        [DataRow("PHONE_NUMBER")]
        [DataRow("PhONe_NumbEr")]
        [DataRow("TELEPHONE")]
        [DataRow("telephone")]
        public void GetAllSorted_ByPhone_ShouldReturnSorted(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.OrderBy(x => x.Phone);

            var result = _client.GetAsync(_routePrefix + "?sortby=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("EMAIL")]
        [DataRow("email")]
        [DataRow("E-MAIL")]
        [DataRow("e-mail")]
        [DataRow("MAIL")]
        [DataRow("mAiL")]
        public void GetAllSorted_ByEmail_ShouldReturnSorted(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.OrderBy(x => x.Email);

            var result = _client.GetAsync(_routePrefix + "?sortby=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("City")]
        [DataRow("CITY")]
        [DataRow("TOWN")]
        [DataRow("ToWn")]
        public void GetAllSorted_ByCity_ShouldReturnSorted(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.OrderBy(x => x.City);

            var result = _client.GetAsync(_routePrefix + "?sortby=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("STATE")]
        [DataRow("sTaTe")]
        [DataRow("COUNTY")]
        [DataRow("County")]
        public void GetAllSorted_ByState_ShouldReturnSorted(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result.OrderBy(x => x.State);

            var result = _client.GetAsync(_routePrefix + "?sortby=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("asdfok")]
        public void GetAllSorted_BadQuery_ShouldReturnAll(string sortBy)
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Act
            var resultAsc = _client.GetAsync(_routePrefix + "?sortby=" + sortBy).Result.Content
                .ReadAsAsync<IEnumerable<Customer>>().Result;
            var resultDesc = _client.GetAsync(_routePrefix + "?sortby=" + sortBy+"&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            resultAsc.Should().BeEquivalentTo(expected);
            resultDesc.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSorted_Descending_ShouldSortDescending()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>()
                .Result.OrderByDescending(x => x.Name);

            // Act
            var result = _client.GetAsync(_routePrefix + "?sortby=Name&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Customer>>().Result;

            //Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedAndPaged_ShouldReturnRequested()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;
            var expected = allRecords.Skip(11).Take(1).Append(allRecords.Skip(5).FirstOrDefault())
                                                    .Append(allRecords.Skip(18).FirstOrDefault());

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=3&pgIndex=5&sortby=Name")
                .Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescendingAndPaged_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result
                .OrderByDescending(c => c.Phone).Skip(4).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=2&pgIndex=3&sortby=Phone&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>()
                .Result.Where(x => x.State == "Texas").OrderByDescending(x => x.CustomerId);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=Texas&sortBy=City")
                .Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>()
                .Result.Where(x => x.State == "Texas");

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=Texas&sortBy=City&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>()
                .Result.Where(x => x.CustomerId == 10);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=Texas&pgSize=1&pgIndex=3&sortBy=City")
                .Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Customer>>()
                .Result.Where(x => x.Name.Contains("Hansen"));

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=Texas&pgSize=1&pgIndex=3&sortBy=City&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Customer>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #endregion
    }
}
