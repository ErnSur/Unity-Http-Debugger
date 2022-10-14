using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QuickEye.RequestWatcher
{
    [FilePath(FilePath, FilePathAttribute.Location.ProjectFolder)]
    internal class RequestConsoleDatabase : ScriptableSingleton<RequestConsoleDatabase>
    {
        public const string FilePath = "Logs/http.log";

        public RequestList requests;

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

        private void OnRequestsOnAdded(HDRequest request)
        {
            Save();
        }

        private void OnRequestsOnBeforeClear()
        {
            foreach (var request in requests)
            {
                request.Dispose();
            }

            Save();
        }

        private void OnRequestsOnRemoved(HDRequest request)
        {
            Save();
            request.Dispose();
        }

        [ContextMenu("Save")]
        private void Save()
        {
            Save(true);
            var assetObjects = requests.Cast<Object>().ToList();
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