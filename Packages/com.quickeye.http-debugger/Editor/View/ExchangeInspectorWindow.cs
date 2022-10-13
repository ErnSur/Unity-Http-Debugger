using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;

namespace QuickEye.RequestWatcher
{
    [CustomEditor(typeof(ScriptableRequest))]
    internal class ExchangeInspectorWindow : Editor
    {
        private static Dictionary<HDRequest, ScriptableRequest> _scriptableRequests;
        private static readonly Dictionary<ScriptableRequest, int> _UsedTargets = new();

        public static void Select(HDRequest request, bool readOnly = false)
        {
            InitRequestDic();
            var sr = GetTargetFor(request);
            sr.isReadOnly = readOnly;
            Selection.activeObject = sr;
        }

        private static void InitRequestDic()
        {
            if (_scriptableRequests != null)
                return;
            _scriptableRequests = _UsedTargets
                .ToDictionary(kvp => kvp.Key.request, kvp => kvp.Key);
        }

        private static ScriptableRequest GetTargetFor(HDRequest request)
        {
            DestroyUnusedTargets();

            if (_scriptableRequests.TryGetValue(request, out var sr))
                return sr;
            sr = CreateInstance<ScriptableRequest>();
            _scriptableRequests[request] = sr;
            sr.request = request;
            return sr;
        }

        private static void DestroyUnusedTargets()
        {
            var unusedKeys = _scriptableRequests
                .Where(kvp => !_UsedTargets.ContainsKey(kvp.Value))
                .Select(kvp => kvp.Key)
                .ToArray();
            foreach (var key in unusedKeys)
            {
                DestroyImmediate(_scriptableRequests[key]);
                _scriptableRequests.Remove(key);
            }
        }

        private ExchangeInspector _inspectorController;
        private VisualElement _fullWindowRoot;
        private ScriptableRequest Target => (ScriptableRequest)target;

        public override VisualElement CreateInspectorGUI()
        {
            var root = GetRoot();
            InitViewController();
            return root;
        }
        
        private void Awake()
        {
            if (!_UsedTargets.ContainsKey(Target))
            {
                _UsedTargets[Target] = 0;
            }

            _UsedTargets[Target]++;
        }

        private void OnDestroy()
        { 
            if(!_UsedTargets.ContainsKey(Target))
                return;
            
            _UsedTargets[Target]--;
            if (_UsedTargets[Target] == 0)
            {
                _UsedTargets.Remove(Target);
            }
        }

        private void InitViewController()
        {
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.quickeye.http-debugger/Editor/UIAssets/ExchangeInspector.uxml");
            uxml.CloneTree(_fullWindowRoot);
            _inspectorController = new ExchangeInspector(_fullWindowRoot);
            var readOnly = serializedObject.FindProperty("isReadOnly").boolValue;
            _inspectorController.Setup(serializedObject.FindProperty("request"), readOnly);
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