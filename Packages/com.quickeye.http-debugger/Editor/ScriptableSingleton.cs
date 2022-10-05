using System.Linq;
using UnityEngine;

namespace QuickEye.RequestWatcher
{
    public abstract class SingletonScriptableObject<T> : SingletonScriptableObject
        where T : SingletonScriptableObject<T>
    {
        private static T _instance;
        public static T Instance => GetInstance();

        private static T GetInstance()
        {
            if (_instance == null)
                _instance = GetOrCreateInstance<T>();
            return _instance;
        }
    }

    public abstract class SingletonScriptableObject : ScriptableObject
    {
        protected static T GetOrCreateInstance<T>() where T : ScriptableObject
        {
            if (TryGetLoadedInstance<T>(out var obj))
                return obj;
            obj = CreateInstance<T>();
            obj.name = typeof(T).Name;
            return obj;
        }

        private static bool TryGetLoadedInstance<T>(out T instance) where T : ScriptableObject
        {
            instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
            return instance != null;
        }
    }
}