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

        protected override void processMessageContent(string msg)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(msg);

            Thread.Sleep(1000);

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

