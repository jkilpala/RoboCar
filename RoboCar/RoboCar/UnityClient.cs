using Newtonsoft.Json;
using RoboCar.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace RoboCar
{
    class UnityClient
    {
        private readonly static Lazy<UnityClient> _instance = new Lazy<UnityClient>(() => new UnityClient());
        public static UnityClient Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        StreamSocketListenerConnectionReceivedEventArgs args = null;        
        public StreamSocketListenerConnectionReceivedEventArgs Args
        {
            get
            {
                return args;
            }            
        }
        //Todo: event mData change to control arduino
        private MoveData _mData;
        public MoveData mData
        {
            get
            {
                return _mData;
            }
            set
            {
                _mData = value;
                MoveDataChanged(_mData);
            }
        }
        public delegate void MoveDataChangedEvent(MoveData md);
        public event MoveDataChangedEvent MoveDataChanged;
        
        public void SetConnection(StreamSocketListenerConnectionReceivedEventArgs args)
        {
            this.args = args;
        }
        public async void ClearConnection()
        {            
            await args.Socket.CancelIOAsync();
            args.Socket.Dispose();
            this.args = null;
            System.Diagnostics.Debug.WriteLine("UnityClient::SetConnection: Clear connection!");
        }
        public async void SendGpsData(RoboCar.Data.GPSData gpsData)
        {
            var packetToSend = new DataPacket { Type = PacketType.GPSReading, Data = JsonConvert.SerializeObject(gpsData) };
            SendToStream(JsonConvert.SerializeObject(packetToSend));
        }
        public async void SendToStream(string message)
        {
            try
            {
                //string request = "send";
                Stream outStream = args.Socket.OutputStream.AsStreamForWrite();
                StreamWriter writer = new StreamWriter(outStream);
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("UnityClient::SendToStream:: exception: " + ex.Message);                
            }
        }
    }
}
