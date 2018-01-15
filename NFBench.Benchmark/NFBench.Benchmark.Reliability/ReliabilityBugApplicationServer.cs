using System;
using NFBench.Benchmark.ReferenceImplementation;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace NFBench.Benchmark.Reliability
{
    public class ReliabilityBugApplicationServer : ReferenceApplicationServer
    {
        public ReliabilityBugApplicationServer(string hostname, int port) : base(hostname, port)
        {
        }

        public override void start()
        {
            debugMessage("Listening");
            listening = true;
            Random r = new Random();

            try {
                while (listening)
                {
                    if (listener.Available > 0)
                    {
                        int random = r.Next(0, 100);
                        if (random > 80)
                        {
                            listener.Close();
                            Thread.Sleep(500);
                            listener = new UdpClient(new IPEndPoint(IPAddress.Parse(mHostname), mPort));
                        }
                        listener.BeginReceive(new AsyncCallback(receiveMessageCallback), listener);
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
    }
}

