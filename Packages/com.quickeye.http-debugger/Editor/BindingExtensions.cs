using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    internal static class BindingExtensions
    {
        public static void TrackPropertyValueAndInit(this VisualElement ve, SerializedProperty sp,
            Action<SerializedProperty> callback)
        {
            if (sp == null)
                return;
            ve.TrackPropertyValue(sp, callback);
            ve.RegisterCallback<AttachToPanelEvent>(InitValue);

            void InitValue(AttachToPanelEvent e)
            {
                callback?.Invoke(sp);
                ve.UnregisterCallback<AttachToPanelEvent>(InitValue);
            }
        }
    }
}