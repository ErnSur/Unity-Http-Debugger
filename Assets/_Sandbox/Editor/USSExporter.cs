using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.Editor
{
    internal class UssExporter : EditorWindow
    {
        private static AssetBundle EditorAssetBundle;

        [MenuItem("Window/UI Toolkit/Uss Exporter")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<UssExporter>();
            wnd.titleContent = new GUIContent("Uss Exporter");
        }

        private string[] paths;
        private string[] searchResult;
        private ListView listView;
        [SerializeField]
        private string searchString;

        

        private void OnEnable()
        {
            if (EditorAssetBundle == null)
                EditorAssetBundle = GetEditorAssetBundle();
            paths = GetUssPaths();
        }

        private void CreateGUI()
        {
            var searchField = new ToolbarSearchField();
            var toolbar = new Toolbar();
            toolbar.Add(searchField);
            rootVisualElement.Add(toolbar);
            searchField.RegisterValueChangedCallback(evt =>
            {
                if (string.IsNullOrWhiteSpace(evt.newValue))
                {
                    Setup(paths);
                    return;
                }

                searchResult = (from path in paths
                    where path.ToLower().Contains(evt.newValue.ToLower())
                    select path).ToArray();

                Setup(searchResult);
            });
            listView = new ListView();
            listView.style.flexGrow = 1;
            listView.fixedItemHeight = 16;
            listView.makeItem = () => new Label();
            listView.bindItem = (element, i) =>
            {
                var e = (Label)element;
                e.text = listView.itemsSource[i].ToString();
            };
            listView.itemsSource = paths;
            listView.onItemsChosen += objects =>
            {
                foreach (string path in objects)
                {
                    Export(path);
                }
            };
            rootVisualElement.Add(listView);
        }

        private void Setup(IList list)
        {
            listView.itemsSource = list;
            listView.Rebuild();
        }

        public static void Export(string sheetPath)
        {
            var sheet = EditorGUIUtility.Load(sheetPath) as StyleSheet;
            var projectPath = $"Assets/Uss/{Path.GetFileName(sheetPath)}";

            if (Path.GetExtension(projectPath) == ".asset")
                projectPath = Path.ChangeExtension(projectPath, null);

            Directory.CreateDirectory(Path.GetDirectoryName(projectPath));
            Debug.Log($"created at: {projectPath}");
            WriteStyleSheet(sheet, projectPath);
            AssetDatabase.ImportAsset(projectPath);
        }

        public static string[] GetUssPaths()
        {
            return (from path in EditorAssetBundle.GetAllAssetNames()
                let asset = EditorAssetBundle.LoadAsset<StyleSheet>(path)
                where asset != null
                select path).ToArray();
        }

        public static void WriteStyleSheet(StyleSheet sheet, string path)
        {
            var ass = typeof(EditorWindow).Assembly;
            var type = ass.GetType("UnityEditor.StyleSheets.StyleSheetToUss");
            var method = type.GetMethod(
                "WriteStyleSheet",
                BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, new object[] { sheet, path, null });
        }

        private static AssetBundle GetEditorAssetBundle()
        {
            var editorGUIUtility = typeof(EditorGUIUtility);
            var getEditorAssetBundle = editorGUIUtility.GetMethod(
                "GetEditorAssetBundle",
                BindingFlags.NonPublic | BindingFlags.Static);

            return (AssetBundle)getEditorAssetBundle.Invoke(null, null);
        }
    }
}