using UnityEngine;
using System.Collections;

namespace Mio.TileMaster {
    public class SessionDataModel {
        public LevelDataModel currentLevel;
        public LevelDataModel lastLevel;
        public LevelDataModel currentLevelOnline;
        public bool needAttentionAchievement;
        public bool needAttentionSongList;
    }
}