using DragonDrop.BLL.DataServices;
using DragonDrop.UnitTests.Helpers;
using DragonDrop.UnitTests.MockRepoes;
using DragonDrop.WPF.StoreViews;
using DragonDrop.WPF.StoreViews.Tabs.Models;
using DragonDrop.WPF.StoreViews.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows;

namespace DragonDrop.UnitTests.WPF
{
    [TestClass]
    public class StoreVMTests
    {
        private TestDb _db;
        private TestCustomerRepo _custRepo;
        //private TestOrderRepo _ordRepo;
        //private TestOrderItemRepo _itemRepo;
        private TestProductRepo _prodRepo;
        private CustomerDataService _custService;
        private ProductDataService _prodService;
        //private OrderDataService _ordService;
        //private OrderItemDataService _itemService;
        private StoreView _storeView;
        private StoreViewModel _storeVM;
        private PrivateObject _vm;

        [TestInitialize]
        public void Init()
        {
            if (Application.ResourceAssembly == null)
                Application.ResourceAssembly = typeof(StoreView).Assembly;

            _db = new TestDb();
            _custRepo = new TestCustomerRepo(_db);
            //_ordRepo = new TestOrderRepo(_db);
            //_itemRepo = new TestOrderItemRepo(_db);
            _prodRepo = new TestProductRepo(_db);
            _custService = new CustomerDataService(_custRepo);
            //_ordService = new OrderDataService(_ordRepo);
            //_itemService = new OrderItemDataService(_itemRepo);
            _prodService = new ProductDataService(_prodRepo);

            var cust = _db.Customers.FirstOrDefault();

            _storeView = new StoreView(false, cust);

            _storeVM = new StoreViewModel(_storeView, _prodService, false, cust);
            _vm = new PrivateObject(typeof(StoreViewModel), _storeView, _prodService, false, cust);
        }

        [TestMethod]
        public void AddToCart_New_ShouldAddNewCartItem()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            var expected = new UserOrderProd
            {
                Name = prod.Name,
                Price = prod.UnitPrice.GetValueOrDefault(),
                ProductId = prod.ProductId,
                Quantity = 1
            };

            // Act
            _storeVM.AddToCartCommand.Execute(prod.ProductId);

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void AddToCart_AlreadyInCart_ShouldBumpExistingCartItem()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            var expected = new UserOrderProd
            {
                Name = prod.Name,
                Price = prod.UnitPrice.GetValueOrDefault(),
                ProductId = prod.ProductId,
                Quantity = 2
            };
            _storeVM.AddToCartCommand.Execute(prod.ProductId);

            // Act
            _storeVM.AddToCartCommand.Execute(prod.ProductId);

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void AddToCart_InexistentProduct_ShouldNotAdd()
        {
            _storeVM.AddToCartCommand.Execute(Generators.GenInexistentProductId());

            // Assert
            _storeVM.Cart.Should().BeEmpty();
        }

        [TestMethod]
        public void AddToCart_OutOfStock_ShouldAddNewCartItem()
        {
            // Arrange
            var prod = Generators.GenProduct();
            prod.Stock = 0;
            _prodService.Create(prod);
            var storeVM = new StoreViewModel(_storeView, _prodService, false, _db.Customers.FirstOrDefault());

            // Act
            storeVM.AddToCartCommand.Execute(prod.ProductId);

            // Assert
            storeVM.Cart.Should().BeEmpty();
        }

        [TestMethod]
        public void RemoveFromCart_HappyFlow_ShouldRemove()
        {
            // Arrange
            var prodId = _db.Products.FirstOrDefault().ProductId;
            _storeVM.AddToCartCommand.Execute(prodId);

            // Act
            _storeVM.RemoveFromCartCommand.Execute(prodId);

            // Assert
            _storeVM.Cart.Should().BeEmpty();
        }

        [TestMethod]
        public void RemoveFromCart_ProdDoesntExist_ShouldLeaveCartUnchanged()
        {
            // Arrange
            var prodId = _db.Products.FirstOrDefault().ProductId;
            var prodId2 = _db.Products.Skip(1).FirstOrDefault().ProductId;
            _storeVM.AddToCartCommand.Execute(prodId2);

            // Act
            _storeVM.RemoveFromCartCommand.Execute(prodId);

            // Assert
            _storeVM.Cart.SingleOrDefault().ProductId.Should().Be(prodId2);
        }

        [TestMethod]
        public void BumpInCart_HappyFlow_ShouldIncreaseByOne()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            var expected = new UserOrderProd
            {
                ProductId = prod.ProductId,
                Name = prod.Name,
                Price = prod.UnitPrice.GetValueOrDefault(),
                Quantity = 2
            };
            _storeVM.AddToCartCommand.Execute(prod.ProductId);

            // Act
            _storeVM.BumpInCartCommand.Execute(prod.ProductId);

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void BumpInCart_ProductNotInCart_ShouldAddToCart()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            var expected = new UserOrderProd
            {
                ProductId = prod.ProductId,
                Name = prod.Name,
                Price = prod.UnitPrice.GetValueOrDefault(),
                Quantity = 1
            };

            // Act
            _storeVM.BumpInCartCommand.Execute(prod.ProductId);

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void BumpInCart_ProductDoesntExist_ShouldLeaveCartUntouched()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            var expected = new UserOrderProd
            {
                ProductId = prod.ProductId,
                Name = prod.Name,
                Price = prod.UnitPrice.GetValueOrDefault(),
                Quantity = 1
            };
            _storeVM.AddToCartCommand.Execute(prod.ProductId);

            // Act
            _storeVM.BumpInCartCommand.Execute(Generators.GenInexistentProductId());

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void BumpInCart_ProductOutOfStock_ShouldNotBump()
        {
            // Arrange;
            var prod = _db.Products.FirstOrDefault();
            _storeVM.AddToCartCommand.Execute(prod.ProductId);
            _storeVM.ChangeQtyInCartCommand.Execute((prod.ProductId, prod.Stock));
            var expected = new UserOrderProd
            {
                Name = prod.Name,
                ProductId = prod.ProductId,
                Price = prod.UnitPrice.GetValueOrDefault(),
                Quantity = prod.Stock
            };


            // Act
            _storeVM.BumpInCartCommand.Execute(prod.ProductId);

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void DescreaseInCart_HappyFlow_ShouldDecreaseBy1()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            _storeVM.AddToCartCommand.Execute(prod.ProductId);
            _storeVM.ChangeQtyInCartCommand.Execute((prod.ProductId, 2));
            var expected = new UserOrderProd
            {
                Name = prod.Name,
                Price = prod.UnitPrice.GetValueOrDefault(),
                ProductId = prod.ProductId,
                Quantity = 1
            };

            // Act
            _storeVM.DecreaseInCartCommand.Execute(prod.ProductId);

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void DescreaseInCart_ProductNotInCart_ShouldLeaveUnchanged()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            var prod2Id = _db.Products.Skip(1).FirstOrDefault().ProductId;
            _storeVM.AddToCartCommand.Execute(prod.ProductId);
            _storeVM.ChangeQtyInCartCommand.Execute((prod.ProductId, 2));
            var expected = new UserOrderProd
            {
                Name = prod.Name,
                Price = prod.UnitPrice.GetValueOrDefault(),
                ProductId = prod.ProductId,
                Quantity = 2
            };

            // Act
            _storeVM.DecreaseInCartCommand.Execute(prod2Id);

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void DescreaseInCart_Already0_ShouldLeaveUnchanged()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            _storeVM.AddToCartCommand.Execute(prod.ProductId);
            _storeVM.ChangeQtyInCartCommand.Execute((prod.ProductId, 0));
            var expected = new UserOrderProd
            {
                Name = prod.Name,
                Price = prod.UnitPrice.GetValueOrDefault(),
                ProductId = prod.ProductId,
                Quantity = 0
            };

            // Act
            _storeVM.DecreaseInCartCommand.Execute(prod.ProductId);

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ChangeQtyInCart_HappyFlow_ShouldChangeToDesired()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            _storeVM.AddToCartCommand.Execute(prod.ProductId);
            var expected = new UserOrderProd
            {
                Name = prod.Name,
                Price = prod.UnitPrice.GetValueOrDefault(),
                ProductId = prod.ProductId,
                Quantity = 2
            };

            // Act
            _storeVM.ChangeQtyInCartCommand.Execute((prod.ProductId, 2));

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ChangeQtyInCart_InexistentProduct_ShouldLeaveUnchanged()
        {
            // Arrange
            var prod2Id = _db.Products.Skip(1).FirstOrDefault().ProductId;
            var prod = _db.Products.FirstOrDefault();
            _storeVM.AddToCartCommand.Execute(prod.ProductId);
            var expected = new UserOrderProd
            {
                Name = prod.Name,
                Price = prod.UnitPrice.GetValueOrDefault(),
                ProductId = prod.ProductId,
                Quantity = 1
            };

            // Act
            _storeVM.ChangeQtyInCartCommand.Execute((prod2Id, 2));

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ChangeQtyInCart_OverStock_ShouldChangeToStockLimit()
        {
            // Arrange
            var prod = _db.Products.FirstOrDefault();
            _storeVM.AddToCartCommand.Execute(prod.ProductId);
            var expected = new UserOrderProd
            {
                Name = prod.Name,
                Price = prod.UnitPrice.GetValueOrDefault(),
                ProductId = prod.ProductId,
                Quantity = prod.Stock
            };

            // Act
            _storeVM.ChangeQtyInCartCommand.Execute((prod.ProductId, prod.Stock+10));

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Purge_HappyFlow_ShouldOnlyErase0QtyItems()
        {
            // Arrange
            var prodObj = _db.Products.FirstOrDefault();
            var prod1 = prodObj.ProductId;
            var prod2 = _db.Products.Skip(1).FirstOrDefault().ProductId;
            _storeVM.AddToCartCommand.Execute(prod1);
            _storeVM.AddToCartCommand.Execute(prod2);
            _storeVM.DecreaseInCartCommand.Execute(prod2);

            var expected = new UserOrderProd
            {
                Name = prodObj.Name,
                Price= prodObj.UnitPrice.GetValueOrDefault(),
                ProductId=prodObj.ProductId,
                Quantity=1
            };

            // Act
            _storeVM.PurgeCommand.Execute();

            // Assert
            _storeVM.Cart.SingleOrDefault().Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ClearCart_HappyFlow_ShouldRemoveAll()
        {
            // Arrange
            var prod1 = _db.Products.SingleOrDefault().ProductId;
            var prod2 = _db.Products.Skip(1).FirstOrDefault().ProductId;
            _storeVM.AddToCartCommand.Execute(prod1);
            _storeVM.AddToCartCommand.Execute(prod2);
            _storeVM.DecreaseInCartCommand.Execute(prod2);

            // Act
            _storeVM.ClearCartCommand.Execute();

            // Assert
            _storeVM.Cart.Should().BeEmpty();
        }
    }
}
