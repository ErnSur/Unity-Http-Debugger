// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 1.6.0
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    partial class HeadersView
    {
        private MultiColumnListView headerList;
        private Column toggleCol;
        private Column nameCol;
        private Column valueCol;
    
        protected void AssignQueryResults(VisualElement root)
        {
            headerList = root.Q<MultiColumnListView>("header-list");
            toggleCol = headerList.columns["toggle-col"];
            nameCol = headerList.columns["name-col"];
            valueCol = headerList.columns["value-col"];
        }
    }
}
