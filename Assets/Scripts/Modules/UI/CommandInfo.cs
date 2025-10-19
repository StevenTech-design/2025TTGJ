using System.Collections.Generic;
using UnityEngine;

namespace TTGJ.UI
{
	public class CommandInfo : MonoBehaviour
	{
		[SerializeField] private Transform content;       // 条目父节点
		[SerializeField] private GameObject itemPrefab;   // 直接拖入预制体
		[SerializeField] private int prewarmCount = 8;

		private readonly List<GameObject> _active = new List<GameObject>();
		private readonly Stack<GameObject> _pool = new Stack<GameObject>();

		private void Awake()
		{
			// 预热
			if (itemPrefab == null) return;
			for (int i = 0; i < prewarmCount; i++)
			{
				var go = Instantiate(itemPrefab);
				go.name = itemPrefab.name;          // 统一命名，避免 (Clone)
				go.SetActive(false);
				go.transform.SetParent(transform, false);
				_pool.Push(go);
			}
		}

		public void ShowCommandInfo(List<(KeyCode, string)> commandInfoTargets)
		{
			Clear();
			if (itemPrefab == null || commandInfoTargets == null) return;
			int count = commandInfoTargets.Count;
			for (int i = 0; i < count; i++)
			{
				var go = GetItem();
				go.transform.SetParent(content, false);
				go.SetActive(true);

				if (!go.TryGetComponent<CommandInfoItem>(out var item))
					item = go.AddComponent<CommandInfoItem>();
				item.Set(commandInfoTargets[i].Item1, commandInfoTargets[i].Item2);

				_active.Add(go);
			}
		}

		public void Clear()
		{
			for (int i = 0; i < _active.Count; i++)
			{
				ReturnItem(_active[i]);
			}
			_active.Clear();
		}

		private GameObject GetItem()
		{
			if (_pool.Count > 0)
				return _pool.Pop();

			var go = Instantiate(itemPrefab);
			go.name = itemPrefab.name;
			go.SetActive(false);
			return go;
		}

		private void ReturnItem(GameObject go)
		{
			if (go == null) return;
			go.SetActive(false);
			go.transform.SetParent(transform, false);
			_pool.Push(go);
		}

		private void OnDisable()
		{
			Clear();
		}
	}
}