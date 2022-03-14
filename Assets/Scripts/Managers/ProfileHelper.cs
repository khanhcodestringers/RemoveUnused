using UnityEngine;
using System.Collections;
using Mio.Utils.MessageBus;
using System;
//using Parse;
//using Facebook.Unity;
//using System.Threading.Tasks;
using Mio.Utils;
using System.Collections.Generic;
//using UnityParseHelpers;
using MovementEffects;

namespace Mio.TileMaster {
    //Well, this here, my friend, is where my hope end. But don't give up, since a nice girl is cheering for you
    //                  .-'-'`-.               .-'`-`-. 
    //               .-'-       \             /       -`-. 
    //              /    -.    - \           / -    .-    \ 
    //             /_/___ __.'-\  \        /  /-`.__ ___\_\ 
    //            /./ \_ \___ / \  |      |  / \ ___/ _/ \.\ 
    //            \/      /_ \  /  |      |  \  / _\      \/ 
    //             |  __\/   /./   |      |   \.\   \/__  | 
    //             \  \_\  .'      |      |      `.  /_/  / 
    //           _/ \____.'       /        \       `.____/ \_ 
    //         /'        \      /'|        |`\      /        `\ 
    //        |           \    /  |        |  \    /           | 
    //        \/           \  \   |        |   /  /           \/ 
    //       /              `\ \   \      /   / /'              \ 
    //      /                 | \  |      |  / |                 \ 
    //     /   /              \    |      |    /              \   \ 
    //  .-'  ,'       \        \   \      /   /        /       `,  `-. 
    // |'  ./          `\       \   |    |   /       /'          \.  `| 
    // \   |'            \      `   |    |   '      /            `|   / 
    //  `-/\     ./      |`\     \  |    |  /     /'|      \.     /\-' 
    //    ||`---'        |  \     . |    | .     /  |        `---'|| 
    //    \\             /   \    | \    / |    /   \             // 
    //     ||           /\_   \    \|    |/    /   _/\           || 
    //     \ \         /|  `\_|\    '    `    /|_/'  |\         / / 
    //      | `\        |      |    \    /    |      |        /' | 
    //      |   \       |      |     \  /     |      |       /   | 
    //      |'  |        \      \    |  |    /      /        |  `| 
    //      \   |         `-.    \   |  |   /    .-'         |   / 
    //-----._\__._           \   |   |  |   |   /           _.__/_.----- 
    //            `-.         |  |   |  |   |  |         .-' 
    //                        |  \   |  |   /  | 
    //                        |  _|  |  |  |_  | 
    //                        / /||..|  |..||\ \ 
    //     :F_P:            .' |/||||/  \||||\| `.            :F_P: 
    //    ---._________.---'   ` \|''    ``|/ '   `---._________.--- 
    public class ProfileHelper : MonoSingleton<ProfileHelper> {
        private static Message MSG_DIAMOND_CHANGED = new Message(MessageBusType.DiamondChanged);
        private static Message MSG_LIFE_CHANGED = new Message(MessageBusType.LifeChanged);
        private static string currentDevice;

        private UserDataModel rawUserData;
        private UserDataModel RawUserData {
            get { return rawUserData; }
            set {
                rawUserData = value;
                if (value != null) {
                    isUserDataInitialized = true;
                }
            }
        }

        //private CloudUserDataModel userData;

        private bool isInit = false;
        private bool isUserDataInitialized = false;


        //used to delay push operation, avoid overload on server
        //private static float lastPushTime = -30;
        //private static float delayBetweenPush = 15;
        private bool isUserDatachanged = false;
        private Timing timer;

        public void Initialize (Action<bool> OnInitializeCompleted = null) {
            currentDevice = GameManager.Instance.DeviceID;            
            MessageBus.Instance.Subscribe(MessageBusType.AchievementDataChanged, OnAchivementDataChanged);
            Helpers.CallbackWithValue(OnInitializeCompleted, true);
        }


        //private int currentCount = 0;
        IEnumerator<float> CheckPushUserData () {
            while (true) {
                //print("Checking");
                yield return 0;
                if (isUserDatachanged) {
                    //only proceed if user is not playing game or initializing game to prevent hiccup
                    if (SceneManager.Instance.CurrentScene != ProjectConstants.Scenes.MainGame
                        && SceneManager.Instance.CurrentScene != ProjectConstants.Scenes.SplashScreen) {
                        //MessageBus.Annouce(MSG_USERDATA_CHANGED);
                        PushUserData(true);
                        isUserDatachanged = false;
                    }
                }
            }
        }

        private void OnAchivementDataChanged (Message msg) {
            if (rawUserData != null) {
                isUserDatachanged = true;
            }
        }

        public void SaveLocalUserData () {            
            RawUserData.lastsyncDevice = currentDevice;
            GameManager.Instance.SaveUserData();
        }

        public void LoadLocalUserData () {
            //if (ParseUser.CurrentUser != null) {
            //    if (ParseUser.CurrentUser.ContainsKey(GameConsts.PK_DATA)) {
            //        string userData = ParseUser.CurrentUser[GameConsts.PK_DATA] as string;
            //        //GameManager.Instance.LoadLocalUserData();
            //        //print("Loading user data: \n\n" + userData);
            //        DeserializeUserData(userData);
            //    }
            //    else {
            //        RawUserData = PrepareNewUserData();
            //    }
            //}
            //else {
            //    Debug.LogWarning("Parse User is null. Maybe an error. Using new player data");
            //    RegisterOrLoginParse(currentDevice, currentDevice);
            //    RawUserData = PrepareNewUserData();
            //}
            //DeserializeUserData(userData);
        }

        public void IncreaseDataVersion () {
            RawUserData.lastsyncDevice = currentDevice;
            RawUserData.dataVersion += 1;
        }

        /// <summary>
        /// Push user data from local device onto cloud
        /// </summary>
        /// <param name="force">Bypass caching, and immidiately push user data onto cloud</param>
        public void PushUserData (bool force = false) {
            if (rawUserData == null) {
                Debug.LogWarning("Trying to push null data. No-go.");
                return;
            }

            isUserDatachanged = false;

            SaveLocalUserData();
        }

        public bool IsUserDataInitialized {
            get { return isUserDataInitialized; }
        }

        internal bool HasConnectedFacebook () {
            return rawUserData.isLoggedInFacebook;
        }

        /// <summary>
        /// Load user data from local storage
        /// </summary>
        /// <param name="OnInitializeCompleted"></param>
        public void InitializeUserData (Action<bool> OnInitializeCompleted) {
            GameManager.Instance.LoadLocalUserData();

            isInit = true;
            Helpers.CallbackWithValue(OnInitializeCompleted, true);
        }             

        /// <summary>
        /// Convert current user data into a json object for saving purpose
        /// </summary>
        public string SerializedUserData () {
            return GetJSONString(rawUserData);
        }

        /// <summary>
        /// Parse user data from a previous backup json object
        /// </summary>
        public void DeserializeUserData (string json) {
            //initialize user data if input string is empty
            if (string.IsNullOrEmpty(json)) {
                //print("Null JSON, creating new user");
                RawUserData = PrepareNewUserData();
            }
            else {
                //print(json);
                UserDataModel data = ParseUserDataModel(json);
                if (data == null) {
                    Debug.LogError("Could not de-serialize user data...");
                    //rawUserData = PrepareNewUserData();
                }
                else {
                    //print("Successfully parsed user data");
                    RawUserData = data;
                }
            }
        }

        private UserDataModel PrepareNewUserData () {
            UserDataModel ud = new UserDataModel();
            ud.life = MaxLife;
            ud.diamond = 20;

            ud.listHighscore = new List<ScoreItemModel>();
            ud.lastsyncDevice = currentDevice;
            ud.listBoughtSongs = new List<string>();
            return ud;
        }
        private string GetJSONString (UserDataModel data) {
            string json = JsonUtility.ToJson(data);
            return json;
        }

        private UserDataModel ParseUserDataModel (string json) {
            UserDataModel data = new UserDataModel();
            if (!string.IsNullOrEmpty(json)) {
                data = JsonUtility.FromJson<UserDataModel>(json);                
                return data;
            }
            else {
                data.listHighscore = new List<ScoreItemModel>(25);
                return data;
            }
        }

        #region Get User information
        //public int CurrentXP {
        //    get {
        //        return isInit ? rawUserData.xp : 0;
        //    }

        //    set {
        //        if (isInit) {
        //            rawUserData.xp = value;
        //        }
        //    }
        //}

        //public int CurrentPlayerLevel {
        //    get {
        //        return isInit ? rawUserData.level : 0;
        //    }

        //    set {
        //        if (isInit) {
        //            rawUserData.level = value;
        //        }
        //    }
        //}

        public long TimeToResumeAds {
            get { return rawUserData.timeToResumeAds; }
            set {
                rawUserData.timeToResumeAds = value;
                isUserDatachanged = true;
            }
        }
        //public int MaxPlayerXP {
        //    get;
        //    private set;
        //}
        public List<string> ListBoughtSongs {
            get { return rawUserData.listBoughtSongs; }
        }


        public double LastLifeCountdownUnixTimeStamp {
            get { return isInit ? rawUserData.timeStartCountdown : 0; }
            set {
                if (isInit) {
                    if (rawUserData.timeStartCountdown != value) {
                        rawUserData.timeStartCountdown = value;
                        isUserDatachanged = true;
                    }
                }
            }
        }

        private int oldLife = 0;
        public int CurrentLife {
            get {
                return isInit ? rawUserData.life : 0;
            }

            set {
                if (isInit) {
                    oldLife = rawUserData.life;
                    rawUserData.life = value;
                    if (oldLife != value) {
                        //print("Setting lives to " + value);
                        isUserDatachanged = true;
                        MessageBus.Annouce(MSG_LIFE_CHANGED);
                    }
                    //LivesManager.Instance.SetLife(value, false);
                }
            }
        }

        public int MaxLife {
            get {
                if (isInit) {
                    return rawUserData.isLoggedInFacebook ? GameManager.Instance.GameConfigs.maxLifeLinkedUser : GameManager.Instance.GameConfigs.maxLifeGuestUser;
                }
                else {
                    return 20;
                }
            }
        }

        private int oldDiamond = 0;
        public int CurrentDiamond {
            get {
                return isInit ? rawUserData.diamond : 0;
            }

            set {
                if (isInit) {
                    oldDiamond = rawUserData.diamond;
                    rawUserData.diamond = value;
                    if (oldDiamond != value) {
                        //print("Saving user data after changing diamond");
                        if (SceneManager.Instance.CurrentScene != ProjectConstants.Scenes.MainGame) {
                            SaveLocalUserData();
                        }
                        isUserDatachanged = true;
                        MessageBus.Annouce(MSG_DIAMOND_CHANGED);
                    }
                }
            }
        }

        public string AchievementPropertiesCSV {
            get {
                return isInit ? rawUserData.achievementPropertiesValueCSV : "";
            }

            set {
                if (isInit) {
                    rawUserData.achievementPropertiesValueCSV = value;
                }
            }
        }

        public string AchievementUnlockedAndClaimedCSV {
            get {
                return isInit ? rawUserData.achievementUnlockedAndClaimedCSV : "";
            }

            set {
                if (isInit) {
                    rawUserData.achievementUnlockedAndClaimedCSV = value;
                }
            }
        }

        public List<ScoreItemModel> ListHighScore {
            get {
                return isInit ? rawUserData.listHighscore : null;
            }

            set {
                if (isInit) {
                    rawUserData.listHighscore = value;
                }
            }
        }

        public int TotalStars {
            get {
                return HighScoreManager.Instance.TotalStars;
            }
        }
        public int TotalCrowns {
            get {
                return HighScoreManager.Instance.TotalCrowns;
            }
        }

        public bool IsLoggedInFacebook {
            get {
                return false;                
            }
        }

        
        #endregion
        
    }
}