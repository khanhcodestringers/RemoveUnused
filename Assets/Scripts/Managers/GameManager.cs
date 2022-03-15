using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mio.Utils;
using Mio.Utils.MessageBus;
using Mio.TileMaster;
//using Facebook.Unity;
//using Facebook.MiniJSON;
//using Parse;
//using Parse.Utilities;
//using UnityParseHelpers;

namespace Mio.TileMaster {
    /// <summary>
    /// Responsible for managing the game's state
    /// </summary>
    public class GameManager : MonoSingleton<GameManager> {
        #region Const variables
        private const string GAME_CONFIG_FILE = "config";
        private const string STORE_DATA_FILE = "store";
        private const string LOCALIZATION_DATA_FILE = "Localization";
        private const string GAME_VERSION_DATA_FILE = "version";
        private const string ACHIEVEMENT_FILE = "achievement";
        private const string ACHIEVEMENT_PROPERTY_FILE = "achievementProperties";
        private const string GAME_STATE_DATA_FILE = "gf0gf9h8";
        private const string USER_DATA_FILE = "lkh456";
        private const string ACHIEVEMENT_DATA_FILE = "544jk6kh7";
        private const string ACHIEVEMENT_DATA_DUMP_FILE = "k5h46kj56";
        private const string ACHIEVEMENT_PROPERTY_DATA_FILE = "kjh34jh5";
        private const string ACHIEVEMENT_PROPERTY_DATA_DUMP_FILE = "654gfh324dg";
        private static readonly string ENCRYPTION_KEY = string.Empty;//"jk65kj8hf290g93";
        #endregion

        private static Message msgGameDataChanged = new Message(MessageBusType.GameDataChanged);

        #region Public properties
        public bool printDebug = true;

        [Header("For multi-language support")]
        public UIFont referenceFontAtlas;
        [HideInInspector]
        public UIFont defaultFont;
        [HideInInspector]
        public UIFont japaneseFont;
        [HideInInspector]
        public UIFont koreanFont;


        [Header("Game's configuration URL")]
        [SerializeField]
        private string configURL;
        private GameConfigModel configs;
        public GameConfigModel GameConfigs {
            get { return configs; }
            private set {
                configs = value;
            }
        }

        private SessionDataModel sessionData;
        public SessionDataModel SessionData {
            get { return sessionData; }
            set { sessionData = value; }
        }

        //Store data        
        private StoreDataModel storeData;
        public StoreDataModel StoreData {
            get { return storeData; }
            set {
                storeData = value;
            }
        }


       
        private GameVersionModel versionsData;
        public GameVersionModel VersionsData {
            get { return versionsData; }
            private set { versionsData = value; }
        }

        private string deviceID;
        private bool deviceIDFetched = false;
        public string DeviceID {
            get {
                if (deviceIDFetched) {
                    return deviceID;
                }else {
                    deviceIDFetched = true;
#if UNITY_ANDROID
                    //deviceID = SystemInfo.deviceUniqueIdentifier;
                    AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
                    AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
                    deviceID = secure.CallStatic<string>("getString", contentResolver, "android_id");
#else
                    deviceID = SystemInfo.deviceUniqueIdentifier;
#endif
                    return deviceID;
                }
            }
        }

        private GameDataModel gameData;
        public GameDataModel GameData {
            get { return gameData; }
            set {
                gameData = value;
            }
        }

        public byte[] LocalizationData { get; internal set; }

        //private bool diamondChanged = false;

#endregion

        private string storedataWritePath, gamestateWritePath, gameconfigWritePath, gameversionWritePath, userdataWritePath,
            achievementPropertiesWritePath, achievementDataWritePath, achievementPropertiesDumpWritePath, achievementDataDumpWritePath;

        
        public void GetPathLocalConfig()
        {
            storedataWritePath = FileUtilities.GetWritablePath(STORE_DATA_FILE);
            gameconfigWritePath = FileUtilities.GetWritablePath(GAME_CONFIG_FILE);
            gamestateWritePath = FileUtilities.GetWritablePath(GAME_STATE_DATA_FILE);
            gameversionWritePath = FileUtilities.GetWritablePath(GAME_VERSION_DATA_FILE);
            userdataWritePath = FileUtilities.GetWritablePath(USER_DATA_FILE);
            achievementDataWritePath = FileUtilities.GetWritablePath(ACHIEVEMENT_DATA_FILE);
            achievementDataDumpWritePath = FileUtilities.GetWritablePath(ACHIEVEMENT_DATA_DUMP_FILE);
            achievementPropertiesWritePath = FileUtilities.GetWritablePath(ACHIEVEMENT_PROPERTY_DATA_FILE);
            achievementPropertiesDumpWritePath = FileUtilities.GetWritablePath(ACHIEVEMENT_PROPERTY_DATA_DUMP_FILE);
        }
        
        void Awake () {
            GetPathLocalConfig();
            
            //generate save, load path to reduce string generation
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
            //if (enableRemoteConsole) {
            //    remoteConsoleServer.StartServer();
            //}
        }
        

        void OnApplicationPause (bool state) {
            if (ProfileHelper.Instance.IsUserDataInitialized) {
                SaveUserData();
                ProfileHelper.Instance.PushUserData(true);
            }
        }

        public void Initialize () {
            //ParseUser.LogOut();
            MessageBus.Instance.Subscribe(MessageBusType.UserDataChanged, OnUserDataChanged);
            MessageBus.Instance.Subscribe(MessageBusType.UserDataConflictSolved, OnUserDataConflictSolved);
            MessageBus.Instance.Subscribe(MessageBusType.GameDataChanged, OnGameDataChanged);

            //well, this code is a hack, to reduce number of drawcall by 1 :v
            //remember, 1 camera = 1 drawcall
            var goSolidCam = GameObject.Find("SolidCamera");
            if (goSolidCam != null) {
                var solidCam = goSolidCam.GetComponent<Camera>();
                if (solidCam != null) {
                    solidCam.clearFlags = CameraClearFlags.Nothing;
                }
            }
            //FacebookManager.Instance.onSuccedInitFB += OnSuceedFB;
            //MessageBus.Instance.Subscribe(MessageBusType.DiamondChanged, OnDiamondBalanceChanged);
        }
        
        #region Message Handlers
        private void OnUserDataConflictSolved (Message obj) {
            
        }

        private void OnGameDataChanged (Message obj) {
            SaveGameData();
        }

        private void OnUserDataChanged (Message obj) {
            //print("Saving user data");
            SaveUserData(false);
        }
#endregion

#region Save / Load game's data
        public void SaveStoreData () {
            FileUtilities.SerializeObjectToFile<StoreDataModel>(storeData, storedataWritePath, ENCRYPTION_KEY, true);
        }

        public void LoadStoreDataFromLocal () {
            //storeData = FileUtilities.DeserializeObjectFromFile<StoreDataModel>(storedataWritePath, ENCRYPTION_KEY, true);
            //if (storeData == null) {
            //    storeData = new StoreDataModel();
            //}
            var t = Resources.Load<TextAsset>(STORE_DATA_FILE);
            if (t == null) {
                storeData = new StoreDataModel();
            }
            else {
                //Debug.Log(t.text);
                //storeData = FileUtilities.DeserializeObjectFromText<StoreDataModel>(t.text);
                string csv = t.text;
                //fsData jsonData = fsJsonParser.Parse(json);
                StoreDataModel currentStoreData = new StoreDataModel();
                //then de-serialize it for easier access                    
                bool success = ParseStoreData(csv, ref currentStoreData);

                if (!success) {
                    //if fail to de-serialize, we're doomed
                    Debug.LogWarning("Could not de-serialize store data");
                }
                else {
                    //store data has been parsed successfully, 
                    storeData = currentStoreData;
                    storeData.version = VersionsData.store;
                    //SaveStoreData();
                    //Debug.Log("New game store data loaded, version: " + currentStoreData.version);
                }
            }


        }

        public void SaveGameConfigData () {
            FileUtilities.SerializeObjectToFile<GameConfigModel>(configs, gameconfigWritePath, ENCRYPTION_KEY, true);
        }

        public void LoadGameConfigDataFromLocal () {
            //configs = FileUtilities.DeserializeObjectFromFile<GameConfigModel>(gameconfigWritePath, ENCRYPTION_KEY, true);
            //if (configs == null) {
            //    configs = new GameConfigModel();
            //}
            var t = Resources.Load<TextAsset>(GAME_CONFIG_FILE);
            if(t == null) {
                configs = new GameConfigModel();
            }
            else {
                configs = FileUtilities.DeserializeObjectFromText<GameConfigModel>(t.text);
            }
        }
        
        public void SaveGameData () {
            //this.Print("Saving game data...");
            FileUtilities.SerializeObjectToFile<GameDataModel>(gameData, gamestateWritePath, ENCRYPTION_KEY, true);
        }

        public void LoadLocalGameData () {
            //this.Print("Reading local game data ...");
            gameData = FileUtilities.DeserializeObjectFromFile<GameDataModel>(gamestateWritePath, ENCRYPTION_KEY, true);
            if (gameData == null) {
                this.Print("No local game data found, creating new one...");
                gameData = new GameDataModel();
                gameData.numStart = 0;
            }
            GameData.numStart += 1;

        }

        public void StoreLastPlayLevel () {
            //this.Print("Storing last level: " + SessionData.currentLevel.songData.storeID);
            SessionData.lastLevel = SessionData.currentLevel;
            GameData.lastPlaySongID = SessionData.currentLevel.songData.storeID;
            MessageBus.Annouce(msgGameDataChanged);
            //this.Print("Last song ID: " + GameData.lastPlaySongID);
        }

        private void SaveGameVersionsData () {
            //this.Print("Saving game versions' data...");
            FileUtilities.SerializeObjectToFile(versionsData, gameversionWritePath, ENCRYPTION_KEY, true);
        }

        internal void LoadLocalGameVersions () {
            var t = Resources.Load<TextAsset>(GAME_VERSION_DATA_FILE);
            if(t != null) {
                versionsData = FileUtilities.DeserializeObjectFromText<GameVersionModel>(t.text);
            }
            else {
                versionsData = new GameVersionModel();
            }
            //this.Print("Reading local game versions' data ...");
            //versionsData = FileUtilities.DeserializeObjectFromFile<GameVersionModel>(gameversionWritePath, ENCRYPTION_KEY, true);
        }


        public void SaveUserData (bool alsoSaveToParse = false) {
            //print("Saving user's data...");
            ProfileHelper.Instance.IncreaseDataVersion();
            //ProfileHelper.Instance.SaveLocalUserData();
            string json = ProfileHelper.Instance.SerializedUserData();
            //print("Saving user data " + json);

//#if !NETFX_CORE
            FileUtilities.SaveFileWithPassword(json, userdataWritePath, ENCRYPTION_KEY, true);
//#else
//            PlayerPrefs.SetString("ptnw", json);
//            PlayerPrefs.Save();
//#endif
        }

        internal void LoadLocalUserData () {

            string json;
            json = FileUtilities.LoadFileWithPassword(userdataWritePath, ENCRYPTION_KEY, true);
            ProfileHelper.Instance.DeserializeUserData(json);
        }
        #endregion

#region Download data from internet
private bool ParseStoreData (string csv, ref StoreDataModel storeData) {
            if (string.IsNullOrEmpty(csv)) {
                return false;
            }

            bool res = true;
            //StoreDataModel storeData = new StoreDataModel();
            var songs = new List<SongDataModel>(100);
            var listHot = new List<SongDataModel>(10);
            var listNew = new List<SongDataModel>(10);
            Dictionary<string, int> columnName = new Dictionary<string, int>(10);
            CSVReader.LoadFromString(
                csv,
                //LineReader
                (lineIndex, content) => {
                    if (lineIndex == 0) {
                        //parse header line, then, when parsing rows, instead of calling content[0], we can use content[columnName["ID"]]
                        //for the sake of easy understanding
                        for (int i = 0; i < content.Count; i++) {
                            columnName.Add(content[i], i);
                        }
                        //print("Header column has " + content.Count + " columns");
                    }
                    else {
                        //Debug.LogError(csv);
                        //print(string.Format("Checking content row {0}, num column {1}", lineIndex, content.Count));
                        if (content.Count == columnName.Count) {
                            //Column's name: ID,name,author,songURL,fileVersion,storeID,price,starsToUnlock,maxBPM,tickPerTile,speedModifier,speedPerStar,type
                            SongDataModel song = new SongDataModel();
                            song.ID = int.Parse(content[columnName["ID"]]);
                            song.name = content[columnName["name"]];
                            song.author = content[columnName["author"]];
                            song.songURL = content[columnName["songURL"]];
                            song.version = int.Parse(content[columnName["fileVersion"]]);
                            song.storeID = content[columnName["storeID"]];
                            song.pricePrimary = int.Parse(content[columnName["price"]]);
                            song.starsToUnlock = int.Parse(content[columnName["starsToUnlock"]]);
                            song.maxBPM = int.Parse(content[columnName["maxBPM"]]);
                            song.tickPerTile = int.Parse(content[columnName["tickPerTile"]]);
                            song.speedModifier = float.Parse(content[columnName["speedModifier"]]);
                            if (!string.IsNullOrEmpty(content[columnName["speedPerStar"]].ToString())) {

                            }
                            //song.speedPerStar = content.GetField<List<float>>( "HeaderName" );
                            song.type = (SongItemType)(int.Parse(content[columnName["type"]]));

                            if (song.type == SongItemType.Hot) {
                                listHot.Add(song);
                            }
                            else if (song.type == SongItemType.New) {
                                listNew.Add(song);
                            }

                            //print("Add song " + song.name);
                            songs.Add(song);
                        }
                    }
                }
            );

            storeData.listAllSongs = songs;
            storeData.listHotSongs = listHot;
            storeData.listNewSongs = listNew;
            return res;
        }
        #endregion
    }
}