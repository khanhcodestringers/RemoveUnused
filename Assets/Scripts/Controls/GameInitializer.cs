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

        public SplashScreenController splashScreen;

        private bool[] gamestepInitialized;

        void Start()
        {
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
            SceneManager.Instance.Initialize();
            //GeoLocationManager.Instance.Initialize();
            MidiPlayer.Instance.Initialize();
            MidiPlayer.Instance.ShouldPlay = true;

            OnLanguageConfirmed();
        }

        private void ShowLanguageConfirmation()
        {
            return;
            
            string systemLanguage = (Application.systemLanguage.ToString());
            for (int i = 0; i < SUPPORTED_LANGUAGES.Length; i++)
            {
                if (systemLanguage.Contains(SUPPORTED_LANGUAGES[i]))
                {
                    splashScreen.ShowLanguageOptions(systemLanguage);
                    return;
                }
            }

            splashScreen.ShowLanguageOptions("English");
        }

        private void OnLanguageConfirmed()
        {
            //Debug.Log("Game is starting...");
            splashScreen.ShowLoading();
            InitializeGame();
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

            Timing.RunCoroutine(C_InitializeGameSteps());
            //start coroutine checking the game and enter if all steps have been initialized
            Timing.RunCoroutine(C_EnterGameWhenReady());

            //make that bitch calculates its id
            string nothing = GameManager.Instance.DeviceID;
        }

        /// <summary>
        /// Initialize each step of the game correctly
        /// </summary>
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
            }

            //initialize game configs
            int stepGameConfig = (int)InitializeState.GameConfigs;
            if (gamestepInitialized[stepGameConfig] != true)
            {
                Timing.RunCoroutine(C_InitializeGameConfig());
            }
            while (gamestepInitialized[stepGameConfig] != true)
            {
                yield return Timing.WaitForSeconds(0.3f);
            }

            //initialize steps that can be run parallel
            int stepStoreData = (int)InitializeState.GameStoreData;
            if (gamestepInitialized[stepStoreData] != true)
            {
                Timing.RunCoroutine(C_InitializeStoreData());
            }

            int stepAchievement = (int)InitializeState.AchievementData;
            if (gamestepInitialized[stepAchievement] != true)
            {
                Timing.RunCoroutine(C_InitializeAchievementData());
            }

            int stepUserData = (int)InitializeState.UserData;
            if (gamestepInitialized[stepUserData] != true)
            {
                Timing.RunCoroutine(C_InitializeUserData());
            }

            int stepLocalization = (int)InitializeState.GameLocalization;
            if (gamestepInitialized[stepLocalization] != true)
            {
                Timing.RunCoroutine(C_InitializeGameLocalization());
            }

            int stepGameState = (int)InitializeState.GameStates;
            if (gamestepInitialized[stepGameState] != true)
            {
                PrepareGameState();
            }
        }

        /// <summary>
        /// The sequence to wait and start game as necessary
        /// </summary>
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

            //restore achievement data here after the player profile has been initialized
            if (!string.IsNullOrEmpty(ProfileHelper.Instance.AchievementPropertiesCSV)
                && !string.IsNullOrEmpty(ProfileHelper.Instance.AchievementUnlockedAndClaimedCSV))
            {
                // AchievementHelper.Instance.RestoreAchievementDataFromDump(ProfileHelper.Instance.AchievementPropertiesCSV, ProfileHelper.Instance.AchievementUnlockedAndClaimedCSV);
            }

            while (!MidiPlayer.Instance.IsInitialized)
            {
                yield return Timing.WaitForSeconds(0.2f);
            }

            // splashScreen.EnterGame();
            SSSceneManager.Instance.LoadMenu(Scenes.MainMenu.GetName());
            SceneManager.Instance.OpenScene(Scenes.HomeUI);
        }

        /// <summary>
        /// Download and check local version of all download-able config files to see if there are anything need updated
        /// </summary>
        IEnumerator<float> C_InitializeGameVersions()
        {
            //print("Initializing version");
            yield return Timing.WaitForSeconds(0.5f);
            GameManager.Instance.LoadLocalGameVersions();
            //currentState = InitializeState.GameVersions;
            //Debug.Log("Initializing game version...");
            OnGameVersionsInitialized();
            //yield return Timing.WaitForSeconds(0.25f);
            //GameManager.Instance.RefreshGameVersionsData(OnGameVersionsInitialized, OnGameVersionRefreshFailed);
        }

        private void OnGameVersionsInitialized()
        {
            gamestepInitialized[(int)InitializeState.GameVersions] = true;
            //Timing.RunCoroutine(C_InitializeGameConfig());
        }

        private void OnGameVersionRefreshFailed(string errorMessage)
        {
            return;
            Debug.LogError("Error downloading game version data: " + errorMessage);
            splashScreen.ShowMessage(Localization.Get("er_download_version_failed"));
            splashScreen.ShowRetryButton();
        }

        /// <summary>
        /// Prepare latest available game config for this game
        /// </summary>
        IEnumerator<float> C_InitializeGameConfig()
        {
            //load old game config first 
            GameManager.Instance.LoadGameConfigDataFromLocal();
            //currentState = InitializeState.GameConfigs;
            yield return Timing.WaitForSeconds(0.25f);
            //Debug.Log("Initializing game config...");
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
            splashScreen.ShowMessage(Localization.Get("er_download_config_failed"));
            splashScreen.ShowRetryButton();
        }

        IEnumerator<float> C_InitializeGameLocalization()
        {
            //currentState = InitializeState.GameLocalization;
            //Debug.Log("Initializing game localization...");
            yield return Timing.WaitForSeconds(0.1f);
            GameManager.Instance.RefreshLocalizationData(OnGameLocalizationDataInitialized, OnGameLocalizationDataRefreshFailed);
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
            splashScreen.ShowMessage(Localization.Get("er_download_localization_failed"));
            splashScreen.ShowRetryButton();
        }

        IEnumerator<float> C_InitializeAchievementData()
        {
            //yield return Timing.WaitForSeconds(0.5f);
            //Debug.Log("Initializing achievement data");
            GameManager.Instance.LoadLocalAchievementData();
            //currentState = InitializeState.AchievementData;
            yield return Timing.WaitForSeconds(0.25f);
            //Debug.Log("Initializing Achievement data...");
            //            LivesManager.Instance.Initialize();
            GameManager.Instance.RefreshAchievementData(OnAchivementInitialized, OnAchievementRefreshFailed);
        }

        private void OnAchievementRefreshFailed(string obj)
        {
            return;
            splashScreen.ShowMessage(Localization.Get("er_download_ach_failed"));
            splashScreen.ShowRetryButton();
        }

        private void OnAchivementInitialized()
        {
            gamestepInitialized[(int)InitializeState.AchievementData] = true;
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
                splashScreen.ShowMessage(Localization.Get("er_download_userdata_failed"));
                splashScreen.ShowRetryButton();
            }
        }
        
        public void PrepareGameState()
        {

            MidiPlayer.Instance.LoadSong(MidiPlayer.Instance.backgroundMidiFile.bytes);

            GameManager.Instance.SessionData = new SessionDataModel();
            gamestepInitialized[(int)InitializeState.GameStates] = true;
        }


        /// <summary>
        /// Prepare latest available store data for this game
        /// </summary>
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


        private void OnStoreDataRefreshFailed(string errorMessage)
        {
            Debug.LogError("Error downloading store data " + errorMessage);
            return;
            splashScreen.ShowMessage(Localization.Get("er_download_storedata_failed"));
            splashScreen.ShowRetryButton();
        }

        private void InitializePushNotificationService()
        {
//            if (string.IsNullOrEmpty(GameManager.Instance.GameConfigs.onesignal_app_id))
//            {
//                Debug.LogError("Missing onesignal_app_id for Onesignal Init");
//                return;
//            }
//#if UNITY_IOS
//			OneSignal.Init(GameManager.Instance.GameConfigs.onesignal_app_id, null);
//#else
//            // set up for android 
//            if (string.IsNullOrEmpty(GameManager.Instance.GameConfigs.google_project_id))
//            {
//                Debug.LogError("Missing google_project_id for Onesignal Init");
//                return;
//            }
//            OneSignal.Init(GameManager.Instance.GameConfigs.onesignal_app_id, GameManager.Instance.GameConfigs.google_project_id);
//#endif

        }

        private void InitializeIAPService()
        {
#if !UNITY_EDITOR
#if UNITY_IOS || UNITY_ANDROID
            IAPManager.Instance.Initialize();
#endif
#endif
        }

        /// <summary>
        /// Button retry clicked
        /// </summary>
        public void RetryLastInitialization()
        {
            splashScreen.HideMessage();
            splashScreen.HideRetryButton();

            Timing.RunCoroutine(C_InitializeGameSteps());            
        }
    }
}