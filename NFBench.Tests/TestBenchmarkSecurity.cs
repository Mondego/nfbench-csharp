using System;
using NUnit.Framework;
using NFBench.Runner;
using System.Threading;

namespace NFBench.Tests
{
    public class TestBenchmarkSecurity
    {
        //[Test]
        public void TestBuggySecurityApplication()
        {
            HeadlessBenchmarkWrapper hbw = new HeadlessBenchmarkWrapper(
                "security",
                "127.0.0.1",
                60738,
                Utility.getReferencedAssemblyLocation("NFBench.Benchmark.Security")
            );

            hbw.Start();

            for (int i = 0; i < 15; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(1000);
            }

            hbw.Stop();
        }
    }
}

