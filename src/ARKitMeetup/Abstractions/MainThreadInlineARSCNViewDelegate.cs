using ARKit;
using CoreFoundation;
using SceneKit;

namespace ARKitMeetup.Abstractions
{
    public class MainThreadInlineARSCNViewDelegate : InlineARSCNViewDelegate
    {
        public override void Update(ISCNSceneRenderer renderer, double timeInSeconds)
            => DispatchQueue.MainQueue.DispatchAsync(() => base.Update(renderer, timeInSeconds));

        public override void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
            => DispatchQueue.MainQueue.DispatchAsync(() => base.DidAddNode(renderer, node, anchor));

        public override void DidUpdateNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
            => DispatchQueue.MainQueue.DispatchAsync(() => base.DidUpdateNode(renderer, node, anchor));

        public override void DidRemoveNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
            => DispatchQueue.MainQueue.DispatchAsync(() => base.DidRemoveNode(renderer, node, anchor));
    }
}
