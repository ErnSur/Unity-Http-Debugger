using System;
using UnityEditor;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    [CreateAssetMenu]
    internal class ScriptableRequest : SingletonScriptableObject<ScriptableRequest>
    {
        [MenuItem("Test/Scriptable Request")]
        public static void Select() => Selection.activeObject = Instance;
        public static void Select(HDRequest request)
        {
            Selection.activeObject = Instance;
            Instance.request = request;
        }

        [SerializeField]
        private HDRequest request;

        private void OnValidate()
        {
            // Save request
            // in case of stash item, save to disk
            // playmode, nothing
        }

        // public HDRequest Request
        // {
        //     get => request;
        //     set
        //     {
        //         //var serObj =new SerializedObject(this);
        //         request = value;
        //         //EditorUtility.SetDirty(this);
        //         //serObj.Update();
        //     }
        // }
    }
}