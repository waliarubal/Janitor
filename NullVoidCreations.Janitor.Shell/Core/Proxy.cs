using System;
using System.Reflection;

namespace NullVoidCreations.Janitor.Shell.Core
{
    class Proxy: MarshalByRefObject
    {
        public Assembly GetAssembly(string assemblyPath)
        {
            return Assembly.LoadFile(assemblyPath);
        }
    }
}
