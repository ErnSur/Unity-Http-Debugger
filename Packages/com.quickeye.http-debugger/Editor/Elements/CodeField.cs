using System.Linq;
using QuickEye.UIToolkit;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    public class CodeField : VisualElement
    {
        public TextField Field => codeFieldInput;
        private Label codeFieldLineNumberLabel;
        private TextField codeFieldInput;
        public CodeField()
        {
            this.InitResources();
            CreateTree();
            codeFieldInput.RegisterCallback<InputEvent>(_ => RefreshCodeLines());
            codeFieldInput.RegisterValueChangedCallback(_ => RefreshCodeLines());
        }

        private void CreateTree()
        {
            var scrollView = new ScrollView();
            scrollView.AddToClassList("common-variables");
            scrollView.AddToClassList("codeField-container");
            var container = new VisualElement();
            container.AddToClassList("codeField-scrollView--container");
            codeFieldLineNumberLabel = new Label();
            codeFieldLineNumberLabel.name = "codeField-lineNumber--label";
            codeFieldInput = new TextField();
            codeFieldInput.name = "codeField--input";
            codeFieldInput.style.flexGrow = 1;
            codeFieldInput.multiline = true;
            
            Add(scrollView);
            scrollView.Add(container);
            container.Insert(0,codeFieldLineNumberLabel);
            container.Add(codeFieldInput);
        }

        private void RefreshCodeLines()
        {
            var lineCount = codeFieldInput.text.Count(x => x == '\n') + 1;
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