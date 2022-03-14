using System.Collections.Generic;
using System;

namespace Mio.TileMaster {
    [Serializable]
    public class LevelDataModel   {
        public SongDataModel songData;
        public List<NoteData> playbackData;
        public List<NoteData> noteData;
        //the bpm of the level
        public float BPM;
        //number of ticks for a whole note
        public int tickPerQuarterNote;
        public int denominator;
    }
}