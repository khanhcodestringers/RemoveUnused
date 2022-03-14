using UnityEngine;
using System.Collections;
using Mio.Utils.MessageBus;
using System;
using System.Collections.Generic;
using Mio.Utils;

namespace Mio.TileMaster {
    public class LivesPopUpController : SSController {
        [Header("Current life")]
        [SerializeField]
        private UILabel lbCurrentLives;

        [Header("Live exchange items")]
        [SerializeField]
        private List<LivesExchangeItem> listLivesExchangeItem;

        [Header("Current life Animation Effect")]
        [SerializeField]
        private Animator lifeBarEffect;

        public override void OnEnableFS () {
            base.OnEnableFS();

            // a fail safe to prevent user from using too old game configs
            if (GameManager.Instance.GameConfigs.lives_exchange_values == null || GameManager.Instance.GameConfigs.lives_exchange_values.Count <= 0) {
                GameManager.Instance.GameConfigs.lives_exchange_values = new List<int>();
                GameManager.Instance.GameConfigs.lives_exchange_values.AddRange(new int[] { 5, 10, 20, 60 });

                GameManager.Instance.GameConfigs.lives_exchange_prices = new List<int>();
                GameManager.Instance.GameConfigs.lives_exchange_prices.AddRange(new int[] { 25, 45, 85, 200 });
            }


            var listValues = GameManager.Instance.GameConfigs.lives_exchange_values;
            var listPrices = GameManager.Instance.GameConfigs.lives_exchange_prices;

            for (int i = 0; i < listLivesExchangeItem.Count; i++) {
                if (i < listValues.Count) {
                    listLivesExchangeItem[i].price = listPrices[i];
                    listLivesExchangeItem[i].livesValue = listValues[i];
                    //print("setting up lives item " + i);
                    listLivesExchangeItem[i].OnLifeItemClicked -= OnLifeItemClicked;
                    listLivesExchangeItem[i].OnLifeItemClicked += OnLifeItemClicked;
                    listLivesExchangeItem[i].InitUI();
                }
            }
        }

        private void OnLifeItemClicked (LivesExchangeItem item) {
            if (ProfileHelper.Instance.CurrentDiamond >= item.price) {
                //print("Buying lives " + item.livesValue);
                ProfileHelper.Instance.CurrentLife += item.livesValue;
                ProfileHelper.Instance.CurrentDiamond -= item.price;
                for (int i = 0; i < GameManager.Instance.GameConfigs.lives_exchange_values.Count; i++) {
                    if (item.livesValue == GameManager.Instance.GameConfigs.lives_exchange_values[i]) {
                        lifeBarEffect.SetTrigger("life" + (i + 1));
                        break;
                    }
                }
            }
            else {
                SceneManager.Instance.OpenPopup(ProjectConstants.Scenes.IAP);
            }
        }

        public override void OnEnable () {
            base.OnEnable();
            lbCurrentLives.text = ProfileHelper.Instance.CurrentLife.ToString();
            MessageBus.Instance.Subscribe(MessageBusType.LifeChanged, OnLivesChanged);
        }

        public override void OnDisable () {
            base.OnDisable();
            MessageBus.Instance.Unsubscribe(MessageBusType.LifeChanged, OnLivesChanged);
        }

        private void OnLivesChanged (Message obj) {
            lbCurrentLives.text = ProfileHelper.Instance.CurrentLife.ToString();
        }

        /// <summary>
        /// close popup when tap btn close
        /// </summary>
        public void CloseThisPopUp () {
            SceneManager.Instance.CloseScene();
        }

        /// <summary>
        /// "Get it" btn TAP
        /// </summary>
        public void GetSupplyX2 () {
            Debug.Log("show Ads for reward life");
        }
    }
}
