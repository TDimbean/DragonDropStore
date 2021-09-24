using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Configuration;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class ShipSingleViewModel : BindableBase
    {
        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);
        private DateTime _realDate;
        private string _dateText;
        private DateTime _pickerDate;

        private int _ordId;
        private IOrderDataService _service;
        private IRelayReloadAndRemoteCloseControl _callingWindow;

        public ShipSingleViewModel(int ordId, IOrderDataService service, IRelayReloadAndRemoteCloseControl callingWindow)
        {
            _callingWindow = callingWindow;
            _service = service;
            _ordId = ordId;

            RealDate = DateTime.Now.Date;

            DateTextUpdateCommand = new DelegateCommand(DateTextUpdateCommandExecute);
            PickerDateUpdateCommand = new DelegateCommand(PickerDateUpdateCommandExecute);
            SubmitCommand = new DelegateCommand(SubmitCommandExecute);
        }

        #region Properties

        public DateTime RealDate
        {
            get => _realDate;
            set
            {
                SetProperty(ref _realDate, value);
                HandleDateChange();
            }
        }

        public string DateText
        {
            get => _dateText;
            set => SetProperty(ref _dateText, value);
        }

        public DateTime PickerDate
        {
            get => _pickerDate;
            set => SetProperty(ref _pickerDate, value);
        }

        public DelegateCommand DateTextUpdateCommand { get; }
        public DelegateCommand PickerDateUpdateCommand { get; }
        public DelegateCommand SubmitCommand { get; }

        #endregion

        #region Privates

        private void DateTextUpdateCommandExecute()
        {
            var newDate = DateTime.Now.Date;
            var parses = DateTime.TryParse(DateText, out newDate);
            if (parses && RealDate.Date != newDate) RealDate = newDate;
            else DateText = RealDate.ToShortDateString();
        }

        private void PickerDateUpdateCommandExecute()
        {
            if (RealDate.Date != PickerDate.Date) RealDate = PickerDate;
        }

        private void SubmitCommandExecute()
        {
            if (RealDate == null)
            {
                MessageBox.Show("Please enter a Shipping Date.",
                    "Dates Mismatch", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var ogOrd = _asyncCalls ? _service.GetAsync(_ordId).Result : _service.Get(_ordId);
            if (ogOrd.OrderDate > RealDate)
            {
                MessageBox.Show("The Shipping Date cannot be set later than the Order Date.",
                    "Dates Mismatch", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var newOrd = new Order
            {
                OrderDate = ogOrd.OrderDate,
                CustomerId = ogOrd.CustomerId,
                OrderId = ogOrd.OrderId,
                PaymentMethodId = ogOrd.PaymentMethodId,
                ShippingMethodId = ogOrd.ShippingMethodId,
                ShippingDate = RealDate.Date,
                OrderStatusId = 2
            };

            if (_asyncCalls) _service.UpdateAsync(newOrd);
            else _service.Update(newOrd);
            if (_callingWindow != null)
            {
                (_callingWindow as IRelayReloadAndRemoteCloseControl).RefreshParent();
                (_callingWindow as IRelayReloadAndRemoteCloseControl).Stop();
            }
        }

        #endregion

        private void HandleDateChange()
        {
            if (RealDate.Date == DateTime.Today && DateText != "Today") DateText = "Today";
            else if (RealDate.ToShortDateString() != DateText) DateText = RealDate.ToShortDateString();
            if (RealDate != PickerDate) PickerDate = RealDate;
        }
    }
}
