// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 1.7.0
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    partial class RequestView
    {
        private QuickEye.UIToolkit.TabDropdown bodyTab;
        private QuickEye.UIToolkit.TabDropdown authTab;
        private QuickEye.UIToolkit.Tab headersTab;
        private QuickEye.UIToolkit.Tab stackTraceTab;
        private QuickEye.WebTools.Editor.CodeField reqBodyField;
        private TemplateContainer headersView;
        private ScrollView stackTraceView;
        private Label stackTraceLabel;
    
        protected void AssignQueryResults(VisualElement root)
        {
            bodyTab = root.Q<QuickEye.UIToolkit.TabDropdown>("body-tab");
            authTab = root.Q<QuickEye.UIToolkit.TabDropdown>("auth-tab");
            headersTab = root.Q<QuickEye.UIToolkit.Tab>("headers-tab");
            stackTraceTab = root.Q<QuickEye.UIToolkit.Tab>("stack-trace-tab");
            reqBodyField = root.Q<QuickEye.WebTools.Editor.CodeField>("req-body-field");
            headersView = root.Q<TemplateContainer>("headers-view");
            stackTraceView = root.Q<ScrollView>("stack-trace-view");
            stackTraceLabel = root.Q<Label>("stack-trace-label");
        }
    }
}
