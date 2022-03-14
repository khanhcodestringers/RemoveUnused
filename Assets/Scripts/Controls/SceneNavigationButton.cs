using UnityEngine;
using System.Collections;
using ProjectConstants;
using Mio.Utils;

namespace Mio.TileMaster {
    public class SceneNavigationButton : MonoBehaviour {
        public Scenes navigateTo = Scenes.HomeUI;
        
        public void NavigateToSpecifiedScene() {
            //Utils.AnalyticsHelper.Instance.LogOpenScene(navigateTo.GetName());
            //Debug.Log("Navigating to " + navigateTo);
            if (navigateTo == Scenes.JoinRoom && ProfileHelper.Instance.CurrentDiamond < GameManager.Instance.GameConfigs.rubyForOnline)
            {
                SceneManager.Instance.OpenPopup(Scenes.MessageInviteFriends,
                    new MessageBoxDataModel(Localization.Get("182") + GameManager.Instance.GameConfigs.rubyForOnline + Localization.Get("183"), Localization.Get("exit_online"),
                        () =>
                        {
                            SceneManager.Instance.CloseScene();
                            SceneManager.Instance.OpenScene(Scenes.HomeUI);
                            SceneManager.Instance.OnMenuHome();
                        }, Localization.Get("227"), () =>
                        {
                            SceneManager.Instance.CloseScene();

                        }));

            }
            else
            {
                SceneManager.Instance.OpenScene(navigateTo, null, true);
            }
        }

        /// <summary>
        /// Default behaviour for NGUI
        /// </summary>
        public void OnClick() {
            //Debug.Log("Clicked on button navigation");
            NavigateToSpecifiedScene();
        }
    }

}