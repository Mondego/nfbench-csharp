using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using InternalTools;
using NUnit.Framework;
using DeepTestFramework;

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
            string referenceApplicationArgumentString = "reference 127.0.0.1 60708";
            string testClientApplicationPath = Assembly.ReflectionOnlyLoad("TestClientApplications").Location;
            string client1Arguments = "127.0.0.1 60708 1";
            string client2Arguments = "127.0.0.1 60708 2";

            // Instrumentation
            // Instrumentation points in ReferenceApplicationServer
            // * 
            // * stopwatch: receiveMessageCallback -> endSendMessageCallback
            // * nMessagesSent & nMessagesReceived value capture

            // Test
            using (SystemProcessWrapperWithInput sut = Driver.ExecuteWithArguments(referenceApplicationPath, referenceApplicationArgumentString))  
            using (SystemProcessWrapperWithInput client1 = Driver.ExecuteWithArguments(testClientApplicationPath, client1Arguments))
            using (SystemProcessWrapperWithInput client2 = Driver.ExecuteWithArguments(testClientApplicationPath, client2Arguments))
            {
                client1.ConsoleInput("#1 Hello World");
                client2.ConsoleInput("#2 Hello World");

                client1.ConsoleInput("@2 #1 Private message for @2");
                client1.ConsoleInput("@7 #1 Private message for @7");

                // Assert.AreEqual(0, Driver.captureValue(snapshotNMessagesSent));
                Thread.Sleep(5000);
            }
        }
    }
}

