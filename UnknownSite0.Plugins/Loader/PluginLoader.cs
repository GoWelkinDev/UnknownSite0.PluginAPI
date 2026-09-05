using System.Runtime.InteropServices;
using UnknownSite0.Plugins.Loader.Constants;

namespace UnknownSite0.Plugins.Loader
{
    /// <summary>
    /// 负责加载和管理插件的核心类
    /// 使用非托管代码 (LoadLibrary) 动态加载插件并调用其导出函数
    /// </summary>
    public unsafe class PluginLoader
    {
        /// <summary>
        /// 主程序暴露给插件的 API 函数表
        /// </summary>
        public HostApiTable Api { get; }

        /// <summary>
        /// 主程序的日志输出委托，用于记录加载过程信息
        /// </summary>
        public Action<string> LogWriter { get; }

        /// <summary>
        /// 初始化 <see cref="PluginLoader"/> 类的新实例
        /// </summary>
        /// <param name="logAction">用于接收日志消息的委托</param>
        /// <param name="apiTable">要传递给插件的主程序 API 函数表</param>
        public PluginLoader(Action<string> logAction, HostApiTable apiTable)
        {
            LogWriter = logAction;
            Api = apiTable;
        }

        /// <summary>
        /// 加载指定路径的插件，并调用其导出的 <c>OnInit</c> 函数
        /// </summary>
        /// <param name="dllPath">插件的完整路径</param>
        /// <returns>如果加载并初始化成功，返回 <c>true</c>; 否则返回 <c>false</c></returns>
        /// <remarks>
        /// 加载成功后模块句柄会保留，以便插件在宿主进程生命周期内保持加载状态
        /// 初始化函数指针类型为 <c>delegate* unmanaged&lt;IntPtr, void&gt;</c>
        /// </remarks>
        public bool LoadPlugin(string dllPath)
        {
            IntPtr hModule = NativeMethods.LoadLibrary(dllPath);
            if (hModule == IntPtr.Zero)
            {
                LogWriter($"Failed to load {dllPath}, error: {Marshal.GetLastWin32Error()}");
                return false;
            }

            IntPtr initPtr = NativeMethods.GetProcAddress(hModule, "OnInit");
            if (initPtr == IntPtr.Zero)
            {
                LogWriter("OnInit not found");
                NativeMethods.FreeLibrary(hModule);
                return false;
            }

            var initFunc = (delegate* unmanaged<IntPtr, void>)initPtr;
            // 将结构体指针传递给插件
            IntPtr apiPtr = Marshal.AllocHGlobal(Marshal.SizeOf<HostApiTable>());
            Marshal.StructureToPtr(Api, apiPtr, false);

            initFunc(apiPtr);

            // 不释放 hModule，插件在整个生命周期内需要保持加载
            return true;
        }

        /// <summary>
        /// 加载指定目录下的所有插件 (*.dll)
        /// </summary>
        /// <param name="directoryPath">插件目录路径</param>
        /// <remarks>
        /// 若目录不存在则静默返回。每个DLL的加载过程会通过 <see cref="LogWriter"/> 记录。
        /// </remarks>
        public void LoadPluginsFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            string[] dllFiles = Directory.GetFiles(directoryPath, "*.dll");

            foreach (string dllPath in dllFiles)
            {
                string fileName = Path.GetFileName(dllPath);
                LogWriter($"Loading plugin: {fileName}");
                LoadPlugin(dllPath);
            }
        }
    }

    /// <summary>
    /// 提供对 Windows Kernel32 动态链接库函数的封装，供插件加载器使用
    /// </summary>
    internal partial class NativeMethods
    {
        [LibraryImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr LoadLibrary(string lpFileName);

        [LibraryImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
        public static partial IntPtr GetProcAddress(IntPtr hModule, string procName);

        [LibraryImport("kernel32.dll", EntryPoint = "FreeLibrary", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool FreeLibrary(IntPtr hModule);
    }
}
