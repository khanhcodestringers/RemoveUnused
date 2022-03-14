using UnityEngine;
using System.Collections;
using Mio.TileMaster;

namespace Mio {
    public class RateButton : MonoBehaviour {
        //public string url_iOS;
        //public string url_Android;

//        private string url;
//        // Use this for initialization
//        void Start() {
//#if UNITY_IOS
//            url = url_iOS;
//#elif UNITY_ANDROID
//            url = url_Android;
//#endif
//        }

        public void OpenExternalRatingUI() {

            SceneManager.Instance.OpenPopup(ProjectConstants.Scenes.RatePopUp);
            //Application.OpenURL(url);
        }
    }
}