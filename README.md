# VR Flatscreen Prototype... Water-Pistols in Spaaaace!!!

A Unity 6 flatscreen first-person prototype set in a moon-base / space environment.

## Demo

[![Watch the gameplay demo](docs/demo_thumbnail.png)](https://github.com/tobyStone/VR_Flatscreen_Prototype/raw/main/docs/demo.mp4)

[Download / watch the gameplay demo](https://github.com/tobyStone/VR_Flatscreen_Prototype/raw/main/docs/demo.mp4)

## About

This project experiments with a flatscreen player controller, a moon-base environment, and FPS-style interaction inside a space-themed Unity scene.

## Built With

- Unity 6
- C#
- GitHub LFS

## Cloning the Repository

This project uses **Git LFS** for larger files, including the gameplay video and some Unity assets.

Before cloning the project, make sure you have the following installed:

- Git
- Git LFS
- Unity Hub
- Unity 6

### 1. Install Git LFS

Open a terminal or Git Bash window and run:

    git lfs install

### 2. Clone the Repository

    git clone https://github.com/tobyStone/VR_Flatscreen_Prototype.git

### 3. Enter the Project Folder

    cd VR_Flatscreen_Prototype

### 4. Pull the Git LFS Files

    git lfs pull

### 5. Open the Project in Unity

1. Open **Unity Hub**.
2. Click **Add**.
3. Select the cloned `VR_Flatscreen_Prototype` folder.
4. Open the project using **Unity 6**.

The first time the project opens, Unity may take a few minutes to rebuild the `Library` folder and import the project assets.

## Running the Flatscreen Prototype

Once the project has opened in Unity:

1. Open the main scene from the `Assets/Scenes` folder.
2. Press the **Play** button in the Unity Editor.
3. Use the flatscreen first-person controls to move around the moon-base environment.
4. Test the water pistol interaction and gameplay behaviour.

## Extending the Project into a VR Game

This project currently works as a flatscreen first-person prototype. To turn it into a VR-controlled project, the main steps are:

1. Add Unity XR support.
2. Enable OpenXR.
3. Add an XR Origin to the scene.
4. Replace or disable the flatscreen player camera.
5. Attach the water pistol to a VR controller.
6. Replace mouse input with VR controller trigger input.
7. Add VR locomotion.
8. Adapt the UI for VR.
9. Test the project using a VR headset.

## 1. Install the Required XR Packages

In Unity, open:

    Window > Package Manager

Install the following packages from the Unity Registry:

    XR Plugin Management
    OpenXR Plugin
    XR Interaction Toolkit
    Input System

If Unity asks whether to enable the new Input System, accept the change and restart the editor.

## 2. Enable OpenXR

Open Unity Project Settings:

    Edit > Project Settings

Then go to:

    XR Plug-in Management

Enable:

    OpenXR

Then go to:

    XR Plug-in Management > OpenXR

Add the interaction profile for the headset and controllers you want to support.

Examples include:

    Oculus Touch Controller Profile
    Valve Index Controller Profile
    HTC Vive Controller Profile
    Microsoft Motion Controller Profile

For a Meta Quest headset using Oculus-style controllers, start with:

    Oculus Touch Controller Profile

## 3. Add an XR Origin to the Scene

In the Unity scene, add an XR Origin:

    GameObject > XR > XR Origin (VR)

This creates a VR player rig with a headset-controlled camera and controller objects.

A typical hierarchy looks like this:

    XR Origin (VR)
      Camera Offset
        Main Camera
        Left Controller
        Right Controller

The `Main Camera` inside the XR Origin will be controlled by the VR headset.

## 4. Replace the Flatscreen Camera

The existing flatscreen player has its own camera and controller script.

To avoid having two active cameras:

1. Find the existing flatscreen player object in the scene.
2. Disable its camera.
3. Disable or remove any mouse-look script attached to the flatscreen camera.
4. Use the `Main Camera` inside `XR Origin (VR)` as the active camera.

The project should now use the headset to control the player view.

## 5. Position the XR Origin

Move the `XR Origin (VR)` to the starting position of the existing flatscreen player.

A sensible starting setup is:

    XR Origin position: ground level
    Main Camera: controlled by headset
    Player height: controlled by headset tracking

Make sure the headset camera does not start inside the floor, a wall, or another object.

## 6. Add VR Locomotion

VR movement can be added using the XR Interaction Toolkit.

Common options are:

    Teleportation
    Continuous movement
    Snap turning
    Continuous turning

For a first VR version, teleportation and snap turning are usually the most comfortable.

A recommended beginner setup is:

    XR Origin (VR)
      Locomotion System
      Teleportation Provider
      Snap Turn Provider

You can later add smooth movement using:

    Continuous Move Provider
    Continuous Turn Provider

## 7. Attach the Water Pistol to the VR Controller

The water pistol should be attached to the player’s right-hand controller.

Example hierarchy:

    XR Origin (VR)
      Camera Offset
        Right Controller
          Water Pistol

After parenting the water pistol to the right controller:

1. Reset or adjust its local position.
2. Rotate it until it points naturally from the player’s hand.
3. Scale it if needed.
4. Test in Play Mode with the headset connected.

The water pistol should move and rotate with the player’s right-hand controller.

## 8. Create a Barrel Transform

For accurate VR shooting, the water pistol should fire from a specific barrel point.

Inside the water pistol object, create an empty child object:

    Water Pistol
      BarrelPoint

Place `BarrelPoint` at the end of the pistol barrel.

The firing direction should come from:

    BarrelPoint.forward

rather than:

    Camera.forward

This means the pistol fires where the player is actually pointing the controller.

## 9. Replace Mouse Shooting with Controller Trigger Input

The flatscreen prototype may currently fire using mouse input, such as:

    Input.GetMouseButtonDown(0)

For VR, this should be replaced with input from the right-hand controller trigger.

The intended behaviour is:

    Right controller trigger pressed
      Fire water pistol from BarrelPoint

The firing script should use the controller or barrel direction instead of the centre of the screen.

Example firing logic:

    Origin: BarrelPoint.position
    Direction: BarrelPoint.forward

## 10. Add XR Controller Interaction

Depending on how the project develops, the water pistol can either be:

    Fixed to the controller

or:

    A grabbable object

For the first VR version, keeping it fixed to the right controller is simpler.

Later, the pistol could be made grabbable using:

    XR Grab Interactable
    XR Direct Interactor
    XR Ray Interactor

A possible later hierarchy could be:

    XR Origin (VR)
      Camera Offset
        Left Controller
          XR Direct Interactor
        Right Controller
          XR Direct Interactor

    Water Pistol
      XR Grab Interactable
      Rigidbody
      Collider
      BarrelPoint

## 11. Convert the UI for VR

The current UI may be screen-space UI designed for a monitor.

For VR, UI is usually more comfortable when it is placed in world space.

Useful changes include:

1. Convert important UI canvases to `World Space`.
2. Place health, ammo, or status UI in front of the player.
3. Make UI text large enough to read in VR.
4. Avoid placing UI too close to the player’s face.
5. Consider attaching key UI elements to the player rig.

A possible VR UI setup is:

    XR Origin (VR)
      Camera Offset
        Main Camera
          PlayerStatusCanvas

Or, for world-space UI:

    Scene
      VRStatusCanvas

The canvas should be placed comfortably in the scene, not directly stuck to the camera unless needed.

## 12. Update Enemy Targeting and Game Logic

If enemies currently target the flatscreen player object, they may need to target the XR Origin instead.

Check any scripts that reference:

    Player
    Main Camera
    PlayerController

These may need to be updated so enemies, health systems, and gameplay events use the VR player rig.

Possible target objects include:

    XR Origin (VR)
    Main Camera inside XR Origin
    A dedicated PlayerTarget object attached to the XR Origin

A useful structure is:

    XR Origin (VR)
      PlayerTarget
      Camera Offset
        Main Camera

Enemies can then target `PlayerTarget`.

## 13. Test in a VR Headset

Connect the VR headset and press Play in Unity.

Check the following:

- The headset controls the camera.
- The player starts in the correct place.
- The right controller controls the water pistol.
- The water pistol points in the correct direction.
- The trigger fires the water pistol.
- Movement works comfortably.
- Turning works comfortably.
- The UI is readable.
- The enemy and health systems still work.
- The player does not start inside geometry.
- The scale of the environment feels correct.

## 14. Suggested Development Order

A sensible order for converting this prototype into VR is:

1. Install XR Plugin Management, OpenXR, XR Interaction Toolkit, and Input System.
2. Enable OpenXR in Project Settings.
3. Add an `XR Origin (VR)` to the scene.
4. Disable the original flatscreen camera.
5. Confirm headset tracking works.
6. Add basic teleportation or snap-turn movement.
7. Attach the water pistol to the right-hand controller.
8. Add a `BarrelPoint` to the pistol.
9. Replace mouse firing with VR trigger input.
10. Fire from the pistol barrel rather than the camera.
11. Convert the UI to world-space or VR-friendly UI.
12. Update enemy targeting to use the XR player.
13. Playtest in the headset.
14. Tune movement speed, turning, weapon position, UI scale, and interaction distance.

## 15. Possible Future VR Improvements

Future VR improvements could include:

- Full VR controller support.
- Pistol aiming using hand tracking or controller tracking.
- Trigger-based water pistol firing.
- Grabbable water pistol using `XR Grab Interactable`.
- Teleport locomotion.
- Smooth locomotion.
- Snap turning.
- VR-friendly health UI.
- VR-friendly ammo UI.
- Spatial audio.
- Haptic feedback when firing.
- Haptic feedback when hit.
- More interactive moon-base objects.
- Improved enemy behaviour.
- Meta Quest support.
- SteamVR/OpenXR support.
- A dedicated VR build of the game.

## Notes

This repository started as a flatscreen Unity prototype. The VR conversion should be treated as an extension of the existing project rather than a complete rewrite.

The safest approach is to keep the current flatscreen version working while building the VR version gradually in a separate scene or branch.

Suggested branch name:

    git checkout -b vr-conversion

This allows the original flatscreen prototype to remain stable while VR features are added and tested.
