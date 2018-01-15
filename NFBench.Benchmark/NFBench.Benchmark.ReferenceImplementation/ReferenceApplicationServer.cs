using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NFBench.Benchmark.ReferenceImplementation
{
    public class ReferenceApplicationServer
    {
        protected ConcurrentDictionary<string, IPEndPoint> mConnections;
        protected bool listening = false;
        protected UdpClient listener;
        protected string mEndpointInfo;
        protected string mHostname;
        protected int mPort;

        public ReferenceApplicationServer(string hostname, int port)
        {
            listener = new UdpClient(new IPEndPoint(IPAddress.Parse(hostname), port));
            mConnections = new ConcurrentDictionary<string, IPEndPoint>();
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
                string message = Encoding.ASCII.GetString(messageBuffer);

                //debugMessage("received message: " + message + " from " + endPoint.ToString());
                mConnections.TryAdd(
                    endPoint.ToString(), 
                    new IPEndPoint(endPoint.Address, endPoint.Port));
                processMessageContent(message);
                listener.BeginReceive(new AsyncCallback(receiveMessageCallback), listener);
            }

            catch (Exception ex) {
                debugMessage(ex.Message);
            }
        }

        protected virtual void processMessageContent(string msg)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(msg);

            // PM
            if (msg[0] == '@') {

            } else {
                foreach (var endp in mConnections) {
                    listener.BeginSend(
                        buffer,
                        buffer.Length,
                        endp.Value,
                        new AsyncCallback((IAsyncResult iar) => {
                            int bytes = listener.EndSend(iar);
                        }),
                        listener
                    );
                }
            }
        }

        public void stop()
        {
            listening = false;
            listener.Close();
        }
    }
}

