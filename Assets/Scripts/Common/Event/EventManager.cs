using System;
using System.Collections.Generic;
using TTGJ.Framework;
using UnityEngine;

namespace TTGJ.Common
{
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<EventType, List<Action<EventParam>>> eventHandlers;

        private void RegisterEvent(EventType eventType, Action<EventParam> handler)
        {
            if (!eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType] = new List<Action<EventParam>>();
            }
            eventHandlers[eventType].Add(handler);
        }

        private void UnregisterEvent(EventType eventType, Action<EventParam> handler)
        {
            if (eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType].Remove(handler);
            }
        }

        private void TriggerEvent(EventType eventType, EventParam eventParam)
        {
            try
            {
                eventHandlers[eventType]?.ForEach(handler => handler(eventParam));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}