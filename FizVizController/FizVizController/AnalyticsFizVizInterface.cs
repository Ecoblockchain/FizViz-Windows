using System;
using System.Diagnostics;
using FizVizController.Commands;

namespace FizVizController
{
    /// <summary>
    /// Convert analytics data into FizViz commands
    /// </summary>
    public class AnalyticsFizVizInterface
    {
        public AnalyticsFizVizInterface(AnalyticsManager analytics)
        {
            DisplayAngleMinimum = 0;
            DisplayAngleMaximum = 360;
            MetricMinimum = 0;
            MetricMaximum = 1000;
            Metric = "";

            analytics.AnalyticsUpdateEvent += AnalyticsManager_AnalyticsUpdateEvent;
            analytics.RealTimeUpdateEvent += AnalyticsManager_RealTimeUpdateEvent;
        }

        public string Metric { get; set; }
        public float MetricMinimum { get; set; }
        public float MetricMaximum { get; set; }

        public float DisplayAngleMinimum
        {
            get
            {
                return displayAngleMinimum;
            }
            set
            {
                displayAngleMinimum = value;
                displayPositionMinimum = value*angleToPositionRatio;
            }
        }
        public float DisplayAngleMaximum
        {
            get
            {
                return displayAngleMaximum;
            }
            set
            {
                displayAngleMaximum = value;
                displayPositionMaximum = value * angleToPositionRatio;
            }
        }

        private float displayAngleMinimum;
        private float displayAngleMaximum;
        private float displayPositionMinimum;
        private float displayPositionMaximum;
        private const float angleToPositionRatio = (NeedlePosition.MAXIMUM_POSITION - 1)/360f;

        private void AnalyticsManager_RealTimeUpdateEvent(AnalyticsManager analytics)
        {
            if (!Metric.StartsWith("rt:")) return;

            float value = analytics.getRealtimeValue(Metric);

            SimplePosition(value);
        }

        private void AnalyticsManager_AnalyticsUpdateEvent(AnalyticsManager analytics)
        {
            if (!Metric.StartsWith("ga:")) return;

            float value = analytics.getRealtimeValue(Metric);

            SimplePosition(value);
        }

        /// <summary>
        /// Do a simple conversion of current metric value to needle position, and update the FizViz accordingly
        /// </summary>
        /// <param name="value"></param>
        private void SimplePosition(float value)
        {
            value = Math.Max(value, MetricMinimum);
            value = Math.Min(value, MetricMaximum);

            uint needlePosition = (uint) ((((value - MetricMinimum) / (MetricMaximum - MetricMinimum)) * (displayPositionMaximum - displayPositionMinimum)) + displayPositionMinimum);

            FizVizCommand positionCommand = new NeedlePosition
            {
                Direction = NeedlePosition.NeedleDirectionValue.DoNotPassZero,
                Position = needlePosition
            };

            Debug.WriteLine("Set position to " + needlePosition);
            if (App.FizViz != null)
            {
                App.FizViz.SendCommand(positionCommand);
            }
        }

    }
}