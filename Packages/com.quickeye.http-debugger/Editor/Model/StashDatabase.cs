using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QuickEye.WebTools.Editor
{
    [FilePath(FilePath, FilePathAttribute.Location.ProjectFolder)]
    internal class StashDatabase : ScriptableSingleton<StashDatabase>
    {
        private const string FilePath = "Library/RequestStash.asset";
        public PersistentRequestList requests = new();

        private void OnEnable()
        {
            requests.Modified += Save;
            requests.Added += OnRequestAdded;
            requests.Removed += OnRequestRemoved;
        }

        private void OnDisable()
        {
            requests.Modified -= Save;
            requests.Added -= OnRequestAdded;
            requests.Removed -= OnRequestRemoved;
        }

        private void OnRequestAdded(RequestData request)
        {
            request.Modified += Save;
        }

        private void OnRequestRemoved(RequestData request)
        {
            request.Modified -= Save;
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
    }
}