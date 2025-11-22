using System;
using System.Collections;
using System.Collections.Generic;
using Steven.Framework;
using UnityEngine;

namespace TTGJ.Env
{
    public class Weather : MonoSingleton<Weather>
    {

        [Header("风力设置")]
        [SerializeField]
        private Vector3 windDirection = Vector3.right; 
        [SerializeField]
        private float windStrength = 2f;
        [Header("光照设置")]
        private int maxLightForce = 3;

        public Vector3 GetWindAtPosition(Vector3 position)
        {
            return windDirection.normalized * windStrength;
        }

        public int GetCurrentLightForce() {
            float hour = TimeSystem.Instance.GetCurrentGameHour();
            if(hour < 6 || hour > 18) {
                return 0;
            }
            float t = Math.Abs(12-hour)/ 6.0f;
            float intensity = Mathf.Lerp(0, maxLightForce, t);
            return Mathf.RoundToInt(intensity);;
        }
        private void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 24;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperLeft;
            int currentIntensity = GetCurrentLightForce();

            string lightText = $"Light: +{currentIntensity}";
            GUI.Label(new Rect(10, 70, 300, 30), lightText, style);
        }

        public float GetCurrentTemperature() {
            return 36.7f;
        }
    }

}