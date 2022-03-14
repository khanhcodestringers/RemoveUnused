using UnityEngine;
using System.Collections;
using ProjectConstants;
using System;
using System.Collections.Generic;
using MovementEffects;
using Mio.Utils.MessageBus;

namespace Mio.TileMaster {
    public class SceneManager : MonoSingleton<SceneManager> {
        public Scenes CurrentScene { get; private set; }
        public Scenes BeforeScene { get; private set; }
        private MainMenuController mainMenu;
        private bool isMenuReady = false;

        //check condition show rating popup
        private int countingEndGame = 0;
        private int ratingCondition = 0;
        private int defaultRatingCondition = 5;
        private const string PLAYERPREF_SHOWRATING = "showRatingInGame";
        private bool isShow;
		private bool canChangeScene = true;

        public event Action<Scenes> onSceneChange;
        
        public string LastOpenSceneName { get; private set; }
        //private static readonly Scenes[] sceneWithFullMenu = { Scenes.HomeUI, Scenes.SettingUI, Scenes.SongList, Scenes.AchievementListUI};
        //private static readonly Scenes[] sceneWithTopMenu = { Scenes.ResultUI };
        //private static readonly Scenes[] sceneWithBottomMenu = { };
        //private static readonly Scenes[] sceneWithoutMenu = { Scenes.MainGame };
        private static Dictionary<string, MainMenuState> menuStateByScene;

        public void Initialize () {
            menuStateByScene = new Dictionary<string, MainMenuState>(15);
            //full menu
            menuStateByScene.Add(Scenes.HomeUI.GetName(), MainMenuState.ShowAll);
            menuStateByScene.Add(Scenes.SettingUI.GetName(), MainMenuState.ShowAll);
            menuStateByScene.Add(Scenes.SongList.GetName(), MainMenuState.ShowAll);
            menuStateByScene.Add(Scenes.AchievementListUI.GetName(), MainMenuState.ShowAll);

            //only top menu
            menuStateByScene.Add(Scenes.ResultUI.GetName(), MainMenuState.ShowTopBar);

            //no menu at all
            menuStateByScene.Add(Scenes.MainGame.GetName(), MainMenuState.HideAll);
			menuStateByScene.Add(Scenes.JoinRoom.GetName(), MainMenuState.HideAll);

            SSSceneManager.Instance.onSceneFocus += OnSceneOpened;

            //PlayerPrefs.DeleteAll();
            //check rating
            int onRating = PlayerPrefs.GetInt(PLAYERPREF_SHOWRATING);
            isShow = (onRating == 1) ? true : false;
        }

        public void RegisterMenu (MainMenuController menu) {
            mainMenu = menu;
            isMenuReady = true;

        }

		public void OnMenuHome(){
			mainMenu.ChangeToggleWhenOnline ();
		}

        private void OnSceneOpened (string sceneName) {
            //Debug.Log("Opening scene: " + sceneName);
            LastOpenSceneName = sceneName;
            SetupMainMenu(sceneName);
        }


        private IEnumerator<float> IEOpenScene (Scenes scene, object data) {
			canChangeScene = false;
            if (onSceneChange != null)
                onSceneChange(scene);
            yield return Timing.WaitForSeconds(0.12f);
            OnProgressChangeScene(scene, data);
			canChangeScene = true;
        }


        /// <summary>
        /// wait for run effect life
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerator<float> IEOpenMainGame (Scenes scene, object data) {
            MessageBus.Annouce(new Message(MessageBusType.PlaySongAction));
            yield return Timing.WaitForSeconds(0.3f);
            OnProgressChangeScene(scene, data);
        }
        /// <summary>
        /// Open specified scene
        /// </summary>
        /// <param name="scene"></param>
        public void OpenScene (Scenes scene, object data = null, bool playAnm = false) {
            if (CurrentScene != scene) {
				if (scene == Scenes.MainGame) {
                    Timing.RunCoroutine(IEOpenMainGame(scene, data));
                }
                else {
					if (playAnm) {
						if (!canChangeScene)
							return;
						Timing.RunCoroutine (IEOpenScene (scene, data));
					}
					else {
						OnProgressChangeScene (scene, data);
					}
                }
            }

            //print("Opening scene " + scene.GetName());

            if (scene == Scenes.MainGame) {
                MidiPlayer.Instance.ShouldPlay = false;
            }
            else {
                MidiPlayer.Instance.ShouldPlay = true;
            }
        }

        private void OnProgressChangeScene (Scenes scene, object data) {
			BeforeScene = CurrentScene;
			CurrentScene = scene;
            Debug.LogError(scene.GetName());
			SSSceneManager.Instance.Screen (scene.GetName (), data);
            //SetupMainMenu(scene);
			if (BeforeScene != Scenes.JoinRoom && BeforeScene != Scenes.OnlineMainGame){
			if (scene == Scenes.SongList || scene == Scenes.HomeUI) {
//				if (isMenuReady)
//					mainMenu.SetSonglist (scene == Scenes.SongList);
				//Utils.AdHelper.Instance.PrepareFullScreenAd();
			} else if (scene == Scenes.ResultUI) {
				//Utils.MessageBus.MessageBus.Instance.SendMessage(msgAchievementDataChanged);

				Timing.RunCoroutine (IECheckShowRatingPopUp ());
			}
		}
        }

        private IEnumerator<float> IECheckShowRatingPopUp() 
        {
            yield return Timing.WaitForSeconds(1.5f);
            CheckShowRatingInGame();
        }
        public void CheckShowRatingInGame()
        {
            //check rating
            if (!isShow)
            {
                if (ratingCondition == 0)
                {
                    if (GameManager.Instance.GameConfigs.ratingCondition > 0)
                        ratingCondition = GameManager.Instance.GameConfigs.ratingCondition;
                    else
                        ratingCondition = defaultRatingCondition;
                }

                countingEndGame++;
                if (countingEndGame >= ratingCondition)
                {
                    OpenPopup(ProjectConstants.Scenes.RatePopUp);
                    isShow = true;
                    PlayerPrefs.SetInt(PLAYERPREF_SHOWRATING, 1);
                    PlayerPrefs.Save();
                }

            }
        }
        public void SetupMainMenu (string sceneName) {
            if (isMenuReady) {
                if (menuStateByScene.ContainsKey(sceneName)) {
                    mainMenu.SetMenuState(menuStateByScene[sceneName]);
					mainMenu.ResetToggleMainMenu (CurrentScene);

                }
            }
        }

        public void OpenPopup (Scenes popup, object data = null) {
            SSSceneManager.Instance.PopUp(popup.GetName(), data);
        }

        public void CloseScene () {
            SSSceneManager.Instance.Close();
        }

        public void LoadMenu () {
            SSSceneManager.Instance.LoadMenu(Scenes.MainMenu.GetName());
        }

        /// <summary>
        /// Show confirmation message before exiting the game
        /// </summary>
        public void OpenExitConfirmation () {
            MessageBoxDataModel msg = new MessageBoxDataModel();
            //msg.message = "The game data on your device and our server do not look alike. Which data would you like to use?";
            //msg.messageNo = "SERVER DATA";
            //msg.messageYes = "CURRENT DATA";
            msg.message = Localization.Get("pu_exit_title");
            msg.messageYes = Localization.Get("pu_exit_btn_no");
            msg.messageNo = Localization.Get("pu_exit_btn_yes"); ;
            msg.OnNoButtonClicked = delegate { Application.Quit(); };
            msg.OnYesButtonClicked = delegate {  };

            OpenPopup(ProjectConstants.Scenes.MessageBoxPopup, msg);
        }

        //public void Open

        //public void SetMenuVisible(bool isActive = true) {
        //    if (isActive) {
        //        SSSceneManager.Instance.ShowMenu();
        //    }
        //    else {
        //        SSSceneManager.Instance.HideMenu();
        //    }
        //}

        public void SetLoadingVisible (bool isActive = true) {
            if (isActive) {
                SSSceneManager.Instance.ShowLoading();
            }
            else {
                SSSceneManager.Instance.HideLoading();
            }
        }
    }
}