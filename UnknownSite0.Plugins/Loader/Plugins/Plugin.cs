using System.Runtime.InteropServices;
using UnknownSite0.Plugins.Loader.Constants;

namespace UnknownSite0.Plugins.Loader.Plugins
{
    public abstract unsafe class Plugin
    {
        private static HostApiTable _api;

        // 插件 API
        public HostApiTable Api => _api;

        // 由插件入口函数调用，传入函数表指针并触发插件初始化
        protected static void Initialize(IntPtr hostApiTablePtr, Plugin instance)
        {
            _api = Marshal.PtrToStructure<HostApiTable>(hostApiTablePtr);
            PluginContext.CurrentApi = _api;
            instance.OnStart();
        }

        // 插件需要实现的启动方法
        protected abstract void OnStart();

        protected void TransitionToScene(string scenePath, bool fadeOut = true)
        {
            var transitionFunc = (delegate* unmanaged<IntPtr, bool, void>)_api.TransitionToScene;
            IntPtr pathPtr = Marshal.StringToCoTaskMemUTF8(scenePath);
            transitionFunc(pathPtr, fadeOut);
            Marshal.FreeCoTaskMem(pathPtr);
        }
    }
}