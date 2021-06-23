using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LipSyncAnimator3D : LipSyncAnimator
{
    Animator animator;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
    }

    public override void PlayAnimation(string audioName, string animationName)
    {
        base.PlayAnimation(audioName, animationName);
        ((AnimatorOverrideController)animator.runtimeAnimatorController)["Speak"] = Resources.Load<AnimationClip>(animationName);
        animator.SetTrigger("Speak");
    }

    public override void Reset()
    {
        base.Reset();
        animator.ResetTrigger("Stop");
    }
}