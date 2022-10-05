using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    // TODO: Split into stash and playmode log classes
    [FilePath("Library/http.log",FilePathAttribute.Location.ProjectFolder)]
    internal class Database : ScriptableSingleton<Database>
    {
        public List<HDRequest> playmodeRequests;
        public List<HDRequest> stash;
        
        private void OnValidate()
        {
            Save(true);
        }

        [MenuItem("Http Debugger/Database")]
        public static void SelectDatabase()
        {
            Selection.activeObject = instance;
        }

        [ContextMenu("Save")]
        private void SaveToLibrary()
        {
            Save(true);
        }
    }
}