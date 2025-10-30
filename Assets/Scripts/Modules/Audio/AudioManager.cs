using UnityEngine;
using TTGJ.Framework;
using UnityEngine.Audio;
using TTGJ.Generate;
using TTGJ.Luban;

namespace TTGJ.Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {

        [SerializeField]
        private AudioSource bgmSource;
        [SerializeField]
        private AudioSource sfxSource;


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


        public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (clip == null) return;
            sfxSource.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        public void PlaySFX(AudioType audioType, float volume = 1f, float pitch = 1f)
        { 
            var audio = LubanManager.Instance.GetAudio((int)audioType);
            string path = audio.AssetPath[UnityEngine.Random.Range(0, audio.AssetPath.Count)];
            var clip = StResources.Instance.LoadByResources<AudioClip>(path);
            if(clip != null) {
                PlaySFX(clip, volume, pitch);
            }
        }

    }
}