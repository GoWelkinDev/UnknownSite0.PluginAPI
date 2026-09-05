using System.Runtime.InteropServices;
using UnknownSite0.Plugins.Loader.Constants;

namespace UnknownSite0.Plugins.Loader
{
    public unsafe class PluginLoader
    {
        public HostApiTable Api { get; }
        public Action<string> LogWriter { get; }

        public PluginLoader(Action<string> logAction, HostApiTable apiTable)
        {
            LogWriter = logAction;
            Api = apiTable;
        }

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
