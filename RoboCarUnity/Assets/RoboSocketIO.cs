using UnityEngine;
using System.Collections;
using BestHTTP;
using BestHTTP.SocketIO;
using System;
using BestHTTP.WebSocket;

public class RoboSocketIO : MonoBehaviour
{

    WebSocket websocket;
    // Use this for initialization
    void Start()
    {
        websocket = new WebSocket(new Uri("wss://192.168.1.12:8001/sockets/"));
        websocket.OnOpen += OnWebSocketOpen;
        websocket.OnMessage += OnMessageReceived;
        websocket.OnError += OnError;
        websocket.OnBinary += OnBinaryMessageReceived;
        websocket.OnClosed += OnWebSocketClosed;
        websocket.OnErrorDesc += OnErrorDesc;

        websocket.Open();

    }

    void OnErrorDesc(WebSocket ws, string error)

    {

        Debug.Log("Error: " + error);

    }

    private void OnBinaryMessageReceived(WebSocket webSocket, byte[] message)

    {

        Debug.Log("Binary Message received from server. Length: " + message.Length);

    }
    private void OnWebSocketClosed(WebSocket webSocket, UInt16 code, string message)

    {

        Debug.Log("WebSocket Closed!");

    }

    private void OnError(WebSocket ws, Exception ex)

    {

        string errorMsg = string.Empty;
        if (ws.InternalRequest.Response != null)
            errorMsg = string.Format("Status Code from Server: {0} and Message: {1}",
            ws.InternalRequest.Response.StatusCode,
            ws.InternalRequest.Response.Message);

        Debug.Log("An error occured: " + (ex != null ? ex.Message : "Unknown: " + errorMsg));
    }
    private void OnMessageReceived(WebSocket webSocket, string message)
    {
        Debug.Log("Text Message received from server: " + message);
    }

    private void OnWebSocketOpen(WebSocket webSocket)
    {
        Debug.Log("Websocket open!");
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void SendStuff(string message)
    {
        websocket.Send("Message to the server");
    }

    public void OnApplicationQuit()
    {
       // websocket.Close();
    }
}
