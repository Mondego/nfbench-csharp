using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using NUnit.Framework;

using InternalTools;

namespace NFBench.Tests
{
    public class TestBenchmarkPerformance
    {
        [Test]
        public void TestBuggyPerformanceApplication()
        {
            SystemProcessWithIOHandler buggyPerformanceApp = 
                new SystemProcessWithIOHandler(
                    path: Assembly.ReflectionOnlyLoad("NFBench.Benchmark.Performance").Location,
                    arguments: "performance 127.0.0.1 60728"                           
                );
            buggyPerformanceApp.Start();

            int swarmCount = 10;
            List<SystemProcessWithIOHandler> swarm = new List<SystemProcessWithIOHandler>();
            for (int i = 0; i < swarmCount; i++) {
                swarm.Add(
                    new SystemProcessWithIOHandler(
                        path: Assembly.ReflectionOnlyLoad("TestClientApplications").Location,
                        arguments: "127.0.0.1 60728 " + i                        
                    )
                );
            }

            foreach (SystemProcessWithIOHandler client in swarm) {
                client.Start();
                Thread.Sleep(500);
            }

            Console.WriteLine("Swarm started. Taking a nap for 5 seconds");
            Thread.Sleep(5000);

            // Cleanup
            buggyPerformanceApp.Stop();
            foreach (SystemProcessWithIOHandler s in swarm) {
                s.Stop();
            }


        }
    }
}

