using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.DAL.Interfaces;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DragonDrop.UnitTests.WPF.Forms
{
    [TestClass]
    [Ignore]

    public class ProductEditVMTests
    {
        private TestDb _db;
        private IProductRepository _repo;
        private ICategoryRepository _catRepo;
        private IProductDataService _service;

        private Product _ogProd;
        private ProductEditViewModel _editVM;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestProductRepo(_db);
            _catRepo = new TestCategoryRepo(_db);
            //_prodRepo = new TestProductRepo(_db);
            _service = new ProductDataService(_repo);
            //_prodService = new ProductDataService(_prodRepo);

            _ogProd = _db.Products.FirstOrDefault();

            _editVM = new ProductEditViewModel(_service, _catRepo, _ogProd.ProductId);
        }

        #region Focus Changes

        #region Name

        [TestMethod]
        public void NameLostFocus_EmptyBox_ShouldInsertOgValue()
        {
            // Arrange
            _editVM.NameEntryText = string.Empty;

            // Act
            _editVM.NameBoxLostFocusCommand.Execute();

            // Assert
            _editVM.NameEntryText.Should().Be(_ogProd.Name);
        }

        [TestMethod]
        public void NameLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.NameEntryText = text;

            // Act
            _editVM.NameBoxLostFocusCommand.Execute();

            // Assert
            _editVM.NameEntryText.Should().Be(text);
        }

        [TestMethod]
        public void NameReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.NameEntryText = "a";

            // Act
            _editVM.NameResetCommand.Execute();

            // Assert
            _editVM.NameEntryText.Should().Be(_ogProd.Name);
        }

        #endregion

        #region Price

        [TestMethod]
        public void PriceLostFocus_EmptyBox_ShouldInsertOgValue()
        {
            // Arrange
            _editVM.PriceEntryText = string.Empty;

            // Act
            _editVM.PriceBoxLostFocusCommand.Execute();

            // Assert
            _editVM.PriceEntryText.Should().Be(string.Format("{0:C}", _ogProd.UnitPrice.GetValueOrDefault()));
        }

        [TestMethod]
        public void PriceLostFocus_ContainsGibberish_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.PriceEntryText = text;

            // Act
            _editVM.PriceBoxLostFocusCommand.Execute();

            // Assert
            _editVM.PriceEntryText.Should().Be(text);
        }

        [TestMethod]
        public void PriceLostFocus_ContainsAmount_ShouldFormatText()
        {
            // Arrange
            var text = "12.99";
            _editVM.PriceEntryText = text;

            // Act
            _editVM.PriceBoxLostFocusCommand.Execute();

            // Assert
            _editVM.PriceEntryText.Should().Be(string.Format("{0:C}",decimal.Parse(text)));
        }

        [TestMethod]
        public void PriceReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.PriceEntryText = "a";

            // Act
            _editVM.PriceResetCommand.Execute();

            // Assert
            _editVM.PriceEntryText.Should().Be(string.Format("{0:C}", _ogProd.UnitPrice.GetValueOrDefault()));
        }

        #endregion

        #region Stock

        [TestMethod]
        public void StockLostFocus_EmptyBox_ShouldInsertOgValue()
        {
            // Arrange
            _editVM.StockEntryText = string.Empty;

            // Act
            _editVM.StockBoxLostFocusCommand.Execute();

            // Assert
            _editVM.StockEntryText.Should().Be(_ogProd.Stock.ToString());
        }

        [TestMethod]
        public void StockLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.StockEntryText = text;

            // Act
            _editVM.StockBoxLostFocusCommand.Execute();

            // Assert
            _editVM.StockEntryText.Should().Be(text);
        }

        [TestMethod]
        public void StockReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.StockEntryText = "a";

            // Act
            _editVM.StockResetCommand.Execute();

            // Assert
            _editVM.StockEntryText.Should().Be(_ogProd.Stock.ToString());
        }

        #endregion

        #region BarCode

        [TestMethod]
        public void CodeLostFocus_EmptyBox_ShouldInsertOgValue()
        {
            // Arrange
            _editVM.CodeEntryText = string.Empty;

            // Act
            _editVM.CodeBoxLostFocusCommand.Execute();

            // Assert
            _editVM.CodeEntryText.Should().Be(_ogProd.BarCode);
        }

        [TestMethod]
        public void CodeLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.CodeEntryText = text;

            // Act
            _editVM.CodeBoxLostFocusCommand.Execute();

            // Assert
            _editVM.CodeEntryText.Should().Be(text);
        }

        [TestMethod]
        public void CodeReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.CodeEntryText = "a";

            // Act
            _editVM.CodeResetCommand.Execute();

            // Assert
            _editVM.CodeEntryText.Should().Be(_ogProd.BarCode);
        }

        #endregion

        #region Description

        [TestMethod]
        public void DescriptionLostFocus_EmptyBox_ShouldInsertOgValue()
        {
            // Arrange
            _editVM.DescEntryText = string.Empty;

            // Act
            _editVM.DescBoxLostFocusCommand.Execute();

            // Assert
            _editVM.DescEntryText.Should().Be(_ogProd.Description);
        }

        [TestMethod]
        public void DescriptionLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _editVM.DescEntryText = text;

            // Act
            _editVM.DescBoxLostFocusCommand.Execute();

            // Assert
            _editVM.DescEntryText.Should().Be(text);
        }

        [TestMethod]
        public void DescriptionReset_HappyFlow_ShouldUseOgValues()
        {
            // Arrange
            _editVM.DescEntryText = "a";

            // Act
            _editVM.DescResetCommand.Execute();

            // Assert
            _editVM.DescEntryText.Should().Be(_ogProd.Description);
        }

        #endregion

        [TestMethod]
        public void CatReset_HappyFlow_ShouldRevertToOgValue()
        {
            // Arrange
            _editVM.SelIndex = _db.Categories.Where(c => c.CategoryId != _ogProd.CategoryId).FirstOrDefault().CategoryId;

            // Act
            _editVM.CatResetCommand.Execute();

            // Assert
            _editVM.SelIndex.Should().Be(_ogProd.CategoryId);
        }

        #endregion

        #region Text Updates

        [TestMethod]
        public void NameTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.NameEntryText = _db.Products.FirstOrDefault().Name;

            // Act
            _editVM.NameBoxTextChangedCommand.Execute();

            // Assert
            _editVM.NameErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void PriceTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.PriceEntryText = _db.Products.FirstOrDefault().UnitPrice.GetValueOrDefault().ToString();

            // Act
            _editVM.PriceBoxTextChangedCommand.Execute();

            // Assert
            _editVM.PriceErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void StockTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.StockEntryText = _db.Products.FirstOrDefault().Stock.ToString();

            // Act
            _editVM.StockBoxTextChangedCommand.Execute();

            // Assert
            _editVM.StockErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void DescTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _editVM.DescEntryText = "a";

            // Act
            _editVM.DescBoxTextChangedCommand.Execute();

            // Assert
            _editVM.DescErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void DescTxtChanged_NormalLength_ShouldUpdateCounter()
        {
            // Arrange
            _editVM.DescEntryText = "a";

            // Act
            _editVM.DescBoxTextChangedCommand.Execute();

            // Assert
            _editVM.RemainingDesc.Should().Be(_editVM.DescEntryText.Length);
            _editVM.DescCounterFg.Should().Be(Brushes.LightGray);
        }

        [TestMethod]
        public void DescTxtChanged_TextLimit_ShouldTrimAndReddenCounter()
        {
            // Arrange
            var text = Generators.GenRandomString(361);
            _editVM.DescEntryText = text;

            // Act
            _editVM.DescBoxTextChangedCommand.Execute();

            // Assert
            _editVM.DescEntryText.Should().Be(text.Substring(0, 360));
            _editVM.RemainingDesc.Should().Be(0);
            _editVM.DescCounterFg.Should().Be(Brushes.LightGray);
        }

        //Bad Names

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void NameTxtChanged_Empty_ShouldShowAppropriateError(string name)
        {
            // Arrange
            _editVM.NameEntryText = name;

            // Act
            _editVM.NameBoxTextChangedCommand.Execute();

            // Assert
            _editVM.NameErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.NameErrorText.Should().Be("Products require a name.");
        }
        
        [TestMethod]
        public void NameTxtChanged_TooLong_ShouldShowAppropriateError()
        {

            // Arrange
            _editVM.NameEntryText = Generators.GenRandomString(51);

            // Act
            _editVM.NameBoxTextChangedCommand.Execute();

            // Assert
            _editVM.NameErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.NameErrorText.Should().Be("Product Name limited to 50 characters.");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("-3")]
        public void PriceTxtChanged_BadPrices_ShouldShowAppropriateError(string price)
        {
            // Arrange
            _editVM.PriceEntryText = price;

            // Act
            _editVM.PriceBoxTextChangedCommand.Execute();

            // Assert
            _editVM.PriceErrorVisibility.Should().Be(Visibility.Visible);
            _editVM.PriceErrorText.Should().Be("A Product's Unit Price must be greter than 0$.");
        }

        #endregion

        [TestMethod]
        public void Submit_HappyyFlow_ShouldUpdateProduct()
        {
            // Arrange
            var oldProd = new Product
            {
                Stock = _ogProd.Stock,
                BarCode = _ogProd.BarCode,
                CategoryId = _ogProd.CategoryId,
                Description = _ogProd.Description,
                Name = _ogProd.Name,
                ProductId = _ogProd.ProductId,
                UnitPrice = _ogProd.UnitPrice
            };

            var prod = Generators.GenProduct();
            prod.ProductId = _ogProd.ProductId;

            _editVM.NameEntryText = prod.Name;
            _editVM.PriceEntryText = prod.UnitPrice.GetValueOrDefault().ToString();
            _editVM.StockEntryText = prod.Stock.ToString();
            _editVM.CodeEntryText = prod.BarCode;
            _editVM.DescEntryText = prod.Description;
            _editVM.SelectedCategory = _db.Categories.SingleOrDefault(c => c.CategoryId == prod.CategoryId);

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(_ogProd.ProductId);

            // Assert
            result.Should().BeEquivalentTo(prod);
            result.Should().NotBeEquivalentTo(oldProd);
        }

        [DataTestMethod]
        [DataRow(null, "12.15", "2", "7 25272 73070 6")]
        [DataRow("", "12.15", "2", "7 25272 73070 6")]
        [DataRow("Kendama89012345678911234567892123456789312345678941", "12.15", "2", "7 25272 73070 6")]
        [DataRow("Kendama", "0", "2", "7 25272 73070 6")]
        [DataRow("Kendama", "-3", "2", "7 25272 73070 6")]
        [DataRow("Kendama", null, "2", "7 25272 73070 6")]
        [DataRow("Kendama", "12.15", "-2", "7 25272 73070 6")]
        [DataRow("Kendama", "12.15", "2", "7 25272 73070 0")]
        [DataRow("Kendama", "12.15", "2", "7 25272 73070-6")]
        [DataRow("Kendama", "12.15", "2", "7 25272 73070a6")]
        [DataRow("Kendama", "12.15", "2", "7 25272 6")]
        [DataRow("Kendama", "12.15", "2", "7 25272 73070 612345 6")]
        public void Submit_BadData_ShouldNotUpdateProduct(string name, string price, string stock, string code)
        {
            // Arrange
            var oldProd = new Product
            {
                Stock = _ogProd.Stock,
                BarCode = _ogProd.BarCode,
                CategoryId = _ogProd.CategoryId,
                Description = _ogProd.Description,
                Name = _ogProd.Name,
                ProductId = _ogProd.ProductId,
                UnitPrice = _ogProd.UnitPrice
            };

            _editVM.NameEntryText = name;
            _editVM.PriceEntryText = price;
            _editVM.StockEntryText = stock;
            _editVM.CodeEntryText = code;
            _editVM.SelectedCategory = _db.Categories.FirstOrDefault();

            // Act
            _editVM.SubmitCommand.Execute();
            var result = _service.Get(_ogProd.ProductId);

            // Assert
            result.Should().BeEquivalentTo(oldProd);
        }
    }
}
