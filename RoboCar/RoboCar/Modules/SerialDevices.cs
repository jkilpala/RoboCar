using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace RoboCar.Modules
{
    class SerialDevices
    {
        private readonly static Lazy<SerialDevices> _instance = new Lazy<SerialDevices>(() => new SerialDevices());
        public static SerialDevices Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public ObservableCollection<DeviceInformation> listOfDevices { get; private set; }

        public SerialDevices()
        {
            ListAvailablePorts();
        }
        private async void ListAvailablePorts()
        {

            listOfDevices = new ObservableCollection<DeviceInformation>();

            try
            {
                string aqs = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);


                //status.Text = "Select a device and connect";


                for (int i = 0; i < dis.Count; i++)
                {
                    listOfDevices.Add(dis[i]);
                }


                //DeviceListSource.Source = listOfDevices;
                //comPortInput.IsEnabled = true;
                //ConnectDevices.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                //status.Text = ex.Message;
            }
        }
        public DeviceInformation GetUSBDevice()
        {
            return listOfDevices.First(item => item.Id.Contains("USB"));
        }
        public DeviceInformation GetSerialDevice()
        {
            return listOfDevices.First(item => !item.Id.Contains("USB"));
        }
    }
}
