using DragonDrop.Infrastructure.Helpers;
using DragonDrop.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class SingleScanViewModel : BindableBase
    {
        #region Fields

        private int _port;
        private string _code;

        private IPAddress _localAdr;
        private byte[] _receivedBuffer = new byte[12];
        private StringBuilder _receivedCode;
        private ManualResetEvent _stopEvent;
        private Thread _loopThread;

        #endregion

        private IReceiveBarcodeAndRemoteCloseControl _callingView;

        public SingleScanViewModel(IReceiveBarcodeAndRemoteCloseControl callingView)
        {
            //Init

            _callingView = callingView;
            _localAdr = Dns.GetHostEntry("localhost").AddressList[0];

            Port = int.Parse(ConfigurationManager.AppSettings["Scanner_Port"]);

            // Commands

            SetPortCommand = new DelegateCommand(SetPortCommandExecute);
            SubmitCommand = new DelegateCommand(SubmitCommandExecute);
            ShutDownCommand = new DelegateCommand(ShutDownCommandExecute);

            //First Run

            _stopEvent = new ManualResetEvent(false);
            _loopThread = new Thread(Loop);
            _loopThread.Start();
        }

        #region ViewModel Props

        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        #endregion

        public DelegateCommand SetPortCommand { get; }
        public DelegateCommand SubmitCommand { get; }
        public DelegateCommand ShutDownCommand { get; }

        private void SubmitCommandExecute()
        {
            var passes = BarcodeVerifier.Check(Code.Replace(" ", string.Empty));

            if (!passes)
            {
                MessageBox.Show("The code is not un a correct format!", "Code Issue",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            _callingView.SetBarcode(Code);
            _callingView.Stop();
        }

        private void SetPortCommandExecute()
        {
            _stopEvent.Set();
            _stopEvent.Reset();
            _loopThread = new Thread(Loop);
            _loopThread.Start();
        }

        private void ShutDownCommandExecute() => _stopEvent.Set();

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

                if (BarcodeVerifier.Check(strCode.Replace(" ",string.Empty))) Code = strCode;
            }

            finally
            {
                stream.Close();
                client.Close();
            }
        }
    }
}
