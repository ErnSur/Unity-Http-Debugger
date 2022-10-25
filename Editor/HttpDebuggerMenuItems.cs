using UnityEditor;

namespace QuickEye.WebTools.Editor
{
    internal static class HttpDebuggerMenuItems
    {
        private const string MenuPath = "Http Debugger/";//"Window/Http Debugger"
        
        [MenuItem(MenuPath+"Console")]
        public static void OpenConsole() => RequestConsoleWindow.Open();
        [MenuItem(MenuPath+"Console Database")]
        public static void SelectConsoleDatabase() => Selection.activeObject = RequestConsoleDatabase.instance;
        [MenuItem(MenuPath+"Stash")]
        public static void OpenStash() => RequestStashWindow.Open();
        [MenuItem(MenuPath+"Stash Database")]
        public static void SelectStashDatabase() => Selection.activeObject = StashDatabase.instance;
        
    }
}