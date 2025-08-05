using System.Collections.Generic;
using _Scripts.Enums;
using UnityEngine;

namespace _Scripts.Services.SoundSystem.Config
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Game/Config/SoundConfig", order = 0)]
    public class SoundConfig : ScriptableObject
    {
        [System.Serializable]
        public class AudioEntry
        {
            public SoundClipName name;
            public AudioClip clip;
        }

        public List<AudioEntry> SfxClips;
        public List<AudioEntry> MusicClips;

        public Dictionary<SoundClipName, AudioClip> BuildLookup()
        {
            var dict = new Dictionary<SoundClipName, AudioClip>();
            foreach (var sfx in SfxClips)
                dict[sfx.name] = sfx.clip;
            foreach (var music in MusicClips)
                dict[music.name] = music.clip;
            return dict;
        }
    }
}