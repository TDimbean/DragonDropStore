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
    public class ProductDataServiceTests
    {
        private IProductDataService _service;
        private IProductRepository _repo;
        private TestDb _db;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestProductRepo(_db);
            _service = new ProductDataService(_repo);
        }

        [TestMethod]
        public void Get_HappyFlow_ShouldReturnTarget()
        {
            var target = _db.Products.FirstOrDefault();

            var result = _service.Get(target.ProductId);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public void Get_InexistentId_ShouldReturnNull()
        {
            var id = Generators.GenInexistentProductId();

            var result = _service.Get(id);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetByBarcode_HappyFlow_ShouldReturnTarget()
        {
            var target = _db.Products.FirstOrDefault();

            var result = _service.GetByBarcode(target.BarCode);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public void GetByBarcode_InexistentId_ShouldReturnNull()
        {
            var badCode = Generators.GenBarCode();

            var result = _service.GetByBarcode(badCode);

            result.Should().BeNull();
        }


        #region Creates

        [TestMethod]
        public void Create_HappyFlow_ShouldCreate()
        {
            // Arrange
            var newItem = Generators.GenProduct();

            // Act
            var result = _service.Create(newItem, true);
            var createdItem = _service.Get(newItem.ProductId);

            // Assert
            result.Should().BeNull();
            createdItem.Should().BeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Create_InexistentCategoryId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.CategoryId = _db.Categories.Count() + 1;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("Products require a valid Category to be placed under\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public void Create_NullCategoryId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.CategoryId = null;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("Products require a valid Category to be placed under\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            createdProduct.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void Create_NoName_ShouldNotCreateAndReturnAppropriateError(string name)
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Name = name;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("Products require a name.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public void Create_NameTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Name = Generators.GenRandomString(51);

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("Product Name limited to 50 characters.");
            createdProduct.Should().BeNull();
        }

        //[TestMethod]
        //public void Create_DescriptionTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Description = Generators.GenRandomString(301);

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("Product Description is limited to 300 characters.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public void Create_NegativeStock_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Stock = -3;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("Product Stock may not be negative.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public void Create_NegativeUnitPrice_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = -3m;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public void Create_UnitPriceZero_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = 0m;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public void Create_NullUnitPrice_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = null;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public void Create_NullBarCode_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = null;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("Products require a Barcode.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public void Create_DuplicateBarCode_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = _db.Products.FirstOrDefault().BarCode;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("Products must have a unique Bar Code.");
            createdProduct.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("12345665432")]
        [DataRow("1234566543210")]
        public void Create_WrongLengthBarCode_ShouldNotCreateAndReturnAppropriateError(string barCode)
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = barCode;

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("Product Barcode must be precisely 12 characters long.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public void Create_BarCodeHasLetters_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = "12345665432a";

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("Product Barcode must be made up entirely of digits.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public void Create_BarCodeDoesNotCheckOut_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = Generators.GenBarCode(null, false);

            // Act
            var result = _service.Create(newItem, true).Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.Should().Be("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public void Create_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Name = null;
            newItem.Description = Generators.GenRandomString(301);

            // Act
            var result = _service.Create(newItem, true).Trim();
            var err1 = result.GetUntilOrEmpty(".").Trim();
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").GetUntilOrEmpty(".").Trim();
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            err1.Should().Be("Products require a name.");
            err2.Should().Be("Product Description is limited to 300 characters.");
            blank.Should().BeEmpty();
            createdProduct.Should().BeNull();
        }

        #endregion

        #region Updates

        [TestMethod]
        public void Update_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var oldItem = new Product
            {
                Stock = target.Stock,
                BarCode = target.BarCode,
                CategoryId = target.CategoryId,
                Description = target.Description,
                Name = target.Name,
                UnitPrice = target.UnitPrice
            };
            var targetId = target.ProductId;
            var newItem = Generators.GenProduct();
            newItem.ProductId = targetId;

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
            var targetId = Generators.GenInexistentProductId();
            var newItem = Generators.GenProduct();
            newItem.ProductId = targetId;
            var oldRepo = _service.GetAll();

            // Act
            var result = _service.Update(newItem, true);
            var newRepo = _service.GetAll();

            // Assert
            result.Should().Be("Product Data Service could not find any Product with ID:\t" + targetId + " in Repo.");
            newRepo.Should().BeEquivalentTo(oldRepo);
        }

        [TestMethod]
        public void Update_InexistentCategoryId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.CategoryId = _db.Categories.Count() + 1;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("Products require a valid Category to be placed under\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NullCategoryId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.CategoryId = null;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("Products require a valid Category to be placed under\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void Update_NoName_ShouldNotUpdateAndReturnAppropriateError(string name)
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Name = name;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("Products require a name.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NameTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Name = Generators.GenRandomString(51);

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("Product Name limited to 50 characters.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_DescriptionTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Description = Generators.GenRandomString(301);

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("Product Description is limited to 300 characters.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NegativeStock_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Stock = -3;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("Product Stock may not be negative.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NegativeUnitPrice_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.UnitPrice = -3m;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_UnitPriceZero_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.UnitPrice = 0m;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NullUnitPrice_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.UnitPrice = null;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_NullBarCode_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = null;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("Products require a Barcode.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_DuplicateBarCode_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = _db.Products.LastOrDefault().BarCode;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("Products must have a unique Bar Code.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("12345665432")]
        [DataRow("1234566543210")]
        public void Update_WrongLengthBarCode_ShouldNotUpdateAndReturnAppropriateError(string barCode)
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = barCode;

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("Product Barcode must be precisely 12 characters long.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_BarCodeHasLetters_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = "12345665432a";

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("Product Barcode must be made up entirely of digits.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_BarCodeDoesNotCheckOut_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = Generators.GenBarCode(null, false);

            // Act
            var result = _service.Update(newItem, true).Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            result.Should().Be("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public void Update_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Name = null;
            newItem.Description = Generators.GenRandomString(301);

            // Act
            var result = _service.Update(newItem, true).Trim();
            var err1 = result.GetUntilOrEmpty(".").Trim();
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".").Trim();
            var blank = result.Replace(err1, "").Replace(err2, "").GetUntilOrEmpty(".").Trim();
            var updatedCustomer = _service.Get(target.ProductId);

            // Assert
            err1.Should().Be("Products require a name.");
            err2.Should().Be("Product Description is limited to 300 characters.");
            blank.Should().BeEmpty();
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
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
            _service.AddStock(prodId, qty);
            var result = _service.Get(prodId).Stock;

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
            _service.AddStock(prodId, 0);
            var result = _service.Get(prodId).Stock;

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
            _service.AddStock(prodId, qty);
            var result = _service.Get(prodId).Stock;

            // Assert
            result.Should().Be(initStock);
        }

        [TestMethod]
        public void AddStock_InexistentId_ShouldNotModify()
        {
            // Arrange
            var initDb = _db.Products.ToList();
            var badProdId = initDb.Count() + 3;

            // Act
            _service.AddStock(badProdId, 1);
            var result = _service.GetAll();

            // Assert
            result.Should().BeEquivalentTo(initDb);
        }

        #endregion

        #region Gets

        [TestMethod]
        public void GetAll_HappyFlow_ShouldFetchAll()
        {
            var result = _service.GetAll();

            result.Should().BeEquivalentTo(_db.Products);
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
        public void GetAllFiltered_NameMatch_ShouldReturnMatch()
        {
            // Arrange
            var key = Generators.GenUnmistakableAddress();
            var targetItem = Generators.GenProduct();
            targetItem.Name = key;
            _service.Create(targetItem);

            var expected = new List<Product> { targetItem };

            // Act
            var result = _service.GetAllFiltered(targetItem.Name);

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
            _service.Create(targetItem);

            var expected = new List<Product> { targetItem };

            // Act
            var result = _service.GetAllFiltered(targetItem.Description);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_BarCodeMatch_ShouldReturnMatch()
        {
            // Arrange
            var targetItem = Generators.GenProduct();
            var expected = new List<Product> { targetItem };

            _service.Create(targetItem);

            // Act
            var result = _service.GetAllFiltered(targetItem.BarCode);

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
            _service.Create(target1);
            var target2 = new Product();
            while (true)
            {
                target2 = gen.NewProduct(manufacturer);
                if (target1.BarCode != target2.BarCode) break;
            }

            _service.Create(target2);

            var expected = new List<Product> { target1, target2 };

            // Act
            var result = _service.GetAllFiltered(manufacturer);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_StockMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Products.FirstOrDefault();
            var expected = new List<Product> { targetItem };

            var result = _service.GetAllFiltered(targetItem.BarCode);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_PriceMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Products.LastOrDefault();
            var expected = new List<Product> { targetItem };

            var result = _service.GetAllFiltered(targetItem.UnitPrice.ToString());

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_Category_ShouldReturnMatch()
        {
            // Arrange
            var targetItem = Generators.GenProduct();
            var expected = new List<Product> { targetItem };

            _service.Create(targetItem);

            // Act
            var result = _service.GetAllFiltered(_db.Categories.SingleOrDefault(c => c.CategoryId == targetItem.CategoryId).Name);

            // Assert
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
            var target1 = _db.Products.FirstOrDefault();
            var target2 = _db.Products.LastOrDefault();
            var search = target1.Stock.ToString() + " " +
                         target2.Stock.ToString();
            var expected = new List<Product> { target1, target2 };

            // Act
            var result = _service.GetAllFiltered(search);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFiltered_OverStock_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var expected = _service.GetAll().ToList();
            expected.Remove(target);

            // Act
            var result = _service.GetAllFiltered(target.Stock, true);

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
            var result = _service.GetAllFiltered(target.Stock, false);

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
            var result = _service.GetAllFiltered(target.UnitPrice.GetValueOrDefault(), true);

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
            var result = _service.GetAllFiltered(target.UnitPrice.GetValueOrDefault(), false);

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
            var result = _service.GetAllFiltered(lowStock, false).ToList();
            result.AddRange(_service.GetAllFiltered(highStock, true));
            result.AddRange(_service.GetAllFiltered(lowPrice, false));
            result.AddRange(_service.GetAllFiltered(highPrice, true));

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region FilteredAndPaged

        [TestMethod]
        public void GetAllFilteredAndPaged_HappyFlow_ShouldReturnMatch()
        {
            var expected = new List<Product> { _service.Get(2) };

            var result = _service.GetAllFilteredAndPaged(_db.Products.FirstOrDefault().UnitPrice.ToString(), 1, 2);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_OverStock_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var expected = new List<Product> { _service.Get(3) };

            // Act
            var result = _service.GetAllFilteredAndPaged(target.Stock, true, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_UnderStock_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.LastOrDefault();
            var expected = new List<Product> { _service.Get(2) };

            // Act
            var result = _service.GetAllFilteredAndPaged(target.Stock, false, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_OverPrice_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.LastOrDefault();
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = target.UnitPrice + 5;
            newItem.ProductId = target.ProductId + 1;

            _service.Create(newItem);

            var expected = new List<Product> { newItem };

            // Act
            var result = _service.GetAllFilteredAndPaged(target.UnitPrice.GetValueOrDefault() - 5, true, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllFilteredAndPaged_UnderPrice_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.LastOrDefault();
            var expected = new List<Product> { _service.Get(2) };

            // Act
            var result = _service.GetAllFilteredAndPaged(target.UnitPrice.GetValueOrDefault(), false, 1, 2);

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

            var result = _service.GetAllSorted(sortBy);

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

            var result = _service.GetAllSorted(sortBy);

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

            var result = _service.GetAllSorted(sortBy);

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

            var result = _service.GetAllSorted(sortBy);

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

            var result = _service.GetAllSorted(sortBy);

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

            var result = _service.GetAllSorted(sortBy);

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

            var result = _service.GetAllSorted(sortBy);

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

            var result = _service.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("sdfhushjb")]
        [DataRow(null)]
        public void GetAllSorted_BadQuery_ShouldReturnSorted(string sortBy)
        {
            var expected = _db.Products;

            var result = _service.GetAllSorted(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedDescending_HappyFlow_ShouldReturnSortedDescending()
        {
            var expected = _db.Products.OrderByDescending(p => p.ProductId);

            var result = _service.GetAllSorted("id", true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetAllSortedAndPaged_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(5).Take(1).Append(_db.Products.Skip(2).FirstOrDefault());

            // Act
            var result = _service.GetAllSortedAndPaged(2, 3, "price");

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
            var result = _service.GetAllSortedAndPaged(2, 3, "manufacturer", true);

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
            var result = _service.GetAllFilteredAndSorted("876259", "desc");

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
            var result = _service.GetAllFilteredAndSorted("rpg", "stock", true);

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
            var result = _service.GetAllFilteredAndSorted(25, true, "cat");

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
            var result = _service.GetAllFilteredAndSorted(25, true, "cat", true);

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
            var result = _service.GetAllFilteredAndSorted(23, false, "name");

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
            var result = _service.GetAllFilteredAndSorted(23, false, "cat", true);

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
            var result = _service.GetAllFilteredAndSorted(25.59m, true, "id");

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
            var result = _service.GetAllFilteredAndSorted(25.59m, true, "desc", true);

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
            var result = _service.GetAllFilteredAndSorted(25.59m, false, "name");

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
            var result = _service.GetAllFilteredAndSorted(25.59m, false, "STOCK", true);

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
            var result = _service.GetAllFilteredSortedAndPaged("rpg", 2, 2, "description");

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
            var result = _service.GetAllFilteredSortedAndPaged("rpg", 2, 2, "price", true);

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
            var result = _service.GetAllFilteredSortedAndPaged(16, true, 2, 2, "stock");


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
            var result = _service.GetAllFilteredSortedAndPaged(16, true, 2, 2, "price", true);

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
            var result = _service.GetAllFilteredSortedAndPaged(53, false, 2, 2, "cat");

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
            var result = _service.GetAllFilteredSortedAndPaged(53, false, 2, 2, "description", true);

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
            var result = _service.GetAllFilteredSortedAndPaged(12.35m, true, 2, 2, "name");

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
            var result = _service.GetAllFilteredSortedAndPaged(12.35m, true, 2, 2, "description", true);

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
            var result = _service.GetAllFilteredSortedAndPaged(49.95m, false, 2, 2, "name");

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
            var result = _service.GetAllFilteredSortedAndPaged(49.95m, false, 2, 2, "cost", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #endregion

        #region Validates

        #region Whole Product Validation

        [TestMethod]
        public void ValidateProduct_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var newItem = Generators.GenProduct();

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateProduct_InexistentCategoryId_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.CategoryId = _db.Categories.Count() + 1;

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Products require a valid Category to be placed under\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProduct_NullCategoryId_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.CategoryId = null;

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Products require a valid Category to be placed under\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ValidateProduct_NoName_ShouldReturnFalseAndAppropriateError(string name)
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Name = name;

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Products require a name.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProduct_NameTooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Name = Generators.GenRandomString(51);

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Product Name limited to 50 characters.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProduct_DescriptionTooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Description = Generators.GenRandomString(301);

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Product Description is limited to 300 characters.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProduct_NegativeStock_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Stock = -3;

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Product Stock may not be negative.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProduct_NegativeUnitPrice_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = -3m;

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProduct_UnitPriceZero_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = 0m;

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProduct_NullUnitPrice_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = null;

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProduct_NullBarCode_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = null;

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Products require a Barcode.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProduct_DuplicateBarCode_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = _db.Products.FirstOrDefault().BarCode;

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Products must have a unique Bar Code.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("12345665432")]
        [DataRow("1234566543210")]
        public void ValidateProduct_WrongLengthBarCode_ShouldReturnFalseAndAppropriateError(string barCode)
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = barCode;

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Product Barcode must be precisely 12 characters long.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProduct_BarCodeHasLetters_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = "12345665432a";

            // Act
            var result = _service.ValidateProduct(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Product Barcode must be made up entirely of digits.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProduct_BarCodeDoesNotCheckOut_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = Generators.GenBarCode(null, false);

            // Act
            var result = _service.ValidateProduct(newItem);
            var createdProduct = _service.Get(newItem.ProductId);

            // Assert
            result.errorList.Trim().Should().Be("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateProduct_MultipleErrors_ShouldReturnFalseAndAppropriateErrors()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Name = null;
            newItem.Description = Generators.GenRandomString(301);

            // Act
            var result = _service.ValidateProduct(newItem);
            var err1 = result.errorList.GetUntilOrEmpty(".").Trim();
            var err2 = result.errorList.Replace(err1, "").GetUntilOrEmpty(".").Trim();
            var blank = result.errorList.Replace(err1, "").Replace(err2, "").GetUntilOrEmpty(".").Trim();

            // Assert
            err1.Should().Be("Products require a name.");
            err2.Should().Be("Product Description is limited to 300 characters.");
            blank.Should().BeEmpty();
            result.isValid.Should().BeFalse();
        }

        #endregion

        #region Partial Validation

        [TestMethod]
        public void ValidateUnitPrice_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            var result = _service.ValidateUnitPrice((decimal?)new Random().Next(2,100));

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateUnitPrice_NegativeUnitPrice_ShouldReturnFalseAndAppropriateError()
        {
            var result = _service.ValidateUnitPrice(-3m);

            // Assert
            result.errorList.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateUnitPrice_UnitPriceZero_ShouldReturnFalseAndAppropriateError()
        {
            var result = _service.ValidateUnitPrice(0m);

            // Assert
            result.errorList.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateUnitPrice_NullUnitPrice_ShouldReturnFalseAndAppropriateError()
        {
            var result = _service.ValidateUnitPrice((decimal?)null);

            // Assert
            result.errorList.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateDescription_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var desc = Generators.GenRandomString(100);

            // Act
            var result = _service.ValidateDescription(desc);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateDescription_TooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var desc = Generators.GenRandomString(361);

            // Act
            var result = _service.ValidateDescription(desc);

            // Assert
            result.errorList.Trim().Should().Be("Product Description is limited to 360 characters.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateName_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var name = Generators.GenRandomString(10);

            // Act
            var result = _service.ValidateName(name);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void ValidateName_NoName_ShouldReturnFalseAndAppropriateError(string name)
        {
            var result = _service.ValidateName(name);

            // Assert
            result.errorList.Trim().Should().Be("Products require a name.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateName_TooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var name = Generators.GenRandomString(51);

            // Act
            var result = _service.ValidateName(name);

            // Assert
            result.errorList.Trim().Should().Be("Product Name limited to 50 characters.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateBarCode_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Assert
            var code = Generators.GenBarCode();

            // Act
            var result = _service.ValidateBarCode(code, 0);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateBarCode_Null_ShouldReturnFalseAndAppropriateError()
        {
            var result = _service.ValidateBarCode((string)null, 0);

            // Assert
            result.errorList.Trim().Should().Be("Products require a Barcode.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateBarCode_DuplicateBarCode_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var code = _db.Products.FirstOrDefault().BarCode;

            // Act
            var result = _service.ValidateBarCode(code, 0);

            // Assert
            result.errorList.Trim().Should().Be("Products must have a unique Bar Code.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("12345665432")]
        [DataRow("1234566543210")]
        public void ValidateBarCode_WrongLength_ShouldReturnFalseAndAppropriateError(string barCode)
        {
            var result = _service.ValidateBarCode(barCode, 0);

            // Assert
            result.errorList.Trim().Should().Be("Product Barcode must be precisely 12 characters long.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateBarCode_HasLetters_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var barCode = "12345665432a";

            // Act
            var result = _service.ValidateBarCode(barCode, 0);

            // Assert
            result.errorList.Trim().Should().Be("Product Barcode must be made up entirely of digits.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateBarCodeFormat_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Assert
            var code = Generators.GenBarCode();

            // Act
            var result = _service.ValidateBarCodeFormat(code);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow("12345665432")]
        [DataRow("1234566543210")]
        public void ValidateBarCodeFormat_WrongLength_ShouldReturnFalseAndAppropriateError(string barCode)
        {
            var result = _service.ValidateBarCodeFormat(barCode);

            // Assert
            result.errorList.Trim().Should().Be("Product Barcode must be precisely 12 characters long.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateBarCodeFormat_HasLetters_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var barCode = "12345665432a";

            // Act
            var result = _service.ValidateBarCodeFormat(barCode);

            // Assert
            result.errorList.Trim().Should().Be("Product Barcode must be made up entirely of digits.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateBarCode_BarCodeDoesNotCheckOut_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var barCode = Generators.GenBarCode(null, false);

            // Act
            var result = _service.ValidateBarCode(barCode, 0);

            // Assert
            result.errorList.Trim().Should().Be("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateBarCodeFormat_BarCodeDoesNotCheckOut_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var barCode = Generators.GenBarCode(null, false);

            // Act
            var result = _service.ValidateBarCodeFormat(barCode);

            // Assert
            result.errorList.Trim().Should().Be("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
            result.isValid.Should().BeFalse();
        }

        #endregion

        #endregion

        [TestMethod]
        public void BarcodeExists_ItDoes_ShouldReturnTrue()
        {
            // Arrange
            var code = _db.Products.FirstOrDefault().BarCode;

            // Act
            var result = _service.BarcodeExists(code);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void BarcodeExists_ItDoesNot_ShouldReturnFalse()
        {
            // Arrange
            var code = _db.Products.FirstOrDefault().BarCode;
            while (_db.Products.Any(p => p.BarCode == code)) code = Generators.GenBarCode();

            // Act
            var result = _service.BarcodeExists(code);

            // Assert
            result.Should().BeFalse();
        }

        #region Async

        [TestMethod]
        public async Task GetAsync_HappyFlow_ShouldReturnTarget()
        {
            var target = _db.Products.FirstOrDefault();

            var result = await _service.GetAsync(target.ProductId);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public async Task GetAsync_InexistentId_ShouldReturnNull()
        {
            var id = Generators.GenInexistentProductId();

            var result = await _service.GetAsync(id);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetByBarcodeAsync_HappyFlow_ShouldReturnTarget()
        {
            var target = _db.Products.FirstOrDefault();

            var result = await _service.GetByBarcodeAsync(target.BarCode);

            result.Should().BeEquivalentTo(target);
        }

        [TestMethod]
        public async Task GetByBarcodeAsync_InexistentId_ShouldReturnNull()
        {
            var badCode = Generators.GenBarCode();

            var result = await _service.GetByBarcodeAsync(badCode);

            result.Should().BeNull();
        }


        #region Creates

        [TestMethod]
        public async Task CreateAsync_HappyFlow_ShouldCreateAsync()
        {
            // Arrange
            var newItem = Generators.GenProduct();

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdItem = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Should().BeNull();
            createdItem.Should().BeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task CreateAsync_InexistentCategoryId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.CategoryId = _db.Categories.Count() + 1;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("Products require a valid Category to be placed under;"+
                "\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_NullCategoryId_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.CategoryId = null;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("Products require a valid Category to be placed under;"+
                "\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            createdProduct.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task CreateAsync_NoName_ShouldNotCreateAndReturnAppropriateError(string name)
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Name = name;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("Products require a name.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_NameTooLong_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Name = Generators.GenRandomString(51);

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("Product Name limited to 50 characters.");
            createdProduct.Should().BeNull();
        }

        //[TestMethod]
        //public async Task CreateAsync_DescriptionTooLong_ShouldNotCreateAndReturnAppropriateError()
        //{
        //    // Arrange
        //    var newItem = Generators.GenProduct();
        //    newItem.Description = Generators.GenRandomString(301);

        //    // Act
        //    var result = await _service.CreateAsync(newItem, true);
        //    var createdProduct = await _service.GetAsync(newItem.ProductId);

        //    // Assert
        //    result.Trim().Should().Be("Product Description is limited to 300 characters.");
        //    createdProduct.Should().BeNull();
        //}

        [TestMethod]
        public async Task CreateAsync_NegativeStock_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Stock = -3;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("Product Stock may not be negative.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_NegativeUnitPrice_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = -3m;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_UnitPriceZero_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = 0m;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_NullUnitPrice_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = null;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_NullBarCodeAsync_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = null;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("Products require a Barcode.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_DuplicateBarCodeAsync_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = _db.Products.FirstOrDefault().BarCode;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("Products must have a unique Bar Code.");
            createdProduct.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("12345665432")]
        [DataRow("1234566543210")]
        public async Task CreateAsync_WrongLengthBarCodeAsync_ShouldNotCreateAndReturnAppropriateError(string barCode)
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = barCode;

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("Product Barcode must be precisely 12 characters long.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_BarCodeHasLetters_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = "12345665432a";

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("Product Barcode must be made up entirely of digits.");
            createdProduct.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateAsync_BarCodeDoesNotCheckOut_ShouldNotCreateAndReturnAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = Generators.GenBarCode(null, false);

            // Act
            var result = await _service.CreateAsync(newItem, true);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.Trim().Should().Be("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,"+
                "\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
            createdProduct.Should().BeNull();
        }

        //[TestMethod]
        //public async Task CreateAsync_MultipleErrors_ShouldNotCreateAndReturnAppropriateErrors()
        //{
        //    // Arrange
        //    var newItem = Generators.GenProduct();
        //    newItem.Name = null;
        //    newItem.Description = Generators.GenRandomString(301);

        //    // Act
        //    var result = await _service.CreateAsync(newItem, true);
        //    var err1 = result.GetUntilOrEmpty(".");
        //    var err2 = result.Replace(err1, "").GetUntilOrEmpty(".");
        //    var blank = result.Replace(err1, "").Replace(err2, "").GetUntilOrEmpty(".");
        //    var createdProduct = await _service.GetAsync(newItem.ProductId);

        //    // Assert
        //    err1.Should().Be("Products require a name.");
        //    err2.Should().Be("Product Description is limited to 300 characters.");
        //    blank.Should().BeEmpty();
        //    createdProduct.Should().BeNull();
        //}

        #endregion

        #region Updates

        [TestMethod]
        public async Task UpdateAsync_HappyFlow_ShouldUpdate()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var oldItem = new Product
            {
                Stock = target.Stock,
                BarCode = target.BarCode,
                CategoryId = target.CategoryId,
                Description = target.Description,
                Name = target.Name,
                UnitPrice = target.UnitPrice
            };
            var targetId = target.ProductId;
            var newItem = Generators.GenProduct();
            newItem.ProductId = targetId;

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
            var targetId = Generators.GenInexistentProductId();
            var newItem = Generators.GenProduct();
            newItem.ProductId = targetId;
            var oldRepo = await _service.GetAllAsync();

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var newRepo = await _service.GetAllAsync();

            // Assert
            result.Should().Be("Product Data Service could not find any Product with ID:\t" + targetId + " in Repo.");
            newRepo.Should().BeEquivalentTo(oldRepo);
        }

        [TestMethod]
        public async Task UpdateAsync_InexistentCategoryId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.CategoryId = _db.Categories.Count() + 1;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("Products require a valid Category to be placed under;"+
                "\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_NullCategoryId_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.CategoryId = null;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("Products require a valid Category to be placed under;"+
                "\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task UpdateAsync_NoName_ShouldNotUpdateAndReturnAppropriateError(string name)
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Name = name;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("Products require a name.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_NameTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Name = Generators.GenRandomString(51);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("Product Name limited to 50 characters.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_DescriptionTooLong_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Description = Generators.GenRandomString(301);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("Product Description is limited to 300 characters.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_NegativeStock_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Stock = -3;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("Product Stock may not be negative.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_NegativeUnitPrice_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.UnitPrice = -3m;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_UnitPriceZero_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.UnitPrice = 0m;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_NullUnitPrice_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.UnitPrice = null;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("A Product's Unit Price must be greter than 0$.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_NullBarCodeAsync_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = null;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("Products require a Barcode.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_DuplicateBarCodeAsync_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = _db.Products.LastOrDefault().BarCode;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("Products must have a unique Bar Code.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [DataTestMethod]
        [DataRow("12345665432")]
        [DataRow("1234566543210")]
        public async Task UpdateAsync_WrongLengthBarCodeAsync_ShouldNotUpdateAndReturnAppropriateError(string barCode)
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = barCode;

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("Product Barcode must be precisely 12 characters long.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_BarCodeHasLetters_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = "12345665432a";

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("Product Barcode must be made up entirely of digits.");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_BarCodeDoesNotCheckOut_ShouldNotUpdateAndReturnAppropriateError()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.BarCode = Generators.GenBarCode(null, false);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            result.Should().Be("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        [TestMethod]
        public async Task UpdateAsync_MultipleErrors_ShouldNotUpdateAndReturnAppropriateErrors()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var newItem = Generators.GenProduct();
            newItem.ProductId = target.ProductId;
            newItem.Name = null;
            newItem.Description = Generators.GenRandomString(301);

            // Act
            var result = await _service.UpdateAsync(newItem, true);
            var err1 = result.GetUntilOrEmpty(".");
            var err2 = result.Replace(err1, "").GetUntilOrEmpty(".");
            var blank = result.Replace(err1, "").Replace(err2, "").GetUntilOrEmpty(".");
            var updatedCustomer = await _service.GetAsync(target.ProductId);

            // Assert
            err1.Should().Be("Products require a name.");
            err2.Should().Be("Product Description is limited to 300 characters.");
            blank.Should().BeEmpty();
            updatedCustomer.Should().BeEquivalentTo(target);
            updatedCustomer.Should().NotBeEquivalentTo(newItem);
        }

        #endregion

        #region AddStock

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(-3)]
        public async Task AddStockAsync_HappyFlow_ShouldAddOrSubstract(int qty)
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            var prodId = prod.ProductId;
            var initStock = prod.Stock;

            // Act
            await _service.AddStockAsync(prodId, qty);
            var result = (await _service.GetAsync(prodId)).Stock;

            // Assert
            result.Should().Be(initStock + qty);
            if (qty < 0) result.Should().BeLessThan(initStock);
        }

        [TestMethod]
        public async Task AddStockAsync_Zero_ShouldNotModify()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            var prodId = prod.ProductId;
            var initStock = prod.Stock;

            // Act
            await _service.AddStockAsync(prodId, 0);
            var result = (await _service.GetAsync(prodId)).Stock;

            // Assert
            result.Should().Be(initStock);
        }

        [TestMethod]
        public async Task AddStockAsync_SubtractTooMuch_ShouldNotModify()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            var prodId = prod.ProductId;
            var initStock = prod.Stock;
            var qty = (prod.Stock + 1) * (-1);

            // Act
            await _service.AddStockAsync(prodId, qty);
            var result = (await _service.GetAsync(prodId)).Stock;

            // Assert
            result.Should().Be(initStock);
        }

        [TestMethod]
        public async Task AddStockAsync_InexistentId_ShouldNotModify()
        {
            // Arrange
            var initDb = _db.Products.ToList();
            var badProdId = initDb.Count() + 3;

            // Act
            await _service.AddStockAsync(badProdId, 1);
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(initDb);
        }

        #endregion


        #region Gets

        [TestMethod]
        public async Task GetAllAsync_HappyFlow_ShouldFetchAll()
        {
            var result = await _service.GetAllAsync();

            result.Should().BeEquivalentTo(_db.Products);
        }

        #region Paginated

        [TestMethod]
        public async Task GetAllPaginatedAsync_HappyFlow_ShouldReturnRightPage()
        {
            // Arrange
            int pgSize = 1;
            int pgIndex = 3;
            var expected = _db.Products.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();

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
            var index = await _service.GetAllAsync();
            var overIndex = index.Count() + 10;

            var result = await _service.GetAllPaginatedAsync(1, overIndex);

            result.Should().BeEmpty();
        }

        #endregion

        #region Filtered

        [TestMethod]
        public async Task GetAllFilteredAsync_NameMatch_ShouldReturnMatch()
        {
            // Arrange
            var key = Generators.GenUnmistakableAddress();
            var targetItem = Generators.GenProduct();
            targetItem.Name = key;
            _service.CreateAsync(targetItem);

            var expected = new List<Product> { targetItem };

            // Act
            var result = await _service.GetAllFilteredAsync(targetItem.Name);

            // Assert
            result.Should().BeEquivalentTo(targetItem);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_DescriptionMatch_ShouldReturnMatch()
        {
            // Arrange
            var key = Generators.GenUnmistakableAddress();
            var targetItem = Generators.GenProduct();
            targetItem.Description = key;
            _service.CreateAsync(targetItem);

            var expected = new List<Product> { targetItem };

            // Act
            var result = await _service.GetAllFilteredAsync(targetItem.Description);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_BarCodeMatch_ShouldReturnMatch()
        {
            // Arrange
            var targetItem = Generators.GenProduct();
            var expected = new List<Product> { targetItem };

            _service.CreateAsync(targetItem);

            // Act
            var result = await _service.GetAllFilteredAsync(targetItem.BarCode);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_ManufacturerMatch_ShouldReturnMatches()
        {
            // Arrange
            var manufacturer = new Random().Next(1, 1000000).ToString("D6");

            var gen = new InstanceTestGenerator(_db);

            var target1 = gen.NewProduct(manufacturer);
            _service.CreateAsync(target1);
            var target2 = new Product();
            while (true)
            {
                target2 = gen.NewProduct(manufacturer);
                if (target1.BarCode != target2.BarCode) break;
            }

            _service.CreateAsync(target2);

            var expected = new List<Product> { target1, target2 };

            // Act
            var result = await _service.GetAllFilteredAsync(manufacturer);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_StockMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Products.FirstOrDefault();
            var expected = new List<Product> { targetItem };

            var result = await _service.GetAllFilteredAsync(targetItem.BarCode);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_PriceMatch_ShouldReturnMatch()
        {
            var targetItem = _db.Products.LastOrDefault();
            var expected = new List<Product> { targetItem };

            var result = await _service.GetAllFilteredAsync(targetItem.UnitPrice.ToString());

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_Category_ShouldReturnMatch()
        {
            // Arrange
            var targetItem = Generators.GenProduct();
            var expected = new List<Product> { targetItem };

            _service.CreateAsync(targetItem);

            // Act
            var result = await _service.GetAllFilteredAsync(_db.Categories.SingleOrDefault(c => c.CategoryId == targetItem.CategoryId).Name);

            // Assert
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
            var target1 = _db.Products.FirstOrDefault();
            var target2 = _db.Products.LastOrDefault();
            var search = target1.Stock.ToString() + " " +
                         target2.Stock.ToString();
            var expected = new List<Product> { target1, target2 };

            // Act
            var result = await _service.GetAllFilteredAsync(search);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_OverStock_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();//exclusion target
            var allProds = await _service.GetAllAsync();
            var expected = allProds.Where(p => p.ProductId != target.ProductId);

            // Act
            var result = await _service.GetAllFilteredAsync(target.Stock, true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_UnderStock_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.LastOrDefault();
            var expected = _db.Products.ToList();
            expected.Remove(target);

            // Act
            var result = await _service.GetAllFilteredAsync(target.Stock, false);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_OverPrice_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var expected = new List<Product> { _db.Products.LastOrDefault() };

            // Act
            var result = await _service.GetAllFilteredAsync(target.UnitPrice.GetValueOrDefault(), true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_UnderPrice_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.LastOrDefault();
            var expected = _db.Products.ToList();
            expected.Remove(target);

            // Act
            var result = await _service.GetAllFilteredAsync(target.UnitPrice.GetValueOrDefault(), false);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAsync_StocksAndPricesOutOfBounds_ShouldReturnEmpty()
        {
            // Arrange
            var lowStock = _db.Products.FirstOrDefault().Stock;
            var highStock = _db.Products.LastOrDefault().Stock;
            var lowPrice = _db.Products.FirstOrDefault().UnitPrice.GetValueOrDefault();
            var highPrice = _db.Products.LastOrDefault().UnitPrice.GetValueOrDefault();

            // Act
            var lowStCall = await _service.GetAllFilteredAsync(lowStock, false);
            var result = lowStCall.ToList();
            var highStCall = await _service.GetAllFilteredAsync(highStock, true);
            result.AddRange(highStCall.ToList());
            var lowPrCall = await _service.GetAllFilteredAsync(lowPrice, false);
            result.AddRange(lowPrCall.ToList());
            var highPrCall = await _service.GetAllFilteredAsync(highPrice, true);
            result.AddRange(highPrCall.ToList());
            //TODO: thedecade of souls
            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region FilteredAndPaged

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_HappyFlow_ShouldReturnMatch()
        {
            var expected = new List<Product> { await _service.GetAsync(2) };

            var result = await _service.GetAllFilteredAndPagedAsync(_db.Products.FirstOrDefault().UnitPrice.ToString(), 1, 2);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_OverStock_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.FirstOrDefault();
            var expected = new List<Product> { await _service.GetAsync(3) };

            // Act
            var result = await _service.GetAllFilteredAndPagedAsync(target.Stock, true, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_UnderStock_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.LastOrDefault();
            var expected = new List<Product> { await _service.GetAsync(2) };

            // Act
            var result = await _service.GetAllFilteredAndPagedAsync(target.Stock, false, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_OverPrice_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.LastOrDefault();
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = target.UnitPrice + 5;
            newItem.ProductId = target.ProductId + 1;

            _service.CreateAsync(newItem);

            var expected = new List<Product> { newItem };

            // Act
            var result = await _service.GetAllFilteredAndPagedAsync(target.UnitPrice.GetValueOrDefault() - 5, true, 1, 2);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndPagedAsync_UnderPrice_ShouldReturnMatches()
        {
            // Arrange
            var target = _db.Products.LastOrDefault();
            var expected = new List<Product> { await _service.GetAsync(2) };

            // Act
            var result = await _service.GetAllFilteredAndPagedAsync(target.UnitPrice.GetValueOrDefault(), false, 1, 2);

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
        public async Task GetAllSortedAsync_ById_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Products.OrderBy(p => p.ProductId);

            var result = await _service.GetAllSortedAsync(sortBy);

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
        public async Task GetAllSortedAsync_ByName_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Products.OrderBy(p => p.Name);

            var result = await _service.GetAllSortedAsync(sortBy);

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
        public async Task GetAllSortedAsync_ByManufacturer_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Products.OrderBy(p => p.Manufacturer);

            var result = await _service.GetAllSortedAsync(sortBy);

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
        public async Task GetAllSortedAsync_ByBarCodeAsync_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Products.OrderBy(p => p.BarCode);

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("STOCK")]
        [DataRow("stOck")]
        [DataRow("STK")]
        [DataRow("sTk")]
        public async Task GetAllSortedAsync_ByStock_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Products.OrderBy(p => p.Stock);

            var result = await _service.GetAllSortedAsync(sortBy);

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
        public async Task GetAllSortedAsync_ByPrice_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Products.OrderBy(p => p.UnitPrice);

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("CATEGORY")]
        [DataRow("CaTegOry")]
        [DataRow("CAT")]
        [DataRow("Cat")]
        public async Task GetAllSortedAsync_ByCategory_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Products.OrderBy(p => p.CategoryId);

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("DESCRIPTION")]
        [DataRow("DecRiptIon")]
        [DataRow("DESC")]
        [DataRow("dEsc")]
        public async Task GetAllSortedAsync_ByDescriptionAsync_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Products.OrderBy(p => p.Description.Length);

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [DataTestMethod]
        [DataRow("sdfhushjb")]
        [DataRow(null)]
        public async Task GetAllSortedAsync_BadQuery_ShouldReturnSortedAsync(string sortBy)
        {
            var expected = _db.Products;

            var result = await _service.GetAllSortedAsync(sortBy);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedDescendingAsync_HappyFlow_ShouldReturnSortedDescending()
        {
            var expected = _db.Products.OrderByDescending(p => p.ProductId);

            var result = await _service.GetAllSortedAsync("id", true);

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(5).Take(1).Append(_db.Products.Skip(2).FirstOrDefault());

            // Act
            var result = await _service.GetAllSortedAndPagedAsync(2, 3, "price");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(4).Take(2);

            // Act
            var result = await _service.GetAllSortedAndPagedAsync(2, 3, "manufacturer", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndSortedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(2).Reverse();

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync("876259", "desc");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredAndSortedDescendingAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(3).Reverse();

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync("rpg", "stock", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllOverStockAndSortedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(2).Take(2).Reverse();

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(25, true, "cat");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllOverStockAndSortedDescendingAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(2).Take(2);

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(25, true, "cat", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllUnderStockAndSortedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(23, false, "name");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllUnderStockAndSortedDescendingAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(23, false, "cat", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllOverPriceAndSortedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(2).Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(25.59m, true, "id");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllOverPriceAndSortedDescendingAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(2).Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(25.59m, true, "desc", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllUnderPriceAndSortedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(3).Take(2);

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(25.59m, false, "name");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllUnderPriceAndSortedDescendingAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(3).Take(2);

            // Act
            var result = await _service.GetAllFilteredAndSortedAsync(25.59m, false, "STOCK", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredSortedAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(1).Take(1);

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync("rpg", 2, 2, "description");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllFilteredSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(1).Take(1);

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync("rpg", 2, 2, "price", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllOverStockSortedAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(2).Take(2).Reverse();

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(16, true, 2, 2, "stock");


            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllOverStockSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(3).Take(2).Reverse();

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(16, true, 2, 2, "price", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllUnderStockSortedAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(1).Take(1).Append(_db.Products.Skip(4).FirstOrDefault());

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(53, false, 2, 2, "cat");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllUnderStockSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(53, false, 2, 2, "description", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllOverPriceSortedAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(12.35m, true, 2, 2, "name");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllOverPriceSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Take(1).Append(_db.Products.LastOrDefault());

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(12.35m, true, 2, 2, "description", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllUnderPriceSortedAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(3).Take(2);

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(49.95m, false, 2, 2, "name");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task GetAllUnderPriceSortedDescendingAndPagedAsync_HappyFlow_ShouldReturnRequested()
        {
            // Arrange
            _db.BumpProducts();
            var expected = _db.Products.Skip(3).Take(2).Reverse();

            // Act
            var result = await _service.GetAllFilteredSortedAndPagedAsync(49.95m, false, 2, 2, "cost", true);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        #endregion

        #endregion

        #region Validates

        #region Whole Product Validation

        [TestMethod]
        public async Task ValidateProductAsync_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var newItem = Generators.GenProduct();

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public async Task ValidateProductAsync_InexistentCategoryId_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.CategoryId = _db.Categories.Count() + 1;

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Products require a valid Category to be placed under;"+
                "\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateProductAsync_NullCategoryId_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.CategoryId = null;

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Products require a valid Category to be placed under;"+
                "\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task ValidateProductAsync_NoName_ShouldReturnFalseAndAppropriateError(string name)
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Name = name;

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Products require a name.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateProductAsync_NameTooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Name = Generators.GenRandomString(51);

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Product Name limited to 50 characters.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateProductAsync_DescriptionTooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Description = Generators.GenRandomString(301);

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Product Description is limited to 300 characters.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateProductAsync_NegativeStock_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Stock = -3;

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Product Stock may not be negative.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateProductAsync_NegativeUnitPrice_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = -3m;

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateProductAsync_UnitPriceZero_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = 0m;

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateProductAsync_NullUnitPrice_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.UnitPrice = null;

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateProductAsync_NullBarCodeAsync_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = null;

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Products require a Barcode.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateProductAsync_DuplicateBarCodeAsync_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = _db.Products.FirstOrDefault().BarCode;

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Products must have a unique Bar Code.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("12345665432")]
        [DataRow("1234566543210")]
        public async Task ValidateProductAsync_WrongLengthBarCodeAsync_ShouldReturnFalseAndAppropriateError(string barCode)
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = barCode;

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Product Barcode must be precisely 12 characters long.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateProductAsync_BarCodeHasLetters_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = "12345665432a";

            // Act
            var result = await _service.ValidateProductAsync(newItem);

            // Assert
            result.errorList.Trim().Should().Be("Product Barcode must be made up entirely of digits.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateProductAsync_BarCodeDoesNotCheckOut_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.BarCode = Generators.GenBarCode(null, false);

            // Act
            var result = await _service.ValidateProductAsync(newItem);
            var createdProduct = await _service.GetAsync(newItem.ProductId);

            // Assert
            result.errorList.Trim().Should().Be("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateProductAsync_MultipleErrors_ShouldReturnFalseAndAppropriateErrors()
        {
            // Arrange
            var newItem = Generators.GenProduct();
            newItem.Name = null;
            newItem.Description = Generators.GenRandomString(301);

            // Act
            var result = await _service.ValidateProductAsync(newItem);
            var err1 = result.errorList.GetUntilOrEmpty(".");
            var err2 = result.errorList.Replace(err1, "").GetUntilOrEmpty(".");
            var blank = result.errorList.Replace(err1, "").Replace(err2, "").GetUntilOrEmpty(".");

            // Assert
            err1.Should().Be("Products require a name.");
            err2.Should().Be("Product Description is limited to 300 characters.");
            blank.Should().BeEmpty();
            result.isValid.Should().BeFalse();
        }

        #endregion

        #region Partial Validation

        [TestMethod]
        public async Task ValidateUnitPriceAsync_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            var result = await _service.ValidateUnitPriceAsync((decimal?)new Random().Next(2, 100));

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public async Task ValidateUnitPriceAsync_NegativeUnitPrice_ShouldReturnFalseAndAppropriateError()
        {
            var result = await _service.ValidateUnitPriceAsync(-3m);

            // Assert
            result.errorList.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateUnitPriceAsync_UnitPriceZero_ShouldReturnFalseAndAppropriateError()
        {
            var result = await _service.ValidateUnitPriceAsync(0m);

            // Assert
            result.errorList.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateUnitPriceAsync_NullUnitPrice_ShouldReturnFalseAndAppropriateError()
        {
            var result = await _service.ValidateUnitPriceAsync((decimal?)null);

            // Assert
            result.errorList.Trim().Should().Be("A Product's Unit Price must be greter than 0$.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateDescriptionAsync_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var desc = Generators.GenRandomString(100);

            // Act
            var result = await _service.ValidateDescriptionAsync(desc);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public async Task ValidateDescriptionAsync_TooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var desc = Generators.GenRandomString(361);

            // Act
            var result = await _service.ValidateDescriptionAsync(desc);

            // Assert
            result.errorList.Trim().Should().Be("Product Description is limited to 360 characters.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateNameAsync_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Arrange
            var name = Generators.GenRandomString(10);

            // Act
            var result = await _service.ValidateNameAsync(name);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task ValidateNameAsync_NoName_ShouldReturnFalseAndAppropriateError(string name)
        {
            var result = await _service.ValidateNameAsync(name);

            // Assert
            result.errorList.Trim().Should().Be("Products require a name.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateNameAsync_TooLong_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var name = Generators.GenRandomString(51);

            // Act
            var result = await _service.ValidateNameAsync(name);

            // Assert
            result.errorList.Trim().Should().Be("Product Name limited to 50 characters.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateBarCodeAsync_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Assert
            var code = Generators.GenBarCode();

            // Act
            var result = await _service.ValidateBarCodeAsync(code, 0);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [TestMethod]
        public async Task ValidateBarCodeAsync_Null_ShouldReturnFalseAndAppropriateError()
        {
            var result = await _service.ValidateBarCodeAsync((string)null, 0);

            // Assert
            result.errorList.Trim().Should().Be("Products require a Barcode.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateBarCodeAsync_DuplicateBarCodeAsync_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var code = _db.Products.FirstOrDefault().BarCode;

            // Act
            var result = await _service.ValidateBarCodeAsync(code, 0);

            // Assert
            result.errorList.Trim().Should().Be("Products must have a unique Bar Code.");
            result.isValid.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("12345665432")]
        [DataRow("1234566543210")]
        public async Task ValidateBarCodeAsync_WrongLength_ShouldReturnFalseAndAppropriateError(string barCode)
        {
            var result = await _service.ValidateBarCodeAsync(barCode, 0);

            // Assert
            result.errorList.Trim().Should().Be("Product Barcode must be precisely 12 characters long.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateBarCodeAsync_HasLetters_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var barCode = "12345665432a";

            // Act
            var result = await _service.ValidateBarCodeAsync(barCode, 0);

            // Assert
            result.errorList.Trim().Should().Be("Product Barcode must be made up entirely of digits.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateBarCodeFormatAsync_HappyFlow_ShouldReturnTrueAndEmpty()
        {
            // Assert
            var code = Generators.GenBarCode();

            // Act
            var result = await _service.ValidateBarCodeFormatAsync(code);

            // Assert
            result.errorList.Should().BeEmpty();
            result.isValid.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow("12345665432")]
        [DataRow("1234566543210")]
        public async Task ValidateBarCodeFormatAsync_WrongLength_ShouldReturnFalseAndAppropriateError(string barCode)
        {
            var result = await _service.ValidateBarCodeFormatAsync(barCode);

            // Assert
            result.errorList.Trim().Should().Be("Product Barcode must be precisely 12 characters long.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateBarCodeFormatAsync_HasLetters_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var barCode = "12345665432a";

            // Act
            var result = await _service.ValidateBarCodeFormatAsync(barCode);

            // Assert
            result.errorList.Trim().Should().Be("Product Barcode must be made up entirely of digits.");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateBarCodeAsync_BarCodeDoesNotCheckOut_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var barCode = Generators.GenBarCode(null, false);

            // Act
            var result = await _service.ValidateBarCodeAsync(barCode, 0);

            // Assert
            result.errorList.Trim().Should().Be("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
            result.isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateBarCodeFormatAsync_BarCodeDoesNotCheckOut_ShouldReturnFalseAndAppropriateError()
        {
            // Arrange
            var barCode = Generators.GenBarCode(null, false);

            // Act
            var result = await _service.ValidateBarCodeFormatAsync(barCode);

            // Assert
            result.errorList.Trim().Should().Be("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
            result.isValid.Should().BeFalse();
        }

        #endregion

        #endregion

        //[TestMethod]
        //public async Task BarcodeExistsAsync_ItDoes_ShouldReturnTrue()
        //{
        //    // Arrange
        //    var code = _db.Products.FirstOrDefault().BarCode;

        //    // Act
        //    var result = await _service.BarcodeExistsAsync(code);

        //    // Assert
        //    result.Should().BeFalse();
        //}

        //[TestMethod]
        //public async Task BarcodeExistsAsync_ItDoesNot_ShouldReturnFalse()
        //{
        //    // Arrange
        //    var code = _db.Products.FirstOrDefault().BarCode;
        //    while (_db.Products.Any(p => p.BarCode == code)) code = Generators.GenBarCode();

        //    // Act
        //    var result = await _service.BarcodeExistsAsync(code);

        //    // Assert
        //    result.Should().BeFalse();
        //}

        #endregion
    }
}
