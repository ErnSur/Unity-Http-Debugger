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


        public static VisualTreeAsset LoadTree<T>()
        {
            return Resources.Load<VisualTreeAsset>(BaseDir + typeof(T).Name);
        }

        private static void LoadThemeStyle(VisualElement ve)
        {
            var styleSuffix = EditorGUIUtility.isProSkin ? "Dark" : "Light";
            var styleSheet = Resources.Load<StyleSheet>($"QuickEye/{nameof(RequestButtonSmall)}-{styleSuffix}");
            ve.styleSheets.Add(styleSheet);
        }

        public static void InitFromUxml<T>(this T ve) where T : VisualElement
        {
            LoadTree<T>().CloneTree(ve);
            ve.AssignQueryResults(ve);
            ve.styleSheets.Add(Resources.Load<StyleSheet>(CommonStyle));
        }
    }
}