
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.UI.Core;

namespace RoboCar.Modules
{
    class GPSModule
    {
        private SerialDevice serialPort;
        private CancellationTokenSource ReadCancellationTokenSource;
        //private CoreDispatcher Dispatcher;

        public GPSModule()
        {
            
        }
        public async void InitDevice()
        {
            DeviceInformation entry = SerialDevices.Instance.GetSerialDevice();

            try
            {
                serialPort = await SerialDevice.FromIdAsync(entry.Id);

                serialPort.WriteTimeout = TimeSpan.FromMilliseconds(50); //1000
                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(50);     //1000
                serialPort.BaudRate = 9600;
                serialPort.Parity = SerialParity.None;
                serialPort.StopBits = SerialStopBitCount.One;
                serialPort.DataBits = 8;
                var device = new NmeaParser.SerialPortDevice(serialPort);


                device.MessageReceived += device_MessageReceived;
                await device.OpenAsync();

                ReadCancellationTokenSource = new CancellationTokenSource();
                //Listen();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GPSModule::ConnectToSerialDevice::Exception: " + ex.Message);
                //throw;
            }
        }
        private void CloseDevice()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
            }
            serialPort = null;
        }
        private void device_MessageReceived(object sender, NmeaParser.NmeaMessageReceivedEventArgs args)
        {
            //Access ui thread
            var _ = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                NmeaParser.Nmea.Gps.Gprmc ulos;
                if (args.Message is NmeaParser.Nmea.Gps.Gprmc)
                {
                    ulos = args.Message as NmeaParser.Nmea.Gps.Gprmc;
                    var gpsData = new RoboCar.Data.GPSData { Latitude = ulos.Latitude, Longitude = ulos.Longitude };
                    if (UnityClient.Instance.Args != null)
                    {
                        UnityClient.Instance.SendGpsData(gpsData);
                    }
                  //  System.Diagnostics.Debug.WriteLine("Latitude: " + ulos.Latitude + "Longitude" + ulos.Longitude);
                }
                //System.Diagnostics.Debug.WriteLine(args.Message.MessageType + ": " + args.Message.ToString());
                //messages.Enqueue(args.Message.MessageType + ": " + args.Message.ToString()); 
                //if (messages.Count > 100) messages.Dequeue(); //Keep message queue at 100 
                //output.Text = string.Join("\n", messages.ToArray()); 
            });
        }
    }
}
