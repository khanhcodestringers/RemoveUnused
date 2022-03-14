using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mio.Utils.MessageBus;
using System;

namespace Mio.TileMaster {
    public enum ScoreType {
        Score,
        Star,
        Crown
    }
    public class HighScoreManager : MonoSingleton<HighScoreManager> {
        private List<ScoreItemModel> listHighscore;
        private bool isInit = false;

        private int totalStar = 0;
        public int TotalStars {
            get { return totalStar; }
        }

        private int totalCrown = 0;
        public int TotalCrowns {
            get { return totalCrown; }
        }

        private static Message MSG_USER_PLAY_RECORD_CHANGED = new Message(MessageBusType.UserPlayRecordChanged);

        public void Initialize() {
            if (!isInit) {
                //print("initializing highscore");
                //if(GameManager.Instance != null) {
                //print("hAAAAAAAAAAAAAAAAere");
                               
                //}
                //print("Initialized");
                isInit = true;
                MessageBus.Instance.Subscribe(MessageBusType.UserPlayRecordChanged, OnUserPlayRecordChanged);
            }

            RefreshMetric();
        }

        private void OnUserPlayRecordChanged (Message obj) {
            //throw new NotImplementedException();
            RefreshMetric();
        }

        private int oldNumCrown, oldNumStar;
        public void RefreshMetric() {
            if(!isInit) { Initialize(); }
            if (!isInit) return;

            listHighscore = ProfileHelper.Instance.ListHighScore;
            if (listHighscore == null) {
                Debug.LogWarning("Could not initialize high-score manager if player profile has not been initialized first");
                return;
            }

            oldNumCrown = totalCrown;
            oldNumStar = totalStar;
            totalCrown = totalStar = 0;
            //Debug.Log(string.Format("Refreshing metrics old crowns {0}, old stars {1}", oldNumCrown, oldNumStar));
            for(int i = 0; i < listHighscore.Count; i++) {
                totalStar +=  Mathf.Clamp(listHighscore[i].highestStar,0,3);
                totalCrown += listHighscore[i].highestCrown;
            }
            //Debug.Log(string.Format("Refreshing metrics new crowns {0}, new stars {1}", totalCrown, totalStar));
            if ((oldNumStar != totalStar) || (oldNumCrown != totalCrown)) {
                MessageBus.Annouce(MSG_USER_PLAY_RECORD_CHANGED);
            }
        }

        public int GetHighScore(string levelName, ScoreType type = ScoreType.Score) {
            // print("Get high-score for level " + levelName + " type: " + type);
            if (isInit) {
                for (int i = 0; i < listHighscore.Count; i++) {
                    if (listHighscore[i].itemName.Equals(levelName)) {
                        //print("Item found");
                        return GetScore(listHighscore[i], type);
                    }
                }
                //print("No item found");
            }

            return 0;
        }

        public bool UpdateHighScore(string levelName, int scoreValue, ScoreType type = ScoreType.Score) {
            //print("Updating high score for level " + levelName + " type: " + type + " value: " + scoreValue);
            if (isInit) {
                bool res = false;

                foreach (ScoreItemModel item in listHighscore) {
                    if (item.itemName.Equals(levelName)) {

                        int currentScore = GetScore(item, type);
                        if (currentScore >= scoreValue) {
                            res = false;
                        }
                        else {
                            SetScore(item, type, scoreValue);
                            res = true;
                        }
                        
                        //print(string.Format("Item found for level {0}, score type {1}, need update = {2}", levelName, type, res));
                        return res;
                    }
                }

                //if we reached this point, it's mean the item is not existed. We shall create one
                ScoreItemModel scoreitem = new ScoreItemModel();
                scoreitem.itemName = levelName;
                listHighscore.Add(scoreitem);
                SetScore(scoreitem, type, scoreValue);
                //print(string.Format("Create new highscore for level {0}, score type {1}, value = {2}", levelName, type, scoreValue));
                return true;
            }

            return false;
        }

        private int GetScore(ScoreItemModel model, ScoreType type) {
            switch (type) {
                case ScoreType.Crown:
                    return model.highestCrown;
                case ScoreType.Score:
                    return model.highestScore;
                case ScoreType.Star:
                    return model.highestStar;
                default:
                    return 0;
            }
        }

        public List<ScoreItemModel> GetCurrentHighScoreList() {
            return listHighscore;
        }

        private void SetScore(ScoreItemModel model, ScoreType type, int score) {
            switch (type) {
                case ScoreType.Crown:
                    model.highestCrown = score;
                    RefreshMetric();
                    break;
                case ScoreType.Score:
                    model.highestScore = score;
                    break;
                case ScoreType.Star:
                    model.highestStar = score;
                    RefreshMetric();
                    break;
            }

            
            //HACKATHON used to speed up programming
            //save game state right after update highscore
            //if (type == ScoreType.Score) {
            //    GameManager.Instance.SaveGameData();
            //}
        }
    }
}