using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LipSyncAnimator2D : LipSyncAnimator
{
    [Tooltip("Mouth sprite filenames before shape letter, e.g. 'Character2_Mouth_'")]
    public string mouthPrefix;
    [Tooltip("SpriteRenderer for the mouth object")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("Number of frames to fade out the last mouth shape. Recommend to keep this at 1 or 0")]
    public int transitionFrames = 1;
    [Tooltip("Delay the visual animation to line up audio and video (in seconds)")]
    public float AVSync;

    TextAsset lipSyncAnimation;

    List<LipSpriteSwap> mouthKeyframe = new List<LipSpriteSwap>();
    Sprite[] mouthSprites;
    string[] param;
    int currentIndex = -1;
    float timer = -1f;
    SpriteRenderer mouthFadeRenderer;
    Color fadeColor = Color.white;
    float fadeCount;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        mouthSprites = new Sprite[LipSyncConstants.MouthShape.Count];
        for (int i = 0; i < mouthSprites.Length; i++)
        {
            mouthSprites[i] = Resources.Load<Sprite>(mouthPrefix + LipSyncConstants.MouthShape[i]);
        }

        mouthFadeRenderer = new GameObject(spriteRenderer.name + " Fadeout").AddComponent<SpriteRenderer>();
        mouthFadeRenderer.transform.SetParent(spriteRenderer.transform.parent);
        mouthFadeRenderer.transform.position = spriteRenderer.transform.position;
        mouthFadeRenderer.transform.rotation = spriteRenderer.transform.rotation;
        mouthFadeRenderer.transform.localScale = spriteRenderer.transform.localScale;
        mouthFadeRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
        mouthFadeRenderer.sprite = spriteRenderer.sprite;
    }

    public override void PlayAnimation(string audioName, string animationName)
    {
        base.PlayAnimation(audioName, animationName);
        StartCoroutine(Animate(animationName));
    }

    IEnumerator Animate(string animationName)
    {
        lipSyncAnimation = Resources.Load<TextAsset>(animationName);
        while (lipSyncAnimation == null)
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }

        string[] mouthLines = lipSyncAnimation.text.Split('\n');

        audioSource.clip = Resources.Load<AudioClip>(mouthLines[0]);

        for (int i = 1; i < mouthLines.Length; i++)
        {
            if (!mouthLines[i].Trim().Equals(""))
            {
                param = mouthLines[i].Split('\t');
                if (param.Length == 2)
                {
                    mouthKeyframe.Add(new LipSpriteSwap(float.Parse(param[0]), LipSyncConstants.MouthShape.FindIndex(param[1].Contains)));
                }
            }
        }

        timer = 0f;
        audioSource.Play();
        currentIndex = 1;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (timer >= 0)
        {
            fadeColor.a = (fadeCount--) / (transitionFrames + 1);
            mouthFadeRenderer.color = fadeColor;
            if ((timer += Time.deltaTime) - AVSync >= mouthKeyframe[currentIndex].time)
            {
                mouthFadeRenderer.sprite = spriteRenderer.sprite;
                spriteRenderer.sprite = mouthSprites[mouthKeyframe[currentIndex].spriteNum];
                fadeCount = transitionFrames;
                currentIndex++;
                if (currentIndex >= mouthKeyframe.Count)
                {
                    currentIndex = 0;
                    timer = -1f;
                }
            }
        }
    }
}