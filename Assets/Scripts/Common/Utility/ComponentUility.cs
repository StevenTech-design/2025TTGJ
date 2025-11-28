using System;
using UnityEngine;

namespace TTGJ.Common
{
    public static class ComponentUtility
    {
        private static readonly Collider[] _colliderResults = new Collider[100];
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
        public static int FindNearInteractableObject<T>(this GameObject gameObject, Vector3 position, Vector3 checkRange, in Collider[] results) where T : Component
        {
            if (gameObject == null)
            {
                return 0;
            }
            Array.Clear(results, 0, results.Length);
            return Physics.OverlapBoxNonAlloc(position, checkRange / 2, results);
        }
        public static T FindNearestInteractableObject<T>(this GameObject gameObject, Vector3 position, Vector3 checkRange, in Collider[] results) where T : Component
        {
            if (gameObject == null)
            {
                return null;
            }
            int count = FindNearInteractableObject<T>(gameObject, position, checkRange, _colliderResults);
            if (count <= 0)
            { 
                return null;
            }
            T nearestObject = _colliderResults[0].GetComponent<T>();
            float minDistance = Vector3.Distance(position, _colliderResults[0].transform.position);
            for (int i = 1; i < count; i++)
            {
                float distance = Vector3.Distance(position, _colliderResults[i].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestObject = _colliderResults[i].GetComponent<T>();
                }
            }
            return nearestObject;
        }
    } 
}