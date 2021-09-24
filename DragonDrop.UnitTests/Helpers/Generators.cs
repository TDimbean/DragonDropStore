using DragonDrop.DAL.Entities;
using DragonDrop.UnitTests.MockRepoes;
using System;
using System.Linq;

namespace DragonDrop.UnitTests.Helpers
{
    public static class Generators
    {
        private static readonly TestDb _context = new TestDb();
        private static Random _rnd = new Random();

        #region Inexistent ID Generators

        public static int GenInexistentCustomerId()
        {
            var id = 0;
            var exists = true;

            while (exists)
            {
                id = _rnd.Next(0, int.MaxValue);
                if (!_context.Customers.Any(c => c.CustomerId == id)) exists = false;
            }
            return id;
        }

        public static int GenInexistentOrderId()
        {
            var id = 0;
            var exists = true;

            while (exists)
            {
                id = _rnd.Next(0, int.MaxValue);
                if (!_context.Orders.Any(c => c.OrderId == id)) exists = false;
            }
            return id;
        }

        public static int GenInexistentProductId()
        {
            var id = 0;
            var exists = true;

            while (exists)
            {
                id = _rnd.Next(0, int.MaxValue);
                if (!_context.Products.Any(c => c.ProductId == id)) exists = false;
            }
            return id;
        }

        public static int GenInexistentPaymentId()
        {
            var id = 0;
            var exists = true;

            while (exists)
            {
                id = _rnd.Next(0, int.MaxValue);
                if (!_context.Payments.Any(p => p.PaymentId == id)) exists = false;
            }
            return id;
        }

        #endregion

        #region New Record Generators

        public static Customer GenCustomer() => new Customer
            {
                Name = "Will Smith",
                Address = GenUnmistakableAddress(),
                Phone = GenPhoneNumber(),
                City = "Guerneville",
                State = "California"
            };

        public static Order GenOrder()
        {
            var ordStat = _rnd.Next(0, 4);
            var ordDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _rnd.Next(1, DateTime.Now.Day));
            var shipDate = ordDate.AddDays(_rnd.Next(1, DateTime.Now.Day - ordDate.Day + 1));

            return new Order
            {
                CustomerId = _rnd.Next(1, _context.Customers.Count() + 1),
                OrderDate = ordDate,
                ShippingDate = ordStat < 2 ? null : (DateTime?)shipDate,
                OrderStatusId = ordStat,
                PaymentMethodId = _rnd.Next(1, 5),
                ShippingMethodId = _rnd.Next(1, 5)
            };
        }

        public static OrderItem GenOrderItem()
        {
            var ordId = 0;
            var prodId = 0;
            var exists = true;
            var attempts = 0;
            while(exists)
            {
                if (attempts == 1000) return null;
                ordId = _rnd.Next(1, _context.Orders.Count() + 1);
                prodId = _rnd.Next(1, _context.Products.Count() + 1);
                if (!_context.OrderItems.Any(i => i.OrderId == ordId && i.ProductId == prodId)) exists = false;
                if (_context.Products.SingleOrDefault(p => p.ProductId == prodId).Stock == 0) exists = true;
                attempts++;
            }
            var qty = _rnd.Next(1, _context.Products.SingleOrDefault(p => p.ProductId == prodId).Stock + 1);

            return new OrderItem { OrderId = ordId, ProductId = prodId, Quantity = qty };
        }

        public static Payment GenPayment()
            => new Payment
            {
                CustomerId = _rnd.Next(1, _context.Products.Count() + 1),
                Amount = Math.Round((decimal)(_rnd.Next(1, 301) + 1m / (decimal)_rnd.Next(2, 9)),2),
                Date = DateTime.Now.AddDays(-_rnd.Next(1, 300)).Date,
                PaymentMethodId = _rnd.Next(1, 5)
            };

        public static Product GenProduct(string manufacturer = null)
            => new Product
            {
                Name = "Baseball Card Pack - Series:" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                Description = "5 card booster for your collection. Guaranteed 1 Rare.",
                CategoryId = 0,
                BarCode = GenBarCode(manufacturer),
                Stock = _rnd.Next(1, 101),
                UnitPrice = _rnd.Next(1, 40) + (decimal)(1m / _rnd.Next(2, 9))
            };

        #endregion

        public static string GenBarCode(string manufacturer = null, bool valid=true)
        {
            var junk = 0;
            var manParses = int.TryParse(manufacturer, out junk);

            var barcode = _rnd.Next(1, 1000000000).ToString("D9") + _rnd.Next(1, 100).ToString("D2");
            if (manufacturer != null && manufacturer.Length == 6 && manParses) barcode = barcode.Replace(barcode.Substring(0, 6), manufacturer);

            var oddSum = 0;
            var evenSum = 0;

            for (int i = 0; i < 11; i++)
            {
                var digit = int.Parse(barcode[i].ToString());

                oddSum += i % 2 == 0 ? digit : 0 ;
                evenSum += i % 2 != 0 ? digit : 0 ;
            }

            var totalSum = oddSum * 3 + evenSum;

            var check = 0;
            while (valid)
            {
                if ((check + totalSum) % 10 == 0) break;
                else check++;
            }
            while(!valid)
            {
                if ((check + totalSum) % 10 == 0) check++;
                else break;
            }


            return barcode + check.ToString();
        }

        public static string GenPhoneNumber()
        {
            var phone = "";
            for (int i = 0; i < 3; i++)
            {
                var j = i != 2 ? 3 : 4;
                for (int k = 0; k < j; k++)
                {
                    phone += _rnd.Next(0, 10).ToString();
                }
                if (i != 2) phone += "-";
            }

            return phone;
        }

        public static DateTime GenUnusedDate()
        {
            var date = new DateTime(1600, 1, 1);
            var exists = true;
            while(exists)
            {
                date = new DateTime(DateTime.Now.Year, _rnd.Next(1, DateTime.Now.Month), _rnd.Next(1, DateTime.Now.Day));
                if (!_context.Orders.Any(o => o.OrderDate == date || o.ShippingDate == date)) exists = false;
            }
            return date;
        }

        public static string GenRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_rnd.Next(s.Length)]).ToArray());
        }

        public static string GenUnmistakableAddress()
        {
            //This generates an unmistakeable address so the tests can search customers by it in lew of an ID
            //It's a workaround but it's better than nothing, when fishing for inexistent records that should return null on any search

            var adr = "";
            var exists = true;

            while (exists)
            {
                adr = Guid.NewGuid().ToString();
                if (!_context.Customers.Any(c => c.Address == adr)) exists = false;
            }

            return adr;
        }

        public static string GenEmailAddress()
        {
            var email = "alpha@omega.net";
            while(_context.Customers.Any(c=>c.Email==email))
            {
                email = GenRandomString(10) + "@" + GenRandomString(6) + ".net";
            }
            return email;
        }
    }
}
