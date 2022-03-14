using UnityEngine;
using System.Collections;
using Mio.Utils;
using Mio.Utils.MessageBus;
using System;
//using Facebook.Unity;
//using Parse;
using MovementEffects;
using System.Collections.Generic;


namespace Mio.TileMaster {
    public class SettingSceneController : SSController {
        //public UILabel lbIPAddress;

        //public UI2DSprite avatar;

        //private FeedbackDataModel feedback;

        //private FeedbackDataModel requestSong;

        //[SerializeField]
        //private UILabel lbFacebookButton;

        //[SerializeField]
        //private UIToggle toggleSwitchAudioType;

        [SerializeField]
        private UIWrapContent wrapItemView;

        //public GameObject objSwitchAudioType;

        public Animator anmController;

        //[SerializeField]
        //private GameObject objCrossPromotion;
        [SerializeField]
        private UITable compTable;


        //private string urlCrossPromotion = string.Empty;
        private bool isFirstStart = true;

        public override void OnKeyBack () {
            SceneManager.Instance.OpenExitConfirmation();
        }

        public override void OnEnableFS () {
            base.OnEnableFS();
        }

        public override void OnEnable () {
            base.OnEnable();
            anmController.SetTrigger("showtoleft");
            SceneManager.Instance.onSceneChange += OnSceneChange;


        }

        public override void OnDisable () {
            base.OnDisable();
            SceneManager.Instance.onSceneChange -= OnSceneChange;

        }

        private IEnumerator<float> C_RefreshTableUI () {
            yield return 1;
            compTable.enabled = true;
            compTable.Reposition();
            
        }

        public void OnSoundTogglePressed (bool enableSound) {
            //print("Mute: " + enableSound);
            AudioManager.Instance.MuteSFX = !enableSound;
        }

        public void OpenCrossPromotion () {

        }

        private void OnSceneChange (ProjectConstants.Scenes scene) {
            //Debug.Log("change");
            anmController.SetTrigger("hidetoright");
        }

        public void OnInforButtonClicked () {
            Debug.Log("Showing test ads");

        }

        
        public void OnChooseLanuageClicked () {
            SceneManager.Instance.OpenPopup(ProjectConstants.Scenes.ChooseLanguagePopUp);
        }

        public void SwitchAudioTypeChange () {
            Timing.RunCoroutine(IEWaitForSwitchAudioType());
        }

        public void OnNoAdsClicked() {
            SceneManager.Instance.OpenPopup(ProjectConstants.Scenes.RemoveAdsPopUp);
        }

        //show loading image in result screen 
        private IEnumerator<float> IEWaitForSwitchAudioType () {
            if (!isFirstStart) {
                yield return Timing.WaitForSeconds(0.1f);                
            }

            isFirstStart = false;
        }
    }
}