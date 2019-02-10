using System.Collections.Generic;
using System.Linq;
using ARKit;
using ARKitMeetup.Demos.D1;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using SceneKit;

namespace ARKitMeetup.Demos.D403
{
    [DisplayInMenu(DisplayName = "Image Tracking + SpriteKit", DisplayDescription = "Using a SpriteKit scene as a material")]
    public class ImageTrackingWithSpriteKitController : BaseARViewController
    {
        public List<SCNNode> Prefabs { get; private set; }

        public override ARConfiguration GetARConfiguration()
            => new ARImageTrackingConfiguration
            {
                TrackingImages = ARReferenceImage.GetReferenceImagesInGroup("cards", null),
                MaximumNumberOfTrackedImages = 4,
            };

        public override SCNScene GetInitialScene()
        {
            Prefabs = SCNScene.FromFile("art.scnassets/basic-prefab.scn")
                .RootNode
                .ChildNodes
                .Where(x => x.Name.StartsWith("char"))
                .ToList();

            foreach (var pf in Prefabs)
            {
                pf.RemoveFromParentNode();
                pf.Scale = new SCNVector3(.1f, .001f, .1f);
            }

            return base.GetInitialScene();
        }

        int i;
        public override void OnNodeAddedForAnchor(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            base.OnNodeAddedForAnchor(renderer, node, anchor);

            SoundManager.PlaySound("miss");

            var imageAnchor = anchor as ARImageAnchor;
            var refSize = imageAnchor.ReferenceImage.PhysicalSize;
            var box = new SCNBox
            {
                Width = refSize.Width * 1.75f,
                Length = refSize.Height * 1.75f,
                Height = 0.0001f,
                ChamferRadius = 0
            };

            box.FirstMaterial.Diffuse.Contents = ShaderScene.Random();
            box.TileTexture(3);

            var pf = Prefabs[i++];

            node.AddChildNode(new SCNNode { Geometry = box });
            node.AddChildNode(pf);

            pf.Position = SCNVector3.Zero;
            pf.RunAction(SCNAction.RotateBy(0, 1.5f, 0, 0.01));
        }
    }
}
