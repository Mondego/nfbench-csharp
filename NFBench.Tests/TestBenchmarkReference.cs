using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using InternalTools;
using NUnit.Framework;
using DeepTestFramework;
using System.IO;

namespace NFBench.Tests
{
    public class TestBenchmarkReference
    {
        [Test]
        public void TestReferenceApplication()
        {
            SystemUnderTestDeploymentAPI Driver = new SystemUnderTestDeploymentAPI();
            InstrumentationAPI Instrumentation = new InstrumentationAPI();

            // Deployment prep
            string referenceApplicationPath =
                Assembly.ReflectionOnlyLoad("NFBench.Benchmark.ReferenceImplementation").Location;
            string weaveToApplicationPath =
                Directory.GetParent(referenceApplicationPath).FullName + "/wovenReferenceImplementation.exe";
            string referenceApplicationArgumentString = "reference 127.0.0.1 60708";
            string testClientApplicationPath = Assembly.ReflectionOnlyLoad("TestClientApplications").Location;
            string client1Arguments = "127.0.0.1 60708 1";
            string client2Arguments = "127.0.0.1 60708 2";

            // Instrumentation
            // Instrumentation points in ReferenceApplicationServer
            // * 
            // * (timer) stopwatch: receiveMessageCallback -> endSendMessageCallback

            Console.WriteLine("Instrumenting system: {0}", referenceApplicationPath);
            Console.WriteLine("Writing to: {0}", weaveToApplicationPath);

            Instrumentation.AddAssemblyFromPath(referenceApplicationPath);
            Instrumentation.SetAssemblyOutputPath(
                "NFBench.Benchmark.ReferenceImplementation",
                weaveToApplicationPath
            );

            InstrumentationPoint stopwatchStartPoint = 
                Instrumentation.AddNamedInstrumentationPoint("startStopwatchGotMessage")
                    .FindInAssemblyNamed("NFBench.Benchmark.ReferenceImplementation")
                    .FindInTypeNamed("ReferenceApplicationServer")
                    .FindMethodNamed("receiveMessageCallback");

            InstrumentationPoint stopwatchEndPoint = 
                Instrumentation.AddNamedInstrumentationPoint("stopStopwatchSentMessage")
                    .FindInAssemblyNamed("NFBench.Benchmark.ReferenceImplementation")
                    .FindInTypeNamed("ReferenceApplicationServer")
                    .FindMethodNamed("endSendMessageCallback");

            Instrumentation.Measure
                .WithStopWatch()
                .StartingAtEntry(stopwatchStartPoint)
                .UntilExit(stopwatchEndPoint);

            // * (val) nMessagesSent -> endSendMessageCallback
            // * (val) nMessagesReceived -> receiveMessageCallback

            // Test
            using (SystemProcessWrapperWithInput sut = Driver.ExecuteWithArguments(weaveToApplicationPath, referenceApplicationArgumentString))  
            using (SystemProcessWrapperWithInput client1 = Driver.ExecuteWithArguments(testClientApplicationPath, client1Arguments))
            using (SystemProcessWrapperWithInput client2 = Driver.ExecuteWithArguments(testClientApplicationPath, client2Arguments))
            {
                client1.ConsoleInput("#1 Hello World");

                long elapsedMilliseconds = (long)Driver.captureValue(stopwatchEndPoint);
                Assert.GreaterOrEqual(100, elapsedMilliseconds);

                // client2.ConsoleInput("#2 Hello World");
                // client1.ConsoleInput("@2 #1 Private message for @2");
                // client1.ConsoleInput("@7 #1 Private message for @7");

                Thread.Sleep(5000);
            }
        }
    }
}

