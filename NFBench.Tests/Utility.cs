using System;
using System.Reflection;

namespace NFBench.Tests
{
    public static class Utility
    {
        public static string getReferencedAssemblyLocation(string assemblyName)
        {
            try
            {
                return Assembly.ReflectionOnlyLoad(assemblyName).Location;
            }

            catch (Exception e) {
                Console.WriteLine("NFBench.Tests.Utility {0}", e.Message);
            }

            return null;
        }
    }
}

