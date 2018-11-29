using ARKitMeetup.Helpers;
using Foundation;
using UIKit;

namespace ARKitMeetup
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
#if DEBUG
            new Continuous.Server.HttpServer().Run();
#endif
            SoundManager.Init();
            
            Window = new UIWindow();
            Window.RootViewController = new HomeViewController();
            Window.MakeKeyAndVisible();
            
            return true;
        }
    }
}

