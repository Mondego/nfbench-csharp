using System;

namespace NFBench.Benchmark.Performance
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length >= 3) {
                string benchmarkName = args[0];
                string hostname = args[1];
                int port = (int)Int64.Parse (args [2]);

                PerformanceBugApplicationServer app = new PerformanceBugApplicationServer(hostname, port);
                app.start();
            }
        }
    }
}
