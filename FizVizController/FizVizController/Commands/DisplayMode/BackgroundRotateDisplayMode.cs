using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI;

namespace FizVizController.Commands
{
    internal class BackgroundRotateDisplayMode : DisplayMode
    {
        public BackgroundRotateDisplayMode()
        {
            mode = Convert.ToByte(DisplayModeValue.BackgroundRotate);
        }

        /****************************************************************
         *                  Public Properties                           *
         ****************************************************************/

        public uint Offset
        {
            get; set;
        }

        /****************************************************************
         *                  Data Access Methods                         *
         ****************************************************************/

        public override IBuffer DataBuffer()
        {
            byte[] data = new byte[3];

            data[0] = Mode;
            WriteTwoBytes(data, Offset, 1);
            return data.AsBuffer();
        }
    }
}
