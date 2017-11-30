using System;

namespace NFBench.Benchmark.Security.Control
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            int applicationServerPort = (int)Int64.Parse (args [0]);

            ControlApplicationServer control = new ControlApplicationServer(applicationServerPort);
            control.start();
        }
    }
}
