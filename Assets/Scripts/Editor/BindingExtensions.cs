using System;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    internal static class BindingExtensions
    {
        public static void TrackPropertyChange(this VisualElement ve, SerializedProperty sp,
            Action<SerializedProperty> callback)
        {
            var bindingTarget = new Label();
            bindingTarget.RegisterValueChangedCallback(evt =>
            {
                if (evt.target != bindingTarget)
                    return;
                callback?.Invoke(sp);
            });
            ve.RegisterCallback<AttachToPanelEvent>(InitValue);

            void InitValue(AttachToPanelEvent e)
            {
                callback?.Invoke(sp);
                ve.UnregisterCallback<AttachToPanelEvent>(InitValue);
            }
            bindingTarget.ToggleDisplayStyle(false);
            ve.Add(bindingTarget);
            bindingTarget.BindProperty(sp);
        }
    }
}