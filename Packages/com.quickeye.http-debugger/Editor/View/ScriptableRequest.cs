using UnityEngine;

namespace QuickEye.RequestWatcher
{
    internal class ScriptableRequest : ScriptableObject
    {
        public HDRequest request;
        public bool isReadOnly;
        
        private void OnValidate()
        {
            // Save request
            // in case of stash item, save to disk
            // playmode, nothing
        }
    }
}