using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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
            ListIPAddresses();
#if DEBUG
            new Continuous.Server.HttpServer().Run();
#endif
            SoundManager.Init();
            
            Window = new UIWindow();
            Window.RootViewController = new HomeViewController();
            Window.MakeKeyAndVisible();
            
            return true;
        }

        private void ListIPAddresses()
        {        
            try
            {
                var inet = 
                    NetworkInterface
                        .GetAllNetworkInterfaces()
                        .SelectMany(x => 
                            x.GetIPProperties()
                             .UnicastAddresses.Where(y => y.Address.AddressFamily == AddressFamily.InterNetwork))
                        .Select(y => y.Address);

                Debug.WriteLine(String.Join(Environment.NewLine, inet));
                
            } catch (Exception ex) { Debug.WriteLine(ex); }
        }
    }
}

