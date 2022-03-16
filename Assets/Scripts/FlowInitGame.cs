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

    public LevelLoader _LevelLoader;
    public TileMasterGamePlay gamelogic;
    private LevelDataModel levelData;

    public int IndexSong = 0;

    protected void ChooseSong(int indexSong)
    {
        LevelDataModel level = new LevelDataModel();
        indexSong = Math.Min(indexSong, _GameManager.StoreData.listAllSongs.Count - 1);
        level.songData = _GameManager.StoreData.listAllSongs[indexSong];
        GameManager.Instance.SessionData.currentLevel = level;
        levelData = GameManager.Instance.SessionData.currentLevel;
    }
    
    [ContextMenu("StartGame")]
    public void StartGame()
    {
        // _MainGameSceneController.gameObject.SetActive(true);
        this.IndexSong = Math.Min(IndexSong, _GameManager.StoreData.listAllSongs.Count - 1);
        ChooseSong(this.IndexSong);
        
        InitController();
        
        if (gamelogic.CachedLevelData == null || !gamelogic.CachedLevelData.songData.name.Equals(levelData.songData.name)) 
        {
            C_StartGame();
        }
        else 
        {
            StartNewGame();
        }
        // _MainGameSceneController.SetupGameStart();
    }

    internal void StartNewGame()
    {
        LivesManager.Instance.ReduceLife();

        gamelogic.PrepareNewGame();
        InGameUIController.Instance.gameStarted = true;
        _LevelLoader.gameObject.SetActive(false);

    }

    protected void InitLevelLoader()
    {
        _LevelLoader.OnSongParsed -= OnSongParsed;
        _LevelLoader.OnSongParsed += OnSongParsed;
    }
    protected void InitGameLogicController()
    {
        gamelogic.OnGameOver -= OnGameOver;
        gamelogic.OnGameOver += OnGameOver;
    }
    public void InitController()
    {
        InitLevelLoader();
        InitGameLogicController();
    }
    private void OnGameOver () 
    {
        return;
    }
    
    public void OnEnable()
    {
        InitFlow();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            // StartGame();
            gamelogic.StartGame();
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            gamelogic.isListenThisSong = !gamelogic.isListenThisSong;
            gamelogic.StartGame();
        }

    }

    public void InitFlow()
    {
        _GameInitializer.OnInitCompleted = initializer => StartGame();
        _GameInitializer.InitAllGameData();   
    }

    void C_StartGame () {
        //try to read local song files first
        SongTileData tileData = SaveBinarySongDataSystem.LoadTileDataFromResource(levelData.songData.storeID);

        if (tileData == null) {
            //if there is no local song, download new file from the internet
            //Debug.Log("Trying to download from the Internet");
            _LevelLoader.DownLoadAndCacheSongData(levelData.songData);
        }
        else {
            //if local song persist, check its version with data from store data
            if (tileData.songDataModel.version != levelData.songData.version) {
                //if the version is not alike, download from the internet
                _LevelLoader.DownLoadAndCacheSongData(levelData.songData);
            }
            else {
                Resources.UnloadUnusedAssets();

                //if local version is the latest, load it up and run the game
                gamelogic.Initialize();
                gamelogic.SetLevelData(levelData);
                gamelogic.SetTilesData(tileData);

                StartNewGame();
            }
        }
    }
    
    private void OnSongParsed (bool isSucceed, LevelDataModel levelData) {
        if (isSucceed) {
            this.levelData.playbackData = levelData.playbackData;
            //sort playback data by time appear
            this.levelData.playbackData.Sort((x, y) => (x.timeAppear.CompareTo(y.timeAppear)));

            this.levelData.noteData = levelData.noteData;
            //sort note data by time appear
            this.levelData.noteData.Sort((x, y) => (x.timeAppear.CompareTo(y.timeAppear)));

            this.levelData.BPM = levelData.BPM;
            this.levelData.tickPerQuarterNote = levelData.tickPerQuarterNote;

            this.levelData.denominator = levelData.denominator;

            //cache level
            GameManager.Instance.SessionData.currentLevel = this.levelData;
            gamelogic.Initialize();
            gamelogic.SetLevelData(this.levelData);

            gamelogic.PrepareTileData(
                //on progressing
                (progress) => {

                },

                //on complete
                () =>
                {
                    StartNewGame();
                },

                //on Error
                (errorMessage) => {
                    Debug.LogError("Error trying to parse song data: " + errorMessage);
                }
            );
        }
    }

    
    private void _initConfigCompleted(GameInitializer obj)
    {
        StartGame();
    }
}
