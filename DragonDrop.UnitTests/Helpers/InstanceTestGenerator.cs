using DragonDrop.DAL.Entities;
using DragonDrop.UnitTests.MockRepoes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonDrop.UnitTests.Helpers
{
    public class InstanceTestGenerator
    {
        private TestDb _context;

        private List<string> _generatedBarCodes;
        private List<int> _generatedProductIds;

        public InstanceTestGenerator()
        {
            _generatedBarCodes = new List<string>();
            _generatedProductIds = new List<int>();
            _context = new TestDb();
        }

        public InstanceTestGenerator(TestDb context)
        {
            _generatedBarCodes = new List<string>();
            _generatedProductIds = new List<int>();
            _context = context;
        }

        public Product NewProduct(string manufacturer = null)
        {
            var rnd = new Random();

            var prodId = _context.Products.Count() + _generatedProductIds.Count() + 1;
            while (_context.Products.Any(p => p.ProductId == prodId) ||
                    _generatedProductIds.Any(i => i == prodId))
                prodId++;
            _generatedProductIds.Add(prodId);

            return new Product
            {
                ProductId=prodId,
                Name = "Baseball Card Pack - Series:" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                Description = "5 card booster for your collection. Guaranteed 1 Rare.",
                CategoryId = 0,
                BarCode = NewBarCode(manufacturer),
                Stock = rnd.Next(1, 101),
                UnitPrice = rnd.Next(1, 40) + (decimal)(1m / rnd.Next(2, 9))
            };
        }

        public string NewBarCode(string manufacturer=null)
        {
            while (true)
            {
                var junk = 0;
                var manParses = int.TryParse(manufacturer, out junk);

                var rnd = new Random();

                var barcode = rnd.Next(1, 1000000000).ToString("D9") + rnd.Next(1, 100).ToString("D2");
                if (manufacturer != null && manufacturer.Length == 6 && manParses)
                    barcode = barcode.Replace(barcode.Substring(0, 6), manufacturer);

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
                while (true)
                {
                    if ((check + totalSum) % 10 == 0) break;
                    else check++;
                }

                barcode = barcode + check.ToString();

                if (!_context.Products.Any(p => p.BarCode == barcode &&
                     !_generatedBarCodes.Any(c => c == barcode)))
                {
                    _generatedBarCodes.Add(barcode);
                    return barcode;
                }
            }
        }
    }
}
