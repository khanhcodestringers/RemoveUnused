using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mio.TileMaster;
using System;
using DG.Tweening;
using Mio.Utils;
using Mio.Utils.MessageBus;

/// <summary>
/// Touch cover: Minimal data using for Touch
/// </summary>
public class TouchCover {
    public TouchPhase phase = TouchPhase.Canceled;
    public Vector3 position = Vector3.zero;
    public int fingerId = 0;
    public TouchCover (int _fingerId) {
        this.fingerId = _fingerId;
    }
    public void Reset () {
    }
}
public class InGameUIController : MonoSingleton<InGameUIController> {
    public Camera noteCamera;
    public GameObject touchEffect;
    public GameObject prefabNoteMulti;
    public GameObject prefabNoteSimple;
    public NoteBonus prefabNoteBonus;
    public Transform poolRoot;
    public Transform lineRoot;
    public Transform rootScale;
    public bool gameStarted = false;

    //which note to play when touch the wrong tile?
    private static readonly int PIANO_NOTE_GAME_OVER = 36;
    private int maxCacheDefault = 12;

    private Queue<NoteMulti> poolNoteMulti = new Queue<NoteMulti>();
    private Queue<NoteSimple> poolNoteSimple = new Queue<NoteSimple>();

    [HideInInspector]
    public TileMasterGamePlay gameplay;
    public Transform cameraTouchPos;
    public Transform screenTouchPos;
    public Transform bottomRightLocation;
    public Transform objParentMove;
    public GameObject prefabDie;
    [HideInInspector]
    public Vector3 cacheDeltaMove = Vector3.zero;
    /// <summary>
    /// The list long note played data.Using for long note
    /// </summary>
    [HideInInspector]
    public Dictionary<int, LongNotePlayedData> listLongNotePlayedData = new Dictionary<int, LongNotePlayedData>();

    List<NoteSimple> listNoteActive = new List<NoteSimple>(200);
    List<NoteBonus> listBonusTiles = new List<NoteBonus>(8);
    NoteTouchEffectManagement noteTouchEffect;

    public int GetCountNoteActive () {
        return listNoteActive.Count;
    }
    [HideInInspector]
    public float distanceCoff = 0;


    /// <summary>
    /// dicTouchCover cache touch cover by mouse and touch
    /// </summary>
    private Dictionary<int, TouchCover> dicTouchCover = new Dictionary<int, TouchCover>();
    /// The dic touch: using for control stage of touch
    /// </summary>
    Dictionary<int, NoteMulti> dicTouch = new Dictionary<int, NoteMulti>();

    public GameObject startObj;
    //init width + length of 1 tile
    public int fixStartHeight = 660 + 480;

    float screenRatioDefault = 9.0f / 16;
    float screenRatioCurrent;
    public static float scale;

    private static int tileWidth = 272;
	public bool canTouch = true;


    void Start () {
        screenRatioCurrent = Screen.width * 1.0f / Screen.height;
        scale = screenRatioDefault / screenRatioCurrent;
        //scale = Math.Rou
        //Debug.Log("Current scale: " + scale);
        noteTouchEffect = new NoteTouchEffectManagement(noteCamera, touchEffect.GetComponent<NoteTouchEffect>());
    }

    public void PlayAutoGame () {
        gameplay.isListenThisSong = true;
        startObj.SetActive(false);
        //gameplay.SetAutoPlayUI();
        gameplay.StartGame();        
    }

    public void ResetForNewGame () {
        distanceCoff = 0;

        noteCamera.transform.localPosition = Vector3.zero;
        startObj.GetComponent<NoteStart>().ResetForNewGame();
        for (int i = 0; i < listNoteActive.Count; i++) {
            PushToPool(listNoteActive[i]);
        }

        for (int b = 0; b < listBonusTiles.Count; b++) {
            Destroy(listBonusTiles[b].gameObject);
        }
        gameStarted = false;
        listBonusTiles.Clear();
        listNoteActive.Clear();
        dicTouchCover.Clear();
        if(noteTouchEffect==null)
            noteTouchEffect = new NoteTouchEffectManagement(noteCamera, touchEffect.GetComponent<NoteTouchEffect>());
        noteTouchEffect.OnGameReset();
    }

    public void Setup (TileMasterGamePlay _gameplay) {
        distanceCoff = 0;

        this.gameplay = _gameplay;
        this.listLongNotePlayedData.Clear();
        for (int i = 0; i < maxCacheDefault; i++) {
            PopSimple("simple_" + i);
            PopMulti("multi_" + i);
        }
        System.Random rand = new System.Random();
        int ran = rand.Next() % 4;
        startObj.transform.SetParent(lineRoot);
        startObj.transform.localScale = Vector3.one;
        startObj.transform.localPosition = new Vector3(ran * tileWidth, 0, 0);
    }

    public void PushToPool (NoteSimple objNote) {
        if (!objNote.isLongNote) {
            NoteSimple simple = (NoteSimple)objNote;
            simple.Reset();
            poolNoteSimple.Enqueue(simple);
        }
        else {
            NoteMulti multi = (NoteMulti)objNote;
            multi.Reset();
            poolNoteMulti.Enqueue(multi);
        }
    }

    public NoteSimple PopSimple (string name) {
        if (poolNoteSimple.Count < 1) {
            GameObject obj = GameObject.Instantiate(prefabNoteSimple) as GameObject;
            NoteSimple simple = obj.GetComponent<NoteSimple>();
            simple.isLongNote = false;
            simple.SetupInPool(poolRoot);
            obj.name = name; // "simple_buffer";
            poolNoteSimple.Enqueue(simple);
        }
        return poolNoteSimple.Dequeue();
    }

    public NoteMulti PopMulti (string name) {
        if (poolNoteMulti.Count < 1) {
            GameObject obj = GameObject.Instantiate(prefabNoteMulti) as GameObject;
            NoteMulti multi = obj.GetComponent<NoteMulti>();
            multi.isLongNote = true;
            multi.SetupInPool(poolRoot);
            obj.name = name; // "multi_buffer";
            poolNoteMulti.Enqueue(multi);
            multi.touchKeepEffect = noteTouchEffect;
        }
        return poolNoteMulti.Dequeue();
    }

    public NoteBonus CreateBonusTile (BonusType type) {
        NoteBonus bonus = GameObject.Instantiate(prefabNoteBonus);
        bonus.name = "bonus";

        //NoteBonus bonus = obj.GetComponent<NoteBonus>();
        bonus.isLongNote = false;
        bonus.bonusType = type;

        return bonus;
    }

    public void CreateNewNote (TileData data, Vector3 spawnPos, int tileIndex, int column, int height, bool withBonusTile = false, BonusType bonusType = BonusType.Diamond) {
        if (data.type == TileType.LongNote) {
            NoteMulti multi = PopMulti("multi_buffer");
            multi.Setup(data, spawnPos, tileIndex, column, height);
            if (listNoteActive.Contains(multi)) {
                listNoteActive.Remove(multi);
            }
            listNoteActive.Add(multi);
        }
        else {
            NoteSimple simple = PopSimple("simple_buffer");
            simple.Setup(data, spawnPos, tileIndex, column, height);
            if (listNoteActive.Contains(simple)) {
                listNoteActive.Remove(simple);
            }
            listNoteActive.Add(simple);
        }

        if (withBonusTile) {
            NoteBonus bonus = CreateBonusTile(bonusType);
            int bonusColumn = (column - 2 >= 0) ? column - 2 : column + 2;
            bonus.Setup(data, spawnPos, tileIndex, bonusColumn, height);
            listBonusTiles.Add(bonus);
        }
    }

    public NoteSimple GetLastNoteGenerate () {
        if (listNoteActive.Count > 0) {
            return listNoteActive[listNoteActive.Count - 1];
        }

        Debug.LogWarning("Could return last generated note, returning null");
        return null;
    }

    Vector3 oldPos, newPos;
    public void ProcessUpdate (float speed) {
        Vector3 translate = Vector3.up * speed * Time.smoothDeltaTime;
        oldPos = noteCamera.transform.localPosition;
        noteCamera.transform.Translate(translate);
        newPos = noteCamera.transform.localPosition;
        cacheDeltaMove = newPos - oldPos;

        Vector3 vecBottom = GetBottomPosition();
        //return;
        // check game over
        for (int i = 0; i < listNoteActive.Count; i++) {
            if (!listNoteActive[i].GetFinish()) {
                //check if a tile has gone pass the designated point or not
                float topPassY = listNoteActive[i].GetTopHeightForPass();
                //if yes, continue checking
                if (vecBottom.y > topPassY) {
                    //if the game is not on auto play mode, proceed to game over state
                    if (!GameConsts.isPlayAuto && !gameplay.isListenThisSong) {
                        //Debug.Log("GameOver " + listNoteActive[i].gameObject.name);
                        gameplay.ChangeStatusToGameOver();

                        StartCoroutine(RoutineGameOverByMissing(listNoteActive[i]));
                        return;
                    }
                    else {
                        if (dicTouchCover.ContainsKey(0)) {
                            listNoteActive[i].Press(dicTouchCover[0]);
                        }else {
                            listNoteActive[i].Press(null);
                        }

                        if (listNoteActive[i].isLongNote) {
                                NoteMulti multi = (NoteMulti)listNoteActive[i];
                                multi.OnShowUIWhenPress(Vector3.zero);
                            }
                        
                    }
                }
                else {
                    break;
                }
            }
        }

        //find used tiles
        List<NoteSimple> listCanReset = new List<NoteSimple>();
        for (int i = 0; i < listNoteActive.Count; i++) {
            if (listNoteActive[i].GetFinish()) {
                float topPassY = listNoteActive[i].GetTopHeightForReset();

                if (vecBottom.y > topPassY) {
                    listCanReset.Add(listNoteActive[i]);
                }
                else {
                    break;
                }
            }
        }

        //recover used tile
        for (int i = 0; i < listCanReset.Count; i++) {
            listNoteActive.Remove(listCanReset[i]);
            listCanReset[i].Reset();
            PushToPool(listCanReset[i]);

            //generate new tiles for each used one
            gameplay.GenerateNextTile();
        }
    }

    public void OnProcessInputControl () {
		if(!canTouch) return;
#if UNITY_EDITOR // in editor simulation one touch

        TouchCover touch = null;
        if (!dicTouchCover.TryGetValue(0, out touch))
        {
            touch = new TouchCover(0);
            dicTouchCover[0] = touch;
        }
        bool isUpdate = false;
        if (Input.GetMouseButtonDown(0))
        {
            //press
            touch.position = Input.mousePosition;
            touch.phase = TouchPhase.Began;
            isUpdate = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //release
            touch.position = Input.mousePosition;
            touch.phase = TouchPhase.Ended;
            isUpdate = true;
        }
        else if (Input.GetMouseButton(0))
        {
            //hole
            touch.position = Input.mousePosition;
            touch.phase = TouchPhase.Stationary;
            isUpdate = true;
        }
        if (isUpdate)
        {
            ProcessControlTouch(touch);

        }
#else 
        // for multi-touch on device
        for (int i = 0; i < Input.touchCount; i++) {
            TouchCover touch = null;

            if (!dicTouchCover.TryGetValue(Input.touches[i].fingerId, out touch)) {
                touch = new TouchCover(Input.touches[i].fingerId);
                dicTouchCover[Input.touches[i].fingerId] = touch;
                //++currentTouch;
                /*if(currentTouch >= 2) {*/
                //Debug.Log("================\n Added touch id " + touch.fingerId + " on frame " + framecount);
                //}
            }
            touch.position = Input.touches[i].position;
            touch.phase = Input.touches[i].phase;
            ProcessControlTouch(touch);
        }

        if(Input.touchCount <= 0) {
            TouchCover touch = null;
            if (!dicTouchCover.TryGetValue(0, out touch)) {
                touch = new TouchCover(0);
                dicTouchCover[0] = touch;
            }
        }
#endif
    }

    public void ProcessControlTouch (TouchCover touch) {
        if (!gameStarted) return;
        if (touch == null) return;

        //Touch Press
        if (touch.phase == TouchPhase.Began) {
            Vector2 rayOrigin = noteCamera.ScreenToWorldPoint(touch.position);

            //check if mouse hit any object?
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero, 100, ProjectConstants.Layers.MainGameMask);
            if (hit && hit.transform != null) {
                string hitObjName = hit.transform.name.ToLower();

                //start object?
                if (gameplay.CurrentGameStage != TileMasterGamePlay.GameState.Playing) {
                    if (hitObjName.Contains("start")) {
                        //game start
                        NoteStart noteStart = hit.transform.gameObject.GetComponent<NoteStart>();
                        if (noteStart != null) {
                            noteStart.Press(touch);
                        }
                    }

                }

                //tiles?
                if (gameplay.CurrentGameStage == TileMasterGamePlay.GameState.Playing ||
                    gameplay.CurrentGameStage == TileMasterGamePlay.GameState.Continue ||
                    gameplay.CurrentGameStage == TileMasterGamePlay.GameState.AutoPlay) {

                    if (gameplay.CurrentGameStage == TileMasterGamePlay.GameState.Continue) {
                        gameplay.StartGame();
                    }

                    if (hitObjName.Contains("simple_")) {
                        NoteSimple simple = hit.transform.gameObject.GetComponent<NoteSimple>();
                        if (simple != null && CanTouchNote(simple)) {
                            simple.Press(touch);
                        }
                    }
                    else if (hitObjName.Contains("multi_")) {
                        NoteMulti multi = hit.transform.gameObject.GetComponent<NoteMulti>();
                        if (multi != null && CanTouchNote(multi)) {
                            multi.Press(touch);
                            multi.OnShowUIWhenPress(GetRootPositionHit(touch));
                        }
                    }
                    else if (hitObjName.Contains("bonus")) {
                        NoteBonus bonus = hit.transform.gameObject.GetComponent<NoteBonus>();
                        if (bonus != null) {
                            bonus.Press(null);
                        }
                    }
                }
            }
            else {
                //if the touch didn't hit any note, check for hit on background
                RaycastHit2D bgCheck = Physics2D.Raycast(rayOrigin, Vector2.zero, 100, ProjectConstants.Layers.BackgroundMask);
                if (bgCheck.transform != null) {
                    if (gameplay.CurrentGameStage == TileMasterGamePlay.GameState.Playing ||
                        gameplay.CurrentGameStage == TileMasterGamePlay.GameState.AutoPlay) {
                        if (CheckGameOver(touch)) {
                            GameOverByPressWrong(touch);
                        }
                    }
                    else if (gameplay.CurrentGameStage == TileMasterGamePlay.GameState.Continue) {
                        gameplay.StartGame();
                    }
                }
            }
        }

        // Touch Hold
        else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) {
            OnHoldTouch(touch);
        }
        else {
            ResetTouch(touch);
        }
    }

    public Vector3 GetRootPositionHit (TouchCover touch) {
        var cameraPos = noteCamera.ScreenToWorldPoint(touch.position);
        screenTouchPos.transform.position = cameraPos;
        return screenTouchPos.transform.localPosition;
    }

    public Vector3 GetBottomPosition () {
        screenTouchPos.transform.position = bottomRightLocation.transform.position;
        return screenTouchPos.transform.localPosition;
    }

    public void CacheTouchForNote (TouchCover touch, NoteMulti multi) {
        dicTouch[touch.fingerId] = multi;
    }

    /// <summary>
    /// Resets the touch. When release of cancel touch
    /// </summary>
    /// <param name="touch">Touch.</param>
    public void ResetTouch (TouchCover touch) {
        if (dicTouch.ContainsKey(touch.fingerId)) {
            dicTouch[touch.fingerId].OnKeepTouchEnd();
            dicTouch.Remove(touch.fingerId);
        }
        if (dicTouchCover.ContainsKey(touch.fingerId)) {
            dicTouchCover.Remove(touch.fingerId);
        }
    }
    public void ClearCacheTouchForNote (TouchCover touch, NoteMulti multi) {
        if (dicTouch.ContainsKey(touch.fingerId)) {
            NoteMulti multiCache = dicTouch[touch.fingerId];
            if (multi == multiCache) {
                dicTouch.Remove(touch.fingerId);
            }
        }
    }
    public void OnHoldTouch (TouchCover touch) {
        if (dicTouch.ContainsKey(touch.fingerId)) {
            NoteMulti multi = dicTouch[touch.fingerId];

            //clean up completed touch references
            if (multi.IsMoveCompleted())
            {
                ResetTouch(touch);
            }
            multi.OnKeepTouch();
            multi.IncreaseActiveHeight(cacheDeltaMove.y);
        }
    }

    #region Wrong notes
    public void GenerateNoteDie (TouchCover touch) {
        Vector3 posGenererate = Vector3.zero;
        float heightGenerate = 480;
        NoteSimple note = null;
        Vector3 vec = GetRootPositionHit(touch);
        posGenererate = vec;
        for (int i = 0; i < listNoteActive.Count; i++) {
            float min = listNoteActive[i].transform.localPosition.y;
            float max = min + listNoteActive[i].height;
            if (vec.y >= min && vec.y <= max) {
                note = listNoteActive[i];
            }
        }
        if (note != null) {
            int x = (int)(posGenererate.x / tileWidth);
            posGenererate = note.transform.localPosition;
            posGenererate.x = x * tileWidth;
            heightGenerate = note.height;
        }
        else {
            //Should never go to here
            int x = (int)(vec.x / tileWidth);
            posGenererate.x = x * tileWidth;
            heightGenerate = vec.y;            
        }

        GameObject objDie = GameObject.Instantiate(prefabDie);
        objDie.transform.parent = lineRoot;
        objDie.transform.localScale = Vector3.one;
        objDie.transform.localPosition = posGenererate;
        objDie.GetComponent<NoteDie>().Setup(heightGenerate);
        GameObject.Destroy(objDie, 2f);
    }

    public void GameOverByPressWrong (TouchCover touch) {
        gameplay.ChangeStatusToGameOver();
        StartCoroutine(RoutineGameOverByPressWrong(touch));
    }

    public IEnumerator RoutineGameOverByPressWrong (TouchCover touch) {
        //visualize where the touch gone wrong
        GenerateNoteDie(touch);
        MidiPlayer.Instance.PlayPianoNote(PIANO_NOTE_GAME_OVER);
        yield return new WaitForSeconds(0.4f);

        if (listNoteActive.Count > 1) {
            Vector3 bottom = GetBottomPosition();
            int stopIndex = 0;
            for (int i = 0; i < listNoteActive.Count; i++) {
                if (listNoteActive[i].IsClickable()) {
                    stopIndex = i;
                    break;
                }
            }

            //calculate the distance need to move back the camera
            float yMove = bottom.y - listNoteActive[stopIndex].transform.localPosition.y;
            Vector3 vecStart = noteCamera.transform.localPosition;
            Vector3 vecEnd = vecStart;

            vecEnd.y -= yMove * scale;

            //reset the last note
            listNoteActive[stopIndex].ResetClickable();
            //move back camera a bit
            noteCamera.transform.DOLocalMove(vecEnd, 0.5f).SetEase(Ease.OutQuad).Play();            
        }

        yield return new WaitForSeconds(0.8f);
        gameplay.ProcessGameOverEvent();
    }

    public IEnumerator RoutineGameOverByMissing (NoteSimple noteDie) {
        MidiPlayer.Instance.PlayPianoNote(PIANO_NOTE_GAME_OVER);
        yield return new WaitForSeconds(0.4f);
        Vector3 vecStart = noteCamera.transform.localPosition;
        Vector3 vecEnd = vecStart;
        
        vecEnd.y -= noteDie.GetDistanceAcceptPass() * scale;

        noteCamera.transform.DOLocalMove(vecEnd, 0.5f).SetEase(Ease.OutQuad).Play();
        yield return new WaitForSeconds(0.4f);

        StartCoroutine(EffectDieByMissing(noteDie));
        yield return new WaitForSeconds(0.5f);

        noteDie.ResetClickable();// dung cho replay
        gameplay.ProcessGameOverEvent();
    }

    void OnlineGameOver()
    {
        MessageBus.Annouce(new Message(MessageBusType.OnMeDieOnline));
        gameplay.currentState = TileMasterGamePlay.GameState.GameOver;
    }

    public bool CheckGameOver (TouchCover touch) {        
        NoteSimple note = null;
        Vector3 vec = GetRootPositionHit(touch);
        //skip first note (which is a special start object)
        if (vec.y <= startObj.transform.localPosition.y + 480) {
            return false;
        }

        //start checking from the end of the note list
        //check for a small bit of tiles only, not to go too far to save calculating time
        for (int i = 0; i < listNoteActive.Count && i < 6; i++) {
            float min = listNoteActive[i].transform.localPosition.y;
            float max = min + listNoteActive[i].height;
            //check if the touch on background has any tiles with the same Y?
            if (vec.y >= min && vec.y <= max) {
                //if yes, check if there is any tile at that exact location?
                float minX = listNoteActive[i].transform.localPosition.x;
                float maxX = minX + tileWidth;
                if (vec.x >= minX && vec.x <= maxX) {
                    //if yes, then the touch is fine, because may be we touch on an used tile              
                    //Debug.Log("Touched on background, but saved by note " + note.gameObject.name);
                    return false;
                }
                else {
                    //if not touch on any tiles, meaning the user has missed it
                    note = listNoteActive[i];
                    break;
                }
            }
        }
        //if there are no tile next to current touch
        if (note == null) {
            //mean the touch is fine
            return false;
        }

        return true;
    }
    #endregion

    private IEnumerator EffectDieByMissing (NoteSimple noteDie) {
        //flash 4 times
        for (int i = 0; i < 8; i++) {
            if (i % 2 == 0) {
                noteDie.gameObject.SetActive(true);
            }
            else {
                noteDie.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(0.1f);
        }
        noteDie.gameObject.SetActive(true);
    }

    /// <summary>
    /// Check if this note can register touch or not
    /// </summary>
    /// <param name="note">The note to check</param>
    /// <returns>Is the specified note eligible for touching</returns>
    private bool CanTouchNote (NoteSimple note) {
        for (int i = 0; i < listNoteActive.Count; i++) {
            //get the last clickable note
            if (listNoteActive[i].IsClickable()) {
                //if that is the same note as we are checking, return true
                if (listNoteActive[i] == note) {
                    return true;
                }
                else {
                    //or else, continue to check
                    //if the lowest note is a dual note
                    if (listNoteActive[i].data.subType == TileType.Dual) {
                        //special case, if i == 0
                        if (i == 0) {
                            //only check for the next tile, since there are no tile before
                            if (listNoteActive[1] == note) {
                                return true;
                            }

                            return false;
                        }
                        //special case, if i is at the end of the list
                        else if (i == listNoteActive.Count - 1) {
                            //only check for the previous tile since there are no tile after
                            if (listNoteActive[listNoteActive.Count - 2] == note) {
                                return true;
                            }

                            return false;
                        }
                        else {
                            //if the note right before or after this clickable note is the one we are checking, return true
                            if (listNoteActive[i - 1] == note ||
                                listNoteActive[i + 1] == note) {
                                return true;
                            }
                        }

                        //all else, false
                        return false;
                    }
                    else {
                        return false;
                    }
                }
            }
        }
        return false;
    }

    private NoteSimple GetLastNoteCanTouch () {
        for (int i = 0; i < listNoteActive.Count; i++) {
            if (listNoteActive[i].IsClickable()) {
                return listNoteActive[i];
            }
        }
        return null;
    }
}

