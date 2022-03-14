using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class CountdownView : MonoBehaviour {
    public UILabel lbCountdown;
    public UISprite spriteCountdown;

    //private float startCountdown;
    private float currentCountdown;
    //private float timePerStep;
    private Tween tween;

    //public bool isTest = false;
    //void Update() {
    //    if (isTest) {
    //        isTest = false;
    //        DoCountdown(10, 1, ()=>{ print("Countdown complete"); });
    //    }
    //}
    /// <summary>
    /// Show countdown effect and call an action when finished
    /// </summary>
    /// <param name="duration">How long will the countdown last</param>
    /// <param name="timePerStep">How long each countdown step last in seconds</param>
    /// <param name="OnComplete">The action to call after countdown completed</param>
    public void DoCountdown(float duration, float timePerStep, Action OnComplete = null) {
        StopCountdown();

        currentCountdown = duration;
        //this.timePerStep = timePerStep;

        gameObject.SetActive(true);

        lbCountdown.text = duration.ToString();
        spriteCountdown.fillAmount = 0;
        int loop = Mathf.CeilToInt(duration / timePerStep);

        tween = DOTween.Sequence()
            .Join(DOTween.To(
                () => spriteCountdown.fillAmount,
                (fill) => spriteCountdown.fillAmount = fill,
                1,
                duration)
                .SetEase(Ease.Linear)
                .Play())
            .Join(DOTween.Sequence()
                .AppendInterval(timePerStep)
                .AppendCallback(() => {
                    currentCountdown -= timePerStep;
                    lbCountdown.text = currentCountdown.ToString();
                })
                .SetLoops(loop)
                .Play()
                )            
            .OnComplete(() => {
                Helpers.Callback(OnComplete);
            })
            .Play();
    }

    internal void StopCountdown() {
        if (tween != null && tween.IsPlaying()) { tween.Kill(false); }
    }
}
