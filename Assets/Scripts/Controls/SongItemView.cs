using UnityEngine;
using System.Collections;
using System;
using Mio.Utils.MessageBus;
//using Facebook.Unity;

namespace Mio.TileMaster {
    public class SongItemView : MonoBehaviour {
        [Header("Song's information")]
        [SerializeField]
        private UILabel lbSongTitle;
        [SerializeField]
        private UILabel lbAuthor;
        [SerializeField]
        private UILabel lbIndex;
        [Header("Song record")]
        [SerializeField]
        private UIToggle[] starImages;
        [SerializeField]
        private UIToggle[] crownImages;

        [Header("Color for new or hot song")]
        [SerializeField]
        private UISprite bannerHot;
        [SerializeField]
        private UISprite bannerNew;

        [Header("Lock Panel Songs")]
        [SerializeField]
        private GameObject lockedGroup;
        [SerializeField]
        private GameObject unlockedGroup;
        [SerializeField]
        private UILabel lblDeficientStar;
        //[SerializeField]
        //private GameObject btnPlaySong;
        //[SerializeField]
        //private UIWidget widgetShowStar;
        [SerializeField]
        private UILabel priceRuby;

        [Header("Show Level Ranking")]
        [SerializeField]
        private UILabel indexRanking;
        //[SerializeField]
        //private GameObject levelRankingBG;

        public event Action<SongDataModel> OnSongSelected;
        public event Action<SongItemView> OnTryToBuySong;

        private SongDataModel model;
        public SongDataModel Model { get { return model; } set { model = value; RefreshItemView(); } }

        private int index;
        public int Index {
            get { return index; }
            set {
                index = value;
                //lbIndex.text = index.ToString();
            }
        }

        //private Transform tf;
        // Use this for initialization
        //void Start () {
        //    //tf = transform;
        //    //bannerSongStatus.cachedGameObject.SetActive(false);
        //    //if (lbSongTitle == null) lbSongTitle = tf.Find("lbSongTitle").GetComponent<UILabel>();
        //    //if (lbAuthor == null) lbAuthor = tf.Find("lbAuthor").GetComponent<UILabel>();
        //    //if (lbIndex == null) lbIndex = tf.Find("Index/lbIndex").GetComponent<UILabel>();

        //}


        private void RefreshViewLevel (Message obj) {
            Debug.Log("levelraking show");
            RefreshView();
        }

        public void RefreshView () {
            lbAuthor.text = model.author;
            lbSongTitle.text = model.name;

            if (lbIndex != null) lbIndex.text = Index.ToString();

            CheckCurrentStarToUnlockSong();

            switch (model.type) {
                case SongItemType.Normal:
                    if (bannerHot != null) bannerHot.cachedGameObject.SetActive(false);
                    if (bannerNew != null) bannerNew.cachedGameObject.SetActive(false);
                    break;
                case SongItemType.New:
                    //bannerSongStatus.cachedGameObject.SetActive(true);
                    //bannerSongStatus.color = colorBannerNew;
                    if (bannerHot != null) bannerHot.cachedGameObject.SetActive(false);
                    if (bannerNew != null) bannerNew.cachedGameObject.SetActive(true);
                    //lbSongStatus.text = Localization.Get("new");
                    //lbSongStatus.color = colorFontNew;
                    break;
                case SongItemType.Hot:
                    //bannerSongStatus.cachedGameObject.SetActive(true);
                    //bannerSongStatus.color = colorBannerHot;
                    if (bannerHot != null) bannerHot.cachedGameObject.SetActive(true);
                    if (bannerNew != null) bannerNew.cachedGameObject.SetActive(false);
                    //lbSongStatus.text = Localization.Get("hot"); ;
                    //lbSongStatus.color = colorFontHot;
                    break;
            }
        }

        public void ShowLevelRanking () {
            SceneManager.Instance.OpenPopup(ProjectConstants.Scenes.LevelRanking, model);
        }

        private void CheckCurrentStarToUnlockSong () {
            int currStar = ProfileHelper.Instance.TotalStars;
            bool shouldLock = false;

            //only display song as locked if there is not enough stars to unlock
            if (model.starsToUnlock > currStar || model.starsToUnlock < 0) {
                //and the song is not bought
                if (!IsBought(model.storeID)) {
                    shouldLock = true;
                }
            }

            if (shouldLock) {
                ShowLockedUI();
            }
            else {
                ShowUnlockedUI();
            }
        }

        private void ShowLockedUI () {
            if(lockedGroup != null) lockedGroup.SetActive(true);
            if(unlockedGroup != null) unlockedGroup.SetActive(false);

            if (lblDeficientStar != null) {
                //lock song by diamond and obliged to buy with diamonds
                if (model.starsToUnlock < 0)
                    lblDeficientStar.text = "";
                else
                    lblDeficientStar.text = (model.starsToUnlock - ProfileHelper.Instance.TotalStars) + Localization.Get("deficientstarunlocksongs");
            }

            if (priceRuby != null) {
                priceRuby.text = "x " + model.pricePrimary.ToString();
            }
        }

        private void ShowUnlockedUI () {
            if (lockedGroup != null) lockedGroup.SetActive(false);
            if (unlockedGroup != null) unlockedGroup.SetActive(true);
            //if (AccessToken.CurrentAccessToken != null) {
            //    if (indexRanking != null) {
            //        int rank = RankingManager.Instance.GetIndexLevelRankingCurrentUser(model.storeID);
            //        indexRanking.text = rank > 0 ? rank.ToString() : "-";
            //        if (levelRankingBG != null) {
            //            levelRankingBG.SetActive(true);
            //        }
            //    }
            //}
            //else {
            //
            //}
        }

        private bool IsBought (string storeID) {
            if (ProfileHelper.Instance.ListBoughtSongs == null) {
                return false;
            }

            for (int i = 0; i < ProfileHelper.Instance.ListBoughtSongs.Count; i++) {
                if (ProfileHelper.Instance.ListBoughtSongs[i].CompareTo(storeID) == 0) {
                    return true;
                }
            }

            return false;
        }

        public void SetTitle (string title) {
            if (lbSongTitle != null) lbSongTitle.text = title;
        }

        public void RefreshItemView () {
            if (Model != null && gameObject.activeInHierarchy) {
                RefreshView();

                if (lockedGroup != null) {
                    if (lockedGroup.activeSelf) {
                        HideSongResult();
                    }
                    else {
                        ShowSongResult();
                    }
                }
            }
        }

        private void HideSongResult () {
            HideAllToogle(starImages);
            HideAllToogle(crownImages);
        }

        private void ShowSongResult () {
            int crown = HighScoreManager.Instance.GetHighScore(Model.storeID, ScoreType.Crown);

            if (crown > 0) {
                SetNumCrowns(crown);
            }
            else {
                int star = HighScoreManager.Instance.GetHighScore(Model.storeID, ScoreType.Star);
                SetNumStars(star);
            }
        }

        private void HideAllToogle (UIToggle[] t) {
            foreach (UIToggle sprite in t) {
                sprite.gameObject.SetActive(false);
            }
        }
        //void OnEnable () {
        //    RefreshItemView();
        //}

        public void SetNumStars (int stars) {
            if (starImages == null || starImages.Length == 0) return;

            int count = 1;
            foreach (UIToggle sprite in starImages) {
                sprite.gameObject.SetActive(true);
                if (count <= stars) {
                    sprite.Set(true);
                }
                else {
                    sprite.Set(false);
                }

                ++count;
            }

            foreach (UIToggle crown in crownImages) {
                crown.gameObject.SetActive(false);
            }
        }

        public void SetNumCrowns (int crowns) {
            if (crownImages == null || crownImages.Length == 0) return;

            int count = 1;
            foreach (UIToggle sprite in crownImages) {
                sprite.gameObject.SetActive(true);
                if (count <= crowns) {
                    sprite.Set(true);
                }
                else {
                    sprite.Set(false);
                }

                ++count;
            }

            foreach (UIToggle star in starImages) {
                star.gameObject.SetActive(false);
            }
        }

        public void OnSongItemClicked () {
            if (OnSongSelected != null) {
                OnSongSelected(Model);
            }
        }

        public void OnSongBuyItemClicked () {
            if (OnTryToBuySong != null) {
                OnTryToBuySong(this);
            }
        }

    }
}