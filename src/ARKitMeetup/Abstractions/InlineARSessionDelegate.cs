using System;
using ARKit;

namespace ARKitMeetup.Abstractions
{
    public class InlineARSessionDelegate : ARSessionDelegate
    {
        public Action<ARSession, ARFrame> OnFrameUpdate { get; set; }
        public Action<ARSession, ARCamera> OnCameraDidChangeTrackingState { get; set; }

        public override void DidUpdateFrame(ARSession session, ARFrame frame)
        {
            using (frame)
            {
                OnFrameUpdate?.Invoke(session, frame);
            }
        }

        public override void CameraDidChangeTrackingState(ARSession session, ARCamera camera)
            => OnCameraDidChangeTrackingState?.Invoke(session, camera);

        public override void WasInterrupted(ARSession session)
        {
            // pause session
            session.Pause();

            // specify that existing tracking should be discarded
            var opts = ARSessionRunOptions.RemoveExistingAnchors
                | ARSessionRunOptions.ResetTracking;

            // restart session
            session.Run(session.Configuration, opts);
        }
    }
}
