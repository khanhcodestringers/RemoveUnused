using UnityEngine;
using System.Collections;

namespace Mio.TileMaster {
    public class ContinuePopUpController : SSController {
        public CountdownView countdown;

        [Header("Shown when user can reach new highscore")]
        [SerializeField]
        private RecordView starView;
        [SerializeField]
        private RecordView crownView;
        [SerializeField]
        private UILabel lbSupportiveMessage;

        [Header("Shown when user already has new highscore")]
        [SerializeField]
        private UILabel lbEncourageMessage;

        [Header("Price to continue")]
        [SerializeField]
        private UILabel lbPriceToContinue;


        private int priceToContinue = 0;
        private MainGameSceneController maingame;

        public override void OnKeyBack() {
            FinishGame();
        }

        public override void OnEnable() {
            base.OnEnable();
			InGameUIController.Instance.canTouch = false;
            StartCountDown();
        }

		public override void OnDisable() {
			base.OnDisable();
			InGameUIController.Instance.canTouch = true;
		}
        public void StartCountDown() {
            //count down for 8 seconds, then finish the game
            countdown.DoCountdown(8, 1, FinishGame);
        }

        public override void OnSet(object data) {
            base.OnSet(data);

            maingame = data as MainGameSceneController;

            starView.gameObject.SetActive(false);
            crownView.gameObject.SetActive(false);
            lbEncourageMessage.gameObject.SetActive(false);
            lbSupportiveMessage.gameObject.SetActive(false);

            //MidiPlayer.Instance.ShouldPlay = true;

            //get current star
            int star = Counter.GetQuantity(Counter.KeyStar);
            int currentScore = Counter.GetQuantity(Counter.KeyScore);
            int highScore = HighScoreManager.Instance.GetHighScore(GameManager.Instance.SessionData.currentLevel.songData.storeID, ScoreType.Score);
            int scoreTillNextHighScore = highScore - currentScore;
            int tilesTillNextStar = maingame.gamelogic.GetNumTileTillNextStar(star);

            priceToContinue = maingame.gamelogic.GetPriceForContinuePlaying();
            //how much does it cost to continue playing
            lbPriceToContinue.text = "x " + priceToContinue.ToString();

            if (scoreTillNextHighScore > 0) {
                if (tilesTillNextStar > scoreTillNextHighScore) {
                    lbEncourageMessage.gameObject.SetActive(true);
                    //lbEncourageMessage.text = string.Format("[4E4E4EFF]Only [FF6347]{0}[-] more \n to reach highscore.\n Continue?", scoreTillNextHighScore);
                    lbEncourageMessage.text = Localization.Get("pu_continue_encourage1") + scoreTillNextHighScore + Localization.Get("pu_continue_encourage2");
                }
                else {
                    if (star <= 5) {
                        ++star;
                        lbSupportiveMessage.gameObject.SetActive(true);
                        lbSupportiveMessage.text = Localization.Get("pu_continue_supportive1") + tilesTillNextStar + Localization.Get("pu_continue_supportive2");
                        if (star <= 3) {
                            starView.gameObject.SetActive(true);
                            starView.SetVisible(true);
                            starView.ShowNumRecord(star);
                        }
                        else {
                            crownView.gameObject.SetActive(true);
                            crownView.SetVisible(true);
                            crownView.ShowNumRecord(star - 3);
                        }
                    }
                }
            }
            else {
                lbEncourageMessage.gameObject.SetActive(true);
                //lbEncourageMessage.text = string.Format("[4E4E4EFF]New Highscore [FF6347]{0}[-]!!!\n Raise the bar higher ?", currentScore);
                lbEncourageMessage.text = Localization.Get("pu_continue_encourage3") + currentScore + Localization.Get("pu_continue_encourage4");
            }

        }

        /// <summary>
        /// Finish current game and go to result scene
        /// </summary>
        public void FinishGame() {

            countdown.StopCountdown();
            SceneManager.Instance.CloseScene();
            maingame.EndGame();
        }

        /// <summary>
        /// Continue to play this game, at current progress
        /// </summary>
        public void ContinueGame() {
            if (ProfileHelper.Instance.CurrentDiamond >= priceToContinue) {
                ProfileHelper.Instance.CurrentDiamond -= priceToContinue;

                countdown.StopCountdown();
                SceneManager.Instance.CloseScene();
                maingame.ContinueGame(false);
            }else {
                SceneManager.Instance.OpenPopup(ProjectConstants.Scenes.IAP);
				countdown.StopCountdown();
            }
        }
    }
}