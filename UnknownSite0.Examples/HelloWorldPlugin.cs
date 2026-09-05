using System.Runtime.InteropServices;
using UnknownSite0.Plugins.Features.Logger;
using UnknownSite0.Plugins.Loader.Plugins;

namespace UnknownSite0.Examples
{
    public class HelloWorldPlugin : Plugin
    {
        private static readonly HelloWorldPlugin? _instance;

        protected override void OnStart()
        {
            Log.PrintLog("HelloWorldPlugin started. Hello, World!");
        }

        [UnmanagedCallersOnly(EntryPoint = "OnInit")]
        public static void OnInit(IntPtr hostApiTablePtr)
        {
            var _instance = new HelloWorldPlugin();
            Initialize(hostApiTablePtr, _instance);
        }
    }
}
