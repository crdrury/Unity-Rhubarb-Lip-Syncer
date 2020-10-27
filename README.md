# Unity Rhubarb Lip Syncer

This is a Unity Editor script for automating lip sync animations using [Rhubarb Lip Sync](https://github.com/DanielSWolf/rhubarb-lip-sync). Currently it is only fully functional on Windows. Theoretically Mac and Linux should be usable with a few extra steps, but this is currently untested. Steps to do so are at the bottom of this page. The following prerequisites are still required for this approach.

## Prerequisites

Much of the process is automated, but there are some things that must be done manually first.

1. Install [Rhubarb Lip Sync](https://github.com/DanielSWolf/rhubarb-lip-sync), and [add rhubarb.exe to your PATH variable.](https://helpdeskgeek.com/windows-10/add-windows-path-environment-variable/)

2. Download the Unity Rhubarb Lip Syncer and copy LipSyncer.cs to your Scripts folder, and LipSyncerEditor.cs to your Editor folder. Both folders should be in the Assets folder for your project.

3. Create BlendShapes for each mouth shape listed in the Rhubarb README. In Blender these can be [created as "Shape Keys"](https://docs.blender.org/manual/en/latest/animation/shape_keys/introduction.html). Note that you will need to add Shape Keys to each object that will be animated for mouth shapes (face, teeth, tongue, facial hair, etc.)

    I like to start by posing the face using armature bones. Then I can select each object in the Modifiers view, click Copy on the armature modifier, then Apply As Shape Key on the copy and rename the key in the Object Data Properties view. Once all objects have a shape key for the current mouth shape, clear the pose and start again on the next mouth shape. When you are done, each object involved in mouth shapes should have shape keys called **X** (no mouth shape), **A**, **B**, **C**, **D**, **E**, **F**, **G**, and **H**. It is important that **X** is first in the list of shapes, so it will be used as the basis shape. If it is not first in the list, it can be selected and moved to the top of the list with the arrow to the right of the Shape Keys panel.

    If you are adding blinking to the animation, all objects in the blink (face, eyelashes, etc.) need a shape key called **Blink**.

    If you are adding eyebrow movements, all objects involved will need shape keys call **Eyebrows Raised**, **Eyebrows Sad**, and **Eyebrows Angry**.

    ![Blender Shape Keys](https://user-images.githubusercontent.com/39220609/97245481-80b6cf00-17d1-11eb-9e58-589626d5dcdb.png)

    These Shape Keys will be imported to Unity as BlendShapes with the same names. Note that **X** is not listed, since it is the basis shape, i.e. the shape when all BlendShapes are set to 0.

    ![Unity BlendShapes](https://user-images.githubusercontent.com/39220609/97245311-261d7300-17d1-11eb-8c40-9f2c90f36f10.png)


## Using the GUI

Now you can add the Lip Syncer script to any game object in your scene. Set your object references and variables (detailed below).

![Lip Syncer Editor GUI](https://user-images.githubusercontent.com/39220609/97245398-57963e80-17d1-11eb-8f12-5d563991add7.png)

* **Parent Object-** Animated objects must be this object, or children of this object. The animation should ultimately be applied to this object.
* **Mouth Objects-** References to all game objects that have mouth shape keys for this animation.

* **Source Audio-** Audio file containing the dialog to be lip synced.
* **Source Audio Script (Optional)-** A text transcription of the dialog in **Source Audio**. If included, the Rhubarb analysis will give more accurate results.

* **Animate Blinks-** Whether or not to add slightly randomized blinks to the animation.
* **Blink Objects-** References to all game objects that have blink shape keys for this animation.
* **Blink Min-** The minimum amount of time between blinks, in seconds.
* **Blink Max-** The maximum amount of time between blinks, in seconds.
* **Blink Length-** The duration of a blink, in seconds.

* **Animate Eyebrows-** Whether or not to add randomized eyebrow movements to the animation.
* **Eyebrow Objects-** References to all game objects that have eyebrow shape keys for this animation.

* **Animation Name-** What to name the resulting animation. Saved to Assets/Animations/Resources/.
* **Phenome List-** A text file with a list of mouth shapes and timestamps. This will be generated and referenced by the Rhubarb analysis.

**Analyze Audio And Generate Animation-** Opens a command line window and runs Rhubarb to generate a phenome list. When Rhubarb is done running, create and save the animation using the phenome list.

**Generate Animation From Text-** If Rhubarb has already generated a phenome list, you can skip re-analyzing the audio and only re-do the animation. For instance, you may want to adjust your blink times and re-animate. The animation portion runs much quicker than the audio analysis portion, so analysis can be skipped if it is unnecessary.


## Steps For Partial Functionality on Mac/Linux

The only part of the script that should be Window-specific is the command-line execution of Rhubarb. Theoretically it should be possible to manually run the analysis and provide the phenome list to the animation script.

1. Download and install [Rhubarb Lip Sync](https://github.com/DanielSWolf/rhubarb-lip-sync).

2. In the command line, navigate to your audio file and run Rhubarb something like this:

> rhubarb -o "**phenomeList**" "**sourceAudio**" [-d "**sourceAudioScript**"]

**phenomeList-** filename for the phenome list to be saved to.
**sourceAudio-** filename of the dialog audio file.
**sourceAudioScript-** optional text transcription of the audio file for more accurate results. If omitted, omit the -d as well.

eg.

> rhubarb -o "Test Animation Mouth Shapes.txt" "testdialog.wav"

or

> rhubarb -o "Test Animation Mouth Shapes.txt" "testdialog.wav" -d "testdialogscript.txt"

Once Rhubarb has finished running, the resulting phenome list file can be passed to the Phenome List field in the Lip Syncer script and the **Generate Animation From Text** button may be used.
