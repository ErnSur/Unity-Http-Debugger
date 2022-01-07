using System;
using UnityEditor;
using static UnityEditor.EditorWindow;

namespace QuickEye.RequestWatcher
{
    
    internal static class EditorFullscreenUtility
    {
        private static Type prevWindowType;

        public static EditorWindow ToggleEditorFullscreen<T>() where T : EditorWindow
        {
            return ToggleEditorFullscreen(typeof(T));
        }

        public static EditorWindow ToggleEditorFullscreen(Type editorType)
        {
            if (focusedWindow != null && focusedWindow.GetType() != editorType)
            {
                prevWindowType = focusedWindow.GetType();
                focusedWindow.maximized = false;
            }
            var wnd = GetWindow(editorType);
            if (wnd == null)
                return null;
            if (wnd.maximized)
            {
                wnd.maximized = false;
                if (prevWindowType != null)
                {
                    GetWindow(prevWindowType);
                }
            }
            else
                wnd.maximized = true;

            return wnd;
        }
    }
}