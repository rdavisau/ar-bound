using System;
using UIKit;

namespace ARKitMeetup.Abstractions
{
    public class BaseViewController : UIViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UIApplication.SharedApplication.IdleTimerDisabled = true;
        }
    }
}
