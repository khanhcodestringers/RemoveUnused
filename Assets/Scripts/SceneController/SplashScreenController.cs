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
        
        public void HideRetryButton() {
            //print("Hiding retry button");
            retryButton.alpha = 1;
            
            loadingAnimation.gameObject.SetActive(true);
            TweenAlpha.Begin(retryButton.cachedGameObject, durationFadeOut, 0);
            retryButton.cachedGameObject.SetActive(false);
        }
    }
}