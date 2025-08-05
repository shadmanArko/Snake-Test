using SoundSystem.Enums;

namespace SoundSystem.Events
{
    public struct PlayMusicEvent
    {
        public SoundClipName ClipName;
        public bool Loop;
    }
}