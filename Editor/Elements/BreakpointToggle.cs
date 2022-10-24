using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    internal class BreakpointToggle : Toggle
    {
        private const string BreakpointHoverClass = "breakpoint-toggle--hover";
        private string _breakpointName;

        public string BreakpointName
        {
            get => _breakpointName;
            set
            {
                if (_breakpointName == value)
                    return;
                _breakpointName = value;
                SetValueWithoutNotify(IsBreakpointOn());
            }
        }

        public BreakpointToggle()
        {
            this.InitResources();
            AddToClassList("breakpoint-toggle");
            SetBindingCallbacks();
            RegisterCallback<AttachToPanelEvent>(OnAttach);
            RegisterCallback<DetachFromPanelEvent>(OnDetach);
        }

        private void OnDetach(DetachFromPanelEvent evt)
        {
            UnregisterHoverElement(parent);
        }

        private void OnAttach(AttachToPanelEvent evt)
        {
            RegisterHoverElement(parent);
        }

        private void RegisterHoverElement(VisualElement element)
        {
            element.RegisterCallback<MouseEnterEvent>(EnableHoverStyle);
            element.RegisterCallback<MouseLeaveEvent>(DisableHoverStyle);
        }

        private void UnregisterHoverElement(VisualElement element)
        {
            element.UnregisterCallback<MouseEnterEvent>(EnableHoverStyle);
            element.UnregisterCallback<MouseLeaveEvent>(DisableHoverStyle);
        }

        private void EnableHoverStyle(IMouseEvent _) => EnableInClassList(BreakpointHoverClass, true);
        private void DisableHoverStyle(IMouseEvent _) => EnableInClassList(BreakpointHoverClass, false);

        private bool IsBreakpointOn() => RequestConsoleDatabase.instance.breakpoints.Contains(BreakpointName);

        private void SetBindingCallbacks()
        {
            var serializedObject = new SerializedObject(RequestConsoleDatabase.instance);
            var prop = serializedObject.FindProperty(RequestConsoleDatabase.BreakpointPropertyName);
            this.TrackPropertyValue(prop,
                _ =>
                {
                    SetValueWithoutNotify(IsBreakpointOn());
                });
            this.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                    RequestConsoleDatabase.instance.breakpoints.Add(BreakpointName);
                else
                    RequestConsoleDatabase.instance.breakpoints.Remove(BreakpointName);
            });
        }

        private new class UxmlFactory : UxmlFactory<BreakpointToggle> { }
    }
}