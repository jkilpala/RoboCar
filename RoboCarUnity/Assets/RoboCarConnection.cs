using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System;
using System.Text;

public class RoboCarConnection : MonoBehaviour
{

    //the name of the connection, not required but better for overview if you have more than 1 connections running
    public string conName = "Localhost";

    //ip/address of the server, 127.0.0.1 is for your own computer
    public string conHost = "192.168.1.12";

    //port for the server, make sure to unblock this in your router firewall if you want to allow external connections
    public int conPort = 8000;

    //a true/false variable for connection status
    public bool socketReady = false;

    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;


    byte[] data = new byte[1024];
    string receivedMsg = string.Empty;

    public RoboDataManager rDataManager;
    void Start()
    {
        rDataManager = RoboDataManager.Instance;
        setupSocket();
    }
    //try to initiate connection
    public void setupSocket()
    {
        try
        {
            mySocket = new TcpClient(conHost, conPort);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            socketReady = true;

            InvokeRepeating("receiveData", 0.001f, 0.01f);
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);
        }
    }

    //send message to server
    public void writeSocket(string theLine)
    {
        if (!socketReady)
            return;
        String tmpString = theLine + "\r\n";
        theWriter.Write(tmpString);
        theWriter.Flush();
    }

    //read message from server
    //public string readSocket()
    //{
    //    String result = "";
    //    if (theStream.DataAvailable)
    //    {
    //        Byte[] inStream = new Byte[mySocket.SendBufferSize];
    //        theStream.Read(inStream, 0, inStream.Length);
    //        result += System.Text.Encoding.UTF8.GetString(inStream);
    //    }
    //    return result;
    //}

    void receiveData()
    {
        try
        {
            if (!socketReady)
            {
                Debug.Log("Connection not ready");
                return;
            }

            int numberOfBytesRead = 0;
            if (theStream.CanRead)
            {
                try
                {
                    numberOfBytesRead = theStream.Read(data, 0, data.Length);
                    receivedMsg = Encoding.UTF8.GetString(data, 0, numberOfBytesRead);
                    Debug.Log("receivedMsg " + receivedMsg);
                    NetCommandHandler(receivedMsg);

                }
                catch (Exception ex)
                {
                    Debug.Log("Error in networkstream: " + ex.Message);                    
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error receiving data: " + ex.Message);
            
        }
    }


    //keep connection alive, reconnect if connection lost
    public void maintainConnection()
    {
        if(!theStream.CanRead)
        {
            setupSocket();
        }
    }

    //disconnect from the socket
    public void closeSocket()
    {
        if (!socketReady)
            return;
        theWriter.Close();
        theReader.Close();
        mySocket.Close();
        socketReady = false;
    }

    
    
    public void OnApplicationQuit()
    {
        if (mySocket != null)
        {
            theStream.Close();
            mySocket.Close();
        }
        //closeSocket();
    }

    void NetCommandHandler(string data)
    {
        var dPacket = JsonUtility.FromJson<DataPacket>(receivedMsg);

        switch (dPacket.Type)
        {
            case PacketType.GPSReading:
                //Cross thread call
                rDataManager.AddGpsData(dPacket.Data);
                //CommandPool.Add(() => { rDataManager.AddGpsData(dPacket.Data); });
                //rDataManager.AddGpsData(dPacket.Data);
                break;
            case PacketType.CommandFromUnity:
                break;
            default:
                break;
        }
    }
}
