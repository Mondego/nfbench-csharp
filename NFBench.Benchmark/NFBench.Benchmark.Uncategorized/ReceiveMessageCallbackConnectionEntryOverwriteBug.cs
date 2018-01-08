using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NFBench.Benchmark.Uncategorized
{
    public class ReceiveMessageCallbackConnectionEntryOverwriteBug
    {
        private ConcurrentDictionary<string, IPEndPoint> mConnections;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private bool listening = false;
        private UdpClient listener;
        private string mEndpointInfo;

        public ReceiveMessageCallbackConnectionEntryOverwriteBug (int port)
        {
            listener = new UdpClient(new IPEndPoint(IPAddress.Any, port));
            mConnections = new ConcurrentDictionary<string, IPEndPoint>();
            mEndpointInfo = ((IPEndPoint)listener.Client.LocalEndPoint).ToString();
        }

        private void debugMessage (string message)
        {
            Console.WriteLine("NFBench.Benchmark.Uncategorized {0} --- {1}",
                mEndpointInfo, message);      
        }

        public void start()
        {
            debugMessage("started");
            listening = true;

            try {
                while (listening) {
                    allDone.Reset();
                    listener.BeginReceive(new AsyncCallback(receiveMessageCallback), listener);
                    allDone.WaitOne();
                }
            }

            catch (ObjectDisposedException)
            {
                Console.WriteLine("Listener safely closed.");
            }

            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void receiveMessageCallback(IAsyncResult ar)
        {
            allDone.Set();
            if (listening == false) return;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] messageBuffer = ((UdpClient)ar.AsyncState).EndReceive(ar, ref endPoint);
            listener.BeginReceive(new AsyncCallback(receiveMessageCallback), listener);

            string message = Encoding.ASCII.GetString(messageBuffer);
            debugMessage("received message: " + message + " from " + endPoint.ToString());
            mConnections.TryAdd(
                mEndpointInfo, // buggy, will overwrite entries
                endPoint);

            foreach (var endp in mConnections) {
                listener.BeginSend(messageBuffer, messageBuffer.Length, endp.Value, new AsyncCallback(sendMessageCallback), listener);
            }
        }

        private void sendMessageCallback(IAsyncResult ar)
        {
            int bytesSent = ((UdpClient)ar.AsyncState).EndSend(ar);
        }

        public void stop()
        {
            listening = false;
            listener.Close();
        }
    }
}

