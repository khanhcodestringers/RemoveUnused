using System;
using UnityEngine;
using Mio.Utils;
using MovementEffects;
using System.Collections.Generic;
using Mio.Utils.MessageBus;
using ProjectConstants;
using Mio.TileMaster;

public class FlowInitGame : MonoBehaviour
{
    public GameInitializer _GameInitializer;
    public GameManager _GameManager;

    public MainGameSceneController _MainGameSceneController;
    public int IndexSong = 0;

    protected void ChooseSong(int indexSong)
    {
        LevelDataModel level = new LevelDataModel();
        level.songData = _GameManager.StoreData.listAllSongs[indexSong];
        GameManager.Instance.SessionData.currentLevel = level;

    }
    
    [ContextMenu("StartGame")]
    public void StartGame()
    {
        _MainGameSceneController.gameObject.SetActive(true);
        ChooseSong(this.IndexSong);
        _MainGameSceneController.SetupGameStart();
    }

    public void OnEnable()
    {
        InitFlow();
    }

    public void InitFlow()
    {
        // _GameInitializer.OnInitCompleted += _initConfigCompleted;
        _GameInitializer.InitAllGameData();   
    }

    private void _initConfigCompleted(GameInitializer obj)
    {
        _MainGameSceneController.SetupGameStart();
    }
}
