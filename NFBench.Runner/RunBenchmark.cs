using System;
using System.Reflection;
using System.Threading.Tasks;

using NFBench.Services.ChatClient;
using NFBench.Benchmark.Security.Control;
using NFBench.Benchmark.Security.Buggy;

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
                    Console.WriteLine("Starting prototype security benchmark (WIP)");

                    int controlPort = 60708;
                    int buggyPort = 60808;

                    string pathToControlLauncher = Assembly.ReflectionOnlyLoad("NFBench.Benchmark.Security.Control").Location;
                    string pathToBuggyLauncher = Assembly.ReflectionOnlyLoad("NFBench.Benchmark.Security.Buggy").Location;

                    ServiceProcessWrapper controlWrapper = new ServiceProcessWrapper(pathToControlLauncher, controlPort.ToString());
                    ServiceProcessWrapper buggyWrapper = new ServiceProcessWrapper(pathToBuggyLauncher, buggyPort.ToString());

                    Task.Run(() => {
                        controlWrapper.Start();
                    });
                    Task.Run(() => {
                        buggyWrapper.Start();
                    });

                    while (true) { }

                    break;
                default:
                    Console.WriteLine("No benchmark {0} found.", benchmarkName);
                    break;
                }
            }
        }
    }
}
