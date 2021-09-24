using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
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

    public class ProductAddVMTests
    {
        private TestDb _db;
        private IProductRepository _repo;
        private ICategoryRepository _catRepo;
        private IProductDataService _service;

        private ProductAddViewModel _addVM;

        [TestInitialize]
        public void Init()
        {
            _db = new TestDb();
            _repo = new TestProductRepo(_db);
            _catRepo = new TestCategoryRepo(_db);
            _service = new ProductDataService(_repo);


            _addVM = new ProductAddViewModel(_service, _catRepo);
        }

        #region Focus Changes

        #region Name

        [TestMethod]
        public void NameLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.NameEntryText = string.Empty;

            // Act
            _addVM.NameBoxLostFocusCommand.Execute();

            // Assert
            _addVM.NameEntryText.Should().Be("Name");
        }

        [TestMethod]
        public void NameLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.NameEntryText = text;

            // Act
            _addVM.NameBoxLostFocusCommand.Execute();

            // Assert
            _addVM.NameEntryText.Should().Be(text);
        }

        [TestMethod]
        public void NameGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.NameEntryText = "Name";

            // Act
            _addVM.NameBoxGotFocusCommand.Execute();

            // Assert
            _addVM.NameEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void NameGotFocus_HadText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.NameEntryText = text;

            // Act
            _addVM.NameBoxGotFocusCommand.Execute();

            // Assert
            _addVM.NameEntryText.Should().Be(text);
        }

        #endregion

        #region Price

        [TestMethod]
        public void PriceLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.PriceEntryText = string.Empty;

            // Act
            _addVM.PriceBoxLostFocusCommand.Execute();

            // Assert
            _addVM.PriceEntryText.Should().Be("Price");
        }

        [TestMethod]
        public void PriceLostFocus_ContainsGibberish_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.PriceEntryText = text;

            // Act
            _addVM.PriceBoxLostFocusCommand.Execute();

            // Assert
            _addVM.PriceEntryText.Should().Be(text);
        }

        [TestMethod]
        public void PriceLostFocus_ContainsAmount_ShouldFormatText()
        {
            // Arrange
            var text = "12.99";
            _addVM.PriceEntryText = text;

            // Act
            _addVM.PriceBoxLostFocusCommand.Execute();

            // Assert
            _addVM.PriceEntryText.Should().Be(string.Format("{0:C}", decimal.Parse(text)));
        }

        [TestMethod]
        public void PriceGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.PriceEntryText = "Price";

            // Act
            _addVM.PriceBoxGotFocusCommand.Execute();

            // Assert
            _addVM.PriceEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void PriceGotFocus_HadText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.PriceEntryText = text;

            // Act
            _addVM.PriceBoxGotFocusCommand.Execute();

            // Assert
            _addVM.PriceEntryText.Should().Be(text);
        }

        #endregion

        #region Stock

        [TestMethod]
        public void StockLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.StockEntryText = string.Empty;

            // Act
            _addVM.StockBoxLostFocusCommand.Execute();

            // Assert
            _addVM.StockEntryText.Should().Be("Stock");
        }

        [TestMethod]
        public void StockLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.StockEntryText = text;

            // Act
            _addVM.StockBoxLostFocusCommand.Execute();

            // Assert
            _addVM.StockEntryText.Should().Be(text);
        }

        [TestMethod]
        public void StockGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.StockEntryText = "Stock";

            // Act
            _addVM.StockBoxGotFocusCommand.Execute();

            // Assert
            _addVM.StockEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void StockGotFocus_HadText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.StockEntryText = text;

            // Act
            _addVM.StockBoxGotFocusCommand.Execute();

            // Assert
            _addVM.StockEntryText.Should().Be(text);
        }

        #endregion

        #region BarCode

        [TestMethod]
        public void CodeLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.CodeEntryText = string.Empty;

            // Act
            _addVM.CodeBoxLostFocusCommand.Execute();

            // Assert
            _addVM.CodeEntryText.Should().Be("Barcode");
        }

        [TestMethod]
        public void CodeLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.CodeEntryText = text;

            // Act
            _addVM.CodeBoxLostFocusCommand.Execute();

            // Assert
            _addVM.CodeEntryText.Should().Be(text);
        }

        [TestMethod]
        public void CodeGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.CodeEntryText = "Barcode";

            // Act
            _addVM.CodeBoxGotFocusCommand.Execute();

            // Assert
            _addVM.CodeEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void CodeGotFocus_HadText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.CodeEntryText = text;

            // Act
            _addVM.CodeBoxGotFocusCommand.Execute();

            // Assert
            _addVM.CodeEntryText.Should().Be(text);
        }
       
        #endregion

        #region Description

        [TestMethod]
        public void DescriptionLostFocus_EmptyBox_ShouldInsertPlaceholder()
        {
            // Arrange
            _addVM.DescEntryText = string.Empty;

            // Act
            _addVM.DescBoxLostFocusCommand.Execute();

            // Assert
            _addVM.DescEntryText.Should().Be("Description");
        }

        [TestMethod]
        public void DescriptionLostFocus_ContainsText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.DescEntryText = text;

            // Act
            _addVM.DescBoxLostFocusCommand.Execute();

            // Assert
            _addVM.DescEntryText.Should().Be(text);
        }

        [TestMethod]
        public void DescriptionGotFocus_Placeholder_ShouldEmptySelf()
        {
            // Arrange
            _addVM.DescEntryText = "Description";

            // Act
            _addVM.DescBoxGotFocusCommand.Execute();

            // Assert
            _addVM.DescEntryText.Should().Be(string.Empty);
        }

        [TestMethod]
        public void DescriptionGotFocus_HadText_ShouldKeepText()
        {
            // Arrange
            var text = "a";
            _addVM.DescEntryText = text;

            // Act
            _addVM.DescBoxGotFocusCommand.Execute();

            // Assert
            _addVM.DescEntryText.Should().Be(text);
        }

        #endregion
        
        #endregion

        #region Text Updates

        [TestMethod]
        public void NameTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.NameEntryText = _db.Products.FirstOrDefault().Name;

            // Act
            _addVM.NameBoxTextChangedCommand.Execute();

            // Assert
            _addVM.NameErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void PriceTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.PriceEntryText = _db.Products.FirstOrDefault().UnitPrice.GetValueOrDefault().ToString();

            // Act
            _addVM.PriceBoxTextChangedCommand.Execute();

            // Assert
            _addVM.PriceErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void StockTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.StockEntryText = _db.Products.FirstOrDefault().Stock.ToString();

            // Act
            _addVM.StockBoxTextChangedCommand.Execute();

            // Assert
            _addVM.StockErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void DescTxtChanged_Valid_ErrorShouldBeCollapsed()
        {
            // Arrange
            _addVM.DescEntryText = "a";

            // Act
            _addVM.DescBoxTextChangedCommand.Execute();

            // Assert
            _addVM.DescErrorVisibility.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void DescTxtChanged_NormalLength_ShouldUpdateCounter()
        {
            // Arrange
            _addVM.DescEntryText = "a";

            // Act
            _addVM.DescBoxTextChangedCommand.Execute();

            // Assert
            _addVM.RemainingDesc.Should().Be(_addVM.DescEntryText.Length);
            _addVM.DescCounterFg.Should().Be(Brushes.LightGray);
        }

        [TestMethod]
        public void DescTxtChanged_TextLimit_ShouldTrimAndReddenCounter()
        {
            // Arrange
            var text = Generators.GenRandomString(361);
            _addVM.DescEntryText = text;

            // Act
            _addVM.DescBoxTextChangedCommand.Execute();

            // Assert
            _addVM.DescEntryText.Should().Be(text.Substring(0, 360));
            _addVM.RemainingDesc.Should().Be(0);
            _addVM.DescCounterFg.Should().Be(Brushes.LightGray);
        }

        //Bad Names

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void NameTxtChanged_Empty_ShouldShowAppropriateError(string name)
        {
            // Arrange
            _addVM.NameEntryText = name;

            // Act
            _addVM.NameBoxTextChangedCommand.Execute();

            // Assert
            _addVM.NameErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.NameErrorText.Should().Be("Products require a name.");
        }

        [TestMethod]
        public void NameTxtChanged_TooLong_ShouldShowAppropriateError()
        {

            // Arrange
            _addVM.NameEntryText = Generators.GenRandomString(51);

            // Act
            _addVM.NameBoxTextChangedCommand.Execute();

            // Assert
            _addVM.NameErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.NameErrorText.Should().Be("Product Name limited to 50 characters.");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("-3")]
        public void PriceTxtChanged_BadPrices_ShouldShowAppropriateError(string price)
        {
            // Arrange
            _addVM.PriceEntryText = price;

            // Act
            _addVM.PriceBoxTextChangedCommand.Execute();

            // Assert
            _addVM.PriceErrorVisibility.Should().Be(Visibility.Visible);
            _addVM.PriceErrorText.Should().Be("A Product's Unit Price must be greter than 0$.");
        }

        #endregion

        [TestMethod]
        public void Submit_HappyyFlow_ShouldRegisterProduct()
        {
            // Arrange
            var prod = Generators.GenProduct();

            _addVM.NameEntryText = prod.Name;
            _addVM.PriceEntryText = prod.UnitPrice.GetValueOrDefault().ToString();
            _addVM.StockEntryText = prod.Stock.ToString();
            _addVM.CodeEntryText = prod.BarCode;
            _addVM.DescEntryText = prod.Description;
            _addVM.SelectedCategory = _db.Categories.FirstOrDefault();

            var initCount = _service.GetAll().Count();

            // Act
            _addVM.SubmitCommand.Execute();
            var result = _service.GetAllFiltered(prod.BarCode).SingleOrDefault();
            var newCount = _service.GetAll().Count();

            // Assert
            result.Should().BeEquivalentTo(prod);
            newCount.Should().BeGreaterThan(initCount);
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
        public void Submit_BadData_ShouldNotRegisterProduct(string name, string price, string stock, string code)
        {
            // Arrange
            _addVM.NameEntryText = name;
            _addVM.PriceEntryText = price;
            _addVM.StockEntryText = stock;
            _addVM.CodeEntryText = code;
            _addVM.SelectedCategory = _db.Categories.FirstOrDefault();

            var initCount = _service.GetAll().Count();

            // Act
            _addVM.SubmitCommand.Execute();
            var result = _service.Get(0);
            var newCount = _service.GetAll().Count();

            // Assert
            result.Should().BeNull();
            newCount.Should().Be(initCount);
        }
    }
}
