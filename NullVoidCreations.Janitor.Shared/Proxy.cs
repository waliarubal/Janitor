using System;
using System.IO;
using System.Reflection;

namespace NullVoidCreations.Janitor.Shared
{
    public class Proxy: MarshalByRefObject
    {
        public Assembly GetAssembly(string assemblyPath)
        {
            var data = File.ReadAllBytes(assemblyPath);
            return Assembly.Load(data);
        }
    }
}
