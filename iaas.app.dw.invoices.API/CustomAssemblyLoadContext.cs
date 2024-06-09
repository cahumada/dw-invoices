using System.Reflection;
using System.Runtime.Loader;

namespace iaas.app.dw.invoices.API
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unmanagedDllName"></param>
        /// <returns></returns>
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
}
