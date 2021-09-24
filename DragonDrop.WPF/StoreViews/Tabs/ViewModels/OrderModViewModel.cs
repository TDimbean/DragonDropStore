using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.WPF.StoreViews.Tabs.SubViews;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class OrderModViewModel : BindableBase
    {
        private int _selPayIndex;
        private int _selShipIndex;

        public OrderModViewModel(int ordId, IPaymentMethodRepository payRepo, IShippingMethodRepository shipRepo,
                                IOrderStatusRepository statRepo, IOrderDataService service)
        {
            var ord = service.Get(ordId);

            // Init

            SubmitCommand = new DelegateCommand<RoutedEventArgs>(SubmitCommandExecute);

            PayMethods = new List<PaymentMethod>(payRepo.GetAll());
            ShipMethods = new List<ShippingMethod>(shipRepo.GetAll());

            SelPayIndex = ord.PaymentMethodId - 1;
            SelShipIndex = ord.ShippingMethodId - 1;

            PayMethWarnVisibility = ord.OrderStatusId == 3 ? Visibility.Visible : Visibility.Collapsed;
            ShipMethWarnVisibility = ord.OrderStatusId > 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        #region Properties

        public DelegateCommand<RoutedEventArgs> SubmitCommand { get; }

        public List<PaymentMethod> PayMethods { get; set; }
        public List<ShippingMethod> ShipMethods { get; set; }

        public PaymentMethod SelectedPayMethod { get; set; }
        public ShippingMethod SelectedShipMethod { get; set; }

        public int SelPayIndex
        {
            get => _selPayIndex;
            set => SetProperty(ref _selPayIndex, value);
        }

        public int SelShipIndex
        {
            get => _selShipIndex;
            set => SetProperty(ref _selShipIndex, value);
        }

        public Visibility PayMethWarnVisibility { get; set; }
        public Visibility ShipMethWarnVisibility { get; set; }

        #endregion

        private void SubmitCommandExecute(RoutedEventArgs e)
        {
            var win = ((e.Source as Button).Parent as StackPanel)
                .Parent as OrderModificationWindow;

            if (SelectedPayMethod == null) SelectedPayMethod = new PaymentMethod { PaymentMethodId = -1, Name = "Error" };
            if (SelectedShipMethod == null) SelectedShipMethod = new ShippingMethod { ShippingMethodId = -1, Name = "Error" };

            // Create a request and send it to the system to be adressed by one of the Customer Support agents
            win.DisplayMessage("A customer support agent has been notified of your desired changes and will contact you shortly"+
                " for confirmation.");
        }
    }
}
