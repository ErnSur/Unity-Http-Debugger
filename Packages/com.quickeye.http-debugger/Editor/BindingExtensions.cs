using System;
using QuickEye.RequestWatcher.Aid;
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
        
        public static void TrackPropertyChange<T>(this VisualElement ve, string bp,
            Action<T> callback)
        {
            var bindingTarget = new ValueTracker<T>();
            bindingTarget.SetUp(bp,OnValueChange);
            ve.RegisterCallback<AttachToPanelEvent>(InitValue);

            void OnValueChange(ChangeEvent<T> evt)
            {
                if (evt.target != bindingTarget)
                    return;
                callback?.Invoke(evt.newValue);
            }
            void InitValue(AttachToPanelEvent e)
            {
                callback?.Invoke(bindingTarget.value);
                ve.UnregisterCallback<AttachToPanelEvent>(InitValue);
            }
            //bindingTarget.ToggleDisplayStyle(false);
            ve.Add(bindingTarget);
            bindingTarget.bindingPath = bp;
        }
    }
}