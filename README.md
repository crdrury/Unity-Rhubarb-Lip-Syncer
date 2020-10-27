# Unity Rhubarb Lip Syncer

This is a Unity Editor script for automating lip sync animations using [Rhubarb Lip Sync](https://github.com/DanielSWolf/rhubarb-lip-sync). Currently it is only functional on Windows.

## Prerequisites

Much of the process is automated, but there are some things that must be done manually first.

1. Install [Rhubarb Lip Sync](https://github.com/DanielSWolf/rhubarb-lip-sync), and [add rhubarb.exe to your PATH variable.](https://helpdeskgeek.com/windows-10/add-windows-path-environment-variable/)

2. Download the Unity Rhubarb Lip Syncer and copy LipSyncer.cs to your Scripts folder, and LipSyncerEditor.cs to your Editor folder. Both folders should be in the Assets folder for your project.

3. Create BlendShapes for each mouth shape listed in the Rhubarb README. In Blender these can be [created as "Shape Keys"](https://docs.blender.org/manual/en/latest/animation/shape_keys/introduction.html). Note that you will need to add Shape Keys to each object that will be animated for mouth shapes (face, teeth, tongue, facial hair, etc.)

I like to start by posing the face using armature bones. Then I can select each object in the Modifiers view, click Copy on the armature modifier, then Apply As Shape Key on the copy and rename the key in the Object Data Properties view. Once all objects have a shape key for the current mouth shape, clear the pose and start again on the next mouth shape. When you are done, each object involved in mouth shapes should have shape keys called **X** (no mouth shape), **A**, **B**, **C**, **D**, **E**, **F**, **G**, and **H**.

If you are adding blinking to the animation, all objects in the blink (face, eyelashes, etc.) need a shape key called **Blink**.

If you are adding eyebrow movements, all objects involved will need shape keys call **Eyebrows Raised**, **Eyebrows Angry**, and **Eyebrows Sad**.