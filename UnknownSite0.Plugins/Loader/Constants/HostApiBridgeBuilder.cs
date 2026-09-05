using System.Runtime.InteropServices;

namespace UnknownSite0.Plugins.Loader.Constants
{
    public static unsafe class HostApiBridgeBuilder
    {
        // 委托定义
        public delegate void TransitionToSceneDelegate(IntPtr scenePathPtr, bool fadeOut);
        public delegate void PrintLogDelegate(IntPtr messagePtr);

        public static HostApiTable Create(TransitionToSceneDelegate transition, PrintLogDelegate printLog)
        {
            return new HostApiTable
            {
                TransitionToScene = (IntPtr)(delegate* unmanaged<IntPtr, bool, void>)Marshal.GetFunctionPointerForDelegate(transition),
                PrintLog = (IntPtr)(delegate* unmanaged<IntPtr, void>)Marshal.GetFunctionPointerForDelegate(printLog)
            };
        }
    }
}
