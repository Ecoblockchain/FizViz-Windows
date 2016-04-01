using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;

using System.Threading;
using System.Diagnostics;
using Windows.UI.Xaml;
using System.Collections;

namespace FizVizController
{
    public class AnalyticsManager
    {
        /// <summary>
        /// Number of days worth of historical data to use in calculations
        /// </summary>
        const int HISTORICAL_DAYS = 7;

        /// <summary>
        /// Time period between realtime analytics updates (in seconds)
        /// </summary>
        const int REALTIME_UPDATE_INTERVAL = 5;

        /// <summary>
        /// Time period between regular analytics updates (in seconds);
        /// </summary>
        const int REGULAR_UPDATE_INTERVAL = 60;

        /// <summary>
        /// Regular analytics fields to retrieve
        /// </summary>
        string[] metrics = new string[] { "ga:sessions", "ga:pageviews", "ga:users", "ga:bounceRate", "ga:avgSessionDuration" };

        /// <summary>
        /// Realtime Metrics Bank 1
        /// Note: Multiple banks are needed because Google Analytics does not allow you to retrieve all types of 
        /// realtime metrics at the same time.
        /// </summary>
        string[] realtimeMetrics1 = new string[] { "rt:activeUsers" };

        /// <summary>
        /// Realtime Metrics Bank 2
        /// Note: Multiple banks are needed because Google Analytics does not allow you to retrieve all types of 
        /// realtime metrics at the same time.
        /// </summary>
        string[] realtimeMetrics2 = new string[] { "rt:pageviews" };

        /// <summary>
        /// Google Analytics Profile ID for Data Collection - ie: "ga:XXXXXXX"
        /// </summary>
        public void SetProfileID(string id)
        {
            if (!id.StartsWith("ga:"))
            {
                ProfileID = "ga:" + id;
            }
            else
            {
                ProfileID = id;
            }
        }

        private string ProfileID;
        
        /// <summary>
        /// This delegate is called when an authentication error occurs
        /// </summary>
        /// <param name="error">Text description of error</param>
        public delegate void AuthenticationErrorHandler(string error);

        /// <summary>
        /// This delegate is called when a metrics error occurs
        /// </summary>
        /// <param name="error">Text description of error</param>
        public delegate void MetricsErrorHandler(string error);

        /// <summary>
        /// This delegate is called any time the system updates real time analytics
        /// </summary>
        /// <param name="manager">Convenience pointer back to the manager so you can retrieve data</param>
        public delegate void RealTimeAnalyticsUpdateHandler(AnalyticsManager manager);

        /// <summary>
        /// This delegate is called any time the system updates regular analytics
        /// </summary>
        /// <param name="manager">Convenience pointer back to the manager so you can retrieve data</param>
        public delegate void AnalyticsUpdateHandler(AnalyticsManager manager);

        public event AuthenticationErrorHandler AuthErrorEvent;
        public event MetricsErrorHandler MetricsErrorEvent;
        public event RealTimeAnalyticsUpdateHandler RealTimeUpdateEvent;
        public event AnalyticsUpdateHandler AnalyticsUpdateEvent;

        private AnalyticsService Service { get; set; }
        private UserCredential Credential { get; set; }
        private object DataStore { get; set; }

        private DispatcherTimer realTimeTimer;
        private DispatcherTimer regularTimer;


        public AnalyticsManager()
        {
            // Start our timers
            realTimeTimer = new DispatcherTimer();
            realTimeTimer.Interval = new TimeSpan(0, 0, REALTIME_UPDATE_INTERVAL);
            realTimeTimer.Tick += realTimeTimer_Tick;
            realTimeTimer.Start();

            regularTimer = new DispatcherTimer();
            regularTimer.Interval = new TimeSpan(0, 0, REGULAR_UPDATE_INTERVAL);
            regularTimer.Tick += regularTimer_Tick;
            regularTimer.Start();
        }

        private void realTimeTimer_Tick(object sender, object e)
        {
            if (Service != null && ProfileID != null)
            {
                // Load latest realtime data
                ResetRealtimeData();
                LoadRealtime(realtimeMetrics1);
                LoadRealtime(realtimeMetrics2);

                // Notify
                RealTimeUpdateEvent(this);
            }
        }

        private void regularTimer_Tick(object sender, object e)
        {
            if (Service != null && ProfileID != null)
            {
                // Load analytics data
                LoadToday(metrics);

                // Update historical data
                LoadHistoricalData(HISTORICAL_DAYS, metrics);

                // Notify
                AnalyticsUpdateEvent(this);
            }
        }

        public async Task AuthenticateAsync()
        {
            Service = null;

            try
            {
                Task<UserCredential> task = GoogleWebAuthorizationBroker.AuthorizeAsync(
                     new Uri("ms-appx:///Assets/client-secrets.json"),
                    new[] { Uri.EscapeUriString(AnalyticsService.Scope.Analytics) },
                    "user",
                    CancellationToken.None);

                await task;
                Credential = task.Result;

                // Create the service.
                Service = new AnalyticsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = Credential,
                    ApplicationName = "FizViz",
                });

                
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Debug.WriteLine("ERROR: " + e.Message);
                    AuthErrorEvent(e.Message);
                }
            }


        }

        public async Task LogOutAsync()
        {
            await Credential.RevokeTokenAsync(CancellationToken.None);
            Credential = null;     
        }
    
        public AnalyticDataPoint GetAnalyticsData(string[] dimensions, string[] metrics, DateTime startDate, DateTime endDate)
        {
            AnalyticDataPoint data = new AnalyticDataPoint();
            
            //Make initial call to service.
            //Then check if a next link exists in the response,
            //if so parse and call again using start index param.
            GaData response = null;
            do
            {
                int startIndex = 1;
                if (response != null && !string.IsNullOrEmpty(response.NextLink))
                {
                    Uri uri = new Uri(response.NextLink);
                    var paramerters = uri.Query.Split('&');
                    string s = paramerters.First(i => i.Contains("start-index")).Split('=')[1];
                    startIndex = int.Parse(s);
                }

                try
                {
                    var request = BuildAnalyticRequest(ProfileID, dimensions, metrics, startDate, endDate, startIndex);
                    response = request.Execute();
                }
                catch(Google.GoogleApiException exception)
                {
                    Debug.WriteLine(exception.Error.Message);
                    MetricsErrorEvent(exception.Error.Message);
                }
                data.ColumnHeaders = response.ColumnHeaders;
                data.Rows.AddRange(response.Rows);

            } while (!string.IsNullOrEmpty(response.NextLink));

            return data;
        }

        public RealtimeDataPoint GetRealtimeData(string[] dimensions, string[] metrics)
        {
            RealtimeDataPoint data = new RealtimeDataPoint();

            //Make initial call to service.
            //Then check if a next link exists in the response,
            //if so parse and call again using start index param.
            RealtimeData response = null;

            var request = BuildRealtimeRequest(ProfileID, dimensions, metrics);
            response = request.Execute();
            data.ColumnHeaders = response.ColumnHeaders;

            if (response.Rows != null)
                data.Rows.AddRange(response.Rows);
            else
                data.Rows.Add(new string[] { "0" });

            return data;
        }

        private DataResource.GaResource.GetRequest BuildAnalyticRequest(string profileId, string[] dimensions, string[] metrics,
                                                                            DateTime startDate, DateTime endDate, int startIndex)
        {
            DataResource.GaResource.GetRequest request = Service.Data.Ga.Get(profileId, startDate.ToString("yyyy-MM-dd"),
                                                                                endDate.ToString("yyyy-MM-dd"), string.Join(",", metrics));

            if (dimensions != null)
                request.Dimensions = string.Join(",", dimensions);
            request.StartIndex = startIndex;
            return request;
        }

        private DataResource.RealtimeResource.GetRequest BuildRealtimeRequest(string profileId, string[] dimensions, string[] metrics)
        {

            DataResource.RealtimeResource.GetRequest request = Service.Data.Realtime.Get(profileId, string.Join(",", metrics));

            if (dimensions != null)
                request.Dimensions = string.Join(",", dimensions);

            return request;
        }

        public IList<Profile> GetAvailableProfiles()
        {
            var response = Service.Management.Profiles.List("~all", "~all").Execute();
            return response.Items;
        }

        public class AnalyticDataPoint
        {
            public AnalyticDataPoint()
            {
                Rows = new List<IList<string>>();
            }

            public IList<GaData.ColumnHeadersData> ColumnHeaders { get; set; }
            public List<IList<string>> Rows { get; set; }
        }

        public class RealtimeDataPoint
        {
            public RealtimeDataPoint()
            {
                Rows = new List<IList<string>>();
            }

            public IList<RealtimeData.ColumnHeadersData> ColumnHeaders { get; set; }
            public List<IList<string>> Rows { get; set; }
        }


        private AnalyticDataPoint historicalData;
        private int historyDays = 0;
        private AnalyticDataPoint todayData;
        private ArrayList realtimeDataPoints;

        /// <summary>
        /// Load historical metrics data for a specified timeframe
        /// </summary>
        /// <param name="numberOfDays">Number of days to load</param>
        /// <param name="metrics">Array of metrics to load</param>
        public void LoadHistoricalData(int numberOfDays, string[] metrics)
        {
            DateTime startDate = DateTime.Today.AddDays(-numberOfDays);
            DateTime endDate = DateTime.Today.AddSeconds(-1);

            historicalData = GetAnalyticsData(null, metrics, startDate, endDate);
            historyDays = numberOfDays;
        }

        /// <summary>
        /// Load snapshot of today's metrics data
        /// </summary>
        /// <param name="metrics">Array of metric names to load</param>
        private void LoadToday(string[] metrics)
        {
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(1).AddSeconds(-1);

            todayData = GetAnalyticsData(null, metrics, startDate, endDate);
        }

        /// <summary>
        /// load a realtime data point
        /// </summary>
        /// <param name="metrics">Array of real time metric names to load</param>
        private void LoadRealtime(string[] metrics)
        {
            realtimeDataPoints.Add(GetRealtimeData(null, metrics));
        }

        private void ResetRealtimeData()
        {
            realtimeDataPoints = new ArrayList();
        }

        /// <summary>
        /// Retrieve the historical average value for a metric
        /// </summary>
        /// <param name="metric">Name of metric to retrieve</param>
        /// <returns>Average value of the metric</returns>
        public float getHistoricalAverage(string metric)
        {
            float avg = 0.0f;

            // Find the metric
            int c = 0;
            foreach(GaData.ColumnHeadersData header in historicalData.ColumnHeaders)
            {
                if (header.Name == metric)
                {
                    string strval = historicalData.Rows[0][c];
                    float floatVal = float.Parse(strval);
                    avg = floatVal / historyDays;
                    break;
                }
                ++c;
            }

            return avg;
        }

        /// <summary>
        /// Retrieve the historical value for a metric
        /// </summary>
        /// <param name="metric">Name of metric to retrieve</param>
        /// <returns>Average value of the metric</returns>
        public float getHistoricalValue(string metric)
        {
            float val = 0.0f;

            // Find the metric
            int c = 0;
            foreach (GaData.ColumnHeadersData header in historicalData.ColumnHeaders)
            {
                if (header.Name == metric)
                {
                    string strval = historicalData.Rows[0][c];
                    val = float.Parse(strval);
                    
                    break;
                }
                ++c;
            }

            return val;
        }

        public float getTodayValue(string metric)
        {
            float val = 0.0f;

            // Find the metric
            int c = 0;
            foreach (GaData.ColumnHeadersData header in todayData.ColumnHeaders)
            {
                if (header.Name == metric)
                {
                    string strval = todayData.Rows[0][c];
                    val = float.Parse(strval);

                    break;
                }
                ++c;
            }

            return val;
        }

        public float getRealtimeValue(string metric)
        {
            float val = 0.0f;


            foreach (RealtimeDataPoint realtimeData in realtimeDataPoints)
            {
                // Find the metric
                int c = 0;

                bool done = false;

                foreach (RealtimeData.ColumnHeadersData header in realtimeData.ColumnHeaders)
                {
                    if (header.Name == metric)
                    {
                        string strval = realtimeData.Rows[0][c];
                        val = float.Parse(strval);
                        done = true;

                        break;
                    }
                    ++c;
                }

                if (done)
                    break;
            }

            return val;
        }
    }
}
