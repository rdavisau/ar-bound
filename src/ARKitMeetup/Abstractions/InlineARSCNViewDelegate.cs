using System;
using ARKit;
using SceneKit;

namespace ARKitMeetup.Abstractions
{
    public class InlineARSCNViewDelegate : ARSCNViewDelegate
    {
        public Action<ISCNSceneRenderer, double> OnUpdate { get; set; }
        public Action<ISCNSceneRenderer, SCNNode, ARAnchor> OnNodeAddedForAnchor { get; set; }
        public Action<ISCNSceneRenderer, SCNNode, ARAnchor> OnNodeUpdatedForAnchor { get; set; }
        public Action<ISCNSceneRenderer, SCNNode, ARAnchor> OnNodeRemovedForAnchor { get; set; }

        public override void Update(ISCNSceneRenderer renderer, double timeInSeconds)
            => OnUpdate?.Invoke(renderer, timeInSeconds);

        public override void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
            => OnNodeAddedForAnchor?.Invoke(renderer, node, anchor);

        public override void DidUpdateNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
            => OnNodeUpdatedForAnchor?.Invoke(renderer, node, anchor);

        public override void DidRemoveNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
            => OnNodeRemovedForAnchor?.Invoke(renderer, node, anchor);
    }
}
