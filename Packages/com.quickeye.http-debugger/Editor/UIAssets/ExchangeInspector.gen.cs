// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 1.6.0
// -----------------------
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    partial class ExchangeInspector
    {
        private TwoPaneSplitView exchangePane;
        private TemplateContainer requestViewRoot;
        private TemplateContainer responseViewRoot;
        private Label noSelectView;
    
        protected void AssignQueryResults(VisualElement root)
        {
            exchangePane = root.Q<TwoPaneSplitView>("exchange-pane");
            requestViewRoot = root.Q<TemplateContainer>("request-view-root");
            responseViewRoot = root.Q<TemplateContainer>("response-view-root");
            noSelectView = root.Q<Label>("no-select-view");
        }
    }
}