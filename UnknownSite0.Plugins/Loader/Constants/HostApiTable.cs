using System.Runtime.InteropServices;

namespace UnknownSite0.Plugins.Loader.Constants
{
    // 主程序暴露给插件的函数表
    [StructLayout(LayoutKind.Sequential)]
    public struct HostApiTable
    {
        public IntPtr TransitionToScene;
        public IntPtr PrintLog;
    }

    // 插件导出的初始化函数签名
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void PluginInitDelegate(IntPtr hostApiTable);
}
