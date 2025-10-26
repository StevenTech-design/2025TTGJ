using TTGJ.Framework;
using UnityEngine;

namespace TTGJ.Data {
    public class DataManager : Singleton<DataManager> {
        public bool GetShowTipUIState() { 
            return PlayerPrefs.GetInt("ShowTipUIState", 0) == 1;
        }
        public void SetShowTipUIState(bool state) { 
            int value = state ? 1 : 0;
            PlayerPrefs.SetInt("ShowTipUIState", value);
        }
        public float GetMusicVolume() { 
            return PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        }
        public void SetMusicVolume(float volume) { 
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
        public int GetFieldOfView() { 
            return PlayerPrefs.GetInt("FieldOfView", 60);
        }
        public void SetFieldOfView(int fieldOfView) { 
            PlayerPrefs.SetInt("FieldOfView", fieldOfView);
        }
    }
}