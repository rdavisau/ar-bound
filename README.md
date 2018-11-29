# ar-bound

![](https://ryandavis.io/content/images/2018/11/arbound-low.gif)

Demo app for the 'Introduction to ARKit' talk (https://ryandavis.io/introduction-to-arkit/). It demonstrates a number of the core features in ARKit (to varying degrees of sophistication) using theming and elements from everybody's favourite 90's SNES RPG, [Earthbound](https://en.wikipedia.org/wiki/EarthBound). 

# Demos

Appropriately, the demos are found in the 'Demos' folder, organised by the ARKit feature they cover. These demos use sound quite heavily so it is best to have that enabled. Since these are ARKit demos you will need to run them on a physical device. 

## World Tracking

A single demo that demonstrates the setup and function of a basic ARKit world tracking session, without any extras (_"all R, no A"_). It includes a couple of debugging features, such as the continuous display of position and tracking state, and the ability to enable/disable the display of ARKit debugging information (tap the tracking state indicator to toggle). 

This class is the base class for all other demos, so they get the debugging and setup 'for free', so you can just focus on the incremental code required to use other features. 

You can return to the menu from this and any other scene by tapping the door icon in the top left. 

## Virtual Content

These demos show how to include virtual SceneKit content in an AR session, including:

- loading a SceneKit scene from file (.scn) straight into the AR world
- loading a SceneKit scene and using contents as 'prefabs' to place in the AR world
- using `SCNAction`s to control movement of virtual content (kind of janky because I'm bad a math/ran out of time)
- a broken 'earthbound home designer' that allows you to select earthbound sprites and tap to place them in the virtual world. If you try it you'll find it doesn't work as you expect because we haven't got any awareness of real world features (like walls and floors), so virtual content gets placed behind/below them in 3D space.

## Plane Tracking

These demos show how ARKit plane tracking works, in three stages. Each stage builds slightly on the previous so you can focus on the change in code:

- plane detection: receiving planes and updates from ARKit by placing transparent overlays in the virtual scene. Different sounds play depending on whether ARKit is adding, updating or removing detected planes

- hit testing support: the above + support for continuous (per AR frame) hit testing to determine whether a surface is being pointed at. Tap the 'column' icon in the bottom left to switch to hit testing mode after you have detected some surfaces. 

- content placement support: the above + the ability to add virtual content to the planes by tapping when pointing at a detected surface while in hit testing mode. 

You can toggle showing/hiding the transparent overlays by long-pressing the 'column' icon.

## Image Detection

Demonstrates the image detection feature of ARKit. At the event, attendees held up their Meetup profile picture which was recognised and used to anchor their position in the room and produce a floating version of the avatar. Then, it was possible to point at any of the detected avatars and tap to display a brief summary of their Meetup profile retrieved from the Meetup api. 

These calls have been replaced in the sample with hard-coded data. You can print (or display on screen) the Earthbound character files in the Resources folder (ness, paula, jeff, poo) and they will work instead.

## Face Tracking

Demonstrates some of the face tracking features of ARKit, in several steps:

- detecting a face: plays a spooky sound when a face is detected
- tracking a face: the above + receiving updates to a face as it moves and displaying a peppermint mesh over the face in 3D
- expression detection: the above + display of the top five strongest 'blend shapes' detected by ARKit and very naive/rudimentary detection of some expressions. Try making 'mouth wide open', 'big smile', 'frown' or 'winking' expressions. These were only tested on me and only consider a single blendshape so may not work well for others.

# Extending AR Bound

Besides helping satisfy a long unfulfilled desire to make something Earthbound themed, AR-bound was designed to make it easy to quickly test different ARKit features. If you want to try out different features you can easily add a screen to the menu by creating a view controller that derives from `BaseARViewController` (this gets you the debugging overlays, etc.) and then adding a `DisplayInMenu` attribute to the class to give it a title etc. Just take a look at any of the existing ones if you get stuck. Menu items are ordered by namespace so use that knowledge to put your screens before or after the baked in ones. 

# Known Issues

* the scrolling background on the home screen is hack that lasted long enough for the meetup but the sad truth will be revealed if you leave it running long enough
* the flying ship doesn't know how to face the direction it's flying 🧐
* various other bugs and bits of jank due to my cripplingly poor familiarity with 3D math, vectors, matrices etc. and the fact that this is just a demo app 😎

# Attribution

* 'Apple Kid' font from the [Ultimate Earthbound Font Pack](https://earthboundcentral.com/2009/11/ultimate-earthbound-font-pack/)
* 'Choose a File' bgm, assorted Earthbound sounds and sprites from Starman.net
    * [BGM](http://starmen.net/mother2/music/), [Sounds](http://starmen.net/mother2/soundfx/), [Sprites](http://starmen.net/mother2/images/game/)

Larry O'Brien's posts on ARKit with Xamarin were a good help and so was `ToSCNMatrix4`.

<small>P.S. You can toggle the menu BGM by tapping the 'AR Bound' title.</small>
