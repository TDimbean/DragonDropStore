using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
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
    /// Interaction logic for CustomerAddWindow.xaml
    /// </summary>
    public partial class CustomerAddWindow : Window, IRelayReloadAndRemoteCloseControl
    {
        IReloadableControl _callingTab;
        StoreView _storeView;

        public CustomerAddWindow(IReloadableControl callingTab, StoreView storeView = null)
        {
            if (storeView != null) storeView.Lock();

            _callingTab = callingTab;
            _storeView = storeView;

            InitializeComponent();

            var container = new UnityContainer();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ICustomerDataService, CustomerDataService>();
            container.RegisterInstance(this as IRelayReloadAndRemoteCloseControl);
            container.RegisterInstance(storeView);

            DataContext = container.Resolve<CustomerAddViewModel>();

            NameBox.Focus();
            NameBox.Text = "Name";
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public void RefreshParent() => _callingTab.Reload();

        public void Stop() => Close();

        protected override void OnClosing(CancelEventArgs e)
        {
            if(_storeView!=null) _storeView.Unlock();
            base.OnClosing(e);
        }
    }
}
