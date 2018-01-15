using System;
using NFBench.Benchmark.ReferenceImplementation;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace NFBench.Benchmark.Performance
{
    public class PerformanceBugApplicationServer : ReferenceApplicationServer
    {
        public PerformanceBugApplicationServer(string hostname, int port) : base(hostname, port)
        {
            
        }

        public override void start()
        {
            debugMessage("Listening");
            listening = true;
          
            try {
                while (listening)
                {
                    if (listener.Available > 0)
                    {
                        Task.Run(() => {
                            listener.BeginReceive(new AsyncCallback(receiveMessageCallback), listener);
                        });
                    }
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

        protected override void receiveMessageCallback(IAsyncResult ar)
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
                //listener.BeginReceive(new AsyncCallback(receiveMessageCallback), listener);
            }

            catch (Exception ex) {
                debugMessage(ex.Message);
            }
        }

        protected override void processMessageContent(string msg)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(msg);

            // PM
            if (msg[0] == '@') {

            } else {
                foreach (var endp in mConnections) {
                    Task.Run(() => {
                        listener.Send(buffer, buffer.Length, endp.Value);
                    });
                }
            }
        }
    }
}

