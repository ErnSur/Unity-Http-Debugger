using System.Linq;
using QuickEye.UIToolkit;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    public class CodeField : VisualElement
    {
        [Q("VisualElement")]
        private VisualElement VisualElement;

        [Q("codeField-lineNumber--label")]
        private Label codeFieldLineNumberLabel;

        [Q("codeField--input")]
        private TextField textField;

        public TextField Field => textField;

        public CodeField()
        {
            this.InitResources();
            
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

            codeFieldLineNumberLabel.text = lineNumbersText;
        }

        public new class UxmlFactory : UxmlFactory<CodeField, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
        }
    }
}