using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace QuickEye.WebTools.Editor
{
    internal static class ScriptableSingletonUtility
    {
        public static void SaveToSerializedFileAndForgetSafe(Object[] objectsToSave, string filePath, bool saveAsText)
        {
            if (string.IsNullOrEmpty(filePath))
                return;
            var directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            InternalEditorUtility.SaveToSerializedFileAndForget(objectsToSave, filePath, saveAsText);
        }
    }
}