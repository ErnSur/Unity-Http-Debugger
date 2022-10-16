// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 1.6.0
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    partial class RequestHeaderView
    {
        private VisualElement icon;
        private TextField nameField;
        private EnumField reqTypeMenu;
        private TextField reqUrlField;
        private Button reqSendButton;
    
        protected void AssignQueryResults(VisualElement root)
        {
            icon = root.Q<VisualElement>("icon");
            nameField = root.Q<TextField>("name-field");
            reqTypeMenu = root.Q<EnumField>("req-type-menu");
            reqUrlField = root.Q<TextField>("req-url-field");
            reqSendButton = root.Q<Button>("req-send-button");
        }
    }
}
