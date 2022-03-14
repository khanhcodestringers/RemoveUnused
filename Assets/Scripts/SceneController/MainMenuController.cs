using UnityEngine;
using System.Collections;
using ProjectConstants;
using Mio.Utils.MessageBus;
using System;
using MovementEffects;
using System.Collections.Generic;

namespace Mio.TileMaster
{
	public enum MainMenuState
	{
		HideAll,
		ShowTopBar,
		ShowBottomBar,
		ShowAll
	}

	public class MainMenuController : SSController
	{
		[SerializeField]
		private GameObject liveBG;
		[SerializeField]
		[Tooltip ("The camera to set clear flag to NONE, reduce draw call")]
		private Camera menuCamera;

		[Header ("Menu bars")]
		[SerializeField]
		private UIWidget topBar;
		[SerializeField]
		private TweenPosition tweenTopBar;
		[SerializeField]
		private UIWidget bottomBar;
		[SerializeField]
		private TweenPosition tweenBottomBar;
		[SerializeField]
		private float menubarFloatDistance = 250.0f;

		[Header ("User's metrics")]
		[SerializeField]
		private UILabel lbLife;
		[SerializeField]
		private UILabel lbLifeCountdown;
		[SerializeField]
		private GameObject goLifeCountdown;
		[SerializeField]
		private UILabel lbDiamond;
		[SerializeField]
		private UILabel lbStar;
		[SerializeField]
		private UILabel lbCrown;

		[SerializeField]
		private UIToggle btnSongList;
		[SerializeField]
		private UIToggle btnHome;
		[SerializeField]
		private List<UIToggle> btns;
		[SerializeField]
		private GameObject onlineObj;
		[SerializeField]
		private UIGrid gridToggle;
		//[SerializeField]
		//private UILabel lbPlayerLevel;
		//[SerializeField]
		//private UIProgressBar progressBar;
        


		[Header ("Animation ")]
        //[SerializeField]
        //private GameObject diamondObj;
		[SerializeField]
		[Tooltip ("Minimum value of diamond metric to start playing the warning animation")]
		private int minMetricDiamonds = 3;
		//[SerializeField]
		//private GameObject lifeObj;
		[SerializeField]
		[Tooltip ("Minimum value of life metric to start playing the warning animation")]
		private int minMetricLife = 3;

		private const string WARNING_ANIMATION_NAME = "PulsingAnimation";
		private const string ADD_ANIMATION_NAME = "DiamondEffectInMainMenu";

		public Animation[] diamondAnims, lifeAnims;
		private AnimationState asWarnLife, asWarnDiamond;
		private const int IndexAnimationAdd = 0;
		private const int IndexAnimationWarning = 1;

		private MainMenuState currentState = MainMenuState.HideAll;
		private string[] int2string;

		[SerializeField]
		private Animation animAchievementTab;
		[SerializeField]
		private Animation animSonglistTab;

		void Update ()
		{
			CheckAttentionTabs ();
		}

		int _ccount = 0;

		/// <summary>
		/// Well lol this one is absolutely garbage. But consider this one "room for improvement". Delete and alter as you wish
		/// </summary>
		public void CheckAttentionTabs ()
		{
			if (++_ccount < 5)
				return;
			_ccount = 0;
			if (GameManager.Instance.SessionData.needAttentionAchievement) {
				if (!animAchievementTab.isPlaying) {
					animAchievementTab.gameObject.SetActive (true);
					animAchievementTab.Play ();
				}
			} else {
				if (animAchievementTab.isPlaying) {
					animAchievementTab.Stop ();
					animAchievementTab.gameObject.SetActive (false);
				}
			}

			if (GameManager.Instance.SessionData.needAttentionSongList) {
				if (!animSonglistTab.isPlaying) {
					animSonglistTab.gameObject.SetActive (false);
					animSonglistTab.Play ();
				}
			} else {
				if (animSonglistTab.isPlaying) {
					animSonglistTab.Stop ();
					animSonglistTab.gameObject.SetActive (false);
				}
			}
		}

		public override void OnEnableFS ()
		{
			base.OnEnableFS ();
			SceneManager.Instance.RegisterMenu (this);
			goLifeCountdown.SetActive (false);
			int2string = new string[60];
			//pre cache all posible ToString() for timer countdown
			for (int i = 0; i < 60; i++) {
				int2string [i] = i.ToString ("00");
			}

			//diamondAnims = diamondObj.GetComponents<Animation>();
			//lifeAnims = lifeObj.GetComponents<Animation>();
		}

		public override void Start ()
		{
			base.Start ();
			SetPosForToggle ();
			tweenTopBar.to = topBar.cachedTransform.localPosition;
			tweenTopBar.from = new Vector3 (tweenTopBar.to.x, tweenTopBar.to.y + menubarFloatDistance, tweenTopBar.to.y);
			tweenBottomBar.to = bottomBar.cachedTransform.localPosition;
			tweenBottomBar.from = new Vector3 (tweenBottomBar.to.x, tweenBottomBar.to.y - menubarFloatDistance, tweenBottomBar.to.y);

			tweenBottomBar.PlayForward ();
			tweenTopBar.PlayForward ();
			currentState = MainMenuState.ShowAll;
		}

		//public bool isTest = false;
		//public bool isForwarded = false;
		//void Update() {
		//    if (isTest) {
		//        isTest = false;
		//        if (isForwarded) {
		//            tweenBottomBar.PlayReverse();
		//            tweenTopBar.PlayReverse();
		//        }
		//        else {
		//            tweenBottomBar.PlayForward();
		//            tweenTopBar.PlayForward();
		//        }

		//        isForwarded = !isForwarded;
		//    }
		//}

		public override void OnEnable ()
		{
			base.OnEnable ();

			//menuCamera.clearFlags = CameraClearFlags.
			//MessageBus.Instance.Subscribe(MessageBusType.XPChanged, OnUserXPChanged);
			//MessageBus.Instance.Subscribe(MessageBusType.UserLevelChanged, OnUserLevelChanged);
			MessageBus.Instance.Subscribe (MessageBusType.TimeLifeCountDown, OnLifeGenerateTimeCountdown);
			MessageBus.Instance.Subscribe (MessageBusType.LifeChanged, OnUserLifeChanged);
			MessageBus.Instance.Subscribe (MessageBusType.DiamondChanged, OnDiamondChanged);
			MessageBus.Instance.Subscribe (MessageBusType.UserPlayRecordChanged, OnUserPlayRecordChanged);
			MessageBus.Instance.Subscribe (MessageBusType.PlaySongAction, PlayAnmMinusLifeWhenPlaySong);
			//MessageBus.Instance.Subscribe(MessageBusType.UserDataChanged, OnUserDataChanged);

			StartCoroutine (RefreshUserView ());
		}

		public override void OnDisable ()
		{
			base.OnDisable ();

			//MessageBus.Instance.Unsubscribe(MessageBusType.XPChanged, OnUserXPChanged);
			//MessageBus.Instance.Unsubscribe(MessageBusType.UserLevelChanged, OnUserLevelChanged);
			MessageBus.Instance.Unsubscribe (MessageBusType.TimeLifeCountDown, OnLifeGenerateTimeCountdown);
			MessageBus.Instance.Unsubscribe (MessageBusType.LifeChanged, OnUserLifeChanged);
			MessageBus.Instance.Unsubscribe (MessageBusType.DiamondChanged, OnDiamondChanged);
			MessageBus.Instance.Unsubscribe (MessageBusType.UserPlayRecordChanged, OnUserPlayRecordChanged);
			MessageBus.Instance.Unsubscribe (MessageBusType.PlaySongAction, PlayAnmMinusLifeWhenPlaySong);
			//MessageBus.Instance.Unsubscribe(MessageBusType.UserDataChanged, OnUserDataChanged);
		}

		/// <summary>
		/// Sets the position for toggle when on/off Online.
		/// </summary>
		public void SetPosForToggle ()
		{
			if (GameManager.Instance.GameConfigs.flagOnline == false) {
				onlineObj.SetActive (false);
				gridToggle.cellWidth = 161;
				gridToggle.repositionNow = true;
			}
		}

		public void SetMenuState (MainMenuState nextState)
		{
			//print("Setting menu state from " + currentState + " to " + nextState);
			if (nextState != currentState) {
				switch (nextState) {
				case MainMenuState.HideAll:
					liveBG.SetActive (false);
					if (currentState == MainMenuState.ShowTopBar) {
						tweenTopBar.PlayReverse ();
					} else if (currentState == MainMenuState.ShowBottomBar) {
						tweenBottomBar.PlayReverse ();
					} else {
						tweenTopBar.PlayReverse ();
						tweenBottomBar.PlayReverse ();
					}

					Timing.RunCoroutine (DelayedDisable (1f));
                        //topBar.cachedGameObject.SetActive(false);
                        //bottomBar.cachedGameObject.SetActive(false);
					break;
				case MainMenuState.ShowTopBar:
					if (!gameObject.activeInHierarchy) {
						gameObject.SetActive (true);
					}
					liveBG.SetActive (false);
					if (currentState == MainMenuState.ShowBottomBar) {
						tweenTopBar.PlayForward ();
						tweenBottomBar.PlayReverse ();
					} else if (currentState == MainMenuState.ShowAll) {
						tweenBottomBar.PlayReverse ();
					} else {
						tweenTopBar.PlayForward ();
					}
					break;
				case MainMenuState.ShowBottomBar:
					if (!gameObject.activeInHierarchy) {
						gameObject.SetActive (true);
					}
					liveBG.SetActive (true);
					if (currentState == MainMenuState.ShowTopBar) {
						tweenBottomBar.PlayForward ();
						tweenTopBar.PlayReverse ();
					} else if (currentState == MainMenuState.ShowAll) {
						tweenTopBar.PlayReverse ();
					} else {
						tweenBottomBar.PlayForward ();
					}
					break;
				case MainMenuState.ShowAll:
					if (!gameObject.activeInHierarchy) {
						gameObject.SetActive (true);
					}
					liveBG.SetActive (true);
					if (currentState == MainMenuState.ShowTopBar) {
						tweenBottomBar.PlayForward ();
					} else if (currentState == MainMenuState.ShowBottomBar) {
						tweenTopBar.PlayForward ();
					} else {
						tweenTopBar.PlayForward ();
						tweenBottomBar.PlayForward ();
					}
					break;
				}

				currentState = nextState;
			}
		}

		private IEnumerator<float> DelayedDisable (float delay)
		{
			yield return Timing.WaitForSeconds (delay);
			gameObject.SetActive (false);
		}

		public void SetSonglist (bool isSongList)
		{
			//	btnSongList.Set (isSongList);

		}


		public void ChangeToggleWhenOnline ()
		{
			//btns [0].Set (true);
		}

		public void ResetToggleMainMenu (ProjectConstants.Scenes scene)
		{
			switch (scene) {
			case ProjectConstants.Scenes.HomeUI:
				if (btns [0].value != true)
					btns [0].Set (true);
				break;
			case ProjectConstants.Scenes.JoinRoom:
				if (btns [1].value != true)
				btns [1].Set (true);
				break;
			case ProjectConstants.Scenes.SongList:
				if (btns [2].value != true)
					btns [2].Set (true);
				break;
			case ProjectConstants.Scenes.AchievementListUI:
				if (btns [3].value != true)
					btns [3].Set (true);
				break;
			case ProjectConstants.Scenes.SettingUI:
				if (btns [4].value != true)
					btns [4].Set (true);
				break;
			default:
				break;
			}
		}

		private void OnLifeGenerateTimeCountdown (Message obj)
		{
			if (obj.data != null) {
				int numSeconds = (int)(obj.data);
				if (numSeconds <= 0) {
					goLifeCountdown.SetActive (false);
				} else {
					if (!goLifeCountdown.activeInHierarchy) {
						goLifeCountdown.SetActive (true);
					}

					int minutes = Mathf.FloorToInt (numSeconds * 1.0f / 60);
					int seconds = Mathf.Clamp (numSeconds - minutes * 60, 0, 59);
					lbLifeCountdown.text = string.Format ("{0}:{1}", int2string [minutes], int2string [seconds]);
				}
			}
		}

		private void OnUserDataChanged (Message obj)
		{
			//updating view should wait for 1 frame to ensure everything has finished
			StartCoroutine (RefreshUserView ());
		}

		private IEnumerator RefreshUserView ()
		{
			yield return null;
			menuCamera.clearFlags = CameraClearFlags.Nothing;
			lbStar.text = ProfileHelper.Instance.TotalStars.ToString ();
			lbCrown.text = ProfileHelper.Instance.TotalCrowns.ToString ();
			lbLife.text = ProfileHelper.Instance.CurrentLife.ToString ();
			lbDiamond.text = ProfileHelper.Instance.CurrentDiamond.ToString ();

			ShowWarningDiamondAnimation (ProfileHelper.Instance.CurrentDiamond);
			ShowWarningLifeAnimation (ProfileHelper.Instance.CurrentLife);
		}

		//private void OnUserXPChanged(Message obj) {
		//    //if(gameObject.)
		//}

		//private void OnUserLevelChanged(Message obj) {
		//    lbPlayerLevel.text = ProfileHelper.Instance.CurrentPlayerLevel.ToString();
		//}

		private void OnUserPlayRecordChanged (Message obj)
		{
			if (gameObject.activeInHierarchy) {
				StartCoroutine (RefreshUserRecordView ());
			}
		}


		private IEnumerator RefreshUserRecordView ()
		{
			yield return null;
			if (gameObject.activeInHierarchy) {
				lbStar.text = ProfileHelper.Instance.TotalStars.ToString ();
				lbCrown.text = ProfileHelper.Instance.TotalCrowns.ToString ();
			}
		}

		private void OnUserLifeChanged (Message obj)
		{
			int currLife = int.Parse (lbLife.text);
			lbLife.text = ProfileHelper.Instance.CurrentLife.ToString ();

			if (currLife < ProfileHelper.Instance.CurrentLife) {
				lifeAnims [IndexAnimationAdd].Play (ADD_ANIMATION_NAME);
			}

			ShowWarningLifeAnimation (currLife);
		}

		private void ShowWarningLifeAnimation (int currentLife)
		{
			if (currentLife <= minMetricLife) {
				if (!lifeAnims [IndexAnimationWarning].IsPlaying (WARNING_ANIMATION_NAME)) {
					lifeAnims [IndexAnimationWarning] [WARNING_ANIMATION_NAME].wrapMode = WrapMode.Loop;
					//Debug.Log("Playing warning animation");
					asWarnLife = lifeAnims [IndexAnimationWarning].PlayQueued (WARNING_ANIMATION_NAME);
				}
			} else {
				//Debug.Log("Stopping warn animation");
				if (lifeAnims [IndexAnimationWarning].IsPlaying (WARNING_ANIMATION_NAME)) {
					asWarnLife.wrapMode = WrapMode.Once;
				}
				//lifeAnm[WARNING_ANIMATION_NAME].wrapMode = WrapMode.Once;
			}
		}


		private void OnDiamondChanged (Message obj)
		{
			int currDiamond = int.Parse (lbDiamond.text);
			lbDiamond.text = ProfileHelper.Instance.CurrentDiamond.ToString ();

			if (currDiamond < ProfileHelper.Instance.CurrentDiamond) {
				diamondAnims [IndexAnimationAdd].Play (ADD_ANIMATION_NAME);
			}

			ShowWarningDiamondAnimation (ProfileHelper.Instance.CurrentDiamond);
		}

		private void ShowWarningDiamondAnimation (int currentDiamond)
		{
			if (currentDiamond <= minMetricDiamonds) {
				if (!diamondAnims [IndexAnimationWarning].IsPlaying (WARNING_ANIMATION_NAME)) {
					diamondAnims [IndexAnimationWarning] [WARNING_ANIMATION_NAME].wrapMode = WrapMode.Loop;
					//Debug.Log("Playing warning animation");
					asWarnDiamond = diamondAnims [IndexAnimationWarning].PlayQueued (WARNING_ANIMATION_NAME);
				}
			} else {
				//Debug.Log("Stopping warn animation");
				if (diamondAnims [IndexAnimationWarning].IsPlaying (WARNING_ANIMATION_NAME)) {
					asWarnDiamond.wrapMode = WrapMode.Once;
				}
				//lifeAnm[WARNING_ANIMATION_NAME].wrapMode = WrapMode.Once;
			}
		}

		public void PlayAnmMinusLifeWhenPlaySong (Message mess)
		{
			lifeAnims [IndexAnimationAdd].Play ("minuslifewhenplaysong");
		}
	}
}