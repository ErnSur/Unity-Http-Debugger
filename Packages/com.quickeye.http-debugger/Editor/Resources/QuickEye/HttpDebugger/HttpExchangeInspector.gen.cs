// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 1.6.0
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    partial class HttpExchangeInspector
    {
        private TwoPaneSplitView exchangePane;
        private TemplateContainer requestView;
        private TemplateContainer responseView;
        private Label noSelectView;
    
        protected void AssignQueryResults(VisualElement root)
        {
            exchangePane = root.Q<TwoPaneSplitView>("exchange-pane");
            requestView = root.Q<TemplateContainer>("request-view");
            responseView = root.Q<TemplateContainer>("response-view");
            noSelectView = root.Q<Label>("no-select-view");
        }
    }
}
