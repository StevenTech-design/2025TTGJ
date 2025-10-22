using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TTGJ.Framework;

namespace TTGJ.Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [Header("Mixer (optional)")]
        [SerializeField] private AudioMixerGroup bgmMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;

        [Header("SFX Pool Settings")]
        [SerializeField] private int sfxPoolSize = 16;
        [SerializeField] private bool sfxAutoExpand = true;
        [SerializeField] private float sfxSpatialBlend3D = 1f;

        private readonly Dictionary<string, AudioSource> keyToBgmSource = new Dictionary<string, AudioSource>();
        private readonly List<AudioSource> sfxSources = new List<AudioSource>();
        private Transform bgmRoot;
        private Transform sfxRoot;
        private int sfxNextIndex = 0;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            EnsureRoots();
            EnsureSfxPool();
        }

        // ============== BGM ==============

        /// <summary>
        /// 播放一个BGM通道；可并行播放多个通道。返回该通道的key。
        /// </summary>
        public string PlayBGM(AudioClip clip, string key = null, bool loop = true, float volume = 1f)
        {
            if (clip == null)
                return null;

            EnsureRoots();

            if (string.IsNullOrEmpty(key))
            {
                key = $"bgm_{keyToBgmSource.Count + 1}";
            }

            AudioSource source;
            if (!keyToBgmSource.TryGetValue(key, out source) || source == null)
            {
                source = CreateBgmSource(key);
                keyToBgmSource[key] = source;
            }

            source.clip = clip;
            source.loop = loop;
            source.volume = Mathf.Clamp01(volume);
            source.Play();
            return key;
        }

        /// <summary>
        /// 停止并销毁指定key的BGM通道。
        /// </summary>
        public void StopBGM(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            AudioSource source;
            if (keyToBgmSource.TryGetValue(key, out source) && source != null)
            {
                source.Stop();
                Destroy(source.gameObject);
            }
            keyToBgmSource.Remove(key);
        }

        /// <summary>
        /// 停止全部BGM通道。
        /// </summary>
        public void StopAllBGM()
        {
            foreach (var kv in keyToBgmSource)
            {
                if (kv.Value != null)
                {
                    kv.Value.Stop();
                    Destroy(kv.Value.gameObject);
                }
            }
            keyToBgmSource.Clear();
        }

        /// <summary>
        /// 设置某个BGM通道音量。
        /// </summary>
        public void SetBGMVolume(string key, float volume)
        {
            AudioSource source;
            if (keyToBgmSource.TryGetValue(key, out source) && source != null)
            {
                source.volume = Mathf.Clamp01(volume);
            }
        }

        private AudioSource CreateBgmSource(string key)
        {
            EnsureRoots();
            var go = new GameObject($"BGM_{key}");
            go.transform.SetParent(bgmRoot, false);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = true;
            src.spatialBlend = 0f; // 2D音乐
            if (bgmMixerGroup != null) src.outputAudioMixerGroup = bgmMixerGroup;
            return src;
        }

        // ============== SFX ==============

        /// <summary>
        /// 播放一个2D音效（UI/非空间化）。
        /// </summary>
        public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (clip == null)
                return;

            EnsureSfxPool();
            var src = GetNextSfxSource();
            src.transform.localPosition = Vector3.zero;
            src.spatialBlend = 0f;
            src.pitch = Mathf.Clamp(pitch, -3f, 3f);
            src.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        /// <summary>
        /// 在世界坐标播放3D音效（碰撞等）。
        /// </summary>
        public void PlaySFXAt(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (clip == null)
                return;

            EnsureSfxPool();
            var src = GetNextSfxSource();
            src.transform.position = position;
            src.spatialBlend = Mathf.Clamp01(sfxSpatialBlend3D);
            src.pitch = Mathf.Clamp(pitch, -3f, 3f);
            src.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        /// <summary>
        /// 向后兼容旧接口。
        /// </summary>
        public void PlaySound(AudioClip clip)
        {
            PlaySFX(clip);
        }

        private void EnsureRoots()
        {
            if (bgmRoot == null)
            {
                var bgmObj = new GameObject("BGM");
                bgmObj.transform.SetParent(transform, false);
                bgmRoot = bgmObj.transform;
            }

            if (sfxRoot == null)
            {
                var sfxObj = new GameObject("SFX");
                sfxObj.transform.SetParent(transform, false);
                sfxRoot = sfxObj.transform;
            }
        }

        private void EnsureSfxPool()
        {
            EnsureRoots();
            if (sfxSources.Count >= sfxPoolSize)
                return;

            int need = sfxPoolSize - sfxSources.Count;
            for (int i = 0; i < need; i++)
            {
                sfxSources.Add(CreateSfxSource());
            }
        }

        private AudioSource CreateSfxSource()
        {
            var go = new GameObject("SFX_Source");
            go.transform.SetParent(sfxRoot, false);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f;
            src.rolloffMode = AudioRolloffMode.Linear;
            src.minDistance = 1f;
            src.maxDistance = 50f;
            if (sfxMixerGroup != null) src.outputAudioMixerGroup = sfxMixerGroup;
            return src;
        }

        private AudioSource GetNextSfxSource()
        {
            if (sfxSources.Count == 0)
            {
                if (!sfxAutoExpand)
                {
                    // 创建一个临时源
                    return CreateSfxSource();
                }
                sfxSources.Add(CreateSfxSource());
            }

            var src = sfxSources[sfxNextIndex];
            sfxNextIndex = (sfxNextIndex + 1) % sfxSources.Count;
            return src;
        }
    }
}