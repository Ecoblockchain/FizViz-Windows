using FizVizController.Commands;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FizVizController
{
    public sealed partial class FizVizConfig : UserControl
    {
        public FizVizConfig()
        {
            this.InitializeComponent();
            this.InitializeLightingModes();
        }

        public void SetUiEnabled(bool commandsEnabled)
        {
            ViewUtility.SetUiEnabled(ControlGrid, LightingModeButton, commandsEnabled);
        }

        /****************************************************************
         *                       UI Callbacks                           *
         ****************************************************************/

        /// <summary>
        /// Generate and send the display mode to FizViz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LightingModeButton_Click(object sender, RoutedEventArgs e)
        {
            ViewUtility.ResetControlBorders(ControlGrid);

            DisplayMode displayMode;
            switch ((DisplayMode.DisplayModeValue)LightingModeComboBox.SelectedValue)
            {
                case DisplayMode.DisplayModeValue.HotNeedle:
                    {
                        displayMode = GetHotNeedle();
                    }
                    break;
                case DisplayMode.DisplayModeValue.BlockNeedle:
                    {
                        displayMode = GetBlockNeedle();
                    }
                    break;
                case DisplayMode.DisplayModeValue.BackgroundRotate:
                    {
                        displayMode = GetBackgroundRotate();
                    }
                    break;
                case DisplayMode.DisplayModeValue.MinMax:
                    {
                        displayMode = GetMinMax();
                    }
                    break;
                default:
                    return;
            }

            if (displayMode == null) return;

            App.FizViz.SendCommand(displayMode);
        }



        private void LightingModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetParametersDisplayed();
        }

        private void UseHighlight_Checked(object sender, RoutedEventArgs e)
        {
            SetParametersDisplayed();
        }

        private void UseHighlight_Unchecked(object sender, RoutedEventArgs e)
        {
            SetParametersDisplayed();
        }

        /****************************************************************
         *                  Helper functions                            *
         ****************************************************************/

        /// <summary>
        /// Update which parameters are visible based on which mode is selected and on whether we are in highlight mode
        /// </summary>
        private void SetParametersDisplayed()
        {
            bool useHighlight = UseHighlight.IsChecked.HasValue && UseHighlight.IsChecked.Value;

            DisplayMode.DisplayModeValue displayMode = (DisplayMode.DisplayModeValue) LightingModeComboBox.SelectedValue;

            BrightHoldLabel.Visibility = displayMode == DisplayMode.DisplayModeValue.BlockNeedle ? Visibility.Visible : Visibility.Collapsed;
            BrightHoldTextBox.Visibility = displayMode == DisplayMode.DisplayModeValue.BlockNeedle ? Visibility.Visible : Visibility.Collapsed;

            PixelOffsetLabel.Visibility = displayMode == DisplayMode.DisplayModeValue.BackgroundRotate
                ? Visibility.Visible
                : Visibility.Collapsed;
            PixelOffsetTextBox.Visibility = displayMode == DisplayMode.DisplayModeValue.BackgroundRotate
                ? Visibility.Visible
                : Visibility.Collapsed;
            if (displayMode == DisplayMode.DisplayModeValue.HotNeedle ||
                displayMode == DisplayMode.DisplayModeValue.BlockNeedle)
            {
                HighlightMultiplierLabel.Visibility = useHighlight ? Visibility.Visible : Visibility.Collapsed;
                HighlightMultiplierTextBox.Visibility = useHighlight ? Visibility.Visible : Visibility.Collapsed;
                HotColorLabel.Visibility = useHighlight ? Visibility.Collapsed : Visibility.Visible;
                HotColorComboBox.Visibility = useHighlight ? Visibility.Collapsed : Visibility.Visible;
                UseHighlightLabel.Visibility = Visibility.Visible;
                UseHighlight.Visibility = Visibility.Visible;
            }
            else
            {
                HighlightMultiplierLabel.Visibility = Visibility.Collapsed;
                HighlightMultiplierTextBox.Visibility = Visibility.Collapsed;
                HotColorLabel.Visibility = displayMode == DisplayMode.DisplayModeValue.MinMax ? Visibility.Visible : Visibility.Collapsed;
                HotColorComboBox.Visibility = displayMode == DisplayMode.DisplayModeValue.MinMax ? Visibility.Visible : Visibility.Collapsed;
                UseHighlightLabel.Visibility = Visibility.Collapsed;
                UseHighlight.Visibility = Visibility.Collapsed;
            }
            FadeDurationLabel.Visibility = displayMode == DisplayMode.DisplayModeValue.BackgroundRotate
                ? Visibility.Collapsed
                : Visibility.Visible;
            FadeDurationTextBox.Visibility = displayMode == DisplayMode.DisplayModeValue.BackgroundRotate
                ? Visibility.Collapsed
                : Visibility.Visible;
            FadeDurationLabel.Text = displayMode == DisplayMode.DisplayModeValue.MinMax ? "Min Max Reset (s)" : "Fade Duration (ms)";
        }

        /// <summary>
        /// Set up current lighting modes and default colors
        /// </summary>
        private void InitializeLightingModes()
        {
            LightingModeComboBox.DisplayMemberPath = "Key";
            LightingModeComboBox.SelectedValuePath = "Value";
            LightingModeComboBox.Items.Add(new KeyValuePair<string, DisplayMode.DisplayModeValue>("Hot Needle", DisplayMode.DisplayModeValue.HotNeedle));
            LightingModeComboBox.Items.Add(new KeyValuePair<string, DisplayMode.DisplayModeValue>("Block Needle", DisplayMode.DisplayModeValue.BlockNeedle));
            LightingModeComboBox.Items.Add(new KeyValuePair<string, DisplayMode.DisplayModeValue>("BG Rotate", DisplayMode.DisplayModeValue.BackgroundRotate));
            LightingModeComboBox.Items.Add(new KeyValuePair<string, DisplayMode.DisplayModeValue>("Min Max", DisplayMode.DisplayModeValue.MinMax));
            LightingModeComboBox.SelectedIndex = 0;

            ViewUtility.InitializeColorComboBox(HotColorComboBox);

            // TODO - read current values from FizViz on connect
            HotColorComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Get a block needle command based on the user input
        /// </summary>
        /// <returns></returns>
        private DisplayMode GetBlockNeedle()
        {
            ushort? fadeTime = ViewUtility.GetUShort(FadeDurationTextBox);
            if (!fadeTime.HasValue) return null;

            ushort? holdTime = ViewUtility.GetUShort(BrightHoldTextBox);
            if (!holdTime.HasValue) return null;

            float highlight = 2.0f;
            bool useHighlight = UseHighlight.IsChecked.HasValue && UseHighlight.IsChecked.Value;
            if (!float.TryParse(HighlightMultiplierTextBox.Text, out highlight) && useHighlight)
            {
                HighlightMultiplierTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return null;
            }

            return new BlockNeedleDisplayMode
            {
                HotColor = (Color)HotColorComboBox.SelectedValue,
                FadeTime = fadeTime.Value,
                HoldTime = holdTime.Value,
                UseHighlight = useHighlight,
                HighlightMultiplier = highlight
            };
        }

        /// <summary>
        /// Get a hot needle command based on the user input
        /// </summary>
        /// <returns></returns>
        private DisplayMode GetHotNeedle()
        {
            ushort? fadeTime = ViewUtility.GetUShort(FadeDurationTextBox);
            if (!fadeTime.HasValue) return null;

            float highlight = 2.0f;
            bool useHighlight = UseHighlight.IsChecked.HasValue && UseHighlight.IsChecked.Value;
            if (!float.TryParse(HighlightMultiplierTextBox.Text, out highlight) && useHighlight)
            {
                HighlightMultiplierTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return null;
            }

            return new HotNeedleDisplayMode
            {
                HotColor = (Color)HotColorComboBox.SelectedValue,
                FadeTime = fadeTime.Value,
                UseHighlight = useHighlight,
                HighlightMultiplier = highlight
            };
        }

        private DisplayMode GetBackgroundRotate()
        {
            ushort? offset = ViewUtility.GetUShort(PixelOffsetTextBox);
            if (!offset.HasValue) return null;

            return new BackgroundRotateDisplayMode
            {
                Offset = offset.Value
            };
        }

        private DisplayMode GetMinMax()
        {
            ushort? fadeTime = ViewUtility.GetUShort(FadeDurationTextBox);
            if (!fadeTime.HasValue) return null;

            return new MinMaxDisplayMode
            {
                NeedleColor = (Color)HotColorComboBox.SelectedValue,
                ResetTime = fadeTime.Value
            };
        }

    }
}
