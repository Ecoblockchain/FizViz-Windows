using FizVizController.Commands;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FizVizController
{
    public sealed partial class ManualControl : UserControl
    {
        private DispatcherTimer randomPositionTimer;
        private Random random = new Random();

        public ManualControl()
        {
            this.InitializeComponent();
            InitializeNeedleDirection();
            randomPositionTimer = null;
        }

        /****************************************************************
         *                       UI Callbacks                           *
         ****************************************************************/

        /// <summary>
        /// Generate and send a manual needle position command to FizViz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NeedlePositionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewUtility.ResetControlBorders(ControlGrid);

            ushort? position = ViewUtility.GetUShort(NeedlePositionTextBox);
            if (!position.HasValue) return;

            NeedlePosition positionCommand = new NeedlePosition
            {
                Position = position.Value,
                Direction = (NeedlePosition.NeedleDirectionValue)NeedleDirectionComboBox.SelectedValue
            };
            App.FizViz.SendCommand(positionCommand);
        }

        private void RandomButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (randomPositionTimer == null)
            {
                randomPositionTimer = new DispatcherTimer();
                randomPositionTimer.Interval = new TimeSpan(0, 0, 1);
                randomPositionTimer.Tick += RandomPositionTimerOnTick;
                randomPositionTimer.Start();
            }
            else
            {
                randomPositionTimer.Stop();
                randomPositionTimer = null;
            }
        }

        private void RandomPositionTimerOnTick(object sender, object o)
        {
            int? min = ViewUtility.GetInt(MinRandomTextBox);
            int? max = ViewUtility.GetInt(MaxRandomTextBox);
            if (!min.HasValue || !max.HasValue) return;
            int position = random.Next(min.Value, max.Value);
            NeedlePosition positionCommand = new NeedlePosition
            {
                Position = (uint)position,
                Direction = NeedlePosition.NeedleDirectionValue.DoNotPassZero
            };
            App.FizViz.SendCommand(positionCommand);
        }

        /****************************************************************
         *                  Helper functions                            *
         ****************************************************************/

        public void SetUiEnabled(bool commandsEnabled)
        {
            ViewUtility.SetUiEnabled(ControlGrid, NeedlePositionButton, commandsEnabled);
        }

        private void InitializeNeedleDirection()
        {
            NeedleDirectionComboBox.DisplayMemberPath = "Key";
            NeedleDirectionComboBox.SelectedValuePath = "Value";
            NeedleDirectionComboBox.Items.Add(new KeyValuePair<string, NeedlePosition.NeedleDirectionValue>("Clockwise", NeedlePosition.NeedleDirectionValue.Clockwise));
            NeedleDirectionComboBox.Items.Add(new KeyValuePair<string, NeedlePosition.NeedleDirectionValue>("Counter-Clockwise", NeedlePosition.NeedleDirectionValue.CounterClockwise));
            NeedleDirectionComboBox.Items.Add(new KeyValuePair<string, NeedlePosition.NeedleDirectionValue>("Closest", NeedlePosition.NeedleDirectionValue.Closest));
            NeedleDirectionComboBox.Items.Add(new KeyValuePair<string, NeedlePosition.NeedleDirectionValue>("Do Not Cross Zero", NeedlePosition.NeedleDirectionValue.DoNotPassZero));
            NeedleDirectionComboBox.SelectedIndex = 0;
        }


    }
}
