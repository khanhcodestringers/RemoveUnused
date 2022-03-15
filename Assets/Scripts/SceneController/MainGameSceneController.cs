using UnityEngine;
using System.Collections;
using System;
using Mio.Utils;
//using UnityParseHelpers;
using System.Collections.Generic;
using MovementEffects;

namespace Mio.TileMaster {
    public class MainGameSceneController : SSController {
        private enum UIState {
            Initialized,
            Downloading,
            DownloadError,
            ParsingLevelData,
            ParseError,
            TransitioningToGamePlay
        };

        [SerializeField]
        private UIState currentState = UIState.Initialized;

        public LevelLoader levelLoader;
        private LevelDataModel levelData;

        public TileMasterGamePlay gamelogic;

        [Header("Elements to communicate with player")]
        public UISlider progressbar;
        public UILabel lbMessage;

        [Header("Elements to show after download failed")]
        public UIButton backButton;
        public UIButton retryButton;

        public List<GameObject> listToDisable;

        private string command;

        /// <summary>
        /// Will be called when opened with not-null parameter
        /// </summary>
        /// <param name="data"></param>
        public override void OnSet (object data) {
            if (data != null) {
                command = (string)data;

                //if the game scene is brought up with additional command, check it
                if (command.Contains("replay")) {
                    
                }
                else if (command.Contains("continue")) {
                    ContinueGame();
                }

                command = string.Empty;
            }
        }

        protected void InitLevelLoader()
        {
            levelLoader.OnSongParsed -= OnSongParsed;
            levelLoader.OnSongParsed += OnSongParsed;
            levelLoader.OnDownloadSongError -= OnSongDownloadError;
            levelLoader.OnDownloadSongError += OnSongDownloadError;
            levelLoader.OnDownloadSongProgress -= OnSongDownloading;
            levelLoader.OnDownloadSongProgress += OnSongDownloading;
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

        public void SetupGameStart()
        {
            InitController();
            
            ChangeState(UIState.Initialized);
            //SceneManager.Instance.SetMenuVisible(false);
            levelLoader.gameObject.SetActive(true);

            LoadLevel();
            Timing.RunCoroutine(DisableGameObjects(0.1f));
        }
        
        public override void OnEnable () {
            
        }

        /// <summary>
        /// Disable specified game objects after some delay
        /// </summary>
        /// <param name="delay"></param>
        private IEnumerator<float> DisableGameObjects (float delay) {
            yield return Timing.WaitForSeconds(delay);
            for (int i = 0; i < listToDisable.Count; i++) {
                listToDisable[i].SetActive(false);
            }
        }

        public override void OnDisable () {
            levelLoader.OnDownloadSongProgress -= OnSongDownloading;
            levelLoader.OnSongParsed -= OnSongParsed;
            gamelogic.OnGameOver -= OnGameOver;
            base.OnDisable();
        }

        /// <summary>
        /// Prepare data and fire up the main game's UI
        /// </summary>
        private void LoadLevel () {
            levelData = GameManager.Instance.SessionData.currentLevel;

            //only try to load game if the level to load is different from cached, or there is no level being loaded
            if (gamelogic.CachedLevelData == null || !gamelogic.CachedLevelData.songData.name.Equals(levelData.songData.name)) {
                Timing.RunCoroutine(C_StartGame());
            }
            else {
                StartNewGame();
            }
        }

        /// <summary>
        /// Load up the level, and start the game as needed
        /// </summary>
        /// <returns></returns>
        IEnumerator<float> C_StartGame () {
            ChangeState(UIState.Downloading);
            //a little time to calm down
            yield return Timing.WaitForSeconds(0.1f);

            //try to read local song files first
            SongTileData tileData = SaveBinarySongDataSystem.LoadTileDataFromResource(levelData.songData.storeID);

            if (tileData == null) {
                //if there is no local song, download new file from the internet
                yield return 0;
                //Debug.Log("Trying to download from the Internet");
                levelLoader.DownLoadAndCacheSongData(levelData.songData);
            }
            else {
                //if local song persist, check its version with data from store data
                if (tileData.songDataModel.version != levelData.songData.version) {
                    //if the version is not alike, download from the internet
                    yield return 0;
                    levelLoader.DownLoadAndCacheSongData(levelData.songData);
                }
                else {
                    Resources.UnloadUnusedAssets();
                    yield return Timing.WaitForSeconds(0.25f);
                    //if local version is the latest, load it up and run the game
                    gamelogic.Initialize();
                    yield return 0;
                    gamelogic.SetLevelData(levelData);
                    yield return 0;
                    gamelogic.SetTilesData(tileData);
                    yield return 0;
                    StartNewGame();
                }
            }
        }

        private void OnSongDownloading (float progress) {
            progressbar.value = progress;
        }

        private void OnSongDownloadError (string obj) {
            Debug.LogWarning("Song download failed with error: " + obj);
            ChangeState(UIState.DownloadError);
        }

        internal void EndGame () {
            SceneManager.Instance.OpenScene(ProjectConstants.Scenes.ResultUI, this);
        }

        internal void StartNewGame () {
            if (currentState != UIState.TransitioningToGamePlay) {

                //1 life per game
                LivesManager.Instance.ReduceLife();

                //disable loom so that no GC will be alloc
                //Loom.Instance.SetEnable(false);
                
                ChangeState(UIState.TransitioningToGamePlay);

                gamelogic.PrepareNewGame();
                InGameUIController.Instance.gameStarted = true;
                //log achievement
                //AchievementHelper.Instance.LogAchievement("numTurnPlayed");
                levelLoader.gameObject.SetActive(false);
            }
        }

        private void OnGameOver () {
            
            Debug.LogError("EndGame");
            return;
            
            if (!gamelogic.isListenThisSong && gamelogic.CanContinue()) {
                SceneManager.Instance.OpenPopup(ProjectConstants.Scenes.ContinuePopup, this);
            }
            else {
                InGameUIController.Instance.gameStarted = false;
                EndGame();
            }
        }

        internal void ContinueGame (bool afterPause = true) {
            gamelogic.ContinueGame(afterPause);
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
                ChangeState(UIState.ParsingLevelData);

                gamelogic.PrepareTileData(
                    //on progressing
                    (progress) => {
                        progressbar.value = progress;
                    },

                    //on complete
                    () => {
                        progressbar.value = 1;
                        StartNewGame();
                    },

                    //on Error
                    (errorMessage) => {
                        Debug.LogError("Error trying to parse song data: " + errorMessage);
                        lbMessage.text = Localization.Get("er_parse_song_failed");
                        ChangeState(UIState.ParseError);
                    }
                    );
            }
        }

        //public void OnRetryButtonClicked () {
        //    //DownloadAndParseLevel();
        //   // LoadLevel();
        //}

        public void OnHomeButtonClicked () {
            SceneManager.Instance.OpenScene(ProjectConstants.Scenes.SongList);
        }

        private void ChangeState (UIState state) {
            currentState = state;
            RefreshUI(currentState);
        }

        private void RefreshUI (UIState state) {
            switch (state) {
                case UIState.Initialized:
                case UIState.Downloading: {
                        //hide unnecessary elements
                        backButton.gameObject.SetActive(false);
                        retryButton.gameObject.SetActive(false);
                        //show progress elements
                        progressbar.gameObject.SetActive(true);
                        progressbar.value = 0;
                        lbMessage.gameObject.SetActive(true);
                        //show message
                        lbMessage.text = Localization.Get("maingame_downloading");
                    }
                    break;

                case UIState.ParsingLevelData: {
                        //hide unnecessary elements
                        backButton.gameObject.SetActive(false);
                        retryButton.gameObject.SetActive(false);
                        //show progress elements
                        progressbar.gameObject.SetActive(true);
                        lbMessage.gameObject.SetActive(true);
                        //show message
                        lbMessage.text = Localization.Get("maingame_preparing");
                    }
                    break;

                //this state need user interaction, display any interactable elements for them
                case UIState.ParseError:
                case UIState.DownloadError: {
                        backButton.gameObject.SetActive(true);
                        retryButton.gameObject.SetActive(true);
                        lbMessage.gameObject.SetActive(true);
                        lbMessage.text = Localization.Get("er_download_song_failed");
                        //hide unnecessary elements
                        progressbar.gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }
}