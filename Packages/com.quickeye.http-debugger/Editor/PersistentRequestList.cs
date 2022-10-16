using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    /// <summary>
    /// List created specifically for ScriptableSingleton Databases
    /// It ensures that list items have correct hideFlags and are disposed when unreferenced
    /// </summary>
    [Serializable]
    internal class PersistentRequestList : ListWithEvents<RequestData>
    {
        public PersistentRequestList()
        {
            Init();
        }

        public PersistentRequestList(IEnumerable<RequestData> collection) : base(collection)
        {
            Init();
        }

        private void Init()
        {
            Added += r => r.hideFlags = HideFlags.DontSaveInEditor;
            Removed += r => r.Dispose();
            BeforeClear += DisposeAll;
            
            void DisposeAll()
            {
                foreach (var request in this)
                {
                    request.Dispose();
                }
            }
        }
    }
}