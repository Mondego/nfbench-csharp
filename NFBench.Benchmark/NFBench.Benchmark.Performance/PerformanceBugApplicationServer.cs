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

        protected override void processMessage(string message, string endp)
        {
            Thread.Sleep(1000);

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
    }
}

