using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace NFBench.Benchmark.Security.Buggy
{
    public class BuggyApplicationServer
    {
        private List<IPEndPoint> mConnections;
        private bool listening;
        private UdpClient listener;
        private string mEndpointInfo;
        private List<string> log;

        public BuggyApplicationServer(int port)
        {
            listener = new UdpClient(new IPEndPoint(IPAddress.Any, port));
            mConnections = new List<IPEndPoint>();
            log = new List<string>();
            mEndpointInfo = ((IPEndPoint)listener.Client.LocalEndPoint).ToString();
            debugMessage("started");
        }

        private void debugMessage (string message)
        {
            Console.WriteLine("NFBench.Benchmark.Security.ControlApplicationServer {0} --- {1}",
                mEndpointInfo, message);      
        }

        // Trashfire
        public void start()
        {
            listening = true;

            while (listening) {
                if (listener.Available > 0) {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] messageBuffer = listener.Receive(ref endPoint);
                    string message = Encoding.ASCII.GetString(messageBuffer);
                    mConnections.Add(new IPEndPoint(endPoint.Address, endPoint.Port));
                    log.Add(endPoint.ToString() + ": " + message);
                    foreach (IPEndPoint ipe in mConnections) {
                        string sendThis = String.Join(" ", log.ToArray());
                        byte[] datagram = Encoding.ASCII.GetBytes(sendThis);
                        listener.Send(datagram, datagram.Length, ipe);
                    }
                }
            }
        }

        public void stop()
        {
            // lol nah
        }
    }
}