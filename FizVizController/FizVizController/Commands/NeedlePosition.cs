using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace FizVizController.Commands
{
    internal class NeedlePosition : FizVizCommand
    {
        public enum NeedleDirectionValue : byte
        {
            Clockwise = 0,
            CounterClockwise = 1,
            Closest = 2,
            DoNotPassZero = 3
        };

        public const int MAXIMUM_POSITION = 1200;

        /****************************************************************
         *                  Public Properties                           *
         ****************************************************************/

        public uint Position
        {
            get; set;
        }

        public NeedleDirectionValue Direction
        {
            get; set;
        }

        /****************************************************************
         *                  Data Access Methods                         *
         ****************************************************************/

        public override byte Command => 0x41;

        public override IBuffer DataBuffer()
        {
            byte[] data = new byte[3];

            WriteTwoBytes(data, Position, 0);
            data[2] = Convert.ToByte(Direction);

            return data.AsBuffer();
        }

    }
}
