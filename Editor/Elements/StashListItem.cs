using System;
using System.Collections.Generic;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    public class StashListItem : VisualElement
    {
        public Action Deleted;
        public Action Duplicated;

        private readonly Label _typeLabel;
        private readonly Label _nameLabel;
        private readonly Label _dropdownButton;

        public StashListItem()
        {
            this.InitResources();
            AddToClassList("stash-list-item");
            Add(_typeLabel = new Label().Class("stash-list-item__type-label"));
            Add(_nameLabel = new Label().Class("stash-list-item__name-label"));
            Add(_dropdownButton = new Label("▼").Class("stash-list-item__dropdown").Clicked(() =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Duplicate"), false, () => Duplicated?.Invoke());
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, () => Deleted?.Invoke());
                menu.ShowAsContext();
            }));
            RegisterCallback<MouseEnterEvent>(evt => { _dropdownButton.ToggleDisplayStyle(true); });

            RegisterCallback<MouseLeaveEvent>(evt => { _dropdownButton.ToggleDisplayStyle(false); });

            _dropdownButton.ToggleDisplayStyle(false);
        }

        public void BindProperties(SerializedProperty typeProp, SerializedProperty nameProp)
        {
            _typeLabel.text = EnumIndexToDisplayText(typeProp.enumValueIndex);
            _typeLabel.TrackPropertyValue(typeProp,
                p => { _typeLabel.text = EnumIndexToDisplayText(p.enumValueIndex); });
            _nameLabel.BindProperty(nameProp);
        }

        private static string EnumIndexToDisplayText(int i) => ((HttpMethodType)i).ToString()[..3].ToUpperInvariant();

        public new class UxmlFactory : UxmlFactory<StashListItem, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription _text = new UxmlStringAttributeDescription()
                { name = "text", defaultValue = "New Request" };

            private UxmlEnumAttributeDescription<HttpMethodType> _type =
                new UxmlEnumAttributeDescription<HttpMethodType>() { name = "type" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ve.As<StashListItem>()._nameLabel.text = _text.GetValueFromBag(bag, cc);
                ve.As<StashListItem>()._typeLabel.text = _type.GetValueFromBag(bag, cc).ToString();
            }
        }
    }
}