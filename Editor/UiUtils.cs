using System;
using QuickEye.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    internal static class UiUtils
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


        public static void InitializeTabStatePersistence(VisualElement root, string viewDataKey, params Tab[] tabs)
        {
            // Because TabGroup and Tabs do not support viewDataKey right now
            // I create a textfield that will store last active tab name

            var lastActiveTabField = new TextField
            {
                name = "last-active-tab-data-holder",
                viewDataKey = viewDataKey,
                style = { display = DisplayStyle.None }
            };

            root.Add(lastActiveTabField);
            foreach (var tab in tabs)
            {
                tab.RegisterValueChangedCallback(e => { lastActiveTabField.value = tab.name; });
            }

            lastActiveTabField.RegisterValueChangedCallback(e =>
            {
                foreach (var tab in tabs)
                {
                    if (e.newValue == tab.name && !tab.value)
                        tab.value = true;
                }
            });
        }
    }
}