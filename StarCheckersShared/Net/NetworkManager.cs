using System;
using System.Net;
using Newtonsoft;
using Newtonsoft.Json;
using System.Xml;
using Newtonsoft.Json.Serialization;

namespace StarCheckersWindows
{
    public class NetworkManager : IDisposable
    {
        public IPAddress ServerIP { get; set;}
        public int ServerPort { get; set;}
        private SyncClient client;

        public NetworkManager(IPAddress serverIP, int serverPort)
        {
            ServerIP = serverIP;
            ServerPort = serverPort;

            client = new SyncClient(serverIP, serverPort);
            client.StartClient();
        }

        public NetworkMessage SendReceiveMessage(NetworkMessageType type, object obj)
        {
            string message = JsonConvert.SerializeObject(new NetworkMessage(type, obj));
//            string answer = new AsyncClient(ServerIP, ServerPort).StartClient(message);
            string answer = client.SendAndReceiveMessage(message);
            return JsonConvert.DeserializeObject<NetworkMessage>(answer);
        }

        public NetworkMessage SendReceiveMessage(NetworkMessage networkMessage)
        {
            string message = JsonConvert.SerializeObject(networkMessage);
//            string answer = new AsyncClient(ServerIP, ServerPort).StartClient(message);
            string answer = client.SendAndReceiveMessage(message);
            return JsonConvert.DeserializeObject<NetworkMessage>(answer);
        }

        #region IDisposable implementation

        public void Dispose()
        {
            client.SendAndReceiveMessage("end");
            client.StopClient();
        }

        #endregion
    }
}

