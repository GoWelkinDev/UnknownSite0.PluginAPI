using System.Runtime.InteropServices;
using UnknownSite0.Plugins.Features.Logger;
using UnknownSite0.Plugins.Loader.Plugins;

namespace UnknownSite0.Examples
{
    /// <summary>
    /// 示例插件，继承 <see cref="Plugin"/> 并实现基本功能
    /// 该插件在启动时输出一条日志消息
    /// </summary>
    public class HelloWorldPlugin : Plugin
    {
        private static HelloWorldPlugin? _instance;

        /// <summary>
        /// 插件启动时调用的方法，输出欢迎日志
        /// </summary>
        protected override void OnStart()
        {
            Log.PrintLog("HelloWorldPlugin started. Hello, World!");
        }

        /// <summary>
        /// 插件导出的初始化入口点，由插件加载器通过 <c>OnInit</c> 名称调用
        /// </summary>
        /// <param name="hostApiTablePtr">指向宿主 API 函数表的非托管指针</param>
        /// <remarks>
        /// 此方法被 <see cref="UnmanagedCallersOnlyAttribute"/> 标记，
        /// 表示仅可由非托管代码调用。方法内部创建插件实例并调用
        /// <see cref="Plugin.Initialize"/> 完成初始化。
        /// </remarks>
        [UnmanagedCallersOnly(EntryPoint = "OnInit")]
        public static void OnInit(IntPtr hostApiTablePtr)
        {
            _instance = new HelloWorldPlugin();
            Initialize(hostApiTablePtr, _instance);
        }
    }
}
