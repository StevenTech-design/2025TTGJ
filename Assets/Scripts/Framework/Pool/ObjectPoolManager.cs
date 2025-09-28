using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TTGJ.Framework
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        private Dictionary<string, Queue<GameObject>> _objectPools = new Dictionary<string, Queue<GameObject>>();
        private GameObject _objectPoolRoot;

        public async UniTask<GameObject> GetGameObject(string path)
        {
            string fileName = Path.GetFileName(path);
            GameObject res = GetGameObjectByPool(fileName);
            if (res == null)
            {
                res = await StResources.Instance.Load<GameObject>(path);
            }
            return res;
        }

        private GameObject GetGameObjectByPool(string key)
        {
            if (!_objectPools.TryGetValue(key, out var queue))
            {
                return null;
            }
            if (queue.Count <= 0)
            {
                return null;
            }
            GameObject res = null;
            while (queue.Count > 0 && res == null)
            {
                res = queue.Dequeue();
            }
            return res;
        }
        public void ReturnGameObjectToPool(GameObject obj)
        {
            string key = obj.name.Replace("(Clone)", "");
            if (_objectPools.TryGetValue(key, out var queue))
            {
                queue.Enqueue(obj);
                return;
            }
            _objectPools[key] = new Queue<GameObject>();
            _objectPools[key].Enqueue(obj);
            ResetGameObject(obj);
        }

        private void ResetGameObject(GameObject obj)
        {
            if (_objectPoolRoot == null)
            {
                _objectPoolRoot = new GameObject("ObjectPoolRoot");
                _objectPoolRoot.transform.position = Vector3.up * 10000;
            }
            obj.SetActive(false);
            obj.transform.SetParent(_objectPoolRoot.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }
    }
}