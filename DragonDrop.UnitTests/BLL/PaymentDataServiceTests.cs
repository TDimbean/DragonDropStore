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
    public class PaymentDataServiceTests
    {
        private IPaymentDataService _service;
        private IPaymentRepository _repo;
        private TestDb _db;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestPaymentRepo(_db);
            _service = new PaymentDataService(_repo);
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldReturnTarget()
        {
            var target = _db.Payments.FirstOrDefault();

            var result = _service.Get(target.PaymentId);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNull()
        {
            var id = Generators.GenInexistentPaymentId();

            var result = _service.Get(id);

            result.Should().BeNull();
        }

        #region Creates

        [TestMethod]
        public void Create_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenPayment();

            // Act
            var result = _service.Create(newItem, true);
            var createdItem = _service.Get(newItem.PaymentId);
            // Assert
            result.Should().BeNull();
            createdItem.Should().BeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Create_InexistentCustomerId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.CustomerId = Generators.GenInexistentCustomerId();

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdPayment = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("Payments require a valid CustomerId.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public void Create_InexistentPaymentMethodIdAsync_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.PaymentMethodId = _db.PaymentMethods.Count() + 1;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdPayment = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("Payment Method not found; Please submit a valid Payment Method ID.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public void Create_DateNull_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Date = null;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdPayment = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("A payment's Date cannot be blank.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public void Create_DateAfterToday_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdPayment = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public void Create_AmountZero_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Amount = 0m;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdPayment = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("Amount must be greater than 0.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public void Create_AmountNull_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Amount = null;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdPayment = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("Amount must be greater than 0.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public void Create_AmountNegative_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Amount = -3m;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdPayment = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("Amount must be greater than 0.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public void Create_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Amount = -3m;
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = _service.Create(newItem, true).Trim();
            var err1 = result.GetUntilOrEmpty(".").Trim();
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();
            var createdPayment = _service.Get(newItem.PaymentId);

            // Assert
            err1.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            err2.Should().Be("Amount must be greater than 0.");
            blank.Should().BeEmpty();
            createdPayment.Should().BeNull();
        }

        #endregion

        #region Updates

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var oldItem = new Payment
            {
                PaymentId = target.PaymentId,
                CustomerId=target.CustomerId,
                PaymentMethodId=target.PaymentMethodId,
                Amount=target.Amount,
                Date=target.Date
            };
            var targetId = target.PaymentId;
            var newItem = Generators.GenPayment();
            newItem.PaymentId = targetId;

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
            var targetId = Generators.GenInexistentPaymentId();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = targetId;
            var oldRepo = _service.GetAll();

            // Act
            var result = _service.Update(newItem, true).Trim();
            var newRepo = _service.GetAll();

            // Assert
            result.Should().Be("No Payment with ID: " + newItem.PaymentId + " found in Repo.");
            newRepo.Should().BeEquivalentTo(oldRepo);
        }

        [TestMethod]
        public void Update_InexistentCustomerId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.CustomerId = Generators.GenInexistentCustomerId();

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedOrder = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("Payments require a valid CustomerId.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_InexistentPaymentMethodIdAsync_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.PaymentMethodId = _db.PaymentMethods.Count() + 1;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedOrder = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("Payment Method not found; Please submit a valid Payment Method ID.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NullDate_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.Date = null;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedOrder = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("A payment's Date cannot be blank.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_DateAfterToday_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedOrder = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NegativeAmount_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.Amount = -3m;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedOrder = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("Amount must be greater than 0.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NullAmount_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.Amount = null;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedOrder = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("Amount must be greater than 0.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_AmountZero_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.Amount = 0m;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedOrder = _service.Get(newItem.PaymentId);

            // Assert
            result.Should().Be("Amount must be greater than 0.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.Amount = -3;
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = _service.Update(newItem, true).Trim();
            var err1 = result.GetUntilOrEmpty(".").Trim();
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").Trim();
            var updatedPayment = _service.Get(newItem.PaymentId);

            // Assert
            err1.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            err2.Should().Be("Amount must be greater than 0.");
            blank.Should().BeEmpty();
            updatedPayment.Should().BeEquivalentTo(target);
            updatedPayment.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region Gets

        [TestMethod]
        public void GetAll_HappyFlow_ShouldFetchAll()
        {
            var result = _service.GetAll();

            result.Should().BeEquivalentTo(_db.Payments);
        }

        #region Paginated

        [TestMethod]
        public void GetAllPaginated_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            int pgSize = 1;
            int pgIndex = 3;
            var expected = _db.Payments.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();

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
        public void GetAllFiltered_AmountMatch_ShouldReturnMatch()
        {
            // Arrange
            var uniAmnt = 1m;
            var rnd = new Random();
            while (true)
            {
                if (_db.Payments.Any(p => p.Amount == uniAmnt)) uniAmnt = (decimal)rnd.Next(1, 201);
                else break;
            }

            var targetItem = Generators.GenPayment();
            targetItem.Amount = uniAmnt;
            _service.Create(targetItem);

            var expected = new List<Payment> { targetItem };

            // Act
            var result = _service.GetAllFiltered(targetItem.Amount.ToString());

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_CustomerNameMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Payments.FirstOrDefault();
            var expected = new List<Payment> { targetItem };

            var result = _service.GetAllFiltered(_db.Customers.SingleOrDefault(c => c.CustomerId == targetItem.CustomerId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_DateMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Payments.FirstOrDefault();
            var expected = new List<Payment> { targetItem };

            var result = _service.GetAllFiltered(targetItem.Date.GetValueOrDefault().ToShortDateString());

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_MethodMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Payments.FirstOrDefault();
            var expected = new List<Payment> { targetItem };

            var result = _service.GetAllFiltered(_db.PaymentMethods.SingleOrDefault(m => m.PaymentMethodId == targetItem.PaymentMethodId).Name);

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
            var target1 = Generators.GenPayment();
            var target2 = Generators.GenPayment();

            #region Generate unique Amounts as Identifiers, then add new Payments to the repo to serve as targets
            var rnd = new Random();
            var amnt1 = 1m;
            var amnt2 = 1m;

            while (true)
            {
                if (_db.Payments.Any(p => p.Amount == amnt1)) amnt1 = (decimal)rnd.Next(1, 201);
                else break;
            }

            while (true)
            {
                if (_db.Payments.Any(p => p.Amount == amnt2)) amnt2 = (decimal)rnd.Next(1, 201);
                else break;
            }

            target1.Amount = amnt1;
            target2.Amount = amnt2;

            _service.Create(target1);
            _service.Create(target2);
            #endregion

            var search = amnt1.ToString() + " " + amnt2.ToString();
            var expected = new List<Payment> { target1, target2 };

            // Act
            var result = _service.GetAllFiltered(search);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_HappyFlow_ShouldReturnMatch()
        {
            var expected = new List<Payment> { _service.Get(2) };

            var result = _service.GetAllFilteredAndPaged(_db.Payments.FirstOrDefault().Amount.ToString(), 1, 2);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_BeforeDate_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.LastOrDefault();
            var expected = _db.Payments.ToList();
            expected.Remove(target);

            // Act
            var result = _service.GetAllFiltered(target.Date.GetValueOrDefault(), true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_AfterDate_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var expected = _db.Payments.ToList();
            expected.Remove(target);

            // Act
            var result = _service.GetAllFiltered(target.Date.GetValueOrDefault(), false);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_OverAmount_ShouldReturnMatches()
        {
            var expected = _db.Payments.Skip(2).Take(1);

            var result = _service.GetAllFiltered(25.59m, true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_UnderAmount_ShouldReturnMatches()
        {
            var expected = _db.Payments.Take(2);

            var result = _service.GetAllFiltered(60m, false);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_DatesAndAmountsOutOfBounds_ShouldReturnEmpty()
        {
            // Arrange
            var lowAmnt = _db.Payments.FirstOrDefault().Amount.GetValueOrDefault();
            var highAmnt = _db.Payments.LastOrDefault().Amount.GetValueOrDefault();
            var earlyDate = _db.Payments.FirstOrDefault().Date.GetValueOrDefault();
            var lateDate = _db.Payments.LastOrDefault().Date.GetValueOrDefault();

            // Act
            var result = _service.GetAllFiltered(lowAmnt, false).ToList();
            result.AddRange(_service.GetAllFiltered(highAmnt, true));
            result.AddRange(_service.GetAllFiltered(earlyDate, true));
            result.AddRange(_service.GetAllFiltered(lateDate, false));

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_BeforeDate_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.LastOrDefault();
            var expected = new List<Payment> { _service.Get(2) };

            // Act
            var result = _service.GetAllFilteredAndPaged(target.Date.GetValueOrDefault(), true, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_AfterDate_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var expected = new List<Payment> { _service.Get(3) };

            // Act
            var result = _service.GetAllFilteredAndPaged(target.Date.GetValueOrDefault(), false, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_OverAmount_ShouldReturnMatches()
        {
            // Arrange

            _db.BumpPayments();

            var expected = _db.Payments.Skip(5).Take(1);

            // Act
            var result = _service.GetAllFilteredAndPaged(25.59m, true, 3, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_UnderAmount_ShouldReturnMatches()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Where(p => p.PaymentId % 3 == 0);

            // Act
            var result = _service.GetAllFilteredAndPaged(74.59m, false, 2, 2);

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
        public void GetAllSorted_ByIdAsync_ShouldReturnRequested(string sortBy)
        {
            var expected = _db.Payments;

            var result = _service.GetAllSorted(sortBy);

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
        public void GetAllSortedDescending_ByIdAsync_ShouldReturnRequested(string sortBy)
        {
            var expected = _db.Payments;
            expected.Reverse();

            var result = _service.GetAllSorted(sortBy, true);

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
        public void GetAllSorted_ByCustomerIdAsync_ShouldReturnRequested(string sortBy)
        {
            var expected = _db.Payments;
            expected.Reverse();

            var result = _service.GetAllSorted(sortBy);

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
        public void GetAllSortedDescending_ByCustomerIdAsync_ShouldReturnRequested(string sortBy)
        {
            var expected = _db.Payments;

            var result = _service.GetAllSorted(sortBy, true);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("AMT")]
        [DataRow("aMt")]
        [DataRow("AMOUNT")]
        [DataRow("amoUNt")]
        public void GetAllSorted_ByAmount_ShouldReturnRequested(string sortBy)
        {
            var expected = _db.Payments;

            var result = _service.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("AMT")]
        [DataRow("aMt")]
        [DataRow("AMOUNT")]
        [DataRow("amoUNt")]
        public void GetAllSortedDescending_ByAmount_ShouldReturnRequested(string sortBy)
        {
            var expected = _db.Payments;
            expected.Reverse();

            var result = _service.GetAllSorted(sortBy, true);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("DATE")]
        [DataRow("daTe")]
        public void GetAllSorted_ByDate_ShouldReturnRequested(string sortBy)
        {
            var expected = _db.Payments;

            var result = _service.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("DATE")]
        [DataRow("daTe")]
        public void GetAllSortedDescending_ByDate_ShouldReturnRequested(string sortBy)
        {
            var expected = _db.Payments;
            expected.Reverse();

            var result = _service.GetAllSorted(sortBy);

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
            var expected = _db.Payments;

            var result = _service.GetAllSorted(sortBy);

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
            var expected = _db.Payments;
            expected.Reverse();

            var result = _service.GetAllSorted(sortBy, true);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("M76fgtD")]
        [DataRow(null)]
        public void GetAllSorted_BadQuery_ShouldReturnAll(string sortBy)
        {
            var expected = _db.Payments;

            var result = _service.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            var expected = new List<Payment> { _db.Payments.FirstOrDefault() };

            var result = _service.GetAllSortedAndPaged(2, 2, "cust");

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            var expected = new List<Payment> { _db.Payments.FirstOrDefault() };

            var result = _service.GetAllSortedAndPaged(2, 2, "id", true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            var expected = _db.Payments.Take(2);
            expected.Reverse();

            var result = _service.GetAllFilteredAndSorted("25.59", "cust");

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            var expected = _db.Payments.Take(2);

            var result = _service.GetAllFilteredAndSorted("25.59", "date", true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllBeforeAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _db.Payments.Take(2);
            expected.Reverse();

            var when = new DateTime(2019, 9, 13);

            // Act
            var result = _service.GetAllFilteredAndSorted(when, true, "cust");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllBeforeAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _db.Payments.Take(2);

            var when = new DateTime(2019, 9, 13);

            // Act
            var result = _service.GetAllFilteredAndSorted(when, true, "cust", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllAfterAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _db.Payments.Skip(1).Take(2);
            expected.Reverse();

            var when = new DateTime(2019, 9, 10);

            // Act
            var result = _service.GetAllFilteredAndSorted(when, false, "cust");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllAfterAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _db.Payments.Skip(1).Take(2);
            expected.Reverse();

            var when = new DateTime(2019, 9, 10);

            // Act
            var result = _service.GetAllFilteredAndSorted(when, false, "amt", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _db.Payments.Take(2);
            expected.Reverse();

            var amt = 60.00m;

            // Act
            var result = _service.GetAllFilteredAndSorted(amt, false, "cust");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            var expected = _db.Payments.Take(2);
            expected.Reverse();

            var amt = 60m;

            // Act
            var result = _service.GetAllFilteredAndSorted(amt, false, "date", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(2).Reverse();

            var amt = 60m;

            // Act
            var result = _service.GetAllFilteredAndSorted(amt, true, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(2);
            expected.Reverse();

            var amt = 60m;

            // Act
            var result = _service.GetAllFilteredAndSorted(amt, true, "id", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(1);

            // Act
            var result = _service.GetAllFilteredSortedAndPaged("Ced", 2, 2, "amt");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(4).Take(1);

            // Act
            var result = _service.GetAllFilteredSortedAndPaged("Ced", 2, 2, "date", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllBeforeSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(5);

            var when = new DateTime(2019, 9, 10);

            // Act
            var result = _service.GetAllFilteredSortedAndPaged(when, true, 2, 2, "id");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllBeforeSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(5);

            var when = new DateTime(2019, 9, 10);

            // Act
            var result = _service.GetAllFilteredSortedAndPaged(when, true, 2, 2, "cust", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllAfterSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(2).Take(1).Append(_db.Payments.Skip(8).FirstOrDefault());

            var when = new DateTime(2019, 6, 12);

            // Act
            var result = _service.GetAllFilteredSortedAndPaged(when, false, 2, 2, "meth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllAfterSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Take(2);

            var when = new DateTime(2019, 3, 19);

            // Act
            var result = _service.GetAllFilteredSortedAndPaged(when, false, 2, 2, "amt", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(2).Take(2);

            var amt = 25.59m;

            // Act
            var result = _service.GetAllFilteredSortedAndPaged(amt, true, 2, 2, "meth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(4);

            var amt = 25.59m;

            // Act
            var result = _service.GetAllFilteredSortedAndPaged(amt, true, 2, 2, "meth", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(5).Take(1).Append(_db.Payments.Skip(2).FirstOrDefault());

            var amt = 74.59m;

            // Act
            var result = _service.GetAllFilteredSortedAndPaged(amt, false, 2, 2, "meth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Take(2);

            var amt = 74.59m;

            // Act
            var result = _service.GetAllFilteredSortedAndPaged(amt, false, 2, 2, "amt", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region ByCustomerId

        [TestMethod]
        public void GetAllByCustomerId_HappyFlow_ShouldReturnMatches()
        {
            // Arrange
            var target1 = _db.Payments.FirstOrDefault();

            var target2 = Generators.GenPayment();
            target2.CustomerId = target1.CustomerId;
            _service.Create(target2);

            var expected = new List<Payment> { target1, target2 };

            // Act
            var result = _service.GetAllByCustomerId(target1.CustomerId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerId_InexistentId_ShouldReturnNull()
        {
            var custId = Generators.GenInexistentCustomerId();

            var result = _service.GetAllByCustomerId(custId);

            result.Should().BeNull();
        }
        
        [TestMethod]
        public void GetAllByCustomerIdAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(5).Take(1);

            // Act
            var result = _service.GetAllByCustomerIdAndPaged(3, 2, 2);

            // Assert
        }

        [TestMethod]
        public void GetAllByCustomerIdAndFiltered_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(4).Take(1);

            // Act
            var result = _service.GetAllByCustomerIdAndFiltered(3, "cash");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndBefore_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(2);

            var when = new DateTime(2019, 9, 10);

            // Act
            var result = _service.GetAllByCustomerIdAndFiltered(3, when, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndAfter_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Take(1);

            var when = new DateTime(2019, 3, 21);

            // Act
            var result = _service.GetAllByCustomerIdAndFiltered(3, when, false);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndOver_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(2);

            var amt = 60m;

            // Act
            var result = _service.GetAllByCustomerIdAndFiltered(3, amt, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndUnder_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(4).Take(1).Append(_db.Payments.FirstOrDefault());

            var amt = 120m;

            // Act
            var result = _service.GetAllByCustomerIdAndFiltered(3, amt, false);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(2).Reverse().Append(_db.Payments.FirstOrDefault());
            expected.Reverse();

            // Act
            var result = _service.GetAllByCustomerIdAndSorted(3, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(2).Append(_db.Payments.FirstOrDefault());

            // Act
            var result = _service.GetAllByCustomerIdAndSorted(3, "amt", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(1);

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndPaged(3, "Cash", 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdBeforeAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(6).Take(2);

            var when = new DateTime(2019, 6, 15);

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndPaged(3, when, true, 2, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAfterAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(8).Take(1);

            var when = new DateTime(2019, 6, 10);

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndPaged(3, when, false, 2, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdOverAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(2);

            var amt = 63.49m;

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndPaged(3, amt, true, 2, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdUnderAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(6).Take(2);

            var amt = 80.59m;

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndPaged(3, amt, false, 2, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(4).Take(1).Append(_db.Payments.Skip(7).FirstOrDefault());

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndSorted(3, "Cash", "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(1).Append(_db.Payments.Skip(4).FirstOrDefault());

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndSorted(3, "Cash", "date", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdBeforeAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(3).Take(2).Reverse();

            var when = new DateTime(2019, 6, 10);

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndSorted(3, when, true, "meth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdBeforeAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(3).Take(2);

            var when = new DateTime(2019, 6, 10);

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndSorted(3, when, true, "meth", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAfterAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Take(1).Append(_db.Payments.LastOrDefault());

            var when = new DateTime(2019, 6, 12);

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndSorted(3, when, false, "payMeth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdAfterAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Take(1).Append(_db.Payments.LastOrDefault());

            var when = new DateTime(2019, 6, 12);

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndSorted(3, when, false, "date", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdOverAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(3).Take(1).Append(_db.Payments.Skip(8).FirstOrDefault());

            var amt = 74.59m;

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndSorted(3, amt, true, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdOverAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(3).Take(1).Append(_db.Payments.Skip(8).FirstOrDefault());

            var amt = 74.59m;

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndSorted(3, amt, true, "meth", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdUnderAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(6).Take(1).Append(_db.Payments.FirstOrDefault());

            var amt = 74.49m;

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndSorted(3, amt, false, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdUnderAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Take(1).Append(_db.Payments.Skip(6).FirstOrDefault());

            var amt = 74.49m;

            // Act
            var result = _service.GetAllByCustomerIdFilteredAndSorted(3, amt, false, "date", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(1);

            // Act
            var result = _service.GetAllByCustomerIdSortedAndPaged(3, 2, 2, "meth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(4).Take(1);

            // Act
            var result = _service.GetAllByCustomerIdSortedAndPaged(3, 2, 2, "date", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdFilteredSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(1);

            // Act
            var result = _service.GetAllByCustomerIdFilteredSortedAndPaged(3, "cash", 1, 2, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdBeforeSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(1).Append(_db.Payments.Skip(3).FirstOrDefault());

            var when = new DateTime(2019, 6, 15);

            // Act
            var result = _service.GetAllByCustomerIdFilteredSortedAndPaged(3, when, true, 2, 2, "amt");

            // Assert
        }

        [TestMethod]
        public void GetAllByCustomerIdAfterSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(2);

            var when = new DateTime(2019, 3, 21);

            // Act
            var result = _service.GetAllByCustomerIdFilteredSortedAndPaged(3, when, false, 2, 2, "meth");

            // Assert
        }

        [TestMethod]
        public void GetAllByCustomerIdOverSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(8).Take(1).Append(_db.Payments.Skip(3).FirstOrDefault());

            var amt = 74.49m;

            // Act
            var result = _service.GetAllByCustomerIdFilteredSortedAndPaged(3, amt, false, 2, 2, "meth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerIdUnderSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(1).Append(_db.Payments.FirstOrDefault());

            var amt = 80.59m;

            // Act
            var result = _service.GetAllByCustomerIdFilteredSortedAndPaged(3, amt, true, 2, 2, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #endregion

        #region Validates

        #region Whole Payment Validation

        [TestMethod]
        public void ValidatePayment_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var newItem = Generators.GenPayment();

            // Act
            var result = _service.ValidatePayment(newItem);

            // Assert
            result.errorList.Trim().Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidatePayment_InexistentCustomerId_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.CustomerId = Generators.GenInexistentCustomerId();

            // Act
            var result = _service.ValidatePayment(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Payments require a valid CustomerId.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePayment_InexistentPaymentMethodIdAsync_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.PaymentMethodId = _db.PaymentMethods.Count() + 1;

            // Act
            var result = _service.ValidatePayment(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Payment Method not found; Please submit a valid Payment Method ID.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePayment_DateNull_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Date = null;

            // Act
            var result = _service.ValidatePayment(newItem);

            // Assert
            result.errorList.Trim().Should().Be("A payment's Date cannot be blank.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePayment_DateAfterToday_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = _service.ValidatePayment(newItem);

            // Assert
            result.errorList.Trim().Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePayment_AmountZero_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Amount = 0m;

            // Act
            var result = _service.ValidatePayment(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Amount must be greater than 0.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePayment_AmountNull_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Amount = null;

            // Act
            var result = _service.ValidatePayment(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Amount must be greater than 0.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePayment_AmountNegative_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Amount = -3m;

            // Act
            var result = _service.ValidatePayment(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Amount must be greater than 0.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidatePayment_MultipleErrors_ShouldReturnFalseAndAppropriateErrors()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Amount = -3m;
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = _service.ValidatePayment(newItem);
            var err1 = result.errorList.GetUntilOrEmpty(".").Trim();
            var err2 = result.errorList.Replace(err1, "").GetUntilOrEmpty(".").Trim();
            var blank = result.errorList.Replace(err1, "").Replace(err2, "").Trim();
            var createdPayment = _service.Get(newItem.PaymentId);

            // Assert
            err1.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            err2.Should().Be("Amount must be greater than 0.");
            blank.Should().BeEmpty();
            result.isValid.Should().BeFalse();
        }

        #endregion

        #region PropertyValidation

        [TestMethod]
        public void ValidateCustomerId_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var custId = _db.Customers.FirstOrDefault().CustomerId;

            // Act
            var result = _service.ValidateCustomerId(custId);

            // Assert
            result.errorList.Trim().Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateCustomerId_Inexistent_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var badId = Generators.GenInexistentCustomerId();

            // Act
            var result = _service.ValidateCustomerId(badId);

            // Assert
            result.errorList.Trim().Should().Be("Payments require a valid CustomerId.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateAmount_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var amount = (decimal)new Random().Next(2, 100);

            // Act
            var result = _service.ValidateAmount(amount);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateAmount_AmountZero_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var amount = 0m;

            // Act
            var result = _service.ValidateAmount(amount);

            // Assert
            result.errorList.Trim().Should().Be("Amount must be greater than 0.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateAmount_AmountNull_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var amount = (decimal?)null;

            // Act
            var result = _service.ValidateAmount(amount);

            // Assert
            result.errorList.Trim().Should().Be("Amount must be greater than 0.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateAmount_NegativeAmount_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var amount = -3m;

            // Act
            var result = _service.ValidateAmount(amount);

            // Assert
            result.errorList.Trim().Should().Be("Amount must be greater than 0.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateDate_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            var result = _service.ValidateDate(DateTime.Now);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateDate_DateNull_ShouldReturnFalseAndAppropriateError()
        {
            var result = _service.ValidateDate((DateTime?)null);

            // Assert
            result.errorList.Trim().Should().Be("A payment's Date cannot be blank.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateDate_DateAfterToday_ShouldReturnFalseAndAppropriateError()
        {
            var result = _service.ValidateDate(DateTime.Now.AddDays(3));

            // Assert
            result.errorList.Trim().Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            result.isValid.Should().BeFalse();
        }

        #endregion

        #endregion

        #region Async

        [TestMethod]
        public async Task GetAsync_HappyFlow_ShouldReturnTarget()
        {
            var target = _db.Payments.FirstOrDefault();

            var result = await _service.GetAsync(target.PaymentId);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public async Task GetAsync_InexistentId_ShouldReturnNull()
        {
            var id = Generators.GenInexistentPaymentId();

            var result = await _service.GetAsync(id);

            result.Should().BeNull();
        }

        #region Creates

        [TestMethod]
        public async Task CreateAsync_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenPayment();

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdItem = await _service.GetAsync(newItem.PaymentId);
            // Assert
            result.Should().BeNull();
            createdItem.Should().BeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task CreateAsync_InexistentCustomerId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.CustomerId = Generators.GenInexistentCustomerId();

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdPayment = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("Payments require a valid CustomerId.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_InexistentPaymentMethodIdAsync_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.PaymentMethodId = _db.PaymentMethods.Count() + 1;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdPayment = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("Payment Method not found; Please submit a valid Payment Method ID.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_DateNull_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Date = null;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdPayment = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("A payment's Date cannot be blank.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_DateAfterToday_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdPayment = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_AmountZero_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Amount = 0m;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdPayment = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("Amount must be greater than 0.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_AmountNull_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Amount = null;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdPayment = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("Amount must be greater than 0.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_AmountNegative_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Amount = -3m;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdPayment = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("Amount must be greater than 0.");
            createdPayment.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.Amount = -3m;
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var err1 = result.GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".");
            var blank = result.Replace(err1, "").Replace(err2, "");
            var createdPayment = await _service.GetAsync(newItem.PaymentId);

            // Assert
            err1.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            err2.Should().Be("Amount must be greater than 0.");
            blank.Should().BeEmpty();
            createdPayment.Should().BeNull();
        }

        #endregion

        #region Updates

        [TestMethod]
        public async Task UpdateAsync_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var oldItem = new Payment
            {
                PaymentId = target.PaymentId,
                CustomerId = target.CustomerId,
                PaymentMethodId = target.PaymentMethodId,
                Amount = target.Amount,
                Date = target.Date
            };
            var targetId = target.PaymentId;
            var newItem = Generators.GenPayment();
            newItem.PaymentId = targetId;

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
            var targetId = Generators.GenInexistentPaymentId();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = targetId;
            var oldRepo = await _service.GetAllAsync();

            // Act
            var result = (await _service.UpdateAsync(newItem, true)).Trim();
            var newRepo = await _service.GetAllAsync();

            // Assert
            result.Should().Be("No Payment with ID: " + newItem.PaymentId + " found in Repo.");
            newRepo.Should().BeEquivalentTo(oldRepo);
        }

        [TestMethod]
        public async Task UpdateAsync_InexistentCustomerId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.CustomerId = Generators.GenInexistentCustomerId();

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedOrder = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("Payments require a valid CustomerId.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_InexistentPaymentMethodIdAsync_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.PaymentMethodId = _db.PaymentMethods.Count() + 1;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedOrder = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("Payment Method not found; Please submit a valid Payment Method ID.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_NullDate_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.Date = null;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedOrder = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("A payment's Date cannot be blank.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_DateAfterToday_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedOrder = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_NegativeAmount_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.Amount = -3m;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedOrder = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("Amount must be greater than 0.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_NullAmount_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.Amount = null;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedOrder = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("Amount must be greater than 0.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_AmountZero_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.Amount = 0m;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedOrder = await _service.GetAsync(newItem.PaymentId);

            // Assert
            result.Should().Be("Amount must be greater than 0.");
            updatedOrder.Should().BeEquivalentTo(target);
            updatedOrder.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var newItem = Generators.GenPayment();
            newItem.PaymentId = target.PaymentId;
            newItem.Amount = -3;
            newItem.Date = DateTime.Now.AddDays(3);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var err1 = result.GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".");
            var blank = result.Replace(err1, "").Replace(err2, "");
            var updatedPayment = await _service.GetAsync(newItem.PaymentId);

            // Assert
            err1.Should().Be("A payment cannot occur at a later date; Please review the submitted Payment Date.");
            err2.Should().Be("Amount must be greater than 0.");
            blank.Should().BeEmpty();
            updatedPayment.Should().BeEquivalentTo(target);
            updatedPayment.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region Gets

        [TestMethod]
        public async Task GetAllAsync_HappyFlow_ShouldFetchAll()
        {
            var result = await _service.GetAllAsync();

            result.Should().BeEquivalentTo(_db.Payments);
        }

        #region Paginated

        [TestMethod]
        public async Task GetAllPaginatedAsync_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            int pgSize = 1;
            int pgIndex = 3;
            var expected = _db.Payments.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();

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

        #region Filtered

        [TestMethod]
        public async Task GetAllFilteredAsync_AmountMatch_ShouldReturnMatch()
        {
            // Arrange
            var uniAmnt = 1m;
            var rnd = new Random();
            while (true)
            {
                if (_db.Payments.Any(p => p.Amount == uniAmnt)) uniAmnt = (decimal)rnd.Next(1, 201);
                else break;
            }

            var targetItem = Generators.GenPayment();
            targetItem.Amount = uniAmnt;
            await _service.CreateAsync(targetItem);

            var expected = new List<Payment> { targetItem };

            // Act
            var result = await _service.GetAllFilteredAsync(targetItem.Amount.ToString());

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_CustomerNameMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Payments.FirstOrDefault();
            var expected = new List<Payment> { targetItem };

            var result = await _service.GetAllFilteredAsync(_db.Customers.SingleOrDefault(c => c.CustomerId == targetItem.CustomerId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_DateMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Payments.FirstOrDefault();
            var expected = new List<Payment> { targetItem };

            var result = await _service.GetAllFilteredAsync(targetItem.Date.GetValueOrDefault().ToShortDateString());

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_MethodMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Payments.FirstOrDefault();
            var expected = new List<Payment> { targetItem };

            var result = await _service.GetAllFilteredAsync(_db.PaymentMethods.SingleOrDefault(m => m.PaymentMethodId == targetItem.PaymentMethodId).Name);

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
            var target1 = Generators.GenPayment();
            var target2 = Generators.GenPayment();

            #region Generate unique Amounts as Identifiers, then add new Payments to the repo to serve as targets
            var rnd = new Random();
            var amnt1 = 1m;
            var amnt2 = 1m;

            while (true)
            {
                if (_db.Payments.Any(p => p.Amount == amnt1)) amnt1 = (decimal)rnd.Next(1, 201);
                else break;
            }

            while (true)
            {
                if (_db.Payments.Any(p => p.Amount == amnt2)) amnt2 = (decimal)rnd.Next(1, 201);
                else break;
            }

            target1.Amount = amnt1;
            target2.Amount = amnt2;

            await _service.CreateAsync(target1);
            await _service.CreateAsync(target2);
            #endregion

            var search = amnt1.ToString() + " " + amnt2.ToString();
            var expected = new List<Payment> { target1, target2 };

            // Act
            var result = await _service.GetAllFilteredAsync(search);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_HappyFlow_ShouldReturnMatch()
        {
            var expected = new List<Payment> { await _service.GetAsync(2) };

            var result = await _service.GetAllFilteredAndPagedAsync(_db.Payments.FirstOrDefault().Amount.ToString(), 1, 2);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_BeforeDate_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.LastOrDefault();
            var expected = _db.Payments.ToList();
            expected.Remove(target);

            // Act
            var result = await _service.GetAllFilteredAsync(target.Date.GetValueOrDefault(), true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_AfterDate_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var expected = _db.Payments.ToList();
            expected.Remove(target);

            // Act
            var result = await _service.GetAllFilteredAsync(target.Date.GetValueOrDefault(), false);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_OverAmount_ShouldReturnMatches()
        {
            var expected = _db.Payments.Skip(2).Take(1);

            var result = await _service.GetAllFilteredAsync(25.59m, true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_UnderAmount_ShouldReturnMatches()
        {
            var expected = _db.Payments.Take(2);

            var result = await _service.GetAllFilteredAsync(60m, false);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_DatesAndAmountsOutOfBounds_ShouldReturnEmpty()
        {
            // Arrange
            var lowAmnt = _db.Payments.FirstOrDefault().Amount.GetValueOrDefault();
            var highAmnt = _db.Payments.LastOrDefault().Amount.GetValueOrDefault();
            var earlyDate = _db.Payments.FirstOrDefault().Date.GetValueOrDefault();
            var lateDate = _db.Payments.LastOrDefault().Date.GetValueOrDefault();

            // Act
            var lowAmtCall = await _service.GetAllFilteredAsync(lowAmnt, false);
            var result = lowAmtCall.ToList();
            result.AddRange(await _service.GetAllFilteredAsync(highAmnt, true));
            result.AddRange(await _service.GetAllFilteredAsync(earlyDate, true));
            result.AddRange(await _service.GetAllFilteredAsync(lateDate, false));

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_BeforeDate_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.LastOrDefault();
            var expected = new List<Payment> { await _service.GetAsync(2) };

            // Act
            var result = await _service.GetAllFilteredAndPagedAsync(target.Date.GetValueOrDefault(), true, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_AfterDate_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var expected = new List<Payment> { await _service.GetAsync(3) };

            // Act
            var result = await _service.GetAllFilteredAndPagedAsync(target.Date.GetValueOrDefault(), false, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_OverAmount_ShouldReturnMatches()
        {
            // Arrange

            _db.BumpPayments();

            var expected = _db.Payments.Skip(5).Take(1);

            // Act
            var result = await _service.GetAllFilteredAndPagedAsync(25.59m, true, 3, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_UnderAmount_ShouldReturnMatches()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Where(p => p.PaymentId % 3 == 0);

            // Act
            var result = await _service.GetAllFilteredAndPagedAsync(74.59m, false, 2, 2);

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
        public async Task GetAllSortedAsync_ByIdAsync_ShouldReturnRequestedAsync(string sortBy)
        {
            var expected = _db.Payments;

            var result = await _service.GetAllSortedAsync(sortBy);

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
        public async Task GetAllSortedDescending_ByIdAsync_ShouldReturnRequestedAsync(string sortBy)
        {
            var expected = _db.Payments;
            expected.Reverse();

            var result = await _service.GetAllSortedAsync(sortBy, true);

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
        public async Task GetAllSortedAsync_ByCustomerIdAsync_ShouldReturnRequestedAsync(string sortBy)
        {
            var expected = _db.Payments;
            expected.Reverse();

            var result = await _service.GetAllSortedAsync(sortBy);

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
        public async Task GetAllSortedDescending_ByCustomerIdAsync_ShouldReturnRequestedAsync(string sortBy)
        {
            var expected = _db.Payments;

            var result = await _service.GetAllSortedAsync(sortBy, true);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("AMT")]
        [DataRow("aMt")]
        [DataRow("AMOUNT")]
        [DataRow("amoUNt")]
        public async Task GetAllSortedAsync_ByAmount_ShouldReturnRequestedAsync(string sortBy)
        {
            var expected = _db.Payments;

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("AMT")]
        [DataRow("aMt")]
        [DataRow("AMOUNT")]
        [DataRow("amoUNt")]
        public async Task GetAllSortedDescending_ByAmount_ShouldReturnRequestedAsync(string sortBy)
        {
            var expected = _db.Payments;
            expected.Reverse();

            var result = await _service.GetAllSortedAsync(sortBy, true);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("DATE")]
        [DataRow("daTe")]
        public async Task GetAllSortedAsync_ByDate_ShouldReturnRequestedAsync(string sortBy)
        {
            var expected = _db.Payments;

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("DATE")]
        [DataRow("daTe")]
        public async Task GetAllSortedDescending_ByDate_ShouldReturnRequestedAsync(string sortBy)
        {
            var expected = _db.Payments;
            expected.Reverse();

            var result = await _service.GetAllSortedAsync(sortBy);

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
        public async Task GetAllSortedAsync_ByMethod_ShouldReturnRequestedAsync(string sortBy)
        {
            var expected = _db.Payments;

            var result = await _service.GetAllSortedAsync(sortBy);

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
        public async Task GetAllSortedDescending_ByMethod_ShouldReturnRequestedAsync(string sortBy)
        {
            var expected = _db.Payments;
            expected.Reverse();

            var result = await _service.GetAllSortedAsync(sortBy, true);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("M76fgtD")]
        [DataRow(null)]
        public async Task GetAllSortedAsync_BadQuery_ShouldReturnAll(string sortBy)
        {
            var expected = _db.Payments;

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            var expected = new List<Payment> { _db.Payments.FirstOrDefault() };

            var result = await _service.GetAllSortedAndPagedAsync(2, 2, "cust");

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            var expected = new List<Payment> { _db.Payments.FirstOrDefault() };

            var result = await _service.GetAllSortedAndPagedAsync(2, 2, "id", true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            var expected = _db.Payments.Take(2);
            expected.Reverse();

            var result = await _service.GetAllFilteredAndSortedAsync("25.59", "cust");

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            var expected = _db.Payments.Take(2);

            var result = await _service.GetAllFilteredAndSortedAsync("25.59", "date", true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllBeforeAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            var expected = _db.Payments.Take(2);
            expected.Reverse();

            var when = new DateTime(2019, 9, 13);

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(when, true, "cust");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllBeforeAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            var expected = _db.Payments.Take(2);

            var when = new DateTime(2019, 9, 13);

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(when, true, "cust", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllAfterAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            var expected = _db.Payments.Skip(1).Take(2);
            expected.Reverse();

            var when = new DateTime(2019, 9, 10);

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(when, false, "cust");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllAfterAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            var expected = _db.Payments.Skip(1).Take(2);
            expected.Reverse();

            var when = new DateTime(2019, 9, 10);

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(when, false, "amt", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllUnderAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            var expected = _db.Payments.Take(2);
            expected.Reverse();

            var amt = 60.00m;

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(amt, false, "cust");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllUnderAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            var expected = _db.Payments.Take(2);
            expected.Reverse();

            var amt = 60m;

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(amt, false, "date", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllOverAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(2).Reverse();

            var amt = 60m;

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(amt, true, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllOverAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(2);
            expected.Reverse();

            var amt = 60m;

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(amt, true, "id", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(1);

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync("Ced", 2, 2, "amt");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(4).Take(1);

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync("Ced", 2, 2, "date", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllBeforeSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(5);

            var when = new DateTime(2019, 9, 10);

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(when, true, 2, 2, "id");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllBeforeSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(5);

            var when = new DateTime(2019, 9, 10);

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(when, true, 2, 2, "cust", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllAfterSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(2).Take(1).Append(_db.Payments.Skip(8).FirstOrDefault());

            var when = new DateTime(2019, 6, 12);

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(when, false, 2, 2, "meth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllAfterSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Take(2);

            var when = new DateTime(2019, 3, 19);

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(when, false, 2, 2, "amt", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllOverSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(2).Take(2);

            var amt = 25.59m;

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(amt, true, 2, 2, "meth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllOverSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(4);

            var amt = 25.59m;

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(amt, true, 2, 2, "meth", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllUnderSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(5).Take(1).Append(_db.Payments.Skip(2).FirstOrDefault());

            var amt = 74.59m;

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(amt, false, 2, 2, "meth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllUnderSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Take(2);

            var amt = 74.59m;

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(amt, false, 2, 2, "amt", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region ByCustomerId

        [TestMethod]
        public async Task GetAllByCustomerIdAsync_HappyFlow_ShouldReturnMatches()
        {
            // Arrange
            var target1 = _db.Payments.FirstOrDefault();

            var target2 = Generators.GenPayment();
            target2.CustomerId = target1.CustomerId;
            await _service.CreateAsync(target2);

            var expected = new List<Payment> { target1, target2 };

            // Act
            var result = await _service.GetAllByCustomerIdAsync(target1.CustomerId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAsync_InexistentId_ShouldReturnNull()
        {
            var custId = Generators.GenInexistentCustomerId();

            var result = await _service.GetAllByCustomerIdAsync(custId);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(5).Take(1);

            // Act
            var result = await _service.GetAllByCustomerIdAndPagedAsync(3, 2, 2);

            // Assert
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndFilteredAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(4).Take(1);

            // Act
            var result = await _service.GetAllByCustomerIdAndFilteredAsync(3, "cash");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndBefore_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(2);

            var when = new DateTime(2019, 9, 10);

            // Act
            var result = await _service.GetAllByCustomerIdAndFilteredAsync(3, when, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndAfter_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Take(1);

            var when = new DateTime(2019, 3, 21);

            // Act
            var result = await _service.GetAllByCustomerIdAndFilteredAsync(3, when, false);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndOver_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(2);

            var amt = 60m;

            // Act
            var result = await _service.GetAllByCustomerIdAndFilteredAsync(3, amt, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndUnder_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(4).Take(1).Append(_db.Payments.FirstOrDefault());

            var amt = 120m;

            // Act
            var result = await _service.GetAllByCustomerIdAndFilteredAsync(3, amt, false);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(2).Reverse().Append(_db.Payments.FirstOrDefault());
            expected.Reverse();

            // Act
            var result = await _service.GetAllByCustomerIdAndSortedAsync(3, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(2).Append(_db.Payments.FirstOrDefault());

            // Act
            var result = await _service.GetAllByCustomerIdAndSortedAsync(3, "amt", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdFilteredAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(1);

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndPagedAsync(3, "Cash", 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdBeforeAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(6).Take(2);

            var when = new DateTime(2019, 6, 15);

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndPagedAsync(3, when, true, 2, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAfterAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(8).Take(1);

            var when = new DateTime(2019, 6, 10);

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndPagedAsync(3, when, false, 2, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdOverAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(2);

            var amt = 63.49m;

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndPagedAsync(3, amt, true, 2, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdUnderAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(6).Take(2);

            var amt = 80.59m;

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndPagedAsync(3, amt, false, 2, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdFilteredAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(4).Take(1).Append(_db.Payments.Skip(7).FirstOrDefault());

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndSortedAsync(3, "Cash", "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdFilteredAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(1).Append(_db.Payments.Skip(4).FirstOrDefault());

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndSortedAsync(3, "Cash", "date", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdBeforeAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(3).Take(2).Reverse();

            var when = new DateTime(2019, 6, 10);

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndSortedAsync(3, when, true, "meth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdBeforeAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(3).Take(2);

            var when = new DateTime(2019, 6, 10);

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndSortedAsync(3, when, true, "meth", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAfterAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Take(1).Append(_db.Payments.LastOrDefault());

            var when = new DateTime(2019, 6, 12);

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndSortedAsync(3, when, false, "payMeth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAfterAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Take(1).Append(_db.Payments.LastOrDefault());

            var when = new DateTime(2019, 6, 12);

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndSortedAsync(3, when, false, "date", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdOverAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(3).Take(1).Append(_db.Payments.Skip(8).FirstOrDefault());

            var amt = 74.59m;

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndSortedAsync(3, amt, true, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdOverAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(3).Take(1).Append(_db.Payments.Skip(8).FirstOrDefault());

            var amt = 74.59m;

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndSortedAsync(3, amt, true, "meth", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdUnderAndSortedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(6).Take(1).Append(_db.Payments.FirstOrDefault());

            var amt = 74.49m;

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndSortedAsync(3, amt, false, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdUnderAndSortedDescending_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Take(1).Append(_db.Payments.Skip(6).FirstOrDefault());

            var amt = 74.49m;

            // Act
            var result = await _service.GetAllByCustomerIdFilteredAndSortedAsync(3, amt, false, "date", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(3).Take(1);

            // Act
            var result = await _service.GetAllByCustomerIdSortedAndPagedAsync(3, 2, 2, "meth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(4).Take(1);

            // Act
            var result = await _service.GetAllByCustomerIdSortedAndPagedAsync(3, 2, 2, "date", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdFilteredSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(1);

            // Act
            var result = await _service.GetAllByCustomerIdFilteredSortedAndPagedAsync(3, "cash", 1, 2, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdBeforeSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(1).Append(_db.Payments.Skip(3).FirstOrDefault());

            var when = new DateTime(2019, 6, 15);

            // Act
            var result = await _service.GetAllByCustomerIdFilteredSortedAndPagedAsync(3, when, true, 2, 2, "amt");

            // Assert
        }

        [TestMethod]
        public async Task GetAllByCustomerIdAfterSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(2);

            var when = new DateTime(2019, 3, 21);

            // Act
            var result = await _service.GetAllByCustomerIdFilteredSortedAndPagedAsync(3, when, false, 2, 2, "meth");

            // Assert
        }

        [TestMethod]
        public async Task GetAllByCustomerIdOverSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(8).Take(1).Append(_db.Payments.Skip(3).FirstOrDefault());

            var amt = 74.49m;

            // Act
            var result = await _service.GetAllByCustomerIdFilteredSortedAndPagedAsync(3, amt, true, 2, 2, "meth");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllByCustomerIdUnderSortedAndPagedAsync_HappyFlow_ShouldReturnRequestedAsync()
        {
            // Arrange
            _db.BumpPaymentsDouble();
            var expected = _db.Payments.Skip(7).Take(1).Append(_db.Payments.FirstOrDefault());

            var amt = 80.59m;

            // Act
            var result = await _service.GetAllByCustomerIdFilteredSortedAndPagedAsync(3, amt, false, 2, 2, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #endregion

        #endregion
    }
}
