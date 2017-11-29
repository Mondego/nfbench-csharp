using System;
using System.Reflection;

using NFBench.Benchmark.Security;
using NFBench.Services.ChatClient;
using System.Threading.Tasks;

namespace NFBench.Runner
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting NFBench");

            if (args.Length > 0) {
                string benchmarkName = args[0];

                switch (benchmarkName) {
                case "security": 
                    Console.WriteLine("Starting prototype security benchmark");
                    // Console.WriteLine(Assembly.ReflectionOnlyLoad("NFBench.Services.ChatClient").Location);

                    Task.Run(() => {
                        ControlApplicationServer control = new ControlApplicationServer(60708);
                        Console.WriteLine("Control server running at localhost:60708");
                        control.start();
                    });
                    
                    Task.Run(() => {
                        BuggyApplicationServer buggy = new BuggyApplicationServer(60808);
                        Console.WriteLine("Buggy server running at localhost:60808");
                        buggy.start();
                    });
                   
                    while (true) 
                    {
                    }

                    break;
                default:
                    Console.WriteLine("No benchmark {0} found.", benchmarkName);
                    break;
                }
            }
        }
    }
}
