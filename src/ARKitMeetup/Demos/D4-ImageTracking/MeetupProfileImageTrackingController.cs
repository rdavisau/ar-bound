using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARKit;
using ARKitMeetup.Demos.D4;
using ARKitMeetup.Demos.Helpers;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using Foundation;
using SceneKit;
using UIKit;

namespace ARKitMeetup.Demos.D401
{
    [DisplayInMenu(DisplayName = "Image Detection", DisplayDescription = "NSSet<ARReferenceImage> let's go")]
    public class MeetupProfileImageTrackingController : GenericImageTrackingViewController<MeetupProfile>
    {
        protected override async Task<List<ImageDetectionReferenceItem<MeetupProfile>>> GetReferenceItems()
        {
            // meetup api is flaky so try a few times
            int i = 0;
            while (i++ < 3)
            {
                try { return await StaticMeetupService.GetRsvps(); }
                catch (Exception ex)
                {
                    SoundManager.PlaySound("miss");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            
            // if we got here, t o o o  bad
            SoundManager.PlaySound("spooky");
            return new List<ImageDetectionReferenceItem<MeetupProfile>>();
        }
        
        protected override ARReferenceImage GetReferenceImage(ImageDetectionReferenceItem<MeetupProfile> item)
        {
            var imageData = NSData.FromArray(item.ImageData);
            var image = UIImage.LoadFromData(imageData).Resize(500);

            return new ARReferenceImage(image.CGImage, ImageIO.CGImagePropertyOrientation.Up, item.RealWorldSizeCms / 100);
        }

        protected override SCNNode GetNodeForItem(MeetupProfile item)
        {
            var node =
                SCNScene.FromFile("art.scnassets/thin-prefab.scn")
                    .RootNode
                    .ChildNodes
                    .Where(x => x.Name.StartsWith("char"))
                    .ToList()
                    .Random();

            node = node.Clone();
            node.Geometry = node.Geometry.Copy() as SCNGeometry;
            node.Geometry.Materials = new[] { node.Geometry.Materials.First().Copy() as SCNMaterial };
            
            var imageData =  ReferenceItems.FirstOrDefault(x => x.ItemData == item).DisplayData;
            var img = UIImage.LoadFromData(NSData.FromArray(imageData));
            node.Geometry.FirstMaterial.Diffuse.Contents = img;

            node.RemoveFromParentNode();
            node.Scale = SmallScale;
            
            return node;
        }

        public override void SelectionChanged(SCNNode fromNode, MeetupProfile fromItem, SCNNode toNode, MeetupProfile toItem)
        {
            if (toNode != null)
            {
                SoundManager.PlaySound("attack1");
                toNode.Scale = LargeScale;
            }

            if (fromNode != null)
                fromNode.Scale = SmallScale;
        }

        private UIViewController _displayedViewController;
        public override void ItemTapped(SCNNode node, MeetupProfile item)
        {
            base.ItemTapped(node, item);
            
            var dialogController = new DisplayMeetupProfileViewController(item);
            var dialogView = dialogController.View;

            dialogController.WillMoveToParentViewController(this);
            View.AddSubview(dialogView);
            View.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(dialogView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(dialogView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1, -100),
                NSLayoutConstraint.Create(dialogView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 5),
            });
            dialogController.DidMoveToParentViewController(this);
                      
            _displayedViewController = dialogController;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            if (_displayedViewController?.View != null)
            {
                try {
                    _displayedViewController.View.RemoveFromSuperview();
                    _displayedViewController = null;
                } catch { }
            }
            else base.TouchesEnded(touches, evt);
        }

        private SCNVector3 SmallScale = new SCNVector3(.15f, .15f, 0.0005f);
        private SCNVector3 LargeScale = new SCNVector3(.30f, .30f, 0.001f);
    }
}
