using TTGJ.Framework;
using System.Collections.Generic;
using UnityEngine;
using TTGJ.Config;
using TTGJ.Generate;

namespace TTGJ.Buff
{
    public class BuffManager : Singleton<BuffManager>
    {
        BuffConfigAsset buffConfigs;
        private Dictionary<GameObject, List<BuffBase>> buffs = new Dictionary<GameObject, List<BuffBase>>();
        public T AddBuff<T>(Transform target, bool isOverride = true) where T : BuffBase
        {
            if (!buffs.ContainsKey(target.gameObject))
            {
                buffs.Add(target.gameObject, new List<BuffBase>());
            }
            if (isOverride)
            {
                RemoveAllBuff(target);
            }
            T buffer = target.gameObject.AddComponent<T>();
            buffer.duration = ConfigManager.Instance.GetBuffDuration(buffer);
            buffer.checkInterval = ConfigManager.Instance.GetBuffCheckInterval(buffer);
            buffs[target.gameObject].Add(buffer);
            return buffer;
        }
        public void RemoveBuff(Transform target, BuffBase buff)
        {
            if (!buffs.ContainsKey(target.gameObject))
            {
                return;
            }
            buffs[target.gameObject].Remove(buff);
        }
        public void RemoveAllBuff(Transform target)
        {
            if (!buffs.ContainsKey(target.gameObject))
            {
                return;
            }
            for (int i = 0; i < buffs[target.gameObject].Count; i++)
            {
                if (buffs[target.gameObject][i] == null)
                {

                    continue;
                }
                buffs[target.gameObject][i].EndBuff();
            }
            buffs[target.gameObject].Clear();
        }
    }
}