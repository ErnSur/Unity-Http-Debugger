using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QuickEye.RequestWatcher
{
    [FilePath(FilePath, FilePathAttribute.Location.ProjectFolder)]
    internal class StashDatabase : RequestDatabase<StashDatabase>
    {
        private const string FilePath = "Library/RequestStash.asset";
        
        [ContextMenu("Save")]
        public void Save()
        {
            Save(true);
            return;
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[]
            {
                this,
                // add all scriptable requests here
            }, FilePath, true);
        } 
    }
}