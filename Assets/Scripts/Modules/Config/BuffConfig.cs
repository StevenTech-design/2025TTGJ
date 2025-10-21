using System;
using TTGJ.Buff;

namespace TTGJ.Config
{
    [Serializable]
    public class BuffConfig
    {
        public BuffType buffType;
        public float duration = 30f;
        public float checkInterval = 0.1f;
    }
}