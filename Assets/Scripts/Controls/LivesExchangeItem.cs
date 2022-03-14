using UnityEngine;
using System;

namespace Mio.TileMaster
{
    public class LivesExchangeItem : MonoBehaviour
    {
        [Header("UILabel")]
        [SerializeField]
        private UILabel lblEnergySupply;

        [SerializeField]
        private UILabel lblRubyPrice;


        [Header("Values")]
        [SerializeField]
        public int livesValue;

        [SerializeField]
        public int price;

        public event Action<LivesExchangeItem> OnLifeItemClicked;
        //void Start()
        //{
        //    //InitUI();
        //}

        /// <summary>
        /// init ui for x5,x10,x20 panel prefab
        /// </summary>
        public void InitUI()
        {
            lblEnergySupply.text = "x" + livesValue;
            lblRubyPrice.text = "x" + price;
        }

        /// <summary>
        /// "BuyEnergySupply" btn buy
        /// </summary>
        public void BuyEnergySupply()
        {
            Helpers.CallbackWithValue(OnLifeItemClicked, this);
            //Debug.Log(string.Format("Buy : {0} heart with {1} rubies", enerySupply, rubyPrice));
        }

    }
}
