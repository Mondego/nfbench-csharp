using System;

namespace NFBench.Benchmark.Security.Buggy
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            int applicationServerPort = (int)Int64.Parse (args [0]);

            BuggyApplicationServer buggy = new BuggyApplicationServer(applicationServerPort);
            buggy.start();
        }
    }
}
