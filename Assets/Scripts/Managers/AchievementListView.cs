using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Achievement;
using Mio.Utils;

/// <summary>
/// Hello, here is a little gift for you
//         (`.  : \               __..----..__
  //        `.`.| |:          _,-':::''' '  `:`-._
  //          `.:\||       _,':::::'         `::::`-.
  //            \\`|    _,':::::::'     `:.     `':::`.
  //             ;` `-''  `::::::.                  `::\
  //          ,-'      .::'  `:::::.         `::..    `:\
  //        ,' /_) -.            `::.           `:.     |
  //      ,'.:     `    `:.        `:.     .::.          \
  // __,-'   ___,..-''-.  `:.        `.   /::::.         |
  //|):'_,--'           `.    `::..       |::::::.      ::\
  // `-'                 |`--.:_::::|_____\::::::::.__  ::|
  //                     |   _/|::::|      \::::::|::/\  :|
  //                     /:./  |:::/        \__:::):/  \  :\
  //                   ,'::'  /:::|        ,'::::/_/    `. ``-.__
  //     glhf         ''''   (//|/\      ,';':,-'         `-.__  `'--..__
  //                                                           `''---::::'
/// </summary>

namespace Mio.TileMaster {
    public class AchievementListView : MonoBehaviour {
        [SerializeField]
        private UIWrapContent itemListContainer;
        [SerializeField]
        private AchievementType achievementType = AchievementType.Once;
        private List<Achievement.AchievementModel> listItemData;
        //private List<AchievementItemView> listAchievementItem;
        private Dictionary<GameObject, AchievementItemView> currentItemList;

        private bool isInitialized = false;

        public void Initialize() {
			if (!isInitialized) {
				//lastViewStoreVersion = 0;
				//listAchievementItem = new List<AchievementItemView>(20);

				currentItemList = new Dictionary<GameObject, AchievementItemView> (20);
				//lol, get those achievement items' data
                itemListContainer.maxIndex = 0;
				itemListContainer.minIndex = -(listItemData.Count - 1);
				itemListContainer.onInitializeItem -= OnAchievementItemInitialize;
				itemListContainer.onInitializeItem += OnAchievementItemInitialize;

				//get all song item view
				Transform scrolllist = itemListContainer.transform;
				int numItem = scrolllist.childCount;
				for (int i = 0; i < numItem; i++) {
					AchievementItemView v = scrolllist.GetChild (i).GetComponent<AchievementItemView> ();
					if (v != null) {
						v.OnClaimAchievement += OnClaimingAchievement;
						currentItemList.Add (v.gameObject, v);
					}

					//hide unnecessary items
					if (i >= listItemData.Count) {
						v.gameObject.SetActive (false);
					}
				}

				isInitialized = true;
			} else {
			
				foreach (KeyValuePair<GameObject,AchievementItemView> item in currentItemList) {
					if (item.Key.activeSelf) {
						item.Value.Model = listItemData[item.Value.index];
						item.Value.RefreshItemView();
					}
				}
			}
        }

        public void RefreshAchievementList() {
            Initialize();

            itemListContainer.minIndex = itemListContainer.maxIndex = 0;
            itemListContainer.minIndex = -(listItemData.Count - 1);
            //itemListContainer
        }
        
        private void OnAchievementItemInitialize(GameObject go, int wrapIndex, int realIndex) {
            //Debug.Log("Refreshing item at index " + wrapIndex + " real index: " + realIndex);
            AchievementItemView item;
            if (!currentItemList.ContainsKey(go)) {
                item = go.GetComponent<AchievementItemView>();
                currentItemList.Add(go, item);
            }
            else {
                item = currentItemList[go];
            }

            if (item == null) {
                Debug.LogWarning("Could not get SongItemView component from game object " + go.name);
                return;
            }

            //print("Initializing item at wrap index " + wrapIndex + " real index: " + realIndex + " game object: " + go.name);
            //set up song view item's data
            if (listItemData != null) {
                int dataIndex = ConvertFromScrollIndexToDataIndex(realIndex);
                //print(string.Format("Convert from {0} into {1}", realIndex, dataIndex));
                if (dataIndex >= 0 && dataIndex < listItemData.Count) {
                    //item.Index = dataIndex + 1;
                    //change it's data and view
                    item.Model = listItemData[dataIndex];
                    item.RefreshItemView();
					item.index = dataIndex;
                }
                else {
                    Debug.LogWarning(string.Format("--Scroll index ({0}) has gone out of bound ({1}) at game object {2}", dataIndex, listItemData.Count, go.name));
                }
            }
            else {
                Debug.LogWarning("--List song data is null. Could not setup item's data");
            }
        }

        private void OnClaimingAchievement(AchievementItemView ach) {
            //Debug.Log("On claim achievement button clicked");
            //ach.Model.isUnlocked = true;
            ach.RefreshItemView();
            //throw new NotImplementedException();
        }

        private void OnAchievementClaimed(List<AchievementRewardModel> rewards) {
            if (rewards.Count > 0) {
                for (int i = 0; i < rewards.Count; i++) {
                    if (rewards[i].type.Contains("diamond")) {
                        ProfileHelper.Instance.CurrentDiamond += rewards[i].value;
                    }
                    else if (rewards[i].type.Contains("life")) {
                        ProfileHelper.Instance.CurrentLife += rewards[i].value;
                    }
                }
            }
        }

        /// <summary>
        /// Convert index of wrap content ui into index of song data
        /// Keep in mind that this function do NOT check for null in listSongData
        /// </summary>
        private int ConvertFromScrollIndexToDataIndex(int realScrollIndex) {
            //Math.abs, not really faster, but still using it as a reference lol
            int absRealScrollIndex = (realScrollIndex + (realScrollIndex >> 31)) ^ (realScrollIndex >> 31);
            absRealScrollIndex = absRealScrollIndex % listItemData.Count;

            return (realScrollIndex <= 0) ? (absRealScrollIndex) : (listItemData.Count - absRealScrollIndex);
        }

        // Use this for initialization
//        void Start() {
//            Initialize();
//        }

        //void OnEnable() {
        //    //refresh scroll list
        //    //itemListContainer.SortBasedOnScrollMovement();
        //}
    }
}