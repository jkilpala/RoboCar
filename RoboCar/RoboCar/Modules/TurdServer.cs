using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.System.Threading;
using RoboCar.Data;

namespace RoboCar
{
    class TurdServer : IDisposable
    {
        StreamSocketListener socketListener;
        public TurdServer()
        {
            try
            {
                //Create a StreamSocketListener to start listening for TCP connections.
                socketListener = new StreamSocketListener();

                //Hook up an event handler to call when connections are received.
                socketListener.ConnectionReceived += SocketListener_ConnectionReceived;

                //CoreApplication.Properties.Add("listener", socketListener);
                //Start listening for incoming TCP connections on the specified port. You can specify any port that' s not currently in use.

            }
            catch (Exception e)
            {
                //Handle exception.
                System.Diagnostics.Debug.WriteLine("Error: " +e.Message);
            }
        }

        public void Dispose()
        {
            socketListener.Dispose();
        }

        public async void StartServer()
        {
            await ThreadPool.RunAsync(async workItem =>
            {
                await socketListener.BindServiceNameAsync("8000");
                //await listener.BindServiceNameAsync(port.ToString());

            });
            
        }
        UnityClient uClient;
        private async void SocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender,
    Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            UnityClient.Instance.SetConnection(args);
           // CoreApplication.Properties.Add("listener", uClient);
            //Read line from the remote client.
            while (true)
            {
                Stream inStream = args.Socket.InputStream.AsStreamForRead();
                StreamReader reader = new StreamReader(inStream);
                string request = string.Empty;
                try
                {
                    request = await reader.ReadLineAsync();
                    System.Diagnostics.Debug.WriteLine("TurdServer::SocketListener::request " + request);
                    HandleCommand(request);
                }
                catch (Exception ex)
                {
                    UnityClient.Instance.ClearConnection();
                    System.Diagnostics.Debug.WriteLine("Client disconnected." + ex.Message);
                    break;
                }

                //Send the line back to the remote client.
                //Stream outStream = args.Socket.OutputStream.AsStreamForWrite();
                //StreamWriter writer = new StreamWriter(outStream);
                //await writer.WriteLineAsync(request);
                //await writer.FlushAsync(); 
            }
        }

        private void HandleCommand(string request)
        {
            var dPacket = JsonConvert.DeserializeObject<DataPacket>(request);
            switch (dPacket.Type)
            {
                case PacketType.GPSReading:
                    break;
                case PacketType.CommandFromUnity:
                    UnityClient.Instance.mData = JsonConvert.DeserializeObject<MoveData>(dPacket.Data);
                    break;
                default:
                    break;
            }
        }

        private async void SendToStream(StreamSocketListenerConnectionReceivedEventArgs args)
        {
            string request = "send";
            Stream outStream = args.Socket.OutputStream.AsStreamForWrite();
            StreamWriter writer = new StreamWriter(outStream);
            await writer.WriteLineAsync(request);
            await writer.FlushAsync();
        }
    }
}
