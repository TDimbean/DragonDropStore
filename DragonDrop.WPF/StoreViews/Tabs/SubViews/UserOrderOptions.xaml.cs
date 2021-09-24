using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.Models;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for UserOrderOptions.xaml
    /// </summary>
    public partial class UserOrderOptions : Window
    {
        private bool _areProdsExpanded = true;
        private int _ordId;

        public UserOrderOptions(Order ord)
        {
            InitializeComponent();

            _ordId = ord.OrderId;

            var container = new UnityContainer();
            container.RegisterType<IOrderItemRepository, OrderItemRepository>();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<IOrderItemDataService, OrderItemDataService>();
            container.RegisterType<IProductDataService, ProductDataService>();
            container.RegisterInstance(ord);

            DataContext = container.Resolve<UserOrderOptionsViewModel>();
        }

        private void ReqMod_Click(object sender, RoutedEventArgs e)
            => new OrderModificationWindow(_ordId).Show();

        private void ShowProd_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;

            _areProdsExpanded = !_areProdsExpanded;
            if (!_areProdsExpanded)
            {
                btn.ToolTip = "Show Products";
                custDG.Visibility = Visibility.Collapsed;
                ProdsArrow.RenderTransform = new ScaleTransform(1, 1);
                return;
            }
                btn.ToolTip = "Hide Products";
            custDG.Visibility = Visibility.Visible;
            ProdsArrow.RenderTransform = new ScaleTransform(1, -1);
        }

        private void RepIssue_Click(object sender, RoutedEventArgs e)
        {
            // Call Customer Support Notifier. The call will return true or false to indicate if the message was received
            bool msgReceived = true;
            if (!msgReceived)
            {

                MessageBox.Show(
                    "Something went wrong. Please try again.", "Support request Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBox.Show(
                "A Customer Support Agent has been notified and will contact you shortly." +
                "If your contact information has channged, please make sure to update it in your Account Settings, then" +
                " send this request again.", "Support request registered", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GridProdInfo_Click(object sender, RoutedEventArgs e)
            => new UserProdDetailWindow((custDG.SelectedItem as UserOrderProd).ProductId).Show();

        private void Done_Click(object sender, RoutedEventArgs e) => Close();
    }
}
