using System;
using NUnit.Framework;
using NFBench.Runner;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NFBench.Tests
{
    public class TestBenchmarkPerformance
    {
        [Test]
        public void TestBuggyPerformanceApplication()
        {
            HeadlessBenchmarkWrapper hbw = new HeadlessBenchmarkWrapper(
                "performance",
                "127.0.0.1",
                60728,
                Utility.getReferencedAssemblyLocation("NFBench.Benchmark.Performance")
            );

            int swarmCount = 10;
            List<TestChatClient> swarm = new List<TestChatClient>();
            for (int tccId = 0; tccId < swarmCount; tccId++) {
                swarm.Add(new TestChatClient("127.0.0.1", 60728, tccId));
            }
            hbw.Start();
            Thread.Sleep(3000);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (TestChatClient tcc in swarm) {
                Task.Run(() => {
                    tcc.start();
                });
                Thread.Sleep(100);
            }

            Thread.Sleep(swarmCount * swarmCount * 1000);
            stopwatch.Stop();
            Console.WriteLine("Time elapsed with set naps: {0}", stopwatch.Elapsed);

            foreach (TestChatClient tcc in swarm) {
                Task.Run(() => {
                    tcc.stop();
                });
            }
            hbw.Stop();
        }
    }
}

