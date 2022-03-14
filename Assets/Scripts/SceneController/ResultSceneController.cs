using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mio.Utils.MessageBus;
using Mio.Utils;
//using UnityParseHelpers;
//using Facebook.Unity;
//using Parse;
using MovementEffects;
using DG.Tweening;
using UnityEngine.UI;
using System;
using ProjectConstants;

namespace Mio.TileMaster {
    public class ResultSceneController : SSController {
        public Animator animatorBestScoreEffect;
        public SongItemView songInfo;
        public UILabel lbScore;
        public UILabel lbHighScore;
        public UISprite imgNewBest;
        public UIGrid gridLevalRankingItem;

        public Animation anmEffectMoveButton;        

        public GameObject contentLoggedIn;
        public GameObject contentNotLoggedIn;
        public GameObject levelRankingObj;
        public GameObject loadingObj;

        private static Message saveUserDataMessage = new Message(MessageBusType.UserDataChanged);
        private static Message gameDataChangedMessage = new Message(MessageBusType.GameDataChanged);

        private int score, star, crown;

        public const int SOUND_PIANO_FINISHED_SCORING = 88;
        public const string TEXT_0 = "0";
        //private bool isNewBestScore;
        private bool isNavigatingAway = false;
        private bool hasNewSongsUnlocked = false;


        public override void OnDisable () {
            base.OnDisable();
            anmEffectMoveButton.Stop();
        }


        public override void OnEnable () {
            base.OnEnable();

            ResetScene();
            GameConsts.isPlayAuto = false;

            DisplaySongResult();
			// AchievementHelper.Instance.LogAchievement("songFinish");
        }

        private void ResetScene () {
            isNavigatingAway = false;
            anmEffectMoveButton.Play("resetAnimations");
            hasNewSongsUnlocked = false;
            lbScore.text = TEXT_0;
        }

        private void DisplaySongResult () {
            MidiPlayer.Instance.ShouldPlay = true;
            animatorBestScoreEffect.gameObject.SetActive(false);

            score = Counter.GetQuantity(Counter.KeyScore);
            star = Mathf.Clamp(Counter.GetQuantity(Counter.KeyStar), 0, 3);
            crown = Counter.GetQuantity(Counter.KeyStar) - 3;
            if (crown < 0) crown = 0;

            //return 1 live for user if they haven't score any point at all
            if (score <= 0) {
                ProfileHelper.Instance.CurrentLife += 1;
            }

            //set song title
            songInfo.SetTitle(GameManager.Instance.SessionData.currentLevel.songData.name);
            
            int animatingScore = 0;
            //update score with a little animation
            DOTween.To(() => animatingScore, x => animatingScore = x, score, 1f)                
                .OnUpdate(() => {
                    lbScore.text = animatingScore.ToString();
                    //MidiPlayer.Instance.PlayPianoNote(SOUND_PIANO_FINISHED_SCORING);
                })
                .SetDelay(0.5f)
                //.OnComplete(() => { MidiPlayer.Instance.PlayPianoNote(SOUND_PIANO_FINISHED_SCORING); })
                .Play();

            if (crown > 0) {
                songInfo.SetNumCrowns(crown);
            }
            else {
                songInfo.SetNumStars(star);
            }

            string songID = GameManager.Instance.SessionData.currentLevel.songData.storeID;
            bool isNewBestScore = false;

            //update last play levels
            GameManager.Instance.StoreLastPlayLevel();

            //update highscore
            if (HighScoreManager.Instance.UpdateHighScore(songID, score)) {
                isNewBestScore = true;
                //check and show newly unlocked songs
                Timing.RunCoroutine(C_ShowUnlockedSongs(2.5f));
                //only update crown and star if getting a new highscore
                if (crown > 0) {
                    if (HighScoreManager.Instance.UpdateHighScore(songID, crown, ScoreType.Crown)) {
                        //if this is the first time reach 3 crowns, also set achievement
                        if (crown == 3) {
                            // AchievementHelper.Instance.LogAchievement("song6Stars");
                        }
                    };
                }
                if (star > 0) {
                    if (HighScoreManager.Instance.UpdateHighScore(songID, star, ScoreType.Star)) {
                        //if this is the first time reach 3 stars, also set achievement
                        if (star == 3) {
                            //AchievementHelper.Instance.LogAchievement("song3Stars");
                        }
                    }
                }
            }

            //MidiPlayer.Instance.ShouldPlay = false;
            MessageBus.Annouce(saveUserDataMessage);

            ProfileHelper.Instance.PushUserData(true);

            if (!isNewBestScore) {
                lbHighScore.text = Localization.Get("maingame_highscore") + " " + HighScoreManager.Instance.GetHighScore(songID).ToString();
            }
            else {
                lbHighScore.text = "";
                if (score > 0) {
                    Timing.RunCoroutine(C_ShowNewBest(2f));
                }
            }

            //achievement logging
            //if (crown == 3) AchievementHelper.Instance.LogAchievement("turn6Stars");
            //if (star == 3) AchievementHelper.Instance.LogAchievement("turn3Stars");
            MessageBus.Annouce(gameDataChangedMessage);

            float delaySceneAnimation = isNewBestScore ? 3f : 2f;
            Timing.RunCoroutine(C_ShowSceneAnimation(delaySceneAnimation));
        }


        private IEnumerator<float> C_ShowSceneAnimation (float delay) {
            yield return Timing.WaitForSeconds(delay);

            //wait for the "new songs unlocked" popup to be closed before showing more control
            if (hasNewSongsUnlocked) {
                yield return Timing.WaitForSeconds(1.5f);                
                while (!SceneManager.Instance.LastOpenSceneName.Contains(Scenes.ResultUI.GetName())) {
                    yield return Timing.WaitForSeconds(0.1f);
                }
                yield return Timing.WaitForSeconds(0.8f);
            }

            anmEffectMoveButton.Play("ResultSceneTranlateButton");
            //wait more here to allow the ads to be loaded
            yield return Timing.WaitForSeconds(2f);
            anmEffectMoveButton.Play("controlButtons");

        }

        private IEnumerator<float> C_ShowNewBest (float delay) {
            yield return Timing.WaitForSeconds(delay);
            animatorBestScoreEffect.gameObject.SetActive(true);
            animatorBestScoreEffect.SetTrigger("NewBest");
            //TODO: play new best sound here
        }

        private IEnumerator<float> C_ShowUnlockedSongs (float delay) {
            //record current number of stars 
            int oldStar = HighScoreManager.Instance.TotalStars;
            //wait 1 frame to collect new star record as other actions are going on
            yield return 1;
            
            int newStar = HighScoreManager.Instance.TotalStars;

            if (isNavigatingAway) { yield break; }

            //check for newly unlocked songs with star required falls between oldStar and newStar
            if (oldStar < newStar) {
                //list of newly unlocked songs
                List<SongDataModel> unlockedSongs = new List<SongDataModel>(5);

                var allsongs = GameManager.Instance.StoreData.listAllSongs;
                for (int i = 0; i < allsongs.Count; i++) {
                    if ((allsongs[i].starsToUnlock > oldStar) && (allsongs[i].starsToUnlock <= newStar)) {
                        if (!IsBought(allsongs[i].storeID)) {
                            unlockedSongs.Add(allsongs[i]);
                        }
                    }
                }


                //prepare to open unlocked scene
                NewlyUnlockedSongModel songs = new NewlyUnlockedSongModel();
                songs.newlyUnlockedSongs = unlockedSongs;

                if (unlockedSongs.Count >= 1) {
                    hasNewSongsUnlocked = true;
                    GameManager.Instance.SessionData.needAttentionSongList = true;
                    yield return Timing.WaitForSeconds(delay);
					// AchievementHelper.Instance.LogAchievement("numSongPlayed",unlockedSongs.Count);
                    //call open unlocked scene
                    SceneManager.Instance.OpenPopup(ProjectConstants.Scenes.SongUnlockedPopup, songs);
                }
            }
        }

        private bool IsBought (string storeID) {
            if (ProfileHelper.Instance.ListBoughtSongs == null) {
                return false;
            }

            for (int i = 0; i < ProfileHelper.Instance.ListBoughtSongs.Count; i++) {
                if (ProfileHelper.Instance.ListBoughtSongs[i].CompareTo(storeID) == 0) {
                    return true;
                }
            }

            return false;
        }

        //Testing purpose
        public void ShowScoreAnimation () {
            int animatingScore = 0;
            //int loop = 0;
            //update score with a little animation
            DOTween.To(() => animatingScore, x => animatingScore = x, UnityEngine.Random.Range(1000, 5000), 0.8f)
                .OnUpdate(() => {
                    lbScore.text = animatingScore.ToString();
                })
                .Play();
        }

        public void Replay () {
           
            if (LivesManager.Instance.CanPlaySong()) {
                isNavigatingAway = true;
                SceneManager.Instance.OpenScene(ProjectConstants.Scenes.MainGame, "replay");
            }
            else {
                LivesManager.Instance.SuggestRechargeLives();
            }
        }

        public void ShowVideoReward () {

        }

        public void Menu () {           
            isNavigatingAway = true;
            SceneManager.Instance.OpenScene(ProjectConstants.Scenes.SongList);
        }
   
    }
}