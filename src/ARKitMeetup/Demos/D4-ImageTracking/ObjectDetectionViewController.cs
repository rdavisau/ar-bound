using ARKit;
using ARKitMeetup.Helpers;
using ARKitMeetup.Abstractions;
using System.Diagnostics;
using SceneKit;
using System;
using ARKitMeetup.Demos.D1;
using ARKitMeetup.Models;
using UIKit;
using ARKitMeetup.Demos.D204;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace ARKitMeetup.Demos.D404
{
    [DisplayInMenu(DisplayName = "Earthbound Series Museum", DisplayDescription = "Powered by ARKit Object Detection")]
    public class DetectObjectsViewController : BaseARViewController
    {
        public Dictionary<string, SCNNode> Panels { get; private set; }
        public Dictionary<SCNNode, SCNNode> NodeLookup { get; private set; } = new Dictionary<SCNNode, SCNNode>();
        public SCNNode SelectedNode { get; private set; }
        
        public EBTextDialogViewController TitleDialog { get; private set; }
        public EBTextDialogViewController InfoDialog { get; private set; }

        public override ARConfiguration GetARConfiguration()
        {
            return new ARWorldTrackingConfiguration
            {
                DetectionObjects = ARReferenceObject.GetReferenceObjects("objects", null),
            };
        }
        
        public override SCNScene GetInitialScene()
        {
            Panels = SCNScene.FromFile("art.scnassets/basic-panels.scn")
                .RootNode
                .ChildNodes
                .Where(x => x.Name.StartsWith("m"))
                .ToDictionary(x => x.Name);

            foreach (var pf in Panels.Values)
            {
                pf.RemoveFromParentNode();
                pf.Scale = new SCNVector3(pf.Scale.X * .1f, pf.Scale.Y * .1f, pf.Scale.Z * .1f);
            }

            TitleDialog = new EBTextDialogViewController(24);
            TitleDialog.Label.TextAlignment = UITextAlignment.Center;
            
            InfoDialog = new EBTextDialogViewController(24);
            InfoDialog.Label.TextAlignment = UITextAlignment.Center;
            
            View.AddSubview(InfoDialog.View);
            View.AddSubview(TitleDialog.View);
            
            View.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(TitleDialog.View, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(TitleDialog.View, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, InfoDialog.View, NSLayoutAttribute.Top, 1, -65),
                NSLayoutConstraint.Create(TitleDialog.View, NSLayoutAttribute.Width, NSLayoutRelation.LessThanOrEqual, View, NSLayoutAttribute.Width, 1, -125),
            
                NSLayoutConstraint.Create(InfoDialog.View, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(InfoDialog.View, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1, 0),
                NSLayoutConstraint.Create(InfoDialog.View, NSLayoutAttribute.Width, NSLayoutRelation.LessThanOrEqual, View, NSLayoutAttribute.Width, 1, -40),
            });
            InfoDialog.View.Transform = CGAffineTransform.MakeTranslation(0, View.Frame.Height);
            TitleDialog.View.Transform = CGAffineTransform.MakeTranslation(0, View.Frame.Height);
            InfoDialog.Label.TextColor = UIColor.White;
            TitleDialog.Label.TextColor = UIColor.White;
            
            return base.GetInitialScene();
        }

        public override void OnNodeAddedForAnchor(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            base.OnNodeAddedForAnchor(renderer, node, anchor);

            if (anchor is ARObjectAnchor objectAnchor)
                OnFoundThing(node, objectAnchor);
        }

        bool adding = false;
        private HashSet<string> _alreadySeen = new HashSet<string>();
        private void OnFoundThing(SCNNode node, ARObjectAnchor anchor)
        {
            adding = true;
            var size = anchor.ReferenceObject.Extent;
            var name = ReplacementNameFor(anchor.Name.Split('-')[0]);

            if (_alreadySeen.Contains(name))
                return;

            _alreadySeen.Add(name);
            
            if (!_contentFor.TryGetValue(name, out var content))
            {
                SoundManager.PlaySound("spooky");
                adding = false;
                return;
            }

            NodeLookup[node] = node;
            
            var panel = Panels[content.Logo];
            panel.Position = new SCNVector3(0, content.Offset, -.05f);
            node.AddChildNode(panel);

            var refSize = anchor.ReferenceObject.Extent;
            var invisiNode = new SCNNode
            {
                Geometry = new SCNBox { Height = refSize.Y, Width = refSize.X, Length = refSize.Z },
                Position = new SCNVector3(0, .05f, 0)
            };
            invisiNode.Geometry.FirstMaterial.Diffuse.Contents = UIColor.FromWhiteAlpha(1.0f, 0.05f);
            
            node.AddChildNode(invisiNode);
            NodeLookup[invisiNode] = node;
            node.Name = name;

            foreach (var sound in _contentFor.Values.Select(x => x.BGM))
                SoundManager.PauseSound(sound);

            SoundManager.PlaySound(content.BGM, false);
            adding = false;
        }

        public override void OnFrameUpdate(ARSession session, ARFrame frame)
        {
            base.OnFrameUpdate(session, frame);
            if (adding)
                return;
            
            var results = SCNView.HitTest(View.Center, new SCNHitTestOptions { SortResults = true, BackFaceCulling = false, SearchMode = SCNHitTestSearchMode.All, FirstFoundOnly = false });

            SCNNode newNode = null;
            
            var match = results.FirstOrDefault(r => NodeLookup.ContainsKey(r.Node));
            if (match != null)
            {
                newNode = match.Node;
            }

            var changed = newNode != SelectedNode;
            var oldNode = SelectedNode;
            
            SelectedNode = newNode;
            
            if (changed)
            {
                var panelTransform =
                    SelectedNode != null
                    ? CGAffineTransform.MakeIdentity()
                    : CGAffineTransform.MakeTranslation(0, 1000);

                if (SelectedNode != null)
                {
                    var content = _contentFor[NodeLookup[match.Node].Name];
                    InfoDialog.Label.Text = content.Description;
                    TitleDialog.Label.Text = content.Name;
                }    
                
                UIView.Animate(.2, 0, UIViewAnimationOptions.CurveEaseOut, 
                    () => 
                    { 
                        InfoDialog.View.Transform = panelTransform;
                        TitleDialog.View.Transform = panelTransform;
                    }, null);
            }
        }
        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            foreach (var sound in _contentFor.Values.Select(x => x.BGM))
                SoundManager.PauseSound(sound);
        }

        private string ReplacementNameFor(string name)
        {
            return name == "monkey"
                ? "ninten"
                : name;
        }

        private readonly Dictionary<string, Content> _contentFor = new Dictionary<string, Content>
        {
            ["ninten"] = Content.For("win-m1", "m1", "Earthbound 0 (NES)", .23,
                        "EarthBound 0 is the first installment in the Mother series. It tells the story of Ninten, " + 
                        "a 12-year-old boy from Podunk, who travels around America using his psychic powers to " +
                        "save the planet from an evil race of mind-controlling aliens."
                        ),
                        
            ["ness"] = Content.For("win-m2", "m2", "Earthbound (SNES)", .12, 
                        "EarthBound, subtitled 'The War Against Giygas!' chronicles the adventures of Ness, a " + 
                        "13-year-old boy who journeys around the world using his 'PSI' in order to save the " +
                        "future from an alien of pure evil, intending to sentence all of reality to the horror of eternal darkness."
                        ),
                        
            ["lucas"] = Content.For("win-m3", "m3", "Mother 3 (GBA)", .12,
                        "Mother 3 is set some unknown amount of years after EarthBound. Chaos ensues after an " + 
                        "invasion by the Pigmask Army, who slowly construct a police state while experimenting on the land's flora and fauna. " +
                        "Lucas and his friends must band together to defeat the Pigmask Army."
                        )
        };
    }

    public class Content
    {
        public static Content For(string bgm, string logo, string name, double offset, string desc)
            => new Content
            {
                BGM = bgm,
                Logo = logo,
                Name = name,
                Offset = (float) offset,
                Description = desc
            };

        public string BGM { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public float Offset { get; set; }
        public string Description { get; set; }
    }
}
