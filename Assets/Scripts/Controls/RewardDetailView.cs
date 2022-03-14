using UnityEngine;
using System.Collections;

namespace Mio.TileMaster {
    public class RewardDetailView : MonoBehaviour {
        [SerializeField]
        private UISprite rewardIcon;
        [SerializeField]
        private UILabel lbRewardValue;

        public void SetReward(string iconSpriteName, string rewardValue) {
            if(rewardIcon != null) {
                rewardIcon.spriteName = iconSpriteName;
            }

            if(lbRewardValue != null) {
                lbRewardValue.text = rewardValue;
            }
        }
    }
}