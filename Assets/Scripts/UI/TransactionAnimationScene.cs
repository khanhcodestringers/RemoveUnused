using UnityEngine;
using System.Collections;

public class TransactionAnimationScene : SSMotion
{

    public const float COMMON_TIME = 0.25f;

    public Animation animationController;
    public AnimationClip animationShow;
    public AnimationClip animationHide;

    public override void PlayShow()
    {
        if (animationController != null)
        {
            animationController.Stop();
            animationController.clip = animationShow;
            animationController.Play();
        }
        base.PlayShow();
    }

    public override void PlayHide()
    {
        if (animationController != null)
        {
            animationController.Stop();
            animationController.clip = animationHide;
            animationController.Play();
        }
        base.PlayHide();
    }

    public override float TimeHide()
    {
        if (animationHide != null)
        {
            return animationHide.length;
        }
        else
            return 0;
    }
    public override float TimeShow()
    {
        if (animationShow != null)
            return animationShow.length;
        else
            return 0;
    }
    //public override float TimeShow()
    //{

    //    return 0;
    //}
}
