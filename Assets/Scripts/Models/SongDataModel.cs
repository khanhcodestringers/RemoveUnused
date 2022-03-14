using System;
using System.Collections.Generic;

namespace Mio.TileMaster {
    public enum SongItemType {
        Normal = 0,
        Hot = 1,
        New = 2
    }
    [Serializable]
    public class SongDataModel {
        //unique id for each song
        public int ID;
        //name of the song to be display to user
        public string name;
        //author or origin of the song
        public string author;
        //url to download level data
        public string songURL;
        //the unique id of this song item
        public string storeID;
        //version of this song data
        public int version;
        //price in primary currency of the game (currency bought from cash)
        public int pricePrimary;        
        //how many star needed to unlock this song
        public int starsToUnlock;

        //maximum speed this level will reach
        public float maxBPM;
        //number of ticks for a black tiles
        public int tickPerTile;
        //faster or slower speed
        public float speedModifier;
        public List<float> speedPerStar;
        //type of the song (new, hot or normal one)
        public SongItemType type;
    }
}