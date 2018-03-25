using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using InternalTools;

namespace NFBench.Benchmark.ReferenceImplementation
{
    public class ReferenceApplicationServer
    {
        protected ConcurrentDictionary<string, IPEndPoint> mConnections;
        protected ConcurrentDictionary<int, string> mConnectionIds;
        protected bool listening = false;
        protected UdpClient listener;
        protected string mEndpointInfo;
        protected string mHostname;
        protected int mPort;
        protected int nMessagesSent = 0;
        protected int nMessagesReceived = 0;

        public ReferenceApplicationServer(string hostname, int port)
        {
            listener = new UdpClient(new IPEndPoint(IPAddress.Parse(hostname), port));
            mConnections = new ConcurrentDictionary<string, IPEndPoint>();
            mConnectionIds = new ConcurrentDictionary<int, string>();
            mEndpointInfo = ((IPEndPoint)listener.Client.LocalEndPoint).ToString();
            mHostname = hostname;
            mPort = port;
        }

        protected void debugMessage (string message)
        {
            Console.WriteLine("{0} {1} --- {2}",
                this.GetType().Name,
                mEndpointInfo, 
                message);      
        }

        public virtual void start()
        {
            debugMessage("Listening");
            listening = true;

            try {
                listener.BeginReceive(new AsyncCallback(receiveMessageCallback), listener);

                while (listening)
                {
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

            finally
            {
                debugMessage("Done");
                stop();
            }
        }

        protected virtual void receiveMessageCallback(IAsyncResult ar)
        {
            try
            {
                if (listening == false) return;

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] messageBuffer = ((UdpClient)ar.AsyncState).EndReceive(ar, ref endPoint);
                string received = Encoding.ASCII.GetString(messageBuffer);

                mConnections.TryAdd(
                    endPoint.ToString(), 
                    new IPEndPoint(endPoint.Address, endPoint.Port));

                nMessagesReceived += 1;
                processMessage(received, endPoint.ToString());

                listener.BeginReceive(new AsyncCallback(receiveMessageCallback), listener);
            }

            catch (Exception ex) {
                debugMessage(ex.Message);
            }
        }

        protected virtual void processMessage(string message, string endp)
        {
            if (message[0] == '@') {
                int senderId = (int)Int64.Parse(message.Split(new char[] { ' ' }, 3)[1].Remove(0, 1));
                mConnectionIds.TryAdd(senderId, endp);
                handlePrivateMessage(message, senderId);
            }
            else
            {
                int senderId = (int)Int64.Parse(message.Split(new char[] { ' ' }, 2)[0].Remove(0, 1));
                mConnectionIds.TryAdd(senderId, endp);
                handleBroadcastMessage(message, senderId);
            }
        }

        protected virtual void handlePrivateMessage(string message, int sentBy)
        {
            int pmUid = 
                (int)Int64.Parse(
                    message.Split(new char[] { ' ' }, 2)[0]
                    .Remove(0, 1));

            if (mConnectionIds.ContainsKey(pmUid)) {
                IPEndPoint pmDestination = mConnections[mConnectionIds[pmUid]];
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                nMessagesSent += 1;
                listener.BeginSend(
                    buffer,
                    buffer.Length,
                    pmDestination,
                    new AsyncCallback(endSendMessageCallback),
                    listener
                );
            } else {
                IPEndPoint destination = mConnections[mConnectionIds[sentBy]];
                byte[] buffer = Encoding.ASCII.GetBytes("[Failed Delivery] " + message);
                nMessagesSent += 1;
                listener.BeginSend(
                    buffer,
                    buffer.Length,
                    destination,
                    new AsyncCallback(endSendMessageCallback),
                    listener
                );
            }
        }

        protected virtual void handleBroadcastMessage(string message, int sentBy)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);

            foreach (var endp in mConnections) {
                nMessagesSent += 1;
                listener.BeginSend(
                    buffer,
                    buffer.Length,
                    endp.Value,
                    new AsyncCallback(endSendMessageCallback),
                    listener
                );
            }
        }

        protected virtual void endSendMessageCallback(IAsyncResult ar)
        {
            int bytes = listener.EndSend(ar);
        }

        public void stop()
        {
            listening = false;
            listener.Close();
        }
    }
}

