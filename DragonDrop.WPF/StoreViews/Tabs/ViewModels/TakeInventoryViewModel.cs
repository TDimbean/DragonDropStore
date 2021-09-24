using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure.Helpers;
using DragonDrop.WPF.Interfaces;
using DragonDrop.WPF.StoreViews.Tabs.SubViews;
using DragonDrop.WPF.StoreViews.Tabs.ViewModels.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class TakeInventoryViewModel : BindableBase, IAwaitUnlock
    {
        //Ctor Fields
        private IRemoteCloseControl _callingView;
        private IProductDataService _prodServ;

        //Visual Fields
        private int _checkQty;
        private Product _curScannedProd;

        private bool _canSubmit;
        private bool _justChecked;
        private bool _workingWithInc;

        //Scan Fields
        private TcpListener _server;
        private int _port;
        private string _code;
        private bool _halt;

        private IPAddress _localAdr;
        private byte[] _receivedBuffer = new byte[12];
        private StringBuilder _receivedCode;

        public TakeInventoryViewModel(IProductDataService prodServ, IRemoteCloseControl callingView = null)
        {
            //Trickle-down 
            _callingView = callingView;
            _prodServ = prodServ;

            //Grid Init
            IncomingList = new ObservableCollection<ProcessingProduct>();

            //Commands
            CheckCommand = new DelegateCommand(CheckCommandExecute);
            QtyChangedCommand = new DelegateCommand(QtyChangedCommandExecute);
            SetPortCommand = new DelegateCommand(SetPortCommandExecute);
            ShutDownCommand = new DelegateCommand(ShutDownCommandExecute);
            SubmitCommand = new DelegateCommand(SubmitCommandExecute);
            SelectCommand = new DelegateCommand<int?>(SelectCommandExecute);

            //Scan
            _localAdr = Dns.GetHostEntry("localhost").AddressList[0];
            Port = int.Parse(ConfigurationManager.AppSettings["Scanner_Port"]);

            //First Run
            Loop();
        }

        #region Properties

        public ObservableCollection<ProcessingProduct> IncomingList { get; set; }

        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string Code
        {
            get => _code;
            set
            {
                var wasNull = _code == null;
                var same = _code == value;
                SetProperty(ref _code, value);

                _curScannedProd = _prodServ.GetByBarcode(_code.Replace(" ", string.Empty));
                Application.Current.Dispatcher.Invoke(() => { _callingWindow.RefreshCheckBtn(); });

                if (!wasNull && same) CheckQty++;
                else CheckQty = 1;
            }
        }

        public bool CanCheck => BtnText != string.Empty;

        public bool CanSubmit { get => _canSubmit; set => SetProperty(ref _canSubmit, value); }

        public bool AcceptMode => !_workingWithInc;

        public int CheckQty
        {
            get => _checkQty;
            set => SetProperty(ref _checkQty, value);
        }

        public string BtnText
            => _curScannedProd == null ? _justChecked ? string.Empty : Code : _curScannedProd.Name;
        
        // Commands

        public DelegateCommand CheckCommand { get; }
        public DelegateCommand QtyChangedCommand { get; }
        public DelegateCommand SetPortCommand { get; }
        public DelegateCommand ShutDownCommand { get; }
        public DelegateCommand SubmitCommand { get; }
        public DelegateCommand<int?> SelectCommand { get; }

        #endregion

        #region Privates

        #region Commands

        private void ShutDownCommandExecute()
        {
            _server.Stop();
        }

        private void SetPortCommandExecute()
        {
            _server.Stop();
            Loop();
        }

        private void CheckCommandExecute()
        {
            _justChecked = true;


            var existent = IncomingList.SingleOrDefault(p=>p.BarCode == Code.Replace(" ",string.Empty));

            if (existent != null)
            {
                if (!_workingWithInc) existent.Quantity += CheckQty;
                else
                {
            if (CheckQty == 0) return;
                    existent.Quantity -= CheckQty;
                    if (existent.Quantity == 0) IncomingList.Remove(existent);
                    _workingWithInc = false;
                }
            }
            else
            {
                var prod = _prodServ.GetByBarcode(Code.Replace(" ", string.Empty));

                if (prod == null)
                {
                    var decission = MessageBox.Show("There is no Product associated with this Barcode." +
                        " Would you like to create one?", "Unknown Barcode",
                        MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                    if (decission == MessageBoxResult.Yes)
                    {
                        ShutDownCommandExecute();
                        Port = int.MaxValue;
                        if (_callingView != null) (_callingView as Window).IsEnabled = false;
                        new ProductAddWindow(barCode: Code, callingUnlockable: this).Show();
                    }
                    return;
                }

            if (CheckQty == 0) return;
                IncomingList.Add(new ProcessingProduct
                {
                    BarCode = prod.BarCode,
                    Name = prod.Name,
                    ProductId = prod.ProductId,
                    Quantity = CheckQty
                });
            }

            // Clean-up
            _curScannedProd = null;
            CheckQty = 0;
            _callingWindow.RefreshGrid();
            _callingWindow.RefreshCheckBtn();
        }

        private void QtyChangedCommandExecute()
        {
            if (_curScannedProd == null) CheckQty = 0;
        }
        
        private void SubmitCommandExecute()
        {
            foreach (var prod in IncomingList)
                _prodServ.AddStock(prod.ProductId, prod.Quantity);

            _callingView.Stop();
        }

        private void SelectCommandExecute(int? prodId)
        {
            if (prodId == null)
            {
                _workingWithInc = false;
                return;
            }
            Code = _prodServ.Get(prodId.GetValueOrDefault()).BarCode;
            CheckQty = IncomingList.SingleOrDefault(p => p.ProductId == prodId).Quantity;
            _workingWithInc = true;
        }

        #endregion

        #region Methods

        private TakeInventoryWindow _callingWindow => _callingView as TakeInventoryWindow;
        
        // Scanner

        private void Loop()
        {
            _server = new TcpListener(_localAdr, _port);
            _server.Start();
            _server.BeginAcceptTcpClient(Server_AcceptTcpClientCallback, null);
        }

        private void ServeClient(object State)
        {
            var client = (TcpClient)State;
            var stream = client.GetStream();

            try
            {
                // Communicate with your client
                _receivedCode = new StringBuilder();
                _receivedBuffer = new byte[12];

                stream.Read(_receivedBuffer, 0, 12);

                foreach (var b in _receivedBuffer)
                {
                    if (_receivedCode.Length == 1 || _receivedCode.Length == 7 || _receivedCode.Length == 13)
                        _receivedCode.Append(" ");
                    _receivedCode.Append(Convert.ToChar(b).ToString());
                }

                var strCode = _receivedCode.ToString();

                if (BarcodeVerifier.Check(strCode.Replace(" ", string.Empty))) Code = strCode;
                _justChecked = false;
            }

            finally
            {
                stream.Close();
                client.Close();
            }
        }

        public void Unlock()
        {
            if (_callingView != null) (_callingView as Window).IsEnabled = true;
            Port = int.Parse(ConfigurationManager.AppSettings["Scanner_Port"]);
            SetPortCommandExecute();
        }

        private void Server_AcceptTcpClientCallback(IAsyncResult ar)
        {
            try
            {

            var client = _server.EndAcceptTcpClient(ar);
            _server.BeginAcceptTcpClient(Server_AcceptTcpClientCallback, null);
            Task.Run(() => { ServeClient(client); });
            }
            catch (Exception)
            {

                Debug.WriteLine("Failed Callback on server.");
            }
        }

        #endregion

        #endregion
    }
}
