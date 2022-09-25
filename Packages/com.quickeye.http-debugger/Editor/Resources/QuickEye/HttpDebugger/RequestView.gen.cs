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
        private QuickEye.RequestWatcher.Tab bodyTab;
        private QuickEye.RequestWatcher.TabDropdown authTab;
        private QuickEye.RequestWatcher.Tab headersTab;
        private QuickEye.RequestWatcher.CodeField reqBodyField;
        private VisualElement headersView;
    
        protected void AssignQueryResults(VisualElement root)
        {
            reqTypeMenu = root.Q<EnumField>("req-type-menu");
            reqUrlField = root.Q<TextField>("req-url-field");
            reqSendButton = root.Q<UnityEditor.UIElements.ToolbarButton>("req-send-button");
            bodyTab = root.Q<QuickEye.RequestWatcher.Tab>("body-tab");
            authTab = root.Q<QuickEye.RequestWatcher.TabDropdown>("auth-tab");
            headersTab = root.Q<QuickEye.RequestWatcher.Tab>("headers-tab");
            reqBodyField = root.Q<QuickEye.RequestWatcher.CodeField>("req-body-field");
            headersView = root.Q<VisualElement>("headers-view");
        }
    }
}
