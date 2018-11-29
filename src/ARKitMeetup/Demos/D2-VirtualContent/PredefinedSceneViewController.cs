using System;
using ARKitMeetup.Demos.D1;
using ARKitMeetup.Models;
using SceneKit;

namespace ARKitMeetup.Demos.D201
{
    [DisplayInMenu(DisplayName = "Predefined Scene", DisplayDescription = "No code, only .scn")]
    public class PredefinedSceneViewController : BaseARViewController
    {
        public override SCNScene GetInitialScene() 
            => SCNScene.FromFile("art.scnassets/basic-scene.scn");
    }
}
