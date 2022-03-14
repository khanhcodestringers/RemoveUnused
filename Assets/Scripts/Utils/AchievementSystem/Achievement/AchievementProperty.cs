
using System;
using UnityEngine;

namespace Achievement {
    public class Property {
        private PropertyModel m_Data;
        public PropertyModel Data {
            get { return m_Data; }
            private set { m_Data = value; }
        }

        /// <summary>
        /// Get or set the name of this property
        /// </summary>
        public string ID {
            get { return Data.ID; }
            private set { Data.ID = value; }
        }

        /// <summary>
        /// The current value of this property
        /// </summary>
        public int Value {
            get { return Data.currentValue; }
            set {
                Data.currentValue = value;
                //Debug.Log("Callback when value change: " + OnValueChanged);
                if(OnValueChanged != null) {
                    OnValueChanged(this);
                }
            }
        }

        /// <summary>
        /// The initial value of this property
        /// </summary>
        public int InitialValue {
            get { return Data.initValue; }
            private set { Data.initValue = value; }
        }

        public event Action<Property> OnValueChanged;
        
        public Property(string id, int initialValue, int value) {
            Data = new PropertyModel();
            this.ID = id;
            this.InitialValue = initialValue;
            this.Value = value;
        }

        public Property(PropertyModel data) {
            this.Data = data;
        }

        public void Reset() {
            //Debug.Log("Resetting value for property " + ID + " to: " + InitialValue);
            this.Value = this.InitialValue;
        }

        //private string m_ActivationRule;
        ///// <summary>
        ///// What is the activation rule of this property (Less than, equal to or greater than)
        ///// </summary>
        //public string ActivationRule {
        //    get { return m_ActivationRule; }
        //    set { m_ActivationRule = value; }
        //}

        //private int m_ActivationValue;
        ///// <summary>
        ///// The minimum value to count this property as activated
        ///// </summary>
        //public int ActivationValue {
        //    get { return m_ActivationValue; }
        //    set { m_ActivationValue = value; }
        //}

        //public bool IsActive() {
        //    switch (m_ActivationRule) {
        //        case AchievementManager.ACTIVE_IF_EQUALS_TO:
        //            return m_Value == m_ActivationValue;
        //        case AchievementManager.ACTIVE_IF_GREATER_THAN:
        //            return m_Value > m_ActivationValue;
        //        case AchievementManager.ACTIVE_IF_LESS_THAN:
        //            return m_Value < m_ActivationValue;
        //    }

        //    Debug.Log(string.Format("The property {0} has invalid activation rule ({1}) and will be marked as inactive", m_Name, m_ActivationRule));
        //    return false;
        //}
    }
}