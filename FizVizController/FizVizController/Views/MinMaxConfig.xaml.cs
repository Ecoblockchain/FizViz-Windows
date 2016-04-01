using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FizVizController.Commands;
using Windows.UI;

namespace FizVizController
{
    public sealed partial class MinMaxConfig : UserControl
    {
        public MinMaxConfig()
        {
            this.InitializeComponent();
            // Fill in some default colors
            ViewUtility.InitializeColorComboBox(MinColorComboBox);
            ViewUtility.InitializeColorComboBox(MaxColorComboBox);
            MinColorComboBox.SelectedIndex = 0;
            MaxColorComboBox.SelectedIndex = 0;
        }

        public void SetUiEnabled(bool commandsEnabled)
        {
            ViewUtility.SetUiEnabled(ControlGrid, MinMaxButton, commandsEnabled);
        }


        /****************************************************************
         *                       UI Callbacks                           *
         ****************************************************************/
        
        /// <summary>
        /// Generate and send a min max configuration message to FizViz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinMaxButton_Click(object sender, RoutedEventArgs e)
        {
            ViewUtility.ResetControlBorders(ControlGrid);

            ushort? reset = ViewUtility.GetUShort(ResetTextBox);
            if (!reset.HasValue) return;

            MinMaxCommand command = new MinMaxCommand
            {
                MinColor = (Color)MinColorComboBox.SelectedValue,
                MaxColor = (Color)MaxColorComboBox.SelectedValue,
                DisplayMin = DisplayMin.IsChecked.HasValue && DisplayMin.IsChecked.Value,
                DisplayMax = DisplayMax.IsChecked.HasValue && DisplayMax.IsChecked.Value,
                ResetDelay = reset.Value
            };

            App.FizViz.SendCommand(command);
        }
    }


}
