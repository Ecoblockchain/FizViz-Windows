using System;
using Windows.Storage.Streams;

namespace FizVizController.Commands
{
    public abstract class FizVizCommand
    {
        public abstract byte Command
        {
            get;
        }

        /// <summary>
        /// This retrieves the data to be sent to the FizViz.
        /// Call PrepareToSendData() to initialize for transmission.
        /// If the data cannot be sent in one transmission, a call to DataBuffer() will update internal state
        /// such that the next call to DataBuffer() will retrieve the next set of data to be sent.
        /// TransmissionComplete() will return true after all data has been iterated.
        /// </summary>
        /// <returns></returns>
        public abstract IBuffer DataBuffer();

        /// <summary>
        /// Override this if there is setup required before sending a command
        /// </summary>
        public virtual void PrepareToSendData()
        {
        }

        /// <summary>
        /// Returns true if the command has been sent in full.
        /// If this returns false, the caller should continue retrieving data from DataBuffer()
        /// and sending it.
        /// </summary>
        /// <returns></returns>
        public virtual bool TransmissionComplete()
        {
            return true;
        }

        /****************************************************************
         *                  Helper Methods                              *
         ****************************************************************/

        /// <summary>
        /// Write a color object to the given data array starting at offset
        /// </summary>
        /// <param name="data"></param>
        /// <param name="c"></param>
        /// <param name="offset"></param>
        protected void WriteColorBytes(byte[] data, Windows.UI.Color c, uint offset)
        {
            data[offset++] = Convert.ToByte(c.R & 0x7F);
            data[offset++] = Convert.ToByte((c.R >> 7) & 0x7F);
            data[offset++] = Convert.ToByte(c.G & 0x7F);
            data[offset++] = Convert.ToByte((c.G >> 7) & 0x7F);
            data[offset++] = Convert.ToByte(c.B & 0x7F);
            data[offset] = Convert.ToByte((c.B >> 7) & 0x7F);
        }

        /// <summary>
        /// Write a number as two bytes to the data array starting at offset
        /// The number must fit in 14 bits.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        protected void WriteTwoBytes(byte[] data, uint value, uint offset)
        {
            data[offset++] = Convert.ToByte(value & 0x7F);
            data[offset] = Convert.ToByte((value >> 7) & 0x7F);
        }

        /// <summary>
        /// Write a float into 8 bytes in the data array starting at offset
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        protected void WriteFloat(byte[] data, float value, uint offset)
        {
            byte[] floatBytes = BitConverter.GetBytes(value);
            for (uint i = 0; i < 4; i++)
            {
                WriteTwoBytes(data, (uint)floatBytes[i], offset + (i * 2));
            }
        }

        // TODO - add a command to retrieve this from the FizViz
        public const int NEOPIXEL_COUNT = 133;

        protected const int COLOR_BYTE_COUNT = 6;
    }
}
