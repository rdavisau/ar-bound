using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARKitMeetup.Demos;
using ARKitMeetup.Demos.D1;
using ARKitMeetup.Demos.D4;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ARKitMeetup
{
    public class HomeViewController : UIViewController
    {
        const int MenuViewCapInset = 20;
        const int MenuViewPadding = MenuViewCapInset + 10;
        
        public UIView BackgroundView { get; set; }
        public UIView HeaderView { get; set; }
        public UIView HeaderLabel { get; set; }
        public UILabel StartLabel { get; private set; }
        public UIView MenuView { get; set; }
        public UIImageView MenuContainerImageView { get; set; }
        public UITableView MenuTableView { get; private set; }

        public List<MenuItem> Scenes { get; set; }
        
        public override void ViewDidLoad()
        {
            SetupViews();
            DetectScenes();
            SetupTableView();
        }
        
        private void DetectScenes()
        {
            Scenes = 
                typeof(BaseARViewController).Assembly.GetTypes()
                .Where(x => x.IsSubclassOf(typeof(BaseARViewController)) || x == typeof(BaseARViewController))
                .Where(x => !x.IsAbstract)
                .OrderBy(x => x.Namespace)
                .Select(x =>
                {
                    var att = x.GetCustomAttribute<DisplayInMenuAttribute>();
                    if (att == null)
                        return null;

                    return new MenuItem
                    {
                        Title = att.DisplayName,
                        Description = att.DisplayDescription,
                        Type = x,
                    };
                })
                .Where(x => x != null)
                .ToList();
        }

        private void SetupTableView()
        {
            var selectedIndexPath = NSIndexPath.FromItemSection(0, 0);
            MenuTableView.Source = new InlineTableViewSourceWithoutRowHeight
            {
                _RowsInSection = (tv, section) => Scenes.Count,
                _GetCell = (tv, indexPath) =>
                {
                    var scene = Scenes[indexPath.Row];

                    var cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "abc") {BackgroundColor = UIColor.Clear};

                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                    cell.TextLabel.Text = scene.Title;
                    cell.TextLabel.Lines = 0;
                    cell.TextLabel.TextColor = UIColor.White;
                    cell.TextLabel.Font = UIFont.FromName("Apple-Kid", 48);

                    cell.DetailTextLabel.Text = scene.Description;
                    cell.DetailTextLabel.TextColor = UIColor.White.ColorWithAlpha(.75f);
                    cell.DetailTextLabel.Font = UIFont.FromName("Apple-Kid", 28);
                    cell.DetailTextLabel.Lines = 0;

                    cell.ImageView.Image = selectedIndexPath == indexPath
                        ? UIImage.FromFile("selection.png")
                        : UIImage.FromFile("noselection.png");
                    cell.ImageView.Transform = CGAffineTransform.MakeTranslation(-4, 0); 
                        
                    return cell;
                },
                _RowSelected = (tv, indexPath) =>
                {
                    var currSelection = selectedIndexPath;  
                    if (currSelection != indexPath)
                    {
                        selectedIndexPath = indexPath;

                        SoundManager.PlaySound("click");
                        MenuTableView.ReloadRows(new[] {currSelection, indexPath}, UITableViewRowAnimation.None);
                    } 
                    else
                    {
                        SoundManager.PlaySound("wow");
                        TransitionToScene(indexPath.Row);
                    }
                },
            };
        }
        
        private void TransitionToScene(int row)
        {
            SoundManager.PauseSound("006-choose_a_file");
            
            var scene = Scenes[row];
            
            UIView.Animate(.75f, () =>
            {
                MenuView.Transform = CGAffineTransform.MakeTranslation(0, View.Frame.Height);
                BackgroundView.BackgroundColor = UIColor.Black;
                HeaderView.Alpha = 0;       
            },
            () => PresentViewController(Activator.CreateInstance(scene.Type) as UIViewController, false, null));   
        }

        bool firstAppearance = true;
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SoundManager.PlaySound("006-choose_a_file", true);
            
            BackgroundView.Transform = CGAffineTransform.MakeIdentity();
            FakeAnimateBackground();

            UIView.Animate(1, 0, UIViewAnimationOptions.Autoreverse | UIViewAnimationOptions.Repeat | UIViewAnimationOptions.CurveEaseIn, 
                () => StartLabel.Alpha = 0, null); 
                
            if (firstAppearance)
            {
                firstAppearance = false;
                return; 
            }
            
            UIView.Animate(.75f, () =>
            {
                MenuView.Transform = CGAffineTransform.MakeIdentity();
                BackgroundView.BackgroundColor = UIColor.FromPatternImage(UIImage.FromFile("tile-small.png"));
                HeaderView.Alpha = 1;   
            });
        }

        private void SetupViews()
        {
            View.BackgroundColor = UIColor.Black; 
            BackgroundView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };
            BackgroundView.BackgroundColor = UIColor.FromPatternImage(UIImage.FromFile("tile-small.png"));  
            HeaderView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, };
            HeaderLabel = new UILabel
            {
                Font = UIFont.FromName("Apple-Kid", 120),
                Text = "AR Bound",
                AdjustsFontSizeToFitWidth = true,
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            HeaderLabel.Layer.ShadowOffset = CGSize.Empty;
            HeaderLabel.Layer.ShadowColor = UIColor.Black.CGColor;
            HeaderLabel.Layer.ShadowOpacity = .75f;

            StartLabel = new UILabel
            {
                Text = "SELECT A DEMO",
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.FromName("Apple-Kid", 32),
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            StartLabel.Layer.ShadowOffset = CGSize.Empty;
            StartLabel.Layer.ShadowColor = UIColor.Black.CGColor;
            StartLabel.Layer.ShadowOpacity = .75f;
            
            MenuView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false }; 
            MenuContainerImageView = new UIImageView(UIImage.FromFile("container.png").CreateResizableImage(new UIEdgeInsets(20, 20, 20, 20)))
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            MenuTableView = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Black,
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
            };

            HeaderView.AddSubviews(HeaderLabel, StartLabel);
            MenuView.AddSubviews(MenuContainerImageView, MenuTableView);
            View.AddSubviews(BackgroundView, HeaderView, MenuView);

            View.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(BackgroundView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 10, -100),
                NSLayoutConstraint.Create(BackgroundView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 10, 100),
                NSLayoutConstraint.Create(BackgroundView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 10, 100),
                NSLayoutConstraint.Create(BackgroundView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 10, -100),
            
                NSLayoutConstraint.Create(HeaderLabel, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, HeaderView, NSLayoutAttribute.CenterY, 1, 5),
                NSLayoutConstraint.Create(HeaderLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, HeaderView, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(HeaderLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, HeaderView, NSLayoutAttribute.Leading, 1, -10),
                NSLayoutConstraint.Create(HeaderLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, HeaderView, NSLayoutAttribute.Trailing, 1, 10),

                NSLayoutConstraint.Create(StartLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, HeaderLabel, NSLayoutAttribute.Bottom, 1, -10),  
                NSLayoutConstraint.Create(StartLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, HeaderView, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(StartLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, HeaderView, NSLayoutAttribute.Leading, 1, -10),
                NSLayoutConstraint.Create(StartLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, HeaderView, NSLayoutAttribute.Trailing, 1, 10),

                NSLayoutConstraint.Create(HeaderView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(HeaderView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, 0),
                NSLayoutConstraint.Create(HeaderView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(HeaderView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 200),

                NSLayoutConstraint.Create(MenuView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(MenuView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, 0),
                NSLayoutConstraint.Create(MenuView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0),
                NSLayoutConstraint.Create(MenuView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, HeaderView, NSLayoutAttribute.Bottom, 1, 0),

                NSLayoutConstraint.Create(MenuContainerImageView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, MenuView, NSLayoutAttribute.Leading, 1, 20),
                NSLayoutConstraint.Create(MenuContainerImageView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, MenuView, NSLayoutAttribute.Trailing, 1, -20),
                NSLayoutConstraint.Create(MenuContainerImageView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, MenuView, NSLayoutAttribute.Bottom, 1, -25),
                NSLayoutConstraint.Create(MenuContainerImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, MenuView, NSLayoutAttribute.Top, 1, 20),

                NSLayoutConstraint.Create(MenuTableView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, MenuContainerImageView, NSLayoutAttribute.Leading, 1, MenuViewPadding),
                NSLayoutConstraint.Create(MenuTableView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, MenuContainerImageView, NSLayoutAttribute.Trailing, 1, -MenuViewPadding),
                NSLayoutConstraint.Create(MenuTableView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, MenuContainerImageView, NSLayoutAttribute.Bottom, 1, -MenuViewPadding),
                NSLayoutConstraint.Create(MenuTableView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, MenuContainerImageView, NSLayoutAttribute.Top, 1, MenuViewPadding),
            });

            HeaderView.AddGestureRecognizer(new UITapGestureRecognizer(gr => SoundManager.ToggleSound("006-choose_a_file")));
            HeaderView.AddGestureRecognizer(new UILongPressGestureRecognizer(gr => PresentViewController(new DisplayMeetupProfileViewController(), true, null)));
        }
        
        private void FakeAnimateBackground()
        {
            // this is so bad but it works for long enough to make it through a meetup presentation
            // i'll be our little secret ok? 
            Task.Run(async () =>
            {
                var duration = 2500;   
                while (true)
                {
                    DispatchQueue.MainQueue.DispatchAsync(() => UIView.Animate(duration, 0, UIViewAnimationOptions.CurveLinear, () => BackgroundView.Transform = CGAffineTransform.MakeTranslation(-20000, -20000), null)); 
                    await Task.Delay(TimeSpan.FromSeconds(duration)); 
                    BackgroundView.Transform = CGAffineTransform.MakeIdentity();   
                } 
            });
        }
    }
}
