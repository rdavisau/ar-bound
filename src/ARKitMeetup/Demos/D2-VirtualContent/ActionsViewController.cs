using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARKit;
using ARKitMeetup.Demos.D1;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using CoreFoundation;
using SceneKit;

namespace ARKitMeetup.Demos.D203
{
    [DisplayInMenu(DisplayName = "Node Actions", DisplayDescription = "Using SCNActions to move virtual content")]
    public class ActionsViewController : BaseARViewController
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        protected bool UpdateBounds = false;
        protected float MinX = -10, MinY = -10f, MinZ = -10;
        protected float MaxX = 10, MaxY = 10, MaxZ = 10;

        public List<SCNNode> Prefabs;
        public SCNNode ShipNode { get; set; }

        public override SCNScene GetInitialScene()
        {
            Prefabs = SCNScene.FromFile("art.scnassets/basic-prefab.scn")
                .RootNode
                .ChildNodes
                .Where(x => x.Name.StartsWith("char"))
                .ToList();

            foreach (var pf in Prefabs)
                pf.RemoveFromParentNode();

            var shipScene = SCNScene.FromFile("art.scnassets/ship");
            ShipNode = shipScene.RootNode.FindChildNode("ship", false);
            ShipNode.RemoveFromParentNode();

            // place the eb characters on the ship
            var i = 0;
            foreach (var pf in Prefabs.OrderByDescending(x => x.Name))
            {
                var node = pf.Clone();
                node.Scale = new SCNVector3(1, 1, 0.0001f);
                node.Position = new SCNVector3(0, 1 + (i / 2 * 0.25f), i++);
                ShipNode.AddChildNode(node);
            }

            var scene = base.GetInitialScene();
            scene.RootNode.AddChildNode(ShipNode);

            return scene;
        }

        public override void OnSessionBegan()
        {
            base.OnSessionBegan();

            var duration = 4;
            Task.Run(async () =>
            {
                while (true)
                {
                    DispatchQueue.MainQueue.DispatchAsync(() => MoveShip(duration));
                    await Task.Delay(TimeSpan.FromSeconds(duration));
                }

            }, _cancellationTokenSource.Token);
        }

        public override void OnFrameUpdate(ARSession session, ARFrame frame)
        {
            base.OnFrameUpdate(session, frame);

            var pos = SCNView.PointOfView.Position;

            if (UpdateBounds)
            {
                MinX = Math.Min(MinX, pos.X);
                MinY = Math.Min(MinY, pos.Y);
                MinZ = Math.Min(MinZ, pos.Z);
                MaxX = Math.Max(MaxX, pos.X);
                MaxY = Math.Max(MaxY, pos.Y);
                MaxZ = Math.Max(MaxZ, pos.Z);
            }
        }

        public void MoveShip(int duration)
        {
            var xs = new[] { MinX, MaxX };
            var ys = new[] { MinY, MaxY };
            var zs = new[] { MinZ, MaxZ };

            var pos = ShipNode.Position;
            while (pos == ShipNode.Position)
                pos = new SCNVector3(xs.Random(), ys.Random(), zs.Random());

            var moveAction = SCNAction.MoveTo(pos, duration - 1);
            moveAction.TimingMode = SCNActionTimingMode.EaseInEaseOut;

            var rotateAction = SCNAction.RotateBy((float)Math.PI / 2, new SCNVector3(0, 0, 1), duration / 4.0);

            ShipNode.Look(pos);
            ShipNode.RunAction(moveAction);
            ShipNode.RunAction(rotateAction);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            _cancellationTokenSource.Cancel();
        }
    }
}
