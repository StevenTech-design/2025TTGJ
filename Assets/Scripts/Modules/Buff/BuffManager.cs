using System;
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
        public BuffBase AddBuff(BuffType buffType, Transform target, bool isOverride = true) {
            switch (buffType)
            {
                case BuffType.BigHead:
                    return AddBuff<BigHeadBuff>(target, isOverride);
                case BuffType.Reveal:
                    return AddBuff<RevealBuff>(target, isOverride);
                case BuffType.FastMove:
                    return AddBuff<FastMoveBuff>(target, isOverride);
                case BuffType.SmallModel:
                    return AddBuff<SmallModelBuff>(target, isOverride);
                case BuffType.BigModel:
                    return AddBuff<BigModelBuff>(target, isOverride);
                case BuffType.ReverseDir:
                    return AddBuff<ReverseDirBuff>(target, isOverride);
                case BuffType.ForwardWateringBuff:
                    return AddBuff<ForwardWateringBuff>(target, isOverride);
                case BuffType.SheepTalk:
                    return AddBuff<SheepTalkBuff>(target, isOverride);
                case BuffType.ReplaceHead:
                    return AddBuff<ReplaceHeadBuff>(target, isOverride);
                case BuffType.ReplaceModel:
                    return AddBuff<ReplaceModelBuff>(target, isOverride);
                case BuffType.HeadFlash:
                    return AddBuff<HeadFlashBuff>(target, isOverride);
                case BuffType.Dyeing:
                    return AddBuff<DyeingBuff>(target, isOverride);
            }
            return null;
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