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
    public class PaymentControllerTests
    {
        private string _routePrefix;
        private HttpClient _client;
        private JsonMediaTypeFormatter _formatter;

        [TestInitialize]
        public void Init()
        {
            _routePrefix = "http://localhost:10957/api/payments";

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
            var fetchedItems = result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            fetchedItems.Should().NotBeEmpty();
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldReturnOkAndRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result.FirstOrDefault();

            // Act
            var status = _client.GetAsync(_routePrefix + "?payId=" + expected.PaymentId).Result;
            var result = status.Content.ReadAsAsync<Payment>().Result;

            // Assert
            status.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnBadRequest()
        {
            // Arrange
            var id = 0;
            while (true)
            {
                if (!_client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.Any(p => p.PaymentId == id)) break;
                id = (new Random()).Next(0, int.MaxValue);
            }

            // Act
            var result = _client.GetAsync(_routePrefix + "?payId=" + id).Result.StatusCode;

            // Assert
            result.Should().Be(HttpStatusCode.NotFound);
        }

        #region Creates

        [TestMethod]
        public void Create_HappyFlow_ShouldReturnOK()
        {
            // Arrange
            var newItem = StaticGenerator.GenPayment();

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsStringAsync().Result;

            // Assert
            result.Should().Be("\"Payment successfully registered.\"");
        }

        [TestMethod]
        public void Create_InexistentCustomerId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenPayment();
            newItem.CustomerId = 0;
            while (true)
            {
                if (_client.GetAsync(_routePrefix.Replace("payments", "customers") + "?custId=" + newItem.CustomerId).Result.StatusCode == HttpStatusCode.OK)
                    newItem.CustomerId = (new Random()).Next(1, int.MaxValue);
                else break;
            }

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content
                .ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Payments require a valid CustomerId.");
        }

        [TestMethod]
        public void Create_InexistentPaymentMethodId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenPayment();
            newItem.PaymentMethodId = 111;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content
                .ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Payment Method not found; Please submit a valid Payment Method ID.");
        }

        [TestMethod]
        public void Create_DateNull_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenPayment();
            newItem.Date = null;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content
                .ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("A payment's Date cannot be blank.");
        }

        [TestMethod]
        public void Create_DateAfterToday_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenPayment();
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content
                .ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
        }

        [TestMethod]
        public void Create_AmountZero_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenPayment();
            newItem.Amount = 0m;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content
                .ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Amount must be greater than 0.");
        }

        [TestMethod]
        public void Create_AmountNull_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenPayment();
            newItem.Amount = null;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content
                .ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Amount must be greater than 0.");
        }

        [TestMethod]
        public void Create_AmountNegative_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenPayment();
            newItem.Amount = -3m;

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content
                .ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Amount must be greater than 0.");
        }

        [TestMethod]
        public void Create_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        {
            // Arrange
            var newItem = StaticGenerator.GenPayment();
            newItem.Amount = -3m;
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = _client.PostAsync(_routePrefix + "?returnErrors=true", newItem, _formatter).Result.Content
                .ReadAsAsync<HttpError>().Result.Message;
            var err1 = result.GetUntilOrEmpty(".").Trim();
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();

            // Assert
            err1.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            err2.Should().Be("Amount must be greater than 0.");
            blank.Should().BeEmpty();
        }

        #endregion

        #region Updates

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var targetItem = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.FirstOrDefault();
            var savedItem = new Payment
            {
                PaymentId = targetItem.PaymentId,
                CustomerId = targetItem.CustomerId,
                PaymentMethodId = targetItem.PaymentMethodId,
                Amount = targetItem.Amount,
                Date = targetItem.Date
            };
            var newItem = StaticGenerator.GenPayment();
            newItem.PaymentId = targetItem.PaymentId;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsStringAsync().Result;
            var fetchedItem = _client.GetAsync(_routePrefix + "?payId=" + targetItem.PaymentId)
                .Result.Content.ReadAsAsync<Payment>().Result;

            // Assert
            result.Should().Be("\"Payment successfully updated.\"");
            fetchedItem.Should().BeEquivalentTo(newItem);
            fetchedItem.Should().NotBeEquivalentTo(savedItem);

            // CleanUp
            var repair = _client.PutAsync(_routePrefix, savedItem, _formatter).Result;
            repair.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public void Update_InexistentId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = StaticGenerator.GenPayment();
            newItem.PaymentId = 0;
            while (true)
            {
                if (_client.GetAsync(_routePrefix + "?payId=" + newItem.PaymentId).Result.StatusCode == HttpStatusCode.OK)
                    newItem.PaymentId = (new Random()).Next(1, int.MaxValue);
                else break;
            }

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message.Trim();

            // Assert
            result.Should().Be("No Payment with ID: " + newItem.PaymentId + " found in Repo.");
        }

        [TestMethod]
        public void Update_InexistentCustomerId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var targetId = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.FirstOrDefault().PaymentId;
            var newItem = StaticGenerator.GenPayment();
            newItem.PaymentId = targetId;
            newItem.CustomerId = 98767;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Payments require a valid CustomerId.");
        }

        [TestMethod]
        public void Update_InexistentPaymentMethodId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var targetId = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.FirstOrDefault().PaymentId;
            var newItem = StaticGenerator.GenPayment();
            newItem.PaymentId = targetId;
            newItem.PaymentMethodId = 98767;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Payment Method not found; Please submit a valid Payment Method ID.");
        }

        [TestMethod]
        public void Update_NullDate_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var targetId = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.FirstOrDefault().PaymentId;
            var newItem = StaticGenerator.GenPayment();
            newItem.PaymentId = targetId;
            newItem.Date = null;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("A payment's Date cannot be blank.");
        }

        [TestMethod]
        public void Update_DateAfterToday_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var targetId = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.FirstOrDefault().PaymentId;
            var newItem = StaticGenerator.GenPayment();
            newItem.PaymentId = targetId;
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
        }

        [TestMethod]
        public void Update_NegativeAmount_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var targetId = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.FirstOrDefault().PaymentId;
            var newItem = StaticGenerator.GenPayment();
            newItem.PaymentId = targetId;
            newItem.Amount = -3m;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Amount must be greater than 0.");
        }

        [TestMethod]
        public void Update_NullAmount_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var targetId = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.FirstOrDefault().PaymentId;
            var newItem = StaticGenerator.GenPayment();
            newItem.PaymentId = targetId;
            newItem.Amount = null;

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Amount must be greater than 0.");
        }

        [TestMethod]
        public void Update_AmountZero_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var targetId = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.FirstOrDefault().PaymentId;
            var newItem = StaticGenerator.GenPayment();
            newItem.PaymentId = targetId;
            newItem.Amount = 0m;

            // Act
            var result = _client.PutAsync(_routePrefix+"?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;

            // Assert
            result.Should().Be("Amount must be greater than 0.");
        }

        [TestMethod]
        public void Update_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var targetId = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.FirstOrDefault().PaymentId;
            var newItem = StaticGenerator.GenPayment();
            newItem.PaymentId = targetId;
            newItem.Amount = -3;
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = _client.PutAsync(_routePrefix + "?returnErrors=true", newItem, _formatter)
                .Result.Content.ReadAsAsync<HttpError>().Result.Message;
            var err1 = result.GetUntilOrEmpty(".").Trim();
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();

            // Assert
            err1.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            err2.Should().Be("Amount must be greater than 0.");
            blank.Should().BeEmpty();
        }

        #endregion

        #region Gets

        [TestMethod]
        public void GetAll_HappyFlow_ShouldFetchAll()
        {
            var result = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().NotBeEmpty();
        }

        #region Paginated

        [TestMethod]
        public void GetAllPaginated_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Skip(10).Take(5);

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=5&pgIndex=3").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_FullPage_ShouldReturnRightSize()
        {
            var result = _client.GetAsync(_routePrefix + "?pgSize=7&pgIndex=5").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Count().Should().Be(7);
        }

        [TestMethod]
        public void GetAllPaginated_PartialPage_ShouldReturnOkAndRightSize()
        {
            // Arrange
            var recordsCount = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.Count();
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
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.Count();

            // Assert
            result.Should().Be(expected);
        }

        [TestMethod]
        public void GetAllPaginated_SizeTooBig_ShouldReturnOkAndAll()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;
            var pgSize = expected.Count() + 4;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=1").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllPaginated_IndexTooHigh_ShouldReturnOkAndEmpty()
        {
            // Arrange
            var pgIndex = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Payment>>()
                .Result.Count() + 10;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=1&pgIndex=" + pgIndex).Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllPaginated_SizeZero_ShouldReturnOkAndAll()
        {
            var result = _client.GetAsync(_routePrefix + "?pgSize=0&pgIndex=12").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllPaginated_IndexZero_ShouldReturnOkAndAll()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=12&pgIndex=0").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow(-1, 2)]
        [DataRow(1, -2)]
        public void GetAllPaginated_NegativeNumbers_ShouldConvertToPositive(int pgSize, int pgIndex)
        {
            var expected = _client.GetAsync(_routePrefix + "?pgSize=" + Math.Abs(pgSize) + "&pgIndex=" + Math.Abs(pgIndex)).Result
                .Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            var result = _client.GetAsync(_routePrefix + "?pgSize=" + pgSize + "&pgIndex=" + pgIndex).Result
                .Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region Filtered

        [TestMethod]
        public void GetAllFiltered_AmountMatch_ShouldReturnMatch()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;
            var targetItem = allRecords.FirstOrDefault();
            var skips = 1;
            while (true)
            {
                if (allRecords.Any(p => p.Amount == targetItem.Amount && p.PaymentId != targetItem.PaymentId))
                {
                    targetItem = allRecords.Skip(skips).FirstOrDefault();
                    skips++;
                }
                else break;
            }

            var expected = new List<Payment> { targetItem };

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + targetItem.Amount.ToString()).Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_CustomerNameMatch_ShouldReturnMatch()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;
            var targetItem = allRecords.FirstOrDefault();
            var skips = 1;
            while (true)
            {
                if (allRecords.Any(p => p.CustomerId == targetItem.CustomerId && p.PaymentId != targetItem.PaymentId))
                {
                    targetItem = allRecords.Skip(skips).FirstOrDefault();
                    skips++;
                }
                else break;
            }

            var searchBy = _client.GetAsync(_routePrefix.Replace("payments", "customers") + "?custId=" + targetItem.CustomerId).Result.Content
                .ReadAsAsync<Customer>().Result.Name;

            var expected = new List<Payment> { targetItem };

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + searchBy).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_DateMatch_ShouldReturnMatch()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;
            var targetItem = allRecords.FirstOrDefault();
            var skips = 1;
            while (true)
            {
                if (allRecords.Any(p => p.Date == targetItem.Date && p.PaymentId != targetItem.PaymentId))
                {
                    targetItem = allRecords.Skip(skips).FirstOrDefault();
                    skips++;
                }
                else break;
            }

            var expected = new List<Payment> { targetItem };

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + targetItem.Date.GetValueOrDefault()
                .ToShortDateString()).Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_MethodMatch_ShouldReturnMatch()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;
            var target = allRecords.FirstOrDefault().PaymentMethodId;

            var expected = allRecords.Where(p => p.PaymentMethodId == target);

            var searchBy = "";

            switch (target)
            {
                case 1:
                    searchBy = "Credit Card";
                    break;
                case 2:
                    searchBy = "Cash";
                    break;
                case 3:
                    searchBy = "PayPal";
                    break;
                case 4:
                    searchBy = "Wire Transfer";
                    break;
                default:
                    break;
            }

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + searchBy).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_InexistentTerm_ShouldReturnEmpty()
        {
            var searchBy = Guid.NewGuid().ToString();

            var result = _client.GetAsync(_routePrefix + "?searchBy=" + searchBy).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllFiltered_DifTermsForDifResults_ShoudReturnBothMatches()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            var skips = 1;
            var target1 = allRecords.FirstOrDefault();
            while (true)
            {
                if (allRecords.Any(p => p.Date == target1.Date && p.PaymentId != target1.PaymentId))
                {
                    target1 = allRecords.Skip(skips).FirstOrDefault();
                    skips++;
                }
                else break;
            }
            skips = 1;

            var remainingRecords = allRecords.ToList();
            remainingRecords.Remove(target1);
            var target2 = remainingRecords.FirstOrDefault();
            while (true)
            {
                if (remainingRecords.Any(p => p.Date == target2.Date && p.PaymentId != target2.PaymentId))
                {
                    target2 = remainingRecords.Skip(skips).FirstOrDefault();
                    skips++;
                }
                else break;
            }

            var searchBy = target1.Date.GetValueOrDefault().ToShortDateString() + " "
                            + target2.Date.GetValueOrDefault().ToShortDateString().ToString();
            var expected = new List<Payment> { target1, target2 };

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + searchBy).Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_HappyFlow_ShouldReturnMatch()
        {
            // Arrange
            var searchBy = _client.GetAsync(_routePrefix.Replace("payments", "customers") + "?custId=4").Result.Content.ReadAsAsync<Customer>()
                .Result.Name;
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result.Skip(48).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=" + searchBy + "&pgSize=2&pgIndex=2").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_BeforeDate_ShouldReturnMatches()
        {
            // Arrange
            var searchBy = "27Jan2017";
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Date < DateTime.Parse(searchBy));

            // Act
            var result = _client.GetAsync(_routePrefix + "?date=" + searchBy + "&before=true").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_AfterDate_ShouldReturnMatches()
        {
            // Arrange
            var searchBy = "27Sep2018";
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Date > DateTime.Parse(searchBy));

            // Act
            var result = _client.GetAsync(_routePrefix + "?date=" + searchBy + "&before=false")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_OverAmount_ShouldReturnMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Amount > 500m);

            // Act
            var result = _client.GetAsync(_routePrefix + "?amount=500&over=true").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_UnderAmount_ShouldReturnMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Amount < 500m);

            // Act
            var result = _client.GetAsync(_routePrefix + "?amount=500&over=false").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_DatesAndAmountsOutOfBounds_ShouldReturnEmpty()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            var lowAmnt = allRecords.OrderBy(p => p.Amount).FirstOrDefault().Amount.ToString();
            var highAmnt = allRecords.OrderByDescending(p => p.Amount).FirstOrDefault().Amount.ToString();
            var earlyDate = allRecords.OrderBy(p => p.Date).FirstOrDefault().Date.GetValueOrDefault().ToShortDateString();
            var lateDate = allRecords.OrderByDescending(p => p.Date).FirstOrDefault().Date.GetValueOrDefault().ToShortDateString();

            // Act
            var result = _client.GetAsync(_routePrefix + "?amount=" + lowAmnt + "&over=false").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result.ToList();
            result.AddRange(_client.GetAsync(_routePrefix + "?amount=" + highAmnt + "&over=true").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result);
            result.AddRange(_client.GetAsync(_routePrefix + "?date=" + earlyDate + "&before=true").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result);
            result.AddRange(_client.GetAsync(_routePrefix + "?date=" + lateDate + "&before=false").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result);

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_BeforeDate_ShouldReturnMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Date < new DateTime(2019, 1, 1)).Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?date=1Jan2019&before=true&pgSize=2&pgIndex=2")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_AfterDate_ShouldReturnMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Date > new DateTime(2019, 1, 1)).Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?date=1Jan2019&before=false&pgSize=2&pgIndex=2")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_OverAmount_ShouldReturnMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Amount > 1000).Skip(2).Take(2);


            // Act
            var result = _client.GetAsync(_routePrefix + "?amount=1000&over=true&pgSize=2&pgIndex=2")
                    .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_UnderAmount_ShouldReturnMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Amount < 1000).Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?amount=1000&over=false&pgSize=2&pgIndex=2")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region Sorts

        [DataTestMethod]
        [DataRow("ID")]
        [DataRow("id")]
        [DataRow("PAYMENTID")]
        [DataRow("paymentId")]
        [DataRow("PAYID")]
        [DataRow("payId")]
        [DataRow("PAYMENT")]
        [DataRow("paYmeNt")]
        [DataRow("PAY")]
        [DataRow("pay")]
        [DataRow("PAY_ID")]
        [DataRow("pay_Id")]
        [DataRow("PAYMENT_ID")]
        [DataRow("payment_id")]
        public void GetAllSorted_ById_ShouldReturnRequested(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .OrderBy(p => p.PaymentId);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("ID")]
        [DataRow("id")]
        [DataRow("PAYMENTID")]
        [DataRow("paymentId")]
        [DataRow("PAYID")]
        [DataRow("payId")]
        [DataRow("PAYMENT")]
        [DataRow("paYmeNt")]
        [DataRow("PAY")]
        [DataRow("pay")]
        [DataRow("PAY_ID")]
        [DataRow("pay_Id")]
        [DataRow("PAYMENT_ID")]
        [DataRow("payment_id")]
        public void GetAllSortedDescending_ById_ShouldReturnRequested(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .OrderByDescending(p => p.PaymentId);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy + "&desc=true").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("CUSTOMER")]
        [DataRow("cUstUmer")]
        [DataRow("CUST")]
        [DataRow("cUst")]
        [DataRow("CUSTOMERID")]
        [DataRow("customerId")]
        [DataRow("CUSTID")]
        [DataRow("custId")]
        [DataRow("CUSTOMER_ID")]
        [DataRow("customer_id")]
        [DataRow("CUST_ID")]
        [DataRow("cust_id")]
        public void GetAllSorted_ByCustomerId_ShouldReturnRequested(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .OrderBy(p => p.CustomerId);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("CUSTOMER")]
        [DataRow("cUstUmer")]
        [DataRow("CUST")]
        [DataRow("cUst")]
        [DataRow("CUSTOMERID")]
        [DataRow("customerId")]
        [DataRow("CUSTID")]
        [DataRow("custId")]
        [DataRow("CUSTOMER_ID")]
        [DataRow("customer_id")]
        [DataRow("CUST_ID")]
        [DataRow("cust_id")]
        public void GetAllSortedDescending_ByCustomerId_ShouldReturnRequested(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .OrderByDescending(p => p.CustomerId);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy + "&desc=true").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("AMT")]
        [DataRow("aMt")]
        [DataRow("AMOUNT")]
        [DataRow("amoUNt")]
        public void GetAllSorted_ByAmount_ShouldReturnRequested(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .OrderBy(p => p.Amount);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("AMT")]
        [DataRow("aMt")]
        [DataRow("AMOUNT")]
        [DataRow("amoUNt")]
        public void GetAllSortedDescending_ByAmount_ShouldReturnRequested(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .OrderByDescending(p => p.Amount);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy + "&desc=true").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("DATE")]
        [DataRow("daTe")]
        public void GetAllSorted_ByDate_ShouldReturnRequested(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .OrderBy(p => p.Date);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("DATE")]
        [DataRow("daTe")]
        public void GetAllSortedDescending_ByDate_ShouldReturnRequested(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .OrderByDescending(p => p.Date);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy + "&desc=true").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("METHOD")]
        [DataRow("MeThOd")]
        [DataRow("METH")]
        [DataRow("meTH")]
        [DataRow("PAYMENTMETHOD")]
        [DataRow("paymentMethod")]
        [DataRow("PAYMETHOD")]
        [DataRow("payMethod")]
        [DataRow("PAYMENTMETH")]
        [DataRow("paymentMeth")]
        [DataRow("PAYMETH")]
        [DataRow("payMeth")]
        [DataRow("PAYMENT_METHOD")]
        [DataRow("payment_method")]
        [DataRow("PAYMENT_METH")]
        [DataRow("payment_meth")]
        [DataRow("PAY_METHOD")]
        [DataRow("pay_method")]
        [DataRow("PAY_METH")]
        [DataRow("pay_meth")]
        public void GetAllSorted_ByMethod_ShouldReturnRequested(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .OrderBy(p => p.PaymentMethodId);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("METHOD")]
        [DataRow("MeThOd")]
        [DataRow("METH")]
        [DataRow("meTH")]
        [DataRow("PAYMENTMETHOD")]
        [DataRow("paymentMethod")]
        [DataRow("PAYMETHOD")]
        [DataRow("payMethod")]
        [DataRow("PAYMENTMETH")]
        [DataRow("paymentMeth")]
        [DataRow("PAYMETH")]
        [DataRow("payMeth")]
        [DataRow("PAYMENT_METHOD")]
        [DataRow("payment_method")]
        [DataRow("PAYMENT_METH")]
        [DataRow("payment_meth")]
        [DataRow("PAY_METHOD")]
        [DataRow("pay_method")]
        [DataRow("PAY_METH")]
        [DataRow("pay_meth")]
        public void GetAllSortedDescending_ByMethod_ShouldReturnRequested(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .OrderByDescending(p => p.PaymentMethodId);

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy + "&desc=true").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("M76fgtD")]
        [DataRow(null)]
        public void GetAllSorted_BadQuery_ShouldReturnAll(string sortBy)
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            var result = _client.GetAsync(_routePrefix + "?sortBy=" + sortBy).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .OrderBy(p => p.PaymentMethodId).Skip(5).Take(5);

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=5&pgIndex=2&sortBy=meth").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .OrderByDescending(p => p.PaymentMethodId).Skip(5).Take(5);

            // Act
            var result = _client.GetAsync(_routePrefix + "?pgSize=5&pgIndex=2&sortBy=meth&desc=true").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.PaymentMethodId == 1).OrderBy(p => p.Amount);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=card&sortBy=amount").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.PaymentMethodId == 1).OrderByDescending(p => p.Amount);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=card&sortBy=amount&desc=true").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllBeforeAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var date = "1Jan2019";
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Date < DateTime.Parse(date)).OrderBy(p => p.Amount);

            // Act
            var result = _client.GetAsync(_routePrefix + "?date=" + date + "&before=true&sortBy=amount").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllBeforeAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var date = "1Jan2019";
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Date < DateTime.Parse(date)).OrderByDescending(p => p.Amount);

            // Act
            var result = _client.GetAsync(_routePrefix + "?date=" + date + "&before=true&sortBy=amount&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllAfterAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var date = "1Jan2019";
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Date > DateTime.Parse(date)).OrderBy(p => p.Amount);

            // Act
            var result = _client.GetAsync(_routePrefix + "?date=" + date + "&before=false&sortBy=amount").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllAfterAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var date = "1Jan2019";
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Date > DateTime.Parse(date)).OrderByDescending(p => p.Amount);

            // Act
            var result = _client.GetAsync(_routePrefix + "?date=" + date + "&before=false&sortBy=amount&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Amount < 1000m).OrderBy(p => p.Date);

            // Act
            var result = _client.GetAsync(_routePrefix + "?amount=1000&over=false&sortBy=date").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Amount < 1000m).OrderByDescending(p => p.Date);

            // Act
            var result = _client.GetAsync(_routePrefix + "?amount=1000&over=false&sortBy=date&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Amount > 1000m).OrderBy(p => p.Date);

            // Act
            var result = _client.GetAsync(_routePrefix + "?amount=1000&over=true&sortBy=date").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Amount > 1000m).OrderByDescending(p => p.Date);

            // Act
            var result = _client.GetAsync(_routePrefix + "?amount=1000&over=true&sortBy=date&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.PaymentMethodId == 1).OrderBy(p => p.Amount).Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=card&pgSize=2&pgIndex=2&sortBy=amount")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.PaymentMethodId == 1).OrderByDescending(p => p.Amount).Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?searchBy=card&pgSize=2&pgIndex=2&sortBy=amount&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllBeforeSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Date < new DateTime(2017, 1, 1)).OrderBy(p => p.Amount).Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?date=1Jan2017&before=true&pgSize=2&pgIndex=2&sortBy=amount")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllBeforeSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Date < new DateTime(2017, 1, 1)).OrderByDescending(p => p.Amount).Skip(2).Take(2);


            // Act
            var result = _client.GetAsync(_routePrefix + "?date=1Jan2017&before=true&pgSize=2&pgIndex=2&sortBy=amount&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllAfterSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Date > new DateTime(2019, 1, 1)).OrderBy(p => p.Amount).Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?date=1Jan2019&before=false&pgSize=2&pgIndex=2&sortBy=amount")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllAfterSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>()
                .Result.Where(p => p.Date < new DateTime(2017, 1, 1)).OrderByDescending(p => p.Amount).Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?date=1Jan2017&before=true&pgSize=2&pgIndex=2&sortBy=amount&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;
            var expected = allRecords.Skip(39).Take(1).Append(allRecords.Skip(16).FirstOrDefault());

            // Act
            var result = _client.GetAsync(_routePrefix + "?amount=1000&over=true&pgSize=2&pgIndex=2&sortBy=date")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var allRecords = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;
            var expected = allRecords.Skip(22).Take(1).Append(allRecords.Skip(7).FirstOrDefault());

            // Act
            var result = _client.GetAsync(_routePrefix + "?amount=1000&over=true&pgSize=2&pgIndex=2&sortBy=date&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            {
                // Arrange
                var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                    .Where(p => p.Amount < 200).OrderBy(p => p.Date).Skip(2).Take(2);

                // Act
                var result = _client.GetAsync(_routePrefix + "?amount=200&over=false&pgSize=2&pgIndex=2&sortBy=date")
                    .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

                // Assert
                result.Should().BeEquivalentTo(expected);
            }
        }

        [TestMethod]
        public void GetAllUnderSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.Amount < 200).OrderByDescending(p => p.Date).Skip(2).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?amount=200&over=false&pgSize=2&pgIndex=2&sortBy=date&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region ByCustomerId

        [TestMethod]
        public void GetAllByCustomerId_HappyFlow_ShouldReturnMatches()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4").Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerId_InexistentId_ShouldReturnNull()
        {
            // Arrange
            var custId = 0;
            while (true)
            {
                if (_client.GetAsync(_routePrefix.Replace("payments", "customers") + "?custId=" + custId).Result.StatusCode == HttpStatusCode.OK)
                    custId = (new Random()).Next(1, int.MaxValue);
                else break;
            }

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=" + custId).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetAllByCustomerIdAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = new List<Payment>()
                { _client.GetAsync(_routePrefix+"?payId=49").Result.Content.ReadAsAsync<Payment>().Result };

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&pgSize=2&pgIndex=2").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndFiltered_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4 && p.PaymentMethodId == 2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&searchBy=cash").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndBefore_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4 && p.Date < new DateTime(2019, 1, 1));

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&date=1Jan2019&before=true").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndAfter_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>()
                .Result.Where(p => p.CustomerId == 4 && p.Date > new DateTime(2018, 1, 1));

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&date=1Jan2018&before=false").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndOver_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4).Skip(1).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&amount=300&over=true").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndUnder_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&amount=1500&over=false").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4).OrderBy(p => p.PaymentMethodId);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&sortBy=meth").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4).OrderByDescending(p => p.PaymentMethodId);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&sortBy=meth&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4).Skip(2).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&searchBy=cash paypal&pgSize=1&pgIndex=2").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdBeforeAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4).Skip(1).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&date=1Jan2019&before=true&pgSize=1&pgIndex=2").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAfterAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4 && p.Date > new DateTime(2018, 1, 1)).Skip(1).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&date=1Jan2018&before=false&pgSize=1&pgIndex=2").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdOverAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4).Skip(2).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&amount=300&over=true&pgSize=1&pgIndex=2").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdUnderAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4).Skip(1).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&amount=1500&over=false&pgSize=1&pgIndex=2").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4 && (p.PaymentMethodId == 2 || p.PaymentMethodId == 3))
                .OrderBy(p => p.Amount);

            // Act
            var fRes = _client.GetAsync(_routePrefix + "?custId=4&searchBy=cash paypal&sortBy=amount").Result;
            var result = fRes.Content.ReadAsAsync<IEnumerable<Payment>>().Result.ToList();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4 && (p.PaymentMethodId == 2 || p.PaymentMethodId == 3))
                .OrderByDescending(p => p.Amount);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&searchBy=cash paypal&sortBy=amount&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdBeforeAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&date=1Jan2019&before=true&sortBy=amount").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdBeforeAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4).Take(2).Reverse();

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&date=1Jan2019&before=true&sortBy=amount&desc=true").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAfterAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4 && p.Date > new DateTime(2018, 1, 1)).OrderBy(p => p.Amount);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&date=1Jan2018&before=false&sortBy=amount").Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAfterAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4 && p.Date > new DateTime(2018, 1, 1)).OrderByDescending(p => p.Amount);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&date=1Jan2018&before=false&sortBy=amount&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdOverAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                    .Where(p => p.CustomerId == 4 && p.Amount>300).OrderBy(p=>p.Date);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&amount=300&over=true&sortBy=date").Result.Content
                    .ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdOverAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                    .Where(p => p.CustomerId == 4).Skip(1).Take(2).Reverse();

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&amount=300&over=true&sortBy=date&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdUnderAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                    .Where(p => p.CustomerId == 4).Take(2).Reverse();

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&amount=1500&over=false&sortBy=date")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdUnderAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                    .Where(p => p.CustomerId == 4).Take(2);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&amount=1500&over=false&sortBy=date&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                    .Where(p => p.CustomerId == 4).Skip(2).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&pgSize=2&pgIndex=2&sortBy=amount")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                    .Where(p => p.CustomerId == 4).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&pgSize=2&pgIndex=2&sortBy=amount&desc=true")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content
                .ReadAsAsync<IEnumerable<Payment>>().Result
                .Where(p => p.CustomerId == 4 && (p.PaymentMethodId == 2 || p.PaymentMethodId == 3))
                .OrderBy(p => p.Date).Skip(1).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&searchBy=cash paypal&pgSize=1&pgIndex=2&sortBy=date")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdBeforeSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                    .Where(p => p.CustomerId == 4 && p.Date < new DateTime(2019, 1, 1))
                    .OrderBy(p => p.Date).Skip(1).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&date=1Jan2019&before=true&pgSize=1&pgIndex=2&sortBy=date")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAfterSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                    .Where(p => p.CustomerId == 4 & p.Date > new DateTime(2018, 1, 1))
                    .OrderBy(p => p.PaymentMethodId).Skip(1).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&date=1Jan2018&before=false&pgSize=1&pgIndex=2&sortBy=meth")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdOverSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                    .Where(p => p.CustomerId == 4 && p.Amount>300)
                    .OrderBy(p=>p.PaymentMethodId).Skip(1).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&amount=300&over=true&pgSize=1&pgIndex=2&sortBy=meth")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdUnderSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _client.GetAsync(_routePrefix).Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result
                    .Where(p => p.CustomerId == 4 && p.Amount<1500).OrderBy(p=>p.Date).Skip(1).Take(1);

            // Act
            var result = _client.GetAsync(_routePrefix + "?custId=4&amount=1500&over=false&pgSize=1&pgIndex=2&sortBy=date")
                .Result.Content.ReadAsAsync<IEnumerable<Payment>>().Result;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #endregion

    }
}
