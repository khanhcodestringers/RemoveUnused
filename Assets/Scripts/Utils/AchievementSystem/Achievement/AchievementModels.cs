using System.Collections.Generic;

namespace Achievement {
    public class AchievementModel  {
        public string ID;
        public List<OperatorModel> listConditions;
        public string title;
        public string description;
        public string icon;
        public List<AchievementRewardModel> listReward;
        //public int rewardValue;
        //public string rewardType;
        //public long unixtimeUnlockDate;
        public bool isActive;
        public bool isHidden;
        public bool isClaimed;
        public bool isUnlocked;
        public string tag;
    }

    public class AchievementRewardModel {
        public string type;
        public int value;
    }

    public class OperatorModel {
        public string ID;
        public string propertyID;
        public string expressionString;
        public int targetValue;        
        public bool isSoloUse;
    }

    public class PropertyModel {
        public string ID;
        public int initValue;
        public int currentValue;
        public string tag;
    }
}