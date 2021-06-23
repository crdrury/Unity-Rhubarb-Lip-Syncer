using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class LipSyncer3D : LipSyncer
{
    [Tooltip("Array of GameObjects affected by the mouth BlendShapes")]
    public GameObject[] mouthObjects;
    [Tooltip("Array of GameObjects affected by the blink BlendShapes")]
    public GameObject[] blinkObjects;
    [Tooltip("Array of GameObjects affected by the eyebrow BlendShapes")]
    public GameObject[] eyebrowObjects;

    [Tooltip("[0-100] Tweak to make mouth animations more or less intense")]
    public float mouthIntensity = 90f;
    [Tooltip("[0-100] Tweak to make blink animations more or less intense")]
    public float blinkIntensity = 80f;
    [Tooltip("[0-100] Tweak to make eyebrow animations more or less intense")]
    public float eyebrowIntensityMin = 10f;
    [Tooltip("[0-100] Tweak to make eyebrow animations more or less intense")]
    public float eyebrowIntensityMax = 40f;

    [Tooltip("Enable blink animation")]
    public bool animateBlinks = true;
    [Tooltip("Minimum time in seconds between blinks")]
    public float blinkMin;
    [Tooltip("Maximum time in seconds between blinks")]
    public float blinkMax;
    [Tooltip("Length on each blink in seconds")]
    public float blinkLength;
    [Tooltip("Enable eyebrow animation")]
    public bool animateEyebrows = true;
    [Tooltip("Delay the visual animation in seconds")]
    public float AVSync;
    

    List<float>[] mouthList;
    List<Keyframe>[] mouthKeyframe;

    AnimationClip resultAnimation;
    int currentIndex = -1, lastIndex = -1;

    readonly float[,] eyebrowMatrix = new float[,]
    {
        { .6f, .3f, .1f },
        { .6f, .2f, .6f },
        { .1f, .5f, .5f },
        { .2f, .7f, .7f },
        { .4f, .4f, .4f },
        { .6f, .6f, .6f },
        { .6f, .6f, .6f },
        { .2f, .6f, .6f },
        { .6f, .6f, .6f }
    };

    public override bool CheckForErrors(int operation)
    {
        error = base.CheckForErrors(operation);

        if (mouthObjects.Length == 0)
            ThrowError("Animated Object Path is required!");
        if (eyebrowIntensityMax < 0 || eyebrowIntensityMax > 100 || eyebrowIntensityMin < 0 || eyebrowIntensityMin > 100)
            ThrowError("Eyebrow Intensities must be between 0 and 100!");
        if (eyebrowIntensityMax < eyebrowIntensityMin)
            ThrowError("Eyebrow Intensity Max must be greater or equal to Eyebrow Intensity Min!");

        return error;
    }

    public override void GenerateAnimation()
    {
        if (CheckForErrors(1))
            return;

        resultAnimation = new AnimationClip
        {
            //            legacy = true
        };

        mouthKeyframe = new List<Keyframe>[LipSyncConstants.MouthShape.Count];
        for (int i = 0; i < mouthKeyframe.Length; i++)
        {
            mouthKeyframe[i] = new List<Keyframe>
            {
                new Keyframe(0f, 0f)
            };
        }

        string[] mouthLines = phonemeList.text.Split('\n');

        foreach (string s in mouthLines)
        {
            if (!s.Trim().Equals(""))
            {
                string[] param = s.Split('\t');
                currentIndex = LipSyncConstants.MouthShape.FindIndex(param[1].Contains);
                if (!s.Trim().Equals("X"))
                {
                    mouthKeyframe[currentIndex].Add(new Keyframe(float.Parse(param[0])  + AVSync - .125f, 0f));
                    mouthKeyframe[currentIndex].Add(new Keyframe(float.Parse(param[0]) + AVSync, mouthIntensity));
                }
                if (lastIndex != -1)
                {
                    mouthKeyframe[lastIndex].Add(new Keyframe(float.Parse(param[0]) + AVSync, 0f));
                }
                lastIndex = currentIndex;
            }
        }

        AssetDatabase.CreateAsset(resultAnimation, LipSyncConstants.AnimationDirectory + animationName + ".anim");

        AudioSource audioSource = parentObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = (AudioSource)parentObject.AddComponent(typeof(AudioSource));
            audioSource.playOnAwake = false;
        }
        audioSource.clip = sourceAudio;

        AnimatorOverrideController charOverride = Resources.Load(parentObject.name + "_Override") as AnimatorOverrideController;

        Animator anim = parentObject.GetComponent<Animator>();
        if (anim == null)
        {
            anim = (Animator)parentObject.AddComponent(typeof(Animator));
        }

        if (charOverride == null)
        {
            charOverride = new AnimatorOverrideController(Resources.Load("Character_Override") as AnimatorOverrideController);
            charOverride["Face Idle"] = Resources.Load(parentObject + "_Idle") as AnimationClip;

            if (charOverride["Face Idle"] == null)
            {
                charOverride["Face Idle"] = new AnimationClip();
            }
            anim.runtimeAnimatorController = charOverride;

            AssetDatabase.CreateAsset(charOverride, LipSyncConstants.AnimationDirectory + parentObject.name + "_Override.overrideController");
        }

        // Add mouth shape curves
        AnimationCurve curve;
        for (int i = 0; i < mouthKeyframe.Length - 1; i++)
        {
            curve = new AnimationCurve(mouthKeyframe[i].ToArray());
            foreach (GameObject o in mouthObjects)
            {
                resultAnimation.SetCurve(RelativePath(o), typeof(SkinnedMeshRenderer), "blendShape." + LipSyncConstants.MouthShape[i], curve);
            }
        }

        float eyeTimer = 0f;

        // Add blink curve
        if (animateBlinks)
        {
            List<Keyframe> blinkFrames = new List<Keyframe>();
            while (eyeTimer < resultAnimation.length - blinkMax)
            {
                eyeTimer += Random.Range(blinkMin, blinkMax);
                blinkFrames.Add(new Keyframe(eyeTimer - blinkLength / 2, 0));
                blinkFrames.Add(new Keyframe(eyeTimer, blinkIntensity));
                blinkFrames.Add(new Keyframe(eyeTimer + blinkLength / 2, 0));
            }
            curve = new AnimationCurve(blinkFrames.ToArray());
            foreach (GameObject o in blinkObjects)
            {
                resultAnimation.SetCurve(RelativePath(o), typeof(SkinnedMeshRenderer), "blendShape.Blink", curve);
            }
        }

        // Add eyebrow curve
        if (animateEyebrows)
        {
            List<Keyframe>[] eyebrowFrames = new List<Keyframe>[3];
            for (int i = 0; i < eyebrowFrames.Length; i++)
                eyebrowFrames[i] = new List<Keyframe>();
            eyeTimer = 0f;
            lastIndex = -1;
            while (eyeTimer < resultAnimation.length - blinkMax)
            {
                eyeTimer += Random.Range(blinkMin / 1.5f, blinkMax);
                currentIndex = Random.Range(0, LipSyncConstants.EyebrowShape.Count);
                float hold = Random.Range(.2f, .8f);
                float blendValue = Random.Range(eyebrowIntensityMin, eyebrowIntensityMax);
                MaybeAddKeyframe(eyebrowFrames[currentIndex], new Keyframe(eyeTimer - Random.Range(.2f, .5f), 0), .5f);
                MaybeAddKeyframe(eyebrowFrames[currentIndex], new Keyframe(eyeTimer, blendValue), .5f);
                MaybeAddKeyframe(eyebrowFrames[currentIndex], new Keyframe(eyeTimer + hold, blendValue + Random.Range(10, 10)), .5f);
                MaybeAddKeyframe(eyebrowFrames[currentIndex], new Keyframe(eyeTimer + hold + Random.Range(.2f, .5f), 0), .5f);

                lastIndex = currentIndex;
            }
            for (int i = 0; i < eyebrowFrames.Length; i++)
            {
                curve = new AnimationCurve(eyebrowFrames[i].ToArray());
                foreach (GameObject o in eyebrowObjects)
                {
                    resultAnimation.SetCurve(RelativePath(o), typeof(SkinnedMeshRenderer), "blendShape.Eyebrows " + LipSyncConstants.EyebrowShape[i], curve);
                }
            }
        }

        if (parentObject.GetComponent<LipSyncAnimator3D>() == null)
            parentObject.AddComponent<LipSyncAnimator3D>();

        AssetDatabase.SaveAssets();
        
        AssetDatabase.Refresh();

        print("Animation saved to " + LipSyncConstants.AnimationDirectory + animationName + ".anim");
    }

    public void MaybeAddKeyframe(List<Keyframe> frames, Keyframe key, float chance)
    {
        if (Random.Range(0, 1) <= chance)
        {
            frames.Add(key);
        }
    }
}
