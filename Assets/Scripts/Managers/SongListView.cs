using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Mio.Utils;
using MovementEffects;

namespace Mio.TileMaster
{
    public class SongListView : MonoBehaviour
    {
        private int lastViewStoreVersion;
        //public UIGrid songItemGrid;
        [SerializeField]
        private UIWrapContent itemScrollList;

        private List<SongDataModel> listSongData;
        private List<SongItemView> listSongItemViews;
        public GameObject songItem2Swap;
        public GameObject prefabEmpty;

        private int limmitItemScroll;

        public Camera cameraForImpesstionNativeAds;
        //public int nativeAdIndex = 5;
        //private bool nativeAdSummoned = false;
        //private int nativeAdLastIndex = -1;

        private bool shouldProcessSongItemClick = true;
        public bool ShouldProcessSongItemClick
        {
            get { return shouldProcessSongItemClick; }
            set
            {
                shouldProcessSongItemClick = value;
                //print("Setting should process song item click: " + value);
            }
        }
        private Dictionary<GameObject, SongItemView> currentItemList;

        public event Action<SongDataModel> OnSongClicked;

        private bool isInitialized = false;
        void Start()
        {
            Initialize();
        }

        void OnEnable()
        {
            //itemScrollList.SortBasedOnScrollMovement();
            //itemScrollList.WrapContent
            //NGUITools.MarkParentAsChanged(itemScrollList.transform.parent.gameObject);
            ShouldProcessSongItemClick = true;
        }

        public void LoadNativeAds(string nativeAdsID)
        {
        
        }

        public void Initialize()
        {
            if (!isInitialized)
            {
                lastViewStoreVersion = 0;
                listSongItemViews = new List<SongItemView>(20);

                currentItemList = new Dictionary<GameObject, SongItemView>(20);
                itemScrollList.onInitializeItem += OnSongItemNeedInitialized;

                //get all song item view
                Transform scrolllist = itemScrollList.transform;
                int numItem = scrolllist.childCount;
                for (int i = 0; i < numItem; i++)
                {
                    SongItemView v = scrolllist.GetChild(i).GetComponent<SongItemView>();
                    if (v != null)
                    {
                        v.OnSongSelected += OnSongItemClicked;
                        v.OnTryToBuySong += OnTryToBuySong;

                        currentItemList.Add(v.gameObject, v);
                        listSongItemViews.Add(v);
                    }
                }

                isInitialized = true;
            }
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
        bool isSpawnAdsFirst;
        bool isSpawnAdsLast;
        bool isSpawnEmptyObj;
   

        private void OnSongItemNeedInitialized(GameObject gameobject, int wrapIndex, int realIndex)
        {
            //Debug.Log(realIndex);
            //Debug.Log(listSongData.Count);
            if (realIndex == limmitItemScroll && !isSpawnEmptyObj) 
            {
                isSpawnEmptyObj = true;
                Transform emptyTf = Instantiate(prefabEmpty).transform;
                emptyTf.transform.SetParent(itemScrollList.transform);
                emptyTf.transform.localScale = Vector3.one;
                Vector2 pos = Vector2.zero;
                pos.x = gameobject.transform.localPosition.x;
                pos.y = Mathf.Abs(gameobject.transform.localPosition.y) + itemScrollList.itemSize;
                if (pos.y > gameobject.transform.localPosition.y)
                    pos.y = -pos.y;

                emptyTf.localPosition = pos;
            }
            int index = Mathf.Abs(realIndex);
            if (index == GameManager.Instance.GameConfigs.indexNativeAds[0] - 1)
            {
                gameobject.SetActive(false);
            }
            else if (index == GameManager.Instance.GameConfigs.indexNativeAds[1])
            {
                gameobject.SetActive(false);
            }
            else
            {
                int indexForGetSong = index;
                if (index > GameManager.Instance.GameConfigs.indexNativeAds[0] - 1)
                {
                    --indexForGetSong;
                }
                if (index > GameManager.Instance.GameConfigs.indexNativeAds[1])
                {
                    --indexForGetSong;
                }
                if (!gameobject.activeSelf)
                    gameobject.SetActive(true);

                //SongItemView item = gameobject.GetComponent<SongItemView>();

                SongItemView item;
                if (!currentItemList.ContainsKey(gameobject))
                {
                    item = gameobject.GetComponent<SongItemView>();
                    currentItemList.Add(gameobject, item);
                }
                else
                {
                    currentItemList.TryGetValue(gameobject, out item);
                }
                item.Index = indexForGetSong + 1;
                if(indexForGetSong < 0 || indexForGetSong >= listSongData.Count) {
                    Debug.Log("Index for song: " + indexForGetSong + " total data count: " + listSongData.Count);
                }
                item.Model = listSongData[indexForGetSong];
            }

        }

        bool isRefreshing = false;
        private IEnumerator<float> C_RefreshScrollList(GameObject obj = null, bool isShowing = false)
        {
            if (isRefreshing) { yield break; }
            isRefreshing = true;
            yield return 0;
            itemScrollList.SortBasedOnScrollMovement();
            yield return 0;
            if (isShowing)
            {
                if (obj != null)
                {
                    Vector3 adPos = obj.transform.localPosition;
                }
                itemScrollList.ResetChildrenPositions();
            }
            yield return 0;
            isRefreshing = false;
        }

        /// <summary>
        /// Convert index of wrap content ui into index of song data
        /// Keep in mind that this function do NOT check for null in listSongData
        /// </summary>
        private int ConvertFromScrollIndexToDataIndex(int realScrollIndex)
        {
            //Math.abs, not really faster, but still using it as a reference lol
            int absRealScrollIndex = (realScrollIndex + (realScrollIndex >> 31)) ^ (realScrollIndex >> 31);
            absRealScrollIndex = absRealScrollIndex % listSongData.Count;

            return (realScrollIndex <= 0) ? (absRealScrollIndex) : (listSongData.Count - absRealScrollIndex);
        }

        public void RefreshSongList(StoreDataModel model)
        {
            Initialize();

            // this.Print("Refreshing song list " + model.version);
            if (model != null && model.version != 0 && model.version != lastViewStoreVersion)
            {
                var listSong = model.listAllSongs;
                //this.Print("List song data has " + listSong.Count + " songs");
                if (listSong == null || listSong.Count <= 0)
                {
                    this.Print("Song list is empty, can't produce list song");
                    return;
                }
                //save reference for better management
                listSongData = listSong;

                //RefreshListItemData(model);
                lastViewStoreVersion = model.version;
            }
            itemScrollList.minIndex = itemScrollList.maxIndex = 0;
            itemScrollList.minIndex = -((listSongData.Count + GameManager.Instance.GameConfigs.indexNativeAds.Count) - 1);
            limmitItemScroll = itemScrollList.minIndex;
            RefreshListItemRecord();
        }

        private void RefreshListItemRecord()
        {
            for (int i = 0; i < listSongItemViews.Count; i++)
            {
                listSongItemViews[i].RefreshItemView();
            }
        }



        private void RefreshScoreRecord(SongItemView songItem)
        {
            int crown = 0, star = 0;
            //set highscore
            star = HighScoreManager.Instance.GetHighScore(songItem.Model.storeID, ScoreType.Star);
            crown = HighScoreManager.Instance.GetHighScore(songItem.Model.storeID, ScoreType.Crown);
            //print("Item: " + songItem.Model.storeID + " crown: " + crown + " star: " + star);
            if (crown > 0)
            {
                songItem.SetNumCrowns(crown);
            }
            else
            {
                songItem.SetNumStars(star);
            }
        }

        private void OnSongItemClicked(SongDataModel obj)
        {
            //check if user has enough lives to continue?
            if (LivesManager.Instance.CanPlaySong())
            {
                //this.Print("Clicked on song item: " + obj.name + " should process song item clicked = " + ShouldProcessSongItemClick);
                if (ShouldProcessSongItemClick)
                {
                    ShouldProcessSongItemClick = false;
                    LevelDataModel level = new LevelDataModel();
                    level.songData = obj;
                    GameManager.Instance.SessionData.currentLevel = level;
                    SceneManager.Instance.OpenScene(ProjectConstants.Scenes.MainGame);
                }
            }
            else
            {
                LivesManager.Instance.SuggestRechargeLives();

            }
        }
    }
}