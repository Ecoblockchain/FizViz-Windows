using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI;

namespace FizVizController.Commands
{
    internal class HotNeedleDisplayMode : DisplayMode
    {
        public HotNeedleDisplayMode()
        {
            mode = Convert.ToByte(DisplayModeValue.HotNeedle);
        }

        /****************************************************************
         *                  Public Properties                           *
         ****************************************************************/

        public Color HotColor
        {
            get; set;
        }

        public uint FadeTime
        {
            get; set;
        }

        public bool UseHighlight
        {
            get; set;
        }

        public float HighlightMultiplier
        {
            get; set;
        }

        /****************************************************************
         *                  Data Access Methods                         *
         ****************************************************************/

        public override IBuffer DataBuffer()
        {
            byte[] data = new byte[18];

            data[0] = Mode;
            WriteColorBytes(data, HotColor, 1);
            data[7] = Convert.ToByte(UseHighlight);
            WriteFloat(data, HighlightMultiplier, 8);
            WriteTwoBytes(data, FadeTime, 16);
            return data.AsBuffer();
        }
    }
}
