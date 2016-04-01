using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace FizVizController
{
    internal class ViewUtility
    {
        /// <summary>
        /// Helper function - given a ComboBox, set it up with a variety of default color options.
        /// </summary>
        /// <param name="comboBox"></param>
        public static void InitializeColorComboBox(ComboBox comboBox)
        {
            comboBox.DisplayMemberPath = "Key";
            comboBox.SelectedValuePath = "Value";
            comboBox.Items.Add(new KeyValuePair<string, Color>("Red", Color.FromArgb(255, 180, 0, 0)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Green", Color.FromArgb(255, 0, 180, 0)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Blue", Color.FromArgb(255, 0, 0, 180)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("White", Color.FromArgb(255, 180, 180, 180)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Yellow", Color.FromArgb(255, 180, 180, 0)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Purple", Color.FromArgb(255, 180, 0, 180)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Teal", Color.FromArgb(255, 0, 180, 180)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Black", Color.FromArgb(255, 0, 0, 0)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Bright Yellow", Color.FromArgb(255, 255, 255, 0)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Bright Blue", Color.FromArgb(255, 0, 0, 255)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Bright Green", Color.FromArgb(255, 0, 255, 0)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Bright Red", Color.FromArgb(255, 255, 0, 0)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Dim Blue", Color.FromArgb(255, 0, 0, 90)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Dim Green", Color.FromArgb(255, 0, 90, 0)));
            comboBox.Items.Add(new KeyValuePair<string, Color>("Dim Red", Color.FromArgb(255, 90, 0, 0)));
        }


        /// <summary>
        /// Helper function - given a Panel, and an optional Button, set any Control children of the Panel
        /// and the Button (if it isn't null) to be enabled or disabled
        /// </summary>
        /// <param name="controlPanel"></param>
        /// <param name="button"></param>
        /// <param name="commandsEnabled"></param>
        public static void SetUiEnabled(Panel controlPanel, Button button, bool commandsEnabled)
        {
            foreach (Control c in controlPanel.Children.OfType<Control>())
            {
                c.IsEnabled = commandsEnabled;
            }
            if (button != null)
            {
                button.IsEnabled = commandsEnabled;
            }
        }

        /// <summary>
        /// Helper function - given a Panel, set the border of any children of the Panel to be gray
        /// </summary>
        /// <param name="controlPanel"></param>
        public static void ResetControlBorders(Panel controlPanel)
        {
            foreach (Control c in controlPanel.Children.OfType<Control>())
            {
                c.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Gray);
            }
        }

        /// <summary>
        /// Given a textbox, attempt to parse out and return an int.
        /// If parse fails, set the textbox border to red and return null.
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        public static int? GetInt(TextBox textBox)
        {
            int value;
            if (int.TryParse(textBox.Text, out value)) return value;
            textBox.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Red);
            return null;
        }

        /// <summary>
        /// Given a textbox, attempt to parse out and return a ushort.
        /// If parse fails, set the textbox border to red and return null.
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        public static ushort? GetUShort(TextBox textBox)
        {
            ushort value;
            if (ushort.TryParse(textBox.Text, out value)) return value;
            textBox.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Red);
            return null;
        }

        /// <summary>
        /// Given a textbox, attempt to parse out and return a float.
        /// If parse fails, set the textbox border to red and return null.
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        public static float? GetFloat(TextBox textBox)
        {
            float value;
            if (float.TryParse(textBox.Text, out value)) return value;
            textBox.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Red);
            return null;
        }
    }
}
