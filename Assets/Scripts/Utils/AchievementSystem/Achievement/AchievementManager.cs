using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mio.TileMaster;

namespace Achievement {
    public class AchievementManager {
        // activation rules
        public const string ACTIVE_IF_GREATER_THAN = ">";
        public const string ACTIVE_IF_LESS_THAN = "<";
        public const string ACTIVE_IF_EQUALS_TO = "=";

        public event Action<Achievement> OnAchievementUnlocked;


        #region Add/Remove Achievements
        //a list of all achievements' name, used when traversed through all achievement. Since list has better performance than dictionary when doing such operation
        private List<string> m_AchievementsIDs;
        private Dictionary<string, Achievement> m_Achievements;
        public Dictionary<string, Achievement> Achievements {
            get { return m_Achievements; }
            set { m_Achievements = value; }
        }

        private Dictionary<string, List<Achievement>> m_AchievementsByTag;

        public List<AchievementModel> GetAllAchievementsData() {
            List<AchievementModel> res = new List<AchievementModel>(m_AchievementsIDs.Count);
            //foreach (string id in m_AchievementsIDs) {
            //    res.Add(Achievements[id].Data);
            //}
            for(int i = 0; i < m_AchievementsIDs.Count; i++) {
                res.Add(Achievements[m_AchievementsIDs[i]].Data);
            }

            return res;
        }

        public List<AchievementModel> GetAllAchievementsDataWithTag (string tag) {
            List<AchievementModel> res = new List<AchievementModel>(m_AchievementsIDs.Count);
            //Debug.Log("getting achievement with tag: " + tag);
            for(int i = 0; i < m_AchievementsIDs.Count; i++) {
                if(string.IsNullOrEmpty(tag) && string.IsNullOrEmpty(Achievements[m_AchievementsIDs[i]].Data.tag)) {
                    res.Add(Achievements[m_AchievementsIDs[i]].Data);
                    //Debug.Log("Added for " + tag + " with achievement " + Achievements[m_AchievementsIDs[i]].Data.title);
                }
                else if (!string.IsNullOrEmpty(tag) && Achievements[m_AchievementsIDs[i]].Data.tag.Contains(tag)) {
                    res.Add(Achievements[m_AchievementsIDs[i]].Data);
                    //Debug.Log("Added for " + tag + " with achievement " + Achievements[m_AchievementsIDs[i]].Data.title);
                }
            }

            return res;
        }

        private void AddAchievement(AchievementModel data) {
            if (data != null) {
                if (!Achievements.ContainsKey(data.ID)) {
                    Achievement ach = Achievement.CreateAchievement(data, this);
                    if (ach != null) {
                        ach.OnAchievementCompleted += OnAchievementCompleted;
                        Achievements.Add(data.ID, ach);
                        m_AchievementsIDs.Add(data.ID);

                        //save reference for tagged properties 
                        if (!string.IsNullOrEmpty(data.tag)) {
                            if (!m_AchievementsByTag.ContainsKey(data.tag)) {
                                m_AchievementsByTag.Add(data.tag, new List<Achievement>(10));
                            }

                            m_AchievementsByTag[data.tag].Add(ach);
                        }
                    }
                }
                else {
                    Debug.Log("Trying to add new achievement, but the collection already contains the same property: " + data.ID);
                }
            }
            else {
                Debug.Log("Trying to add new achievement with NULL data, skipping...");
            }
        }

        private void OnAchievementCompleted(Achievement ach) {
            Debug.Log("Achievement Completed: " + ach.DisplayName);
            if(OnAchievementUnlocked != null) {
                OnAchievementUnlocked(ach);
            }
        }

        private Achievement GetAchievement(string id) {
            if (Achievements.ContainsKey(id)) {
                return Achievements[id];
            }

            Debug.Log("Could NOT found achievement: " + id + ", returning null...");
            return null;
        }
        #endregion

        #region Add/Remove Operators
        private List<string> m_OperatorsIDs;
        private Dictionary<string, Operator> m_Operators;
        public Dictionary<string, Operator> Operators {
            get { return m_Operators; }
            private set { m_Operators = value; }
        }
        internal Operator AddOperator(OperatorModel data) {
            if (data != null) {
                data.ID =  data.propertyID + data.expressionString + data.targetValue.ToString();
                if (!Operators.ContainsKey(data.ID)) {
                    Property prop = GetProperty(data.propertyID);
                    if (prop == null) {
                        Debug.Log("Could not find property: " + data.propertyID + ", can't add operator..." + data.ID);
                        return null;
                    }
                    else {
                        //Debug.Log("Creating new operator " + data.ID);
                        Operator op = new Operator(data, prop);
                        
                        Operators.Add(data.ID, op);
                        m_OperatorsIDs.Add(data.ID);

                        return op;
                    }                    
                }
                else {
                    //Debug.Log("Trying to add new Operator, but the collection already contains the same property: " + data.ID + ", returning existed one...");
                    return Operators[data.ID];
                }
            }
            else {
                Debug.LogError("Trying to add new operator with NULL data, returning NULL...");
                return null;
            }
        }

        internal float GetAchievementProgress(string achievementID) {
            if (Achievements.ContainsKey(achievementID)) {
                return Achievements[achievementID].GetCurrentProgress();
            }

            return 0;
        }
        #endregion

        #region Add/Remove Properties
        //a list of all properties' name, used when traversed through all achievement. Since list has better performance than dictionary when doing such operation
        private List<string> m_PropertiesIDs;
        private Dictionary<string, List<Property>> m_PropertiesByTag;
        private Dictionary<string, Property> m_Properties;
        public Dictionary<string, Property> Properties {
            get { return m_Properties; }
            set { m_Properties = value; }
        }

        /// <summary>
        /// Add new achievement property, based on specified model
        /// </summary>
        /// <param name="propData">Model data of achievement property</param>
        /// <returns>The newly created property, or the existed one</returns>
        private Property AddAchievementProperty(PropertyModel propData) {
            if (propData != null) {
                if (!Properties.ContainsKey(propData.ID)) {
                    Property property = new Property(propData);
                    Properties.Add(propData.ID, property);
                    m_PropertiesIDs.Add(propData.ID);
                    
                    //save reference for tagged properties 
                    if (!string.IsNullOrEmpty(propData.tag)) {
                        if (!m_PropertiesByTag.ContainsKey(propData.tag)) {
                            m_PropertiesByTag.Add(propData.tag, new List<Property>(10));
                        }

                        m_PropertiesByTag[propData.tag].Add(property);
                    }
                    //Debug.Log("Added property: " + propData.ID + " initial value: " + propData.initValue);
                    return property;
                }
                else {
                    Debug.Log("Trying to add new achievement property, but the collection already contains the same property: " + propData.ID + ", returning existed one.");
                    return Properties[propData.ID];
                }
            }
            else {
                Debug.LogWarning("Trying to add new achievement property with NULL data, returning NULL");
                return null;
            }
        }

        /// <summary>
        /// Get a List of all properties' data
        /// </summary>
        public List<PropertyModel> GetAllPropertiesData() {
            List<PropertyModel> res = new List<PropertyModel>(m_PropertiesIDs.Count);
            for(int i = 0; i < m_PropertiesIDs.Count; i++) {
                res.Add(Properties[m_PropertiesIDs[i]].Data);
            }

            return res;
        }

        private Property GetProperty(string id) {
            if (Properties.ContainsKey(id)) {
                return Properties[id];
            }

            Debug.LogWarning("Could NOT found achievement property: " + id + ", returning null...");
            return null;
        }

        public int GetPropertyValue(string propertyID) {
            if (Properties.ContainsKey(propertyID)) {
                return Properties[propertyID].Value;
            }

            Debug.Log("Trying to get value of not-existing property: " + propertyID + " returning -1");
            return -1;
        }

        /// <summary>
        /// Increase a property's value
        /// </summary>
        /// <param name="propertyID">The property to manipulate</param>
        /// <param name="value">The value to increase</param>
        /// <param name="noCallback">Currently not in use, will do in un-forseeable future</param>
        public void IncreaseAchievementProperty(string propertyID, int value = 1, bool noCallback = false) {
            if (Properties.ContainsKey(propertyID)) {
                //Debug.Log("Increasing value of property: " + propertyID + " by " + value);
                Properties[propertyID].Value += value;
            }
            else {
                Debug.Log("Trying to increase value of not-existing property: " + propertyID + ", skipping...");
            }
        }
        public void SetPropertyValue(string propertyID, int value) {
            if (Properties.ContainsKey(propertyID)) {
                Properties[propertyID].Value = value;
            }
            else {
                Debug.Log("Trying to set value of not-existing property: " + propertyID + ", skipping...");
            }
        }

        internal void SetPropertyValue(List<string> propertiesIDs, List<int> values) {
            if (propertiesIDs.Count != values.Count) {
                Debug.LogError(string.Format("Size mismatched: List of properties ({0}) and list of values ({1}) are not identical in size, skipping...", propertiesIDs.Count, values.Count));
                return;
            }

            for (int i = 0; i < propertiesIDs.Count; i++) {
                SetPropertyValue(propertiesIDs[i], values[i]);
            }
        }
        #endregion


        public AchievementManager() {
            
        }

        internal void Initialize(List<PropertyModel> props, List<AchievementModel> achs) {
            Achievements = new Dictionary<string, Achievement>(50);
            m_AchievementsIDs = new List<string>(50);

            Operators = new Dictionary<string, Operator>(50);
            m_OperatorsIDs = new List<string>(50);

            Properties = new Dictionary<string, Property>(10);
            m_PropertiesIDs = new List<string>(10);

            m_PropertiesByTag = new Dictionary<string, List<Property>>(10);
            m_AchievementsByTag = new Dictionary<string, List<Achievement>>(20);

            InitializeProperties(props);
            InitializeAchievements(achs);
        }

        private void InitializeProperties(List<PropertyModel> props) {
            for(int i  = 0; i < props.Count; i++) {
                //Debug.Log("Adding property: " + props[i].ID);
                AddAchievementProperty(props[i]);
            }
            ////Test purpose only.
            //PropertyModel p1 = new PropertyModel();
            //p1.ID = "songfinished";
            //p1.initValue = 0;
            //p1.currentValue = 0;
            //AddAchievementProperty(p1);

            //PropertyModel p2 = new PropertyModel();
            //p2.ID = "songowned";
            //p2.initValue = 2;
            //p2.currentValue = 2;
            //AddAchievementProperty(p2);

            //PropertyModel p3 = new PropertyModel();
            //p3.ID = "5star";
            //p3.initValue = 0;
            //p3.currentValue = 0;
            //AddAchievementProperty(p3);
        }

        private void InitializeAchievements(List<AchievementModel> achs) {
            for(int i = 0; i < achs.Count; i++) {
                //Debug.Log("Adding achievement: " + achs[i].title);
                AddAchievement(achs[i]);
            }
            //OperatorModel buy5song = new OperatorModel();
            //buy5song.expressionString = AchievementManager.ACTIVE_IF_EQUALS_TO;
            //buy5song.targetValue = 5;
            //buy5song.propertyID = "songowned";

            //OperatorModel own3song = new OperatorModel();
            //own3song.expressionString = AchievementManager.ACTIVE_IF_EQUALS_TO;
            //own3song.targetValue = 3;
            //own3song.propertyID = "songowned";

            //OperatorModel play3song = new OperatorModel();
            //play3song.expressionString = AchievementManager.ACTIVE_IF_EQUALS_TO;
            //play3song.targetValue = 3;
            //play3song.propertyID = "songfinished";

            //OperatorModel get3songfullstar = new OperatorModel();
            //get3songfullstar.expressionString = AchievementManager.ACTIVE_IF_EQUALS_TO;
            //get3songfullstar.targetValue = 3;
            //get3songfullstar.propertyID = "5star";


            //AchievementModel a1 = new AchievementModel();
            //a1.title = "Buy 5 song";
            //a1.ID = "buy5song";
            //a1.isActive = true;
            //a1.isUnlocked = false;
            //a1.listConditions = new List<OperatorModel>() { buy5song };
            //AddAchievement(a1);

            //AchievementModel a2 = new AchievementModel();
            //a2.title = "Get 5 stars in 3 songs";
            //a2.ID = "5star3song";
            //a2.isActive = true;
            //a2.isUnlocked = false;
            //a2.listConditions = new List<OperatorModel>() { get3songfullstar };
            //AddAchievement(a2);

            //AchievementModel a3 = new AchievementModel();
            //a3.title = "One by one";
            //a3.ID = "1by1";
            //a3.isActive = true;
            //a3.isUnlocked = false;
            //a3.listConditions = new List<OperatorModel>() { own3song, play3song };
            //AddAchievement(a3);
        }

        public void MarkAchievementAsUnlocked(string id) {
            if (Achievements.ContainsKey(id)) {
                Achievements[id].Data.isUnlocked = true;
            }
        }

        public void MarkAchievementAsClaimed(string id) {
            if (Achievements.ContainsKey(id)) {
                Achievements[id].Data.isClaimed = true;
                //Debug.Log("Marking achievement as claimed with hashcode: " + Achievements[id].Data.GetHashCode());
            }
        }
        /// <summary>
        /// Check all achievements to see if there are any newly completed one
        /// </summary>
        /// <returns>A list of newly unlocked achievements</returns>
        public List<Achievement> CheckAllAchievements() {
            List<Achievement> unlockedAchs = new List<Achievement>(5);

            ////traverse through all achievements currently in list
            //for (int i = 0; i < m_AchievementsIDs.Count; i++) {
            //    var ach = Achievements[m_AchievementsIDs[i]];

            //    //only check if the achievement is not unlocked already
            //    if (!ach.IsUnlocked) {
            //        //an achievement is unlocked only when all of its related properties are activated
            //        int numPropertiesActivated = 0;
            //        //so, we check for them
            //        for (int j = 0; j < ach.Properties.Count; j++) {
            //            var property = Properties[ach.Properties[j].ID];
            //            //if (property.IsActive()) {
            //            //    ++numPropertiesActivated;
            //            //}
            //        }

            //        //newly unlocked achievement will be added into list of result
            //        if (numPropertiesActivated == ach.Properties.Count) {
            //            ach.IsUnlocked = true;
            //            unlockedAchs.Add(ach);
            //        }
            //    }
            //}

            return unlockedAchs;
        }

        /// <summary>
        /// Reset value of all properties has a specified tag
        /// </summary>
        /// <param name="tag">The tag of properties to reset value</param>
        public void ResetPropertiesByTag(string tag) {
            if (m_PropertiesByTag.ContainsKey(tag)) {
                List<Property> props = m_PropertiesByTag[tag];
                if(props != null) {
                    for(int i = 0; i< props.Count; i++) {
                        props[i].Reset();
                    }
                }
            }
        }

        public void ResetAchievementsByTag (string tag) {
            //Debug.Log("Trying to reset achievement with tag: " + tag);
            if (m_AchievementsByTag.ContainsKey(tag)) {
                List<Achievement> achievements = m_AchievementsByTag[tag];
                if (achievements != null) {
                    for (int i = 0; i < achievements.Count; i++) {
                        achievements[i].Reset();
                    }
                }
            }
        }
    }
}