using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Mio.TileMaster {
    public class AchievementListSceneController : SSController {
        [SerializeField]
        private AchievementListView[] achievementListView;

        public Animator anmCotroller;

        [Header("Tab UI")]
        [SerializeField]
        private List<UIToggle> tabButtons;
        [SerializeField]
        private List<GameObject> tabContents;
        private int cacheAchievement = 0;
        //public void OnAchievementClicked(Achievement.AchievementModel achievement) {
        //    print("Achievement item claimed " + achievement.ID);

        //}


        public override void OnEnable () {
            base.OnEnable();
            if (SceneManager.Instance.BeforeScene != ProjectConstants.Scenes.SettingUI)
                anmCotroller.SetTrigger("showtoleft");
            else
                anmCotroller.SetTrigger("showtoright");

            GameManager.Instance.SessionData.needAttentionAchievement = false;
            SceneManager.Instance.onSceneChange += OnChangeScene;
            
            tabButtons[cacheAchievement].value = true;
			for (int i = 0; i < achievementListView.Length; i++) {
				achievementListView [i].RefreshAchievementList ();
			}
        }

		public override void OnEnableFS ()
		{
			base.OnEnableFS ();
	
		}

        public void OnToggleValueChanged () {
            for(int i = 0; i < tabButtons.Count; i++) {
                if (tabButtons[i].gameObject.activeInHierarchy) {
                    if(tabButtons[i].value == true) {
                        tabContents[i].gameObject.SetActive(true);
						achievementListView [i].RefreshAchievementList ();
                        cacheAchievement = i;
                    }else {
                        tabContents[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        public override void OnDisable () {
            base.OnDisable();

            SceneManager.Instance.onSceneChange -= OnChangeScene;
        }

        public override void OnKeyBack () {
            SceneManager.Instance.OpenExitConfirmation();
        }

        private void OnChangeScene (ProjectConstants.Scenes scene) {
            if (scene != ProjectConstants.Scenes.SettingUI)
                anmCotroller.SetTrigger("hidetoright");
            else
                anmCotroller.SetTrigger("hidetoleft");
        }
    }
}