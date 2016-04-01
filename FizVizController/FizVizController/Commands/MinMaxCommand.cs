using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI;

namespace FizVizController.Commands
{
    internal class MinMaxCommand : FizVizCommand
    {
        /****************************************************************
         *                  Public Properties                           *
         ****************************************************************/

        public Color MinColor { get; set; }
        public Color MaxColor { get; set; }
        public bool DisplayMin { get; set; }
        public bool DisplayMax { get; set; }
        public ushort ResetDelay { get; set; }

        /****************************************************************
         *                  Data Access Methods                         *
         ****************************************************************/
        public override byte Command => 0x42;

        public override IBuffer DataBuffer()
        {
            byte[] data = new byte[16];

            data[0] = Convert.ToByte(DisplayMin);
            data[1] = Convert.ToByte(DisplayMax);
            WriteColorBytes(data, MinColor, 2);
            WriteColorBytes(data, MaxColor, 8);
            WriteTwoBytes(data, ResetDelay, 14);

            return data.AsBuffer();
        }
    }
}
