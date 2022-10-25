// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 1.7.0
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    partial class ResponseView
    {
        private QuickEye.UIToolkit.TabDropdown bodyTab;
        private QuickEye.UIToolkit.Tab headersTab;
        private QuickEye.UIToolkit.Tab cookieTab;
        private Label resStatusLabel;
        private QuickEye.WebTools.Editor.CodeField resBodyField;
        private TemplateContainer headersView;
        private Label loadingOverlay;
    
        protected void AssignQueryResults(VisualElement root)
        {
            bodyTab = root.Q<QuickEye.UIToolkit.TabDropdown>("body-tab");
            headersTab = root.Q<QuickEye.UIToolkit.Tab>("headers-tab");
            cookieTab = root.Q<QuickEye.UIToolkit.Tab>("cookie-tab");
            resStatusLabel = root.Q<Label>("res-status-label");
            resBodyField = root.Q<QuickEye.WebTools.Editor.CodeField>("res-body-field");
            headersView = root.Q<TemplateContainer>("headers-view");
            loadingOverlay = root.Q<Label>("loading-overlay");
        }
    }
}
