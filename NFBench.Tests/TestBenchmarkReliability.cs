using System;
using NFBench.Runner;
using NUnit.Framework;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NFBench.Tests
{
    public class TestBenchmarkReliability
    {
        [Test]
        public void TestBuggyReliabilityApplication()
        {
            HeadlessBenchmarkWrapper hbw = new HeadlessBenchmarkWrapper(
                "reliability",
                "127.0.0.1",
                60718,
                Utility.getReferencedAssemblyLocation("NFBench.Benchmark.Reliability")
            );

            int swarmCount = 10;
            List<TestChatClient> swarm = new List<TestChatClient>();
            for (int tccId = 0; tccId < swarmCount; tccId++) {
                swarm.Add(new TestChatClient("127.0.0.1", 60718, tccId));
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

