using System.Linq;
using ARKit;
using ARKitMeetup.Demos.D301;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using UIKit;

namespace ARKitMeetup.Demos.D302
{
    /// <summary>
    /// Extends PlaneTrackingViewController to add a crosshair to the screen and run hittesting on every frame
    /// If there are results, the crosshair illumniates and a sound plays
    /// </summary>
    [DisplayInMenu(DisplayName = "Hit Test Plane Tracking", DisplayDescription = "AR hit testing on detected surfaces")]
    public class PlaneTrackingWithHitTesting : PlaneTrackingViewController
    {
        public UIView Crosshair = new UIView { BackgroundColor = UIColor.Gray, TranslatesAutoresizingMaskIntoConstraints = false };

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.AddSubview(Crosshair);
            View.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(Crosshair, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(Crosshair, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1, 0),
                NSLayoutConstraint.Create(Crosshair, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, 5),
                NSLayoutConstraint.Create(Crosshair, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 5)
            });
        }

        public override void OnFrameUpdate(ARSession session, ARFrame frame)
        {
            base.OnFrameUpdate(session, frame);

            if (PlaneTrackingEnabled)
                return;

            var hits = frame.HitTest(new CoreGraphics.CGPoint(0.5, 0.5), ARHitTestResultType.ExistingPlaneUsingExtent);
            if (!hits?.Any() ?? true)
                Crosshair.BackgroundColor = UIColor.Gray;
            else
            {
                Crosshair.BackgroundColor = UIColor.Green;

                if (PlaySoundOnNodeDetection())
                    SoundManager.PlaySound("text");
            }
        }
    }
}