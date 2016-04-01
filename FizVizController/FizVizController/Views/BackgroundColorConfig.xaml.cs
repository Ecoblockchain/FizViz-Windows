using FizVizController.Commands;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace FizVizController
{
    public sealed partial class BackgroundColorConfig : UserControl
    {
        private enum BackgroundColorMode
        {
            Solid,
            Blend,
            Segment
        };

        public BackgroundColorConfig()
        {
            this.InitializeComponent();
            ViewUtility.InitializeColorComboBox(Color1ComboBox);
            ViewUtility.InitializeColorComboBox(Color2ComboBox);
            ViewUtility.InitializeColorComboBox(Color3ComboBox);
            ViewUtility.InitializeColorComboBox(Color1ComboBox_Gradient);

            ColorModeComboBox.DisplayMemberPath = "Key";
            ColorModeComboBox.SelectedValuePath = "Value";
            ColorModeComboBox.Items.Add(new KeyValuePair<string, BackgroundColorMode>("Solid", BackgroundColorMode.Solid));
            ColorModeComboBox.Items.Add(new KeyValuePair<string, BackgroundColorMode>("Blended", BackgroundColorMode.Blend));
            ColorModeComboBox.Items.Add(new KeyValuePair<string, BackgroundColorMode>("Segmented", BackgroundColorMode.Segment));
            ColorModeComboBox.SelectedIndex = 0;

            UpdateParameterVisibility();
        }

        public void SetUiEnabled(bool commandsEnabled)
        {
            ViewUtility.SetUiEnabled(ControlGrid, BackgroundColorButton, commandsEnabled);
        }

        /****************************************************************
         *                       UI Callbacks                           *
         ****************************************************************/

        private void ColorModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateParameterVisibility();
        }

        private void BackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            ViewUtility.ResetControlBorders(ControlGrid);

            BackgroundColor command = new BackgroundColor();

            Color[] colors = new Color[FizVizCommand.NEOPIXEL_COUNT];
            for (int i = 0; i < FizVizCommand.NEOPIXEL_COUNT; i++)
            {
                colors[i] = Color.FromArgb(0, 0, 0, 0);
            }

            switch ((BackgroundColorMode)ColorModeComboBox.SelectedValue)
            {
                case BackgroundColorMode.Solid:
                    {
                        if (!GetSolid(colors)) return;
                    }
                    break;
                case BackgroundColorMode.Blend:
                    {
                        if (!GetBlend(colors)) return;
                    }
                    break;
                case BackgroundColorMode.Segment:
                    {
                        if (!GetSegment(colors)) return;
                    }
                    break;
            }

            command.BackgroundColors = colors;
            App.FizViz.SendCommand(command);
        }

        /****************************************************************
         *                  Helper functions                            *
         ****************************************************************/

        /// <summary>
        /// Generate an array of background colors that are a blend betwen the
        /// user specified colors, at the specified angles
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        private bool GetBlend(IList<Color> colors)
        {
            double angle1, angle2, angle3;
            int color1Pixel, color2Pixel, color3Pixel;
            if (Color1ComboBox_Gradient.SelectedValue == null)
            {
                Color1ComboBox_Gradient.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
            if (Color2ComboBox.SelectedValue == null)
            {
                Color2ComboBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
            if (Color3ComboBox.SelectedValue == null)
            {
                Color3ComboBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
            if (!ReadAngle(Angle1TextBox, out angle1))
                return false;
            if (!ReadAngle(Angle2TextBox, out angle2))
                return false;
            if (!ReadAngle(Angle3TextBox, out angle3))
                return false;

            if (angle1 >= angle2)
            {
                Angle1TextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                Angle2TextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }

            if (angle2 >= angle3)
            {
                Angle2TextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                Angle3TextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }

            color1Pixel = PixelForAngle(angle1);
            color2Pixel = PixelForAngle(angle2);
            color3Pixel = PixelForAngle(angle3);
            if (color3Pixel == 0)
            {
                color3Pixel = BackgroundColor.NEOPIXEL_COUNT - 1;
            }

            Color color1 = (Color) Color1ComboBox_Gradient.SelectedValue;
            Color color2 = (Color) Color2ComboBox.SelectedValue;
            Color color3 = (Color) Color3ComboBox.SelectedValue;

            int diffSize = color2Pixel - color1Pixel;
            for (int i = 0; i < diffSize; i++)
            {
                colors[color1Pixel + i] = Blend(i, diffSize, color1, color2);
            }

            diffSize = color3Pixel - color2Pixel;
            for (int i = 0; i < diffSize; i++)
            {
                colors[color2Pixel + i] = Blend(i, diffSize, color2, color3);
            }
            colors[color3Pixel] = color3;
            return true;
        }

        /// <summary>
        /// Generate an array of background colors that is split into three segments,
        /// one per user specified color
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        private bool GetSegment(IList<Color> colors)
        {
            double angle1, angle2, angle3;
            int color1Pixel, color2Pixel, color3Pixel;
            if (Color1ComboBox_Gradient.SelectedValue == null)
            {
                Color1ComboBox_Gradient.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
            if (Color2ComboBox.SelectedValue == null)
            {
                Color2ComboBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
            if (Color3ComboBox.SelectedValue == null)
            {
                Color3ComboBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
            if (!ReadAngle(Angle1TextBox, out angle1))
                return false;
            if (!ReadAngle(Angle2TextBox, out angle2))
                return false;
            if (!ReadAngle(Angle3TextBox, out angle3))
                return false;

            if (angle1 >= angle2)
            {
                Angle1TextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                Angle2TextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }

            if (angle2 >= angle3)
            {
                Angle2TextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                Angle3TextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }

            color1Pixel = PixelForAngle(angle1);
            color2Pixel = PixelForAngle(angle2);
            color3Pixel = PixelForAngle(angle3);
            if (color3Pixel == 0)
            {
                color3Pixel = BackgroundColor.NEOPIXEL_COUNT - 1;
            }

            Color color1 = (Color)Color1ComboBox_Gradient.SelectedValue;
            Color color2 = (Color)Color2ComboBox.SelectedValue;
            Color color3 = (Color)Color3ComboBox.SelectedValue;

            for (int i = 0; i < BackgroundColor.NEOPIXEL_COUNT; i++)
            {
                colors[i] = color3;
            }
            for (int i = color1Pixel; i < color2Pixel; i++)
            {
                colors[i] = color1;
            }
            for (int i = color2Pixel; i < color3Pixel; i++)
            {
                colors[i] = color2;
            }
            
            return true;
        }

        /// <summary>
        /// Generate an array of background colors that is all a single color
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        private bool GetSolid(IList<Color> colors)
        {
            if (Color1ComboBox.SelectedValue == null)
            {
                Color1ComboBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
            Color color1 = (Color) Color1ComboBox.SelectedValue;

            double angle1Start, angle1End;

            if (!ReadAngles(StartAngle1TextBox, EndAngle1TextBox, out angle1Start, out angle1End))
                return false;
            int color1StartPixel, color1EndPixel;
            PixelsForAngles(angle1Start, angle1End, out color1StartPixel, out color1EndPixel);

            for (int i = color1StartPixel; i <= color1EndPixel; i++)
            {
                colors[i] = color1;
            }
            return true;
        }


        private void UpdateParameterVisibility()
        {
            bool showAllColorRows = (BackgroundColorMode)ColorModeComboBox.SelectedValue == BackgroundColorMode.Blend || (BackgroundColorMode)ColorModeComboBox.SelectedValue == BackgroundColorMode.Segment;
            foreach (FrameworkElement e in ControlGrid.Children.OfType<FrameworkElement>())
            {
                int row = Grid.GetRow(e);
                if (row == 1)
                {
                    e.Visibility = showAllColorRows ? Visibility.Collapsed : Visibility.Visible;
                }
                else if (row > 1)
                {
                    e.Visibility = showAllColorRows ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private bool ReadAngle(TextBox angleTextBox, out double angle)
        {
            if (!double.TryParse(angleTextBox.Text, out angle) || angle < 0 || angle > 360)
            {
                angleTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
            return true;
        }

        private bool ReadAngles(TextBox angleTextBox1, TextBox angleTextBox2, out double angleStart, out double angleEnd)
        {
            if (!ReadAngle(angleTextBox1, out angleStart))
            {
                angleEnd = 0;
                return false;
            }
            if (!ReadAngle(angleTextBox2, out angleEnd))
            {
                return false;
            }
            if (angleStart >= angleEnd)
            {
                angleTextBox1.BorderBrush = new SolidColorBrush(Colors.Red);
                angleTextBox2.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
            return true;
        }

        private static int PixelForAngle(double angle)
        {
            return (int)(angle / 360.0 * BackgroundColor.NEOPIXEL_COUNT) % BackgroundColor.NEOPIXEL_COUNT;
        }

        private static void PixelsForAngles(double angleStart, double angleEnd, out int pixelStart, out int pixelEnd)
        {
            pixelStart = PixelForAngle(angleStart);
            pixelEnd = PixelForAngle(angleEnd);
            if (pixelEnd == 0)
            {
                pixelEnd = FizVizCommand.NEOPIXEL_COUNT - 1;
            }
        }

        private static Color Blend(int i, int range, Color color1, Color color2)
        {
            float ratioColor1 = (float)(range - i) / range;
            float ratioColor2 = (float)i / range;

            return Color.FromArgb(1, (byte)(color1.R * ratioColor1 + color2.R * ratioColor2), (byte)(color1.G * ratioColor1 + color2.G * ratioColor2), (byte)(color1.B * ratioColor1 + color2.B * ratioColor2));
        }
    }
}
