// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 1.6.0
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    partial class HttpDebuggerWindow
    {
        private QuickEye.RequestWatcher.Tab stashTab;
        private QuickEye.RequestWatcher.Tab playmodeTab;
        private QuickEye.RequestWatcher.Tab mockTab;
        private QuickEye.RequestWatcher.StashView stashView;
        private QuickEye.RequestWatcher.PlaymodeView playmodeView;
    
        protected void AssignQueryResults(VisualElement root)
        {
            stashTab = root.Q<QuickEye.RequestWatcher.Tab>("stash-tab");
            playmodeTab = root.Q<QuickEye.RequestWatcher.Tab>("playmode-tab");
            mockTab = root.Q<QuickEye.RequestWatcher.Tab>("mock-tab");
            stashView = root.Q<QuickEye.RequestWatcher.StashView>("stash-view");
            playmodeView = root.Q<QuickEye.RequestWatcher.PlaymodeView>("playmode-view");
        }
    }
}
