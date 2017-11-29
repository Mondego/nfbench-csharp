using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace NFBench.Services.ChatClient
{
    public class ChatClient
    {
        private int mServerPort;
        private UdpClient mClient;
        private string endpointInfo;
        private bool running;
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public ChatClient(int serverPort)
        {
            mServerPort = serverPort;
            mClient = new UdpClient(0);
            endpointInfo = ((IPEndPoint)mClient.Client.LocalEndPoint).ToString();

            debugMessage("Created chat client");
        }

        private void debugMessage (string message)
        {
            Console.WriteLine("NFBench.Services.ChatClient {0} --- {1}", endpointInfo, message);
        }

        public void start()
        {
            running = true;

            Task.Run(() => {
                while (running) {
                    string message = Console.ReadLine();
                    if (message.Length > 0) {
                        CreateAsyncMessage(message);
                    }
                }
            });

            while (running) 
            {
                if (mClient.Available > 0) {
                    allDone.Reset();
                    mClient.BeginReceive(new AsyncCallback(ReceiveAsyncMessage), mClient);
                    allDone.WaitOne();
                }
            }
        }

        private void ReceiveAsyncMessage(IAsyncResult ar)
        {
            allDone.Set();
            if (running == false)
                return;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] messageBuffer = ((UdpClient)ar.AsyncState).EndReceive(ar, ref endPoint);
            string messageText = Encoding.ASCII.GetString(messageBuffer, 0, messageBuffer.Length);
            debugMessage("Received " + messageText);
        }

        private void CreateAsyncMessage(string message)
        {
            if (running == false)
                return;
            debugMessage("Begin sending " + message);
            IPEndPoint destination = new IPEndPoint(IPAddress.Any, mServerPort);
            byte[] datagram = Encoding.ASCII.GetBytes(message);
            mClient.BeginSend(datagram, datagram.Length, destination, new AsyncCallback(FinishSend), mClient);
        }

        private void FinishSend(IAsyncResult ar)
        {
            int bytesSent = ((UdpClient)ar.AsyncState).EndSend(ar);
        }

        public void stop()
        {
            running = false;
            mClient.Close();
        }
    }
}

