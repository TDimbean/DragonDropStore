using DragonDrop.Infrastructure;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class ScannerConfigViewModel : BindableBase
    {
        #region Fields

        private int _port;
        private string _status;

        private IPAddress _localAdr;
        private byte[] _receivedBuffer = new byte[12];
        private StringBuilder _receivedCode;
        private ManualResetEvent _stopEvent;
        private Thread _loopThread;

        #endregion

        public ScannerConfigViewModel()
        {
            // Init

            _localAdr = Dns.GetHostEntry("localhost").AddressList[0];
            Port = int.Parse(ConfigurationManager.AppSettings.Get("Scanner_Port"));
            Status = "Nothing";

            // Commands

            SetCommand = new DelegateCommand(SetCommandExecute);
            ShutDownCommand = new DelegateCommand(ShutDownCommandExecute);

            // First Run

            _stopEvent = new ManualResetEvent(false);
            _loopThread = new Thread(Loop);
            _loopThread.Start();
        }

        #region Properties

        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        // Commands

        public DelegateCommand SetCommand { get; }
        public DelegateCommand ShutDownCommand { get; }

        #endregion

        private void SetCommandExecute()
        {
            _stopEvent.Set();
            _stopEvent.Reset();

            Status = "Nothing";

            ConfigurationManager.AppSettings.Set("Scanner_Port", _port.ToString());
            _loopThread = new Thread(Loop);
            _loopThread.Start();
        }

        private void ShutDownCommandExecute()=> _stopEvent.Set();

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

                if (_receivedBuffer[0] == 33) _receivedCode.Append("!");
                else foreach (var b in _receivedBuffer)
                    {
                        if (_receivedCode.Length == 1 || _receivedCode.Length == 7 || _receivedCode.Length == 13)
                            _receivedCode.Append(" ");
                        _receivedCode.Append(Convert.ToChar(b).ToString());
                    }

                var strCode = _receivedCode.ToString();

                if (string.IsNullOrEmpty(strCode)) Status = "Nothing";
                else if (strCode == "!") Status = "Ping";
                else Status = "Code";
            }

            finally
            {
                stream.Close();
                client.Close();
            }
        }
    }
}
