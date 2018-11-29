using System;
using System.Collections.Generic;
using System.Linq;
using ARKitMeetup.Demos.D1;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using Foundation;
using SceneKit;
using UIKit;

namespace ARKitMeetup.Demos.D202
{
    [DisplayInMenu(DisplayName = "Prefabs Scene", DisplayDescription = "Using nodes defined in a SceneKit .scn file as 'prefabs' for programmatic use")]
    public class PrefabsSceneViewController : BaseARViewController
    {
        public override string GetPrepareText()
         => "In this demo you can tap to place a random prefab at your current location." + Environment.NewLine + Environment.NewLine + base.GetPrepareText();
    
        public List<SCNNode> Prefabs;
        public List<SCNNode> AllNodes { get; set; } = new List<SCNNode>();

        public override SCNScene GetInitialScene()
        {
            Prefabs = SCNScene.FromFile("art.scnassets/basic-prefab.scn")
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

            var next = Prefabs.Random().Clone();
            next.Position = SCNView.PointOfView.WorldPosition;

            SCNView.Scene.RootNode.AddChildNode(next);
            AllNodes.Add(next);

            SoundManager.PlaySound("attack");
        }
    }
}
