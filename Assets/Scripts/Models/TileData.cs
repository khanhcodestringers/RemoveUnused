
using System.Collections.Generic;
using System;
namespace Mio.TileMaster {
    [Serializable]
    public class TileData {
        public List<NoteData> notes;
        public TileType type = TileType.Empty;
        public TileType subType = TileType.Normal;
        public float soundDelay;
        public float startTime;
        public float startTimeInTicks;
        public int durationInTicks;
        public float duration;
        public int score;
    }
    [Serializable]
    public class SongTileData {
        public SongDataModel songDataModel;
        public List<TileData> titledata;
        //the bpm of the level
        public float BPM;
        //number of ticks for a whole note
        public int tickPerQuarterNote;
        public int denominator;
    }
}