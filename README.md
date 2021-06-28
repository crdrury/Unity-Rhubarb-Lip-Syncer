# Unity Rhubarb Lip Syncer

This is a Unity Editor script for automating lip sync animations using [Rhubarb Lip Sync](https://github.com/DanielSWolf/rhubarb-lip-sync). Currently it is only fully functional on Windows. Theoretically Mac and Linux should be usable with a few extra steps, but this is currently untested. Steps to do so are at the bottom of this page. The following prerequisites are still required for this approach.

## Prerequisites

Much of the process is automated, but there are some things that must be done manually first.

1. Install [Rhubarb Lip Sync](https://github.com/DanielSWolf/rhubarb-lip-sync), and [add rhubarb.exe to your PATH variable.](https://helpdeskgeek.com/windows-10/add-windows-path-environment-variable/)

2. Download the Unity Rhubarb Lip Syncer from here on Github. Copy the Animations, Editor, and Scripts folders into your Assets folder.

3. Place your audio files inside of a folder called Resources. Usually this will be something like Assets/Audio/Resources/.

4. I highly recommend using text transcriptions to help the Rhubarb analysis. To do this, save a text file that contains a typed up version of the dialogue in the audio file, and save it anywhere in your Assets folder.

# 3D Workflow

[![Unity Rhubarb Lip Syncer 3D Demo](https://img.youtube.com/vi/lFZlJYirG1Y/0.jpg)](https://www.youtube.com/watch?v=lFZlJYirG1Y)

*3D Lip Sync Walkthrough Video on YouTube*

## Setting up for 3D Animations

I'll be describing the setup using Blender. The actual process may be different in other 3D modeling software, but the the goal is to create Blend Shapes, which are supporting in most modeling software.

1. Create BlendShapes for each mouth shape listed in the [Rhubarb README](https://github.com/DanielSWolf/rhubarb-lip-sync#user-content-mouth-shapes). In Blender these can be [created as "Shape Keys"](https://docs.blender.org/manual/en/latest/animation/shape_keys/introduction.html). Note that you will need to add Shape Keys to each object that will be animated for mouth shapes (face, teeth, tongue, facial hair, etc.)

    I like to start by posing the face using armature bones. Then I can select each object in the Modifiers view, click Copy on the armature modifier, then Apply As Shape Key on the copy and rename the key in the Object Data Properties view. **Note- in newer versions of Blender there is a "Save as Shape Key" option in the armature modifier, which saves a few clicks.** Once all objects have a shape key for the current mouth shape, clear the pose and start again on the next mouth shape. When you are done, each object involved in mouth shapes should have shape keys called **X** (no mouth shape), **A**, **B**, **C**, **D**, **E**, **F**, **G**, and **H**. It is important that **X** is first in the list of shapes, so it will be used as the basis shape. If it is not first in the list, it can be selected and moved to the top of the list with the arrow to the right of the Shape Keys panel.

    If you are adding blinking to the animation, all objects in the blink (face, eyelashes, etc.) need a shape key called **Blink**.

    If you are adding eyebrow movements, all objects involved will need shape keys call **Eyebrows Raised**, **Eyebrows Sad**, and **Eyebrows Angry**.

    ![Blender Shape Keys](https://user-images.githubusercontent.com/39220609/97245481-80b6cf00-17d1-11eb-9e58-589626d5dcdb.png)

    These Shape Keys will be imported to Unity as BlendShapes with the same names. Note that **X** is not listed, since it is the basis shape, i.e. the shape when all BlendShapes are set to 0.

    ![Unity BlendShapes](https://user-images.githubusercontent.com/39220609/97245311-261d7300-17d1-11eb-8c40-9f2c90f36f10.png)
    
## 3D Lip Sync In Unity

Now you can add the LipSyncer3D Script to any game object in your scene. Set your object references and variables (detailed below).

![LipSyncer3D Editor GUI](https://user-images.githubusercontent.com/39220609/123710122-2e382180-d83c-11eb-95d8-ea3f177bab55.png)

* **Parent Object-** Animated objects must be this object, or children of this object. The animation should ultimately be applied to this object.
* **Source Audio-** Audio file containing the dialog to be lip synced.
* **Source Audio Script (Optional)-** A text transcription of the dialog in **Source Audio**. If included, the Rhubarb analysis will give more accurate results.

* **Extended Mouth Shapes-** Rhubarb allows you to exclude G, H, and X mouth shapes, so you can disable them here if not using them.
* **Phonetic-** By default (unchecked), Rhubarb uses PocketSphinx for the analysis. PocketSphinx will give ther best results, but only for English speech, so non-English audio will require the phonetic analyzer.

* **Mouth Objects-** References to all game objects that have mouth shape keys for this animation.
* **Mouth Intensity-** 1-100 how much each mouth shape will be blended into the animation. Raise or lower this if the mouth movements are too exaggerated or too subtle.

* **Animate Blinks-** Whether or not to add slightly randomized blinks to the animation.
* **Blink Objects-** References to all game objects that have blink shape keys for this animation.
* **Blink Min-** The minimum amount of time between blinks, in seconds.
* **Blink Max-** The maximum amount of time between blinks, in seconds.
* **Blink Length-** The duration of a blink, in seconds.
* **Blink Intensity-** 1-100 how much each blink will be blended into the animation. Raise or lower this if the blinks are too exaggerated or too subtle.

* **Animate Eyebrows-** Whether or not to add randomized eyebrow movements to the animation.
* **Eyebrow Objects-** References to all game objects that have eyebrow shape keys for this animation.
* **Eyebrow Intensity Min/Max-** 1-100 how much each eyebrow movement will be blended into the animation. Raise or lower this if the eyebrow movements are too exaggerated or too subtle.

* **Animation Name-** What to name the resulting animation. Saved to Assets/Animations/Resources/.
* **Phoneme List-** A text file with a list of mouth shapes and timestamps. This will be generated and referenced by the Rhubarb analysis.

**Analyze Audio And Generate Animation-** Opens a command line window and runs Rhubarb to generate a phoneme list. When Rhubarb is done running, create and save the animation using the phoneme list.

**Generate Animation From Text-** If Rhubarb has already generated a phoneme list, you can skip re-analyzing the audio and only re-do the animation. For instance, you may want to adjust your blink times and re-animate. The animation portion runs much quicker than the audio analysis portion, so analysis can be skipped if it is unnecessary.

# 2D Workflow

[![Unity Rhubarb Lip Syncer 2D Demo](https://img.youtube.com/vi/V2-M-fjaJcs/0.jpg)](https://www.youtube.com/watch?v=V2-M-fjaJcs)

*2D Lip Sync Walkthrough Video on YouTube*

## Setting up for 2D Animations

1. Draw or download image sprites for each mouth shape listed in the [Rhubarb README](https://github.com/DanielSWolf/rhubarb-lip-sync#user-content-mouth-shapes).

2. Save these sprites inside of a folder called Resources; typically something like Assets/Sprites/Resources/. Name each sprite ending with the mouth shape letter, with an optional prefix for different sets of mouths. For example, you might have character1_A.png, character1_B.png, etc. and character2_A.png, character2_B.png, etc.

## 2D Lip Sync In Unity

![2D Sprite setup in Unity](https://user-images.githubusercontent.com/39220609/115964522-e4a52080-a4f2-11eb-9caa-158393d0412b.png)

1. Add your character's head sprite to the scene with that character's X mouth sprite as a child. Make sure that the Order In Layer value for the mouth sprite is higher than the face sprite.

2. Add the LipSyncer2D script to the face object and set object references and variables (detailed below).

![LipSyncer2D Editor GUI](https://user-images.githubusercontent.com/39220609/115964266-9e9b8d00-a4f1-11eb-8fe6-6949919529b9.png)

* **Parent Object-** Animated objects must be this object, or children of this object. The animation should ultimately be applied to this object.
* **Source Audio-** Audio file containing the dialog to be lip synced.
* **Source Audio Script (Optional)-** A text transcription of the dialog in **Source Audio**. If included, the Rhubarb analysis will give more accurate results.

* **Mouth Renderer-** The SpriteRenderer for the mouth object.

* **Animation Name-** What to name the resulting animation. Saved to Assets/Animations/Resources/.
* **Phoneme List-** A text file with a list of mouth shapes and timestamps. This will be generated and referenced by the Rhubarb analysis.

**Analyze Audio And Generate Animation-** Opens a command line window and runs Rhubarb to generate a phoneme list. When Rhubarb is done running, create and save the animation using the phoneme list.

**Generate Animation From Text-** If Rhubarb has already generated a phoneme list, you can skip re-analyzing the audio and only re-do the animation. For instance, you may want to adjust your blink times and re-animate. The animation portion runs much quicker than the audio analysis portion, so analysis can be skipped if it is unnecessary.

# Steps For Partial Functionality on Mac/Linux

The only part of the script that should be Window-specific is the command-line execution of Rhubarb. Theoretically it should be possible to manually run the analysis and provide the phoneme list to the animation script.

1. Download and install [Rhubarb Lip Sync](https://github.com/DanielSWolf/rhubarb-lip-sync).

2. In the command line, navigate to your audio file and run Rhubarb something like this:

> rhubarb -o "**phonemeList**" "**sourceAudio**" [-d "**sourceAudioScript**"]

**phonemeList-** filename for the phoneme list to be saved to.
**sourceAudio-** filename of the dialog audio file.
**sourceAudioScript-** optional text transcription of the audio file for more accurate results. If omitted, omit the -d as well.

eg.

> rhubarb -o "Test Animation Mouth Shapes.txt" "testdialog.wav"

or

> rhubarb -o "Test Animation Mouth Shapes.txt" "testdialog.wav" -d "testdialogscript.txt"

Once Rhubarb has finished running, the resulting phoneme list file can be passed to the Phoneme List field in the LipSyncer2D or LipSyncer3D scripts and the **Generate Animation From Text** button may be used.
