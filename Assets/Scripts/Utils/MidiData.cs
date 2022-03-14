using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NoteData {
    public float timeAppear;
    public int nodeID;
    public float duration = 0;
    public int durationInTick = 0;
    public int tickAppear = 0;
    public float volume = 0;
}

public class MidiData {
    public string songName;
    public float beatsPerMinute;
    public float deltaTickPerQuarterNote;
    //used to calculate the default note length in music sheet 2 is quarter note, 3 is eighth note...
    public int denominator;
    public float delay;
    //list note in each track
    public Dictionary<int, List<NoteData>> notesData;
}