// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 1.6.0
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    partial class RequestConsole
    {
        private QuickEye.WebTools.Editor.DropdownButton clearButton;
        private UnityEditor.UIElements.ToolbarSearchField searchField;
        private MultiColumnListView requestList;
        private Column timeCol;
        private Column methodCol;
        private Column idCol;
        private Column urlCol;
        private Column resultCol;
    
        protected void AssignQueryResults(VisualElement root)
        {
            clearButton = root.Q<QuickEye.WebTools.Editor.DropdownButton>("clear-button");
            searchField = root.Q<UnityEditor.UIElements.ToolbarSearchField>("search-field");
            requestList = root.Q<MultiColumnListView>("request-list");
            timeCol = requestList.columns["time-col"];
            methodCol = requestList.columns["method-col"];
            idCol = requestList.columns["id-col"];
            urlCol = requestList.columns["url-col"];
            resultCol = requestList.columns["result-col"];
        }
    }
}
