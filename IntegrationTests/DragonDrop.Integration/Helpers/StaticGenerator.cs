using DragonDrop.Integration.Entities;
using System;
using System.Linq;
using System.Text;

namespace DragonDrop.Integration.Helpers
{
    public static class StaticGenerator
    {
        #region Record Generators

        public static Customer GenCustomer() => new Customer
        {
            Name = "Will Smith",
            Address = Guid.NewGuid().ToString(),
            Phone = GenPhoneNumber(),
            City = "Guerneville",
            State = "California"
        };

        public static Order GenOrder()
        {
            var rnd = new Random();
            var ordStat = rnd.Next(0, 4);
            var ordDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, rnd.Next(1, DateTime.Now.Day));
            var shipDate = ordDate.AddDays(rnd.Next(1, DateTime.Now.Day - ordDate.Day + 1));

            return new Order
            {
                CustomerId = rnd.Next(1, 35),
                OrderDate = ordDate,
                ShippingDate = ordStat < 2 ? null : (DateTime?)shipDate,
                OrderStatusId = ordStat,
                PaymentMethodId = rnd.Next(1, 5),
                ShippingMethodId = rnd.Next(1, 5)
            };
        }

        public static OrderItem GenOrderItem()
            => new OrderItem
            {
                OrderId = (new Random()).Next(0, 21),
                ProductId = (new Random()).Next(0, 70),
                Quantity = (new Random()).Next(0, 51),
            };

        public static Payment GenPayment()
        {
            var rnd = new Random();

            var amt = (decimal)(rnd.Next(1, 301) + 1m / (decimal)rnd.Next(2, 9));
            var amtString = amt.ToString("F2");
            var shortAmt = decimal.Parse(amtString);

            return new Payment
            {
                CustomerId = rnd.Next(1, 36),
                Amount = shortAmt,
                Date = DateTime.Parse(DateTime.Now.AddDays(-rnd.Next(1, 300)).ToShortDateString()),
                PaymentMethodId = rnd.Next(1, 5)
            };
        }

        public static Product GenProduct(string manufacturer = null)
        {
            var rnd = new Random();

            var rawPrice = rnd.Next(1, 40) + (decimal)(1m / rnd.Next(2, 9));
            var priceString = rawPrice.ToString("F2");
            var price = decimal.Parse(priceString);

            return new Product
            {
                Name = "Baseball Card Pack - Series:" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                Description = "5 card booster for your collection. Guaranteed 1 Rare.",
                CategoryId = 0,
                BarCode = GenBarCode(manufacturer),
                Stock = rnd.Next(1, 101),
                UnitPrice = price
            };
        }

        #endregion

        #region Others

        public static string GenPhoneNumber()
        {
            var rnd = new Random();
            var phone = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                var j = i != 2 ? 3 : 4;
                for (int k = 0; k < j; k++)
                {
                    phone.Append(rnd.Next(0, 10).ToString());
                }
                if (i != 2) phone.Append("-");
            }

            return phone.ToString();
        }

        public static string GenRandomString(int length)
        {
            var rnd = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        public static string GenBarCode(string manufacturer = null, bool valid = true)
        {
            var junk = 0;
            var manParses = int.TryParse(manufacturer, out junk);

            var barcode = (new Random()).Next(1, 1000000000).ToString("D9") +
                        (new Random()).Next(1, 100).ToString("D2");
            if (manufacturer != null && manufacturer.Length == 6 && manParses) barcode = barcode.Replace(barcode.Substring(0, 6), manufacturer);

            var oddSum = 0;
            var evenSum = 0;

            for (int i = 0; i < 11; i++)
            {
                var digit = int.Parse(barcode[i].ToString());

                oddSum += i % 2 == 0 ? digit : 0;
                evenSum += i % 2 != 0 ? digit : 0;
            }

            var totalSum = oddSum * 3 + evenSum;

            var check = 0;
            while (valid)
            {
                if ((check + totalSum) % 10 == 0) break;
                else check++;
            }
            while (!valid)
            {
                if ((check + totalSum) % 10 == 0) check++;
                else break;
            }


            return barcode + check.ToString();
        }

        #endregion
    }
}
