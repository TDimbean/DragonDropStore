using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for ProductAddWindow.xaml
    /// </summary>
    public partial class ProductAddWindow : Window, IRelayReloadAndRemoteCloseControl, IReceiveBarcodeControl
    {
        private IReloadableControl _callingTab;
        private StoreView _storeView;
        private IAwaitUnlock _callingUnlockable;

        public ProductAddWindow(IReloadableControl callingTab = null, StoreView storeView = null, string barCode = null,
            IAwaitUnlock callingUnlockable=null)
        {
            if (storeView != null) storeView.Lock();
            _storeView = storeView;
            _callingTab = callingTab;
            _callingUnlockable = callingUnlockable;

            InitializeComponent();

            var container = new UnityContainer();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IProductDataService, ProductDataService>();
            container.RegisterType<ICategoryRepository, CategoryRepository>();
            container.RegisterInstance(this as IRelayReloadAndRemoteCloseControl);

            DataContext = container.Resolve<ProductAddViewModel>();

            if(barCode!=null)(DataContext as ProductAddViewModel).CodeEntryText = barCode;
            NameBox.Focus();
            NameBox.Text = "Name";
        }

        #region Handle Input

        private void CategoryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
                => ControlHelpers.PopComboBoxPlaceholder(e.Source as ComboBox);

        private void PriceBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9,.]");

        private void StockBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");

        private void CodeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9]");
        
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void Combo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            (sender as ComboBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        #endregion

        public void RefreshParent()
        {
            if(_callingTab!=null) _callingTab.Reload();
        }

        public void Stop() => Close();

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_storeView != null) _storeView.Unlock();
            if (_callingUnlockable != null) _callingUnlockable.Unlock();
            base.OnClosing(e);
        }

        private void Scanner_Click(object sender, RoutedEventArgs e)
            => new SingleScanWindow(this as IReceiveBarcodeControl).Show();

        public void SetBarcode(string code)
        => (DataContext as ProductAddViewModel).CodeEntryText = code;
    }
}