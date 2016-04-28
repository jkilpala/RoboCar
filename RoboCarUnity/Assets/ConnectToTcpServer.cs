using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;
using System.Text;
using System;
using System.Collections.Generic;

public class ConnectToTcpServer : MonoBehaviour
{
    public static ConnectToTcpServer _instance;
    public static ConnectToTcpServer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ConnectToTcpServer>();
            }
            return _instance;
        }
    }
    public static Socket Master;
    private string Ip = "192.168.1.12";
    private int port = 8000;

    public RoboDataManager rDataManager;

    public List<Action> CommandPool = new List<Action>(); 
    // Use this for initialization
    void Start()
    {
        rDataManager = RoboDataManager.Instance;
        Master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Ip), port);
        try
        {
            Master.Connect(ip);            
        }
        catch (System.Exception ex)
        {
            Debug.Log("Could not connect: " + ex.Message);
        }
        //StartCoroutine(DATA_IN());
        //Invoke("DATA_IN", 0f);
        //InvokeRepeating("DATA_IN2", 0f, 0.01f);
        Thread t = new Thread(DATA_IN);
        t.Start();
    }

    private void DATA_IN2()
    {
        byte[] Buffer;
        int readBytes;

        try
        {
            Buffer = new byte[Master.SendBufferSize];
            readBytes = Master.Receive(Buffer);
            if (readBytes > 0)
            {
                var message = Encoding.UTF8.GetString(Buffer);
                var dataP = JsonUtility.FromJson<DataPacket>(message);
                NetCommandHandler(dataP);
                //Debug.Log(message);
            }
        }
        catch (System.Exception)
        {
            Debug.Log("Server disconnected");

        }
    }

    private void DATA_IN()
    {
        
        byte[] Buffer;
        int readBytes;

        try
        {
            while (true)
            {
                Buffer = new byte[Master.SendBufferSize];                
                readBytes = Master.Receive(Buffer);
                if (readBytes > 0)
                {
                    var message = Encoding.UTF8.GetString(Buffer);
                    var dataP = JsonUtility.FromJson<DataPacket>(message);
                    NetCommandHandler(dataP);
                    //Debug.Log(message);
                }                                
            }
        }
        catch (System.Exception)
        {
            Debug.Log("ConnectToTcpServer::DATA_IN:Server disconnected");
        }
        
    }

    public void SendSomethingToServer()
    {
        string text = "muumi\n";
        Master.Send(Encoding.UTF8.GetBytes(text));
    }
    // Update is called once per frame
    void Update()
    {
        if (CommandPool.Count > 0)
        {
            //Debug.Log(CommandPool.Count.ToString());
            CommandPool[0]();
            CommandPool.RemoveAt(0);
            if (CommandPool.Count > 0)
            {
                CommandPool[0]();
                CommandPool.RemoveAt(0);
            }
        }
    }

    public void SendMove(int direction)
    {
        Debug.Log("ConnectToTcpServer::SendMove::Direction: " + direction);
        switch (direction)
        {
            case 0:
                SendMoveCommandToServer(new MoveData { Left = true, Right = true, LeftValue = 1f, RightValue = 1f });
                break;
            case 1:
                SendMoveCommandToServer(new MoveData { Left = false, Right = true, LeftValue = 0.0f, RightValue = 1f });
                break;
            case 2:
                SendMoveCommandToServer(new MoveData { Left = false, Right = false, LeftValue = 1f, RightValue = 1f });
                break;
            case 3:
                SendMoveCommandToServer(new MoveData { Left = true, Right = false, LeftValue = 1f, RightValue = 0.0f });
                break;
            default:
                SendMoveCommandToServer(new MoveData { Left = false, Right = false, LeftValue = 0, RightValue = 0 });
                break;
        }
    }

    public void SendControllerMove(bool left, float leftSpeed, bool right, float rightSpeed)
    {
        SendMoveCommandToServer(new MoveData { Left = left, Right = right, LeftValue = leftSpeed, RightValue = rightSpeed });
    }
    MoveData currentMoveData;
    private void SendMoveCommandToServer(MoveData md)
    {
        //TODO lokaali seuranta
        if (md.Left != RoboDataManager.Instance.MData.Left ||
            md.Right != RoboDataManager.Instance.MData.Right ||
            md.LeftValue != RoboDataManager.Instance.MData.LeftValue ||
            md.RightValue != RoboDataManager.Instance.MData.RightValue)
        {
            Debug.Log("ConnectToTcpServer::SendMoveCommandToServer");
            DataPacket toSend = new DataPacket { Type = PacketType.CommandFromUnity, Data = JsonUtility.ToJson(md) };
            var jDataPacket = JsonUtility.ToJson(toSend);
            //Todo callback if completed
            RoboDataManager.Instance.MData = md;
            Master.Send(Encoding.UTF8.GetBytes(string.Concat(jDataPacket, "\n")));
            Debug.Log(string.Format("md left:{0} right:{1} lv:{2} rv:{3}", md.Left, md.Right, md.LeftValue, md.RightValue));
        }
    }
    private void SendCommandToServer(PacketType packet)
    {
        switch (packet)
        {
            case PacketType.GPSReading:
                break;
            case PacketType.CommandFromUnity:
                var moveForward = new MoveData { Left = true, Right = true, LeftValue = 0.5f, RightValue = 0.5f };
                DataPacket toSend = new DataPacket { Type = PacketType.CommandFromUnity, Data = JsonUtility.ToJson(moveForward) };
                var jDataPacket = JsonUtility.ToJson(toSend);
                Master.Send(Encoding.UTF8.GetBytes(jDataPacket));
                break;
            default:
                break;
        }
    }
    void NetCommandHandler(DataPacket dPacket)
    {
        switch (dPacket.Type)
        {
            case PacketType.GPSReading:
                //Cross thread call
                //rDataManager.AddGpsData(dPacket.Data);
                CommandPool.Add(() => { rDataManager.AddGpsData(dPacket.Data); });
                //rDataManager.AddGpsData(dPacket.Data);
                break;
            case PacketType.CommandFromUnity:
                break;
            default:
                break;
        }
    }

    private IEnumerator GetGpsData(string data)
    {
        rDataManager.AddGpsData(data);
        yield return null;
    }

    public void OnApplicationQuit()
    {
        if (Master != null)
        {
            Master.Close(); 
        }
    }
}
