using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal class TabGroup : VisualElement
    {
        public TabGroup()
        {
            this.InitResources();
        }

        private class UxmlFactory : UxmlFactory<TabGroup>
        {
        }
    }
}