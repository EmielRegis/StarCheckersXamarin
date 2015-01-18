using System;
using StarCheckersWindows;

namespace StarCheckersWindows
{
    public sealed class NetworkMessage
    {
        public NetworkMessageType Type { get; set;}
        public object Obj { get; set;}

        public NetworkMessage()
        {

        }

        public NetworkMessage(NetworkMessageType type, object obj)
        {
            Type = type;
            this.Obj = obj;
        }
    }
}

