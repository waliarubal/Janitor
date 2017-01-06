using System;
using System.IO;
using System.Reflection;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class Proxy: MarshalByRefObject
    {
        public Assembly GetAssembly(string assemblyPath)
        {
            var data = File.ReadAllBytes(assemblyPath);
            return Assembly.Load(data);
        }
    }
}
