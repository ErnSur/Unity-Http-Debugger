using System;
using System.Collections.Generic;
using ArteHacker.UITKEditorAid;
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

        private readonly Label typeLabel;
        private readonly EditableLabel nameLabel;
        private readonly Button dropdownButton;

        public RequestButtonBig()
        {
            var styleSuffix = EditorGUIUtility.isProSkin ? "Dark" : "Light";
            var styleSheet = Resources.Load<StyleSheet>($"QuickEye/{nameof(RequestButtonBig)}-{styleSuffix}");
            styleSheets.Add(styleSheet);

            Add(typeLabel = new Label()
                .Class("rbb-type"));

            Add(nameLabel = new EditableLabel()
                .Class("rbb-name"));
            Add(dropdownButton = new Button(){text = "▼"}.Clicked(() =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Duplicate"), false, () => Duplicated?.Invoke());
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, () => Deleted?.Invoke());
                menu.ShowAsContext();
            }));
            RegisterCallback<MouseEnterEvent>(evt => { dropdownButton.ToggleDisplayStyle(true); });

            RegisterCallback<MouseLeaveEvent>(evt => { dropdownButton.ToggleDisplayStyle(false); });
            
            dropdownButton.ToggleDisplayStyle(false);
        }

        public void BindProperties(SerializedProperty typeProp, SerializedProperty nameProp)
        {
            this.TrackPropertyChange(typeProp, p =>
            {
                var enumName = p.enumNames[p.enumValueIndex];
                typeLabel.text = FormatHttpMethodType(enumName);
            });
            nameLabel.BindProperty(nameProp);
        }

        public new class UxmlFactory : UxmlFactory<RequestButtonBig, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription text = new UxmlStringAttributeDescription()
                { name = "text", defaultValue = "New Request" };

            private UxmlEnumAttributeDescription<HttpMethodType> type =
                new UxmlEnumAttributeDescription<HttpMethodType>() { name = "type" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ve.As<RequestButtonBig>().nameLabel.value = text.GetValueFromBag(bag, cc);
                ve.As<RequestButtonBig>().typeLabel.text = type.GetValueFromBag(bag, cc).ToString();
            }
        }
    }
}