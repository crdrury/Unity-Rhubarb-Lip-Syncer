using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LipSyncAnimator3D : MonoBehaviour
{
    AudioSource audioSource;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string audioName, string animationName)
    {
        audioSource.clip = Resources.Load<AudioClip>(audioName);
        ((AnimatorOverrideController)animator.runtimeAnimatorController)["Speak"] = Resources.Load<AnimationClip>(animationName);

        animator.SetTrigger("Speak");
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}