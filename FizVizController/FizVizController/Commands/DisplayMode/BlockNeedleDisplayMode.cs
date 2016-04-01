using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace FizVizController.Commands
{
    internal class BlockNeedleDisplayMode : HotNeedleDisplayMode
    {
        public BlockNeedleDisplayMode()
        {
            mode = Convert.ToByte(DisplayModeValue.BlockNeedle);
        }

        /****************************************************************
         *                  Public Properties                           *
         ****************************************************************/

        public uint HoldTime
        {
            get; set;
        }

        /****************************************************************
         *                  Data Access Methods                         *
         ****************************************************************/

        public override IBuffer DataBuffer()
        {
            byte[] data = new byte[20];

            Array.Copy(base.DataBuffer().ToArray(), data, 18);

            WriteTwoBytes(data, HoldTime, 18);

            return data.AsBuffer();
        }
    }
}
