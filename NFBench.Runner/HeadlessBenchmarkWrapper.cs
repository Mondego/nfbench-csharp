using System;
using System.Threading.Tasks;

namespace NFBench.Runner
{
    public class HeadlessBenchmarkWrapper
    {
        private string mBenchmarkName;
        private string mHostname;
        private int mPort;
        private string mAssemblyLocation;
        private ServiceProcessWrapper mExecutable;

        public HeadlessBenchmarkWrapper(
            string benchmarkName, 
            string appHostname, 
            int appPort, 
            string assemblyLocation
        )
        {
            mBenchmarkName = benchmarkName;
            mHostname = appHostname;
            mPort = appPort;
            mAssemblyLocation = assemblyLocation;
        }

        public void Start()
        {
            string argumentString = mBenchmarkName + " " + mHostname + " " + mPort;

            mExecutable = new ServiceProcessWrapper(
                mAssemblyLocation,
                argumentString
            );

            Task.Run(() => {
                mExecutable.Start();
            });
        }

        public void Stop()
        {
            mExecutable.Stop();
        }
    }
}

