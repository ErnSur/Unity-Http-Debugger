using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    public class VisualElementWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset _treeAsset;
        
        protected static void Open<T>(string title) where T : EditorWindow
        {
            var wnd = GetWindow<T>();
            wnd.titleContent = new GUIContent(title);
        }

        protected virtual void OnEnable()
        {
            _treeAsset.CloneTree(rootVisualElement);
        }
    }
}