using System;
using System.Linq;
using System.Threading.Tasks;
using ARKitMeetup.Demos.Helpers;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using UIKit;

namespace ARKitMeetup.Demos.D4
{
    public class DisplayMeetupProfileViewController : UIViewController
    {
        public MeetupProfile Profile { get; set; }
        public EBDialogViewController Dialog { get; private set; }

        /// <summary>
        /// For use with Continuous only
        /// </summary>
        public DisplayMeetupProfileViewController()
        {
            int i = 0;
            //var profiles = Task.Run(async () => await StaticMeetupService.GetRsvps()).Result; 
            //Profile = profiles.Skip(i++).First().ItemData;

            View.AddGestureRecognizer(new UITapGestureRecognizer(async gr =>
            {
//                Profile = profiles.Skip(i++).First().ItemData;
  //              DisplayProfile();
                
                var al = new AppendingLabel(36f);
    
                Dialog.SetContent(al);

                foreach (var z in Enumerable.Range(0, 3000))
                {
                    await al.SetTextAnimated(String.Join(" ", Enumerable.Range(0, z + 1).Select(_ => "po")), 0.2 - (z * 0.0015));
                    await Task.Delay(TimeSpan.FromSeconds(.5- (z * 0.0015)));  
                }
            }));              
        }  
         
        public DisplayMeetupProfileViewController(MeetupProfile profile)
        { 
            Profile = profile;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.Clear;

            Dialog = new EBDialogViewController();
            Dialog.AddToView(View,
                NSLayoutConstraint.Create(Dialog.View, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1, 0),
                NSLayoutConstraint.Create(Dialog.View, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),
                NSLayoutConstraint.Create(Dialog.View, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 5)
            );
            
            DisplayProfile();
        }

        private void DisplayProfile()
        {
            Dialog.SetContent(GetContent());
            if (Profile.IsGhost)
                SoundManager.PlaySound("spooky");
        }

        public UIView GetContent()
        {
            var al = new AppendingLabel(36f);

            var bio = Profile.Bio != null
                ? $"\"{Profile.Bio}\""
                : "This user doesn't appear to have a bio.";
            
            var text = String.Join(Environment.NewLine,
                $"{Profile.Name} ({Profile.Status})",
                $"{Profile.City}, {Profile.Country}", 
                $"Meetup Age:      {Math.Round((DateTime.Now - Profile.JoinedOn).TotalDays / 365.0), 2} years", 
                $"RSVPs:                 {Profile.YesRsvps}/{Profile.TotalRsvps} ({(double)Profile.YesRsvps/Profile.TotalRsvps:P2})",
                "",
                bio
                );

            al.SetTextAnimated(text, pace: 0.02);
            return al;
        }
    }
}
