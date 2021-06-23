using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LipSyncAnimator : MonoBehaviour
{
    public float stopFadeTime;

    protected AudioSource audioSource;
    float fadeOutTimer = -1f;
    float defaultVolume;

    public virtual void Start()
    {
        if ((audioSource = GetComponent<AudioSource>()) == null)
        {
            gameObject.AddComponent(typeof(AudioSource));
        }

        defaultVolume = audioSource.volume;
    }

    public virtual void PlayAnimation(string audioName, string animationName)
    {
        Reset();
        audioSource.clip = Resources.Load<AudioClip>(audioName);
        audioSource.Play();
    }

    public virtual void Reset()
    {
        fadeOutTimer = -1f;
        audioSource.volume = defaultVolume;
    }

    public virtual void StopAnimation()
    {
        if (fadeOutTimer == -1f)
        {
            fadeOutTimer = stopFadeTime;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (fadeOutTimer != -1f)
        {
            if (fadeOutTimer > 0f)
            {
                fadeOutTimer -= Time.deltaTime;
                audioSource.volume = (fadeOutTimer / stopFadeTime) * defaultVolume;
            }
            else
            {
                audioSource.Stop();
                Reset();
            }
        }
    }
}
