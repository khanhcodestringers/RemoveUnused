using UnityEngine;
using System.Collections;

namespace Mio.TileMaster {
    public class TileMasterSceneManager : SSSceneManager {

        protected override void OnFirstSceneLoad() {
            base.OnFirstSceneLoad();

            //print("First scene loaded");
            //Application.targetFrameRate = 30;
            //SSSceneManager.Instance.LoadMenu("MainMenuUI");
            SceneManager.Instance.OpenScene(ProjectConstants.Scenes.SplashScreen);
        }
                

        public void LogoutParseUser () {
            //Parse.ParseUser.LogOutAsync();
        }
    }
}