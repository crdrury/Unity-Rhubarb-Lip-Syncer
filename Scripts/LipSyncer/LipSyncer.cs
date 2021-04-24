using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public abstract class LipSyncer : MonoBehaviour
{
    [Tooltip("Parent of all animated objects")]
    public GameObject parentObject;
    [Tooltip("Audio file for this animation in .WAV or .OGG format ONLY!")]
    public AudioClip sourceAudio;
    [Tooltip("(Optional) text file transcript of the audio file for better quality lip syncing")]
    public TextAsset sourceAudioScript;
    [Tooltip("Rhubarb analysis result file. This will initially be empy and will be filled once analysis is complete")]
    public TextAsset phonemeList;
    [Tooltip("Animation will be saved to " + LipSyncConstants.AnimationDirectory + "[Animation Name].anim")]
    public string animationName;

    protected bool error;

    // Start is called before the first frame update
    public void Start()
    {
        phonemeList = Resources.Load(sourceAudio.name + " - Mouth Shapes") as TextAsset;
    }

    public void ThrowError(string errorText)
    {
        UnityEngine.Debug.LogError(errorText);
        error = true;
    }

    public virtual bool CheckForErrors(int operation)
    {
        error = false;

        if (parentObject == null)
            ThrowError("Parent Object is required!");
        if (operation == 0 && sourceAudio == null)
            ThrowError("Source Audio is required!");
        if (animationName == null || animationName.Trim().Equals(""))
            ThrowError("Animation Name is required!");
        if (operation == 1 && phonemeList == null)
            ThrowError("Source Text is required!");

        return error;
    }

    public void RhubarbAnalysis()
    {
        if (CheckForErrors(0))
            return;

        phonemeList = null;
        string dataPath = Application.dataPath;
        string audioPath = AssetDatabase.GetAssetPath(sourceAudio);
        string outputAssetNameAddition = " - Mouth Shapes";
        string outputFile = audioPath.Substring(0, audioPath.LastIndexOf(".")) + outputAssetNameAddition + ".txt";
        string args = "/C cd ../ && echo \"Generating phoneme list from audio. Window will close when finished.\" && rhubarb -o \"" + outputFile + "\" \"" + audioPath + "\"";
        if (sourceAudioScript != null)
            args += " -d \"" + AssetDatabase.GetAssetPath(sourceAudioScript) + "\"";

        print(args);

        ProcessStartInfo info = new ProcessStartInfo()
        {
            WorkingDirectory = dataPath,
            FileName = "cmd.exe",
            Arguments = args,
            WindowStyle = ProcessWindowStyle.Normal
        };
        Process p = Process.Start(info);

        p.WaitForExit();
        
        StartCoroutine(FindPhonemeFile(sourceAudio.name + outputAssetNameAddition));
    }

    IEnumerator FindPhonemeFile(string filename)
    {
        print("HIST");

        while (phonemeList == null)
        {
            phonemeList = Resources.Load(filename) as TextAsset;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        GenerateAnimation();
    }

    public abstract void GenerateAnimation();

    public string RelativePath(GameObject o)
    {
        string relativePath = o.name;
        Transform levelUp = o.transform;
        while (levelUp = levelUp.parent)
        {
            if (levelUp == null)
            {
                UnityEngine.Debug.LogError("All animated objects must be children of the parent object!");
            }
            else
            {
                if (levelUp != parentObject.transform)
                {
                    relativePath = levelUp.name + "/" + relativePath;
                }
            }
        }

        return relativePath;
    }
}