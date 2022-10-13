using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QuickEye.RequestWatcher
{
    [FilePath(FilePath, FilePathAttribute.Location.ProjectFolder)]
    internal class StashDatabase : ScriptableSingleton<StashDatabase>
    {
        private const string FilePath = "Library/RequestStash.asset";
        public List<HDRequest> requests;

        [ContextMenu("Save")]
        public void Save()
        {
            Save(true);
            var assetObjects =requests.Cast<Object>().ToList();
            assetObjects.Insert(0,this);
            InternalEditorUtility.SaveToSerializedFileAndForget(assetObjects.ToArray(), FilePath, true);
        } 
    }
}