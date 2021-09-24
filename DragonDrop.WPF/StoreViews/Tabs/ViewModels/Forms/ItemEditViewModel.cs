using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.Helpers;
using DragonDrop.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System.Configuration;
using System.Text;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class ItemEditViewModel : BindableBase
    {
        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);

        #region Fields

        private IOrderItemDataService _service;
        private IOrderDataService _ordService;
        private IProductDataService _prodService;
        private OrderItem _ogItem;

        private string _defError = "This shouldn't be visible.";

        #region OrderId

        private string _ordEntryText;
        private string _ordErrorText;
        private Visibility _ordErrorVisibility;
        private Visibility _ordWarnVisibility;

        #endregion

        #region ProductId

        private string _prodEntryText;
        private string _prodErrorText;
        private Visibility _prodErrorVisibility;
        private Visibility _prodWarnVisibility;

        #endregion

        #region Quantity

        private string _qtyEntryText;
        private string _qtyErrorText;
        private Visibility _qtyErrorVisibility;

        #endregion

        #endregion

        IRelayReloadAndRemoteCloseControl _callingView;

        public ItemEditViewModel
            (OrderItem ogItem, IOrderItemDataService service, 
            IOrderDataService ordService, IProductDataService prodService,
            IRelayReloadAndRemoteCloseControl callingView = null)
        {
            _callingView = callingView;
            _ordService = ordService;
            _prodService = prodService;

            _service = service;
            _ogItem = ogItem;

            #region Commands

            SubmitCommand = _asyncCalls ? new DelegateCommand(SubmitCommandExecuteAsync) :
                new DelegateCommand(SubmitCommandExecute);

            #region OrderId

            OrdBoxLostFocusCommand = new DelegateCommand(OrdBoxLostFocusCommandExecute);
            OrdBoxTextChangedCommand = new DelegateCommand(OrdBoxTextChangedCommandExecute);
            OrdResetCommand = new DelegateCommand(OrdResetCommandExecute);

            #endregion

            #region ProductId

            ProdBoxLostFocusCommand = new DelegateCommand(ProdBoxLostFocusCommandExecute);
            ProdBoxTextChangedCommand = new DelegateCommand(ProdBoxTextChangedCommandExecute);
            ProdResetCommand = new DelegateCommand(ProdResetCommandExecute);

            #endregion

            #region Quantity

            QtyBoxLostFocusCommand = new DelegateCommand(QtyBoxLostFocusCommandExecute);
            QtyBoxTextChangedCommand = new DelegateCommand(QtyBoxTextChangedCommandExecute);
            QtyResetCommand = new DelegateCommand(QtyResetCommandExecute);

            #endregion

            #endregion

            #region Init

            #region Order Id

            OrdEntryText = _ogItem.OrderId.ToString();
            OrdErrorText = _defError;
            OrdErrorVisibility = Visibility.Collapsed;
            OrdWarnVisibility = Visibility.Collapsed;

            #endregion

            #region Product Id

            ProdEntryText = _ogItem.ProductId.ToString();
            ProdErrorText = _defError;
            ProdErrorVisibility = Visibility.Collapsed;
            ProdWarnVisibility = Visibility.Collapsed;

            #endregion

            #region Quantity

            QtyEntryText = _ogItem.Quantity.ToString();
            QtyErrorText = _defError;
            QtyErrorVisibility = Visibility.Collapsed;

            #endregion

            #endregion
        }

        #region Properties

        #region ViewModelProps

        #region OrderIdEntry

        public string OrdEntryText
        {
            get => _ordEntryText;
            set => SetProperty(ref _ordEntryText, value);
        }

        public string OrdErrorText
        {
            get => _ordErrorText;
            set => SetProperty(ref _ordErrorText, value);
        }

        public Visibility OrdErrorVisibility
        {
            get => _ordErrorVisibility;
            set => SetProperty(ref _ordErrorVisibility, value);
        }

        public Visibility OrdWarnVisibility
        {
            get => _ordWarnVisibility;
            set => SetProperty(ref _ordWarnVisibility, value);
        }

        #endregion

        #region ProductIdEntry

        public string ProdEntryText
        {
            get => _prodEntryText;
            set => SetProperty(ref _prodEntryText, value);
        }

        public string ProdErrorText
        {
            get => _prodErrorText;
            set => SetProperty(ref _prodErrorText, value);
        }

        public Visibility ProdErrorVisibility
        {
            get => _prodErrorVisibility;
            set => SetProperty(ref _prodErrorVisibility, value);
        }

        public Visibility ProdWarnVisibility
        {
            get => _prodWarnVisibility;
            set => SetProperty(ref _prodWarnVisibility, value);
        }

        #endregion

        #region QuantityEntry

        public string QtyEntryText
        {
            get => _qtyEntryText;
            set => SetProperty(ref _qtyEntryText, value);
        }

        public string QtyErrorText
        {
            get => _qtyErrorText;
            set => SetProperty(ref _qtyErrorText, value);
        }

        public Visibility QtyErrorVisibility
        {
            get => _qtyErrorVisibility;
            set => SetProperty(ref _qtyErrorVisibility, value);
        }

        #endregion

        #endregion

        #region Commands

        public DelegateCommand SubmitCommand { get; }

        #region Order ID

        public DelegateCommand OrdResetCommand { get; }
        public DelegateCommand OrdBoxLostFocusCommand { get; }
        public DelegateCommand OrdBoxTextChangedCommand { get; }

        #endregion

        #region Product ID

        public DelegateCommand ProdResetCommand { get; }
        public DelegateCommand ProdBoxLostFocusCommand { get; }
        public DelegateCommand ProdBoxTextChangedCommand { get; }

        #endregion

        #region Quantity

        public DelegateCommand QtyResetCommand { get; }
        public DelegateCommand QtyBoxLostFocusCommand { get; }
        public DelegateCommand QtyBoxTextChangedCommand { get; }

        #endregion

        #endregion

        #endregion

        #region Commands

        private async void SubmitCommandExecuteAsync()
        {
            #region Converting Field Entry Data
            var ordId = -1;
            var parses = int.TryParse(OrdEntryText, out ordId);
            if (!parses) ordId = -1;

            var prodId = -1;
            parses = int.TryParse(ProdEntryText, out prodId);
            if (!parses) prodId = -1;

            var qty = -1;
            parses = int.TryParse(QtyEntryText, out qty);
            if (!parses) qty = -1;

            #endregion

            var item = new OrderItem
            {
                OrderId = ordId,
                ProductId = prodId,
                Quantity = qty
            };

            var val = _service.ValidateOrderItem(item);

            if (val.isValid)
            {
                StaticLogger.LogInfo(GetType(), "Admin requesting Manual Order Item Update from WPF form.");
                await _service.UpdateAsync(item);

                if (_callingView == null) return;
                _callingView.RefreshParent();
                _callingView.Stop();
            }
            else
            {
                var bulkList = val.errorList;
                var valErrors = new StringBuilder();
                while (!string.IsNullOrEmpty(bulkList))
                {
                    var line = bulkList.GetUntilOrEmpty(".");
                    valErrors.AppendLine(line);
                    bulkList = bulkList.Replace(line, string.Empty);
                }
                MessageBox.Show(valErrors.ToString(), "Failed to Update Order Item", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void SubmitCommandExecute()
        {
            #region Converting Field Entry Data
            var ordId = -1;
            var parses = int.TryParse(OrdEntryText, out ordId);
            if (!parses) ordId = -1;

            var prodId = -1;
            parses = int.TryParse(ProdEntryText, out prodId);
            if (!parses) prodId = -1;

            var qty = -1;
            parses = int.TryParse(QtyEntryText, out qty);
            if (!parses) qty = -1;

            #endregion

            var item = new OrderItem
            {
                OrderId = ordId,
                ProductId = prodId,
                Quantity = qty
            };

            var val = _service.ValidateOrderItem(item);

            if (val.isValid)
            {
                StaticLogger.LogInfo(GetType(), "Admin requesting Manual Order Item Update from WPF form.");
                _service.Update(item);

                if (_callingView == null) return;
                _callingView.RefreshParent();
                _callingView.Stop();
            }
            else
            {
                var bulkList = val.errorList;
                var valErrors = new StringBuilder();
                while (!string.IsNullOrEmpty(bulkList))
                {
                    var line = bulkList.GetUntilOrEmpty(".");
                    valErrors.AppendLine(line);
                    bulkList = bulkList.Replace(line, string.Empty);
                }
                MessageBox.Show(valErrors.ToString(), "Failed to Update Order Item", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        #region OrderId

        private void OrdResetCommandExecute()
        {
            OrdEntryText = _ogItem.OrderId.ToString();
            if (_asyncCalls) CheckStatusAsync();
            else CheckStatus();
        }

        private void OrdBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(OrdEntryText)) OrdResetCommandExecute();
        }

        private void OrdBoxTextChangedCommandExecute()
        {
            var ordId = 0;
            var idParses = int.TryParse(OrdEntryText, out ordId);

            if (!idParses)
            {
                OrdErrorText = "Invalid Order ID format";
                OrdErrorVisibility = Visibility.Visible;
                return;
            }

            var validation = _service.ValidateOrderId(ordId);
            if (!validation.isValid)
            {
                OrdErrorText = validation.error;
                OrdErrorVisibility = Visibility.Visible;
            }
            else
            {
                OrdErrorText = _defError;
                OrdErrorVisibility = Visibility.Collapsed;
            }

            if (_asyncCalls) CheckStatusAsync();
            else CheckStatus();
        }

        #endregion

        #region ProductId

        private void ProdResetCommandExecute()
        {
            ProdEntryText = _ogItem.ProductId.ToString();
            if (_asyncCalls) CheckStockAsync();
            else CheckStock();
        }

        private void ProdBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(ProdEntryText)) ProdResetCommandExecute();
            if (_asyncCalls) CheckStockAsync();
            else CheckStock();
        }

        private void ProdBoxTextChangedCommandExecute()
        {
            var prodId = 0;
            var idParses = int.TryParse(ProdEntryText, out prodId);

            if (!idParses)
            {
                ProdErrorText = "Invalid Product ID format";
                ProdErrorVisibility = Visibility.Visible;
                return;
            }

            var validation = _service.ValidateProductId(prodId);
            if (!validation.isValid)
            {
                ProdErrorText = validation.error;
                ProdErrorVisibility = Visibility.Visible;
            }
            else
            {
                ProdErrorText = _defError;
                ProdErrorVisibility = Visibility.Collapsed;
            }

            if (_asyncCalls) CheckStockAsync();
            else CheckStock();
        }

        #endregion

        #region Quantity

        private void QtyResetCommandExecute()
        {
            QtyEntryText = _ogItem.Quantity.ToString();
            if (_asyncCalls) CheckStockAsync();
            else CheckStock();
        }

        private void QtyBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(QtyEntryText)) QtyResetCommandExecute();
            if (_asyncCalls) CheckStockAsync();
            else CheckStock();
        }

        private void QtyBoxTextChangedCommandExecute()
        {
            var qty = -1;
            var parses = int.TryParse(QtyEntryText, out qty);

            if (!parses)
            {
                QtyErrorText = "Quantity must be a positive whole number";
                QtyErrorVisibility = Visibility.Visible;
                return;
            }

            var validation = _service.ValidateQuantity(qty);
            if (!validation.isValid)
            {
                QtyErrorText = validation.error;
                QtyErrorVisibility = Visibility.Visible;
            }
            else
            {
                QtyErrorText = _defError;
                QtyErrorVisibility = Visibility.Collapsed;
            }

            if (_asyncCalls) CheckStockAsync();
            else CheckStock();
        }

        #endregion

        #endregion

        private async void CheckStatusAsync()
        {
            var ordId = -1;
            var parses = int.TryParse(OrdEntryText, out ordId);

            var ord = await _ordService.GetAsync(ordId);
            if (ord != null && ord.OrderStatusId > 1) OrdWarnVisibility = Visibility.Visible;
            else OrdWarnVisibility = Visibility.Collapsed;
        }

        private void CheckStatus()
        {
            var ordId = -1;
            var parses = int.TryParse(OrdEntryText, out ordId);

            var ord = _ordService.Get(ordId);
            if (ord != null && ord.OrderStatusId > 1) OrdWarnVisibility = Visibility.Visible;
            else OrdWarnVisibility = Visibility.Collapsed;
        }

        private async void CheckStockAsync()
        {
            var prodId = 0;
            var parses = int.TryParse(ProdEntryText, out prodId);
            var qty = 0;
            parses = parses && int.TryParse(QtyEntryText, out qty);

            if (!parses) return;

            var prod = await _prodService.GetAsync(prodId);
            if (prod != null && prod.Stock < qty) ProdWarnVisibility = Visibility.Visible;
            else ProdWarnVisibility = Visibility.Collapsed;
        }

        private void CheckStock()
        {
            var prodId = 0;
            var parses = int.TryParse(ProdEntryText, out prodId);
            var qty = 0;
            parses = parses && int.TryParse(QtyEntryText, out qty);

            if (!parses) return;

            var prod = _prodService.Get(prodId);
            if (prod != null && prod.Stock < qty) ProdWarnVisibility = Visibility.Visible;
            else ProdWarnVisibility = Visibility.Collapsed;
        }
    }
}
