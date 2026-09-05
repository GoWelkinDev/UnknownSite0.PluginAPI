using System.Runtime.InteropServices;
using UnknownSite0.Plugins.Loader.Plugins;

namespace UnknownSite0.Plugins.Features.Logger
{
    public unsafe class Log
    {
        public static void PrintLog(string message)
        {
            var api = PluginContext.CurrentApi;
            var printFunc = (delegate* unmanaged<IntPtr, void>)api.PrintLog;
            IntPtr msgPtr = Marshal.StringToCoTaskMemUTF8(message);
            printFunc(msgPtr);
            Marshal.FreeCoTaskMem(msgPtr);
        }
    }
}
