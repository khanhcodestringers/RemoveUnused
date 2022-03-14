using UnityEngine;
using System.Collections;
using MovementEffects;
using Mio.TileMaster;
using System.Collections.Generic;
using System;

//using System.Runtime.Serialization.Formatters.Binary;
public class SongConverter : MonoBehaviour {
#if UNITY_EDITOR
    //Track of midi file to store note data
    public static readonly int NoteTrack = 1;
    //Track of midi file used when play back song data
    public static readonly int PlaybackTrack = 0;

    public float tickTolerance = 0.125f;
    public int maximumJobs = 4;
    [Header("Parse 1 song only")]
    public bool shouldParseOneFileOnly = false;
    //public TextAsset midiFile;
    public int songIndex = 0;


    private bool[] parsingList;
    private int totalFile, downloadedFile;
    public void DownloadAllSong () {
        Timing.RunCoroutine(C_DownloadAllSong());
    }

    private IEnumerator<float> C_DownloadAllSong () {
        var storeData = GameManager.Instance.StoreData;
        if (storeData == null) {
            Debug.LogError("Store data is null, aborting...");
            yield break;
        }

        Debug.Log("======== Downloading songs from store data =======");
        Debug.Log("================== Please wait ==================");
        var allSongs = storeData.listAllSongs;
        totalFile = allSongs.Count; downloadedFile = 0;
        int count = 0;
        for (int i = 0; i < allSongs.Count; i++) {
            var song = allSongs[i];
            if (++count >= maximumJobs) {
                yield return Timing.WaitForSeconds(1);
                count = 0;
            }
            //Debug.Log("Downloading file from url: " + song.songURL);
            AssetDownloader.Instance.DownloadAndCacheAsset(song.songURL, song.version + 1, null, null, OnLevelDownloadCompleted, false);
        }
    }

    private string GetMidiPathFromURL (string url) {
        string savePath = url.Substring(url.LastIndexOf('/') + 1);
        Debug.Log("Save path: " + savePath + "---" + url);
        savePath = savePath.Substring(0, savePath.LastIndexOf('.'));
        savePath = GetSavePath("midi/" + savePath);
        return savePath;
    }

    private void OnLevelDownloadCompleted (WWW midi) {
        if (midi != null) {
            ++downloadedFile;
            string savePath = GetMidiPathFromURL(midi.url);
            FileUtilities.SaveFile(midi.bytes, savePath, true);
            Debug.LogFormat("Downloaded {0}/{2} file and saved file midi at {1}", downloadedFile, savePath, totalFile);
        }
        else {
            Debug.LogError("Downloaded failed");
        }
    }

    [ContextMenu("ParseSong")]
    public void ParseSongs () {
        if (!shouldParseOneFileOnly) {
            ParseAllSongs();
        }
        else {
            ParseSpecifiedSong();
        }
    }

    public void ParseSpecifiedSong () {
        var allSongs = GameManager.Instance.StoreData.listAllSongs;
        if (allSongs == null || allSongs.Count <= 0) {
            Debug.LogError("There is no song record, no parsing will happen.");
            return;
        }

        if (songIndex >= allSongs.Count) {
            Debug.LogError("Song index is invalid, no parsing will happen");
            return;
        }

        var midiPath = GetMidiPathFromURL(allSongs[songIndex].songURL);
        var midiBytes = FileUtilities.LoadFile(midiPath, true);
        if (midiBytes == null) {
            Debug.LogError("MIDI file not found or can't be read");
            return;
        }
        ParseMidiSong(midiBytes, allSongs[songIndex]);
    }

    public void ParseAllSongs () {
        var storeData = GameManager.Instance.StoreData;
        if (storeData == null) {
            Debug.LogError("Store data is null, aborting...");
            return;
        }

        Debug.Log("======== Parsing all songs from store data =======");
        Debug.Log("================================================");
        parsingList = new bool[maximumJobs];
        var allSongs = storeData.listAllSongs;
        Timing.RunCoroutine(C_ParseAllSongs(allSongs));
    }

    IEnumerator<float> C_ParseAllSongs (List<SongDataModel> allSongs) {
        //bool pause = false;
        for (int i = 0; i < allSongs.Count; i++) {
            while (ShouldPause()) {
                yield return Timing.WaitForSeconds(0.5f);
            }

            //mark in list as parsing
            for (int j = 0; j < maximumJobs; j++) {
                if (parsingList[j] == false) {
                    parsingList[j] = true;
                    break;
                }
            }
            Debug.LogFormat("Parsing midi file {0} of {1}, name: {2}, url : {3}", i + 1, allSongs.Count, allSongs[i].name,allSongs[i].songURL);
            var midiPath = GetMidiPathFromURL(allSongs[i].songURL);
            var midiBytes = FileUtilities.LoadFile(midiPath, true);
            if (midiBytes == null) {
                Debug.LogFormat("NULL midi at {0}, name: {1}, file path: {2}", i, allSongs[i].name, midiPath);
            }
            ParseMidiSong(midiBytes, allSongs[i]);
        }

        Debug.Log("========= Done Parsing ========");
    }

    private void JobCompleted () {
        for (int i = 0; i < maximumJobs; i++) {
            if (parsingList[i] == true) {
                parsingList[i] = false;
            }
        }
    }

    private bool ShouldPause () {
        for (int i = 0; i < maximumJobs; i++) {
            if (parsingList[i] == false) {
                return false;
            }
        }

        return true;
    }

    public void ParseMidiSong (byte[] midiSong, SongDataModel songModel) {
        //read midi file
        var data = new MidiData();
        MidiParser.ParseNotesData(midiSong, ref data);

        LevelDataModel lv = new LevelDataModel();
        lv.noteData = data.notesData[NoteTrack];
        lv.playbackData = data.notesData[PlaybackTrack];

        lv.playbackData.Sort((x, y) => (x.timeAppear.CompareTo(y.timeAppear)));
        lv.noteData.Sort((x, y) => (x.timeAppear.CompareTo(y.timeAppear)));

        lv.BPM = data.beatsPerMinute;
        lv.denominator = data.denominator;
        lv.tickPerQuarterNote = (int)data.deltaTickPerQuarterNote;

        var ticksPerTile = songModel.tickPerTile;
        var minTickPerTile = Mathf.FloorToInt(ticksPerTile * (1 - tickTolerance));
        var maxTickPerTile = Mathf.CeilToInt(ticksPerTile * (1 + tickTolerance));

        StartCoroutine(PrepareTileData(lv, minTickPerTile, maxTickPerTile, songModel));
    }

    IEnumerator PrepareTileData (LevelDataModel lv, int minTickPerTile, int maxTickPerTile, SongDataModel songDataModel, Action<float> onProgress = null, Action onComplete = null, Action<string> onError = null) {
        //listNoteData = noteData;
        var listTilesData = new List<TileData>(1000);
        var noteData = lv.noteData;
        var playbackData = lv.playbackData;
        //float BPM = lv.BPM;

        //we know that note data will always less or equals to playback data
        //so we will start by traverse through list note data
        NoteData currentNote, nextNote;
        int currentNoteIndex;
        //int loopCount = 0;
        //int maxLoop = 10;
        float lastReleaseThreadTime = Time.realtimeSinceStartup;
        float maxTimeLockThread = 1;
        //this variable is used to reduce number of cast operation
        float noteDataCount = noteData.Count;

        //for each note in view list
        for (int i = 0; i < noteData.Count; i++) {
            currentNoteIndex = i;

            //set up range for checking song data
            currentNote = noteData[currentNoteIndex];
            nextNote = null;

            //don't hog up all the CPU, save some for rendering task
            if (lastReleaseThreadTime + maxTimeLockThread >= Time.realtimeSinceStartup) {
                lastReleaseThreadTime = Time.realtimeSinceStartup;
                Helpers.CallbackWithValue(onProgress, ((i / noteDataCount)));
                yield return null;
            }

            //try to get next view note (must be different at timestamp with current note)
            while (++i < noteData.Count) {
                //++i;
                nextNote = noteData[i];
                //stop the loop right when next note is found
                if (nextNote.timeAppear != currentNote.timeAppear) {
                    //decrease i so that at the end of the loop, it can be increased gracefully
                    --i;
                    break;
                }
            }

            if (i >= noteData.Count) {
                i = noteData.Count - 1;
            }

            //how many notes existed at the same timestamp
            int numConcurrentNotes = i - currentNoteIndex + 1;
            //print("Num concurrent notes" + numConcurrentNotes + " at " + i);

            //for each note, create a tile
            for (int j = currentNoteIndex; j <= i; j++) {
                //print(string.Format("i {0}, j {1}, Concurrent notes: {2}", i, j, numConcurrentNotes));
                //print(string.Format("Current note: {0}, timestamp {1}; Next note; {2}, timestamp: {3}", currentNote.nodeID, currentNote.timeAppear, nextNote.nodeID, nextNote.timeAppear));
                //with each note data, there is a tile
                TileData tileData = new TileData();

                tileData.type = TileType.Normal;
                tileData.notes = new List<NoteData>();
                tileData.startTime = currentNote.timeAppear;
                tileData.startTimeInTicks = currentNote.tickAppear;
                tileData.soundDelay = 0;

                //fetch midi data for tile
                //float endTime = ((nextNote == null) ? currentNote.timeAppear + currentNote.duration : nextNote.timeAppear);
                //AddConcurrentMidiData(
                //        ref tileData,
                //        playbackData,
                //        currentNote.timeAppear,
                //        endTime,
                //        currentNoteIndex
                //        );
                int startTime, endTime;
                startTime = endTime = -1;
                switch (numConcurrentNotes) {
                    //only 1 tile 
                    case 1:
                        tileData.subType = TileType.Normal;
                        startTime = currentNote.tickAppear;
                        endTime = ((nextNote == null) ? currentNote.tickAppear + currentNote.durationInTick : nextNote.tickAppear);
                        break;
                    //dual tile
                    case 2:
                        tileData.subType = TileType.Dual;
                        if (j % 2 == 0) {
                            startTime = currentNote.tickAppear;
                            endTime = currentNote.tickAppear + (int)(currentNote.durationInTick * 0.5f);
                        }
                        else {
                            tileData.soundDelay = currentNote.duration * 0.5f;
                            startTime = currentNote.tickAppear + (int)(currentNote.durationInTick * 0.5f);
                            endTime = ((nextNote == null) ? currentNote.tickAppear + currentNote.durationInTick : nextNote.tickAppear);
                        }

                        break;
                    //big tile
                    case 3:
                        tileData.subType = TileType.Big;
                        if (listTilesData.Count > 1) {
                            TileData lastTileData = listTilesData[listTilesData.Count - 1];
                            if (lastTileData.startTimeInTicks != currentNote.tickAppear) {
                                startTime = currentNote.tickAppear;
                                endTime = ((nextNote == null) ? currentNote.tickAppear + currentNote.durationInTick : nextNote.tickAppear);
                            }
                            else {
                                startTime = endTime = -1;
                            }
                        }
                        break;
                }


                if (startTime >= 0 && endTime >= 0) {
                    //print("Adding note data into tile " + j);
                    AddConcurrentMidiDataByTick(
                            ref tileData,
                            playbackData,
                            startTime,
                            endTime,
                            j
                            );

                    tileData.durationInTicks = currentNote.durationInTick;
                    tileData.duration = currentNote.duration;
                    //Debug.Log(string.Format("Duration of note {0}, ID: {2} is {1}", i, tileData.duration, currentNote.nodeID));

                    //if a tile has duration belong to the normal tile's range
                    if (minTickPerTile <= tileData.durationInTicks && tileData.durationInTicks <= maxTickPerTile) {
                        //set it as so
                        tileData.type = TileType.Normal;
                        listTilesData.Add(tileData);
                    }
                    else {
                        //else, it is either a long note...
                        if (maxTickPerTile < tileData.durationInTicks) {
                            tileData.type = TileType.LongNote;
                            listTilesData.Add(tileData);
                        }
                        else {
                            //... or just an error note, fuck that shit
                            //Debug.LogWarning(string.Format("A tile data has duration of {0}, which is less than a normal tile ({1}), please check. It has Index of {2}, midi note {3}", tileData.durationInTicks, currentLevelData.songData.tickPerTile, currentNoteIndex, currentNote.nodeID));
                        }
                    }
                }
            }
        }

        //Debug.Log("Prepare tile data completed");
        //easy, start render in the next frame
        SongTileData b_data = new SongTileData();
        b_data.BPM = lv.BPM;
        b_data.denominator = lv.denominator;
        b_data.tickPerQuarterNote = lv.tickPerQuarterNote;
        b_data.titledata = listTilesData;
        b_data.songDataModel = songDataModel;
        SaveTileData(b_data, "songs/parsed/" + b_data.songDataModel.storeID);
        JobCompleted();
        yield return null;
        //Helpers.Callback(onComplete);
    }

    private static string GetSavePath (string name) {
        return FileUtilities.GetWritablePath("songs/" + name);
    }

    public static bool SaveTileData (SongTileData saveGame, string name) {
        //BinaryFormatter formatter = new BinaryFormatter();

        //using (FileStream stream = new FileStream(GetSavePath("parsed/"+name+".bytes"), FileMode.Create)) {
        //    try {
        //        formatter.Serialize(stream, saveGame);
        //    }
        //    catch (Exception e) {
        //        Debug.LogWarning(e.Message);
        //        return false;
        //    }
        //}

        //return true;
        return SaveBinarySongDataSystem.SaveTileData(saveGame, name);
    }

    private int AddConcurrentMidiDataByTick (ref TileData tile, List<NoteData> playbackData, int tickAppear, int tickEnd, int startSearchIndex) {
        if (playbackData.Count <= 0) { return 0; }

        //Debug.Log(string.Format("==========================\nAdding playback midi note for tile {0}, time appear {1}, time end {2}", tile.type, tickAppear, tickEnd));
        for (int i = startSearchIndex; i < playbackData.Count; i++) {
            //Debug.Log(string.Format("--Checking note play data {0} at {1} ", playbackData[i].nodeID, playbackData[i].tickAppear));
            if (playbackData[i].tickAppear == tickAppear) {
                //Debug.Log(string.Format("----Same timestamp, added note " + i));
                tile.notes.Add(playbackData[i]);
            }
            else if (playbackData[i].tickAppear > tickAppear) {
                if (playbackData[i].tickAppear < tickEnd) {
                    //Debug.Log(string.Format("----Timestamp in range, added note " + i));
                    tile.notes.Add(playbackData[i]);
                }
                else {
                    //print(string.Format("Added {0} notes", tile.notes.Count));
                    return i;
                }
            }
        }

        return -1;
    }
#endif
}
