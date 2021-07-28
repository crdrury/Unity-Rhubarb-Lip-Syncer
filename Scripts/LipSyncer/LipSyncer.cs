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
    [Tooltip("Animation will be saved to " + LipSyncConstants.AnimationLocalDirectory + "[Animation Name].anim")]
    public string animationName;
    [Tooltip("G shape- 'F' or 'V' sound")]
    public bool extendG = true;
    [Tooltip("H shape- 'L' sound")]
    public bool extendH = true;
    [Tooltip("X shape- Neutral resting position")]
    public bool extendX = true;
    [Tooltip("Include all Rhubarb extended mouth shapes")]
    public bool extendedMouthShapeGroup;
    [Tooltip("Use phonetic speech recognizer for non-English speech")]
    public bool phonetic = false;

    protected bool error;
    protected string outputFile;

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
        string path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
        LipSyncConstants.AnimationDirectory = path.Substring(0, path.IndexOf("Scripts") - 1) + LipSyncConstants.AnimationLocalDirectory;

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
        outputFile = audioPath.Substring(0, audioPath.LastIndexOf(".")) + outputAssetNameAddition + ".txt";
        string args = "/C cd ../ && echo This window will close automatically when finished. && rhubarb -o \"" + outputFile + "\"";
        string extend = "\"" + (extendG ? "G" : "") + (extendH ? "H" : "") + (extendX ? "X" : "") + "\"";
        if (!extend.Equals("\"GHX\""))
            args += " --extendedShapes " + extend;

        args += (phonetic ? " -r phonetic" : "");

        if (sourceAudioScript != null)
            args += " -d \"" + AssetDatabase.GetAssetPath(sourceAudioScript) + "\"";

        args +=  " \"" + audioPath + "\"";

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