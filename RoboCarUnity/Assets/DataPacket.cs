using UnityEngine;
using System.Collections;

public enum PacketType
{
    GPSReading,
    CommandFromUnity
}
public class DataPacket {
    public PacketType Type;
    public string Data;	
}
