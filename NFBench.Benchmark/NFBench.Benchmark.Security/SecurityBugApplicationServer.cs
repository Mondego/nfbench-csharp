using System;
using NFBench.Benchmark.ReferenceImplementation;

namespace NFBench.Benchmark.Security
{
    public class SecurityBugApplicationServer : ReferenceApplicationServer
    {


        public SecurityBugApplicationServer(string hostname, int port) : base(hostname, port)
        {
        }
    }
}

