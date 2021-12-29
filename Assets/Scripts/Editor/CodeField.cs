using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    public class CodeField : BindableElement, INotifyValueChanged<string>
    {
        public new class UxmlFactory : UxmlFactory<CodeField, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            UxmlStringAttributeDescription value = new UxmlStringAttributeDescription { name = "value" };

            UxmlBoolAttributeDescription isDelayed = new UxmlBoolAttributeDescription
                { name = "delayed", defaultValue = true };

            UxmlBoolAttributeDescription isReadOnly = new UxmlBoolAttributeDescription { name = "readonly" };
            UxmlBoolAttributeDescription multiline = new UxmlBoolAttributeDescription { name = "multiline" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var label = ve as CodeField;
                label.SetValueWithoutNotify(value.GetValueFromBag(bag, cc));
                label.isDelayed = isDelayed.GetValueFromBag(bag, cc);
                label.multiline = multiline.GetValueFromBag(bag, cc);
                label.isReadOnly = isReadOnly.GetValueFromBag(bag, cc);
            }
        }

        private TextField textField;
        private Label lineNumbers;
        private VisualElement container;

        public bool multiline
        {
            get => textField.multiline;
            set => textField.multiline = value;
        }

        /// <summary> Whether the TextField inside this label is delayed </summary>
        public bool isDelayed
        {
            get => textField.isDelayed;
            set => textField.isDelayed = value;
        }

        /// <summary> The maximum character length of the TextField. -1 means no limit and it's the default.  </summary>
        public int maxLength
        {
            get => textField.maxLength;
            set => textField.maxLength = value;
        }

        public bool isReadOnly
        {
            get => textField.isReadOnly;
            set => textField.isReadOnly = value;
        }

        /// <summary> The string value of this element.</summary>
        public string value
        {
            get => textField.value;
            set => textField.value = value;
        }

        public CodeField()
        {
            var styleSuffix = EditorGUIUtility.isProSkin ? "Dark" : "Light";
            styleSheets.Add(Resources.Load<StyleSheet>($"QuickEye/CodeField-{styleSuffix}"));

            lineNumbers = new Label();
            textField = new TextField();
            Add(lineNumbers);
            Add(textField);

            textField.RegisterCallback<InputEvent>(_ => RefreshCodeLines());
            textField.RegisterValueChangedCallback(_ => RefreshCodeLines());
        }

        private void RefreshCodeLines()
        {
            var lineCount = textField.text.Count(x => x == '\n') + 1;
            string lineNumbersText = "";
            for (int i = 1; i <= lineCount; ++i)
            {
                if (!string.IsNullOrEmpty(lineNumbersText))
                    lineNumbersText += "\n";

                lineNumbersText += i.ToString();
            }

            lineNumbers.text = lineNumbersText;
        }

        public void SetValueWithoutNotify(string newValue)
        {
            textField.SetValueWithoutNotify(newValue);
            RefreshCodeLines();
        }
    }
}