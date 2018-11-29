using ARKit;
using ARKitMeetup.Demos.D1;
using ARKitMeetup.Demos.Helpers;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using SceneKit;

namespace ARKitMeetup.Demos.D502
{
    [DisplayInMenu(DisplayName = "Face Tracking", DisplayDescription = "and creepy masks")]
    public class FaceTrackingViewController : BaseARViewController
    {
        public FaceNode FaceNode;

        public override ARConfiguration GetARConfiguration()
            => new ARFaceTrackingConfiguration();

        public override SCNScene GetInitialScene()
        {
            FaceNode = new FaceNode(ARSCNFaceGeometry.Create(SCNView.Device));

            return base.GetInitialScene();
        }

        public override void OnNodeAddedForAnchor(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            base.OnNodeAddedForAnchor(renderer, node, anchor);

            if (!(anchor is ARFaceAnchor faceAnchor))
                return;

            foreach (var child in node.ChildNodes)
                child.RemoveFromParentNode();

            node.AddChildNode(FaceNode);

            SoundManager.PlaySound("spooky");
        }

        public override void OnNodeUpdatedForAnchor(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            base.OnNodeUpdatedForAnchor(renderer, node, anchor);

            if (!(anchor is ARFaceAnchor faceAnchor))
                return;

            FaceNode.Update(faceAnchor.Geometry);
        }
    }
}
