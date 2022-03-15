using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Mio.TileMaster {
    public class LevelLoader : MonoBehaviour {
        //Track of midi file to store note data
        public static readonly int NoteTrack = 1;
        //Track of midi file used when play back song data
        public static readonly int PlaybackTrack = 0;
              
        public event Action<string> OnDownloadSongError;
        public event Action<bool, LevelDataModel> OnSongParsed;
        //just a hackathon style of function =.=. sorry, my dear code base
        public event Action<float> OnDownloadSongProgress;

        public LevelDataModel m_levelData;
        public void Initialize() {
            //ChangeState(UIState.Initialized);
        }

        //public void GetLevelData(SongDataModel songData) {
        //        DownLoadAndCacheSongData(songData);
        //}
        public void DownLoadAndCacheSongData(SongDataModel songData)
        {
            m_levelData = new LevelDataModel();
            m_levelData.songData = songData;
            AssetDownloader.Instance.DownloadAndCacheAsset(m_levelData.songData.songURL,
                        m_levelData.songData.version,
                        OnProgressChanged,
                        OnError,
                        OnLevelDownloadCompleted
                        );
        }

        private void OnLevelDownloadCompleted(WWW midi) {
            //try {
            byte[] midiData = midi.bytes;
            var data = new MidiData();
            MidiParser.ParseNotesData(midiData, ref data);

            //LevelDataModel levelData = new LevelDataModel();
            m_levelData.BPM = data.beatsPerMinute;
            m_levelData.denominator = data.denominator;
            m_levelData.tickPerQuarterNote = (int)data.deltaTickPerQuarterNote;
            //this.Print("Ticks per quarter note: " + data.deltaTickPerQuarterNote);

            m_levelData.noteData = data.notesData[NoteTrack];
            m_levelData.playbackData = data.notesData[PlaybackTrack];

            if (m_levelData.noteData == null || m_levelData.playbackData == null)
            {
                Helpers.CallbackWithValue(OnSongParsed, false, null);
            }
            else {
               
                Helpers.CallbackWithValue(OnSongParsed, true, m_levelData);
            }
            //}
            //catch(Exception ex) {
            // this.Print(ex.ToString());
            // Helpers.CallbackWithValue(OnSongParsed, false, null);
            //}
            //string notedata = midiData.text;
            //string[] data = notedata.Split('_');
            //if (OnTemporarySongParsed != null) {
            //    OnTemporarySongParsed(true, data);
            //}
        }

        private void OnError(string message) {
            //lbMessage.text = message;
            //ChangeState(UIState.WaitingForInput);
            Helpers.CallbackWithValue(OnDownloadSongError, message);
        }

        private void OnProgressChanged(float progress) {
            //progressbar.value = progress;
            Helpers.CallbackWithValue(OnDownloadSongProgress, progress);
        }

        
    }
}