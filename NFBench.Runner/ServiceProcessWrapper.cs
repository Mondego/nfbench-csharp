using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace NFBench.Runner
{
    /// <summary>
    /// Process wrapper with redirected I/O stream
    /// </summary>
    public class ServiceProcessWrapper
    {
        private Process p;
        private StreamWriter ProcessStreamInterface;
        private string executablePath;

        /// <summary>
        /// Initializes NFBench.Runner.ServiceProcessWrapper with executable path, necessary args, and an optional working directory.
        /// </summary>
        /// <param name="path">String path of executable file</param>
        /// <param name="arguments">String combining arguments</param>
        /// <param name="workingdir">String path of where to execute the process (optional)</param>
        public ServiceProcessWrapper(string path, string arguments, string workingdir = null)
        {
            try {
                executablePath = path;

                p = new Process();
                p.StartInfo.FileName = executablePath;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardError = true;

                if (workingdir != null) {
                    p.StartInfo.WorkingDirectory = workingdir;
                }

                p.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
                    if (e.Data.Length > 0)
                    {
                        Console.WriteLine(e.Data.ToString().Trim());
                    }
                };

                p.ErrorDataReceived += delegate(object sender, System.Diagnostics.DataReceivedEventArgs e) {
                    if (e.Data.Length > 0) {
                        Console.WriteLine("[PID {0} {1}] {2}", 
                            p.Id, new FileInfo(path).Name,
                            e.Data.ToString().Trim());
                    }
                };
            }

            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Start the stream-instrumented process.
        /// </summary>
        /// <param name="nSecondsWaitBeforeStart">Time to delay process start</param>
        public void Start(int nSecondsWaitBeforeStart = 0)
        {
            try 
            {
                if (nSecondsWaitBeforeStart > 0) {
                    Thread.Sleep(nSecondsWaitBeforeStart * 1000);
                }

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                ProcessStreamInterface = p.StandardInput;
                Thread.Sleep(1000);
            }

            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Sends text to the stdin I/O stream reader of the process.
        /// </summary>
        /// <param name="msg">Message.</param>
        public void SendMessageToProcessConsole(string msg)
        {
            ProcessStreamInterface.WriteLine(msg);
        }

        /// <summary>
        /// Stop the process.
        /// </summary>
        public void Stop()
        {
            ProcessStreamInterface.Close();
            if (!p.HasExited) {
                p.CloseMainWindow();
                p.WaitForExit();
                p.Dispose();
            }
        }
    }
}