using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;
using System;
using Mio.Utils;
using Mio.Utils.MessageBus;
using ProjectConstants;
//using Facebook.Unity;
//using Parse;

namespace Mio.TileMaster
{
    public class HomeSceneController : SSController
    {

        //public UILabel lbLastPlayedSong;
        [SerializeField]
        private LastPlayItem itemLastPlay;

        public List<GameObject> listHideInGame;

        [SerializeField]
        private UITable uiTable;

        [SerializeField]
        private SongItemView prefabSongItem;

        [SerializeField]
        private GameObject prefabEmpty;

        [SerializeField]
        private GameObject goLoginFacebook;

        [SerializeField]
        private GameObject goInviteFriend;

        [SerializeField]
        private UILabel lblDiamondReward;

        private List<SongItemView> songItems;

        //show or hide tips
        [SerializeField]
        private UILabel lbTips;
        [SerializeField]
        private GameObject TipsObj;


        private List<SongItemView> listHotSongItems;
        private List<SongItemView> listNewSongItems;
        private Transform songContainer;

        public Animator anmController;

        //[SerializeField]
        //private string crossPromotionLink = "https://www.microsoft.com/en-us/store/p/2-orb/9np5lsq70rnx";

        private bool shouldProcessSongItemClick = true;
        public bool ShouldProcessSongItemClick {
            get { return shouldProcessSongItemClick; }
            set {
                shouldProcessSongItemClick = value;
                //print("Setting should process song item click: " + value);
            }
        }

        public override void OnEnableFS()
        {
            base.OnEnableFS();
            Resources.UnloadUnusedAssets();
        }

#if UNITY_IOS
        //private static string nativeAdsID = "1057601684309421_1057601884309401";
#elif UNITY_ANDROID
        //private const string nativeAdsID = "979269728858634_979270152191925";
        //private static string nativeAdsID = "1133420346694301_1133420716694264";
#endif

        public override void OnKeyBack()
        {
            SceneManager.Instance.OpenExitConfirmation();
        }

        public override void Start()
        {
            base.Start();

            if (!GameManager.Instance.GameConfigs.enableTips)
            {
                TipsObj.SetActive(false);
            }            

#if !UNITY_EDITOR
            for(int i = 0; i < listHideInGame.Count; i++) {
                listHideInGame[i].SetActive(false);
            }
#endif

            songContainer = uiTable.transform;

            listHotSongItems = new List<SongItemView>(10);
            listNewSongItems = new List<SongItemView>(10);
            Timing.RunCoroutine(DelayedSpawnSongItems());
            Timing.RunCoroutine(ShowInGameAds());


            if (GameManager.Instance.GameConfigs.videoAdsReward != 0)
                lblDiamondReward.text = "x " + GameManager.Instance.GameConfigs.videoAdsReward.ToString();


            CheckDailyReward();

            CheckSession();
        }

        public void CheckSession()
        {
            
        }

        public void ParseMidiFiles()
        {

        }

        private void CheckDailyReward()
        {
            //PlayerPrefs.DeleteKey(GameConsts.KEY_DAILY_REWARD);
            string rewardDate = PlayerPrefs.GetString(GameConsts.KEY_DAILY_REWARD);
            int index;
            string oldRewardDateAndIndex = null;
            DateTime newDate = System.DateTime.Now;
            if (string.IsNullOrEmpty(rewardDate))
            {
                index = 0;
                oldRewardDateAndIndex = Convert.ToString(newDate) + "_" + index;
            }
            else
            {
                string[] dayAndIndex = rewardDate.Split('_');
                DateTime oldDate = Convert.ToDateTime(dayAndIndex[0]);
                index = int.Parse(dayAndIndex[1]);
                if (index > 7)
                    index = 7;
                else
                    index += 1;
                //Debug.Log("LastDay: " + oldDate.DayOfYear);
                //Debug.Log("CurrDay: " + newDate.DayOfYear);
                int difference = newDate.DayOfYear - oldDate.DayOfYear;
                //Debug.Log(difference);
                if (difference < 0)
                {
                    Debug.Log("Hack detected. Congratulation ;). Enjoy your gift.");
                    return;
                }

                if (difference >= 1 && difference < 2)
                {
                    //Debug.Log("New Reward!");
                    oldRewardDateAndIndex = Convert.ToString(newDate) + "_" + index;
                    //PlayerPrefs.SetString("PlayDate", newStringDate);
                    //SceneManager.Instance.OpenPopup();
                    //giveGift();
                }
                else
                    if (difference > 2)
                {
                    index = 0;
                    oldRewardDateAndIndex = Convert.ToString(newDate) + "_" + index;
                    //SceneManager.Instance.OpenPopup();
                }
            }

            if (!string.IsNullOrEmpty(oldRewardDateAndIndex))
            {
                SceneManager.Instance.OpenPopup(Scenes.DailyRewardPopUp, oldRewardDateAndIndex);
                //reset daily achievements
            }
        }

        public void ResetDailyReward()
        {
        }

        public void Add3StarSongs()
        {

        }

        IEnumerator<float> ShowInGameAds()
        {
            yield return Timing.WaitForSeconds(3);
            int index = 0;
#if UNITY_IOS
            index = 1;            
#elif UNITY_ANDROID
            index = 0;
#endif
            
            if (GameManager.Instance.GameConfigs.showFullscreenAds != null && index < GameManager.Instance.GameConfigs.showFullscreenAds.Count)
            {
                if (GameManager.Instance.GameConfigs.showFullscreenAds[index])
                {
                    AdsPopUpModel adData = new AdsPopUpModel();
                    adData.imageUrl = GameManager.Instance.GameConfigs.adsImages[index];
                    adData.message = GameManager.Instance.GameConfigs.adsMessages[index];
                    adData.linkUrl = GameManager.Instance.GameConfigs.adsURL[index];
                    SceneManager.Instance.OpenPopup(ProjectConstants.Scenes.AdsPopup, adData);
                }
            }
        }

        IEnumerator<float> DelayedSpawnSongItems()
        {
            yield return 0f;

            SpawnSongsItem();
            yield return 0f;
            RefreshListView();
            
        }

        private void SpawnSongsItem()
        {
            var listHotSongs = GameManager.Instance.StoreData.listHotSongs;
            var listNewSongs = GameManager.Instance.StoreData.listNewSongs;
            if (songItems == null)
                songItems = new List<SongItemView>();
            for (int i = 0; i < listHotSongs.Count; i++)
            {
                SongItemView item = Instantiate(prefabSongItem);
                listHotSongItems.Add(item);
                item.Index = i + 1;
                item.Model = listHotSongs[i];
                item.OnSongSelected += OnSongItemClicked;
                item.OnTryToBuySong += OnTryToBuySong;
                item.transform.SetParent(songContainer, false);
                item.transform.localScale = Vector3.one;
                songItems.Add(item);
            }

            for (int j = 0; j < listNewSongs.Count; j++)
            {
                SongItemView item = Instantiate(prefabSongItem);
                listNewSongItems.Add(item);
                item.Index = listHotSongs.Count + j + 1;
                item.Model = listNewSongs[j];
                item.OnSongSelected += OnSongItemClicked;
                item.OnTryToBuySong += OnTryToBuySong;
                item.transform.SetParent(songContainer, false);
                item.transform.localScale = Vector3.one;
                //print(string.Format("Created new song item, status = {0}", item.Model.type));
                songItems.Add(item);
            }

            Transform emptyTf = Instantiate(prefabEmpty).transform;
            emptyTf.transform.SetParent(songContainer, false);
            emptyTf.transform.localScale = Vector3.one;
        }

        private void OnTryToBuySong(SongItemView song)
        {
            if (ProfileHelper.Instance.CurrentDiamond >= song.Model.pricePrimary)
            {
                ProfileHelper.Instance.ListBoughtSongs.Add(song.Model.storeID);
                ProfileHelper.Instance.CurrentDiamond -= song.Model.pricePrimary;
                
                song.RefreshView();
            }
            else
            {
                SceneManager.Instance.OpenPopup(ProjectConstants.Scenes.IAP);
            }
        }

        private void OnSongItemClicked(SongDataModel songitem)
        {
            if (LivesManager.Instance.CanPlaySong())
            {
                //this.Print("Clicked on song item: " + obj.name + " should process song item clicked = " + ShouldProcessSongItemClick);
                if (ShouldProcessSongItemClick)
                {
                    ShouldProcessSongItemClick = false;
                    LevelDataModel level = new LevelDataModel();
                    level.songData = songitem;
                    GameManager.Instance.SessionData.currentLevel = level;
                    SceneManager.Instance.OpenScene(ProjectConstants.Scenes.MainGame);
                }
            }
            else
            {
                LivesManager.Instance.SuggestRechargeLives();
            }
        }        

        public override void OnEnable()
        {
            //print("Subscribing event of native ads");
            base.OnEnable();
            anmController.SetTrigger("showtoright");
            SceneManager.Instance.onSceneChange += OnSceneChange;
            ShouldProcessSongItemClick = true;

            // Show random tips
            if (GameManager.Instance.GameConfigs.enableTips && GameManager.Instance.GameConfigs.tipsKey != null)
            {
                int rdTnt = UnityEngine.Random.Range(0, GameManager.Instance.GameConfigs.tipsKey.Count);
                lbTips.text = Localization.Get(GameManager.Instance.GameConfigs.tipsKey[rdTnt]);
            }

            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.SessionData != null && GameManager.Instance.SessionData.lastLevel != null)
                {
                    itemLastPlay.Model = GameManager.Instance.SessionData.lastLevel.songData;
                }
                else
                {
                    if (GameManager.Instance.StoreData != null)
                    {
                        var songsData = GameManager.Instance.StoreData.listAllSongs;
                        if (songsData != null)
                        {
                            //if user has played a song last time
                            if (!string.IsNullOrEmpty(GameManager.Instance.GameData.lastPlaySongID))
                            {
                                foreach (var item in GameManager.Instance.StoreData.listAllSongs)
                                {
                                    //try to open it again
                                    if (GameManager.Instance.GameData.lastPlaySongID.Contains(item.storeID))
                                    {
                                        itemLastPlay.Model = item;
                                        //lastPlaySong = item;
                                        return;
                                    }
                                }
                            }

                            //or else, pick first song in list
                            itemLastPlay.Model = songsData[0];
                            //lastPlaySong = songsData[0];
                        }
                        else
                        {
                            Debug.LogWarning("Can't get song list from store data, can't display last play item");
                            //lastPlaySong = null;
                        }
                    }
                }
            }
            ResetSongListHome();

        }

        public void ResetSongListHome()
        {

            if (songItems != null)
            {
                for (int i = 0; i < songItems.Count; i++)
                {
                    songItems[i].RefreshItemView();
                }
            }
        }
        public override void OnDisable()
        {
            //print("Unsubscribing event of native ads");
            base.OnDisable();
            SceneManager.Instance.onSceneChange -= OnSceneChange;
        }



        public void ReplayLastSong()
        {
            //print("Calling replay");
            if (GameManager.Instance.SessionData.lastLevel != null)
            {
                GameManager.Instance.SessionData.currentLevel = GameManager.Instance.SessionData.lastLevel;
                SceneManager.Instance.OpenScene(ProjectConstants.Scenes.MainGame);
            }
            else
            {
                if (itemLastPlay.Model != null)
                {
                    LevelDataModel level = new LevelDataModel();
                    level.songData = itemLastPlay.Model;
                    GameManager.Instance.SessionData.currentLevel = level;
                    SceneManager.Instance.OpenScene(ProjectConstants.Scenes.MainGame);
                }
            }
        }

        private void OnSceneChange(ProjectConstants.Scenes scene)
        {
            anmController.SetTrigger("hidetoleft");
        }

        public void RefreshListView()
        {
            uiTable.repositionNow = true;
            uiTable.enabled = true;
        }           
    }
}