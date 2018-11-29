using System;
using System.Linq;
using ARKit;
using ARKitMeetup.Demos.D502;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using CoreFoundation;
using Foundation;
using SceneKit;
using UIKit;

namespace ARKitMeetup.Demos.D503
{
    [DisplayInMenu(DisplayName = "Expression Monitoring", DisplayDescription = "How do you do?")]
    public class ExpressionMonitoringViewController : FaceTrackingViewController
    {
        public UILabel Label { get; set; }

        public override SCNScene GetInitialScene()
        {
            Label = new UILabel
            {
                BackgroundColor = UIColor.Black.ColorWithAlpha(.5f),
                Font = UIFont.FromName("Apple-Kid", 24),
                TextColor = UIColor.White,
                Lines = 0,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            var c = new EBDialogViewController();
            c.AddToView(
                View,
                NSLayoutConstraint.Create(c.View, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(c.View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 60),
                NSLayoutConstraint.Create(c.View, NSLayoutAttribute.Width, NSLayoutRelation.Equal, View, NSLayoutAttribute.Width, 1, -100))
                ;

            c.SetContent(Label);

            return base.GetInitialScene();
        }

        ARBlendShapeLocationOptions _lastBlendShapes;
        public override void OnNodeUpdatedForAnchor(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            base.OnNodeUpdatedForAnchor(renderer, node, anchor);

            if (!(anchor is ARFaceAnchor faceAnchor))
                return;

            var blendShapes = faceAnchor.BlendShapes;

            // get the five strongest indicators to display
            var dominantParts = blendShapes.Dictionary
                .GroupBy(x => $"{x.Key}".Split('_')[0], x => (NSNumber)x.Value)
                .Select(x => new { Expression = x.Key, Value = x.Max(y => y.FloatValue) }) // take strongest of left or right when present
                .OrderByDescending(x => x.Value)
                .Select(x => $"{x.Expression}: {x.Value:P0}")
                .Take(5)
                .ToList();

            DispatchQueue.MainQueue.DispatchAsync(() => Label.Text = String.Join(Environment.NewLine, dominantParts));

            NaivelyAndIneffecientlyCheckForChangedExpression(node, blendShapes);
        }

        private void NaivelyAndIneffecientlyCheckForChangedExpression(SCNNode node, ARBlendShapeLocationOptions blendShapes)
        {
            if (_lastBlendShapes == null)
                _lastBlendShapes = blendShapes;

            // mouth wide open WOW
            if (blendShapes.JawOpen > .85 && _lastBlendShapes.JawOpen <= .85)
                SoundManager.PlaySound("wow");

            // blink
            var currBlink = Math.Max((double)blendShapes.EyeBlinkLeft, (double)blendShapes.EyeBlinkRight);
            var lastBlink = Math.Max((double)_lastBlendShapes.EyeBlinkLeft, (double)blendShapes.EyeBlinkRight);
            if (currBlink > .85 && lastBlink <= .85)
                SoundManager.PlaySound("miss");

            // smile
            if (blendShapes.MouthSmileRight > .9 && _lastBlendShapes.MouthSmileRight <= .9)
            {
                SoundManager.PlaySound("eb_newchar");
                AddOrbitingNode(node);
            }
                            
            // frowny looking face 
            if (Math.Max((double)blendShapes.BrowDownLeft, (double)blendShapes.BrowDownRight) > .7 &&
                Math.Max((double)_lastBlendShapes.BrowDownLeft, (double)_lastBlendShapes.BrowDownRight) <= .7)
                SoundManager.PlaySound("eb_behind");
                
            _lastBlendShapes = blendShapes;
        }

        // after "happy" expressions, we add a ness node orbiting the face
        // however, the face currently seems to be able to sit in front of everything else..
        // probably easy to fix
        private static void AddOrbitingNode(SCNNode node)
        {
            var helperNode = new SCNNode() { Position = new SCNVector3(0, 0, -1.5f) };
            node.AddChildNode(helperNode);

            var b = SCNScene.FromFile("art.scnassets/basic-prefab.scn")
                .RootNode
                .ChildNodes
                .First(x => x.Name.StartsWith("char", StringComparison.Ordinal)).Clone();

            b.RemoveFromParentNode();
            b.Scale = new SCNVector3(.25f, .25f, .000001f);
            b.Position = new SCNVector3(0, 0, 1.25f);
            helperNode.AddChildNode(b);

            var rot = SCNAction.RotateBy(0, 3, 0, 1.5);
            var infinite = SCNAction.RepeatActionForever(rot);
            helperNode.RunAction(infinite);
        }
    }
}
