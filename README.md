## Playbook: Technical Task

![Demo Capture](https://github.com/justinmajetich/playbook-technical-task/blob/main/screen-capture.png)

This Unity project showcases a custom transformation control gizmo, similar to those featured in many 3D editors. The gizmo consists of gimbal handles for rotation operations, as well as linear drag handles for translation and scaling. The control gizmo can be set to operate in both local or world space.

Additionally, this demo features an object spawning system. Spawnable objects are represented as draggable buttons in a scrollable, 3D menu. The user may drag and drop these buttons into the scene to spawn an object.

## Installation

Download this repository as a ZIP file and extract. Open Unity Hub (the project is built in Unity version 2020.3.42, but should open without issue in other recent versions). Click “Open” in Unity Hub and select the extracted project folder.

## Build Instructions

With the project open, navigate to Assets>Scenes in the Project window and double-click on the scene “DemoScene” to open.

Next, navigate to File>Build Settings. In the Build Settings dialog box, click “Add Open Scenes” to add “DemoScene” to the “Scenes In Build” list. This should be the only scene in this list. If there are additional scenes listed, remove them by right-clicking and selecting “Remove Selection”.

Staying within Build Settings, make sure “PC, Mac & Linux Standalone” is selected under “Platform”, and select your target platform from the “Target Platform” dropdown. 

Click “Build And Run”. You’ll be prompted to select a destination folder for the build. Once this selection is made, Unity will build the application and automatically launch it when the build is complete. If you’d like to build the application without running it immediately, select “Build” instead of “Build And Run”.

## Use **Instructions**

### Object Spawning

When the application loads, there will be no objects spawned in the scene. To spawn an object, drag and drop one of the 3D buttons from the object menu into the scene. Scroll through the menu by clicking the arrows. You can cancel a spawn operation by dropping the 3D button back into place on the menu.

### Transformation Controls

Click on a spawned object to display its transformation control gizmo. Click and drag the conical gizmo handles to translate an object along the given axis. Click and drag on the spherical handles to scale an object along the given axis. Click and drag on the gimbal handles to rotate an object around the given axis.

Press “Space” to toggle the transformation controls between local and world space. World space scaling is not supported; scale handles will be deactivated in world space mode.
