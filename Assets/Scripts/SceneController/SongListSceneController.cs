using UnityEngine;
using System.Collections;
using System;

namespace Mio.TileMaster {
    public class SongListSceneController : SSController {
        public SongListView viewSongList;
        //public UIScrollView uiScrollView;

        public Animator anmController;
        //public override void OnEnableFS() {
        //    viewSongList.OnSongClicked -= OnSongClicked;
        //    viewSongList.OnSongClicked += OnSongClicked;
        //}
        
        //public override o

        private void OnSongClicked(SongDataModel obj) {
            //this.Print("Song clicked " + obj.songURL);
            
        }

        public override void OnEnable() {
            base.OnEnable();
            if(SceneManager.Instance.BeforeScene != ProjectConstants.Scenes.HomeUI)
                anmController.SetTrigger("showtoright");
            else
                anmController.SetTrigger("showtoleft");

            //Debug.LogError(SceneManager.Instance.CurrentScene.ToString());
            SceneManager.Instance.onSceneChange -= OnSceneChange;
            SceneManager.Instance.onSceneChange += OnSceneChange;
            viewSongList.OnSongClicked -= OnSongClicked;
            viewSongList.OnSongClicked += OnSongClicked;
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SessionData.needAttentionSongList = false;
                viewSongList.RefreshSongList(GameManager.Instance.StoreData);
            }
            //viewSongList.ShouldProcessSongItemClick = true;
        }

        public override void OnDisable() {
            SceneManager.Instance.onSceneChange -= OnSceneChange;
            viewSongList.OnSongClicked -= OnSongClicked;
            base.OnDisable();
        }

        public override void OnKeyBack () {
            SceneManager.Instance.OpenExitConfirmation();
        }

        private void OnSceneChange(ProjectConstants.Scenes scene) {
            if(scene != ProjectConstants.Scenes.HomeUI)
                anmController.SetTrigger("hidetoleft");
            else
                anmController.SetTrigger("hidetoright");

        }
    }
}