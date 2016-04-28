using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboCar
{
    enum PacketType
    {
        GPSReading,
        CommandFromUnity
    }
    class DataPacket
    {
        public PacketType Type;
        public string Data;
    }
}
