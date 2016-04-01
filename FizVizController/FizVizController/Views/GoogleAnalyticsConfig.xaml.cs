using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Google.Apis.Analytics.v3.Data;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace FizVizController
{
    public enum AnalyticsNeedlePositionOption
    {
        RealTimePageViews,
        RealTimeUsers
    };

    public sealed partial class GoogleAnalyticsConfig : UserControl
    {
        public GoogleAnalyticsConfig()
        {
            this.InitializeComponent();
            InitializeMetricsList();
        }


        /****************************************************************
         *                       UI Callbacks                           *
         ****************************************************************/

        private void AuthenticateButton_OnClick(object sender, RoutedEventArgs e)
        {
            DoAuthAsync();
        }

        private void NeedlePositionMetricComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAnalyticsSettings();
        }

        private void NeedleDisplayModeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAnalyticsSettings();
        }

        private void ProfileSiteComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProfileSiteComboBox.SelectedValue == null) return;

            App.AnalyticsManager.SetProfileID((string)ProfileSiteComboBox.SelectedValue);
            UpdateAnalyticsSettings();
        }

        private void MetricTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            UpdateAnalyticsSettings();
        }

        private void MetricTextBox_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key.Equals(VirtualKey.Enter))
            {
                UpdateAnalyticsSettings();
            }
        }

        /****************************************************************
         *                  Helper functions                            *
         ****************************************************************/

        /// <summary>
        /// Fill in the available Google Analytic metrics
        /// </summary>
        private void InitializeMetricsList()
        {
            NeedlePositionMetricComboBox.DisplayMemberPath = "Key";
            NeedlePositionMetricComboBox.SelectedValuePath = "Value";
            NeedlePositionMetricComboBox.Items.Add(new KeyValuePair<string, string>("Real Time Page Views", "rt:pageviews"));
            NeedlePositionMetricComboBox.Items.Add(new KeyValuePair<string, string>("Real Time Active Users", "rt:activeUsers"));
            NeedlePositionMetricComboBox.Items.Add(new KeyValuePair<string, string>("Sessions", "ga:sessions"));
            NeedlePositionMetricComboBox.Items.Add(new KeyValuePair<string, string>("Page Views", "ga:pageviews"));
            NeedlePositionMetricComboBox.Items.Add(new KeyValuePair<string, string>("Users", "ga:users"));
            NeedlePositionMetricComboBox.Items.Add(new KeyValuePair<string, string>("Bounce Rate", "ga:bounceRate"));
            NeedlePositionMetricComboBox.Items.Add(new KeyValuePair<string, string>("Avg Session Duration", "ga:avgSessionDuration"));
            NeedlePositionMetricComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Save current settings to the Analytics FizViz interface
        /// </summary>
        private void UpdateAnalyticsSettings()
        {
            ViewUtility.ResetControlBorders(ControlGrid);

            float? metricMinimum, metricMaximum, angleMinimum, angleMaximum;
            metricMinimum = ViewUtility.GetFloat(MinimumMetricTextBox);
            metricMaximum = ViewUtility.GetFloat(MaximumMetricTextBox);
            angleMinimum = ViewUtility.GetFloat(MinimumAngleTextBox);
            angleMaximum = ViewUtility.GetFloat(MaximumAngleTextBox);
            if (!metricMinimum.HasValue || !metricMaximum.HasValue || !angleMaximum.HasValue || !angleMinimum.HasValue)
                return;

            App.AnalyticsFizVizInterface.MetricMinimum = metricMinimum.Value;
            App.AnalyticsFizVizInterface.MetricMaximum = metricMaximum.Value;
            App.AnalyticsFizVizInterface.DisplayAngleMaximum = angleMaximum.Value;
            App.AnalyticsFizVizInterface.DisplayAngleMinimum = angleMinimum.Value;

            if (NeedlePositionMetricComboBox.SelectedValue != null)
            {
                App.AnalyticsFizVizInterface.Metric = (string) NeedlePositionMetricComboBox.SelectedValue;
            }
        }


        /// <summary>
        /// Authenticate, and then update the profile list.
        /// </summary>
        /// <returns></returns>
        private async Task DoAuthAsync()
        {
            try
            {
                Task t = App.AnalyticsManager.AuthenticateAsync();
                await t;

                IList<Profile> profiles = App.AnalyticsManager.GetAvailableProfiles();

                ProfileSiteComboBox.Items.Clear();

                foreach (Profile profile in profiles)
                {
                    Debug.WriteLine(profile.WebsiteUrl);
                    ProfileSiteComboBox.DisplayMemberPath = "Key";
                    ProfileSiteComboBox.SelectedValuePath = "Value";

                    ProfileSiteComboBox.Items.Add(new KeyValuePair<string, string>(profile.WebsiteUrl, profile.Id));

                }
                AuthenticationStatusLabel.Text = "Status: Authenticated";

            }
            catch (AggregateException e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
