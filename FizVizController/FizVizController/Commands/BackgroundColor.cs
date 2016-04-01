using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI;

namespace FizVizController.Commands
{
    internal class BackgroundColor : FizVizCommand
    {
        public override byte Command => 0x43;

        /****************************************************************
         *                  Public Properties                           *
         ****************************************************************/

        public Color[] BackgroundColors
        {
            get; set;
        }


        /****************************************************************
         *                  Data Access Methods                         *
         ****************************************************************/


        public override void PrepareToSendData()
        {
            offset = 0;
        }

        public override bool TransmissionComplete()
        {
            return offset >= NEOPIXEL_COUNT;
        }

        public override IBuffer DataBuffer()
        {
            // The most data that can be transmitted is 64 bytes.  1 byte is used for command, 2 for start pixel, 1 for pixel transmission count. => at most 10 pixels per transmission.
            byte transmitPixelsCount = Math.Min((byte)10, Convert.ToByte(NEOPIXEL_COUNT - offset));

            byte[] data = new byte[3 + COLOR_BYTE_COUNT * transmitPixelsCount];

            WriteTwoBytes(data, offset, 0);
            data[2] = transmitPixelsCount;

            for (uint i = 0; i < transmitPixelsCount; i++)
            {
                WriteColorBytes(data, BackgroundColors[offset + i], 3 + (i * COLOR_BYTE_COUNT));
            }

            offset += transmitPixelsCount;

            return data.AsBuffer();
        }


        private uint offset;

    }
}
