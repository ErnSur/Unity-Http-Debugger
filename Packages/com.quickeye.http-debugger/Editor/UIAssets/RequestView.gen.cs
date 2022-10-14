// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 1.6.0
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    partial class RequestView
    {
        private EnumField reqTypeMenu;
        private TextField reqUrlField;
        private UnityEditor.UIElements.ToolbarButton reqSendButton;
        private QuickEye.UIToolkit.Tab bodyTab;
        private QuickEye.UIToolkit.TabDropdown authTab;
        private QuickEye.UIToolkit.Tab headersTab;
        private QuickEye.RequestWatcher.CodeField reqBodyField;
        private TemplateContainer headersView;
    
        protected void AssignQueryResults(VisualElement root)
        {
            reqTypeMenu = root.Q<EnumField>("req-type-menu");
            reqUrlField = root.Q<TextField>("req-url-field");
            reqSendButton = root.Q<UnityEditor.UIElements.ToolbarButton>("req-send-button");
            bodyTab = root.Q<QuickEye.UIToolkit.Tab>("body-tab");
            authTab = root.Q<QuickEye.UIToolkit.TabDropdown>("auth-tab");
            headersTab = root.Q<QuickEye.UIToolkit.Tab>("headers-tab");
            reqBodyField = root.Q<QuickEye.RequestWatcher.CodeField>("req-body-field");
            headersView = root.Q<TemplateContainer>("headers-view");
        }
    }
}
