using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for CustomerEditWindow.xaml
    /// </summary>
    public partial class CustomerEditWindow : Window, IRelayReloadAndRemoteCloseControl
    {
        private IReloadableControl _callingTab;
        private StoreView _storeView;

        public CustomerEditWindow(int custId, IReloadableControl callingTab = null, StoreView storeView = null)
        {
            if (storeView != null) storeView.Lock();
            _storeView = storeView;
            _callingTab = callingTab;

            InitializeComponent();

            var container = new UnityContainer();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ICustomerDataService, CustomerDataService>();
            container.RegisterInstance(custId);
            container.RegisterInstance(this as IRelayReloadAndRemoteCloseControl);
            container.RegisterInstance(storeView);

            DataContext = container.Resolve<CustomerEditViewModel>();

            NameBox.Focus();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public void RefreshParent()
        {
            if (_callingTab != null) _callingTab.Reload();
        }

        public void Stop() => Close();

        protected override void OnClosed(EventArgs e)
        {
            if (_storeView != null) _storeView.Unlock();
            base.OnClosed(e);
        }
    }
}
