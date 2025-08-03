using SoundSystem.Enums;

namespace SoundSystem.SoundManager
{
    public interface ISoundManager
    {
        void PlaySfx(SoundClipName clipName);
        void PlayMusic(SoundClipName clipName, bool loop = true);
        void StopMusic();
        void SetSfxVolume(float volume);
        void SetMusicVolume(float volume);
        void MuteAll(bool isMuted);
    }

}