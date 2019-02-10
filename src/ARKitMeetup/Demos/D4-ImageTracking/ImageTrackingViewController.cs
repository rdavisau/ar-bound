using System.Collections.Generic;
using System.Linq;
using ARKit;
using ARKitMeetup.Demos.D1;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using SceneKit;
using UIKit;

namespace ARKitMeetup.Demos.D402
{
    [DisplayInMenu(DisplayName = "Image Tracking", DisplayDescription = "Amazing new Earthbound Card Game")]
    public class ImageTrackingController : BaseARViewController
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
                Width = refSize.Width * .9f,
                Length = refSize.Height * .9f,  
                Height = 0.0001f,
                ChamferRadius = 0
            };

            box.FirstMaterial.Diffuse.Contents = UIColor.White.ColorWithAlpha(.95f); 
            
            var pf = Prefabs[i++];
            
            node.AddChildNode(new SCNNode { Geometry = box });
            node.AddChildNode(pf);

            pf.Position = SCNVector3.Zero;
        }
    }
}
