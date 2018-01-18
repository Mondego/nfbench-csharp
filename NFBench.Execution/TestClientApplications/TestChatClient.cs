using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using InternalTools;

namespace TestClientApplications
{
    public class TestChatClient
    {
        protected UdpClient mClient;
        protected IPAddress mServerIP;
        protected int mServerPort;
        protected string mEndpointInfo;
        protected int mId;
        protected bool running = false;

        public TestChatClient(string serverHostname, int serverPort, int id = -1)
        {
            mClient = new UdpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            mServerIP = IPAddress.Parse(serverHostname);
            mServerPort = serverPort;
            mId = id;
            mEndpointInfo = ((IPEndPoint)mClient.Client.LocalEndPoint).ToString();
        }

        public void start()
        {
            running = true;

            Task.Run(() => {
                while (running) {
                    string message = Console.ReadLine();

                    if (message.Length > 0) {
                        byte[] datagram = Encoding.ASCII.GetBytes(message);
                        Console.WriteLine("[#{0}] sent {2}",  mId, mEndpointInfo, message);
                        mClient.BeginSend(
                            datagram, 
                            datagram.Length, 
                            new IPEndPoint(mServerIP, mServerPort),
                            new AsyncCallback((IAsyncResult ar) => {
                                //
                            }),
                            mClient
                        );
                    }
                }
            });

            mClient.BeginReceive(new AsyncCallback(ReceiveAsyncMessage), mClient);

            while (running) {
            }
        }

        protected void ReceiveAsyncMessage(IAsyncResult ar)
        {
            if (running == false)
                return;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] messageBuffer = ((UdpClient)ar.AsyncState).EndReceive(ar, ref endPoint);
            string messageText = Encoding.ASCII.GetString(messageBuffer, 0, messageBuffer.Length);
            Console.WriteLine("[#{0}] received {2}",  mId, mEndpointInfo, messageText);
            mClient.BeginReceive(new AsyncCallback(ReceiveAsyncMessage), mClient);
        }

        public void stop()
        {
            running = false;
            mClient.Close();
        }


    }
}