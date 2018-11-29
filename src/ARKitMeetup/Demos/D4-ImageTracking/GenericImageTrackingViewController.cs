using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARKit;
using ARKitMeetup.Demos.D1;
using ARKitMeetup.Demos.Helpers;
using ARKitMeetup.Helpers;
using Foundation;
using SceneKit;
using UIKit;

namespace ARKitMeetup.Demos.D4
{
    public abstract class GenericImageTrackingViewController<T> : BaseARViewController
    {    
        protected abstract Task<List<ImageDetectionReferenceItem<T>>> GetReferenceItems();
        protected abstract ARReferenceImage GetReferenceImage(ImageDetectionReferenceItem<T> item);
        protected abstract SCNNode GetNodeForItem(T item);
        public virtual void SelectionChanged(SCNNode fromNode, T fromItem, SCNNode toNode, T toItem) { }
        public virtual void ItemTapped(SCNNode node, T item) { }
        
        public SCNNode SelectedNode { get; set; }
        public T SelectedItem { get; set; }

        public UIView Crosshair = new UIView { BackgroundColor = UIColor.Gray, TranslatesAutoresizingMaskIntoConstraints = false };   

        protected List<ImageDetectionReferenceItem<T>> ReferenceItems { get; set; } = new List<ImageDetectionReferenceItem<T>>();
        protected Dictionary<ARReferenceImage, ImageDetectionReferenceItem<T>> ReferenceLookup = new Dictionary<ARReferenceImage, ImageDetectionReferenceItem<T>>();
        protected Dictionary<SCNNode, ImageDetectionReferenceItem<T>> NodeLookup = new Dictionary<SCNNode, ImageDetectionReferenceItem<T>>();

        public override ARConfiguration GetARConfiguration()
        {
            ReferenceLookup = ReferenceItems.ToDictionary(GetReferenceImage, x => x);
        
            return new ARWorldTrackingConfiguration { DetectionImages = new NSSet<ARReferenceImage>(ReferenceLookup.Keys.ToArray()) };    
        }

        public override void OnNodeAddedForAnchor(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            base.OnNodeAddedForAnchor(renderer, node, anchor);

            var imageAnchor = anchor as ARImageAnchor;
            if (imageAnchor == null)
                return;
                
            SoundManager.PlaySound("camera");

            var anchorTransform = imageAnchor.Transform;
            
            var referenceImage = imageAnchor.ReferenceImage;
            var item = ReferenceLookup[referenceImage];
            var next = GetNodeForItem(item.ItemData);
            
            next.Position = new SCNVector3(anchorTransform.Column3.X, anchorTransform.Column3.Y + 0.05f, anchorTransform.Column3.Z);
            NodeLookup[next] = item;

            var hoverDistance = .025f;
            var hoverDuration = 2;
            var up = SCNAction.MoveBy(0, hoverDistance, 0, hoverDuration);
            var down = SCNAction.MoveBy(0, -hoverDistance, 0, hoverDuration);
            var seq = SCNAction.Sequence(new[] { up, down });
            seq.TimingMode = SCNActionTimingMode.EaseInEaseOut;

            var hover = SCNAction.RepeatActionForever(seq);
            next.RunAction(hover);
            
            SCNView.Scene.RootNode.AddChildNode(next);
        }
        
        public override void OnFrameUpdate(ARSession session, ARFrame frame)
        {
            base.OnFrameUpdate(session, frame);

            var results = SCNView.HitTest(View.Center, new SCNHitTestOptions { SortResults = true, BackFaceCulling = false, SearchMode = SCNHitTestSearchMode.All, FirstFoundOnly = false });

            SCNNode newNode = null;
            T newItem = default(T);
            
            var match = results.FirstOrDefault(r => NodeLookup.ContainsKey(r.Node));
            if (match != null)
            {
                newNode = match.Node;
                newItem = NodeLookup[newNode].ItemData;
            }

            var changed = newNode != SelectedNode;

            var oldNode = SelectedNode;
            var oldItem = SelectedItem;
            
            SelectedNode = newNode;
            SelectedItem = newItem;
            
            if (changed)
                SelectionChanged(oldNode, oldItem, SelectedNode, SelectedItem);

            Crosshair.BackgroundColor = SelectedNode != null
                ? UIColor.Green
                : UIColor.Gray;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            if (SelectedNode != null)
                ItemTapped(SelectedNode, SelectedItem); 
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.AddSubview(Crosshair);
            View.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(Crosshair, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(Crosshair, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1, 0),
                NSLayoutConstraint.Create(Crosshair, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, 5),
                NSLayoutConstraint.Create(Crosshair, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 5)
            });
        }

        public override async Task WaitForReady()
        {
            ReferenceItems = await GetReferenceItems();
            await base.WaitForReady();
        }
    }
}
