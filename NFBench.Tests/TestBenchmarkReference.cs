using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using InternalTools;

namespace NFBench.Tests
{
    public class TestBenchmarkReference
    {
        [Test]
        public void TestReferenceApplication()
        {
            SystemProcessWithIOHandler referenceApp = 
                new SystemProcessWithIOHandler(
                    path: Assembly.ReflectionOnlyLoad("NFBench.Benchmark.ReferenceImplementation").Location,
                    arguments: "reference 127.0.0.1 60708"                           
                );
            referenceApp.Start();

            int swarmCount = 10;
            List<SystemProcessWithIOHandler> swarm = new List<SystemProcessWithIOHandler>();
            for (int i = 0; i < swarmCount; i++) {
                swarm.Add(
                    new SystemProcessWithIOHandler(
                        path: Assembly.ReflectionOnlyLoad("TestClientApplications").Location,
                        arguments: "127.0.0.1 60708 " + i                        
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
            referenceApp.Stop();
            foreach (SystemProcessWithIOHandler s in swarm) {
                s.Stop();
            }
        }
    }
}

