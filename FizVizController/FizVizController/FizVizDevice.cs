using System;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using FizVizController.Commands;

namespace FizVizController
{
    public delegate void DeviceReadyHandler();
    public delegate void DeviceConnectFailedHandler(String message);
    public delegate void DeviceConnectionLostHandler(String message);

    public class FizVizDevice
    {
        public event DeviceReadyHandler DeviceReady;
        public event DeviceConnectionLostHandler ConnectionLost;
        public event DeviceConnectFailedHandler ConnectionFailed;

        private RemoteDevice remoteDevice;
        private IStream connection;

        /****************************************************************
         *                  Constructors                                *
         ****************************************************************/

        public FizVizDevice()
        {
        }

        /****************************************************************
         *                  Public Methods                              *
         ****************************************************************/

        public void Connect(String host, ushort port, uint baudRate)
        {
            Reset();

            connection = new NetworkSerial(new Windows.Networking.HostName(host), port);
            remoteDevice = new RemoteDevice(connection);
            remoteDevice.DeviceReady += OnDeviceReady;
            remoteDevice.DeviceConnectionLost += OnConnectionLost;
            remoteDevice.DeviceConnectionFailed += OnConnectionFailed;
            connection.begin(baudRate, SerialConfig.SERIAL_8N1);
        }

        public void SendCommand(FizVizCommand command)
        {
            command.PrepareToSendData();
            do
            {
                remoteDevice.writeCustomSysexMessage(command.Command, command.DataBuffer());
            }
            while (!command.TransmissionComplete());
        }

        public void Reset()
        {
            Disconnect();
            if (remoteDevice != null)
            {
                remoteDevice.DeviceReady -= OnDeviceReady;
                remoteDevice.DeviceConnectionLost -= OnConnectionLost;
                remoteDevice.DeviceConnectionFailed -= OnConnectionFailed;
            }
            remoteDevice = null;
            connection = null;
        }

        public void Disconnect()
        {
            if (connection != null)
            {
                connection.end();
            }
        }

        /****************************************************************
         *                  Event callbacks                             *
         ****************************************************************/

        public void OnDeviceReady()
        {
            if (DeviceReady != null)
                DeviceReady();
        }

        public void OnConnectionLost(String message)
        {
            if (ConnectionLost != null)
                ConnectionLost(message);
        }

        public void OnConnectionFailed(String message)
        {
            if (ConnectionFailed != null)
                ConnectionFailed(message);
        }


    }
}
