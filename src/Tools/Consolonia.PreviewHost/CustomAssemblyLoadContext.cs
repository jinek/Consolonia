using System.Reflection;
using System.Runtime.Loader;

namespace Consolonia.PreviewHost
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public CustomAssemblyLoadContext() : base(true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null!; // Return null to use the default loading mechanism
        }
    }
}