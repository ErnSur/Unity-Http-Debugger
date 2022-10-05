using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    // TODO: support selection of multiple request at one time with locked inspectors
    // create new Scriptable object for each request and destroy it in editor OnDestroy/onselection change
    [CustomEditor(typeof(ScriptableRequest))]
    internal class ExchangeInspectorWindow : Editor
    {
        public static void Open()
        {
            var sr = ScriptableRequest.Instance;
            Selection.activeObject = sr;
        }
        public static void Select(HDRequest request, bool readOnly = false)
        {
            var sr = ScriptableRequest.Instance;
            Selection.activeObject = sr;
            sr.request = request;
            sr.isReadOnly = readOnly;
        }
        private ExchangeInspector _inspectorController;
        private VisualElement _fullWindowRoot;
        private ScriptableRequest Target => (ScriptableRequest)target;
        private bool _isReadOnly;

        public override VisualElement CreateInspectorGUI()
        {
            var root = GetRoot();
            InitViewController();
            return root;
        }

        private void OnDestroy()
        {
            Debug.Log($"Destroyed Editor");
            // Destroy scriptable object here
        }

        private void InitViewController()
        {
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.quickeye.http-debugger/Editor/UIAssets/ExchangeInspector.uxml");
            uxml.CloneTree(_fullWindowRoot);
            _inspectorController = new ExchangeInspector(_fullWindowRoot);
            Debug.Log($"readonly {_isReadOnly}");
            _inspectorController.Setup(serializedObject.FindProperty("request"), _isReadOnly);
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
        
        internal class ScriptableRequest : SingletonScriptableObject<ScriptableRequest>
        {
            public HDRequest request;
            public bool isReadOnly;
            private void OnValidate()
            {
                // Save request
                // in case of stash item, save to disk
                // playmode, nothing
            }
        }
    }
}