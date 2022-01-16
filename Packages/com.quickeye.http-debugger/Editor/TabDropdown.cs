using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal class TabDropdown : Tab
    {
        public event Action<GenericMenu> BeforeMenuShow;
        public TabDropdown()
        {
            hierarchy.Clear();
            this.InitResources();
        }

        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);
            if (newValue)
            {
                clicked += ShowMenu;
            }
            else
            {
                clicked -= ShowMenu;
            }
        }

        private void ShowMenu()
        {
            var menu = new GenericMenu();
            BeforeMenuShow?.Invoke(menu);
            menu.ShowAsContext();
        }
        
        private class UxmlFactory : UxmlFactory<TabDropdown, UxmlTraits>
        {
        }

        private class UxmlTraits : Tab.UxmlTraits
        {
        }
    }
}