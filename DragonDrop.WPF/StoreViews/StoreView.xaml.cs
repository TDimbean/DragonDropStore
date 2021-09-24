using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using Unity;

namespace DragonDrop.WPF.StoreViews
{
    /// <summary>
    /// Interaction logic for StoreView.xaml
    /// </summary>
    public partial class StoreView : Window
    {
        // When true, closing this window won't shut down the app. 
        // Switching FROM this window to another should be done by: calling that window, setting own isSwitching to true, closing self
        private bool _isSwitching;
        // When, clsing for good, Shutdown isSwitching is false, so shutdown closes all orphaned windows;

        public StoreView(bool isAdmin=false, Customer cust=null)
        {
            var container = new UnityContainer();
            container.RegisterType<IProductRepository, ProductRepository>();
            var prodServ = container.Resolve<ProductDataService>();

            InitializeComponent();
            var vm = new StoreViewModel(this, prodServ, isAdmin, cust);
            DataContext = vm;

            // It's the Width of TabHeaders * their number + either 20(if it's over-all padding) or 4* number of tabs (if it's local padding)
            // You get the tab header Width from the StackPanel width
            Width = vm.Tabs.Count * 96 + 20;
        }

        public void SwitchToCart()
            => tabCtrl.SelectedIndex = 4;

        public void RefreshAdminOrders()
            => ((tabCtrl.Items[2] as Tab).TabContent as IReloadableControl).Reload();

        public void RefreshAdminProducts()
            => ((tabCtrl.Items[4] as Tab).TabContent as IReloadableControl).Reload();

        public void Lock()=>IsEnabled = false;
        public void Unlock()=>IsEnabled = true;

        public void SwitchToMain()
        {
            var container = new UnityContainer();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ICustomerDataService, CustomerDataService>();

            container.Resolve<MainWindow>().Show();
            _isSwitching = true;
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_isSwitching) Application.Current.Shutdown();
            Environment.Exit(0);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!_isSwitching) Application.Current.Shutdown();
            Environment.Exit(0);
        }

        public void Alert(string msg)
            => MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
