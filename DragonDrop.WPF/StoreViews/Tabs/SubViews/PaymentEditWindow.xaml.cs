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
    /// Interaction logic for PaymentEditWindow.xaml
    /// </summary>
    public partial class PaymentEditWindow : Window, IRelayReloadAndRemoteCloseControl
    {
        private IReloadableControl _callingTab;
        private StoreView _storeView;

        public PaymentEditWindow(int targetId, IReloadableControl callingTab=null, StoreView storeView=null)
        {
            if (storeView != null) storeView.Lock();
            _storeView = storeView;
            _callingTab = callingTab;
            InitializeComponent();

            var container = new UnityContainer();
            container.RegisterType<IPaymentRepository, PaymentRepository>();
            container.RegisterType<IPaymentDataService, PaymentDataService>();
            container.RegisterType<IPaymentMethodRepository, PaymentMethodRepository>();
            container.RegisterInstance(targetId);
            container.RegisterInstance(this as IRelayReloadAndRemoteCloseControl);

            DataContext = container.Resolve<PaymentEditViewModel>();

            CustBox.Focus();
        }

        private void MethodCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
               => ControlHelpers.PopComboBoxPlaceholder(e.Source as ComboBox);

        private void AmountBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => ControlHelpers.LimitText(e, "[^0-9,.]");

        private void CustBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        public void RefreshParent()
        {
            if (_callingTab != null) _callingTab.Reload();
        }

        public void Stop() => Close();

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_storeView != null) _storeView.Unlock();
            base.OnClosing(e);
        }
    }
}
