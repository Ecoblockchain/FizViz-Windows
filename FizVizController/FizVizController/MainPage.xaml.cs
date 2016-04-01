using Windows.UI.Xaml.Controls;

namespace FizVizController
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                ConnectionControl.ConnectionStateChanged += ConnectionControl_connectionStateChanged;

                ManualPositionControl.SetUiEnabled(false);
                FizVizConfigControl.SetUiEnabled(false);
                BackgroundColorConfigControl.SetUiEnabled(false);
                MinMaxConfigControl.SetUiEnabled(false);
            };
        }


        private void ConnectionControl_connectionStateChanged(ConnectionState state)
        {
            ManualPositionControl.SetUiEnabled(state == ConnectionState.Connected);
            FizVizConfigControl.SetUiEnabled(state == ConnectionState.Connected);
            BackgroundColorConfigControl.SetUiEnabled(state == ConnectionState.Connected);
            MinMaxConfigControl.SetUiEnabled(state == ConnectionState.Connected);
        }
    }
}
