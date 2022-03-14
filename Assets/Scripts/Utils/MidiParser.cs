using UnityEngine;
using System.Collections;
using CSSynth.Midi;
using System.Collections.Generic;

public class MidiParser : MonoBehaviour {
    /// <summary>
    /// Parse notes from midi data, combine all note event in specified tracks into one array
    /// </summary>
    /// <param name="midiData">Raw midi data</param>
    /// <param name="levelData">Result holder</param>
    /// <param name="tracksToParse">If null, will parse from all available track, else, only process specified tracks</param>
    /// <returns></returns>
    public static int ParseNotesData(byte[] midiData, ref MidiData levelData, List<int> tracksToParse = null) {
        int res = 0;

        //load up midi data
        MidiFile songFile = new MidiFile(midiData);

        //Look for Denominator first, before touching BPM or MicroSecondsPerQuarterNote
        var timesignatureEvents = songFile.getAllMidiEventsofType(MidiHelper.MidiChannelEvent.None, MidiHelper.MidiMetaEvent.Time_Signature);
        if (timesignatureEvents.Count > 0) {
            songFile.Denominator = (byte)timesignatureEvents[0].Parameters[1];
        }
        else {
            //default of midi without time_signature value
            songFile.Denominator = 2;
        }

        //set Tempo for this file, this value will depends on Denominator above
        var tempoEvents = songFile.getAllMidiEventsofType(MidiHelper.MidiChannelEvent.None, MidiHelper.MidiMetaEvent.Tempo);
        if (tempoEvents.Count > 0) {
            songFile.MicroSecondsPerQuarterNote = (uint)tempoEvents[0].Parameters[0];
        }
                
        //update levelData bpm
        levelData.beatsPerMinute = songFile.BeatsPerMinute;
        levelData.denominator = (int)songFile.Denominator;
        levelData.deltaTickPerQuarterNote = songFile.MidiHeader.deltaTicksPerQuarterNote;
        levelData.notesData = new Dictionary<int, List<NoteData>>(16);

        //prepare to calculate timing of each note event
        float secondsPerPulse = (float)songFile.MicroSecondsPerQuarterNote / 1000000 / (float)songFile.MidiHeader.deltaTicksPerQuarterNote;

        //start parsing from track 1, since track 0 is mostly meta events
        for (int i = 0; i < songFile.Tracks.Length; i++) {
            //Debug.Log("Processing track " + i);
            //only parse specified track if required
            if (tracksToParse != null && !tracksToParse.Contains(i)) { continue; }

            MidiEvent[] midiEvents = songFile.Tracks[i].midiEvents;

            
            //list of notes this track contains
            List<NoteData> notesList = new List<NoteData>(100);

            //prepare place to store note's timing
            Dictionary<byte, Stack<MidiEvent>> notesArray = new Dictionary<byte, Stack<MidiEvent>>();

            for (int j = 0; j < midiEvents.Length; j++) {
                if (midiEvents[j].isChannelEvent()) {
                    MidiEvent evt = midiEvents[j];
                    byte note = evt.parameter1;

                    //check for event's type, if is note_on push into timing table
                    if (evt.midiChannelEvent == MidiHelper.MidiChannelEvent.Note_On) {
                        if (notesArray.ContainsKey(note)) {
                            notesArray[note].Push(evt);
                        }
                        else {
                            Stack<MidiEvent> noteTiming = new Stack<MidiEvent>();
                            noteTiming.Push(evt);
                            notesArray.Add(note, noteTiming);
                        }
                        //print("--Added note " + note+ " into array");
                    }//if is note_off, calculate note duration and add to song data
                    else if (evt.midiChannelEvent == MidiHelper.MidiChannelEvent.Note_Off) {
                        MidiEvent onEvt = notesArray[note].Pop();
                        //when to be displayed
                        float noteTime = onEvt.deltaTimeFromStart * secondsPerPulse;
                        //how long the note should be displayed
                        float noteDuration = (evt.deltaTimeFromStart - onEvt.deltaTimeFromStart);

                        NoteData n = new NoteData();
                        n.durationInTick = (int)noteDuration;
                        n.tickAppear = (int) onEvt.deltaTimeFromStart;

                        noteDuration = (noteDuration > 0) ? noteDuration * secondsPerPulse : 0;
                        n.duration = noteDuration;
                        n.volume = onEvt.parameter2 / 127.0f;
                        //print("Volume for note = " + n.volume);

                        n.nodeID = note;
                        n.timeAppear = noteTime;

                        notesList.Add(n);
                        //Debug.Log("Added note: " + note);
                    }
                }
            }

            //add to level data
            levelData.notesData.Add(i, notesList);
        }

        return res;
    }
}
