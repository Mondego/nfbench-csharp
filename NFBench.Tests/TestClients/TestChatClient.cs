using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NFBench.Tests
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
            string msg = "Hello from [#" + mId + "]-" + mEndpointInfo;
            byte[] datagram = Encoding.ASCII.GetBytes(msg);

            mClient.BeginSend(
                datagram, 
                datagram.Length, 
                new IPEndPoint(mServerIP, mServerPort),
                new AsyncCallback((IAsyncResult ar) => {
                    int nBytes = mClient.EndSend(ar);
                    Console.WriteLine("[#{0}-{1}] -> {2}",  mId, mEndpointInfo, msg);
                }),
                mClient
            );
           
            while (running) 
            {
                // Issues with messages from application server otherwise
                if (mClient.Available > 0) {
                    IPEndPoint syncEP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] msgBuffer = mClient.Receive(ref syncEP);
                    string msgReceived = Encoding.ASCII.GetString(msgBuffer, 0, msgBuffer.Length);
                    Console.WriteLine("[#{0}-{1}] -> {2}", mId, mEndpointInfo, msgReceived);
                }
            }
        }

        public void stop()
        {
            running = false;
            mClient.Close();
        }
    }
}