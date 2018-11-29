using System;
using UIKit;

namespace ARKitMeetup.Helpers
{
    public class EBDialogViewController : UIViewController
        {
            public UIImageView MenuContainerImageView { get; set; }
            public UIView ContentView { get; private set; }
            
            public void AddToView(UIView view, params NSLayoutConstraint[] constraints)
            {
                var pvc = view.GetViewController();
                Console.WriteLine($"pvc is {pvc}");

                WillMoveToParentViewController(pvc); 
                pvc.AddChildViewController(this);
                pvc.View.AddSubview(View);
                DidMoveToParentViewController(pvc);

                foreach (var constraint in constraints)
                    pvc.View.AddConstraint(constraint);

                pvc.View.LayoutIfNeeded();  
            }
 
            public override void ViewDidLoad()
            {
                base.ViewDidLoad();
                View.TranslatesAutoresizingMaskIntoConstraints = false;

                MenuContainerImageView = new UIImageView(UIImage.FromFile("container2.png").CreateResizableImage(new UIEdgeInsets(20, 20, 20, 20)))
                {
                    TranslatesAutoresizingMaskIntoConstraints = false
                };

                ContentView = new UIView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    BackgroundColor = UIColor.Black,
                };

                View.AddSubviews(MenuContainerImageView, ContentView);

                View.AddConstraints(new[]
                {
                    NSLayoutConstraint.Create(MenuContainerImageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                    NSLayoutConstraint.Create(MenuContainerImageView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, 0),
                    NSLayoutConstraint.Create(MenuContainerImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Top, 1, -20),
                    NSLayoutConstraint.Create(MenuContainerImageView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Bottom, 1, 20),

                    NSLayoutConstraint.Create(ContentView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, MenuContainerImageView, NSLayoutAttribute.Leading, 1, 20),
                    NSLayoutConstraint.Create(ContentView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, MenuContainerImageView, NSLayoutAttribute.Trailing, 1, -20),
                });
            }

            private UIView _content;
            public void SetContent(UIView content)
            {
                _content?.RemoveFromSuperview();
                _content = content;

                ContentView.AddSubview(_content);
                ContentView.AddConstraints(new[] 
                {
                    NSLayoutConstraint.Create(_content, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Leading, 1, 8),
                    NSLayoutConstraint.Create(_content, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, ContentView, NSLayoutAttribute.Trailing, 1, 8),
                    NSLayoutConstraint.Create(ContentView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _content, NSLayoutAttribute.Top, 1, 0),
                    NSLayoutConstraint.Create(ContentView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, _content, NSLayoutAttribute.Bottom, 1, 0),
                });
            }
        }
}
