using DragonDrop.BLL.DataServices;
using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Implementation;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.DAL.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels;
using System.Windows;
using Unity;

namespace DragonDrop.WPF.StoreViews.Tabs.SubViews
{
    /// <summary>
    /// Interaction logic for OrderModificationWindow.xaml
    /// </summary>
    public partial class OrderModificationWindow : Window
    {
        public OrderModificationWindow(int ordId)
        {
            InitializeComponent();

            var container = new UnityContainer();
            container.RegisterType<IOrderStatusRepository, OrderStatusRepository>();
            container.RegisterType<IPaymentMethodRepository, PaymentMethodRepository>();
            container.RegisterType<IShippingMethodRepository, ShippingMethodRepository>();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IOrderDataService, OrderDataService>();
            container.RegisterInstance(ordId);

            DataContext = container.Resolve<OrderModViewModel>();
        }

        public void DisplayMessage(string msg)
            => MessageBox.Show(msg, "Info", MessageBoxButton.OK, MessageBoxImage.Information);

        private void UpdAdr_Click(object sender, RoutedEventArgs e)
        {
            var msg = string.Empty;
            if (shipWarn.IsVisible)
                msg="The parcel has already been handed over to the courier, please contact them to solve" +
             "your issue. If you have not yet received their contact information, please use the Report Issue button on the Order" +
             " Option Window and one of our Customer Support agents will gladly present you with the appropriate contact info.";
            else
            {
                //Notify the system of adress change request
                msg = "Address change for your Order has been registered. We will check your Account's address info again and" +
                    " make appropriate modifications to the parcel. One of our Customer Support agents will contact you shortly to" +
                    "confirm the change.";
            }
            DisplayMessage(msg);
        }

        private void ChangeQty_Click(object sender, RoutedEventArgs e)
        {
            var msg = string.Empty;
            if (shipWarn.IsVisible)
                msg = "The parcel has already been handed over to the courier. If you would like to return your Order, after receiving"+
                    " it, use the Report Issue button on the Order Options Window and one of our Customer Support agents will"+
                    " contact you shortly.";
            else
            {
                //Notify the system of adress change request
                msg = "One of our Customer Support Agents will contact you shortly to discuss any necessary changes.";
            }
            DisplayMessage(msg);
        }
    }
}
