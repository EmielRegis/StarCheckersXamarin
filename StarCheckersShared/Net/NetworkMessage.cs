using System;
using StarCheckersWindows;

namespace StarCheckersWindows
{
    public sealed class NetworkMessage
    {
        public NetworkMessageType Type;
        public object Object;

        public NetworkMessage()
        {

        }

        public NetworkMessage(NetworkMessageType type, object obj)
        {
            Type = type;
            Object = obj;
        }
    }
}

