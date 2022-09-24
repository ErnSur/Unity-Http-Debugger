using QuickEye.UIToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal static class HttpDebuggerResources
    {
        private const string BaseDir = "QuickEye/HttpDebugger/";
        private const string CommonStyle = BaseDir + "Common";
        
        private static T LoadAsset<T>(string resourcesRelativePath) where T : Object
        {
            var baseResourcesPath = "Packages/com.quickeye.http-debugger/Editor/Resources/";
            var extension = typeof(T) == typeof(StyleSheet) ? ".uss" : ".uxml";
            return AssetDatabase.LoadAssetAtPath<T>(
                $"{baseResourcesPath}{resourcesRelativePath}{extension}");
        }

        public static bool TryLoadTree<T>(out VisualTreeAsset tree)
        {
            tree = LoadAsset<VisualTreeAsset>(BaseDir + typeof(T).Name);
            return tree != null;
        }

        private static bool TryLoadThemeStyle<T>(out StyleSheet styleSheet) where T : VisualElement
        {
            var styleSuffix = EditorGUIUtility.isProSkin ? "Dark" : "Light";
            styleSheet = LoadAsset<StyleSheet>($"{BaseDir}{typeof(T).Name}-{styleSuffix}");
            return styleSheet != null;
        }

        private static bool TryLoadStyle<T>(out StyleSheet styleSheet) where T : VisualElement
        {
            styleSheet = LoadAsset<StyleSheet>($"{BaseDir}{typeof(T).Name}");
            if (typeof(T) == typeof(Tab) && styleSheet == null)
                Debug.Log($"TryLoad Style: {typeof(T).Name} / is null = {styleSheet == null}");
            return styleSheet != null;
        }

        public static void InitResources<T>(this T ve) where T : VisualElement
        {
            if (TryLoadTree<T>(out var tree))
            {
                tree.CloneTree(ve);
                ve.AssignQueryResults(ve);
            }

            ve.styleSheets.Add(Resources.Load<StyleSheet>(CommonStyle));

            if (TryLoadThemeStyle<T>(out var themeStyleSheet))
                ve.styleSheets.Add(themeStyleSheet);

            if (TryLoadStyle<T>(out var styleSheet))
                ve.styleSheets.Add(styleSheet);
        }
    }
}