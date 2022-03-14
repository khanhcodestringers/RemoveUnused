using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Mio.TileMaster {
    public class Tile : MonoBehaviour {
        private static readonly List<NoteData> emptyListNote = new List<NoteData>(1);
        private static int touchSpriteInitialHeight = -1;

        [SerializeField]
        private UISprite sprite;

        [Tooltip("If this note is a long note, then it will have a head")]
        [SerializeField]
        private UISprite headSprite;
        [Tooltip("If this note is a long note, then it need different touch effect")]
        [SerializeField]
        private UISprite touchSprite;

        private bool hasData = false;

        private int m_ID;
        public int ID {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private TileData tileData;
        public TileData TileData {
            get { return tileData; }
            set {
                tileData = value;
                if (tileData != null) {
                    hasData = true;
                    CurrentType = tileData.type;
                }
                else {
                    print("Tile data is null");
                    hasData = false;
                }
            }
        }

        public TileType GetBaseType() {
            if (hasData) { return TileData.type; }

            return TileType.Empty;
        }

        public List<NoteData> GetNotesData() {
            if (hasData) { return TileData.notes; }

            return emptyListNote;
        }

        void Awake() {

            touchSprite.gameObject.SetActive(false);
        }

        void Start() {
            if (touchSpriteInitialHeight < 0) {
                touchSpriteInitialHeight = touchSprite.height;
                //print("Setting touchsprite height into : " + touchSpriteInitialHeight);
            }
        }

        private static readonly string[] spriteNames = {
            "tile_white", //tile empty
            "tile_black", //normal tile
            "finishedTile", //tapped tile
            "bonus_tile_music", //bonus tile
            "bonus_tile_life", //heart tile
            "tile_miss", //error tile
            "long_head", //long note - head part
            "black", //long note - tail part            
            "long_tilelight" //long note - clicked part
        };

        private static readonly string longNoteSprite = spriteNames[(int)TileType.LongNote];

        //public event Action<Tile> OnTileClicked;
        //public event Action<Tile> OnTileHold;

        [SerializeField]
        private bool interactable = false;
        public bool Interactable {
            get { return interactable; }
            set {
                //print(string.Format("Setting interactable to {0} at tile {1}", value, gameObject.name));
                interactable = value;
            }
        }

        [SerializeField]
        private TileType type = TileType.Normal;
        public TileType CurrentType {
            get { return type; }
            set {
                type = value;
                RefreshView();
            }
        }
        

        //public TileType testType = TileType.Normal;
        //void Update() {
        //    if(testType != type) {
        //        CurrentType = testType;
        //    }
        //}

        public void Reset() {
            touchSprite.cachedGameObject.SetActive(false);
            hasData = false;
        }

        private void RefreshView() {
            if (type != TileType.LongNote) {
                sprite.spriteName = spriteNames[(int)type];
                headSprite.cachedGameObject.SetActive(false);
                touchSprite.cachedGameObject.SetActive(false);
            }
            else {
                //this.Print("Refreshing view on tile " + gameObject.name + " changed to type " + type);
                sprite.spriteName = longNoteSprite;
                headSprite.cachedGameObject.SetActive(true);
                touchSprite.height = touchSpriteInitialHeight;
            }
        }

        public void Hold() {
            //this.Print("Hold on " + gameObject.name);
            if (interactable) {
                //if (disableAfterPress) { interactable = false; }
                ChangeSpriteOnClicked();
                //Helpers.CallbackWithValue(OnTileHold, this);
            }
        }

        public bool ShowHoldEffect(int height, bool isBeginToShow = false) {
            if (isBeginToShow) {
                touchSprite.cachedGameObject.SetActive(true);
                touchSprite.height = touchSpriteInitialHeight;
                return true;
            }

            if (touchSpriteInitialHeight + height >= sprite.height) {
                //Interactable = false;
                return false;
            }

            touchSprite.height = touchSpriteInitialHeight + height;
            return true;
        }

        public void Tap() {
            //this.Print("Pressed on " + gameObject.name);
            if (interactable) {
                // if (disableAfterPress) { interactable = false; }
                ChangeSpriteOnClicked();
                //Helpers.CallbackWithValue(OnTileClicked, this);
            }
        }

        /// <summary>
        /// Change display sprite to a suitable one, depends on current type of tile data
        /// </summary>
        public void ChangeSpriteOnClicked() {
            if (hasData) {
                switch (TileData.type) {
                    case TileType.Normal:
                        CurrentType = TileType.Tapped;
                        break;
                    case TileType.Bonus:
                    case TileType.Heart:
                        gameObject.SetActive(false);
                        break;
                }
            }
        }

        public void SetInteractable(bool canInteract = true) {
            interactable = canInteract;
        }

        internal void Flash() {
            //this.Print("Flashing error");
            CurrentType = TileType.Error;
        }

        public void SetTileHeight(int height) {
            //if(CurrentType == TileType.LongNote) {                
            sprite.height = height;
            //}
        }

        /// <summary>
        /// Reset tile's type back to tile data
        /// </summary>
        internal void Recover() {
            if (TileData != null) {
                CurrentType = TileData.type;
            }
            else {
                CurrentType = TileType.Empty;
            }
        }
    }
}