using ARKit;
using SceneKit;
using UIKit;

namespace ARKitMeetup.Demos.Helpers
{
    public class FaceNode : SCNNode
    {
        public FaceNode(ARSCNFaceGeometry faceGeometry)
        {
            var mat = faceGeometry.FirstMaterial;
            mat.LightingModelName = SCNLightingModel.PhysicallyBased;
            mat.Diffuse.Contents = UIImage.FromFile("tile-small.png");
            mat.Diffuse.ContentsTransform = SCNMatrix4.Scale(32, 32, 0);
            mat.Diffuse.WrapS = SCNWrapMode.Repeat;
            mat.Diffuse.WrapT = SCNWrapMode.Repeat;

            Geometry = faceGeometry;
        }

        public void Update(ARFaceGeometry newGeometry)
        {
            ((ARSCNFaceGeometry)Geometry).Update(newGeometry);
        }
    }
}
