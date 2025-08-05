using System;
using System.Collections.Generic;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
using _Scripts.Services.SoundSystem.Config;
using _Scripts.Services.SoundSystem.Enums;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace _Scripts.Services.SoundSystem.SoundManager
{
    public class SoundManager : ISoundManager, IInitializable, IDisposable
    {
        private readonly AudioSource _sfxSource;
        private readonly AudioSource _musicSource;
        private readonly Dictionary<SoundClipName, AudioClip> _clipMap;
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables = new();

        private float _sfxVolume = 1f;
        private float _musicVolume = 1f;
        private bool _isMuted = false;

        public SoundManager(SoundConfig config, IEventBus eventBus)
        {
            _eventBus = eventBus;

            var go = new GameObject("[SoundManager]");
            Object.DontDestroyOnLoad(go);

            _sfxSource = go.AddComponent<AudioSource>();
            _musicSource = go.AddComponent<AudioSource>();

            _clipMap = config.BuildLookup();
            Initialize();
        }

        public void Initialize()
        {
            _eventBus.OnEvent<PlaySfxEvent>()
                .Subscribe(e => PlaySfx(e.ClipName))
                .AddTo(_disposables);

            _eventBus.OnEvent<PlayMusicEvent>()
                .Subscribe(e => PlayMusic(e.ClipName, e.Loop))
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void PlaySfx(SoundClipName clipName)
        {
            if (_isMuted || !_clipMap.TryGetValue(clipName, out var clip)) return;
            _sfxSource.PlayOneShot(clip, _sfxVolume);
        }

        public void PlayMusic(SoundClipName clipName, bool loop = true)
        {
            if (_isMuted || !_clipMap.TryGetValue(clipName, out var clip)) return;
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.volume = _musicVolume;
            _musicSource.Play();
        }

        public void StopMusic() => _musicSource.Stop();

        public void SetSfxVolume(float volume) => _sfxVolume = Mathf.Clamp01(volume);

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            _musicSource.volume = _musicVolume;
        }

        public void MuteAll(bool isMuted)
        {
            _isMuted = isMuted;
            _sfxSource.mute = isMuted;
            _musicSource.mute = isMuted;
        }
    }
}