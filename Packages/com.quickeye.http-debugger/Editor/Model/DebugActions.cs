using UnityEngine;

namespace QuickEye.RequestWatcher
{
    internal class DebugActions
    {
        public static HDRequest[] LoadAllRequests()
        {
            return Resources.FindObjectsOfTypeAll<HDRequest>();
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