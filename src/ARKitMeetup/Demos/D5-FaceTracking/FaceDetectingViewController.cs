using ARKit;
using ARKitMeetup.Demos.D1;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using SceneKit;

namespace ARKitMeetup.Demos.D501
{
    [DisplayInMenu(DisplayName = "Face Detecting", DisplayDescription = "(nothing else)")]
    public class FaceDetectingViewController : BaseARViewController
    {
        public override ARConfiguration GetARConfiguration()
            => new ARFaceTrackingConfiguration();

        public override void OnNodeAddedForAnchor(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            base.OnNodeAddedForAnchor(renderer, node, anchor);

            if (!(anchor is ARFaceAnchor faceAnchor))
                return;

            SoundManager.PlaySound("spooky");
        }
    }
}
