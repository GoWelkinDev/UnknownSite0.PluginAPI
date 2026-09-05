using System.Runtime.InteropServices;
using UnknownSite0.Plugins.Loader.Constants;

namespace UnknownSite0.Plugins.Loader.Plugins
{
    /// <summary>
    /// 所有插件的基类，提供与宿主程序交互的基础功能
    /// 插件继承此类并实现 <see cref="OnStart"/> 方法
    /// </summary>
    public abstract class Plugin
    {
        private static HostApiTable _api;

        /// <summary>
        /// 获取宿主程序提供的 API 函数表，插件可通过它调用宿主功能
        /// </summary>
        public HostApiTable Api => _api;

        /// <summary>
        /// 由插件入口函数调用，用于初始化插件并保存宿主 API 函数表
        /// 通常在插件DLL的导出函数 <c>OnInit</c> 中调用
        /// </summary>
        /// <param name="hostApiTablePtr">指向 <see cref="HostApiTable"/> 结构体的非托管指针</param>
        /// <param name="instance">要初始化的插件实例</param>
        /// <remarks>
        /// 该方法将非托管结构体转换为托管对象，设置全局API，并触发 <see cref="OnStart"/>
        /// </remarks>
        protected static void Initialize(IntPtr hostApiTablePtr, Plugin instance)
        {
            _api = Marshal.PtrToStructure<HostApiTable>(hostApiTablePtr);
            PluginContext.CurrentApi = _api;
            instance.OnStart();
        }

        /// <summary>
        /// 插件启动时调用的抽象方法，子类必须实现具体的初始化逻辑
        /// </summary>
        protected abstract void OnStart();
    }
}