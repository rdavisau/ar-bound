namespace ARKitMeetup.Helpers
{
    public class EBTextDialogViewController : EBDialogViewController
    {
        public AppendingLabel Label { get; set; }
            
        public EBTextDialogViewController(float fontSize)
        {
            Label = new AppendingLabel(36); 
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetContent(Label);
        }
    }
}