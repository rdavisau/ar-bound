using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using ARKit;
using ARKitMeetup.Demos.D1;
using ARKitMeetup.Demos.Helpers;
using ARKitMeetup.Helpers;
using Foundation;
using SceneKit;
using UIKit;
using ARKitMeetup.Models;
using System;
using ARKitMeetup.Abstractions;

namespace ARKitMeetup.Demos.D4ImageTracking
{
    public class ImageTrackingViewController : BaseViewController 
    { 
        public ARSCNView SCNView { get; set; }
        public ARConfiguration TrackingConfiguration { get; private set; }

        public override void ViewDidLoad()
        {   
            base.ViewDidLoad();
                                                
            SCNView = WH.GetOrSet(nameof(SCNView), new ARSCNView { Frame = View.Frame });            
            View.AddSubview(SCNView);

            TrackingConfiguration = new ARWorldTrackingConfiguration { };

            SCNView.Session.Run(TrackingConfiguration, ARSessionRunOptions.RemoveExistingAnchors | ARSessionRunOptions.ResetTracking); 
        }
    }   
}
