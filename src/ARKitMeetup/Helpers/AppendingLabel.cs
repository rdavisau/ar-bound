using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace ARKitMeetup.Helpers
{
    public class AppendingLabel : UILabel
    {
        private Dictionary<char, int> _delayChars = new Dictionary<char, int>
        {
            [','] = 3, [':'] = 4, ['.'] = 4, ['!'] = 2, ['?'] = 4, [';'] = 3
        };

        public AppendingLabel(float size) 
        {
            Font = UIFont.FromName("Apple-Kid", size);
            TranslatesAutoresizingMaskIntoConstraints = false;
            Lines = 0; 
        }
                        
        public async Task SetTextAnimated(string text, double delayAfter = 0.0, double pace = 0.05)   
        {           
            var chars = text.ToCharArray(); 
            var idx = 0;
            var len = chars.Length;
            var tcs = new TaskCompletionSource<bool>();

            var str = new NSMutableAttributedString(text);
            AttributedText = str;

            int didPause = 0;
            NSTimer.CreateScheduledTimer(pace, true, t =>
            {                
                while (chars[idx] == ' ') 
                {
                    idx++;
                        
                    if (idx == chars.Length)
                    {
                        t.Invalidate();
                        tcs.TrySetResult(true);
                        return;
                    }
                }

                var attPart = new NSRange(0, idx + 1);
                var hiddenPart = new NSRange(idx + 1, len - (idx + 1));
                    
                str.AddAttribute(new NSString("NSColor"), UIColor.White, attPart);
                str.AddAttribute(new NSString("NSColor"), UIColor.Black, hiddenPart);
                AttributedText = str;

                int targetDelay = 0;
                if (_delayChars.TryGetValue(chars[idx], out targetDelay))
                {
                    if (didPause == 0)
                        SoundManager.PlaySound("text");    

                    if (didPause < targetDelay)
                    {
                        didPause++;
                        return;
                    } 
                    else didPause = 0;
                }
                else
                    SoundManager.PlaySound("text");
                    
                idx++; 
                                        
                if (idx == chars.Length)
                { 
                    t.Invalidate();
                    tcs.TrySetResult(true);
                    return; 
                }
            });

            await tcs.Task;
            await Task.Delay(TimeSpan.FromSeconds(delayAfter));
        }        }
}