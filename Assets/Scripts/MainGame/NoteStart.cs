using UnityEngine;
using System.Collections;
//using Framework;
using Mio.TileMaster;

public class NoteStart : NoteSimple {
	public GameObject textStart;
    public GameObject loadingAnimation;
	// Use this for initialization
	//void Start () {
	
	//}
	
	//// Update is called once per frame
	//void Update () {
	
	//}
	public override void Press(TouchCover _touchCover){		
		if (!isClickable) {
			return;
		}
        InGameUIController.Instance.gameplay.StartGame();
        isClickable = false;
        gameObject.SetActive(false);
        //EffectWhenFinish ();
        LevelDataModel level = GameManager.Instance.SessionData.currentLevel;
	}
	public void ResetForNewGame(){
        gameObject.SetActive(true);
		Color color = sprite.color;
		color.a = 1;
		sprite.color = color;
		isClickable = true;
		isFinish = false;
	}

    public void SetLoading(bool show) {
        if (show) {
            isClickable = false;
            textStart.SetActive(false);
            loadingAnimation.SetActive(true);
        }else {
            isClickable = true;
            textStart.SetActive(true);
            loadingAnimation.SetActive(false);
        }
    }
}
