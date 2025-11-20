using System;
using System.Collections.Generic;
using Steven.Framework;
using UnityEngine;

namespace TTGJ.Common
{
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<EventType, List<Action<EventParam>>> eventHandlers=new();

        public void RegisterEvent(EventType eventType, Action<EventParam> handler)
        {
            if (!eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType] = new List<Action<EventParam>>();
            }
            eventHandlers[eventType].Add(handler);
        }

        public void UnregisterEvent(EventType eventType, Action<EventParam> handler)
        {
            if (eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType].Remove(handler);
            }
        }

        public void TriggerEvent(EventType eventType, EventParam eventParam)
        {
            if (!eventHandlers.ContainsKey(eventType))
            {
                return;
            }
            foreach (var handler in eventHandlers[eventType])
            {
                handler(eventParam);
            }
        }
    }
}