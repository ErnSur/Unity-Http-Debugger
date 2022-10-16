using System.Collections.Generic;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    internal class DropdownButton : ToolbarButton
    {
        public readonly TextElement Label;
        public readonly VisualElement DropDownArea;

        private GenericMenu _dropdownMenu;

        public GenericMenu DropdownMenu
        {
            get => _dropdownMenu;
            set
            {
                DropDownArea.ToggleDisplayStyle(value != null);
                _dropdownMenu = value;
            }
        }

        public new string text
        {
            get => Label.text;
            set => Label.text = value;
        }

        public DropdownButton()
        {
            style.flexDirection = FlexDirection.Row;

            Label = new TextElement();
            Label.name = "label";
            Add(Label);

            var spacer = new VisualElement();
            spacer.name = "spacer";
            Add(spacer);

            var dropdownIcon = new VisualElement();
            dropdownIcon.name = "dropDown-icon";

            DropDownArea = new VisualElement();
            DropDownArea.RegisterCallback<MouseDownEvent>(evt =>
            {
                evt.PreventDefault();
                evt.StopImmediatePropagation();

                OnDropdownButtonClicked();
            });
            DropDownArea.name = "dropDownArea";
            DropDownArea.Add(dropdownIcon);
            Add(DropDownArea);
        }
        
        private void OnDropdownButtonClicked()
        {
            if (DropdownMenu == null)
                return;
            var menuPosition = new Vector2(layout.xMin, layout.center.y + 2);
            menuPosition = parent.LocalToWorld(menuPosition);
            var menuRect = new Rect(menuPosition, Vector2.zero);
            DropdownMenu.DropDown(menuRect);
        }

        private new class UxmlFactory : UxmlFactory<DropdownButton, UxmlTraits> { }

        private new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription { name = "text" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ((DropdownButton)ve).Label.text = m_Text.GetValueFromBag(bag, cc);
            }
        }
    }
}