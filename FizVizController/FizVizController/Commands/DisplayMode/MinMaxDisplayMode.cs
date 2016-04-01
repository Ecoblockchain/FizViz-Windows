using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI;

namespace FizVizController.Commands
{
    internal class MinMaxDisplayMode : DisplayMode
    {
        public MinMaxDisplayMode()
        {
            mode = Convert.ToByte(DisplayModeValue.MinMax);
        }

        /****************************************************************
         *                  Public Properties                           *
         ****************************************************************/

        /// <summary>
        /// In seconds
        /// </summary>
        public uint ResetTime
        {
            get; set;
        }

        public Color NeedleColor { get; set; }


        /****************************************************************
         *                  Data Access Methods                         *
         ****************************************************************/

        public override IBuffer DataBuffer()
        {
            byte[] data = new byte[9];

            data[0] = Mode;
            WriteTwoBytes(data, ResetTime, 1);
            WriteColorBytes(data, NeedleColor, 3);
            return data.AsBuffer();
        }
    }
}
