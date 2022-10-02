// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 1.6.0
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    partial class ResponseView
    {
        private Label resStatusLabel;
        private QuickEye.RequestWatcher.Tab bodyTab;
        private QuickEye.RequestWatcher.Tab headersTab;
        private QuickEye.RequestWatcher.Tab cookieTab;
        private QuickEye.RequestWatcher.CodeField resBodyField;
        private VisualElement headersView;
        private Label loadingOverlay;
    
        protected void AssignQueryResults(VisualElement root)
        {
            resStatusLabel = root.Q<Label>("res-status-label");
            bodyTab = root.Q<QuickEye.RequestWatcher.Tab>("body-tab");
            headersTab = root.Q<QuickEye.RequestWatcher.Tab>("headers-tab");
            cookieTab = root.Q<QuickEye.RequestWatcher.Tab>("cookie-tab");
            resBodyField = root.Q<QuickEye.RequestWatcher.CodeField>("res-body-field");
            headersView = root.Q<VisualElement>("headers-view");
            loadingOverlay = root.Q<Label>("loading-overlay");
        }
    }
}
