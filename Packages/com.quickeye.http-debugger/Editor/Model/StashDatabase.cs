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
        public RequestList requests = new();

        private void OnEnable()
        {
            requests.Added += OnRequestsOnAdded;
            requests.Removed += OnRequestsOnRemoved;
            requests.BeforeClear += OnRequestsOnBeforeClear;
        }

        private void OnDisable()
        {
            requests.Added -= OnRequestsOnAdded;
            requests.Removed -= OnRequestsOnRemoved;
            requests.BeforeClear -= OnRequestsOnBeforeClear;
        }

        void OnRequestsOnAdded(HDRequest request)
        {
            Save();
        }

        private void OnRequestsOnRemoved(HDRequest request)
        {
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