using System;
using System.Linq;
using System.Net;
using System.Text;

using NFBench.Benchmark.ReferenceImplementation;


namespace NFBench.Benchmark.Security
{
    public class SecurityBugApplicationServer : ReferenceApplicationServer
    {
        public SecurityBugApplicationServer(string hostname, int port) : base(hostname, port)
        {
        }

        protected override void handlePrivateMessage(string message, int sentBy)
        {
            int pmUid = 
                (int)Int64.Parse(
                    message.Split(new char[] { ' ' }, 2)[0]
                    .Remove(0, 1));

            if (mConnectionIds.ContainsKey(pmUid)) {
                string sendTo = mConnectionIds.FirstOrDefault(kvp => kvp.Key != pmUid).Value;

                IPEndPoint pmDestination = mConnections[sendTo];
                 
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                listener.BeginSend(
                    buffer,
                    buffer.Length,
                    pmDestination,
                    new AsyncCallback((IAsyncResult iar) => {
                        int bytes = listener.EndSend(iar);
                    }),
                    listener
                );
            } else {
                handleBroadcastMessage(message, sentBy);
            }
        }
    }
}

