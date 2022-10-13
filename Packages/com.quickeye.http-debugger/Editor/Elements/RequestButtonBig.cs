using System;
using System.Collections.Generic;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    public class RequestButton : VisualElement
    {
        protected string FormatHttpMethodType(string value)
        {
            return value.Substring(0, 3).ToUpperInvariant();
        }
    }

    public class RequestButtonBig : RequestButton
    {
        public Action Deleted;
        public Action Duplicated;

        private readonly Label _typeLabel;
        private readonly EditableLabel _nameLabel;
        private readonly Button _dropdownButton;

        public RequestButtonBig()
        {
            this.InitResources();
            Add(_typeLabel = new Label().Class("rbb-type"));
            Add(_nameLabel = new EditableLabel().Class("rbb-name"));
            Add(_dropdownButton = new Button() { text = "▼" }.Clicked(() =>
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
            this.TrackPropertyChange(typeProp, p =>
            {
                var enumName = p.enumNames[p.enumValueIndex];
                _typeLabel.text = FormatHttpMethodType(enumName);
            });
            _nameLabel.BindProperty(nameProp);
        }

        public new class UxmlFactory : UxmlFactory<RequestButtonBig, UxmlTraits> { }

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
                ve.As<RequestButtonBig>()._nameLabel.value = _text.GetValueFromBag(bag, cc);
                ve.As<RequestButtonBig>()._typeLabel.text = _type.GetValueFromBag(bag, cc).ToString();
            }
        }
    }
}