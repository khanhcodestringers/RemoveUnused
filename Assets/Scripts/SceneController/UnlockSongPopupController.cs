using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MovementEffects;
using Mio.Utils;

namespace Mio.TileMaster {
    public class UnlockSongPopupController : SSController {
        [SerializeField]
        private Transform songsContainer;
        [SerializeField]
        private UIGrid songsGrid;
        [Tooltip("This number must be greater than maximum number of songs can be unlock at the same time")]
        [SerializeField]
        private List<SongItemView> listSongItems;

        [SerializeField]
        private TweenPosition contentTween;
        [SerializeField]
        private GameObject buttonClose;

        private NewlyUnlockedSongModel newSongs;
        private bool shouldProcessSongItemClick;
        public bool ShouldProcessSongItemClick {
            get { return shouldProcessSongItemClick; }
            set {
                shouldProcessSongItemClick = value;
                //print("Setting should process song item click: " + value);
            }
        }

        public override void OnEnableFS () {
            base.OnEnableFS();

            for(int i = 0; i < listSongItems.Count; i++) {
                listSongItems[i].OnSongSelected -= OnSongClicked;
                listSongItems[i].OnSongSelected += OnSongClicked;
                //listSongItems[i].gameObject.SetActive(false);
            }
        }


        public override void OnSet (object data) {
            newSongs = (NewlyUnlockedSongModel)data;
            ShouldProcessSongItemClick = true;
            if (newSongs == null) {
                Debug.LogWarning("Trying to show new unlocked songs without any data. Closing scene");
                SceneManager.Instance.CloseScene();
                return;
            }

            songsGrid.enabled = false;
            contentTween.cachedTransform.localPosition = Vector3.zero;
            //hide all song item views before showing animation
            for (int i = 0; i < listSongItems.Count; i++) {
                listSongItems[i].gameObject.SetActive(false);
                listSongItems[i].transform.localPosition = new Vector3(0, -800 - 400 * i);
            }

            Timing.RunCoroutine(ShowNewlyUnlockedSongs());
            buttonClose.SetActive(false);
        }

        private IEnumerator<float> ShowNewlyUnlockedSongs () {
            if (newSongs == null) {
                yield break;
            }

            //wait for popup animation to complete
            yield return Timing.WaitForSeconds(1.5f);

            //play explosion animation here

            //wait for animation to complete
            //yield return Timing.WaitForSeconds(1f);
            for(int i = 0; i < newSongs.newlyUnlockedSongs.Count; i++) {
                listSongItems[i].Model = newSongs.newlyUnlockedSongs[i];
                listSongItems[i].gameObject.SetActive(true);
                listSongItems[i].RefreshItemView();
            }
            yield return 0;
            songsGrid.enabled = true;

            float heightTweenContent = newSongs.newlyUnlockedSongs.Count * songsGrid.cellHeight * 0.5f + 100;
            contentTween.ResetToBeginning();
            contentTween.to = new Vector3(0, heightTweenContent);
            contentTween.PlayForward();

            yield return Timing.WaitForSeconds(contentTween.duration);
            buttonClose.SetActive(true);
        }

        //public int num = 4;
        //public void Test () {
        //    for (int i = 0; i < num; i++) {
        //        //listSongItems[i].Model = newSongs.newlyUnlockedSongs[i];
        //        listSongItems[i].gameObject.SetActive(true);
        //        //listSongItems[i].RefreshItemView();
        //    }

        //    for(int i = num; i < listSongItems.Count; i++) {
        //        listSongItems[i].gameObject.SetActive(false);
        //    }

        //    songsGrid.enabled = true;

        //    float heightTweenContent = num * songsGrid.cellHeight * 0.5f;
        //    contentTween.ResetToBeginning();
        //    contentTween.to = new Vector3(0, heightTweenContent);
        //    contentTween.PlayForward();
        //}

        private void OnSongClicked (SongDataModel song) {
            if (LivesManager.Instance.CanPlaySong()) {
                //this.Print("Clicked on song item: " + obj.name + " should process song item clicked = " + ShouldProcessSongItemClick);
                if (ShouldProcessSongItemClick) {
                    ShouldProcessSongItemClick = false;
                    LevelDataModel level = new LevelDataModel();
                    level.songData = song;
                    GameManager.Instance.SessionData.currentLevel = level;
                    SceneManager.Instance.OpenScene(ProjectConstants.Scenes.MainGame);
                }
            }
            else {
                LivesManager.Instance.SuggestRechargeLives();
            }
        }

        public void ClosePopup () {
            SceneManager.Instance.CloseScene();
        }
    }
}