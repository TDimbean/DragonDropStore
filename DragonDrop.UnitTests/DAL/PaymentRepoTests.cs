using DragonDrop.DAL.Entities;
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
    public class PaymentRepoTests
    {
        private TestPaymentRepo _repo;
        private TestDb _db;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestPaymentRepo(_db);
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldRetrieveMatch()
        {
            var expected = _db.Payments.FirstOrDefault();

            var result = _repo.Get(expected.PaymentId);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNull()
        {
            var inexistentId = Generators.GenInexistentPaymentId();

            var result = _repo.Get(inexistentId);

            result.Should().BeNull();
        }

        [TestMethod]
        public void Create_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenPayment();
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
            var newItem = new Payment
            {
                Amount = 10m,
                PaymentMethodId = 1,
                CustomerId = _db.Customers.FirstOrDefault().CustomerId
            };
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Create(newItem);
            var newCount = _repo.GetAll().Count();

            // Assert
            newCount.Should().Be(initCount);
        }

        [TestMethod]
        public void Create_InexistentCustomer_ShouldNotCreate()
        {
            // Arrange
            var newItem = Generators.GenPayment();
            newItem.CustomerId = Generators.GenInexistentCustomerId();
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
            var item = _db.Payments.FirstOrDefault();
            var oldItem = new Payment
            {
                PaymentId = item.PaymentId,
                CustomerId = item.CustomerId,
                Amount = item.Amount,
                Date = item.Date,
                PaymentMethodId = item.PaymentMethodId
            };
            var updItem = Generators.GenPayment();
            updItem.PaymentId = item.PaymentId;
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Update(updItem);
            var result = _repo.Get(oldItem.PaymentId);
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
            var item = _db.Payments.FirstOrDefault();
            var oldItem = new Payment
            {
                PaymentId = item.PaymentId,
                Amount = item.Amount,
                CustomerId = item.CustomerId,
                Date= item.Date,
                PaymentMethodId= item.PaymentMethodId
            };
            var updItem = Generators.GenPayment();
            updItem.PaymentId = Generators.GenInexistentPaymentId();
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Update(updItem);
            var result = _repo.Get(oldItem.PaymentId);
            var newRepo = _repo.GetAll();
            var newCount = newRepo.Count();

            // Assert
            newCount.Should().Be(initCount);
            newRepo.Should().ContainEquivalentOf(oldItem);
            result.Should().BeEquivalentTo(oldItem);
            result.Should().NotBeEquivalentTo(updItem);
        }

        [TestMethod]
        public void Update_IncompleteData_ShouldNotUpdate()
        {
            {
                // Arrange
                var item = _db.Payments.FirstOrDefault();
                var oldItem = new Payment
                {
                    PaymentId = item.PaymentId,
                    Amount = item.Amount,
                    CustomerId = item.CustomerId,
                    Date = item.Date,
                    PaymentMethodId = item.PaymentMethodId
                };

                #region Generate an amount that's different from the old value
                var amnt = 1m;
                var rnd = new Random();
                while (true)
                {
                    if (amnt == oldItem.Amount) amnt = (decimal)rnd.Next(1, 201);
                    else break;
                }
                #endregion

                var updItem = new Payment
                {
                    PaymentId = oldItem.PaymentId,
                    CustomerId = oldItem.CustomerId,
                    PaymentMethodId = oldItem.PaymentMethodId,
                    Amount = amnt
                };
                var initCount = _repo.GetAll().Count();

                // Act
                _repo.Update(updItem);
                var result = _repo.Get(oldItem.PaymentId);
                var newRepo = _repo.GetAll();
                var newCount = newRepo.Count();

                // Assert
                newCount.Should().Be(initCount);
                newRepo.Should().ContainEquivalentOf(oldItem);
                result.Should().BeEquivalentTo(oldItem);
                result.Should().NotBeEquivalentTo(updItem);
            }
        }

        #region Gets

        [TestMethod]
        public void GetAll_HappyFlow_ShouldRetrieveAllRecords()
        {
            var result = _repo.GetAll();

            result.Should().BeEquivalentTo(_db.Payments.ToList());
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
        public void GetAllFiltered_amountMatch_ShouldReturnMatch()
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
            _repo.Create(targetItem);

            var expected = new List<Payment> { targetItem };

            // Act
            var result = _repo.GetAllFiltered(targetItem.Amount.ToString());

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_CustomerNameMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Payments.FirstOrDefault();
            var expected = new List<Payment> { targetItem };

            var result = _repo.GetAllFiltered(_db.Customers.SingleOrDefault(c=>c.CustomerId==targetItem.CustomerId).Name);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_DateMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Payments.FirstOrDefault();
            var expected = new List<Payment> { targetItem };

            var result = _repo.GetAllFiltered(targetItem.Date.GetValueOrDefault().ToShortDateString());

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_MethodMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Payments.FirstOrDefault();
            var expected = new List<Payment> { targetItem };

            var result = _repo.GetAllFiltered(_db.PaymentMethods.SingleOrDefault(m => m.PaymentMethodId == targetItem.PaymentMethodId).Name);

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
            var target1 = Generators.GenPayment();
            var target2 = Generators.GenPayment();

            #region Generate unique amounts as Identifiers, then add new Payments to the repo to serve as targets
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

            _repo.Create(target1);
            _repo.Create(target2);
            #endregion

            var search = amnt1.ToString() + " " + amnt2.ToString();
            var expected = new List<Payment> { target1, target2 };

            // Act
            var result = _repo.GetAllFiltered(search);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_HappyFlow_ShouldReturnMatch()
        {
            var expected = new List<Payment> { _repo.Get(2) };

            var result = _repo.GetAllFilteredAndPaged(_db.Payments.FirstOrDefault().Amount.ToString(), 1, 2);

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
            var result = _repo.GetAllFiltered(target.Date.GetValueOrDefault(), true);

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
            var result = _repo.GetAllFiltered(target.Date.GetValueOrDefault(), false);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_Overamount_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.LastOrDefault();
            var expected = new List<Payment> { target };

            // Act
            var result = _repo.GetAllFiltered(target.Amount.GetValueOrDefault() - 10, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_Underamount_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.LastOrDefault();
            var expected = _db.Payments.ToList();
            expected.Remove(target);

            // Act
            var result = _repo.GetAllFiltered(target.Amount.GetValueOrDefault(), false);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_DatesAndamountsOutOfBounds_ShouldReturnEmpty()
        {
            // Arrange
            var lowAmnt = _db.Payments.FirstOrDefault().Amount.GetValueOrDefault();
            var highAmnt = _db.Payments.LastOrDefault().Amount.GetValueOrDefault();
            var earlyDate = _db.Payments.FirstOrDefault().Date.GetValueOrDefault();
            var lateDate = _db.Payments.LastOrDefault().Date.GetValueOrDefault();

            // Act
            var result = _repo.GetAllFiltered(lowAmnt, false).ToList();
            result.AddRange(_repo.GetAllFiltered(highAmnt, true));
            result.AddRange(_repo.GetAllFiltered(earlyDate, true));
            result.AddRange(_repo.GetAllFiltered(lateDate, false));

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_BeforeDate_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.LastOrDefault();
            var expected = new List<Payment> { _repo.Get(2) };

            // Act
            var result = _repo.GetAllFilteredAndPaged(target.Date.GetValueOrDefault(), true, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_AfterDate_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.FirstOrDefault();
            var expected = new List<Payment> { _repo.Get(3) };

            // Act
            var result = _repo.GetAllFilteredAndPaged(target.Date.GetValueOrDefault(), false,1,2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_OverAmount_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.LastOrDefault();

            var newItem = Generators.GenPayment();
            newItem.Amount = target.Amount + 10;
            newItem.PaymentId = target.PaymentId + 1;

            _repo.Create(newItem);

            var expected = new List<Payment> { newItem};

            // Act
            var result = _repo.GetAllFilteredAndPaged(target.Amount.GetValueOrDefault() - 5, true,1,2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_Underamount_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Payments.LastOrDefault();
            var expected = new List<Payment> { _repo.Get(2) };

            // Act
            var result = _repo.GetAllFilteredAndPaged(target.Amount.GetValueOrDefault(), false,1,2);

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
            var expected = _db.Payments;

            var result = _repo.GetAllSorted(sortBy);

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
            var expected = _db.Payments;
            expected.Reverse();

            var result = _repo.GetAllSorted(sortBy, true);

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
            var expected = _db.Payments;
            expected.Reverse();

            var result = _repo.GetAllSorted(sortBy);

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
            var expected = _db.Payments;

            var result = _repo.GetAllSorted(sortBy, true);

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

            var result = _repo.GetAllSorted(sortBy);

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

                var result = _repo.GetAllSorted(sortBy, true);

                result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("DATE")]
        [DataRow("daTe")]
        public void GetAllSorted_ByDate_ShouldReturnRequested(string sortBy)
        {
            var expected = _db.Payments;

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("DATE")]
        [DataRow("daTe")]
        public void GetAllSortedDescending_ByDate_ShouldReturnRequested(string sortBy)
        {
            var expected = _db.Payments;
            expected.Reverse();

            var result = _repo.GetAllSorted(sortBy);

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

            var result = _repo.GetAllSorted(sortBy);

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

            var result = _repo.GetAllSorted(sortBy, true);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("M76fgtD")]
        [DataRow(null)]
        public void GetAllSorted_BadQuery_ShouldReturnAll(string sortBy)
        {
            var expected = _db.Payments;

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            var expected = new List<Payment> { _db.Payments.FirstOrDefault() };

            var result = _repo.GetAllSortedAndPaged(2, 2, "cust");

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            var expected = new List<Payment> { _db.Payments.FirstOrDefault() };

            var result = _repo.GetAllSortedAndPaged(2, 2, "id", true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            var expected = _db.Payments.Take(2);
            expected.Reverse();

            var result = _repo.GetAllFilteredAndSorted("25.59", "cust");

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            var expected = _db.Payments.Take(2);

            var result = _repo.GetAllFilteredAndSorted("25.59", "date", true);

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
            var result = _repo.GetAllFilteredAndSorted(when, true, "cust");

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
            var result = _repo.GetAllFilteredAndSorted(when, true, "cust", true);

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
            var result = _repo.GetAllFilteredAndSorted(when, false, "cust");

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
            var result = _repo.GetAllFilteredAndSorted(when, false, "amt", true);

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
            var result = _repo.GetAllFilteredAndSorted(amt, false, "cust");

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
            var result = _repo.GetAllFilteredAndSorted(amt, false, "date", true);

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
            var result = _repo.GetAllFilteredAndSorted(amt, true, "date");

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
        var result = _repo.GetAllFilteredAndSorted(amt, true, "id", true);

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
            var result = _repo.GetAllFilteredSortedAndPaged("Ced", 2, 2, "amt");

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
            var result = _repo.GetAllFilteredSortedAndPaged("Ced", 2, 2, "date", true);

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
            var result = _repo.GetAllFilteredSortedAndPaged(when, true, 2, 2, "id");

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
            var result = _repo.GetAllFilteredSortedAndPaged(when, true, 2, 2, "cust", true);

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
            var result = _repo.GetAllFilteredSortedAndPaged(when, false, 2, 2, "meth");

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
            var result = _repo.GetAllFilteredSortedAndPaged(when, false, 2, 2, "amt", true);

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
            var result = _repo.GetAllFilteredSortedAndPaged(amt, true, 2, 2, "meth");

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
            var result = _repo.GetAllFilteredSortedAndPaged(amt, true, 2, 2, "meth", true);

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
            var result = _repo.GetAllFilteredSortedAndPaged(amt, false, 2, 2, "meth");

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
            var result = _repo.GetAllFilteredSortedAndPaged(amt, false, 2, 2, "amt", true);

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
            _repo.Create(target2);

            var expected = new List<Payment> { target1, target2 };

            // Act
            var result = _repo.GetAllByCustomerId(target1.CustomerId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllByCustomerId_InexistentId_ShouldReturnEmpty()
        {
            var custId = Generators.GenInexistentCustomerId();

            var result = _repo.GetAllByCustomerId(custId);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllByCustomerIdAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(5).Take(1);

            // Act
            var result = _repo.GetAllByCustomerIdAndPaged(3, 2, 2);

            // Assert
        }

        [TestMethod]
        public void GetAllByCustomerIdAndFiltered_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpPayments();
            var expected = _db.Payments.Skip(4).Take(1);

            // Act
            var result = _repo.GetAllByCustomerIdAndFiltered(3, "cash");

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
            var result = _repo.GetAllByCustomerIdAndFiltered(3, when, true);

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
            var result = _repo.GetAllByCustomerIdAndFiltered(3, when, false);

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
            var result = _repo.GetAllByCustomerIdAndFiltered(3, amt, true);

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
            var result = _repo.GetAllByCustomerIdAndFiltered(3, amt, false);

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
            var result = _repo.GetAllByCustomerIdAndSorted(3, "date");

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
            var result = _repo.GetAllByCustomerIdAndSorted(3, "amt", true);

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
            var result = _repo.GetAllByCustomerIdFilteredAndPaged(3, "Cash", 1, 2);

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
            var result = _repo.GetAllByCustomerIdFilteredAndPaged(3, when, true, 2, 2);

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
            var result = _repo.GetAllByCustomerIdFilteredAndPaged(3, when, false, 2, 2);

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
            var result = _repo.GetAllByCustomerIdFilteredAndPaged(3, amt, true, 2, 2);

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
            var result = _repo.GetAllByCustomerIdFilteredAndPaged(3, amt, false, 2, 2);

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
            var result = _repo.GetAllByCustomerIdFilteredAndSorted(3, "Cash", "date");

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
            var result = _repo.GetAllByCustomerIdFilteredAndSorted(3, "Cash", "date", true);

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
            var result = _repo.GetAllByCustomerIdFilteredAndSorted(3, when, true, "meth");

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
            var result = _repo.GetAllByCustomerIdFilteredAndSorted(3, when, true, "meth", true);

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
            var result = _repo.GetAllByCustomerIdFilteredAndSorted(3, when, false, "payMeth");

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
            var result = _repo.GetAllByCustomerIdFilteredAndSorted(3, when, false, "date", true);

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
            var result = _repo.GetAllByCustomerIdFilteredAndSorted(3, amt, true, "date");

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
            var result = _repo.GetAllByCustomerIdFilteredAndSorted(3, amt, true, "meth", true);

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
            var result = _repo.GetAllByCustomerIdFilteredAndSorted(3, amt, false, "date");

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
            var result = _repo.GetAllByCustomerIdFilteredAndSorted(3, amt, false, "date", true);

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
            var result = _repo.GetAllByCustomerIdSortedAndPaged(3, 2, 2, "meth");

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
            var result = _repo.GetAllByCustomerIdSortedAndPaged(3, 2, 2, "date", true);

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
            var result = _repo.GetAllByCustomerIdFilteredSortedAndPaged(3, "cash", 1, 2, "date");

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
            var result = _repo.GetAllByCustomerIdFilteredSortedAndPaged(3, when, true, 2, 2, "amt");

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
            var result = _repo.GetAllByCustomerIdFilteredSortedAndPaged(3, when, false, 2, 2, "meth");

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
            var result = _repo.GetAllByCustomerIdFilteredSortedAndPaged(3, amt, false, 2, 2, "meth");

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
            var result = _repo.GetAllByCustomerIdFilteredSortedAndPaged(3, amt, true, 2, 2, "date");

            // Assert
            result.Should().BeEquivalentTo(expected);
    }

    #endregion

    #endregion

    #region IdVerifiers

    [TestMethod]
        public void PaymentIdExists_ItDoes_ShouldReturnTrue()
        {
            var id = _db.Payments.FirstOrDefault().PaymentId;

            var result = _repo.PaymentIdExists(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void PaymentIdExists_ItDoesNot_ShouldReturnFalse()
        {
            var id = Generators.GenInexistentPaymentId();

            var result = _repo.PaymentIdExists(id);

            result.Should().BeFalse();
        }

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
            var id = _db.PaymentMethods.Count()+1;

            var result = _repo.PaymentMethodIdExists(id);

            result.Should().BeFalse();
        }

        #endregion
    }
}
