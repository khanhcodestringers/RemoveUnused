using UnityEngine;
using System.Collections;

namespace Mio.TileMaster
{
    public class ResultLevelRankingItemView : MonoBehaviour
    {
        [SerializeField]
        private UILabel lbRank;
        [SerializeField]
		private UI2DSprite sprAvatar;
        [SerializeField]
        private UILabel lbFbName;
        [SerializeField]
        private UILabel lbScore;
        PlayerRankingModel m_playerRankingModel;

        public void InitUI(PlayerRankingModel _playerRankingModel,int index)
        {
            m_playerRankingModel = _playerRankingModel;
            lbRank.text = (index +1).ToString();
            lbFbName.text = _playerRankingModel.user_name;
            lbScore.text = _playerRankingModel.score.ToString();
            InitAvatar();
        }
        public void InitAvatar()
        {
    //        sprAvatar.transform.localScale = new Vector2 (180f,180f);
            if (m_playerRankingModel.avatar == null)
                LoadAvatar();
            else
                sprAvatar.sprite2D = m_playerRankingModel.avatar;

        }

        public void LoadAvatar()
        {
            string linkAvatar = "https://graph.facebook.com/" + m_playerRankingModel.user_id + "/picture?type=normal";
            AssetDownloader.Instance.DownloadAndCacheAsset(linkAvatar, 0,
                ((float p) =>
                {
                    //notthing
                }),
                ((string fail) =>
                {
                    Debug.Log("Error: " + fail);
                }),
                (WWW www) =>
                {
                    Texture2D texture = www.texture;
                    Rect rec = new Rect(0, 0, texture.width, texture.height);
                    m_playerRankingModel.avatar = Sprite.Create(texture, rec, new Vector2(0, 0));
                    InitAvatar();
                });
        }
    }
}
