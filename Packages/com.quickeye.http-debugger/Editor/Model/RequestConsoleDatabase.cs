using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QuickEye.WebTools.Editor
{
    [FilePath(FilePath, FilePathAttribute.Location.ProjectFolder)]
    internal class RequestConsoleDatabase : ScriptableSingleton<RequestConsoleDatabase>, ISerializationCallbackReceiver
    {
        public const string FilePath = "Logs/RequestConsoleDatabase.asset";

        public PersistentRequestList requests;
        public HashSet<string> breakpoints = new HashSet<string>();

        [SerializeField]
        private List<string> serializedBreakpoints = new List<string>();

        private void OnEnable()
        {
            requests.Modified += Save;
        }

        private void OnDisable()
        {
            requests.Modified -= Save;
        }

        [ContextMenu("Save")]
        private void Save()
        {
            var assetObjects = requests.Cast<Object>().ToList();
            assetObjects.Insert(0, this);
            ScriptableSingletonUtility.SaveToSerializedFileAndForgetSafe(assetObjects.ToArray(), FilePath, true);
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            requests.Clear();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            serializedBreakpoints = new List<string>(breakpoints);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            breakpoints = new HashSet<string>(serializedBreakpoints);
        }
    }
}