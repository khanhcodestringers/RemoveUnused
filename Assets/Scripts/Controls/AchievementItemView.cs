using UnityEngine;
using System.Collections;
using Achievement;
using System;

namespace Mio.TileMaster {
    public class AchievementItemView : MonoBehaviour {
        public event Action<AchievementItemView> OnClaimAchievement;

        public const string MESSAGE_HIDDEN_ACHIEVEMENT = "Hidden. Continue playing, and maybe, with some luck, you can unlock it";
        public static readonly Color colorbgClaimed = new Color(215/255f,215/255f,215/255f);
        public static readonly Color colorbgNormal = new Color(1,1,1);

        [Header("Achievement's information")]
        //[SerializeField]
        //private UISprite imgBackground;
        //[SerializeField]
        //private UISprite imgIcon;
        [SerializeField]
        private UILabel lbTitle;
        [SerializeField]
        private UILabel lbDescription;
        [SerializeField]
        private UISprite imgProgress;
        [SerializeField]
        private UISprite imgCompletedStamp;
        [SerializeField]
        private UISprite imgOverlay;

        private AchievementModel model;
        public AchievementModel Model { get { return model; } set { model = value; RefreshItemView(); } }        
		public int index;

        [Header("Rewards")]
        [SerializeField]
        private UIGrid rewardDetailsContainer;
        [SerializeField]
        private GameObject rewardPrefab;
        [SerializeField]
        private RewardDetailView rewardDetail;

        [Header("Interaction")]
        [SerializeField]
        private UIButton btnClaim;
        
        public void RefreshItemView() {
            if(model != null) {
                imgProgress.gameObject.SetActive(true);
                btnClaim.gameObject.SetActive(false);
                //imgBackground.color = colorbgNormal;
                //rewardDetail.gameObject.SetActive(false);
                imgCompletedStamp.gameObject.SetActive(false);

                //set icon image
                //imgIcon.spriteName = model.icon;
                //set title
                lbTitle.text = Localization.Get(model.title);
                lbDescription.text = Localization.Get(model.description);
                //if (model.isHidden && !model.isUnlocked) {
                //    //set description
                //    lbDescription.text = MESSAGE_HIDDEN_ACHIEVEMENT;
                //}
                //else {
                //    lbDescription.text = Localization.Get(model.description);
                //}

                //if achievement is unlocked
                if (model.isUnlocked) {
                    imgProgress.gameObject.SetActive(false);
                    imgOverlay.cachedGameObject.SetActive(false);
                    //and claimed
                    if (model.isClaimed) {
                        //print("Achievement claimed");
                        //show the stamp
                        imgCompletedStamp.gameObject.SetActive(true);
//						rewardDetail.gameObject.SetActive(false);
                        //imgBackground.color = colorbgClaimed;                        
                    }
                    //if not claimed, show the claim button
                    else {
                        btnClaim.gameObject.SetActive(true);
                    }
                }
                //if the achievement is not unlocked
                else {
                    imgOverlay.cachedGameObject.SetActive(true);
                    if (model.listReward.Count > 0) {
                        //show the reward details
//                        rewardDetail.gameObject.SetActive(true);
//                        rewardDetail.SetReward(model.listReward[0].type, model.listReward[0].value.ToString());
                        imgProgress.gameObject.SetActive(true);
                        //imgProgress.fillAmount = 1 - AchievementHelper.Instance.GetAchievementProgress(model.ID);
                        //Debug.Log("Setting progress for achievement: " + model.ID + " with value: " + AchievementHelper.Instance.GetAchievementProgress(model.ID));
                    }
                }
            }
			rewardDetail.SetReward(model.listReward[0].type, model.listReward[0].value.ToString());
        }

        public void OnClaimButtonClicked() {
            Helpers.CallbackWithValue(OnClaimAchievement, this);
        }
    }
}