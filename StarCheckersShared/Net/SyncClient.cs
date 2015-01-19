using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace StarCheckersWindows
{
    public class SyncClient
    {
        private IPAddress serverIP;
        private int serverPort;
        public Socket serverSocket;

        public SyncClient (IPAddress serverIP, int serverPort)
        {
            this.serverIP = serverIP;
            this.serverPort = serverPort;
        }

 
        public void StopClient()
        {
            try
            {
                // Release the socket.
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public string SendAndReceiveMessage(string message)
        {
            try
            {

                // Encode the data string into a byte array.
                byte[] msg = Encoding.ASCII.GetBytes(message);

                // Send the data through the socket.
                int bytesSent = serverSocket.Send(msg);
                Console.WriteLine("Sent = {0}", message);

                byte[] bytes = new byte[1024];
                // Receive the response from the remote device.
                int bytesRec = serverSocket.Receive(bytes);

                string answer = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                Console.WriteLine("Received = {0}", answer);



                return answer;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return "";
            }
        }

        public string ReceiveMessage()
        {
            byte[] bytes = new byte[1024];
            // Receive the response from the remote device.
            int bytesRec = serverSocket.Receive(bytes);

            string answer = Encoding.ASCII.GetString(bytes, 0, bytesRec);
            Console.WriteLine("Received = {0}", answer);
           
            return answer;
        }

        public void SendMessage(string message)
        {
            // Encode the data string into a byte array.
            byte[] msg = Encoding.ASCII.GetBytes(message);

            // Send the data through the socket.
            int bytesSent = serverSocket.Send(msg);
            Console.WriteLine("Sent = {0}", message);
        }

        public void StartClient()
        {

            // Data buffer for incoming data.

            // Connect to a remote device.
            try 
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                //            IPAddress ipAddress = ipHostInfo.AddressList[0];

                IPEndPoint remoteEP = new IPEndPoint(serverIP,serverPort);

                // Create a TCP/IP  socket.
                serverSocket = new Socket(AddressFamily.InterNetwork, 
                    SocketType.Stream, ProtocolType.Tcp );


                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    serverSocket.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        serverSocket.RemoteEndPoint.ToString());

                } 
                catch (ArgumentNullException ane) 
                {
                    Console.WriteLine("ArgumentNullException : {0}",ane.ToString());
                } 
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}",se.ToString());
                } 
                catch (Exception e) 
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            } 
            catch (Exception e) 
            {
                Console.WriteLine( e.ToString());
            }
        }
    }
}

