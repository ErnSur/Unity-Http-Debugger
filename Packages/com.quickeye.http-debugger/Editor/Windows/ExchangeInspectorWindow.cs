using UnityEditor;
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
        private VisualElement _fullWindowRoot;

        public override VisualElement CreateInspectorGUI()
        {
            var root = GetRoot();
            InitViewController();
            return root;
        }

        private void InitViewController()
        {
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.quickeye.http-debugger/Editor/UIAssets/ExchangeInspector.uxml");
            uxml.CloneTree(_fullWindowRoot);
            _inspectorController = new ExchangeInspector(_fullWindowRoot);
            var readOnly = serializedObject.FindProperty("isReadOnly").boolValue;
            _inspectorController.Setup(serializedObject);
        }

        private VisualElement GetRoot()
        {
            var root = new VisualElement();
            _fullWindowRoot = new VisualElement();
            StretchVe(_fullWindowRoot);

            root.RegisterCallback<AttachToPanelEvent>(evt =>
            {
                var fullParent = evt.destinationPanel.visualTree.Q(null, "unity-inspector-main-container");
                fullParent.Add(_fullWindowRoot);
            });
            root.RegisterCallback<DetachFromPanelEvent>(evt => { _fullWindowRoot.RemoveFromHierarchy(); });
            return root;
        }

        private static void StretchVe(VisualElement ve)
        {
            ve.style.position = Position.Absolute;
            ve.style.bottom = ve.style.top = ve.style.left = ve.style.right = 0;
        }

        public override bool UseDefaultMargins() => false;
        protected override void OnHeaderGUI() { }
    }
}