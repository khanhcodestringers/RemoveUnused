using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ParallaxBackground : MonoSingleton<ParallaxBackground> {
    [SerializeField]
    private UISprite background;

    [SerializeField]
    private Vector2 moveLimit;
    [SerializeField]
    private Vector2 scrollLimit;

    public int valueForShowPump = 5;

    public ControllPump pump;

    private Transform scrollView;
    private bool shouldMap = false;
    public bool ShouldMapMovement {
        get { return shouldMap; }
        set { shouldMap = value; }
    }
    private float scrollDistance, moveDistance;
    private Tween t;


    void Awake() 
    {
        float ratioDefault = 9.0f / 16;
        float ratio = Screen.width * 1.0f / Screen.height;
        float scale = ratio / ratioDefault;
        if (scale > 1.0f) 
        {
            moveLimit.x *= scale;
        }
    }

    public void MapMovement(float percentMoved, bool smooth = false, float smoothDuration = 0.5f) {
        Vector3 pos = background.cachedTransform.localPosition;
        float targetY = Mathf.Lerp(moveLimit.x, moveLimit.y, percentMoved);
        if (targetY > (moveLimit.x + valueForShowPump))
        {
            if (isShowPump)
            {
                pump.HidePump();
                isShowPump = false;
            }
        }
        else {
            if (!isShowPump)
            {
                pump.ShowPump();
                isShowPump = true;
            }
        }
        if (smooth) {
            float y = pos.y;

            if (t!= null && t.IsPlaying()) {
                t.Kill();
            }

            t = DOTween.To(() => y, x => y = x, targetY, smoothDuration)
                .SetEase(Ease.InCubic)
                .OnUpdate(() => {
                    //Debug.Log("Smoothing " + y);
                    Vector3 p = background.cachedTransform.localPosition;
                    p.y = y;
                    background.cachedTransform.localPosition = p;
                })
                .Play();
        }
        else {
            pos.y = targetY;
            background.cachedTransform.localPosition = pos;
        }
    }

    bool isShowPump;
    //bool isHidePump;
    //void Update() 
    //{
        
    //}
}
