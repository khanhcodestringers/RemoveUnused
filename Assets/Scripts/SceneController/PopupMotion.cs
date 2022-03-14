using UnityEngine;
using System.Collections;

public class PopupMotion : SSMotion {
    public Animation animationController;
    public AnimationClip animationShow;
    public AnimationClip animationHide;

    public override void PlayShow() {
        if (animationController != null) {
            animationController.Stop();
            animationController.clip = animationShow;
            animationController.Play();
        }
        base.PlayShow();
    }

    public override void PlayHide() {
        if (animationController != null) {
            animationController.Stop();
            animationController.clip = animationHide;
            animationController.Play();
        }
        base.PlayHide();
    }

    public override float TimeHide() {
        return animationHide.length;
        //print(animationController[animationHide.name].time);
        //print(animationController[animationHide.name].normalizedTime);
        //return animationController[animationHide.name].time;
    }
}
