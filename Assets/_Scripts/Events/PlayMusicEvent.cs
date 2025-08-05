using _Scripts.Services.SoundSystem.Enums;

namespace _Scripts.Events
{
    public struct PlayMusicEvent
    {
        public SoundClipName ClipName;
        public bool Loop;
    }
}