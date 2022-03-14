using UnityEngine;
using System.Collections;

namespace Mio.TileMaster {
    public class PopupButton : MonoBehaviour {
        [SerializeField]
        private ProjectConstants.Scenes popupScene;

        public void OpenSpecifiedPopup() {
            SceneManager.Instance.OpenPopup(popupScene);
        }
    }
}