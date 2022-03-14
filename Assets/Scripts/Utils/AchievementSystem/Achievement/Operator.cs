using UnityEngine;
using System;

namespace Achievement {
    public class Operator {
        private OperatorModel m_Data;
        public OperatorModel Data {
            get { return m_Data; }
            set { m_Data = value; }
        }

        private Property property;
        public string PropertyID {
            get { return Data.propertyID; }
        }

        
        public string Expression {
            get { return Data.expressionString; }
            internal set { Data.expressionString = value; }
        }


        private int currentValue;
        public int CurrentValue {
            get { return currentValue; }
        }

        public int TargetValue {
            get { return Data.targetValue; }
            //set { m_TargetValue = value; }
        }

        public string ID {
            get { return Data.ID; }
        }

        /// <summary>
        /// If true, the callback of this operator can only be set to one (1) receiver, the consequences receiver trying to set callback will fail
        /// </summary>
        public bool IsSoloUse {
            get { return Data.isSoloUse; }
        }

        private bool m_IsCompleted;
        public bool IsCompleted {
            get { return m_IsCompleted; }
        }

        private event Action<Operator> OnOperatorDone;
        private event Action<Operator> OnOperatorUndone;

        /// <summary>
        /// Set callback for when operator has completed its purpose
        /// </summary>
        /// <param name="callback"></param>
        public void SetOperatorCompleteCallBack(Action<Operator> callback) {
            if (callback != null) {
                if (IsSoloUse) {
                    if (OnOperatorDone == null) {
                        OnOperatorDone += callback;
                    }
                    else {
                        Debug.Log(string.Format("You are trying to set another call back for a Solo Operator ({0}), skipping...", ID));
                    }
                }
                else {
                    OnOperatorDone += callback;
                }
            }
            else {
                Debug.Log(string.Format("You are trying to set call back for this Operator ({0}) as null, skipping...", ID));
            }
        }

        /// <summary>
        /// Will be called if an operator has value that does not satisfy condition any more
        /// </summary>
        public void SetOperatorUndoneCallback(Action<Operator> callback) {
            if (callback != null) {
                if (IsSoloUse) {
                    if (OnOperatorUndone == null) {
                        OnOperatorUndone += callback;
                    }
                    else {
                        Debug.Log(string.Format("You are trying to set another call back for a Solo Operator ({0}), skipping...", ID));
                    }
                }
                else {
                    OnOperatorUndone += callback;
                }
            }
            else {
                Debug.Log(string.Format("You are trying to set call back for this Operator ({0}) as null, skipping...", ID));
            }
        }        

        public Operator(OperatorModel data, Property property) {
            if (data != null) {
                if ((property != null)) {
                    //check to make sure the expression string is valid
                    if (data.expressionString.Equals(AchievementManager.ACTIVE_IF_EQUALS_TO)
                        || data.expressionString.Equals(AchievementManager.ACTIVE_IF_GREATER_THAN)
                        || data.expressionString.Equals(AchievementManager.ACTIVE_IF_LESS_THAN)) {
                        this.m_Data = data;
                        //data.propertyID = property.ID;
                        //Debug.Log("Add callback for property: " + property.ID);
                        this.property = property;
                        property.OnValueChanged -= OnPropertyValueChanged;
                        property.OnValueChanged += OnPropertyValueChanged;
                    }
                    else {
                        Debug.LogError("Trying to initialize an Achievement Operator with an invalid expression string of " + data.expressionString + ". Skipping...");
                    }
                }
                else {
                    Debug.LogError("Trying to initialize an Achievement Operator with a null property. Skipping...");
                }
            }
            else {
                Debug.LogError("Trying to initialize an Achievement Operator with null data. Skipping...");
            }
        }

        private void OnPropertyValueChanged(Property property) {
            //Debug.Log(string.Format("Property changed: {0}, current value {1} in operator {2}", property.ID, property.Value, ID));
            currentValue = property.Value;
            switch (Expression) {
                case AchievementManager.ACTIVE_IF_EQUALS_TO:
                    if(property.Value >= TargetValue) {
                        MarkDone();
                    }
                    else {
                        MarkUndone();
                    }
                    break;

                case AchievementManager.ACTIVE_IF_GREATER_THAN:
                    if(property.Value > TargetValue) {
                        MarkDone();
                    }
                    else {
                        MarkUndone();
                    }
                    break;

                case AchievementManager.ACTIVE_IF_LESS_THAN:
                    if(property.Value < TargetValue) {
                        MarkDone();
                    }
                    else {
                        MarkUndone();
                    }
                    break;
            }
        }

        public void Reset () {
            //Debug.Log("Resetting operator: " + Data.ID);
            MarkUndone();
            if(property != null) {
                property.Reset();
            }
        }

        private void MarkUndone() {
            if (m_IsCompleted) {
                m_IsCompleted = false;
                if (OnOperatorUndone != null) {
                    OnOperatorUndone(this);
                }
            }
        }

        private void MarkDone() {
            if (!m_IsCompleted) {
                m_IsCompleted = true;
                if (OnOperatorDone != null) {
                    OnOperatorDone(this);
                }
            }
        }
    }
}