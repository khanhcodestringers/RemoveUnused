using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Mio.Utils {
    public class AudioManager : MonoSingleton<AudioManager> {
        [SerializeField]
        private AudioSource bgmPlayer;
        [SerializeField]
        private AudioSource[] sfxPlayers;
        [SerializeField]
        private int maxSFXStream = 5;

        private bool isInitialized = false;
        [SerializeField]
        private List<AudioClip> cachedSound;

        public bool bestPerformance = false;

        public bool cancelSound = false;

        private const string AUDIO_TYPE = "onbestperforment";

        public void Initialize () {
            if (bgmPlayer == null) {
                bgmPlayer = gameObject.AddComponent<AudioSource>();
            }

            bestPerformance = PlayerPrefs.GetInt(AUDIO_TYPE, 0) == 0 ? false : true;

            InitAudioControllWithDevice();
        }

        public void InitAudioControllWithDevice () {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (bestPerformance){
                MakeAudioSource();
            }else{
                AndroidNativeAudio.makePool(maxSFXStream);
            }
#else
            MakeAudioSource();
#endif
            if (cachedSound == null) {
                cachedSound = new List<AudioClip>(100);
            }
            isInitialized = true;
        }

        private float m_Volume = 0.7f;
        public float Volume {
            get { return m_Volume; }
            set {
                m_Volume = value;
                if (isInitialized) {
                    bgmPlayer.volume = m_Volume;
                    SetSFXVolume(m_Volume);
                }
            }
        }

        public void MakeAudioSource () {
            if (sfxPlayers == null || sfxPlayers.Length <= maxSFXStream) {
                sfxPlayers = new AudioSource[maxSFXStream];
            }

            for (int i = 0; i < maxSFXStream; i++) {
                sfxPlayers[i] = gameObject.AddComponent<AudioSource>();
                sfxPlayers[i].playOnAwake = false;
                sfxPlayers[i].dopplerLevel = 0;
                sfxPlayers[i].bypassEffects = true;
                sfxPlayers[i].bypassListenerEffects = true;
                sfxPlayers[i].bypassReverbZones = true;
            }
        }

        public bool MuteSFX { get; set; }

        private void SetSFXVolume (float vol) {
            for (int i = 0; i < sfxPlayers.Length; i++) {
                sfxPlayers[i].volume = vol;
            }
        }


        /// <summary>
        /// Load sound clip from specified path
        /// </summary>
        public int LoadSound (string soundPath) {
            if (!isInitialized) Initialize();

            AudioClip clip = Resources.Load<AudioClip>(soundPath);
            if (clip != null) {
                cachedSound.Add(clip);
                return cachedSound.Count - 1;
            }
            else {
                return -1;
            }
        }

        public int LoadSound (AudioClip clip) {
            if (!isInitialized) Initialize();

            if (clip != null) {
                cachedSound.Add(clip);
                return cachedSound.Count - 1;
            }

            return -1;
        }

        /// <summary>
        /// On Android, we must use soundpool, hence the different call
        /// </summary>
        /// <param name="soundPath"></param>
        /// <returns></returns>
        public int LoadSoundForAndroid (string soundPath) {
            if (!isInitialized) Initialize();

            return AndroidNativeAudio.load(soundPath);
        }

        public void UnLoadSoundForAndroidNative () {
            AndroidNativeAudio.releasePool();
        }

        public void UnLoadSoundForAudioClip () {
            if (sfxPlayers != null) {
                for (int i = 0; i < sfxPlayers.Length; i++) {
                    Destroy(sfxPlayers[i]);
                }
                sfxPlayers = null;
            }
            cachedSound.Clear();
        }

        public void PlaySound (int id, float volume = -1) {
            //if (cancelSound)
            //    return;
            if (!isInitialized) Initialize();

            if (volume == -1) {
                volume = m_Volume;
            }
            else {
                volume = volume * m_Volume;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            if (bestPerformance){
                PlayWithAudioSource(id, volume);
            }else{
                AndroidNativeAudio.play(id, volume);
            }                
#else
            PlayWithAudioSource(id, volume);
#endif
        }

        public void PlaySounds (int[] ids, float volume = -1) {
            if (!isInitialized) Initialize();
            //if (cancelSound)
            //    return;
            if (volume == -1) {
                volume = m_Volume;
            }
            else {
                volume = volume * m_Volume;
            }
                        
            AndroidNativeAudio.play(ids, volume);                            
        }

        public void PlayWithAudioSource (int id, float volume) {
            if (id >= 0 && id < cachedSound.Count) {
                //sfxPlayers[0].PlayOneShot(cachedSound[id]);
                int oldestIndex = 0;
                float longestAudioClip = 0;

                //check all currently sfx player
                for (int i = 0; i < maxSFXStream; i++) {
                    //if there is any idle player
                    if (!sfxPlayers[i].isPlaying) {
                        //use it to play the requested clip
                        sfxPlayers[i].clip = (cachedSound[id]);
                        sfxPlayers[i].volume = volume;
                        sfxPlayers[i].Play();
                        //sfxPlayers[i].PlayOneShot(cachedSound[id]);
                        //print(string.Format("Playing sound ID={0} on player number {1}",id, i));
                        //then end the method
                        return;
                    }
                    //if no player available
                    else {
                        //check the longest one
                        if (sfxPlayers[i].time > longestAudioClip) {
                            //and register it so we can play it later
                            oldestIndex = i;
                            longestAudioClip = sfxPlayers[i].time;
                        }
                    }
                }

                sfxPlayers[oldestIndex].Stop();
                sfxPlayers[oldestIndex].volume = volume;
                sfxPlayers[oldestIndex].clip = (cachedSound[id]);
                sfxPlayers[oldestIndex].Play();
                //sfxPlayers[oldestIndex].PlayOneShot(cachedSound[id]);

            }
        }

        public void SetAudioType (bool bestPerformance, Action onCompleted) {
            if (this.bestPerformance != bestPerformance) {
                this.bestPerformance = bestPerformance;
                MidiPlayer.Instance.SwitchAllSoundAndroidWithType(this.bestPerformance, onCompleted);
                int audioTypeValue = (this.bestPerformance == false ? 0 : 1);
                PlayerPrefs.SetInt(AUDIO_TYPE, audioTypeValue);
                PlayerPrefs.Save();
            }
            else {
                if (onCompleted != null)
                    onCompleted();
            }
        }
    }
}