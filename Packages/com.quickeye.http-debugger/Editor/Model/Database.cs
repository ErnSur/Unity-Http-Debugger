using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    [CreateAssetMenu]
    internal class Database : ScriptableObject
    {
        public static Database Instance => GetInstance();
        private static Database _instance;
        public List<HDRequest> playmodeRequests;
        public List<HDRequest> stash;

        private const string LibraryPath = "Library/http-debugger-data.json";

        private static Database GetInstance()
        {
            return Resources.Load<Database>("Database");
            if (_instance == null)
            {
                _instance = Resources.FindObjectsOfTypeAll<Database>().FirstOrDefault();
                if (_instance == null)
                    _instance = LoadFromLibrary();
            }

            return _instance;
        }

        private void OnDestroy()
        {
//            SaveToLibrary();
        }
        
        [MenuItem("Http Debugger/Database")]
        public static void SelectDatabase()
        {
            Selection.activeObject = Instance;
        }

        [ContextMenu("Save")]
        private void SaveToLibrary()
        {
            var json = JsonUtility.ToJson(this);
            File.WriteAllText(LibraryPath, json);
            Debug.Log($"Saved DB");
        }

        private static Database LoadFromLibrary()
        {
            var json = File.ReadAllText(LibraryPath);
            var asset = CreateInstance<Database>();
            JsonUtility.FromJsonOverwrite(json, asset);
            Debug.Log($"Loaded DB");
            return asset;
        }
    }
}