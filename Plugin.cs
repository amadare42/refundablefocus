using BepInEx;
using BepInEx.Logging;

namespace RefundableFocus
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        public static ManualLogSource Log => Instance.Logger;

        public FocusTracker FocusTracker;
        public RefundView View;
        
        private void Awake()
        {
            Instance = this;

            this.View = new RefundView();
            this.FocusTracker = new FocusTracker(this.View);

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
