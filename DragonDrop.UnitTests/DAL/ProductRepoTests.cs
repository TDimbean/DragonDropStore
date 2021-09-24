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
    public class ProductRepoTests
    {
        private TestProductRepo _repo;
        private TestDb _db;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestProductRepo(_db);
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldRetrieveMatch()
        {
            var expected = _db.Products.FirstOrDefault();

            var result = _repo.Get(expected.ProductId);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNull()
        {
            var inexistentId = Generators.GenInexistentProductId();

            var result = _repo.Get(inexistentId);

            result.Should().BeNull();
        }

        [TestMethod]
        public void Create_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenProduct();
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
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = null;
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Create(newItem);
            var result = _repo.GetAll().ToList();
            var createdItem = result.SingleOrDefault(p => p.BarCode == newItem.BarCode);
            var newCount = result.Count();

            // Assert
            newCount.Should().Be(initCount);
            createdItem.Should().BeNull();
        }

        [TestMethod]
        public void Create_DuplicateBarCode_ShouldNotCreate()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = _db.Products.FirstOrDefault().BarCode;
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Create(newItem);
            var result = _repo.GetAll().ToList();
            var createdItem = result.SingleOrDefault(p => p.Name == newItem.Name);
            var newCount = result.Count();

            // Assert
            newCount.Should().Be(initCount);
            createdItem.Should().BeNull();
        }

        #region Updates

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var item = _db.Products.FirstOrDefault();
            var oldItem = new Product
            {
                ProductId = item.ProductId,
                Name = item.Name,
                Description = item.Description,
                Stock = item.Stock,
                BarCode = item.BarCode,
                CategoryId = item.CategoryId,
                UnitPrice = item.UnitPrice
            };
            var updItem = Generators.GenProduct();
            updItem.ProductId = item.ProductId;
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Update(updItem);
            var result = _repo.Get(oldItem.ProductId);
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
            var item = _db.Products.FirstOrDefault();
            var oldItem = new Product
            {
                ProductId = item.ProductId,
                Name = item.Name,
                Description = item.Description,
                Stock = item.Stock,
                BarCode = item.BarCode,
                CategoryId = item.CategoryId,
                UnitPrice = item.UnitPrice
            };
            var updItem = new Product
            {
                ProductId = Generators.GenInexistentProductId(),
                Name = "New Series Baseball Cards",
                Description = "The new and improved foil is water ressitant. Updated Roster",
                Stock = 10,
                BarCode = oldItem.BarCode,
                CategoryId = 0,
                UnitPrice = 19.99m
            };
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Update(updItem);
            var result = _repo.Get(oldItem.ProductId);
            var newRepo = _repo.GetAll();
            var newCount = newRepo.Count();

            // Assert
            newCount.Should().Be(initCount);
            newRepo.Should().ContainEquivalentOf(oldItem);
            result.Should().BeEquivalentTo(oldItem);
            result.Should().NotBeEquivalentTo(updItem);
        }

        [TestMethod]
        public void Update_IncompleteEntry_ShouldNotUpdate()
        {
            // Arrange
            var item = _db.Products.FirstOrDefault();
            var oldItem = new Product
            {
                ProductId = item.ProductId,
                Name = item.Name,
                Description = item.Description,
                Stock = item.Stock,
                BarCode = item.BarCode,
                CategoryId = item.CategoryId,
                UnitPrice = item.UnitPrice
            };
            var updItem = new Product
            {
                ProductId = item.ProductId,
                Name = "New Series Baseball Cards",
                Description = "The new and improved foil is water ressitant. Updated Roster",
                Stock = 10,
                CategoryId = 0,
                UnitPrice = null
            };
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Update(updItem);
            var result = _repo.Get(oldItem.ProductId);
            var newRepo = _repo.GetAll();
            var newCount = newRepo.Count();

            // Assert
            newCount.Should().Be(initCount);
            newRepo.Should().ContainEquivalentOf(oldItem);
            result.Should().BeEquivalentTo(oldItem);
            result.Should().NotBeEquivalentTo(updItem);
        }

        [TestMethod]
        public void Update_DuplicateBarCode_ShouldNotUpdate()
        {
            // Arrange
            var item = _db.Products.FirstOrDefault();
            var oldItem = new Product
            {
                ProductId = item.ProductId,
                Name = item.Name,
                Description = item.Description,
                Stock = item.Stock,
                BarCode = item.BarCode,
                CategoryId = item.CategoryId,
                UnitPrice = item.UnitPrice
            };
            var updItem = new Product
            {
                ProductId = item.ProductId,
                Name = "New Series Baseball Cards",
                Description = "The new and improved foil is water ressitant. Updated Roster",
                Stock = 10,
                CategoryId = 0,
                UnitPrice = item.UnitPrice,
                BarCode = _db.Products.LastOrDefault().BarCode
            };
            var initCount = _repo.GetAll().Count();

            // Act
            _repo.Update(updItem);
            var result = _repo.Get(oldItem.ProductId);
            var newRepo = _repo.GetAll();
            var newCount = newRepo.Count();

            // Assert
            newCount.Should().Be(initCount);
            newRepo.Should().ContainEquivalentOf(oldItem);
            result.Should().BeEquivalentTo(oldItem);
            result.Should().NotBeEquivalentTo(updItem);
        }

        #endregion

        #region AddStock

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(-3)]
        public void AddStock_HappyFlow_ShouldAddOrSubstract(int qty)
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            var prodId = prod.ProductId;
            var initStock = prod.Stock;

            // Act
            _repo.AddStock(prodId, qty);
            var result = _repo.Get(prodId).Stock;

            // Assert
            result.Should().Be(initStock + qty);
            if (qty < 0) result.Should().BeLessThan(initStock);
        }

        [TestMethod]
        public void AddStock_Zero_ShouldNotModify()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            var prodId = prod.ProductId;
            var initStock = prod.Stock;

            // Act
            _repo.AddStock(prodId, 0);
            var result = _repo.Get(prodId).Stock;

            // Assert
            result.Should().Be(initStock);
        }

        [TestMethod]
        public void AddStock_SubtractTooMuch_ShouldNotModify()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            var prodId = prod.ProductId;
            var initStock = prod.Stock;
            var qty = (prod.Stock + 1) * (-1);

            // Act
            _repo.AddStock(prodId, qty);
            var result = _repo.Get(prodId).Stock;

            // Assert
            result.Should().Be(initStock);
        }

        [TestMethod]
        public void AddStock_InexistentId_ShouldNotModify()
        {
            // Arrange
            var initDb = _db.Products.ToList();
            var badProdId = initDb.Count()+3;

            // Act
            _repo.AddStock(badProdId, 1);
            var result = _repo.GetAll();

            // Assert
            result.Should().BeEquivalentTo(initDb);
        }

        #endregion

        #region Gets

        [TestMethod]
        public void GetAll_HappyFlow_ShouldRetrieveAllRecords()
        {
            var result = _repo.GetAll();

            result.Should().BeEquivalentTo(_db.Products.ToList());
        }

        [TestMethod]
        public void GetByBarcode_HappyFlow_ShouldRetrieveMatch()
        {
            var expected = _db.Products.FirstOrDefault();

            var result = _repo.GetByBarcode(expected.BarCode);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetByBarcode_InexistentCode_ShouldReturnNull()
        {
            var inexistentCode = Generators.GenBarCode();

            var result = _repo.GetByBarcode(inexistentCode);

            result.Should().BeNull();
        }

        #region Paginated

        [TestMethod]
        public void GetAllPaginated_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            int pgSize = 1;
            int pgIndex = 3;
            var expected = _db.Products.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();

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
        public void GetAllFiltered_NameMatch_ShouldReturnMatch()
        {
            // Arrange
            var key = Generators.GenUnmistakableAddress();
            var targetItem = Generators.GenProduct();
            targetItem.Name = key;
            _repo.Create(targetItem);

            var expected = new List<Product> { targetItem };

            // Act
            var result = _repo.GetAllFiltered(targetItem.Name);

            // Assert
            result.Should().BeEquivalentTo(targetItem);
        }

        [TestMethod]
        public void GetAllFiltered_DescriptionMatch_ShouldReturnMatch()
        {
            // Arrange
            var key = Generators.GenUnmistakableAddress();
            var targetItem = Generators.GenProduct();
            targetItem.Description = key;
            _repo.Create(targetItem);

            var expected = new List<Product> { targetItem };

            // Act
            var result = _repo.GetAllFiltered(targetItem.Description);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_BarCodeMatch_ShouldReturnMatch()
        {
            // Arrange
            var targetItem = Generators.GenProduct();
            var expected = new List<Product> { targetItem };

            _repo.Create(targetItem);

            // Act
            var result = _repo.GetAllFiltered(targetItem.BarCode);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_ManufacturerMatch_ShouldReturnMatches()
        {
            // Arrange
            var manufacturer = new Random().Next(1, 1000000).ToString("D6");

            var gen = new InstanceTestGenerator(_db);

            var target1 = gen.NewProduct(manufacturer);
            _repo.Create(target1);
            var target2 = new Product();
            while (true)
            {
                target2 = gen.NewProduct(manufacturer);
                if (target1.BarCode != target2.BarCode) break;
            }

            _repo.Create(target2);

            var expected = new List<Product> { target1, target2 };

            // Act
            var result = _repo.GetAllFiltered(manufacturer);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_StockMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Products.FirstOrDefault();
            var expected = new List<Product> { targetItem };

            var result = _repo.GetAllFiltered(targetItem.BarCode);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_PriceMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Products.LastOrDefault();
            var expected = new List<Product> { targetItem };

            var result = _repo.GetAllFiltered(targetItem.UnitPrice.ToString());

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_Category_ShouldReturnMatch()
        {
            // Arrange
            var targetItem = Generators.GenProduct();
            var expected = new List<Product> { targetItem };

            _repo.Create(targetItem);

            // Act
            var result = _repo.GetAllFiltered(_db.Categories.SingleOrDefault(c => c.CategoryId == targetItem.CategoryId).Name);

            // Assert
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
            var target1 = _db.Products.FirstOrDefault();
            var target2 = _db.Products.LastOrDefault();
            var search = target1.Stock.ToString() + " " +
                         target2.Stock.ToString();
            var expected = new List<Product> { target1, target2 };

            // Act
            var result = _repo.GetAllFiltered(search);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_OverStock_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var expected = _repo.GetAll().ToList();
            expected.Remove(target);

            // Act
            var result = _repo.GetAllFiltered(target.Stock, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_UnderStock_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.LastOrDefault();
            var expected = _db.Products.ToList();
            expected.Remove(target);

            // Act
            var result = _repo.GetAllFiltered(target.Stock, false);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_OverPrice_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var expected = new List<Product> { _db.Products.LastOrDefault() };

            // Act
            var result = _repo.GetAllFiltered(target.UnitPrice.GetValueOrDefault(), true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_UnderPrice_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.LastOrDefault();
            var expected = _db.Products.ToList();
            expected.Remove(target);

            // Act
            var result = _repo.GetAllFiltered(target.UnitPrice.GetValueOrDefault(), false);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_StocksAndPricesOutOfBounds_ShouldReturnEmpty()
        {
            // Arrange
            var lowStock = _db.Products.FirstOrDefault().Stock;
            var highStock = _db.Products.LastOrDefault().Stock;
            var lowPrice = _db.Products.FirstOrDefault().UnitPrice.GetValueOrDefault();
            var highPrice = _db.Products.LastOrDefault().UnitPrice.GetValueOrDefault();

            // Act
            var result = _repo.GetAllFiltered(lowStock, false).ToList();
            result.AddRange(_repo.GetAllFiltered(highStock, true));
            result.AddRange(_repo.GetAllFiltered(lowPrice, false));
            result.AddRange(_repo.GetAllFiltered(highPrice, true));

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region FilteredAndPaged

        [TestMethod]
        public void GetAllFilteredAndPaged_HappyFlow_ShouldReturnMatch()
        {
            var expected = new List<Product> { _repo.Get(2) };

            var result = _repo.GetAllFilteredAndPaged(_db.Products.FirstOrDefault().UnitPrice.ToString(), 1, 2);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_OverStock_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var expected = new List<Product> { _repo.Get(3) };

            // Act
            var result = _repo.GetAllFilteredAndPaged(target.Stock, true, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_UnderStock_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.LastOrDefault();
            var expected = new List<Product> { _repo.Get(2) };

            // Act
            var result = _repo.GetAllFilteredAndPaged(target.Stock, false, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_OverPrice_ShouldReturnMatches()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = _db.Products.LastOrDefault().UnitPrice - 10;
            newItem.ProductId = _db.Products.LastOrDefault().ProductId + 1;
            _repo.Create(newItem);

            var target = _db.Products.FirstOrDefault();
            var expected = new List<Product> { newItem };

            // Act
            var result = _repo.GetAllFilteredAndPaged(target.UnitPrice.GetValueOrDefault(), true, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_UnderPrice_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.LastOrDefault();
            var expected = new List<Product> { _repo.Get(2) };

            // Act
            var result = _repo.GetAllFilteredAndPaged(target.UnitPrice.GetValueOrDefault(), false, 1, 2);

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
            var expected = _db.Products.OrderBy(p => p.ProductId);

            var result = _repo.GetAllSorted(sortBy);

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
            var expected = _db.Products.OrderBy(p => p.Name);

            var result = _repo.GetAllSorted(sortBy);

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
            var expected = _db.Products.OrderBy(p => p.Manufacturer);

            var result = _repo.GetAllSorted(sortBy);

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
            var expected = _db.Products.OrderBy(p => p.BarCode);

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("STOCK")]
        [DataRow("stOck")]
        [DataRow("STK")]
        [DataRow("sTk")]
        public void GetAllSorted_ByStock_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Products.OrderBy(p => p.Stock);

            var result = _repo.GetAllSorted(sortBy);

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
            var expected = _db.Products.OrderBy(p => p.UnitPrice);

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("CATEGORY")]
        [DataRow("CaTegOry")]
        [DataRow("CAT")]
        [DataRow("Cat")]
        public void GetAllSorted_ByCategory_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Products.OrderBy(p => p.CategoryId);

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("DESCRIPTION")]
        [DataRow("DecRiptIon")]
        [DataRow("DESC")]
        [DataRow("dEsc")]
        public void GetAllSorted_ByDescription_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Products.OrderBy(p => p.Description.Length);

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("sdfhushjb")]
        [DataRow(null)]
        public void GetAllSorted_BadQuery_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Products;

            var result = _repo.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescending_HappyFlow_ShouldReturnSortedDescending()
        {
            var expected = _db.Products.OrderByDescending(p=>p.ProductId);

            var result = _repo.GetAllSorted("id", true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(5).Take(1).Append(_db.Products.Skip(2).FirstOrDefault());

            // Act
            var result = _repo.GetAllSortedAndPaged(2, 3, "price");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(4).Take(2);

            // Act
            var result = _repo.GetAllSortedAndPaged(2, 3, "manufacturer", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(2).Reverse();

            // Act
            var result = _repo.GetAllFilteredAndSorted("876259", "desc");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(3).Reverse();

            // Act
            var result = _repo.GetAllFilteredAndSorted("rpg", "stock", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverStockAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(2).Take(2).Reverse();

            // Act
            var result = _repo.GetAllFilteredAndSorted(25, true, "cat");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverStockAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(2).Take(2);

            // Act
            var result = _repo.GetAllFilteredAndSorted(25, true, "cat", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderStockAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = _repo.GetAllFilteredAndSorted(23, false, "name");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderStockAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = _repo.GetAllFilteredAndSorted(23, false, "cat", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverPriceAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(2).Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = _repo.GetAllFilteredAndSorted(25.59m, true, "id");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverPriceAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(2).Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = _repo.GetAllFilteredAndSorted(25.59m, true, "desc", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderPriceAndSorted_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(3).Take(2);

            // Act
            var result = _repo.GetAllFilteredAndSorted(25.59m, false, "name");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderPriceAndSortedDescending_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(3).Take(2);

            // Act
            var result = _repo.GetAllFilteredAndSorted(25.59m, false, "STOCK", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(1).Take(1);

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged("rpg", 2, 2, "description");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(1).Take(1);

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged("rpg", 2, 2, "price", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverStockSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(2).Take(2).Reverse();

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged(16, true, 2, 2, "stock");


            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverStockSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(3).Take(2).Reverse();

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged(16, true, 2, 2, "price", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderStockSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(1).Take(1).Append(_db.Products.Skip(4).FirstOrDefault());

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged(53, false, 2, 2, "cat");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderStockSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged(53, false, 2, 2, "description", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverPriceSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged(12.35m, true, 2, 2, "name");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllOverPriceSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged(12.35m, true, 2, 2, "description", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderPriceSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(3).Take(2);

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged(49.95m, false, 2, 2, "name");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllUnderPriceSortedDescendingAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(3).Take(2).Reverse();

            // Act
            var result = _repo.GetAllFilteredSortedAndPaged(49.95m, false, 2, 2, "cost", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #endregion

        #region BarCode Verifiers

        [TestMethod]
        public void IsUniqueBarCode_ItIs_ShouldReturnTrue()
        {
            // Arrange
            var rnd = new Random();
            var barCode = "101234432101";
            while (true)
            {
                if (_db.Products.Any(p => p.BarCode == barCode))
                    barCode = rnd.Next(100000000, 1000000000).ToString() + rnd.Next(100, 1000).ToString();
                else break;
            }

            // Act
            var result = _repo.IsUniqueBarCode(barCode);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsUniqueBarCode_ItIsForMe_ShouldReturnTrue()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();

            // Act
            var result = _repo.IsUniqueBarCode(target.BarCode, target.ProductId);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsUniqueBarCode_ItIsNotForMe_ShouldReturnTrue()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var dupe = _db.Products.LastOrDefault();

            dupe.BarCode = target.BarCode;
            _repo.Update(dupe);

            // Act
            var result = _repo.IsUniqueBarCode(target.BarCode, target.ProductId);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsUniqueBarCode_ItIsNot_ShouldReturnFalse()
        {
            var barCode = _db.Products.FirstOrDefault().BarCode;

            var result = _repo.IsUniqueBarCode(barCode);

            result.Should().BeFalse();
        }

        #endregion

        #region IdVerifiers

        [TestMethod]
        public void ProductIdExists_ItDoes_ShouldReturnTrue()
        {
            var prodId = _db.Products.FirstOrDefault().ProductId;

            var result = _repo.ProductIdExists(prodId);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void ProductIdExists_ItDoesNot_ShouldReturnFalse()
        {
            var prodId = Generators.GenInexistentProductId();

            var result = _repo.ProductIdExists(prodId);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void CategoryIdExists_ItDoes_ShouldReturnTrue()
        {
            var id = _db.Categories.FirstOrDefault().CategoryId;

            var result = _repo.CategoryIdExists(id);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CategoryIdExists_ItDoesNot_ShouldReturnTrue()
        {
            var id = _db.Categories.Count() + 10;

            var result = _repo.CategoryIdExists(id);

            result.Should().BeFalse();
        }

        #endregion
    }
}
