using System.Linq;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    public class CodeField : TextField
    {
        private const string LineScrollViewName = "line-number--scroll-view";
        private const string LineLabelName = "line-number--label";
        private Label _lineNumberLabel;
        private ScrollView _lineNumberScrollView;
        
        public CodeField()
        {
            this.InitResources();
            AddToClassList("code-field");
            this[0].style.flexDirection = FlexDirection.Row;
            multiline = true;
            SetVerticalScrollerVisibility(ScrollerVisibility.Auto);
            AddLineNumbers();
            RegisterScrollEvent();
        }

        private void AddLineNumbers()
        {
            _lineNumberLabel = new Label() { name = LineLabelName };
            _lineNumberScrollView = new ScrollView
            {
                name = LineScrollViewName,
                mode = ScrollViewMode.Vertical,
                verticalScrollerVisibility = ScrollerVisibility.Hidden,
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };
            _lineNumberScrollView.Add(_lineNumberLabel);
            _lineNumberScrollView.SetEnabled(false);
            this[0].Insert(0, _lineNumberScrollView);
        }

        private void RegisterScrollEvent()
        {
            // This will only work if CodeField was declared in UXML and had its multiline attribute set to true
            var contentScrollView = this.Q<ScrollView>();
            if (contentScrollView != null)
                contentScrollView.verticalScroller.valueChanged +=
                    v => _lineNumberScrollView.verticalScroller.value = v;
        }

        public override void SetValueWithoutNotify(string newValue)
        {
            base.SetValueWithoutNotify(newValue);
            RefreshCodeLines();
        }

        private void RefreshCodeLines()
        {
            if (_lineNumberLabel == null)
                return;

            var lineCount = text.Count(x => x == '\n') + 1;
            string lineNumbersText = "";
            for (int i = 1; i <= lineCount; ++i)
            {
                if (!string.IsNullOrEmpty(lineNumbersText))
                    lineNumbersText += "\n";

                lineNumbersText += i.ToString();
            }

            _lineNumberLabel.text = lineNumbersText;
        }

        public new class UxmlFactory : UxmlFactory<CodeField, UxmlTraits> { }

        public new class UxmlTraits : TextField.UxmlTraits { }
    }
}