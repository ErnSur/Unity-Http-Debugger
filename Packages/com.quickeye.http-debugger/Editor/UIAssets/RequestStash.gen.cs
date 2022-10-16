// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 1.6.0
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    partial class RequestStash
    {
        private UnityEditor.UIElements.ToolbarButton createButton;
        private UnityEditor.UIElements.ToolbarSearchField searchField;
        private ListView stashList;
    
        protected void AssignQueryResults(VisualElement root)
        {
            createButton = root.Q<UnityEditor.UIElements.ToolbarButton>("create--button");
            searchField = root.Q<UnityEditor.UIElements.ToolbarSearchField>("search-field");
            stashList = root.Q<ListView>("stash--list");
        }
    }
}
