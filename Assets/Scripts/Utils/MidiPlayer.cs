using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mio.Utils;
using System.IO;
using DG.Tweening;
using MovementEffects;

[RequireComponent(typeof(AudioSource))]
public class MidiPlayer : MonoSingleton<MidiPlayer> {
    MidiData data;
    List<NoteData> notesData;
    public AudioClip[] auClipSample;
    public AudioSource auPlaySoundNotes;
    private bool shouldPlay = true;
    public bool ShouldPlay {
        get { return shouldPlay; }
        set { shouldPlay = value; }
    }
    //private Dictionary<string, AudioClip> auClips;
    private Dictionary<string, int> pianoSoundIndexByName;
    private Dictionary<int, int> pianoSoundIndexByID;
    Queue<List<int>> queueNoteData = new Queue<List<int>>();

    //[Tooltip("Each note will be raise to a new velocity before playing, depend on this value")]
    //public int noteVelocityModifier = 36;

    public TextAsset backgroundMidiFile;
    private bool isInitialized = false;
    public bool IsInitialized {
        get { return isInitialized; }
    }

    public void Initialize () {
        if (!isInitialized) {
            if (auPlaySoundNotes == null) {
                //at least 1 component is guarantied to be available, since we required it in class definition 
                auPlaySoundNotes = GetComponent<AudioSource>();
            }
            pianoSoundIndexByName = new Dictionary<string, int>(100);
            pianoSoundIndexByID = new Dictionary<int, int>(100);
            //Debug.Log("init piano sample");
            Timing.RunCoroutine(IELoadPianoSound("Piano"));
            
            AudioManager.Instance.Volume = 0.8f;
        }
    }

    private AudioClip[] SortSample (AudioClip[] a) {
        Array.Sort(a, (AudioClip a1, AudioClip a2) => {
            return int.Parse(a1.name).CompareTo(int.Parse(a2.name));
        });
        return a;
    }


    public IEnumerator<float> IELoadPianoSound (string folder, Action onCompleted = null) {
        float lastRelease = Time.realtimeSinceStartup;
        for (int i = 21; i < 109; i++) {
            string soundName = i.ToString();
#if UNITY_ANDROID && !UNITY_EDITOR
            int soundID;
            if (!AudioManager.Instance.bestPerformance)
            {
                soundID = AudioManager.Instance.LoadSoundForAndroid(folder + Path.DirectorySeparatorChar + soundName + ".ogg");
            }
            else 
            {
                soundID = AudioManager.Instance.LoadSound(folder + Path.DirectorySeparatorChar + soundName);
            }
            //this.Print("Loaded sound name " + soundName + " with id: " + soundID.ToString());        
#elif NETFX_CORE
            int soundID = AudioManager.Instance.LoadSound(folder + "/" + soundName);
#else
            int soundID = AudioManager.Instance.LoadSound(folder + Path.DirectorySeparatorChar + soundName);
#endif
            pianoSoundIndexByName.Add(soundName, soundID);
            pianoSoundIndexByID.Add(i, soundID);
            if (Time.realtimeSinceStartup - lastRelease >= 0.02f) {
                lastRelease = Time.realtimeSinceStartup;
                //yield return Timing.WaitForSeconds(0.05f);
                yield return 0;
            }
        }
        if (onCompleted != null)
            onCompleted();
        shouldPlay = true;
        isInitialized = true;
    }


    public void SwitchAllSoundAndroidWithType (bool bestPreformance, Action onCompleted) {
        if (bestPreformance) {
            AudioManager.Instance.UnLoadSoundForAndroidNative();
        }
        else {
            AudioManager.Instance.UnLoadSoundForAudioClip();
        }
        pianoSoundIndexByID.Clear();
        pianoSoundIndexByName.Clear();
        AudioManager.Instance.InitAudioControllWithDevice();
        Timing.RunCoroutine(IELoadPianoSound("Piano", onCompleted));
    }
    /// <summary>
    /// From midi data, parse and edit to make sure it is ready to be played
    /// </summary>
    private void BuildNoteData () {
        //		print ("parse");
        SortedDictionary<float, List<int>> listNotes = new SortedDictionary<float, List<int>>();
        for (int i = 0; i < notesData.Count; i++) {
            float timeAppear = notesData[i].timeAppear;
            if (!listNotes.ContainsKey(timeAppear)) {
                List<int> listNotesID = new List<int>();
                listNotesID.Add(notesData[i].nodeID);
                listNotes.Add(timeAppear, listNotesID);
            }
            else {
                listNotes[timeAppear].Add(notesData[i].nodeID);
            }
        }
        queueNoteData.Clear();
        foreach (float f in listNotes.Keys) {
            queueNoteData.Enqueue(listNotes[f]);
        }
    }

    /// <summary>
    /// Play a single note using piano player
    /// </summary>
    /// <param name="noteID"></param>
    public void PlayPianoNote (int noteID, float volume = -1) {
        if (pianoSoundIndexByID.ContainsKey(noteID)) {
            AudioManager.Instance.PlaySound(pianoSoundIndexByID[noteID], volume);
        }
    }

    private void PlayAudioNotes (List<int> noteID) {
        if (noteID != null && noteID.Count > 0) {
            for (int i = 0; i < noteID.Count; i++) {
                if (pianoSoundIndexByID.ContainsKey(noteID[i])) {
                    AudioManager.Instance.PlaySound(pianoSoundIndexByID[noteID[i]]);
                }
                //this.Print("Playing note " + id);
                //if (id - noteVelocityModifier > 0) {
                //    //this.Print("--Play note successfully: " + id);
                //    auPlaySoundNotes.PlayOneShot(auClipSample[id - noteVelocityModifier]);
                //}
            }
        }
    }

    /// <summary>
    /// Play every note in this list, with a choice to respect different in start time
    /// </summary>
    /// <param name="listNotes">List of note data to play</param>
    /// <param name="timeRatio">How much the time stamp different being modified? Only has effect if the respectTimeStampDifferent is true</param>
    /// <param name="respectTimeStampDifferent">If the note datas have different time stamp, should them be played after each other or the same? </param>
    public void PlayPianoNotes (List<NoteData> listNotes, float timeRatio = 1, bool respectTimeStampDifferent = true, float delay = 0) {
        //print(string.Format("Playing {0} notes, time ratio = {1}, delay = {2}", listNotes.Count, timeRatio, delay));
        if (listNotes != null && listNotes.Count > 0) {
            //var listNotes = notes;
            float startTime = listNotes[0].timeAppear;
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!respectTimeStampDifferent) {
                if (listNotes.Count == 1) {
                    AudioManager.Instance.PlaySound(pianoSoundIndexByID[listNotes[0].nodeID]);
                }
                else {
                    int[] notes = new int[listNotes.Count];
                    for (int i = 0; i < listNotes.Count; i++) {
                        notes[i] = pianoSoundIndexByID[listNotes[i].nodeID];
                    }

                    AudioManager.Instance.PlaySounds(notes, listNotes[0].volume);
                }
            }else {
                float timeDelayed = (listNotes[0].timeAppear - startTime + delay) / timeRatio;
                if (listNotes.Count == 1) {
                    //AudioManager.Instance.PlaySound(pianoSoundIndexByID[listNotes[0].nodeID]);
                    Timing.RunCoroutine(CoroutinePlaySoundDelayed(listNotes[0].nodeID, timeDelayed, listNotes[0].volume));
                }
                else {
                    for (int i = 0; i < listNotes.Count; i++)
                    {
                        if (pianoSoundIndexByID.ContainsKey(listNotes[i].nodeID))
                        {
                            //in case user want to play notes in sequence
                            if (respectTimeStampDifferent)
                            {
                                //int noteID = listNotes[i].nodeID;
                                timeDelayed = (listNotes[i].timeAppear - startTime + delay) / timeRatio;
                                Timing.RunCoroutine(CoroutinePlaySoundDelayed(listNotes[i].nodeID, timeDelayed, listNotes[i].volume));
                               
                            }
                        }
                        else
                        {
                            AudioManager.Instance.PlaySound(pianoSoundIndexByID[listNotes[i].nodeID]);
                        }
                    }
                }
            }
#else
            for (int i = 0; i < listNotes.Count; i++) {
                if (pianoSoundIndexByID.ContainsKey(listNotes[i].nodeID)) {
                    //in case user want to play notes in sequence
                    if (respectTimeStampDifferent) {
                        //int noteID = listNotes[i].nodeID;
                        float timeDelayed = (listNotes[i].timeAppear - startTime + delay) / timeRatio;
                        Timing.RunCoroutine(CoroutinePlaySoundDelayed(listNotes[i].nodeID, timeDelayed, listNotes[i].volume));
                        //AudioManager.Instance.PlaySound(pianoSoundIndexByID[listNotes[i].nodeID]);
                        //we will do just that
                        //DOTween.Sequence()
                        //    .AppendInterval((listNotes[i].timeAppear - startTime) / timeRatio)
                        //    .OnComplete(() => {
                        //        //print("Play sound " + listNotes[i].nodeID + " after " + (listNotes[i].timeAppear - startTime));
                        //        AudioManager.Instance.PlaySound(pianoSoundIndexByID[noteID]);
                        //    })
                        //    .SetRecyclable(true)
                        //    .Play();
                    }                    
                }else{
                    AudioManager.Instance.PlaySound(pianoSoundIndexByID[listNotes[i].nodeID]);
                }
            }
#endif
        }
    }

    IEnumerator<float> CoroutinePlaySoundDelayed (int noteID, float delay, float volume = -1) {
        yield return Timing.WaitForSeconds(delay);
        AudioManager.Instance.PlaySound(pianoSoundIndexByID[noteID], volume);
        //print("--playing note " + noteID);
    }

    IEnumerator<float> CoroutinePlayPianoNoteDelayed (int[] noteIDs, float delay, float volume = -1) {
        yield return Timing.WaitForSeconds(delay);
        AudioManager.Instance.PlaySounds(noteIDs, volume);
        //print("--playing note " + noteID);
    }

    public void PlayNote (string note) {
        //if (auClips.ContainsKey(note)) {
        //    auPlaySoundNotes.PlayOneShot(auClips[note]);
        //}
    }

    /// <summary>
    /// Play next note in queue
    /// </summary>
    public void PlayNextNote () {
        if (!AudioManager.Instance.MuteSFX) {
            if (isInitialized && shouldPlay) {
                if (queueNoteData.Count > 0) {
                    List<int> noteID = queueNoteData.Dequeue();
                    //loop song
                    queueNoteData.Enqueue(noteID);
                    PlayAudioNotes(noteID);
                }
                //else {
                //    LoadSong(midiFile.bytes);
                //}
            }
        }
    }

    public void LoadSong (string filePath, bool isAbsoluteFilePath = false) {
        byte[] midiData = FileUtilities.LoadFile(filePath, isAbsoluteFilePath);
        LoadSong(midiData);
    }

    public void LoadSong (byte[] midiData) {
        data = new MidiData();
        if (midiData == null) {
            return;
        }
        MidiParser.ParseNotesData(midiData, ref data);
        notesData = data.notesData[0];
        BuildNoteData();
    }
    public void Pause () {
        shouldPlay = false;
    }

    public void Stop () {
        shouldPlay = false;
    }

    public void Play () {
        shouldPlay = true;
    }
}
