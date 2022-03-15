using UnityEngine;
using Mio.Utils;
using MovementEffects;
using System.Collections.Generic;
using Mio.Utils.MessageBus;
using ProjectConstants;

namespace Mio.TileMaster {
    //Hear it lay, the start of every thing
   //      .--,       .--,
   // ( (  \.---./  ) )
   //  '.__/o   o\__.'
   //     {=  ^  =}
   //      >  -  <
   //     /       \
   //    //       \\
   //   //|   .   |\\
   //   "'\       /'"_.-~^`'-.
   //      \  _  /--'         `
   //    ___)( )(___
   //   (((__) (__)))
    public class GameInitializer : MonoBehaviour
    {
        private enum InitializeState
        {
            GameStarted,
            GameVersions,
            GameConfigs,
            GameLocalization,
            UserData,
            AchievementData,
            GameStates,
            GameStoreData
        }
        private const int NUM_INITIALIZE_STEP = 8;
        public readonly string[] SUPPORTED_LANGUAGES = { "English", "Japanese", "Vietnamese" };
        
        private bool[] gamestepInitialized;

        protected bool _IsInitializing = false;
        public bool Initted = false;

        public System.Action<GameInitializer> OnInitCompleted = null;
        public void InitAllGameData()
        {
            if (_IsInitializing || Initted)
                return;
            _IsInitializing = true;

            DG.Tweening.DOTween.Init();
            MessageBus.Instance.Initialize();
            //splashScreen.OnLanguageConfirmed += OnLanguageConfirmed;
            // splashScreen.Initialize();

            GameManager.Instance.GetPathLocalConfig();
            GameManager.Instance.LoadLocalGameData();
            GameManager.Instance.SaveGameData();
            //Timing.Instance.TimeBetweenSlowUpdateCalls = 1;
            //currentState = InitializeState.GameStarted;
            GameManager.Instance.Initialize();
            //SceneManager.Instance.Initialize();
            //GeoLocationManager.Instance.Initialize();
            MidiPlayer.Instance.Initialize();
            MidiPlayer.Instance.ShouldPlay = true;

            InitializeGame();
        }
        
        void Start()
        {
            InitAllGameData();
        }
        


        /// <summary>
        /// Start the initialization of the game
        /// </summary>
        private void InitializeGame()
        {
            //Loom.Instance.CheckInitial();
            gamestepInitialized = new bool[NUM_INITIALIZE_STEP];
            gamestepInitialized[(int)InitializeState.GameStarted] = true;
            //Debug.Log("Initializing game...");
            //Timing.RunCoroutine(C_InitializeGameVersions());

            Debug.Log("InitializedGame");
            Timing.RunCoroutine(C_InitializeGameSteps());
            //start coroutine checking the game and enter if all steps have been initialized
            Timing.RunCoroutine(C_EnterGameWhenReady());

            //make that bitch calculates its id
            string nothing = GameManager.Instance.DeviceID;
        }
        
        private IEnumerator<float> C_InitializeGameSteps()
        {
            // initialize game version first
            int stepGameVersion = (int)InitializeState.GameVersions;
            if (gamestepInitialized[stepGameVersion] != true)
            {
                Timing.RunCoroutine(C_InitializeGameVersions());
            }
            while (gamestepInitialized[stepGameVersion] != true)
            {
                yield return Timing.WaitForSeconds(0.3f);
                gamestepInitialized[stepGameVersion] = true;
            }

            //initialize game configs
            int stepGameConfig = (int)InitializeState.GameConfigs;
            if (gamestepInitialized[stepGameConfig] != true)
            {
                Timing.RunCoroutine(C_InitializeGameConfig());
                gamestepInitialized[stepGameConfig] = true;
            }
            while (gamestepInitialized[stepGameConfig] != true)
            {
                yield return Timing.WaitForSeconds(0.3f);
                gamestepInitialized[stepGameConfig] = true;
            }

            //initialize steps that can be run parallel
            int stepStoreData = (int)InitializeState.GameStoreData;
            if (gamestepInitialized[stepStoreData] != true)
            {
                Timing.RunCoroutine(C_InitializeStoreData());
                gamestepInitialized[stepStoreData] = true;
            }

            int stepAchievement = (int)InitializeState.AchievementData;
            if (gamestepInitialized[stepAchievement] != true)
            {
                gamestepInitialized[stepAchievement] = true;
            }

            int stepUserData = (int)InitializeState.UserData;
            if (gamestepInitialized[stepUserData] != true)
            {
                Timing.RunCoroutine(C_InitializeUserData());
                gamestepInitialized[stepUserData] = true;
            }

            int stepLocalization = (int)InitializeState.GameLocalization;
            if (gamestepInitialized[stepLocalization] != true)
            {
                Timing.RunCoroutine(C_InitializeGameLocalization());
                gamestepInitialized[stepLocalization] = true;
            }

            int stepGameState = (int)InitializeState.GameStates;
            if (gamestepInitialized[stepGameState] != true)
            {
                PrepareGameState();
                gamestepInitialized[stepGameState] = true;
            }
            
            Debug.Log("Init Config");
        }
        private IEnumerator<float> C_EnterGameWhenReady()
        {
            int i, count;
            bool shouldWait = true;
            while (shouldWait)
            {
                count = 0;
                //check and count all initialized step of the game 
                for (i = 0; i < NUM_INITIALIZE_STEP; i++)
                {
                    if (gamestepInitialized[i] == true)
                    {
                        ++count;
                    }
                }

                //splashScreen.UpdateLoadProgress(count * 1f / NUM_INITIALIZE_STEP);

                //if number of initialized step is equal to total step, meaning the game has been initialized, no need to wait any more
                if (count == NUM_INITIALIZE_STEP)
                {
                    shouldWait = false;
                }

                yield return Timing.WaitForSeconds(0.3f);
            }

            while (!MidiPlayer.Instance.IsInitialized)
            {
                yield return Timing.WaitForSeconds(0.2f);
            }
            
            Initted = true;

            OnInitCompleted?.Invoke(this);
        }
        IEnumerator<float> C_InitializeGameVersions()
        {
            //yield return Timing.WaitForSeconds(0.5f);
            yield return 0;
            GameManager.Instance.LoadLocalGameVersions();
            
            OnGameVersionsInitialized();
        }

        private void OnGameVersionsInitialized()
        {
            gamestepInitialized[(int)InitializeState.GameVersions] = true;
            //Timing.RunCoroutine(C_InitializeGameConfig());
        }

        IEnumerator<float> C_InitializeGameConfig()
        {
            GameManager.Instance.LoadGameConfigDataFromLocal();
            yield return Timing.WaitForSeconds(0.25f);
            OnGameConfigInitialized(false);
        }


        private void OnGameConfigInitialized(bool configChanged)
        {
            gamestepInitialized[(int)InitializeState.GameConfigs] = true;
        }

        private void OnGameConfigRefreshFailed(string errorMessage)
        {
            Debug.LogError("Error downloading game config data " + errorMessage);
            return;
        }

        IEnumerator<float> C_InitializeGameLocalization()
        {
            //currentState = InitializeState.GameLocalization;
            //Debug.Log("Initializing game localization...");
            yield return Timing.WaitForSeconds(0.1f);
            
        }

        private void OnGameLocalizationDataInitialized()
        {
            //Localization.loadFunction = LoadLocalizationData;
            //byte[] localizeData = System.Text.Encoding.Unicode.GetBytes(GameManager.Instance.LocalizationData);
            if (GameManager.Instance.LocalizationData != null)
            {
                Localization.LoadCSV(GameManager.Instance.LocalizationData, false);
                Localization.LoadAndSelect(Localization.language);
            }
            else
            {
                Debug.LogWarning("Can't load localization CSV");
            }
            //GameManager.Instance.SetupGameFont(Localization.language);
            gamestepInitialized[(int)InitializeState.GameLocalization] = true;
            //Timing.RunCoroutine(InitializeUserData());
        }

        private void OnGameLocalizationDataRefreshFailed(string errorMessage)
        {
            Debug.LogError("Error downloading game localization data: " + errorMessage);
            
            return;
        }
        
        IEnumerator<float> C_InitializeUserData()
        {
            ProfileHelper.Instance.Initialize();
            yield return Timing.WaitForSeconds(0.25f);
            //Debug.Log("Initializing user data...");
            ProfileHelper.Instance.InitializeUserData(OnUserDataInitialized);
        }

        private void OnUserDataInitialized(bool success)
        {
            if (success)
            {
                LivesManager.Instance.Initialize();
                HighScoreManager.Instance.Initialize();
                gamestepInitialized[(int)InitializeState.UserData] = true;

            }
            else
            {
                Debug.LogError("Error loading user data");
            }
        }
        
        public void PrepareGameState()
        {

            MidiPlayer.Instance.LoadSong(MidiPlayer.Instance.backgroundMidiFile.bytes);

            GameManager.Instance.SessionData = new SessionDataModel();
            gamestepInitialized[(int)InitializeState.GameStates] = true;
        }


        IEnumerator<float> C_InitializeStoreData()
        {
            GameManager.Instance.LoadStoreDataFromLocal();
            yield return Timing.WaitForSeconds(0.25f);
            OnStoreDataInitialized(false);
        }

        private void OnStoreDataInitialized(bool isHasChanges)
        {

            gamestepInitialized[(int)InitializeState.GameStoreData] = true;
        }
        

        public void RetryLastInitialization()
        {
            Timing.RunCoroutine(C_InitializeGameSteps());            
        }
    }
}