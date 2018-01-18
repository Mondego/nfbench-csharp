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

            int swarmCount = 3;
            List<SystemProcessWithIOHandler> swarm = new List<SystemProcessWithIOHandler>();
            for (int i = 0; i < swarmCount; i++) {
                swarm.Add(
                    new SystemProcessWithIOHandler(
                        path: Assembly.ReflectionOnlyLoad("TestClientApplications").Location,
                        arguments: "127.0.0.1 60708 " + i
                    )
                );
            }

            for (int i = 0; i < swarmCount; i++) {
                swarm[i].Start();
                Thread.Sleep(500);
                swarm[i].SendMessageToProcessConsole("#" + i + " Hello World");
            }

            swarm[2].SendMessageToProcessConsole("@1 #2 Private message for @1");
            swarm[2].SendMessageToProcessConsole("@7 #2 Private message for @7");

            Thread.Sleep(5000);

            // Cleanup
            referenceApp.Stop();
            foreach (SystemProcessWithIOHandler s in swarm) {
                s.Stop();
            }
        }
    }
}

