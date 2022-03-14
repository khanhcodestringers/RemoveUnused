namespace CSSynth.Midi
{
    public class MidiTrack
    {
        //--Variables
        public uint notesPlayed;
        public ulong totalTime;
        public byte[] programs;
        public byte[] drumPrograms;
        public MidiEvent[] midiEvents;
        public string trackName;
        public string lyricText;
        public string copyrightNotice;
        public string markerText;
        //--Public Properties
        public int EventCount
        {
            get { return midiEvents.Length; }
        }
        //--Public Methods
        public MidiTrack()
        {
            notesPlayed = 0;
            totalTime = 0;
        }
        public bool ContainsProgram(byte program)
        {
            for (int x = 0; x < programs.Length; x++)
            {
                if (programs[x] == program)
                    return true;
            }
            return false;
        }
        public bool ContainsDrumProgram(byte drumprogram)
        {
            for (int x = 0; x < drumPrograms.Length; x++)
            {
                if (drumPrograms[x] == drumprogram)
                    return true;
            }
            return false;
        }
    }
}
