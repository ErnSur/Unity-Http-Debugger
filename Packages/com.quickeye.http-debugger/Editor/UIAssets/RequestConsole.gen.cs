// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 1.6.0
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    partial class RequestConsole
    {
        private QuickEye.RequestWatcher.DropdownButton clearButton;
        private MultiColumnListView requestList;
        private Column timeCol;
        private Column resultCol;
        private Column methodCol;
        private Column idCol;
        private Column urlCol;
    
        protected void AssignQueryResults(VisualElement root)
        {
            clearButton = root.Q<QuickEye.RequestWatcher.DropdownButton>("clear-button");
            requestList = root.Q<MultiColumnListView>("request-list");
            timeCol = requestList.columns["time-col"];
            resultCol = requestList.columns["result-col"];
            methodCol = requestList.columns["method-col"];
            idCol = requestList.columns["id-col"];
            urlCol = requestList.columns["url-col"];
        }
    }
}
