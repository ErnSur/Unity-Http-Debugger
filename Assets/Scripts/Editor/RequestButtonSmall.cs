using System.Collections.Generic;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    public class RequestButtonSmall : RequestButton
    {
        private readonly Label typeLabel;
        private readonly Label nameLabel;

        public RequestButtonSmall()
        {
            var styleSuffix = EditorGUIUtility.isProSkin ? "Dark" : "Light";
            var styleSheet = Resources.Load<StyleSheet>($"QuickEye/{nameof(RequestButtonSmall)}-{styleSuffix}");
            styleSheets.Add(styleSheet);

            // TODO: Truncate type value to only first 3 chars and make it uppper case
            Add(typeLabel = new Label()
                .Class("rbb-type"));

            Add(nameLabel = new Label()
                .Class("rbb-name"));
        }

        public void BindProperties(SerializedProperty typeProp, SerializedProperty nameProp)
        {
            nameLabel.BindProperty(nameProp);
            this.TrackPropertyChange(typeProp, p =>
            {
                var enumName = p.enumNames[p.enumValueIndex];
                typeLabel.text = FormatHttpMethodType(enumName);
            });
        }

        public new class UxmlFactory : UxmlFactory<RequestButtonSmall, UxmlTraits>
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
                ve.As<RequestButtonSmall>().nameLabel.text = text.GetValueFromBag(bag, cc);
                ve.As<RequestButtonSmall>().typeLabel.text = type.GetValueFromBag(bag, cc).ToString();
            }
        }
    }
}