using UnityEngine;
using System.Collections;
using Mio.Utils;

namespace Mio.TileMaster {
    public enum DailyRewardType {
        Claimed,
        Claimable,
        Unclaimable
    }
    public class DailyRewardItemView : MonoBehaviour {

        [Header("UI")]
        [SerializeField]
        private UISprite blurBG;
        [SerializeField]
        private UISprite completedClaim;
        [SerializeField]
        private GameObject btnClaim;
        [SerializeField]
        private UILabel lblDiamond;
        [SerializeField]
        private UILabel lblHeart;
        [SerializeField]
        private GameObject diamondObj;
        [SerializeField]
        private GameObject heartObj;
        [SerializeField]
        private UILabel lblDate;
        [Header("Values")]
        public int diamondReward;
        public int liveReward;
        private string dayAndIndex;

        public void InitUI (DailyRewardType _dailyType, string _dayAndIndex, int dayindex) {
            dayAndIndex = _dayAndIndex;
            lblDate.text = Localization.Get("pu_daily_day") + (dayindex + 1);
            if (diamondReward == 0) {
                diamondObj.SetActive(false);
                heartObj.transform.localPosition = new Vector3(heartObj.transform.localPosition.x - 52, heartObj.transform.localPosition.y);
            }
            else {
                lblDiamond.text = "x" + diamondReward.ToString();
            }

            if (liveReward == 0) {
                heartObj.SetActive(false);
                diamondObj.transform.localPosition = new Vector3(heartObj.transform.localPosition.x - 52, heartObj.transform.localPosition.y);
            }
            else {
                lblHeart.text = "x" + liveReward.ToString();
            }

            if (_dailyType == DailyRewardType.Claimed) {
                blurBG.cachedGameObject.SetActive(false);
                completedClaim.cachedGameObject.SetActive(true);
                btnClaim.SetActive(false);
            }
            else if (_dailyType == DailyRewardType.Claimable) {
                blurBG.cachedGameObject.SetActive(false);
                completedClaim.cachedGameObject.SetActive(false);
                btnClaim.SetActive(true);
            }
            else {
                blurBG.cachedGameObject.SetActive(true);
                completedClaim.cachedGameObject.SetActive(false);
                btnClaim.SetActive(false);
            }
        }

        public void Claim () {
            ProfileHelper.Instance.CurrentDiamond += diamondReward;
            ProfileHelper.Instance.CurrentLife += liveReward;

            PlayerPrefs.SetString(GameConsts.KEY_DAILY_REWARD, dayAndIndex);
            PlayerPrefs.Save();
            SceneManager.Instance.CloseScene();
        }
    }
}
