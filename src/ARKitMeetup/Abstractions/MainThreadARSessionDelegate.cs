using ARKit;
using CoreFoundation;

namespace ARKitMeetup.Abstractions
{
    public class MainThreadARSessionDelegate : InlineARSessionDelegate
    {
        public override void DidUpdateFrame(ARSession session, ARFrame frame)
            => DispatchQueue.MainQueue.DispatchAsync(() => base.DidUpdateFrame(session, frame));

        public override void CameraDidChangeTrackingState(ARSession session, ARCamera camera)
            => DispatchQueue.MainQueue.DispatchAsync(() => base.CameraDidChangeTrackingState(session, camera));

    }
}
