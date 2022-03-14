using System;
using UnityEngine;
namespace Mio.TileMaster {
    [Serializable]
    public class PlayerRankingModel {
        public string user_id;
        public string user_name;
        public string song_id;
        public int totalStar;
        public int totalCrown;
        public int score;
        public Sprite avatar;
        public PlayerRankingModel (string user_id, string user_name, string song_id, int totalStar, int totalCrown, int score) {
            this.user_id = user_id;
            this.user_name = user_name;
            this.song_id = song_id;
            this.totalStar = totalStar;
            this.totalCrown = totalCrown;
            this.score = score;
        }
    }



    public class PlayerRankingComparer : System.Collections.Generic.IComparer<PlayerRankingModel> {
        public int Compare (PlayerRankingModel x, PlayerRankingModel y) {
            if (x.score > y.score) {
                return -1;
            }
            else if (x.score < y.score) {
                return 1;
            }
            return 0;
        }
    }
}