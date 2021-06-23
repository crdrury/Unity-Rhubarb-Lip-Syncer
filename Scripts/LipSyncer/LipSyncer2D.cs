using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LipSyncer2D : LipSyncer
{
    [Tooltip("SpriteRenderer for the mouth")]
    public SpriteRenderer mouthRenderer;

    List<float>[] mouthList;
    List<Keyframe> mouthKeyframe;

    AnimationClip resultAnimation;
    
    public override bool CheckForErrors(int operation)
    {
        error = base.CheckForErrors(operation);

        if (mouthRenderer == null)
            ThrowError("Mouth Renderer is required!");

        return error;
    }

    public override void GenerateAnimation()
    {
        if (CheckForErrors(1))
            return;

        AudioSource audioSource = parentObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = (AudioSource)parentObject.AddComponent(typeof(AudioSource));
            audioSource.playOnAwake = false;
        }
        audioSource.clip = sourceAudio;

//        AssetDatabase.CreateAsset(new TextAsset(), LipSyncConstants.AnimationDirectory + animationName);
//        AssetDatabase.SaveAssets();
//        File.WriteAllText(Application.dataPath + LipSyncConstants.AnimationLocalDirectory + animationName, sourceAudio.name + "\n" + phonemeList.text);
//        File.Delete(LipSyncConstants.AnimationDirectory + animationName + ".txt");
//        File.Delete(LipSyncConstants.AnimationDirectory + animationName + ".meta");
//        File.Move(LipSyncConstants.AnimationDirectory + animationName, LipSyncConstants.AnimationDirectory + animationName + ".txt");
        File.Copy(outputFile, LipSyncConstants.AnimationDirectory + animationName + ".txt", true);
//        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (parentObject.GetComponent<LipSyncAnimator2D>() == null)
            parentObject.AddComponent(typeof(LipSyncAnimator2D));

        parentObject.GetComponent<LipSyncAnimator2D>().spriteRenderer = mouthRenderer;
    }
}
