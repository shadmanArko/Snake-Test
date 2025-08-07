/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using _Scripts.Services.Utils;
using UnityEngine;
using CodeMonkey.Utils;

public class SoundManagers {

    public enum Sound {
        SnakeMove,
        SnakeDie,
        SnakeEat,
        ButtonClick,
        ButtonOver,
    }

    public static void PlaySound(Sound sound) {
        GameObject soundGameObject = new GameObject("SoundSystem");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(sound));
    }

    private static AudioClip GetAudioClip(Sound sound) {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.i.soundAudioClipArray) {
            if (soundAudioClip.sound == sound) {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("SoundSystem " + sound + " not found!");
        return null;
    }

    public static void AddButtonSounds( Button_UI buttonUI) {
        buttonUI.MouseOverOnceFunc += () => SoundManagers.PlaySound(Sound.ButtonOver);
        buttonUI.ClickFunc += () => SoundManagers.PlaySound(Sound.ButtonClick);
    }

}
