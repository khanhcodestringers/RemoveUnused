using UnityEngine;
using System.Collections;
using Mio.Utils.MessageBus;
using Mio.Utils;

namespace Mio.TileMaster {
    public class DiamondIAPItem : MonoBehaviour {
        [Header("Choose Diamond Package")]
      

        [Header("Current Diamond")]
        [Header("UI")]
        [SerializeField]
        private UILabel lblQuantityDiamond;
        [SerializeField]
        private UILabel lblAmount;

        //[Header("Values")]
        //[SerializeField]
        //private int quantityDiamond;
        //[SerializeField]
        //private float amount;

        /// <summary>
        /// "BuyDiamond" btn TAP
        /// </summary>
        public void BuyDiamond () {
          
        }

        //void Start () {
        //    MessageBus.Instance.Subscribe(MessageBusType.CompletedIAPLocalization, InitUI);
        //}

        void OnEnable () {
            InitUI();
        }

        private void InitUI () {
            
        }
    }
}
