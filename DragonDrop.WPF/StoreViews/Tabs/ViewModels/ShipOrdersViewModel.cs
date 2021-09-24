using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.SubViews;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class ShipOrdersViewModel : BindableBase
    {
        #region Fields

        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);

        private IRemoteCloseControl _callingView;

        private IOrderDataService _ordServ;

        private int _checkQty;
        private Order _curSelectedOrd;

        private Visibility _procGridVis = Visibility.Visible;
        private Visibility _shipGridVis = Visibility.Collapsed;

        private string _dateText;
        private DateTime _pickerDate;
        private DateTime _realDate;

        private string _shipBtnTxt;
        private bool _usingShippedGrid;

        private Visibility _dateBoxVis;

        #endregion

        public ShipOrdersViewModel(IOrderDataService ordServ, IRemoteCloseControl callingView)
        {
            _callingView = callingView;
            _ordServ = ordServ;

            ProcessedList = new ObservableCollection<Order>();
            ShippedList = new ObservableCollection<Order>();

            ShippedList.CollectionChanged += UpdateShipped;
            ProcessedList.CollectionChanged += UpdateProcessed;

            RealDate = DateTime.Now;

            ArrowOrientation = new ScaleTransform(1, 1);

            // Grids

            var ordList = ordServ.GetAllProcessed();

            foreach (var ord in ordList)
            {
                ProcessedList.Add(ord);
            }

            // Commands

            ShipCommand = new DelegateCommand(ShipCommandExecute);
            DateTextChangedCommand = new DelegateCommand(DateTextChangedCommandExecute);
            PickerDateChangedCommand = new DelegateCommand(PickerDateChangedCommandExecute);
            SelectCommand = new DelegateCommand<(int ordId, bool fromShipped)?>(SelectCommandExecute);
            MarkCommand = new DelegateCommand(MarkCommandExecute);
        }

        #region Properties

        public ScaleTransform ArrowOrientation { get; set; }

        public ObservableCollection<Order> ProcessedList { get; set; }
        public ObservableCollection<Order> ShippedList { get; set; }

        public Visibility ProcGridVis { get => _procGridVis; set => SetProperty(ref _procGridVis, value); }
        public Visibility ShipGridVis { get => _shipGridVis; set => SetProperty(ref _shipGridVis, value); }

        public DelegateCommand ShipCommand { get; }
        public DelegateCommand DateTextChangedCommand { get; }
        public DelegateCommand PickerDateChangedCommand { get; }
        public DelegateCommand<(int ordId, bool fromShipped)?> SelectCommand { get; }
        public DelegateCommand MarkCommand { get; }

        public bool CanShip => _curSelectedOrd != null;

        public string ShipBtnTxt
        {
            get => _shipBtnTxt;
            set => SetProperty(ref _shipBtnTxt, value);
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

        public DateTime RealDate
        {
            get => _realDate;
            set
            {
                SetProperty(ref _realDate, value);
                HandleDateChange();
            }
        }

        public Visibility DateBoxVis
        {
            get => _usingShippedGrid ? Visibility.Collapsed:Visibility.Visible;
        }

        #endregion

        #region Privates

        #region Commands

        private void ShipCommandExecute()
        {
            var newOrd = new Order()
            {
                CustomerId = _curSelectedOrd.CustomerId,
                OrderId = _curSelectedOrd.OrderId,
                OrderDate = _curSelectedOrd.OrderDate,
                ShippingMethodId = _curSelectedOrd.ShippingMethodId,
                PaymentMethodId = _curSelectedOrd.PaymentMethodId,
                ShippingDate = _curSelectedOrd.OrderStatusId == 1 ? RealDate : (DateTime?)null,
                OrderStatusId = _curSelectedOrd.OrderStatusId == 1 ? 2 : 1
            };

            if (_usingShippedGrid)
            {
                ProcessedList.Add(newOrd);
                ShippedList.Remove(_curSelectedOrd);
            }
            else
            {
                ProcessedList.Remove(_curSelectedOrd);
                ShippedList.Add(newOrd);
            }

            _curSelectedOrd = null;
            RealDate = DateTime.Today;
        }

        private void DateTextChangedCommandExecute()
        {
            if (DateText == "Today" && RealDate != DateTime.Today) RealDate = DateTime.Today;
            else
            {
                var newDate = DateTime.Today;
                var parses = DateTime.TryParse(DateText, out newDate);
                if (parses && newDate != RealDate) RealDate = newDate;
            }
        }

        private void PickerDateChangedCommandExecute() => RealDate = PickerDate;

        private void SelectCommandExecute((int ordId, bool fromShipped)? info)
        {
            //_curSelectedOrd = _ordServ.Get(info.GetValueOrDefault().ordId);
            var joinedList = ProcessedList.ToList();
            joinedList.AddRange(ShippedList.ToList());
            _curSelectedOrd = joinedList.SingleOrDefault(o => o.OrderId == info.GetValueOrDefault().ordId);

            var win = _callingView as ShipOrdersWindow;

            if (info.GetValueOrDefault().fromShipped)
            {
                ShipBtnTxt = "Recall";
                _usingShippedGrid = true;
                ArrowOrientation = new ScaleTransform(1, -1);
                win.EmptyProcSel();
            }
            else
            {
                ShipBtnTxt = "Ship On";
                _usingShippedGrid = false;
                ArrowOrientation = new ScaleTransform(1, 1);
                win.EmptyShipSel();
            }

            win.RefreshDateVis();
            win.RefreshArrows();
            
            RealDate = DateTime.Now;
        }

        private void MarkCommandExecute()
        {
            foreach (var ord in ShippedList)
            {
                if(_asyncCalls) _ordServ.PromoteProcessedAsync(ord.OrderId, ord.ShippingDate.GetValueOrDefault());
                else _ordServ.PromoteProcessed(ord.OrderId, ord.ShippingDate.GetValueOrDefault());
            }

            _callingView.Stop();
        }

        #endregion

        // Methods

        private void HandleDateChange()
        {
            if (PickerDate != RealDate) PickerDate = RealDate;
            if (RealDate.Date == DateTime.Today)
            {
                if (DateText != "Today") DateText = "Today";
            }
            else if (DateText != RealDate.ToShortDateString()) DateText = RealDate.ToShortDateString();
        }

        private void UpdateProcessed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ProcessedList.Count == 0) ProcGridVis = Visibility.Collapsed;
            else if (ProcGridVis != Visibility.Visible) ProcGridVis = Visibility.Visible;
        }

        private void UpdateShipped(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ShippedList.Count == 0) ShipGridVis = Visibility.Collapsed;
            else if (ShipGridVis != Visibility.Visible) ShipGridVis = Visibility.Visible;
        }

        #endregion
    }
}
