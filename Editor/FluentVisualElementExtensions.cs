using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickEye.WebTools.Editor
{
    public static class FluentVisualElementExtensions
    {
        public static T As<T>(this VisualElement ve) where T : VisualElement
        {
            return (T)ve;
        }

        public static IEnumerable<StyleSheet> GetStyleSheets(this VisualElement ve)
        {
            for (int i = 0; i < ve.styleSheets.count; i++)
            {
                yield return ve.styleSheets[i];
            }
        }
        
        internal static void LogStyleSheets(this VisualElement ve)
        {
            var text = string.Join(", ", ve.GetStyleSheets().Select(s => s.name));
            Debug.Log($"[{ve.GetType().Name}]{ve.name} Style Sheets: {text}");
        }
    }
}