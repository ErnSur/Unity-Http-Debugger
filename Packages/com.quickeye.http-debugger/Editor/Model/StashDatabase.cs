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
        public RequestList requests = new();

        private void OnEnable()
        {
            foreach (var request in requests)
            {
                request.Modified += Save;
            }
            requests.Added += OnRequestsOnAdded;
            requests.Removed += OnRequestsOnRemoved;
            requests.BeforeClear += OnRequestsOnBeforeClear;
        }

        private void OnDisable()
        {
            foreach (var request in requests)
            {
                request.Modified -= Save;
            }
            requests.Added -= OnRequestsOnAdded;
            requests.Removed -= OnRequestsOnRemoved;
            requests.BeforeClear -= OnRequestsOnBeforeClear;
        }

        void OnRequestsOnAdded(RequestData request)
        {
            request.hideFlags = HideFlags.DontSaveInEditor;
            request.Modified += Save;
            Save();
        }

        private void OnRequestsOnRemoved(RequestData request)
        {
            request.Modified -= Save;
            Save();
            request.Dispose();
        }

        private void OnRequestsOnBeforeClear()
        {
            foreach (var request in requests)
            {
                request.Dispose();
            }
            Save();
        }

        [ContextMenu("Save")]
        private void Save()
        {
            Save(true);
            var assetObjects = requests.Cast<Object>().Where(o => o != null).ToList();
            assetObjects.Insert(0, this);
            InternalEditorUtility.SaveToSerializedFileAndForget(assetObjects.ToArray(), FilePath, true);
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            requests.Clear();
        }
    }
}