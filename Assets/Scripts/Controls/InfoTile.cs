using UnityEngine;
using System.Collections;

namespace Mio.TileMaster {
    public class InfoTile : MonoBehaviour {
        public UILabel lbSongTitle;
        public UILabel lbHighScore;

        public void SetSongInfo(string title, int score) {
            lbHighScore.text = score.ToString();
            lbSongTitle.text = title;
        }
    }
}