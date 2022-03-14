using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Mio.TileMaster {
    public class TilesRow : MonoBehaviour {
        [SerializeField]
        private List<Tile> tiles;
        private List<Tile> mainTile;
        [SerializeField]
        private UIWidget eobTile;
        //[SerializeField]
        //private int mainIndex;
        [SerializeField]
        private int rowHeight;
        public int RowHeight {
            get { return rowHeight; }
            private set { rowHeight = value; }
        }

        private int m_ID;
        public int ID {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public float GetPositionY() {
            return eobTile.cachedTransform.position.y;
        }
        public Transform tf;

        //public event Action<Tile> OnTileClicked;
        //public event Action<Tile> OnTileHold;

        void Awake() {
            tf = transform;

            mainTile = new List<Tile>(4);
            Reset();
            //this.Print("Initialized completed");
        }

        public void Reset(int height = -1) {
            for (int i = 0; i < 4; i++) {
                //tile.OnTileClicked -= OnTileItemClicked;
                //tile.OnTileClicked += OnTileItemClicked;
                //tile.OnTileHold -= OnTileItemHold;
                //tile.OnTileHold += OnTileItemHold;
                tiles[i].CurrentType = TileType.Empty;
                tiles[i].Interactable = true;
                tiles[i].ID = ID * 10 + i;
                //tiles[i].gameObject.name = string.Format("Tile {0} of row {1}", i, gameObject.name);
                if (height > 0) {
                    tiles[i].Reset();
                    tiles[i].SetTileHeight(height);
                }
            }
            mainTile.Clear();
            RowHeight = height;
        }

        private void OnTileItemHold(Tile tile) {
           // Helpers.CallbackWithValue(OnTileHold, tile);
        }

        private void OnTileItemClicked(Tile tile) {
            //Helpers.CallbackWithValue(OnTileClicked, tile);
        }

        public List<Tile> GetMainTiles() {
            return mainTile;
        }

        public void SetTileData(int index, TileData data) {
            //this.Print(string.Format("{2} is setting tile data at index {0}, type {1}", index, data.type, gameObject.name));
            tiles[index].TileData = data;
            //mainIndex = index;
            //save reference of main tile for later usage
            if (data.type == TileType.Normal
                || data.type == TileType.LongNote) {
                mainTile.Add(tiles[index]);
            }            
        }

        public Tile GetTileAt(int index) {
            return tiles[index];
        }

        internal void SetInteractable(bool value = true) {
            for(int i = 0; i < 4; i ++) {
                tiles[i].Interactable = value;
            }
        }

        internal void FlashError() {
            if (mainTile != null) {
                for(int i = 0; i < mainTile.Count; i++) {
                    if(mainTile[i].TileData.type == TileType.Normal || mainTile[i].TileData.type == TileType.LongNote) {
                        mainTile[i].Flash();
                    }
                }
            }
        }

        internal void Recover() {
            for (int i = 0; i < 4; i++) {
                if (tiles[i].CurrentType == TileType.Error) {
                    tiles[i].Recover();
                }
            }
        }
    }
}