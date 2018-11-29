using System;
using System.Collections.Generic;
using System.Linq;
using ARKitMeetup.Demos.D1;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using Foundation;
using SceneKit;
using UIKit;

namespace ARKitMeetup.Demos.D204
{
    [DisplayInMenu(DisplayName = "Earthbound Home Decorator", DisplayDescription = "Create your perfect earthbound room (except it's janky)")]
    public class EarthboundHomeDecoratorViewController : BaseARViewController
    {
        public List<UIImage> Images { get; private set; }
        public UITableView ImageTableView { get; private set; }
        public SCNNode PaintNode { get; private set; }

        public EarthboundHomeDecoratorViewController()
        {
            Images = NSBundle.MainBundle
                .PathsForResources(".gif", "sprites")
                .OrderBy(f => f)
                .Select(UIImage.FromFile)
                .ToList();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetupImageTableView();
        }

        public override void OnSessionBegan()
        {
            var b = SCNScene.FromFile("art.scnassets/basic-prefab.scn")
                .RootNode
                .ChildNodes
                .First(x => x.Name.StartsWith("char", StringComparison.Ordinal));

            PaintNode = b.Clone();
            PaintNode.RemoveFromParentNode();
            PaintNode.Position = new SCNVector3(0, 0, -1.5f);
            PaintNode.Opacity = 0.75f;

            SCNView.PointOfView.AddChildNode(PaintNode);

            base.OnSessionBegan();
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            if (!SessionStarted)
                return;

            var placeNode = PaintNode.Clone();
            var transform = PaintNode.WorldTransform;

            placeNode.Geometry = PaintNode.Geometry.Copy() as SCNGeometry;
            placeNode.Geometry.Materials = new[] { placeNode.Geometry.Materials.First().Copy() as SCNMaterial };
            placeNode.RemoveFromParentNode();
            placeNode.Opacity = 1;
            placeNode.WorldTransform = transform;
            placeNode.Look(SCNView.PointOfView.Position);

            SCNView.Scene.RootNode.AddChildNode(placeNode);
        }

        private void SetSelectedImage(UIImage image)
        {
            SoundManager.PlaySound("click");
            PaintNode.Geometry.Materials.First().Diffuse.Contents = image;
        }

        #region Setup TableView
        private void SetupImageTableView()
        {
            ImageTableView = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Source = CreateTableViewSource(),
            };
            ImageTableView.Layer.CornerRadius = 10f;

            View.AddSubview(ImageTableView);
            View.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(ImageTableView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, 10),
                NSLayoutConstraint.Create(ImageTableView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1, 0),
                NSLayoutConstraint.Create(ImageTableView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 100),
                NSLayoutConstraint.Create(ImageTableView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, -100),
                NSLayoutConstraint.Create(ImageTableView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, 60),
            });
        }

        private UITableViewSource CreateTableViewSource()
        {
            const string reuse = "imageCell";
            return new InlineTableViewSourceWithoutRowHeight
            {
                _RowsInSection = (tv, indexPath) => Images.Count(),
                _GetCell = (tv, indexPath) =>
                {
                    var cell = tv.DequeueReusableCell(reuse) ?? CreateTableViewCell();
                    var imageView = cell.ContentView.Subviews.OfType<UIImageView>().First();

                    imageView.Image = Images[indexPath.Row];

                    return cell;
                },
                _RowSelected = (tv, indexPath) => SetSelectedImage(Images[indexPath.Row])
            };
        }

        private static UITableViewCell CreateTableViewCell()
        {
            var cell = new UITableViewCell { };
            var imageView = new UIImageView { ContentMode = UIViewContentMode.Center, TranslatesAutoresizingMaskIntoConstraints = false };
            cell.ContentView.AddSubview(imageView);
            cell.ContentView.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(imageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, cell.ContentView, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(imageView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, cell.ContentView, NSLayoutAttribute.Trailing, 1, 0),
                NSLayoutConstraint.Create(imageView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, cell.ContentView, NSLayoutAttribute.CenterY, 1, 0),
                NSLayoutConstraint.Create(imageView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 30),
            });

            return cell;
        }

        #endregion
    }
}
