using UnityEngine;
using TTGJ.Framework;
using UnityEngine.Audio;
using TTGJ.Generate;
using TTGJ.Luban;
using System;

namespace TTGJ.Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {

        private AudioSource bgmSource;
        private AudioSource sfxSource;

        private void Awake()
        {
            // BGM
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
            bgmSource.spatialBlend = 0f; // 2D 声音

            // SFX
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f; // 2D 声音
        }


        public void PlayBGM(AudioClip clip, bool loop = true, float volume = 1f)
        {
            if (clip == null) return;
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.volume = Mathf.Clamp01(volume);
            bgmSource.Play();
        }
        public void PlayBGM(string path, bool loop = true, float volume = 1f)
        { 
            var clip = StResources.Instance.LoadByResources<AudioClip>(path);
            if(clip != null) {
                PlayBGM(clip);
            }
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
        public void SetSFXVolume(float volume)
        {
            if (sfxSource != null)
            {
                sfxSource.volume = Mathf.Clamp01(volume);
            }
        }
        public void SetBVolume(float volume)
        {
            SetBGMVolume(volume);
            SetSFXVolume(volume);
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