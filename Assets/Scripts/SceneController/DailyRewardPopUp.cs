using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Mio.TileMaster {
    public class DailyRewardPopUp : SSController {
        [Header("List Daily reward")]
        [SerializeField]
        private List<DailyRewardItemView> listDailyItem;

        private int dayIndex;
        private string stringDateAndIndex;

        public override void OnSet (object data) {
            base.OnSet(data);
            stringDateAndIndex = (string)data;
            dayIndex = int.Parse(stringDateAndIndex.Split('_')[1]);
            //if (dayIndex > 7) {
            //    dayIndex = 7;
            //    stringDateAndIndex = stringDateAndIndex.Split('_')[0] + "_" + dayIndex;
            //}
            InitUI();
        }

        private void InitUI () {
            var diamondRewards = GameManager.Instance.GameConfigs.dailyrewardDiamonds;
            var lifeRewards = GameManager.Instance.GameConfigs.dailyrewardLives;

            //make sure list reward is fine and initialize if needed
            if (diamondRewards == null || diamondRewards.Count <= 0) {
                diamondRewards = new List<int> { 5, 7, 9, 0, 5, 7, 10 };
            }

            if (lifeRewards == null || lifeRewards.Count <= 0) {
                lifeRewards = new List<int> { 0, 0, 0, 3, 2, 3, 5 };
            }

            for (int i = 0; i < listDailyItem.Count; i++) {

                //a reward will be shown if it has diamond or live reward value
                bool canShow = false;
                if (i < diamondRewards.Count) {
                    canShow = true;
                    listDailyItem[i].diamondReward = diamondRewards[i];
                }

                if (i < lifeRewards.Count) {
                    canShow = true;
                    listDailyItem[i].liveReward = lifeRewards[i];
                }

                if (canShow) {
                    if (i < dayIndex) {
                        listDailyItem[i].InitUI(DailyRewardType.Claimed, stringDateAndIndex, i);
                    }
                    else if (i == dayIndex) {
                        listDailyItem[i].InitUI(DailyRewardType.Claimable, stringDateAndIndex, i);
                    }
                    else {
                        listDailyItem[i].InitUI(DailyRewardType.Unclaimable, stringDateAndIndex, i);
                    }
                }
            }
        }
        public void ClosePopUp () {
            SceneManager.Instance.CloseScene();
        }
    }
}
