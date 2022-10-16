using UnityEngine;

namespace QuickEye.WebTools
{
    // For quick debugging with Odin Static Inspector
    internal class DebugActions
    {
        public static RequestData[] LoadAllRequests()
        {
            return Resources.FindObjectsOfTypeAll<RequestData>();
        }
        
        public static void UnloadUnusedAssets()
        {
             var task =Resources.UnloadUnusedAssets();
             task.completed += operation =>
             {
                 Debug.Log($"Unloaded Unused Assets");
             };
        }
    }
}