using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.Infrastructure;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs;
using DragonDrop.WPF.StoreViews.Tabs.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using Unity;

namespace DragonDrop.WPF.StoreViews.ViewModels
{
    public class StoreViewModel : BindableBase
    {
        private ObservableCollection<Tab> _tabs;
        private ObservableCollection<UserOrderProd> _cart;
        private StoreView _storeView;
        private UnityContainer _container;
        private IProductDataService _prodService;

        public StoreViewModel
            (StoreView storeView, IProductDataService prodService, bool isAdmin = false, Customer cust = null)
        {
            _storeView = storeView;
            _prodService = prodService;

            #region DI Container

            var container = new UnityContainer();

            container.RegisterType<IPaymentRepository, PaymentRepository>();
            container.RegisterType<IPaymentDataService, PaymentDataService>();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ICustomerDataService, CustomerDataService>();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IOrderDataService, OrderDataService>();
            container.RegisterType<IOrderItemRepository, OrderItemRepository>();
            container.RegisterType<IOrderItemDataService, OrderItemDataService>();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IProductDataService, ProductDataService>();
            if (cust != null)
            {
                container.RegisterInstance(cust);
                container.RegisterInstance(cust.CustomerId);
            }
            container.RegisterInstance(_storeView);
            container.RegisterInstance(this);

            _container = container;

            #endregion

            #region Init Tabs

            _tabs = isAdmin ?
                new ObservableCollection<Tab>
                {
                    new Tab ("Menu", "dragon", container.Resolve<AdminMenuTab>()),
                    new Tab ("Customers", "customers", container.Resolve<AdminCustomersTab>()),
                    new Tab ("Orders", "orders", container.Resolve<AdminOrdersTab>()),
                    new Tab ("Items", "orderItem", container.Resolve<AdminItemsTab>()),
                    new Tab ("Products", "products", container.Resolve<AdminProductsTab>()),
                    new Tab ("Payments", "payments", container.Resolve<AdminPaymentsTab>())
                } :
                new ObservableCollection<Tab>
            {
                new Tab("Menu", "dragon", container.Resolve<UserMenuTab>()),
                new Tab("Store", "store", container.Resolve<UserStoreTab>()),
                new Tab("Orders", "orders", container.Resolve<UserOrdersTab>()),
                new Tab("Payments", "payments",container.Resolve<UserPaymentsTab>()),
                new Tab("Cart", "cart", container.Resolve<UserCartTab>()),
                new Tab("Account", "account", container.Resolve<UserAccountTab>())
            };

            #endregion

            if (cust!=null) UserId = cust.CustomerId;
            Cart = new ObservableCollection<UserOrderProd>();

            #region Commands

            AddToCartCommand = new DelegateCommand<int?>(AddToCartCommandExecute);
            RemoveFromCartCommand = new DelegateCommand<int?>(RemoveFromCartCommandExecute);
            BumpInCartCommand = new DelegateCommand<int?>(BumpInCartCommandExecute);
            DecreaseInCartCommand = new DelegateCommand<int?>(DecreaseInCartCommandExecute);
            ChangeQtyInCartCommand = 
                new DelegateCommand<(int prodId, int newQty)?>(ChangeQtyInCartCommandExecute);
            PurgeCommand = new DelegateCommand(PurgeCommandExecute);
            ClearCartCommand = new DelegateCommand(ClearCartCommandExecute);
            LockStoreCommand = new DelegateCommand(LockStoreCommandExecute);
            UnlockStoreCommand = new DelegateCommand(UnlockStoreCommandExecute);

            #endregion
        }

        #region Properties

        public int UserId { get; }

        public ObservableCollection<Tab> Tabs
        {
            get => _tabs;
            set { }
        }

        public ObservableCollection<UserOrderProd> Cart
        {
            get => _cart;
            set => SetProperty(ref _cart, value);
        }

        public DelegateCommand<int?> AddToCartCommand { get; }
        public DelegateCommand<int?> RemoveFromCartCommand { get; }
        public DelegateCommand<int?> BumpInCartCommand { get; }
        public DelegateCommand<int?> DecreaseInCartCommand { get; }
        public DelegateCommand<(int prodId,int newQty)?> ChangeQtyInCartCommand { get; }
        public DelegateCommand PurgeCommand { get; }
        public DelegateCommand ClearCartCommand { get; }
        public DelegateCommand LockStoreCommand { get; }
        public DelegateCommand UnlockStoreCommand { get; }

        #endregion

        #region Private Methods

        //TODO Make all this Spaghetti readable

        private void AddToCartCommandExecute(int? prodId)
        {
            var prod = _prodService.Get(prodId.GetValueOrDefault());
            if (prod==null)
            {
                StaticLogger.LogError(GetType(), "Tried to add Product to cart but none found with ID: " + prodId);
                _storeView.Alert("Tried to add Product to Cart, but could not find it. Please try refreshing the store.");
                return;
            }

            if (prod.Stock<1)
            {
                _storeView.Alert("Sadly, this item is currently out of stock.");
                return;
            }

            var cartProd = Cart.SingleOrDefault(p => p.ProductId == prodId);
            if (cartProd == null)
                Cart.Add(new UserOrderProd
                {
                    Name = prod.Name,
                    ProductId = prod.ProductId,
                    Price = prod.UnitPrice.GetValueOrDefault(),
                    Quantity = 1
                });
            else
            {
                Cart.SingleOrDefault(p => p.ProductId == prodId).Quantity += 1;
            }
        }

        private void RemoveFromCartCommandExecute(int? prodId)
        {
            var cartProd = Cart.SingleOrDefault(p => p.ProductId == prodId);
            if (cartProd == null)
            {
                StaticLogger.LogError(GetType(), "Tried to remove Product from cart but none found with ID: " + prodId);
                _storeView.Alert("Tried to remove Product from Cart, but could not find it. Please try refreshing the store.");
                return;
            }
            else Cart.Remove(cartProd);
        }

        private void BumpInCartCommandExecute(int? prodId)
        {
            var prod = _prodService.Get(prodId.GetValueOrDefault());
            if (prod == null)
            {
                StaticLogger.LogError(GetType(), "Tried to add Product to cart but none found with ID: " + prodId);
                _storeView.Alert("Tried to add Product to Cart, but could not find it. Please try refreshing the store.");
                return;
            }

            var cartProd = Cart.SingleOrDefault(p => p.ProductId == prodId);
            if (cartProd == null)
            {
                if (prod.Stock > 0)
                {
                    var newProd = new UserOrderProd
                    {
                        Name = prod.Name,
                        ProductId = prod.ProductId,
                        Price = prod.UnitPrice.GetValueOrDefault(),
                        Quantity = 1
                    };
                    Cart.Add(newProd);
                    cartProd = newProd;
                }
                else
                {
                    _storeView.Alert("Sadly, this item is currently out of stock.");
                    return;
                }
            }
            else
            {
                if (prod.Stock > cartProd.Quantity)
                    Cart.SingleOrDefault(p => p.ProductId == prodId).Quantity += 1;
                else
                {
                    _storeView.Alert("Sadly, this is all there is of your selected item. If you require more, please contact Support" +
                    " about an advanced order or check again at a later date.");
                    return;
                }
            }
        }

        private void DecreaseInCartCommandExecute(int? prodId)
        {
            var cartProd = Cart.SingleOrDefault(p => p.ProductId == prodId);
            if (cartProd == null)
            {
                _storeView.Alert("Tried to remove Product from Cart but couldn't find it. Please try refreshing the view.");
                return;
            }
            else cartProd.Quantity -= cartProd.Quantity > 0 ? 1 : 0;
        }

        private void ChangeQtyInCartCommandExecute((int prodId, int newQty)? vals)
        {
            var prodId = vals.GetValueOrDefault().prodId;

            var prod = _prodService.Get(prodId);
            if (prod == null)
            {
                StaticLogger.LogError(GetType(), "Tried to modify Product quantity but none found in Repo with ID: " + prodId);
                _storeView.Alert("Tried to modify Product quantity, but could not find it. Please try refreshing the store.");
                return;
            }
               
            var cartProd = Cart.SingleOrDefault(p => p.ProductId == prodId);
            if (cartProd == null)
            {
                StaticLogger.LogError(GetType(), "Tried to modify Product quantity but none in Cart found with ID: " + prodId);
                _storeView.Alert("This product is no longer in your cart. Please try refreshing the store.");
                return;
            }
            else
            {
                if (vals.GetValueOrDefault().newQty > prod.Stock)
                    cartProd.Quantity = prod.Stock;
                else cartProd.Quantity = vals.GetValueOrDefault().newQty;
            }
        }

        private void PurgeCommandExecute()
            => Cart = new ObservableCollection<UserOrderProd>(Cart.Where(p => p.Quantity > 0));

        private void ClearCartCommandExecute() => Cart = new ObservableCollection<UserOrderProd>();

        private void LockStoreCommandExecute() => _storeView.Lock();
        private void UnlockStoreCommandExecute() => _storeView.Unlock();

        #endregion

        public void ResetTabs()
        {
            foreach (var tab in Tabs)
            {
                var resetable = tab.TabContent as IReloadableControl;
                if (resetable != null) resetable.Reload();
            }
        }

    }
}
