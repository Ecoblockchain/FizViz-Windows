using System;
using System.Diagnostics;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FizVizController
{
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected
    };
    public delegate void ConnectionChangedEventHandler(ConnectionState state);

    public sealed partial class FizVizConnection : UserControl
    {
        public event ConnectionChangedEventHandler ConnectionStateChanged;
        private ConnectionState state = ConnectionState.Disconnected;
        public ConnectionState State {
            get
            {
                return state;
            }
            set
            {
                state = value;
                ConnectionStateChanged?.Invoke(state);
            }
        }

        private DispatcherTimer timeout;
        // stopwatch for tracking connection timing
        private readonly Stopwatch connectionStopwatch = new Stopwatch();
        private CancellationTokenSource cancelTokenSource;
        private const uint BaudRate = 57600;

        public FizVizConnection()
        {
            this.InitializeComponent();
            SetUiEnabled(true);
            ConnectionStateChanged += FizVizConnection_connectionStateChanged;
        }

        private void Connect()
        {
            State = ConnectionState.Connecting;

            SetUiEnabled(false);

            string host = IPAddressTextBox.Text;
            string port = PortTextBox.Text;
            ushort portnum = 0;

            if (Uri.CheckHostName(host) == UriHostNameType.Unknown)
            {
                ConnectMessage.Text = "You have entered an invalid host or IP.";
                ResetConnection();
                return;
            }

            if (!ushort.TryParse(port, out portnum))
            {
                ConnectMessage.Text = "You have entered an invalid port number.";
                ResetConnection();
                return;
            }

            App.FizViz = new FizVizDevice();
            App.FizViz.DeviceReady += OnDeviceReady;
            App.FizViz.ConnectionFailed += OnConnectionFailed;
            App.FizViz.ConnectionLost += OnConnectionLost;

            connectionStopwatch.Reset();
            connectionStopwatch.Start();

            //start a timer for connection timeout
            timeout = new DispatcherTimer {Interval = new TimeSpan(0, 0, 30)};
            timeout.Tick += Connection_TimeOut;
            timeout.Start();

            App.FizViz.Connect(host, portnum, BaudRate);
        }

        public void SetUiEnabled(bool connectEnabled)
        {
            ConnectButton.IsEnabled = connectEnabled;
        }

        /****************************************************************
         *                       UI Callbacks                           *
         ****************************************************************/

        /// <summary>
        /// Called if the Connect button is pressed
        /// </summary>
        /// <param name="sender">The object invoking the event</param>
        /// <param name="e">Arguments relating to the event</param>
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void PortTextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && ConnectButton.IsEnabled)
            {
                Connect();
            }
        }

        private void IPAddressTextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && PortTextBox.IsEnabled)
            {
                PortTextBox.Focus(FocusState.Programmatic);
            }
        }

        /****************************************************************
         *                  Event callbacks                             *
         ****************************************************************/

        private void OnConnectionFailed(string message)
        {
            var action = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                timeout.Stop();
                ConnectMessage.Text = "Connection attempt failed: " + message;

                connectionStopwatch.Stop();

                ResetConnection();
            }));
        }

        private void OnConnectionLost(string message)
        {
            var action = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                timeout.Stop();
                ConnectMessage.Text = "Connection was lost: " + message;

                connectionStopwatch.Stop();

                ResetConnection();
            }));
        }

        private void OnDeviceReady()
        {
            var action = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                timeout.Stop();
                ConnectMessage.Text = "Successfully connected!";

                State = ConnectionState.Connected;
            }));
        }

        private void Connection_TimeOut(object sender, object e)
        {
            var action = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                timeout.Stop();
                ConnectMessage.Text = "Connection attempt timed out.";

                ResetConnection();
            }));
        }

        /// <summary>
        /// This function is invoked if a cancellation is invoked for any reason on the connection task
        /// </summary>
        private void OnConnectionCancelled()
        {
            timeout.Stop();
            ConnectMessage.Text = "Connection attempt cancelled.";

            ResetConnection();
        }


        /****************************************************************
         *                  Helper functions                            *
         ****************************************************************/

        private void FizVizConnection_connectionStateChanged(ConnectionState state)
        {
            SetUiEnabled(state != ConnectionState.Connecting);
        }

        private void ResetConnection()
        {
            if (App.FizViz != null)
            {
                App.FizViz.ConnectionFailed -= OnConnectionFailed;
                App.FizViz.ConnectionLost -= OnConnectionLost;
                App.FizViz.DeviceReady -= OnDeviceReady;
                App.FizViz.Reset();
            }

            cancelTokenSource?.Dispose();

            App.FizViz = null;
            cancelTokenSource = null;
            State = ConnectionState.Disconnected;
        }
    }
}
