using UnityEngine;
using TTGJ.Framework;
using UnityEngine.Audio;

namespace TTGJ.Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioMixerGroup audioMixerGroup;

        private AudioSource bgmSource;
        private AudioSource sfxSource;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            // 创建BGM音源
            GameObject bgmObj = new GameObject("BGM");
            bgmObj.transform.SetParent(transform, false);
            bgmSource = bgmObj.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
            bgmSource.outputAudioMixerGroup = audioMixerGroup;

            // 创建SFX音源
            GameObject sfxObj = new GameObject("SFX");
            sfxObj.transform.SetParent(transform, false);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.outputAudioMixerGroup = audioMixerGroup;
        }

        // ============== BGM ==============

        public void PlayBGM(AudioClip clip, bool loop = true, float volume = 1f)
        {
            if (clip == null) return;
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.volume = Mathf.Clamp01(volume);
            bgmSource.Play();
        }

        public void StopBGM()
        {
            if (bgmSource != null && bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }
        }

        public void SetBGMVolume(float volume)
        {
            if (bgmSource != null)
            {
                bgmSource.volume = Mathf.Clamp01(volume);
            }
        }

        // ============== SFX ==============

        public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (clip == null) return;
            sfxSource.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        /// <summary>
        /// 播放3D空间化音效（碰撞等）
        /// </summary>
        public void PlaySFXAt(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (clip == null) return;
            
            // 创建一个临时的3D音源
            GameObject tempSfx = new GameObject("TempSFX");
            tempSfx.transform.position = position;
            var tempSrc = tempSfx.AddComponent<AudioSource>();
            tempSrc.playOnAwake = false;
            tempSrc.spatialBlend = 1f; // 3D空间化
            tempSrc.outputAudioMixerGroup = audioMixerGroup;
            tempSrc.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
            tempSrc.PlayOneShot(clip, Mathf.Clamp01(volume));
            
            // 播放完后销毁
            Destroy(tempSfx, clip.length + 1f);
        }

        /// <summary>
        /// 向后兼容
        /// </summary>
        public void PlaySound(AudioClip clip)
        {
            PlaySFX(clip);
        }
    }
}