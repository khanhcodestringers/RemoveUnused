using UnityEngine;
using System.Collections;
using ProjectConstants;
using System;
using MovementEffects;
using System.Collections.Generic;

namespace Mio.TileMaster {
    public class SplashScreenController : SSController {
        [SerializeField]
        private TweenAlpha logoFadeOutTween;
        
        [Header("Message to communicate with player")]
        [SerializeField]
        private UILabel lbMessage;
        [SerializeField]
        private UISprite retryButton;
        [SerializeField]
        private Animator loadingAnimation;
        [SerializeField]
        private GameObject loadingContainer;
        [SerializeField]
        private LoadingImagesBar loadingBar;

        [Header("Animation duration when fade in, fade out the elements")]
        [SerializeField]
        private float durationFadeIn = 0.5f;
        [SerializeField]
        private float durationFadeOut = 0.25f;

        [Header("Language options")]
        [SerializeField]
        private GameObject languageContainer;
        [SerializeField]
        private TweenAlpha languageOptionTween;

        public Action OnLanguageConfirmed;

        private Dictionary<string, UIToggle> listToggles;

        public override void OnEnableFS() {
            base.OnEnableFS();
            //print(Application.systemLanguage.ToString());
            //StartCoroutine(ShowLoading());
        }

        public void Initialize() {
            lbMessage.cachedGameObject.SetActive(false);
            retryButton.cachedGameObject.SetActive(false);
            loadingAnimation.gameObject.SetActive(false);
            

            //var toggles = gameObject.GetComponentsInChildren<UIToggle>();
            //if(toggles != null) {
            //    listToggles = new Dictionary<string, UIToggle>(10);
            //    for(int i = 0; i < toggles.Length; i++) {
            //        listToggles.Add(toggles[i].gameObject.name.Substring(6), toggles[i]);
            //    }
            //}

            languageContainer.gameObject.SetActive(false);
            loadingContainer.gameObject.SetActive(false);

            //print('H');
        }

        public void SplashScreenFinished() {
            
        }

        internal void ShowLogo() {
            logoFadeOutTween.AddOnFinished(SplashScreenFinished);
            logoFadeOutTween.PlayReverse();
        }

        public void EnterGame () {
            SSSceneManager.Instance.LoadMenu(Scenes.MainMenu.GetName());
            SceneManager.Instance.OpenScene(Scenes.HomeUI);
        }

        public void UpdateLoadProgress(float p) {
            loadingBar.SetProgress(p);
        }

        public void ShowLoading() {
            loadingContainer.gameObject.SetActive(true);
            loadingAnimation.gameObject.SetActive(true);
            languageContainer.gameObject.SetActive(false);
        }

        public void ShowLanguageOptions (string defaultLanguage) {
            languageContainer.gameObject.SetActive(true);

            if (listToggles.ContainsKey(defaultLanguage)) {
                listToggles[defaultLanguage].value = true;
            }

            languageOptionTween.PlayReverse();
        }

        public void OnLanguageChanged () {
            if (UIToggle.current.value) {
                string language = UIToggle.current.gameObject.name.Substring(6);
                //Debug.Log(language);
                Localization.LoadAndSelect(language);
                GameManager.Instance.SetupGameFont(language);
            }
        }

        public void ConfirmLanguage () {
            Timing.RunCoroutine(C_ConfirmLanguage());
        }

        private IEnumerator<float> C_ConfirmLanguage () {
            languageOptionTween.PlayForward();
            yield return Timing.WaitForSeconds(languageOptionTween.duration);
            Helpers.Callback(OnLanguageConfirmed);
        }

        //public bool isTest = false;
        //void Update() {
        //    if (isTest) {
        //        isTest = false;
        //        lbMessage.alpha = 1;
        //        TweenAlpha.Begin(lbMessage.cachedGameObject, 0.5f, 0);
        //    }
        //}

        public void ShowMessage(string message) {
            //print("Showing message: " + message);
            lbMessage.text = message;
            lbMessage.cachedGameObject.SetActive(true);
            lbMessage.alpha = 0;
            TweenAlpha.Begin(lbMessage.cachedGameObject, durationFadeIn, 1);
            loadingAnimation.gameObject.SetActive(false);
        }

        public void HideMessage() {
            //print("Hiding message");
            lbMessage.alpha = 1;
            TweenAlpha.Begin(lbMessage.cachedGameObject, durationFadeOut, 0);
        }

        public void ShowRetryButton() {
            //print("Showing retry button");
            retryButton.cachedGameObject.SetActive(true);
            retryButton.alpha = 0;
            loadingAnimation.gameObject.SetActive(false);
            TweenAlpha.Begin(retryButton.cachedGameObject, durationFadeIn, 1);
        }

        public void HideRetryButton() {
            //print("Hiding retry button");
            retryButton.alpha = 1;
            
            loadingAnimation.gameObject.SetActive(true);
            TweenAlpha.Begin(retryButton.cachedGameObject, durationFadeOut, 0);
            retryButton.cachedGameObject.SetActive(false);
        }
    }
}