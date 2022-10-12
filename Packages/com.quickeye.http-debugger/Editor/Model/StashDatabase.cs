using UnityEditor;

namespace QuickEye.RequestWatcher
{
    [FilePath("Library/RequestStash.json",FilePathAttribute.Location.ProjectFolder)]
    internal class StashDatabase : RequestDatabase<StashDatabase>
    {
    }
}