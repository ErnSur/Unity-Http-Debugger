using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    [FilePath("Logs/http.log",FilePathAttribute.Location.ProjectFolder)]
    internal class RequestConsoleDatabase : RequestDatabase<RequestConsoleDatabase>
    {
        [ContextMenu("Save")]
        public void Save() => Save(true);
    }

    internal abstract class RequestDatabase<T> : ScriptableSingleton<T> where T : ScriptableObject
    {
        public List<HDRequest> requests;
        
        [ContextMenu("Save")]
        public void Save() => Save(true);
        // Create AddRequest method and save asset there?
    }
}