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
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class ProcessOrderViewModel : BindableBase
    {
        #region Fields

        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);

        //Ctor Fields
        private IRelayReloadAndRemoteCloseControl _callingView;
        private IProductDataService _prodServ;
        private IOrderDataService _ordServ;
        private int _ordId;

        //Visual Fields
        private int _checkQty;
        private Product _curScannedProd;

        private Visibility _procGridVis = Visibility.Collapsed;
        private Visibility _unProcGridVis = Visibility.Visible;
        private bool _canSubmit;

        //Scan Fields
        private int _port;
        private string _code;
        private string _prevCode;

        private IPAddress _localAdr;
        private byte[] _receivedBuffer = new byte[12];
        private StringBuilder _receivedCode;
        private ManualResetEvent _stopEvent;
        private Thread _loopThread;

        #endregion

        public ProcessOrderViewModel
            (int ordId, IOrderDataService ordServ, IProductDataService prodServ,
            IOrderItemDataService itemServ, IRelayReloadAndRemoteCloseControl callingView)
        {
            //Trickle-down 
            _callingView = callingView;

            _prodServ = prodServ;
            _ordServ = ordServ;
            _ordId = ordId;
            
            //Grid Lists Init
            ProcessedList = new ObservableCollection<ProcessingProduct>();
            UnprocessedList = new ObservableCollection<ProcessingProduct>();

            ProcessedList.CollectionChanged += UpdateProcessed;
            UnprocessedList.CollectionChanged += UpdateUnprocessed;

            //Commands
            CheckCommand = new DelegateCommand(CheckCommandExecute);
            QtyChangedCommand = new DelegateCommand(QtyChangedCommandExecute);
            SetPortCommand = new DelegateCommand(SetPortCommandExecute);
            ShutDownCommand = new DelegateCommand(ShutDownCommandExecute);
            SubmitCommand = new DelegateCommand(SubmitCommandExecute);

            //Grids
            var itemList = _asyncCalls ? itemServ.GetAllByOrderId(ordId) :
                itemServ.GetAllByOrderIdAsync(ordId).Result;

            foreach (var item in itemList)
            {
                var prod = _asyncCalls ? prodServ.GetAsync(item.ProductId).Result :
                    prodServ.Get(item.ProductId);

                UnprocessedList.Add(new ProcessingProduct
                {
                    Name = prod.Name,
                    ProductId = prod.ProductId,
                    BarCode = prod.BarCode,
                    Quantity = item.Quantity

                });
            }

            //Scan
            _localAdr = Dns.GetHostEntry("localhost").AddressList[0];
            Port = int.Parse(ConfigurationManager.AppSettings["Scanner_Port"]);

            //First Run
            _stopEvent = new ManualResetEvent(false);
            _loopThread = new Thread(Loop);
            _loopThread.Start();
        }

        #region Properties

        public ObservableCollection<ProcessingProduct> UnprocessedList { get; set; }
        public ObservableCollection<ProcessingProduct> ProcessedList { get; set; }

        public Visibility UnProcGridVis { get => _unProcGridVis; set => SetProperty(ref _unProcGridVis, value); }
        public Visibility ProcGridVis { get => _procGridVis; set => SetProperty(ref _procGridVis, value); }

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

                try
                {
                    _curScannedProd = _prodServ.Get(UnprocessedList.SingleOrDefault
                        (p => p.BarCode == _code.Replace(" ", string.Empty)).ProductId);
                }
                catch (Exception)
                {
                    _curScannedProd = null;
                }
                
                if (_curScannedProd==null)
                    MessageBox.Show("There are no unprocessed products that match this Barcode!",
                        "Mismatch", MessageBoxButton.OK, MessageBoxImage.Warning);

                if(!wasNull && same)CheckQty++;
                else CheckQty = 1;
                Application.Current.Dispatcher.Invoke(() => { _callingWindow.RefreshCheckBtn(); });
            }
        }

        public bool CanCheck=>_curScannedProd != null;

        public bool CanSubmit { get => _canSubmit; set => SetProperty(ref _canSubmit, value); }

        public int CheckQty
        {
            get => _checkQty;
            set => SetProperty(ref _checkQty, value);
        }

        public string CurScannedTitle => _curScannedProd ==null ? string.Empty : _curScannedProd.Name;

        // Commands

        public DelegateCommand CheckCommand { get; }
        public DelegateCommand QtyChangedCommand { get; }
        public DelegateCommand SetPortCommand { get; }
        public DelegateCommand ShutDownCommand { get; }
        public DelegateCommand SubmitCommand { get; }

        #endregion

        #region Privates

        #region Commands

        private void ShutDownCommandExecute() => _stopEvent.Set();

        private void SetPortCommandExecute()
        {
            _stopEvent.Set();
            _stopEvent.Reset();
            _loopThread = new Thread(Loop);
            _loopThread.Start();
        }

        private void CheckCommandExecute()
        {
            if (CheckQty == 0) return;

            var proProd = ProcessedList.SingleOrDefault(p => p.ProductId == _curScannedProd.ProductId);
            var procQty = proProd == null ? 0 : proProd.Quantity;

            if (CheckQty + procQty > _curScannedProd.Stock)
            {
                MessageBox.Show("Insufficient stock!");
                return;
            }

            var unpProd = GetUnprocessed(_curScannedProd.ProductId);

            if (proProd == null)
            {
                proProd = new ProcessingProduct
                {
                    BarCode = unpProd.BarCode,
                    Name = unpProd.Name,
                    ProductId = unpProd.ProductId,
                    Quantity = CheckQty
                };
                ProcessedList.Add(proProd);
            }
            else proProd.Quantity += CheckQty;
            unpProd.Quantity -= CheckQty;
            if (unpProd.Quantity == 0) UnprocessedList.Remove(unpProd);

            // Clean-up
            _curScannedProd = null;
            CheckQty = 0;
            _callingWindow.RefreshGrids();
            _callingWindow.RefreshCheckBtn();
        }

        private void QtyChangedCommandExecute()
        {
            if (_curScannedProd == null) CheckQty = 0;
            else
            {
                // This breaks if you change the order of Check>Clean-up. It can be avoided with an additional null check
                var unpQty = UnprocessedList.SingleOrDefault(p => p.ProductId == _curScannedProd.ProductId).Quantity;
                if (_checkQty > unpQty) CheckQty = unpQty;
            }
        }

        private void SubmitCommandExecute()
        {
            if (UnprocessedList.Any()) return;
            if(_asyncCalls)
            {
                foreach (var prod in ProcessedList)
                    _prodServ.AddStockAsync(prod.ProductId, -prod.Quantity);
                _ordServ.PromoteReceivedAsync(_ordId);
            }

            else
            {
            foreach (var prod in ProcessedList)
                _prodServ.AddStock(prod.ProductId, -prod.Quantity);
            _ordServ.PromoteReceived(_ordId);
            }

            _callingView.RefreshParent();
            _callingView.Stop();
        }

        private ProcessOrderWindow _callingWindow => _callingView as ProcessOrderWindow;
        
        #endregion

        #region Methods

        // UI

        private void UpdateProcessed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ProcessedList.Count == 0) ProcGridVis = Visibility.Collapsed;
            else if (ProcGridVis != Visibility.Visible) ProcGridVis = Visibility.Visible;
        }

        private void UpdateUnprocessed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (UnprocessedList.Count == 0)
            {
                UnProcGridVis = Visibility.Collapsed;
                CanSubmit = true;
            }

            else if (UnProcGridVis != Visibility.Visible) UnProcGridVis = Visibility.Visible;
        }

        private ProcessingProduct GetUnprocessed(int prodId)
            => UnprocessedList.SingleOrDefault(p => p.ProductId == prodId);

        // Scanner

        private void Loop()
        {
            TcpListener server = new TcpListener(_localAdr, _port);
            var waitHandle = new WaitHandle[2];
            waitHandle[0] = _stopEvent;

            Debug.WriteLine("Listening on port {0}", _port);

            server.Start();
            while (true)
            {
                var result = server.BeginAcceptTcpClient(null, null);
                waitHandle[1] = result.AsyncWaitHandle;
                // Wait for next client to connect or StopEvent
                var handleCode = WaitHandle.WaitAny(waitHandle);
                if (handleCode == 0)  // StopEvent was set (from outside), terminate loop
                    break;
                if (handleCode == 1)
                {
                    TcpClient client = server.EndAcceptTcpClient(result);

                    client.ReceiveTimeout = 90000;
                    client.SendTimeout = 90000;

                    // client is connected, spawn thread for it and continue to wait for others
                    var t = new Thread(ServeClient);
                    t.IsBackground = true;
                    t.Start(client);
                }
            }
            server.Stop();

            Debug.WriteLine("Listener stopped");
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
            }

            finally
            {
                stream.Close();
                client.Close();
            }
        }


        //Doesnt work because it won't set the visibility, since it's not a ref
        //private void UpdateCollection(ObservableCollection<ProcessingProduct> col, Visibility vis)
        //{
        //    if (col.Count == 0) vis = Visibility.Collapsed;
        //    else if (vis != Visibility.Visible) vis = Visibility.Visible;
        //}

        #endregion

        #endregion
    }
}
