using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Mio.TileMaster {
    public class LastPlayItem : MonoBehaviour {
        [SerializeField]
        private UILabel lbSongTitle;
        [SerializeField]
        private UISprite imgCDCover;

        //public List<string> cdSpriteNames;

        private SongDataModel model;
        public SongDataModel Model {
            get { return model; }
            set {
                if(model != value) {
                    model = value;
                    RefreshView();
                }                
            }
        }


        //private int spriteIndex = 0;

        private void RefreshView() {
            //if (cdSpriteNames.Count > 0) {
            //    int nextIndex = UnityEngine.Random.Range(0, cdSpriteNames.Count);
            //    if (nextIndex == spriteIndex) {
            //        spriteIndex = (spriteIndex + 1) % cdSpriteNames.Count;
            //    }
            //    else {
            //        spriteIndex = nextIndex;
            //    }
            //    imgCDCover.spriteName = cdSpriteNames[spriteIndex];
            //}
            lbSongTitle.text = model.name;
        }
    }
}