using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ARKit;
using ARKitMeetup.Abstractions;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using CoreGraphics;
using Foundation;
using SceneKit;
using UIKit;

namespace ARKitMeetup.Demos.D1
{
    /// <summary>
    /// This screen demonstrates World Tracking and includes a few debugging overlays.
    /// All other demo screens derive directly or indirectly from this one, so it includes
    /// several virtual methods that they can override in order to customise behaviour
    /// </summary>
    [DisplayInMenu(DisplayName = "Base AR Scene", DisplayDescription = "World tracking, nothing else")]
    public class BaseARViewController : BaseViewController
    {
        private TaskCompletionSource<bool> _pendingTouchCompleter = new TaskCompletionSource<bool>();
        protected bool SessionStarted;
        
        public ARSCNView SCNView { get; set; }
        public UIView PositionView { get; set; }
        public UILabel PositionLabel { get; set; }
        public UIImageView DoneButton { get; set; }
        public UIView MapView { get; set; }
        public UIView TrackingStatusIndicator { get; set; }

        public virtual ARConfiguration GetARConfiguration() 
            => new ARWorldTrackingConfiguration { PlaneDetection = ARPlaneDetection.Horizontal | ARPlaneDetection.Vertical };
        
        public virtual string GetPrepareText() 
            => "Find a good place to start. Then, tap to begin."; 

        public virtual SCNScene GetInitialScene() 
            => new SCNScene();
            
        public virtual void OnSessionBegan() 
            => SessionStarted = true;
            
        public virtual SCNDebugOptions GetDebugOptions() 
            => SCNDebugOptions.ShowBoundingBoxes | SCNDebugOptions.ShowWireframe | ARSCNDebugOptions.ShowFeaturePoints | ARSCNDebugOptions.ShowWorldOrigin;

        public virtual Task WaitForReady()
            => DisplayPreparePrompt();
            
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetupARView();
            SetupDebugOverlay();
            SetupDebugHandlers();
            
            SCNView.Scene = GetInitialScene();
            SetDelegates();
        }
        
        public void SetupARView()
        {
            SCNView = new ARSCNView { TranslatesAutoresizingMaskIntoConstraints = false };
            View.AddSubview(SCNView);
            View.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(SCNView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(SCNView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, 0),
                NSLayoutConstraint.Create(SCNView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(SCNView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0),
            });
        }

        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            await WaitForReady();
            ToggleDebugOptions();

            SCNView.Session.Run(GetARConfiguration());
            OnSessionBegan();
        }

        #region Delegates
        public void SetDelegates()
        {
            SCNView.Delegate = new InlineARSCNViewDelegate
            {
                OnUpdate = OnSceneUpdate,
                OnNodeAddedForAnchor = OnNodeAddedForAnchor,
                OnNodeUpdatedForAnchor = OnNodeUpdatedForAnchor,
                OnNodeRemovedForAnchor = OnNodeRemovedForAnchor,
            };
            
            SCNView.Session.Delegate = new MainThreadARSessionDelegate
            {
                OnFrameUpdate = OnFrameUpdate,
                OnCameraDidChangeTrackingState = OnCameraDidChangeTrackingState
            };
        }
        
        public virtual void OnFrameUpdate(ARSession session, ARFrame frame)
        {
            UpdatePositionDisplay(frame);
        }

        public virtual void OnCameraDidChangeTrackingState(ARSession session, ARCamera camera)
        {
            UpdateTrackingStateDisplay(camera);
        }
        
        public virtual void OnSceneUpdate(ISCNSceneRenderer renderer, double dt)
        {

        }
        
        public virtual void OnNodeAddedForAnchor(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {

        }

        public virtual void OnNodeUpdatedForAnchor(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {

        }
        
        public virtual void OnNodeRemovedForAnchor(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            
        }
        #endregion

        private void UpdatePositionDisplay(ARFrame frame)
        {
            if (frame?.Camera == null)
                return;
                
            var t = frame.Camera.Transform;
            PositionLabel.Text = $"{t.Column3.X:N2}, {t.Column3.Y:N2}, {t.Column3.Z:N2}";
        }

        private void UpdateTrackingStateDisplay(ARCamera camera)
        {
            if (!_trackingColors.TryGetValue(camera.TrackingState, out var color))
                color = UIColor.Gray;

            UIView.Animate(.2, () => TrackingStatusIndicator.BackgroundColor = color);
        }
        
        private void ToggleDebugOptions()
        {
            var enable = SCNView.DebugOptions == SCNDebugOptions.None;
            SCNView.DebugOptions =
                enable
                ? GetDebugOptions()
                : SCNDebugOptions.None;

            UIView.Animate(.2, () => { 
            TrackingStatusIndicator.Layer.ShadowColor = UIColor.White.CGColor;
                TrackingStatusIndicator.Layer.ShadowRadius = enable ? 15 : 0;
                TrackingStatusIndicator.Layer.ShadowOpacity = enable ? .5f : 0f;
                TrackingStatusIndicator.Layer.ShadowOffset = CGSize.Empty;
            });  
        }

        #region View Setup
        
        const int TrackingStatusSize = 30;
        const float MiniFont = 24f;
        const float MaxiFont = 64f;
        const float MiniRadius = 14f;
        const float MaxiRadius = 4f;
        private bool _positionMaximised;
        
        private void SetupDebugOverlay()
        {
            PositionView = new UIView { BackgroundColor = UIColor.Black.ColorWithAlpha(.85f), TranslatesAutoresizingMaskIntoConstraints = false, };
            PositionLabel = new UILabel { Font = UIFont.FromName("Apple-Kid", MiniFont), AdjustsFontSizeToFitWidth = true, TextAlignment = UITextAlignment.Center, Text = "Smaaaaash", TextColor = UIColor.White, TranslatesAutoresizingMaskIntoConstraints = false };
            PositionView.Layer.CornerRadius = MiniRadius;
            PositionView.AddSubview(PositionLabel);
            
            DoneButton = new UIImageView { UserInteractionEnabled= true, TranslatesAutoresizingMaskIntoConstraints = false };
            DoneButton.Image = UIImage.FromBundle("sprites/sprite110.gif"); 
            
            TrackingStatusIndicator = new UIView { BackgroundColor = UIColor.Gray, TranslatesAutoresizingMaskIntoConstraints = false };
            TrackingStatusIndicator.Layer.CornerRadius = TrackingStatusSize / 2;

            View.AddSubviews(PositionView, TrackingStatusIndicator, DoneButton);
            View.AddConstraints(new[]
            {                
                NSLayoutConstraint.Create(PositionLabel, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, PositionView, NSLayoutAttribute.CenterY, 1, -10),
                NSLayoutConstraint.Create(PositionLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, PositionView, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(PositionLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, PositionView, NSLayoutAttribute.Leading, 1, -10),
                NSLayoutConstraint.Create(PositionLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, PositionView, NSLayoutAttribute.Trailing, 1, 10),

                NSLayoutConstraint.Create(TrackingStatusIndicator, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, -20),
                NSLayoutConstraint.Create(TrackingStatusIndicator, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, -20),
                NSLayoutConstraint.Create(TrackingStatusIndicator, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, TrackingStatusSize),
                NSLayoutConstraint.Create(TrackingStatusIndicator, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, TrackingStatusSize),
                
                NSLayoutConstraint.Create(DoneButton, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, -20),
                NSLayoutConstraint.Create(DoneButton, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 40),
            });

            View.AddConstraints(PositionViewMiniConstraints);
        }
        

        private NSLayoutConstraint[] _positionViewMinimisedConstraints;
        public NSLayoutConstraint[] PositionViewMiniConstraints
        {
            get
            {
                if (_positionViewMinimisedConstraints == null)
                    _positionViewMinimisedConstraints = new[] {
                        NSLayoutConstraint.Create(PositionView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 8),
                        NSLayoutConstraint.Create(PositionView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                        NSLayoutConstraint.Create(PositionView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 50),
                        NSLayoutConstraint.Create(PositionView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, 200),
                    };

                return _positionViewMinimisedConstraints;
            }
        }

        private NSLayoutConstraint[] _positionViewMaximisedConstraints;
        public NSLayoutConstraint[] PositionViewMaximisedConstraints  
        {
            get
            {
                if (_positionViewMaximisedConstraints == null)
                    _positionViewMaximisedConstraints = new[] {
                        NSLayoutConstraint.Create(PositionView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1, 0),
                        NSLayoutConstraint.Create(PositionView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                        NSLayoutConstraint.Create(PositionView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, 330),
                        NSLayoutConstraint.Create(PositionView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, 110),
                    };

                return _positionViewMaximisedConstraints;
            }
        }
        
        public override bool ShouldAutorotate() => true;
        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations() => UIInterfaceOrientationMask.AllButUpsideDown;
        #endregion

        #region Debug Setup
        private void SetupDebugHandlers()
        {
            PositionView.AddGestureRecognizer(new UITapGestureRecognizer(gr =>
            {
                var toRemove = _positionMaximised ? PositionViewMaximisedConstraints : PositionViewMiniConstraints;
                var toAdd = _positionMaximised ? PositionViewMiniConstraints : PositionViewMaximisedConstraints;
                var fontSize = _positionMaximised ? MiniFont : MaxiFont;
                var radius = _positionMaximised ? MiniRadius : MaxiRadius;

                UIView.AnimateNotify(.45, 0, .9f, 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.AllowUserInteraction, () =>
                {
                    PositionView.Layer.CornerRadius = radius;
                    View.RemoveConstraints(toRemove);
                    View.AddConstraints(toAdd);
                    View.LayoutIfNeeded();
                }, null);

                UIView.Animate(.05,
                    () => PositionLabel.Alpha = 0,
                    () =>
                    {
                        PositionLabel.Font = PositionLabel.Font.WithSize(fontSize);
                        UIView.Animate(.5, .1, UIViewAnimationOptions.CurveEaseOut, () => PositionLabel.Alpha = 1, null);
                    });

                _positionMaximised = !_positionMaximised;
            }));

            TrackingStatusIndicator.AddGestureRecognizer(new UITapGestureRecognizer(gr => ToggleDebugOptions()));
            DoneButton.AddGestureRecognizer(new UITapGestureRecognizer(gr =>
            {
                DismissViewController(true, null);
                SoundManager.PlaySound("doorclose"); 
            }));
        }
        
        private async Task DisplayPreparePrompt()
        {
            var text = new EBTextDialogViewController(36);
            text.View.TranslatesAutoresizingMaskIntoConstraints = false;
            text.AddToView(View,
                NSLayoutConstraint.Create(text.View, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1, -20),
                NSLayoutConstraint.Create(text.View, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(text.View, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 40)
            );

            var offScreen = CGAffineTransform.MakeTranslation(0, View.Frame.Height);
            var onScreen = CGAffineTransform.MakeIdentity();

            text.View.Transform = offScreen;
            await UIView.AnimateAsync(.15, () => text.View.Transform = onScreen);
            await text.Label.SetTextAnimated(GetPrepareText());
            await _pendingTouchCompleter.Task;
            await UIView.AnimateAsync(.15, () => text.View.Transform = offScreen);

            text.View.RemoveFromSuperview();
        }
        
        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            _pendingTouchCompleter.TrySetResult(true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            
            try { 
                SCNView.Session.Pause();
                SCNView.Session.Dispose();
             } catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Guess that wasn't such a good idea after all.");
            }
        }

        private Dictionary<ARTrackingState, UIColor> _trackingColors = new Dictionary<ARTrackingState, UIColor>
        {
            [ARTrackingState.Normal] = UIColor.FromRGB(5,144,51),
            [ARTrackingState.Limited] = UIColor.Orange,
            [ARTrackingState.NotAvailable] = UIColor.Red
        };
        #endregion
    }
}
