using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Mio.TileMaster {
    [System.Serializable]
    public class GameConfigModel {
        // config file version
        public int configVersion;

        // Config for maximum user's life
		public long TIME_DISCONNECT_ALLOW=3600;//1h
		//server realtime URL
		public string ONLINE_URL_SERVER = "";
        public int maxLifeLinkedUser;
        public int maxLifeGuestUser;
        // how many seconds needed to elapse to increase life?
        public int secondsPerLife;


        //time per session
        public int sessionTime = 30;


        public int rubyForOnline = 2;

        public float pecentRewardOnline = 1;
        // Push Notification IDs
        public string onesignal_app_id;
        public string google_project_id;

        // Game Center and Google Play Game ID
        //public string gamecenter_id;
        //public string googleplaygame_id;

        // Admob Id
        //public string admob_ios;
        //public string admob_android;

        //Mopub ID
        //public string mopub_ios;
        //public string mopub_android;
        public List<string> mopub_ids;
        public List<string> mopub_reward_ids;
        public float adsInterval;
        //public string unityads_zoneid;
        public List<bool> priorityMopub;
        public List<string> nativeAdsSlot1;
        public List<string> nativeAdsSlot2;


        public List<int> indexNativeAds;
        //public List<string> nativead_idsAndroid = new List<string> { "979269728858634_1074602419325364", "979269728858634_1074602495992023" };
        //public List<string> nativead_idsIOS = new List<string> { "979269728858634_1074602592658680", "979269728858634_1074602699325336" };

		//on/off online
		public bool flagOnline = true;
        //public string nativead_ios;
        //public string nativead_android;

        //unity ads
        public int videoAdsReward;
        //public List<string> unityads_ids;

        //in app purchase value
        public List<int> iap_values;
        public List<string> iap_prices;

        //life exchange value
        public List<int> lives_exchange_values;
        public List<int> lives_exchange_prices;

        //ad interval
        //public List<float> adsInterval;

        //public List<string> adsIntervalID;
        // Google analytics
        //public string ga_id_ios;
        //public string ga_id_android;
        //public string ga_id_others;
        public List<string> ga_ids;

        //Cross#promotion fullscreen ads
        public List<bool> showFullscreenAds;
        public List<string> adsMessages;
        public List<string> adsImages;
        public List<string> adsURL;

        //public string levelTableURL;
        //public int levelTableVersion;
        public string eventsURL;
        // store data file URL
        public string storeDataURL;
        //URL of the csv file contains properties of achievement
        public string achievementPropertiesURL;
        //URL of the csv file contains definition of achievements in game
        public string achievementURL;
        public string localizationURL;
        
        /// IN#GAME CONFIG 
        /// 
        //Price for continue game
        public int maxDiamondPerGame;
        public float diamondChance;
        public int maxContinuePerGame;
        public int startingPriceToContinue;
        //at which point the game start spawning diamonds
        public int scoreToDropDiamond;
        public float speedToDropDiamond;
		public float TIME_SYNC_LIMIT=0.1f;
        public bool onlineChallenge = true;

        //daily reward
        public List<int> dailyrewardLives;
        public List<int> dailyrewardDiamonds;

        //speed 
        public List<float> defaultSpeedTable;

        // show hide and Tips description
        public bool enableTips;
        public List<string> tipsKey;

        public int ratingCondition;
        public List<string> urlCrossPromotion;

		//public List<string> crossPromotionIngame = new List<string>{"Top Billboard#236#235#234#http://m.onelink.me/7e3e70ad#http://m.onelink.me/7e3e70ad#http://m.onelink.me/7e3e70ad#1#1",
		//	"Magic Touch#237#235#234#https://play.google.com/store/apps/details?id=com.musicheroesrevenge.magicnotes#https://itunes.apple.com/us/app/magic-touch-piano-rhythm/id1051575834?mt=8#https://s3.amazonaws.com/pianochallengeproduction/icon/MN.png#0#0","Just1Tap#238#235#234#http://m.onelink.me/7e3e70ad#https://itunes.apple.com/app/id1160137878#https://s3.amazonaws.com/pianochallengeproduction/icon/J1T.png#1#0",
		//	"PianoIdol#239#235#234#http://m.onelink.me/519fb239#http://m.onelink.me/8512ce44#https://s3.amazonaws.com/pianochallengeproduction/icon/PI.png#0#0"
		//};
        public List<string> crossPromotionIngame;

        public string crossDefault;
        //public string crossDefault = "PianoIdol#239#235#234#http://m.onelink.me/519fb239#http://m.onelink.me/8512ce44#https://s3.amazonaws.com/pianochallengeproduction/icon/PI.png#0#0";
        public bool enableNativeAdsHomeScreen = false;

        //[Header("Playerpref keys")]
        //public string KEY_GAME_VERSION = "GameVersion";
        //public string KEY_GAME_DATA_VERSION = "GameDataVersion";
        //public string KEY_GAME_NUM_START = "NumStart";

        //[Header("Saved data keys")]
        //public string KEY_GAME_LEVEL_PLAYED = "LevelPlayed";
        //public string KEY_GAME_OPTION_CONFIG = "OptionConfig";

        //[Header("Profile keys")]
        //public string KEY_PROFILE_LEVEL_UP_INFO_CLASS = "LevelUpData";
        //public string KEY_PROFILE_LEVEL_NUMBER = "levelNumb";
        //public string KEY_PROFILE_XP_TO_COMPLETE = "xpToComplete";
        //public string KEY_PROFILE_USER_CLASS = "UserProfile";
        //public string KEY_PROFILE_USER_ID = "userId";
        //public string KEY_PROFILE_FACEBOOK_ID = "facebookId";
        //public string KEY_PROFILE_ITEMS_PURCHASE = "itemsPurchase";
        //public string KEY_PROFILE_COIN = "coin";
        //public string KEY_PROFILE_RUBY = "ruby";
        //public string KEY_PROFILE_LEVEL = "level";
        //public string KEY_PROFILE_INFO = "info";
        //public string KEY_PROFILE_INFO_DEVIDE_NAME = "devideName";
        //public string KEY_PROFILE_INFO_SYSTEM_NAME = "systemName";

        //public string KEY_PROFILE_AVATAR_PICTURE_NAME = "AvatarPicture";
        //public string KEY_PROFILE_DATA_LOCAL_FILE_NAME = "UserProfileData";
        //public string KEY_PROFILE_DATA_FIELD_NAME = "data";
        //public string KEY_GAME_IS_OFFLINE = "isOffline";
        //public string KEY_PROFILE_LOGIN_TIME = "loginTime";
        //public string KEY_USER_SESSION_COUNTER = "sessionCounter";
    }
}