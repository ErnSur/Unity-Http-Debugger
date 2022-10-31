using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace QuickEye.DependencyMessage.WebTools
{
    [CreateAssetMenu]
    internal class DependencyMessage : ScriptableObject
    {
        [InitializeOnLoadMethod]
        private static void ShowMessage()
        {
            var messageData = LoadMessage();
            if (!messageData.ShouldShowMessage())
                return;
            var packageManifestJson = messageData.GetPackageManifestJson();
            var packageName = packageManifestJson?.displayName;
            var title = $"{packageName} has missing dependency";

            var messageSb = new StringBuilder();
            messageSb.AppendLine($"{packageName} has been added to the project but it is missing its dependencies.");
            messageSb.AppendLine($"Add following packages:");

            foreach (var gitDependency in messageData.dependencies)
            {
                messageSb.AppendLine($"- {gitDependency.name}");
            }

            var result = EditorUtility.DisplayDialogComplex(title, messageSb.ToString(), "Install automatically",
                "Cancel",
                "Cancel, never show again");

            switch (result)
            {
                case 0:
                    AddPackages(messageData.dependencies);
                    break;
                case 1:
                    break;
                case 2:
                    messageData.SetNeverShowFlag(true);
                    break;
            }
        }

        [SerializeField]
        private PackageManifest packageManifest;

        public GitDependency[] dependencies;

        private bool ShouldShowMessage()
        {
            if (!TryGetNeverShowPrefKey(out var key))
                return false;
            return !EditorPrefs.GetBool(key, false);
        }

        private void SetNeverShowFlag(bool enabled)
        {
            if (!TryGetNeverShowPrefKey(out var key))
                return;
            EditorPrefs.SetBool(key, enabled);
        }

        private bool TryGetNeverShowPrefKey(out string key)
        {
            if (packageManifest == null)
            {
                key = null;
                return false;
            }

            var packageManifestGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(packageManifest));
            key = $"NeverShow-{packageManifestGuid}";
            return true;
        }

        private static void AddPackages(GitDependency[] dependencies)
        {
            foreach (var gitDependency in dependencies)
            {
                UnityEditor.PackageManager.Client.Add(gitDependency.gitUrl);
            }
        }

        private static DependencyMessage LoadMessage()
        {
            var messages = Resources.LoadAll<DependencyMessage>("");
            return messages.FirstOrDefault();
        }
        
        [ContextMenu("Add Packages")]
        private void AddPackages()
        {
            AddPackages(dependencies);
        }

        private PackageManifestJson GetPackageManifestJson()
        {
            return packageManifest == null ? null : JsonUtility.FromJson<PackageManifestJson>(packageManifest.text);
        }
    }

    [Serializable]
    internal class GitDependency
    {
        public string name;
        public string gitUrl;
    }

    [Serializable]
    internal class PackageManifestJson
    {
        public string name;
        public string displayName;
    }
}