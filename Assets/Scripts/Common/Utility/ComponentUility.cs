using UnityEngine;

namespace TTGJ.Common
{
    public static class ComponentUtility
    {
        public static T TryAddComponent<T>(this GameObject gameObject) where T : Component
        { 
            if (gameObject == null)
                return null;
                
            if (gameObject.TryGetComponent<T>(out var existingComponent))
                return existingComponent;
            return gameObject.AddComponent<T>();
        }
        public static T TryAddComponent<T>(this Transform transform) where T : Component
        {
            if (transform == null)
                return null;
                
            return TryAddComponent<T>(transform.gameObject);
        }
    } 
}