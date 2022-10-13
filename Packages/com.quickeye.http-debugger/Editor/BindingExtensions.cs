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
            if (sp == null)
                return;
            var bindingTarget = new Label();
            bindingTarget.name = "rw-property-tracker";
            bindingTarget.RegisterValueChangedCallback(OnValueChange);
            ve.RegisterCallback<AttachToPanelEvent>(InitValue);

            void OnValueChange(ChangeEvent<string> evt)
            {
                if (evt.target != bindingTarget)
                    return;
                callback?.Invoke(sp);
            }

            void InitValue(AttachToPanelEvent e)
            {
                callback?.Invoke(sp);
                ve.UnregisterCallback<AttachToPanelEvent>(InitValue);
            }

            bindingTarget.ToggleDisplayStyle(false);
            ve.Add(bindingTarget);
            bindingTarget.bindingPath = sp.propertyPath;
            bindingTarget.Bind(sp.serializedObject);
        }
    }
}