using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mio.TileMaster;
//using Framework;
using DG.Tweening;

public class NoteMulti : NoteSimple {
    public const int MAX_COLLIDER_ADDITIONAL_SIZE = 240;
    // Use this for initialization
    LongNotePlayedData playData = null;
    public SpriteRenderer headSprite;
    public SpriteRenderer tailSprite;
    public SpriteRenderer bgSprite;

    [SerializeField]
    private Animator blinkEffect;

    public NoteTouchEffectManagement touchKeepEffect; 
     
    public Sprite sprite43Ratio;

    public Sprite[] spriteHead;
    public GameObject objActive;
    private float cacheYHeight = 0;
    private bool isMoveComplete = false;

    private bool isActiveAuto = false;
    [SerializeField]
    //private TextMesh lbScoreLabel;
    private NumericSprite lbScoreLabel;

    //private static Color normal = Cor;
    //private static Color colorSpriteActive = new Color(71.0f / 255f, 161.0f / 255.0f, 236.0f / 255.0f);
    //private static Color colorTextNormal = Color.black;

    private static Dictionary<int, string> listScoreText = new Dictionary<int, string>(100);
     

    //private Tween tweenFadeOut;

    // Update is called once per frame
    void Update() {
        if(isActiveAuto) {
            if(!isMoveComplete) {
                IncreaseActiveHeight(InGameUIController.Instance.cacheDeltaMove.y);
            }
            if(!isClickable) {
                OnKeepTouch();
            }
        }
    }

    private string GetScoreText(int score) {
        if(!listScoreText.ContainsKey(score)) {
            //print("creating new key for score " + score);
            listScoreText[score] = string.Format("+{0}", score);
        }

        return listScoreText[score];
    }

    public new void Setup(TileData _data, Vector3 _spawnPos, int _tileIndex, int _column, int _height) {
        base.Setup(_data, _spawnPos, _tileIndex, _column, _height);
        //if(colorTextNormal == Color.black) {
        //    colorTextNormal = lbScoreLabel.color;
        //}
        //hide score label
        lbScoreLabel.text = GetScoreText(_data.score);
        Color t = lbScoreLabel.color;
        t.a = 1;
        lbScoreLabel.color = t;
        lbScoreLabel.gameObject.SetActive(false);

        //Debug.Log("Current scale: " + InGameUIController.scale);
        //swap sprite for screen ratio of 4:3 (iPad)
        if(InGameUIController.scale <= 0.8f) {
            bgSprite.sprite = sprite43Ratio;
        }

        this.height = _height;
        if (box == null) {
            box = gameObject.GetComponent<BoxCollider2D>();
        }
        float speedRatio = InGameUIController.Instance.gameplay.GetSpeedRatio();
        float addY = 0;
        float colliderX = BASE_COLLIDER_WIDTH;
        if(speedRatio > 0) {
            addY = (speedRatio - 1) * MAX_COLLIDER_ADDITIONAL_SIZE;
            if(addY > MAX_COLLIDER_ADDITIONAL_SIZE) {
                addY = MAX_COLLIDER_ADDITIONAL_SIZE;
            }

            addY += 100;
            colliderX = speedRatio < MAX_COLLIDER_EXPAND ? speedRatio * BASE_COLLIDER_WIDTH : BASE_COLLIDER_WIDTH * MAX_COLLIDER_EXPAND;
        }
        box.size = new Vector2(colliderX, 700 + addY);
        box.offset = colliderOffset + new Vector2(0, addY * 0.25f);

        Vector3 scale = sprite.transform.localScale;
        scale.y = (int)(_height / 4.80f);
        sprite.transform.localScale = scale;
        lbScoreLabel.transform.localPosition = new Vector3(40, height);

        playData = new LongNotePlayedData();
        playData.timeStampBeginTouch = 0;
        playData.noteDataPlayedIndex = -1;

        objActive.SetActive(false);
        headSprite.sprite = spriteHead[0];
        cacheYHeight = 0;
        isClickable = true;
        isFinish = false;
        isMoveComplete = false;
        isActiveAuto = false;

        //iTween[] listOld = gameObject.GetComponents<iTween>();
        //for (int i = 0; i < listOld.Length; i++) {
        //    if (listOld[i] != null) {
        //        Destroy(listOld[i]);
        //    }
        //}

        sprite.DOKill();
        //sprite.color = normal;
        bgSprite.gameObject.SetActive(true);
        //Color color = sprite.color;
        //color.a = 1;
        sprite.color = Color.white; 
    }

    public override void Press(TouchCover _touchCover) {
        this.touchCover = _touchCover;
        if(!isClickable) {
            return;
        }
        
        isClickable = false;
        isFinish = true;
        isMoveComplete = false;
        if(touchCover != null) InGameUIController.Instance.CacheTouchForNote(touchCover, this);
        MidiPlayer.Instance.PlayPianoNotes(data.notes, InGameUIController.Instance.gameplay.GetSpeedRatio(), true, data.soundDelay);
        InGameUIController.Instance.gameplay.IncreaseAndShowScore();
        InGameUIController.Instance.gameplay.TileFinished();
        //AchievementHelper.Instance.LogAchievement("totalNoteHit");
        //isClickable = false;
        if(GameConsts.isPlayAuto || InGameUIController.Instance.gameplay.isListenThisSong) {
            isActiveAuto = true;
        }
        blinkEffect.gameObject.SetActive(true);
        blinkEffect.StartPlayback();

        if (!InGameUIController.Instance.gameplay.isListenThisSong) {
            //Debug.Log("Is listen this song; " + InGameUIController.Instance.gameplay.isListenThisSong);
            touchKeepEffect.OnTouchDown(gameObject, touchCover);
        }

    }

     
    public void OnShowUIWhenPress(Vector3 posTouch) {
        posTouch.y += 160;
        //Debug.LogError ("Pos:"+posTouch+",Pos now:"+this.transform.localPosition+",touch:"+touchCover.position);
        float yHeight = posTouch.y - this.transform.localPosition.y;
        if(yHeight < 320) {
            yHeight = 320;
        }
        //Debug.LogError ("Cache Height=" + yHeight);
        cacheYHeight = yHeight;
        SetupActiveHeight();
        objActive.SetActive(true);
    }

    public void IncreaseActiveHeight(float addY) {
        cacheYHeight += addY / InGameUIController.scale;
        //Debug.LogError ("IncreaseActiveHeight:" + cacheYHeight);
        if(cacheYHeight >= height) {
            headSprite.sprite = spriteHead[1];
            SetupActiveHeight();
            OnReleaseTouch();
        } else {
            SetupActiveHeight();
        }

    }

    public void SetupActiveHeight() {
        float heightActive = cacheYHeight;
        if(heightActive >= height) {
            heightActive = height;
        }
        Vector3 scale = tailSprite.transform.localScale;
        float scaleY = (heightActive / 4.8f) - 10 / InGameUIController.scale;
        scale.y = scaleY;
        tailSprite.transform.localScale = scale;

        Vector3 posHead = headSprite.transform.localPosition;
        //posHead.y = heightActive - 96 / InGameUIController.scale;
        posHead.y = heightActive - 48 / InGameUIController.scale;
        headSprite.transform.localPosition = posHead;

    }

    public bool IsMoveCompleted() {
        return isMoveComplete;
    }
    public override float GetDistanceAcceptPass() {
        return 700;
    }

    //private float lastUpdate = 0;
    //if the tile is hold, prepare to play its music
    //List<NoteData> notesToPlay = new List<NoteData>(15);
    public override void OnKeepTouch() {

    }

    public override void OnReleaseTouch() {
        //Debug.Log("OnReleaseTouch");
        if(!isMoveComplete) {
            //Debug.LogError ("OnReleaseTouch");
            //int score = (int)System.Math.Round(height / 480.0f);
            //if (score < 2) {
            //	score = 2;
            //}
            isMoveComplete = true;
            
            lbScoreLabel.gameObject.SetActive(true);
            //increase tile's score minus one because we have already increased it when first touched
            InGameUIController.Instance.gameplay.IncreaseAndShowScore(data.score - 1);
            EffectWhenFinish();

            blinkEffect.StopPlayback();
            blinkEffect.gameObject.SetActive(false);
        }

        
        //Debug.Log("hide");
        //touchKeepEffect.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.9f).OnComplete(() => {
        //    touchKeepEffect.gameObject.SetActive(false);
        //}).Play();

        //touchNoteEffect.gameObject.SetActive(false);
    }

    public void OnKeepTouchEnd() {
        blinkEffect.StopPlayback();
        blinkEffect.gameObject.SetActive(false);
        touchKeepEffect.OnTouchUp(gameObject);

        //Debug.Log("OnKeepTouchEnd "+ isMoveComplete);
    }

    protected override void EffectWhenFinish() {
        
        bgSprite.gameObject.SetActive(false);
        objActive.SetActive(false);
         
        sprite.DOFade(0.25f, 0.1f).Play();
        DOTween.ToAlpha(() => lbScoreLabel.color, x => lbScoreLabel.color = x, 0.1f, 0.5f).Play();

    }

    public override void Reset() {
        base.Reset(); 

    }
}
