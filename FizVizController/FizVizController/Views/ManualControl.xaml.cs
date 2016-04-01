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
        public ManualControl()
        {
            this.InitializeComponent();
            InitializeNeedleDirection();
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
