using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NmeaParser;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RoboCar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer;

        //GpioPin pin;
        //int LED_PIN = 5;
        //GpioPinValue pinValue;
        GPIOModule gpioModule;
        Modules.GPSModule gpsModule;
        //Arduino Control
        MotorController motorController;
        
        TurdServer turdS;

        bool _arduinoModuleOn = true;
        bool _turdServerModuleOn = true;
        bool _gpsModuleOn = true;
        public MainPage()
        {
            this.InitializeComponent();
            //List Serial devices
            //ListAvailablePorts();
            gpsModule = new Modules.GPSModule();
            gpsModule.InitDevice();

            //Connect to GPS and Arduino
            //ConnectToSerialDevice();           
                       
            turdS = new TurdServer();
            turdS.StartServer();

            motorController = new MotorController();
            motorController.SetupRemoteArduino();
                        
            //timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(2000);
            //timer.Tick += Timer_Tick;
            //gpioModule = new GPIOModule();
            //gpioModule.InitGPIO();
            //if (gpioModule.pin != null)
            //{
            //    timer.Start();
                
            //}
        }

        private void Timer_Tick(object sender, object e)
        {
            //if (gpioModule.pinValue == GpioPinValue.High)
            //{
            //    gpioModule.pinValue = GpioPinValue.Low;
            //}
            //else
            //{
            //    gpioModule.pinValue = GpioPinValue.High;
            //}
            //gpioModule.pin.Write(gpioModule.pinValue);
            //if (UnityClient.Instance.Args != null)
            //{
            //    //UnityClient.Instance.SendToStream("kakia");
            //}
            // motorController.WriteToPin();
        }
        SerialDevice serialPort;
        

        
        ObservableCollection<DeviceInformation> listOfDevices;

        //List<DeviceInformation> listOfDevices;
        private DataReader dataReaderObject;
        private CancellationTokenSource ReadCancellationTokenSource;
        //private async void ListAvailablePorts()
        //{

        //    listOfDevices = new ObservableCollection<DeviceInformation>();

        //    try
        //    {
        //        string aqs = SerialDevice.GetDeviceSelector();
        //        var dis = await DeviceInformation.FindAllAsync(aqs);


        //        //status.Text = "Select a device and connect";


        //        for (int i = 0; i < dis.Count; i++)
        //        {
        //            listOfDevices.Add(dis[i]);
        //        }


        //        //DeviceListSource.Source = listOfDevices;
        //        //comPortInput.IsEnabled = true;
        //        //ConnectDevices.SelectedIndex = -1;
        //    }
        //    catch (Exception ex)
        //    {
        //        //status.Text = ex.Message;
        //    }
        //}

        
   //     private void device_MessageReceived(object sender, NmeaParser.NmeaMessageReceivedEventArgs args)
 		//{ 

            
 		//	var _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
 			
   //         {
   //             NmeaParser.Nmea.Gps.Gprmc ulos;
   //             if (args.Message is NmeaParser.Nmea.Gps.Gprmc)
   //             {
   //                 ulos = args.Message as NmeaParser.Nmea.Gps.Gprmc;
   //                 var gpsData = new RoboCar.Data.GPSData { Latitude = ulos.Latitude, Longitude = ulos.Longitude };
   //                 if (UnityClient.Instance.Args != null)
   //                 {
   //                     UnityClient.Instance.SendGpsData(gpsData);
   //                 }
   //                 System.Diagnostics.Debug.WriteLine("Latitude: " +ulos.Latitude +"Longitude" +ulos.Longitude);
   //             }
   //              //System.Diagnostics.Debug.WriteLine(args.Message.MessageType + ": " + args.Message.ToString());
   //           //messages.Enqueue(args.Message.MessageType + ": " + args.Message.ToString()); 
 		//		//if (messages.Count > 100) messages.Dequeue(); //Keep message queue at 100 
 		//		//output.Text = string.Join("\n", messages.ToArray()); 
 		//	}); 
 		//}


//private async void ConnectToSerialDevice()
//        {
//            if (listOfDevices.Count <= 0)
//            {
//                return;
//            }
//            DeviceInformation entry = (DeviceInformation)listOfDevices[0];
//            if (_arduinoModuleOn && listOfDevices.Count > 1)
//            {
//                motorController.SetupRemoteArduino(listOfDevices[1]);
//            }
//            try
//            {
//                serialPort = await SerialDevice.FromIdAsync(entry.Id);

//                serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
//                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
//                serialPort.BaudRate = 9600;
//                serialPort.Parity = SerialParity.None;
//                serialPort.StopBits = SerialStopBitCount.One;
//                serialPort.DataBits = 8;
//                var device = new NmeaParser.SerialPortDevice(serialPort);
               

//                device.MessageReceived += device_MessageReceived;
//                await device.OpenAsync();
                
//                ReadCancellationTokenSource = new CancellationTokenSource();
//                //Listen();
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine("MainPage::ConnectToSerialDevice::Exception: " + ex.Message);
//                throw;
//            }
//        }
        //private async void device_NmeaMessageReceived(NmeaParser.NmeaDevice sender, NmeaParser.Nmea.NmeaMessage args)
        //{
            
        //    // called when a message is received
        //}
        /// <summary>
        /// - Create a DataReader object
        /// - Create an async task to read from the SerialDevice InputStream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private async void Listen()
        //{
        //    try
        //    {
        //        if (serialPort != null)
        //        {
        //            dataReaderObject = new DataReader(serialPort.InputStream);

        //            // keep reading the serial input
        //            while (true)
        //            {
        //                await ReadAsync(ReadCancellationTokenSource.Token);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.GetType().Name == "TaskCanceledException")
        //        {
        //            //status.Text = "Reading task was cancelled, closing device and cleaning up";
        //            CloseDevice();
        //        }
        //        else
        //        {
        //            //status.Text = ex.Message;
        //        }
        //    }
        //    finally
        //    {
        //        // Cleanup once complete
        //        if (dataReaderObject != null)
        //        {
        //            dataReaderObject.DetachStream();
        //            dataReaderObject = null;
        //        }
        //    }
        //}

        /// <summary>
        /// ReadAsync: Task that waits on data and reads asynchronously from the serial device InputStream
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //private async Task ReadAsync(CancellationToken cancellationToken)
        //{
        //    Task<UInt32> loadAsyncTask;

        //    uint ReadBufferLength = 1024;

        //    // If task cancellation was requested, comply
        //    cancellationToken.ThrowIfCancellationRequested();

        //    // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
        //    dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

        //    // Create a task object to wait for data on the serialPort.InputStream
        //    loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);

        //    // Launch the task and wait
        //    UInt32 bytesRead = await loadAsyncTask;
        //    if (bytesRead > 0)
        //    {
        //        var text = dataReaderObject.ReadString(bytesRead);
        //        System.Diagnostics.Debug.WriteLine(text);
        //        //status.Text = "bytes read successfully!";
        //    }
        //    System.Diagnostics.Debug.WriteLine("");
        //}

        /// <summary>
        /// CancelReadTask:
        /// - Uses the ReadCancellationTokenSource to cancel read operations
        /// </summary>
        //private void CancelReadTask()
        //{
        //    if (ReadCancellationTokenSource != null)
        //    {
        //        if (!ReadCancellationTokenSource.IsCancellationRequested)
        //        {
        //            ReadCancellationTokenSource.Cancel();
        //        }
        //    }
        //}

        /// <summary>
        /// CloseDevice:
        /// - Disposes SerialDevice object
        /// - Clears the enumerated device Id list
        /// </summary>
        //private void CloseDevice()
        //{
        //    if (serialPort != null)
        //    {
        //        serialPort.Dispose();
        //    }
        //    serialPort = null;

        ////    comPortInput.IsEnabled = true;
        // //   sendTextButton.IsEnabled = false;
        //  //  rcvdText.Text = "";
        //    listOfDevices.Clear();
        //}
        


    }
}
