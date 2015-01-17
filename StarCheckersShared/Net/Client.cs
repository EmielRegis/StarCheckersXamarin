using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace StarCheckersWindows
{
    public class Client
    {
        private IPAddress serverIP;
        private int serverPort;

        public Client (IPAddress serverIP, int serverPort)
        {
            this.serverIP = serverIP;
            this.serverPort = serverPort;
        }

        public void StartClient(out string message)
        {

            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];
            message = "";
            // Connect to a remote device.
            try 
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                //            IPAddress ipAddress = ipHostInfo.AddressList[0];

                IPEndPoint remoteEP = new IPEndPoint(serverIP,serverPort);

                // Create a TCP/IP  socket.
                Socket client = new Socket(AddressFamily.InterNetwork, 
                    SocketType.Stream, ProtocolType.Tcp );


                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    client.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        client.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                    // Send the data through the socket.
                    int bytesSent = client.Send(msg);

                    // Receive the response from the remote device.
                    int bytesRec = client.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.ASCII.GetString(bytes,0,bytesRec));

                    message = Encoding.ASCII.GetString(bytes,0,bytesRec);

                    // Release the socket.
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();

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

