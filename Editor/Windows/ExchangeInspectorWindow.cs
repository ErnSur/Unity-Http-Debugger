using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    [CustomEditor(typeof(RequestData), true)]
    internal class ExchangeInspectorWindow : UnityEditor.Editor
    {
        public static void Select(RequestData request, bool readOnly)
        {
            request.isReadOnly = readOnly;
            Selection.activeObject = request;
        }

        private ExchangeInspector _inspectorController;

        public override VisualElement CreateInspectorGUI()
        {
            var root = GetRoot();
            InitViewController(root);
            return root;
        }

        private void InitViewController(VisualElement root)
        {
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.quickeye.http-debugger/Editor/UIAssets/ExchangeInspector.uxml");
            uxml.CloneTree(root);
            _inspectorController = new ExchangeInspector(root);
            _inspectorController.Setup(serializedObject);
        }

        private VisualElement GetRoot()
        {
            var root = new VisualElement();
            root.name = "exchange-inspector";

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Packages/com.quickeye.http-debugger/Editor/UIAssets/ExchangeInspector.style.uss");

            root.RegisterCallback<AttachToPanelEvent>(evt =>
            {
                EnableClassForAllParents(root, "exchange-inspector-ancestor", true);
                evt.destinationPanel.visualTree.styleSheets.Add(styleSheet);
            });
            root.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                //change to destination panel to cause an exception
                evt.originPanel?.visualTree?.styleSheets.Remove(styleSheet);
            });
            return root;
        }

        private static void EnableClassForAllParents(VisualElement root, string className, bool enabled)
        {
            root = root.hierarchy.parent;
            while (root != null)
            {
                root.EnableInClassList(className, enabled);
                root = root.hierarchy.parent;
            }
        }

        public override bool UseDefaultMargins() => false;
        protected override void OnHeaderGUI() { }
    }
}