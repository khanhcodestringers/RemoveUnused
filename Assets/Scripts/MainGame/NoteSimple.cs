using UnityEngine;
using System.Collections;
using Mio.TileMaster;
//using Framework;
using DG.Tweening;
public class NoteSimple : MonoBehaviour {
	public SpriteRenderer sprite;
    protected const int BASE_COLLIDER_WIDTH = 322;
    protected const int BASE_COLLIDER_HEIGHT = 480;
    protected const float MAX_COLLIDER_EXPAND = 1.2f;
    public bool isLongNote = false;
    public bool isBonus = false;
    private Transform poolRoot;
	[HideInInspector]
	public TileData data;
	//private int tileIndex;
	private int indexColumn;
	[HideInInspector]
	public int height;
	protected bool isClickable=true;
	protected TouchCover touchCover;
	protected bool isFinish=false;
    [SerializeField]
    protected BoxCollider2D box = null;
    [SerializeField]
    protected Vector2 colliderOffset;

    //protected Tweener tweenFadeOut = null;
    private Transform tf;

    private int tileIndex = 0;


    public bool IsClickable(){
		return isClickable;
	}

    public void Setup(TileData _data,Vector3 _spawnPos,int _tileIndex,int _column,int _height){
        if (!isBonus) {
            this.tileIndex = _tileIndex;
            this.data = _data;            
        }

        this.indexColumn = _column;
        tf = this.transform;

        Transform parent = InGameUIController.Instance.lineRoot;
		tf.SetParent (parent);
		tf.localScale = Vector3.one;
		tf.localPosition = new Vector3 (indexColumn*272, _spawnPos.y+InGameUIController.Instance.fixStartHeight, 0);
        if (box == null) {
            box = gameObject.GetComponent<BoxCollider2D>();
        }
        if (data.type == TileType.Normal) {
            //calculate collider's size
            float speedRatio = InGameUIController.Instance.gameplay.GetSpeedRatio();
            float addY = 0;
            float colliderX = BASE_COLLIDER_WIDTH;
            if (speedRatio > 0) {
                addY = (speedRatio - 1) * 240;
                if (addY > 240) {
                    addY = 240;
                }

                addY += 200;
                colliderX = speedRatio < MAX_COLLIDER_EXPAND ? speedRatio * BASE_COLLIDER_WIDTH : BASE_COLLIDER_WIDTH * MAX_COLLIDER_EXPAND;
            }
            box.size = new Vector2(colliderX, 480 + addY);
            box.offset = colliderOffset + new Vector2(0, addY * 0.25f);

            this.height = 480;
            this.isClickable = true;
            this.isFinish = false;

            sprite.DOKill(false);
            Color color = sprite.color;
            color.a = 1;
            sprite.color = color;
        }

        //Debug.Log(data.startTime);
    
    }
	public void SetupInPool(Transform _rootPool){
		this.poolRoot = _rootPool;
		Reset ();
	}
	public virtual void Reset(){
        
        tf = this.transform;
		tf.SetParent (poolRoot);
		tf.localScale = Vector3.one;
		tf.localPosition = Vector3.zero;
		isClickable = true;
		isFinish = false;
        //reset to default stage
        isSyncToServer = false;

    }
	public virtual void Press(TouchCover _touchCover){
		this.touchCover = _touchCover;
		if (!isClickable) {
			return;
		}
        isClickable = false;
		isFinish = true;
		//play appropriate sound
		MidiPlayer.Instance.PlayPianoNotes(data.notes, InGameUIController.Instance.gameplay.GetSpeedRatio(), true, data.soundDelay);
		InGameUIController.Instance.gameplay.IncreaseAndShowScore ();
        InGameUIController.Instance.gameplay.TileFinished();
        // AchievementHelper.Instance.LogAchievement("totalNoteHit");
		EffectWhenFinish ();
	}
	public virtual void OnKeepTouch(){
		
	}
	public virtual void OnReleaseTouch(){
		
	}

	protected virtual void EffectWhenFinish(){
        sprite.DOFade(0.1f, 0.1f).Play();
	}

	public virtual float  GetDistanceAcceptPass(){
		return 480;
	}
	public float GetTopHeightForPass(){
		if (!GameConsts.isPlayAuto && !InGameUIController.Instance.gameplay.isListenThisSong) {
			return tf.localPosition.y + GetDistanceAcceptPass ();
		} else {
			return tf.localPosition.y-200;
		}
	}
	public float GetTopHeightForReset(){
		return tf.localPosition.y + height + 400;
	}
	public bool GetFinish(){
		return isFinish;	
	}
	public void ResetClickable(){
		isClickable = true;
	}

    bool isSyncToServer = false;
    public void SetOnCompleteOutScene()
    {
        if (!isSyncToServer)
        {

            Counter.AddNote(tileIndex.ToString(), data.score);
            isSyncToServer = true;
        }
    }
}
