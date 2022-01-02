using QuickEye.UIToolkit;
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

        public static void InitFromUxml<T>(this T ve) where T : VisualElement
        {
            LoadTree<T>().CloneTree(ve);
            ve.AssignQueryResults(ve);
            ve.styleSheets.Add(Resources.Load<StyleSheet>(CommonStyle));
        }
    }
}