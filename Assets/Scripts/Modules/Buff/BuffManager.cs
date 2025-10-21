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
            buffer.duration = GetBuffDuration(buffer);
            buffer.checkInterval = GetBuffCheckInterval(buffer);
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
        private float GetBuffDuration(BuffBase buff)
        {
            if (buffConfigs == null)
            { 
                InitBuffConfigs();
            }
            if (!buffConfigs.TryGet(buff.GetBuffType(), out float duration))
            {
                return 30f;
            }
            return duration;
        }
        private float GetBuffCheckInterval(BuffBase buff)
        {
            if (buffConfigs == null)
            {
                InitBuffConfigs();
            }
            if (!buffConfigs.TryGet(buff.GetBuffType(), out float checkInterval))
            {
                return 0.1f;
            }
            return checkInterval;
        }
        private void InitBuffConfigs()
        {
            buffConfigs = StResources.Instance.LoadByResources<BuffConfigAsset>(ResPathConfig.Config_BuffConfigAsset);
            buffConfigs.RebuildCache();
        }
    }
}