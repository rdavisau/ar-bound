using System.Collections.Generic;
using System.IO;
using System.Linq;
using AVFoundation;
using Foundation;

namespace ARKitMeetup.Helpers
{
    public static class SoundManager
    {
        private static Dictionary<string, AVAudioPlayer> _sounds;

        public static void Init()
        {
            _sounds = 
                Enumerable.Concat(
                    NSBundle.MainBundle.PathsForResources(".wav", "audio"),
                    NSBundle.MainBundle.PathsForResources(".mp3", "audio"))
                .ToDictionary(Path.GetFileNameWithoutExtension, f =>
                {
                    var av = AVAudioPlayer.FromUrl(NSUrl.FromString(f));
                    av.PrepareToPlay();

                    return av;
                });
        }
        
        public static void PlaySound(string sound, bool loop = false)
        {
            if (_sounds.TryGetValue(sound, out var av))
            {
                if (loop) // bgm
                {
                    av.NumberOfLoops = 10000;
                    av.Volume = .45f;
                }
                
                av.Play();
            }
        }
        
        public static void PauseSound(string sound)
        {
            if (_sounds.TryGetValue(sound, out var av))
                av.Pause();
        }        
        
        public static void ToggleSound(string sound)
        {
            if (_sounds.TryGetValue(sound, out var av))
            {
                av.Volume = System.Math.Abs(av.Volume) < 0.1 ? .45f : 0f;
            }
        }        
    }
}