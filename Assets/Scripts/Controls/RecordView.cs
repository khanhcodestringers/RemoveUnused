using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

namespace Mio.TileMaster {
    [RequireComponent(typeof(UIWidget))]
    public class RecordView : MonoBehaviour {
        [SerializeField]
        private UIGrid gridLayout;
        [SerializeField]
        private UIToggle[] recordItem;
        [SerializeField]
        private UIWidget widget;

        void Start() {
            if(widget == null) { widget = gameObject.GetComponent<UIWidget>(); }
        }

        /// <summary>
        /// Show or hide this control, with or without animation. NOTE: this method also set the alpha of this widget to either 0 or 1
        /// </summary>
        /// <param name="visible">Show or hide?</param>
        /// <param name="fadeDuration">Default is 0, meaning no animation</param>
        public void SetVisible(bool visible, float fadeDuration = 0) {
            float toAlpha = visible ? 1 : 0;

            if (fadeDuration == 0) {
                widget.cachedGameObject.SetActive(visible);
                widget.alpha = toAlpha;
            }
            else {                
                bool shouldVisible = visible;
                if (shouldVisible) { widget.cachedGameObject.SetActive(true); }
                DOTween.To((alpha) => widget.alpha = alpha, widget.alpha, toAlpha, fadeDuration)
                    .OnComplete(() => widget.cachedGameObject.SetActive(shouldVisible))
                    .Play();
            }
        }

        /// <summary>
        /// Show a specified number of record items
        /// </summary>
        /// <param name="num">Number of record items to show</param>
        /// <param name="hideInactive">Should this control show the inactive ones</param>
        public void ShowNumRecord(int num, bool hideInactive = true) {
            //validate before going
            if (num <= recordItem.Length) {
                for (int i = 0; i < recordItem.Length; i++) {
                    //for each suitable record item
                    if (i < num) {
                        //show it     
                        recordItem[i].gameObject.SetActive(true);
                        recordItem[i].value = true;
                    }
                    else {
                        recordItem[i].value = false;
                        //if we need to show the inactive ones, do it
                        if (!hideInactive) {                            
                            recordItem[i].gameObject.SetActive(true);
                        }else {
                            recordItem[i].gameObject.SetActive(false);
                        }
                    }
                }
				gridLayout.repositionNow = true;
                StartCoroutine(RefreshView());
            }
        }

        //public bool isTest = false;
        //void Update() {
        //    if (isTest) {
        //        isTest = false;
        //        ShowNumRecord(Counter.Count("0"));
        //    }
        //}

        private IEnumerator RefreshView() {
            yield return 0.2f;
		//	gridLayout.enabled = true;
            //enable gird layout so that it will animate newly added star
			gridLayout.repositionNow = true;
        }
    }
}