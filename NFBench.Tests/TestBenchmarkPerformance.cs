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
        //[Test]
        public void TestBuggyPerformanceApplication()
        {
            SystemProcessWithIOHandler buggyPerformanceApp = 
                new SystemProcessWithIOHandler(
                    path: Assembly.ReflectionOnlyLoad("NFBench.Benchmark.Performance").Location,
                    arguments: "performance 127.0.0.1 60728"                           
                );
            buggyPerformanceApp.Start();

            int swarmCount = 3;
            List<SystemProcessWithIOHandler> swarm = new List<SystemProcessWithIOHandler>();
            for (int i = 0; i < swarmCount; i++) {
                swarm.Add(
                    new SystemProcessWithIOHandler(
                        path: Assembly.ReflectionOnlyLoad("TestClientApplications").Location,
                        arguments: "127.0.0.1 60728 " + i
                    )
                );
            }

            for (int i = 0; i < swarmCount; i++) {
                swarm[i].Start();
                Thread.Sleep(500);
                swarm[i].SendMessageToProcessConsole("#" + i + " Hello World");
            }

            Thread.Sleep(5000);

            buggyPerformanceApp.Stop();
            foreach (SystemProcessWithIOHandler s in swarm) {
                s.Stop();
            }
        }
    }
}

