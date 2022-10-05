using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    [CustomEditor(typeof(ScriptableRequest))]
    internal class ScriptableRequestEditor : Editor
    {
        private ExchangeInspector _inspectorController;
        private VisualElement _fullWindowRoot;
        private ScriptableRequest Target => (ScriptableRequest)target;
        private bool IsReadOnly => Target.isReadOnly;

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
            _inspectorController.Setup(serializedObject.FindProperty("request"), IsReadOnly);
        }

        private VisualElement GetRoot()
        {
            var root = new VisualElement();
            root.name = "======ROOT===";
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