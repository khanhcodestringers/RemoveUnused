using System;
using System.Collections.Generic;
using UnityEngine;

namespace Achievement {
    /// <summary>
    /// An achievement 
    /// </summary>
    public class Achievement {
        public event Action<Achievement> OnAchievementCompleted;

        private AchievementManager m_Manager;

        private Dictionary<OperatorModel, Operator> allOperators;
        private AchievementModel m_Data;
        public AchievementModel Data {
            get { return m_Data; }
            private set { m_Data = value; }
        }
         
        /// <summary>
        /// Name of this achievement
        /// </summary>
        public string DisplayName {
            get { return Data.title; }
        }

        public string ID {
            get { return Data.ID; }
        }

        public string Description {
            get { return Data.description; }
        }
        public bool IsUnlocked {
            get { return Data.isUnlocked; }
        }

        public bool IsActive {
            get { return Data.isActive; }
        }

        public bool IsHidden {
            get { return Data.isHidden; }
        }

        private int m_TotalOperator = 0;
        private int m_OperatorCompleted = 0;
        public int NumOperatorCompleted {
            get { return m_OperatorCompleted; }
        }

        private Achievement(AchievementModel data) {            
            m_Data = data;
            m_OperatorCompleted = 0;
        }

        private void OnOperatorCompleted(Operator op) {
            TryUnlockAchievement();
        }

        private void OnOperatorUndone(Operator op) {
            
        }

        public float GetCurrentProgress() {
            float currentProgress = 0;
            //Debug.Log("Getting progress for achievement: " + Data.ID);
            for (int i = 0; i < m_TotalOperator; i++) {
                //Debug.Log("--Checking operator: " + allOperators[Data.listConditions[i]].ID);
                if (allOperators[Data.listConditions[i]].IsCompleted) {
                    //Debug.Log("--Operator completed");
                    currentProgress += 1.0f / m_TotalOperator;
                }
                else {
                    //Debug.Log("--Operator not completed");
                    currentProgress += 1.0f / m_TotalOperator * (allOperators[Data.listConditions[i]].CurrentValue * 1.0f / allOperators[Data.listConditions[i]].TargetValue);
                }
            }
            return currentProgress;
        }

        public void Reset () {
            //Debug.Log("Resetting achievement: " + Data.ID);
            if(Data!= null) {
                if (Data.isUnlocked || Data.isClaimed) {
                    Data.isUnlocked = false;
                    Data.isClaimed = false;                    
                }

                for (int i = 0; i < Data.listConditions.Count; i++) {
                    allOperators[Data.listConditions[i]].Reset();
                }
            }
        }

        public void TryUnlockAchievement() {
            m_OperatorCompleted = 0;
            for(int i = 0; i < m_TotalOperator; i++) {
                if (allOperators[Data.listConditions[i]].IsCompleted) {
                    ++m_OperatorCompleted;
                }
            }

            //Debug.Log("Trying to unlock achievement " + Data.ID);
            //Debug.Log("Operator completed: " + m_OperatorCompleted);
            //Debug.Log("Total Operator: " + m_TotalOperator);
            if (m_OperatorCompleted == m_TotalOperator) {
                Data.isUnlocked = true;
                if (OnAchievementCompleted != null) {
                    OnAchievementCompleted(this);
                }
            }
        }

        /// <summary>
        /// A factory to create achievement from specified data
        /// </summary>
        /// <param name="data">Not null please</param>
        /// <param name="manager">NOT NULL also</param>
        /// <returns></returns>
        public static Achievement CreateAchievement(AchievementModel data, AchievementManager manager) {
            if(data == null) {
                Debug.LogWarning("Trying to create achievement with null data, skipping...");
                return null;
            }

            if(manager == null) {
                Debug.LogWarning("Trying to create achievement without manager, skipping...");
                return null;
            }

            Achievement ach = new Achievement(data);
            ach.m_Manager = manager;

            if (!data.isUnlocked) {
                ach.Initialize();
                //ach.allOperators = new Dictionary<OperatorModel, Operator>(5);
                //ach.m_OperatorCompleted = 0;
                //ach.m_TotalOperator = data.listConditions.Count;

                ////Debug.Log("Creating achievement: " + data.ID + ", num condition: " + data.listConditions.Count);
                //for (int i = 0; i < listOperators.Count; i++){
                //    Operator op = manager.AddOperator(listOperators[i]);
                //    if (op != null) {
                //        op.SetOperatorCompleteCallBack(ach.OnOperatorCompleted);
                //        op.SetOperatorUndoneCallback(ach.OnOperatorUndone);
                //        ach.allOperators.Add(listOperators[i], op);
                //    }
                //    else {
                //        Debug.Log("Can NOT find suitable operator, creating Achievement failed for " + data.ID);
                //        return null;
                //    }
                //}
            }

            return ach;
        }

        public void Initialize() {
            if(Data == null) {
                Debug.LogWarning("Trying to initialize a null achievement, skipping...");
                return;
            }

            var listOperators = Data.listConditions;
            if (listOperators == null) {
                Debug.LogWarning(string.Format("List condition for achievement {0} is null, achievement will be created without any logic", Data.ID));
                return;
            }

            allOperators = new Dictionary<OperatorModel, Operator>(5);
            m_OperatorCompleted = 0;
            m_TotalOperator = Data.listConditions.Count;

            //Debug.Log("Creating achievement: " + data.ID + ", num condition: " + data.listConditions.Count);
            for (int i = 0; i < listOperators.Count; i++) {
                Operator op = m_Manager.AddOperator(listOperators[i]);
                if (op != null) {
                    op.SetOperatorCompleteCallBack(OnOperatorCompleted);
                    op.SetOperatorUndoneCallback(OnOperatorUndone);
                    allOperators.Add(listOperators[i], op);
                }
                else {
                    Debug.Log("Can NOT find suitable operator, creating Achievement failed for " + Data.ID);
                    return;
                }
            }
        }        
    }
}