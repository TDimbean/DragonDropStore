using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure.Helpers;
using DragonDrop.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DragonDrop.UnitTests.HelpersTests
{
    [TestClass]
    public class GeneratorsTests
    {
        private DragonDrop_DbContext _context;

        [TestInitialize]
        public void Init()
        {
            _context = new DragonDrop_DbContext();
        }

        [TestMethod]
        public void GenInexistentCustomerId_HappyFlow_ShouldGenerateInexistent()
        {
            var generated = Generators.GenInexistentCustomerId();

            var result = _context.Customers.Any(c => c.CustomerId == generated);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void GenInexistentOrderId_HappyFlow_ShouldGenerateInexistent()
        {
            var generated = Generators.GenInexistentOrderId();

            var result = _context.Orders.Any(o => o.OrderId == generated);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void GenInexistentPaymentId_HappyFlow_ShouldGenerateInexistent()
        {
            var generated = Generators.GenInexistentPaymentId();

            var result = _context.Payments.Any(p => p.PaymentId == generated);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void GenInexistentProductId_HappyFlow_ShouldGenerateInexistent()
        {
            var generated = Generators.GenInexistentProductId();

            var result = _context.Products.Any(p => p.ProductId == generated);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void GenOrder_HappyFlow_ShouldGenerateValid()
        {
            var generated = Generators.GenOrder();
            _context.Orders.Add(generated);

            var result = _context.Orders.Find(generated.OrderId);

            result.Should().BeEquivalentTo(generated);
        }

        [TestMethod]
        public void GenOrderItem_HappyFlow_ShouldGenerateValid()
        {
            var generated = Generators.GenOrderItem();
            _context.OrderItems.Add(generated);

            var result = _context.OrderItems.Find(generated.OrderId, generated.ProductId);

            result.Should().BeEquivalentTo(generated);
        }

        [TestMethod]
        public void GenProduct_HappyFlow_ShouldGenerateValid()
        {
            var generated = Generators.GenProduct();
            _context.Products.Add(generated);

            var result = _context.Products.Find(generated.ProductId);

            result.Should().BeEquivalentTo(generated);
        }

        [TestMethod]
        public void GenPayment_HappyFlow_ShouldGenerateValid()
        {
            var generated = Generators.GenPayment();
            _context.Payments.Add(generated);

            var result = _context.Payments.Find(generated.PaymentId);

            result.Should().BeEquivalentTo(generated);
        }

        [TestMethod]
        public void GenPhoneNumber_HappyFlow_ShouldGenerateValid()
        {
            var generated = Generators.GenPhoneNumber();
            long junk = 0;

            generated.Length.Should().Be(12);
            generated.Substring(3, 1).Should().Be("-");
            generated.Substring(7, 1).Should().Be("-");
            var shortened = generated.Remove(7, 1).Remove(3, 1);
            long.TryParse(shortened, out junk).Should().BeTrue();
        }

        [TestMethod]
        public void GenUnusedDate_HappyFlow_ShouldGenerateValidUniqueDate()
        {
            var generated = Generators.GenUnusedDate();
            var junk = new DateTime();

            DateTime.TryParse(generated.ToShortDateString(), out junk).Should().BeTrue();
            _context.Orders.Any(o => o.OrderDate == generated || o.ShippingDate == generated).Should().BeFalse();
        }

        [TestMethod]
        public void GenUnmistakeableAddress_HappyFlow_ShouldGenerateUniqueString()
        {
            var generated = Generators.GenUnmistakableAddress();

            var result = _context.Customers.Any(c => c.Address == generated);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void GenBarcode_NoManufacturer_ShouldGenerateValid()
        {
            var code = Generators.GenBarCode();

            var result = BarcodeVerifier.Check(code);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void GenBarcode_WithManufacturer_ShouldGenerateValid()
        {
            // Arrange
            var manufacturer = new Random().Next(1, 1000000).ToString("D6");
            var code = Generators.GenBarCode(manufacturer);

            // Act
            var result = BarcodeVerifier.Check(code);

            // Assert
            result.Should().BeTrue();
            code.Substring(0, 6).Should().Be(manufacturer);
        }
    }
}
