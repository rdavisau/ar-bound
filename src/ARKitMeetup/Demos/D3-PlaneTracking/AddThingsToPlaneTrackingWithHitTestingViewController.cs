using System.Collections.Generic;
using System.Linq;
using ARKit;
using ARKitMeetup.Demos.D204;
using ARKitMeetup.Demos.D302;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using Foundation;
using SceneKit;
using UIKit;

namespace ARKitMeetup.Demos.D303
{
    /// <summary>
    /// Extends PlaneTrackingWithHitTestingViewController to allow placing of new nodes
    /// at the location of a positive hit test
    /// </summary>
    [DisplayInMenu(DisplayName = "Add things to Hit Test Plane Tracking", DisplayDescription = "A little bit better than Earthbound Home Designer")]
    public class AddThingsToPlaneTrackingWithHitTestingViewController : PlaneTrackingWithHitTesting
    {
        public List<SCNNode> Prefabs;
                
        public override SCNScene GetInitialScene()
        {
            Prefabs = SCNScene.FromFile("art.scnassets/thin-prefab.scn")
                .RootNode
                .ChildNodes
                .Where(x => x.Name.StartsWith("char"))
                .ToList();
                    
            foreach (var pf in Prefabs)
                pf.RemoveFromParentNode();
                            
            return base.GetInitialScene();
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            if (!SessionStarted || PlaneTrackingEnabled)
                return;

            var frame = SCNView.Session.CurrentFrame;
            if (frame is null)
                return;
                
            using (frame)
            {
                var hits = frame.HitTest(new CoreGraphics.CGPoint(0.5, 0.5), ARHitTestResultType.ExistingPlaneUsingExtent);
                var target = hits.FirstOrDefault(x => x.Anchor as ARPlaneAnchor != null);
                if (target == null)
                    return;
                                    
                var wt = target.WorldTransform.ToSCNMatrix4();
                                 
                var next = Prefabs.Random().Clone();
                next.Scale = new SCNVector3(.15f, .15f, .0005f);
                next.Position = new SCNVector3(wt.Column3.X, wt.Column3.Y + .5f, wt.Column3.Z);
                next.Look(SCNView.PointOfView.WorldPosition);
               
                var body = SCNPhysicsBody.CreateDynamicBody();
                body.PhysicsShape = SCNPhysicsShape.Create(next, new NSDictionary());
                body.Restitution = 0.5f;
                body.Friction = 0.5f;
                next.PhysicsBody = body;
                
                SCNView.Scene.RootNode.AddChildNode(next); 
            }
        }
    }
}