using UnityEngine.UI;
using UnityEngine.UIElements;

namespace QuickEye.RequestWatcher
{
    public static class FluentVisualElementExtensions
    {
        public static T As<T>(this VisualElement ve) where T : VisualElement
        {
            return (T)ve;
        }

        
    }
}