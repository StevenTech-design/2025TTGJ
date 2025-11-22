using System;
using UnityEngine;
using Steven.Framework;

namespace TTGJ.Env
{
    public class TimeSystem : MonoSingleton<TimeSystem>
    {
        private const string TOTAL_HOURS_KEY = "GameTime_TotalHours";
        private const string LAST_REAL_TIME_KEY = "GameTime_LastRealTime";
        private const string REAL_SECONDS_PER_HOUR_KEY = "GameTime_RealSecondsPerHour";
        
        // 现实秒数对应游戏1小时（可调整，默认20秒）
        [SerializeField] private float realSecondsPerGameHour = 20f;
        
        private float totalGameHours = 0f;
        private double lastUpdateRealTime = 0;
        private bool isPaused = false;

        public float RealSecondsPerGameHour
        {
            get => realSecondsPerGameHour;
            set
            {
                realSecondsPerGameHour = value;
                PlayerPrefs.SetFloat(REAL_SECONDS_PER_HOUR_KEY, value);
                PlayerPrefs.Save();
            }
        }

        private void Awake()
        {
            LoadTimeData();
        }

        private void Start()
        {
            lastUpdateRealTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private void Update()
        {
            if (isPaused)
                return;

            // 计算自上次更新以来经过的现实时间（秒）
            double currentRealTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            double deltaRealSeconds = currentRealTime - lastUpdateRealTime;
            
            if (deltaRealSeconds > 0)
            {
                // 计算对应的游戏时间（小时）
                float deltaGameHours = (float)(deltaRealSeconds / realSecondsPerGameHour);
                
                // 累加游戏时间
                totalGameHours += deltaGameHours;
                
                // 更新上次更新时间
                lastUpdateRealTime = currentRealTime;
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            isPaused = pauseStatus;
            if (pauseStatus)
            {
                SaveTimeData();
            }
            else
            {
                lastUpdateRealTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                SaveTimeData();
            }
            else
            {
                lastUpdateRealTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        private void OnDestroy()
        {
            SaveTimeData();
        }

        /// <summary>
        /// 加载时间数据
        /// </summary>
        private void LoadTimeData()
        {
            // 加载累计游戏时间
            totalGameHours = PlayerPrefs.GetFloat(TOTAL_HOURS_KEY, 0f);
            
            // 加载时间比例
            if (PlayerPrefs.HasKey(REAL_SECONDS_PER_HOUR_KEY))
            {
                realSecondsPerGameHour = PlayerPrefs.GetFloat(REAL_SECONDS_PER_HOUR_KEY);
            }
            else
            {
                PlayerPrefs.SetFloat(REAL_SECONDS_PER_HOUR_KEY, realSecondsPerGameHour);
            }
            
            // 加载上次保存的现实时间
            string lastTimeStr = PlayerPrefs.GetString(LAST_REAL_TIME_KEY, "0");
            if (double.TryParse(lastTimeStr, out double lastTime) && lastTime > 0)
            {
                lastUpdateRealTime = lastTime;
                // 计算从上次保存到现在经过的时间
                double currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                double elapsedSeconds = currentTime - lastTime;
                if (elapsedSeconds > 0)
                {
                    // 累加这段时间对应的游戏时间
                    totalGameHours += (float)(elapsedSeconds / realSecondsPerGameHour);
                }
            }
            else
            {
                lastUpdateRealTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 保存时间数据
        /// </summary>
        private void SaveTimeData()
        {
            PlayerPrefs.SetFloat(TOTAL_HOURS_KEY, totalGameHours);
            PlayerPrefs.SetFloat(REAL_SECONDS_PER_HOUR_KEY, realSecondsPerGameHour);
            PlayerPrefs.SetString(LAST_REAL_TIME_KEY, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 获取当前游戏时间（小时，0-24）
        /// </summary>
        public float GetCurrentGameHour()
        {
            return totalGameHours % 24f;
        }

        /// <summary>
        /// 获取当前天数（从游戏开始计算）
        /// </summary>
        public int GetCurrentDay()
        {
            return Mathf.FloorToInt(totalGameHours / 24f) + 1;
        }

        /// <summary>
        /// 获取格式化的时间字符串（am/pm 00:00）
        /// </summary>
        public string GetFormattedTime()
        {
            float hour = GetCurrentGameHour();
            int hours = Mathf.FloorToInt(hour);
            int minutes = Mathf.FloorToInt((hour - hours) * 60f);
            
            string period = hours >= 12 ? "pm" : "am";
            int displayHour = hours % 12;
            if (displayHour == 0) displayHour = 12;
            
            return $"{period} {displayHour:D2}:{minutes:D2}";
        }

        /// <summary>
        /// 获取完整的时间显示字符串（包含天数）
        /// </summary>
        public string GetFullTimeString()
        {
            return $"{GetFormattedTime()} - Day {GetCurrentDay()}";
        }

        /// <summary>
        /// 暂停/恢复时间
        /// </summary>
        public void SetPaused(bool paused)
        {
            isPaused = paused;
            if (!paused)
            {
                lastUpdateRealTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        /// <summary>
        /// OnGUI显示时间
        /// </summary>
        private void OnGUI()
        {
            // 设置GUI样式
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 24;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperLeft;
            
            // 显示时间（左上角）
            string timeText = GetFormattedTime();
            string dayText = $"Day {GetCurrentDay()}";
            
            GUI.Label(new Rect(10, 10, 300, 30), timeText, style);
            GUI.Label(new Rect(10, 40, 300, 30), dayText, style);
        }
    }
}