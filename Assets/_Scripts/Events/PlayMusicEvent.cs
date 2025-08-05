using _Scripts.Enums;

namespace _Scripts.Events
{
    public struct PlayMusicEvent
    {
        public SoundClipName ClipName;
        public bool Loop;
    }
}