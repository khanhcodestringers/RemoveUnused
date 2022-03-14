using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using DG.Tweening;
using MovementEffects;

namespace Mio.TileMaster {
    public class LongNotePlayedData {
        //system time when started to play the note data list
        public float timeStampBeginTouch = 0;
        //system time when played the first note
        public float timeStartPlayFirstNote = 0;
        //the time stamp of the first note played
        public float timeStampOfFirstNote = 0;

        //the order index of last played note
        public int noteDataPlayedIndex = -1;
        public bool touchRelease = false;
        public bool isScored = false;
    }

    public enum MusicNoteType {
        ThirdtySecond = 0,
        Sixteenth,
        Eighth,
        Quarter,
        Half,
        WholeNote,
        FreeStyle
    }

    [Serializable]
    public class ParsedTilesData {
        public int version;
        public List<TileData> listTiles;
    }

    public class TileMasterGamePlay : MonoBehaviour {
        public const string TRIGGER_NORMAL = "Reset";
        public const string TRIGGER_FIRST_STAR = "FirstStar";
        public const string TRIGGER_FIRST_CROWN = "FirstCrown";
        public const string TRIGGER_SECOND_CROWN = "SecondCrown";        
        public const string TRIGGER_THIRD_CROWN = "ThirdCrown";

        //a really small number (entropy)
        public static readonly float ENTROPY = 0.005f;
        public static readonly int TILE_COLUMN = 4;
        public enum GameState {
            Invalid,
            Initialized,
            Playing,
            Paused,
            GameOver,
            Continue,
            AutoPlay
        }

        
        // This class is a mess, bear with me
        //    ()_()()_()
        //    / ..)(.. \
        // __/  ( || )  \_
        //(_/    *  *   (_)
        // |       |   | ||
        // |  |    |   \_/|
        // \__/    |      |
        //   >      \    /
        //  <__,--,__|/|_>
        

        #region Editor's variable
        public Camera tileRenderCamera;
        public NoteStart noteStart;
        //public TilesRow tilePrefab;

        //[Tooltip("The first tile will be spawn at this row")]
        //public Transform tileStartPosition;
        //[Tooltip("When the game end, error tile will be move back to this position")]
        //public Transform tileErrorPosition;
        //[Tooltip("Any tiles go lower than this point will be destroy")]
        //public Transform tileEndPosition;
        //[Tooltip("Column to spawn tile into")]
        //public Transform[] tileColumns;
        [Tooltip("Normal (black) tile's height in screen with ratio of 16:9")]
        public int normalTileHeightScreenRatio169 = 484;
        private int tileRowHeight = 480;

        [SerializeField]
        //private UIPanel panelTiles;
        public UIWidget tilesContainer;
        [Tooltip("Any objects listed here will be disable when the scene start")]
        [SerializeField]
        private List<GameObject> disableOnStart;
        [Tooltip("The endless label to be shown")]
        [SerializeField]
        private UILabel endlessLabel;

        [Header("Stars and crowns")]
        [Space(5)]
        [Tooltip("Star sprites to be shown when needed")]
        [SerializeField]
        private RecordView recordviewStars;
        [Tooltip("Crown sprites to be shown when needed")]
        [SerializeField]
        private RecordView recordviewCrowns;

        //[SerializeField]
        //private ParticleSystem liveBG;

        [Tooltip("The tile to display level info")]
        public InfoTile infoTile;
        public UILabel lbScoreDisplay;

        public Animation anmScoreDisplay;

        [Header("Live background variables")]
        [SerializeField]
        private Animator bgAnimator;
        [SerializeField]
        private ParticleSystem particleBubble;
        //[SerializeField]
        //private ParticleSystem particleStarDust;
        [SerializeField]
        private ParticleSystem particleSnow;

        [Header("Tile's size")]
        [Tooltip("Index 0 is head, index 1 is tail")]
        [SerializeField]
        //this one is used for calculating tile's length in unit
        private List<Transform> tileMarkArray;
        [SerializeField]
        private float tileLengthInUnit;
        #endregion

        #region Tile management
        //hold references for all spawned tiles
        private List<TilesRow> tiles;
        //hold a record of how many tiles have been spawned
        private int numTileGenerated = 0;
        //at which index a dual tile has been created?
        private int lastDualTileIndex = -1;
        //a reference for quick accessing of youngest tile
        //private TilesRow lastGeneratedTile = null;
        //used to make sure 2 consequence tiles will not have the same column
        private int lastGeneratedTileColumn = 0;
        //used to check if the game is over or not
        //private int oldestTileIndex = 0;
        //number of tiles have been touched
        private int numTileConsumed = 0;
        #endregion

        #region Reading the MIDI
        //read from level file, hmmm... BPM?
        //private int tickPerWholeNote = 1920;
        //read from level file, how many ticks (in MIDI) there are in a normal tile
        private int ticksPerTile = 96;
        //read from MIDI file, the... denominator of the song, refer to wiki for more information, could not be clearer on this :D
        private int denominator = 2;
        //an error-tolerance to determine if a note in midi should be treat as a normal tile, or just a part of a long tile.
        //if a note has duration of 119, when ticksPerTile is 120, that note will be a normal tile if tickTolerance is more than 0.01 (because 120 * 0.99 <= 119 <= 120 * 1.01)
        private float tickTolerance = 0.125f;
        private int minTickPerTile, maxTickPerTile;
        //duration of note, from thirty-second to whole note (6 in total)
        //private static readonly float[] relativeDuration = new float[] { 1f / 32f, 1f / 16f, 1f / 8f, 1f / 4f, 1f / 2f, 1f };
        //num tick each note have
        //private int[] tickPerNoteType;
        //range of num ticks each note have, from thirty-second to whole note (6 note => 12 point in total)
        //private int[] tickRange = new int[12];

        #endregion
            
        #region Game Speed
        //control the start, current and max speed of the game
        private float baseRunSpeed = 0.75f;
        private float currentRunSpeed = 0.75f;
        private float maxRunSpeed = 0.75f * 4;

        //game speed to start with after user chose to Continue game
        private float speedAfterContinue = 2f;

        //base run speed of the game, in local NGUI position format
        private float baseLocalRunSpeed;

        //use to notify the game that it need to increase its speed
        private bool isIncreasingSpeed = false;
        private float targetSpeed;
        //private float speedDelta;
        //flash tang toc notes;
        //private bool waitForUserFamilialSpeedNotes = false;        

        //read from level config, how fast the game should run?
        private float speedModifier = 1.0f;
        //this variable is used to control how fast audio notes in consequence will be play. 1 for normal speed
        private float speedRatio;

        private List<float> speedTableNotes = new List<float>(6);
        #endregion
        
        #region Miscellaneous 
        private LevelDataModel currentLevelData;
        /// <summary>
        /// Get the level data which has been loaded into this game logic
        /// </summary>
        public LevelDataModel CachedLevelData { get { return currentLevelData; } }

        public GameState currentState = GameState.Invalid;

        //store data of each tiles will appear in the game
        private List<TileData> listTilesData;
        //private List<NoteData> listNoteData;

        //an array of index, at which the game will reward user with 1 star
        private int[] starRecords = new int[7];
        private int currentNumStar = 0;

        //only used in calculating speed, nothing serious here, should be removed
        private int dataCountUsedInSpeedCalculating = 0;

        //list of long notes that has been registered as touched, this list will help us prevent users from touching the long note many time, as well as decide when, where and what sound to play
        private Dictionary<int, LongNotePlayedData> listLongNotePlayedData;

        public const int MAX_RETRY = 3;
        private int numContinue = 0;

        //private float diamondChances = 0.05f;
        //public const int maxDiamondPerGame = 4;
        private int diamondDropped = 0;

        [SerializeField]
        private GameObject btnStopAutoPlay;

        //is the song being played or just auto playing?
        public bool isListenThisSong = false;
        #endregion

        //if 2 notes have time delta below this threshold, they will be move closer together
        private readonly float MINIMUM_TIME_DIFFERENT = 0.05f;

        public event Action OnGameOver;

        private bool isInit = false;

        public float GetSpeedRatio () {
            return speedRatio;
        }
        public float GetCurrentSpeed () {
            return currentRunSpeed;
        }

        void Start () {
        }

        public void Initialize () {
            if (isInit)
                return;

            listLongNotePlayedData = new Dictionary<int, LongNotePlayedData>(5000);
            //tickPerNoteType = new int[6];
            InGameUIController.Instance.Setup(this);
            isInit = true;
        }

        public void SetLevelData (LevelDataModel dat) {
            currentLevelData = dat;
            ticksPerTile = dat.songData.tickPerTile;
            denominator = dat.denominator;
            //tickPerWholeNote = dat.tickPerQuarterNote * 4;
            speedModifier = dat.songData.speedModifier;

            minTickPerTile = Mathf.FloorToInt(ticksPerTile * (1 - tickTolerance));
            maxTickPerTile = Mathf.CeilToInt(ticksPerTile * (1 + tickTolerance));
        }

        public void SetTilesData (SongTileData levelData) {
            //SongTileData levelData = SaveBinarySongDataSystem.LoadTileDataFromResource(currentLevelData.songData.storeID);
            if (levelData != null) {
                if (levelData.songDataModel.version == currentLevelData.songData.version) {
                    listTilesData = levelData.titledata;
                    ticksPerTile = levelData.songDataModel.tickPerTile;
                    this.currentLevelData.BPM = levelData.BPM;
                    this.currentLevelData.denominator = denominator = levelData.denominator;
                    this.currentLevelData.tickPerQuarterNote = levelData.tickPerQuarterNote;
                }
            }
        }

        public void LoadTileData () {
            SongTileData levelData = SaveBinarySongDataSystem.LoadTileDataFromResource(currentLevelData.songData.storeID);
            if (levelData != null) {
                if (levelData.songDataModel.version == currentLevelData.songData.version) {
                    listTilesData = levelData.titledata;
                    ticksPerTile = levelData.songDataModel.tickPerTile;
                    this.currentLevelData.BPM = levelData.BPM;
                    this.currentLevelData.denominator = denominator = levelData.denominator;
                    this.currentLevelData.tickPerQuarterNote = levelData.tickPerQuarterNote;
                }
            }
        }

        public void PrepareTileData (Action<float> onProgress = null, Action onComplete = null, Action<string> onError = null) {
            SongTileData listTileData = SaveBinarySongDataSystem.LoadTileData(currentLevelData.songData.storeID);

            if (listTileData != null) {
                if (listTileData.songDataModel.version == currentLevelData.songData.version) {
                    listTilesData = listTileData.titledata;
                    Helpers.Callback(onComplete);
                }
                else {
                    CoroutineTitleData(onProgress, onComplete, onError);
                }
            }
            else {
                CoroutineTitleData(onProgress, onComplete, onError);
            }
        }
        private void CoroutineTitleData (Action<float> onProgress = null, Action onComplete = null, Action<string> onError = null) {
            StartCoroutine(PrepareTileData(currentLevelData.noteData, currentLevelData.playbackData, currentLevelData.BPM, onProgress, onComplete, onError));
        }

        /// <summary>
        /// From provided note data, prepare tile data for the game. After this methods is call, the game can be played
        /// </summary>
        IEnumerator PrepareTileData (List<NoteData> noteData, List<NoteData> playbackData, float BPM, Action<float> onProgress = null, Action onComplete = null, Action<string> onError = null) {
            //listNoteData = noteData;
            listTilesData = new List<TileData>(1000);

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
                        //int _endTimeTmp = i < noteData.Count - 1 ? noteData[i + 1].tickAppear: endTime;
                        AddConcurrentMidiDataByTick(
                                ref tileData,
                                playbackData,
                                startTime,
                                endTime,
                                j
                                );

                        tileData.durationInTicks = currentNote.durationInTick;
                        tileData.duration = currentNote.duration;
                       
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

            //easy, start render in the next frame
            SongTileData b_data = new SongTileData();
            b_data.titledata = listTilesData;
            b_data.songDataModel = currentLevelData.songData;
            SaveBinarySongDataSystem.SaveTileData(b_data, b_data.songDataModel.storeID);
            yield return null;
            Helpers.Callback(onComplete);
        }

        /// <summary>
        /// Auto add all midi note that has the same time stamp as specified
        /// </summary>
        /// <param name="tile">Tile object to add note into</param>
        /// <param name="playbackData">The list of note data to add</param>
        /// <param name="timeAppear">Begin to check note data from this time stamp (inclusive)</param>
        /// <param name="timeEnd">Note data before this timestamp will be added (exclusive)</param>
        /// <param name="startSearchIndex">Index of playbackData list to start searching</param>
        /// <returns></returns>
        private int AddConcurrentMidiDataByTime (ref TileData tile, List<NoteData> playbackData, float timeAppear, float timeEnd, int startSearchIndex) {
            if (playbackData.Count <= 0) { return 0; }
            //this.Print(string.Format("Adding playback midi note for tile {0}, time appear {1}, time end {2}", tile.type, timeAppear, timeEnd));
            for (int i = startSearchIndex; i < playbackData.Count; i++) {
                //this.Print(string.Format("--Checking note play data {0} at {1} ", playbackData[i].nodeID, playbackData[i].timeAppear));
                if (playbackData[i].timeAppear == timeAppear) {
                    //this.Print(string.Format("----Added "));
                    tile.notes.Add(playbackData[i]);

                }
                else if (playbackData[i].timeAppear > timeAppear) {
                    if (playbackData[i].timeAppear < timeEnd) {
                        //this.Print(string.Format("----Added "));
                        tile.notes.Add(playbackData[i]);
                    }
                    else {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Auto add all midi note that has the same time as specified
        /// </summary>
        /// <param name="tile">Tile object to add note into</param>
        /// <param name="playbackData">The list of note data to add</param>
        /// <param name="tickAppear">Begin to check note data from this time stamp (inclusive)</param>
        /// <param name="tickEnd">Note data before this timestamp will be added (exclusive)</param>
        /// <param name="startSearchIndex">Index of playbackData list to start searching</param>
        /// <returns></returns>
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



        public void ResetGame () {
            Counter.Clear();
           
            listLongNotePlayedData.Clear();

            //reset game speed
            if (isIncreasingSpeed) {
                Timing.KillCoroutine(coroutineIncreasingSpeed);
                isIncreasingSpeed = false;
            }

            isListenThisSong = false;
            btnStopAutoPlay.SetActive(false);
            PrepareSpeedTableNotes();

            //hide all record view
            if (recordviewCrowns.gameObject.activeInHierarchy) {
                recordviewCrowns.ShowNumRecord(0);
                recordviewCrowns.SetVisible(false);
            }
            if (recordviewStars.gameObject.activeInHierarchy) {
                recordviewStars.ShowNumRecord(0);
                recordviewStars.SetVisible(false);
            }
            currentNumStar = 0;

            tileRowHeight = normalTileHeightScreenRatio169;
            
            //pre-calculate at which index we will increase number of star
            starRecords[0] = Mathf.CeilToInt(listTilesData.Count / 3);
            starRecords[1] = Mathf.CeilToInt(listTilesData.Count / 3 * 2);
            starRecords[2] = Mathf.CeilToInt(listTilesData.Count);
            starRecords[3] = Mathf.CeilToInt(listTilesData.Count / 3 * 4);
            starRecords[4] = Mathf.CeilToInt(listTilesData.Count * 2);
            starRecords[5] = Mathf.CeilToInt(listTilesData.Count * 3);
            starRecords[6] = Mathf.CeilToInt(listTilesData.Count * 100000);

           

            //lbScoreDisplay.text = "0";
			IncreaseAndShowScore (0);

            numContinue = 0;

            diamondDropped = 0;

            bgAnimator.SetTrigger(TRIGGER_NORMAL);
            particleBubble.gameObject.SetActive(false);
            //particleStarDust.gameObject.SetActive(false);
            particleSnow.gameObject.SetActive(false);

            infoTile.SetSongInfo(currentLevelData.songData.name, HighScoreManager.Instance.GetHighScore(currentLevelData.songData.storeID));

            numTileConsumed = numTileGenerated = 0;
            //nextNotePlaybackDataIndex = 0;
            lastDualTileIndex = -1;

            //calculate how long (in unit) a tile should be, with proper scale from reference screen ratio;
            tileLengthInUnit = tileMarkArray[0].position.y - tileMarkArray[1].position.y;

            //calculate how many tiles must be spawned each second to keep up with the speed of the song
            float tilesPerSecond = (currentLevelData.BPM / 60f * currentLevelData.tickPerQuarterNote * 4 / Mathf.Pow(2, denominator) / ticksPerTile);

            //speed of the game is in Unit/Second, hence, equals to ((number of tiles) * tile length) per second. Then multiply by unique modifier from level's description
            baseRunSpeed = ConvertTilePerSec2Speed(tilesPerSecond);
            currentRunSpeed = baseRunSpeed * speedTableNotes[0];

            //mark base speed for tile placement
            baseLocalRunSpeed = tilesPerSecond * tileRowHeight * speedModifier;

            //slow to fast, prevent user from missing at first tile
            targetSpeed = currentRunSpeed;
            if (targetSpeed < speedAfterContinue) {
                isIncreasingSpeed = false;
            }
            else {
                IncreaseGameSpeed(speedAfterContinue, targetSpeed);
            }

            //old formula, can be run but is wrong, need heavily assistance from level description
            //currentRunSpeed = baseRunSpeed = (1/ (currentLevelData.BPM / 60f * currentLevelData.tickPerQuarterNote * 4 / Mathf.Pow(2,denominator) / ticksPerTile)) * speedModifier;

            speedRatio = speedModifier;
            maxRunSpeed = baseRunSpeed * (currentLevelData.songData.maxBPM / currentLevelData.BPM);

            currentState = GameState.Initialized;

            //calculate which value to use for data count. Its value must be dividable by 3
            dataCountUsedInSpeedCalculating = listTilesData.Count;
            if (dataCountUsedInSpeedCalculating % 3 != 0) {
                int min, max;
                min = max = dataCountUsedInSpeedCalculating;
                while (true) {
                    --min;
                    ++max;
                    if (min % 3 == 0) {
                        dataCountUsedInSpeedCalculating = min;
                        break;
                    }

                    if (max % 3 == 0) {
                        dataCountUsedInSpeedCalculating = max;
                        break;
                    }
                }
            }


            tilesContainer.gameObject.SetActive(true);

            InGameUIController.Instance.ResetForNewGame();
            //Collect GC now, when the game is preparing
            GC.Collect();
        }

        private float ConvertTilePerSec2Speed (float tilesPerSecond) {
            return (((tilesPerSecond) * tileLengthInUnit)) * speedModifier;
        }

        public int GetNumTileTillNextStar (int currentStar) {
            if (currentStar >= 0 && currentStar < starRecords.Length) {
                return starRecords[currentStar] - numTileConsumed;
            }

            return -1;
        }

        //do not use this variable anywhere else
        private int zz_rest_distance = 0;

        public void GenerateNextTile () {
            //Debug.LogError ("GenerateNextTile:"+listTilesData.Count+","+numTileGenerated+","+InGameUIController.Instance.GetCountNoteActive());
            //this.Print("Generating next tile");
            if (listTilesData.Count <= 0) { return; }

            if (numTileGenerated < 0) { numTileGenerated = 0; }
            //get suitable tile data
            int tileIndex = numTileGenerated % listTilesData.Count;
            if (tileIndex == 0 && numTileGenerated != 0) {
                zz_rest_distance = 1500;
            }
            else {
                zz_rest_distance = 0;
            }

            TileData tileData = listTilesData[tileIndex];

            //calculate tile's position
            Vector3 spawnPos = new Vector3(0, 480 - 1140, 0);//tileStartPosition.localPosition;
            //random tile's location
            int tileColumIndex = 0;

            //if this is not the first tile to be spawn
            if (numTileGenerated > 0) {
                NoteSimple note = InGameUIController.Instance.GetLastNoteGenerate();
                TileData lastTile = note.data;
                //print(string.Format("Num tile generated: {0}, last dual tile index {1}", numTileGenerated, lastDualTileIndex));
                if (((lastTile.subType != TileType.Dual) || (tileData.subType != TileType.Dual)) || (numTileGenerated - lastDualTileIndex <= 1)) {

                    //if the tile is a dual note or a note right after dual note, set it as next to the last generated tile
                    if (tileData.subType == TileType.Dual || lastTile.subType == TileType.Dual) {
                        tileColumIndex = (lastGeneratedTileColumn + 1) % TILE_COLUMN;
                    }
                    else {
                        //if the tile is a normal one, random for a column index
                        tileColumIndex = UnityEngine.Random.Range(0, TILE_COLUMN);
                        //but the random column can not be the same as the last one
                        if (tileColumIndex == lastGeneratedTileColumn) {
                            //so we improvise here
                            tileColumIndex = lastGeneratedTileColumn + UnityEngine.Random.Range(1, TILE_COLUMN - 1);
                            tileColumIndex = tileColumIndex % TILE_COLUMN;
                        }
                    }

                    //calculate time appear of this to-be-generated-tile
                    float supposeAppearTime = lastTile.startTime + lastTile.duration;
                    float appearTime = tileData.startTime;

                    //if it's not too far from the last tile
                    if (appearTime <= supposeAppearTime + MINIMUM_TIME_DIFFERENT) {
                        //spawn it at normal position (without any empty area in between them)
                        spawnPos.y = note.transform.localPosition.y + note.height - 1140 + zz_rest_distance;
                    }
                    else {
                        spawnPos.y = note.transform.localPosition.y + note.height - 1140 + zz_rest_distance + (appearTime - supposeAppearTime) * baseLocalRunSpeed;
                    }
                }
                else {
                    //record current index as last dual tile created
                    lastDualTileIndex = numTileGenerated;
                    tileColumIndex = (lastGeneratedTileColumn + 2) % TILE_COLUMN;
                    spawnPos.y = note.transform.localPosition.y - 1140 + zz_rest_distance;
                }
            }

            lastGeneratedTileColumn = tileColumIndex;

            //create a new tile
            int tileLength = 0;
            if (tileData.type == TileType.LongNote) {
                //print(string.Format("Duration in ticks: {0}, tick per tile: {1}", tileData.durationInTicks, ticksPerTile));
                tileData.score = Mathf.RoundToInt(tileData.durationInTicks * 1.0f / ticksPerTile);
                tileLength = Mathf.RoundToInt(tileData.durationInTicks * 1.0f / ticksPerTile * tileRowHeight);
            }
            else {
                tileData.score = 1;
                tileLength = tileData.score * tileRowHeight;
            }


            bool withBonusTile = false;

            if (CurrentGameStage == GameState.Playing) {
                if (numTileGenerated > listTilesData.Count) {
                    if (tileData.subType != TileType.Dual) {
                        if (diamondDropped < GameManager.Instance.GameConfigs.maxDiamondPerGame) {
                            if (currentRunSpeed >= GameManager.Instance.GameConfigs.speedToDropDiamond
                                && Counter.GetQuantity(Counter.KeyScore) >= GameManager.Instance.GameConfigs.scoreToDropDiamond) {
                                float chance = UnityEngine.Random.Range(0, 1.001f);
                                if (chance <= GameManager.Instance.GameConfigs.diamondChance) {
                                    withBonusTile = true;
                                    ++diamondDropped;
                                }
                            }
                        }
                    }
                }
            }

            InGameUIController.Instance.CreateNewNote(tileData, spawnPos, numTileGenerated, tileColumIndex, tileLength, withBonusTile);

            numTileGenerated++;
        }

        /// <summary>
        /// Setup UI when autoplay is clicked
        /// </summary>
        private void SetAutoPlayUI () {
           
            //particleBubble.maxParticles = 50;
            particleBubble.gameObject.SetActive(true);
            bgAnimator.SetTrigger(TRIGGER_SECOND_CROWN);
            particleSnow.gameObject.SetActive(true);
            particleSnow.Play();
            btnStopAutoPlay.SetActive(true);
        }

        /// <summary>
        /// Show animation when a new star has been reached
        /// </summary>
        /// <param name="stars"></param>
        private void ShowRecordProgress (int stars) {
            //play background animation
            if (stars == 1) {
                bgAnimator.SetTrigger(TRIGGER_FIRST_STAR);
            }
            else if (stars == 2) {
                var main = particleBubble.main;
                main.maxParticles = 10;
                particleBubble.gameObject.SetActive(true);
            }
            else if (stars == 3) {
                var main = particleBubble.main;
                main.maxParticles = 50;
            }
            else if (stars == 4) {
                bgAnimator.SetTrigger(TRIGGER_FIRST_CROWN);
                particleSnow.gameObject.SetActive(true);
                particleSnow.Play();
            }
            else if (stars == 5) {
                bgAnimator.SetTrigger(TRIGGER_SECOND_CROWN);
            }else if(stars == 6) {
                bgAnimator.SetTrigger(TRIGGER_THIRD_CROWN);
            }

            recordviewCrowns.SetVisible(true, 0.25f);
            recordviewCrowns.ShowNumRecord(stars - 3);

            DOTween.Sequence()
                .AppendInterval(1.25f)
                .OnComplete(() => {
                    recordviewStars.SetVisible(false, 0.25f);
                    recordviewCrowns.SetVisible(false, 0.25f);
                })
                .Play();

            if (stars == 3) {
                endlessLabel.gameObject.SetActive(true);
                DOTween.Sequence()
                .AppendInterval(1.25f)
                .OnComplete(() => {
                    endlessLabel.gameObject.SetActive(false);
                })
                .Play();
            }
        }

        private IEnumerator<float> coroutineIncreasingSpeed;
        /// <summary>
        /// Increase speed of the game smoothly
        /// </summary>
        /// <param name="startSpeed">Speed to smoothing from</param>
        /// <param name="targetSpeed">Speed to achieve after smoothing end</param>
        /// <param name="duration">The duration of the smoothing process</param>
        /// <param name="timeStep">How often the speed should be increase</param>
        private void IncreaseGameSpeed (float startSpeed, float targetSpeed, float duration = 3f, float timeStep = 0.1f) {
            //if(startSpeed == null) {
            //    startSpeed = currentRunSpeed;
            //}

            if (duration < 0 || timeStep <= 0 || timeStep > duration) {
                Debug.LogWarning(string.Format("Invalid arguments when trying to increase game speed. Duration: {0}, TimeStep: {1}", duration, timeStep));
                return;
            }

            //targetSpeed = Mathf.Max(targetSpeed, this.targetSpeed);
            targetSpeed = Mathf.Min(targetSpeed, maxRunSpeed);

            //do nothing if the speed is too much already
            if (startSpeed >= Mathf.Min(targetSpeed, maxRunSpeed)) {
                isIncreasingSpeed = false;
                return;
            }

            if (!isIncreasingSpeed) {
                //Debug.Log("Calling increasing speed");
                coroutineIncreasingSpeed = Timing.RunCoroutine(C_SmoothIncreasingSpeed(startSpeed, targetSpeed, duration, timeStep));
            }
            else {
                //Debug.Log("Speed is still increasing, restarting coroutine");
                Timing.KillCoroutine(coroutineIncreasingSpeed);
                coroutineIncreasingSpeed = Timing.RunCoroutine(C_SmoothIncreasingSpeed(startSpeed, targetSpeed, duration, timeStep));
            }
        }

        /// <summary>
        /// As it said, increasing speed of the game smooth-fucking-ly
        /// </summary>
        /// <param name="startSpeed">From this speed</param>
        /// <param name="targetSpeed">To this fucking one</param>
        /// <param name="duration">In this much seconds</param>
        /// <param name="timeStep">With the frequency equals to this value</param>
        /// <returns></returns>
        private IEnumerator<float> C_SmoothIncreasingSpeed (float startSpeed, float targetSpeed, float duration, float timeStep) {
            isIncreasingSpeed = true;

            float speedStep = (targetSpeed - startSpeed) / duration * timeStep;

            currentRunSpeed = startSpeed;

            while (currentRunSpeed < targetSpeed) {
                yield return Timing.WaitForSeconds(timeStep);

                //only increase speed if the game is playing
                if (currentState == GameState.Playing) {
                    currentRunSpeed += speedStep;
                    speedRatio = (currentRunSpeed / baseRunSpeed) * speedModifier;
                }
            }

            isIncreasingSpeed = false;
        }


        void Update () {
            InGameUIController.Instance.OnProcessInputControl();

            if (currentState == GameState.Playing ||
                currentState == GameState.AutoPlay) {
                
                //check for new UI
                InGameUIController.Instance.ProcessUpdate(currentRunSpeed);
            }
        }

        public void TileFinished () {
            ++numTileConsumed;
        }

        /// <summary>
        /// This right here, my friend, is an example of bad code, DO NOT follow
        /// </summary>
        /// <param name="score"></param>
        public void IncreaseAndShowScore (int score = 1) {
            if (isListenThisSong)
                return;

            if (score >= 0) {
                anmScoreDisplay.Play();
                lbScoreDisplay.text = Counter.Count(Counter.KeyScore, score).ToString();
                TryIncreaseSpeed();
            }
        }


        /// <summary>
        /// Fetch the speed sheet from thin air, ready to move
        /// </summary>
        private void PrepareSpeedTableNotes () {
            if (currentLevelData.songData.speedPerStar != null) {
                //priority speed table of each level
                speedTableNotes = currentLevelData.songData.speedPerStar;
            }
            else if (GameManager.Instance.GameConfigs != null && GameManager.Instance.GameConfigs.defaultSpeedTable != null) {
                //if level does not contain speed table, use general one from config file
                speedTableNotes = currentLevelData.songData.speedPerStar = GameManager.Instance.GameConfigs.defaultSpeedTable;
            }
            else {
                //in case game data can't be read, use default speed table
                speedTableNotes = new List<float>() { 1f, 1.1f, 1.25f, 1.5f, 1.8f, 0.05f };
            }
        }

        /// <summary>
        /// Oh, we got a bonus tiles, better handle that shit
        /// </summary>
        /// <param name="tile"></param>
        public void ProcessBonusTile (NoteBonus tile) {
            switch (tile.bonusType) {
                case BonusType.Diamond:
                    ProfileHelper.Instance.CurrentDiamond += 1;
                    break;
                case BonusType.Life:
                    ProfileHelper.Instance.CurrentLife += 1;
                    break;
            }
        }

        /// <summary>
        /// Try hard, but not too hard, should be safe to call every frame
        /// </summary>
        private void TryIncreaseSpeed () {
            //check to see if we need to increase moving speed or not
            if (!isIncreasingSpeed && currentRunSpeed < maxRunSpeed) {               
                if (currentNumStar < starRecords.Length) {
                    if (numTileConsumed >= starRecords[currentNumStar]) {
                        if (numTileConsumed < starRecords[currentNumStar + 1]) {
                            currentNumStar = Counter.Count(Counter.KeyStar);
                            ShowRecordProgress(currentNumStar);
                            if (currentNumStar >= 3) {
                                //isIncreasingSpeed = true;
                                targetSpeed = baseRunSpeed * speedTableNotes[(currentNumStar - 3) + 1];
                                IncreaseGameSpeed(currentRunSpeed, targetSpeed);
                                //speedDelta = (targetSpeed - currentRunSpeed) / 240f;
                                return;
                            }
                        }
                    }
                }

                if (currentNumStar >= 6) {
                    //start to increase speed only if the player has finished original song
                    //increase speed each 1/3 of the song
                    if ((((numTileConsumed - dataCountUsedInSpeedCalculating) * 3) % dataCountUsedInSpeedCalculating) == 0) {
                        targetSpeed = currentRunSpeed * (1 + speedTableNotes[speedTableNotes.Count - 1]);
                        IncreaseGameSpeed(currentRunSpeed, targetSpeed);
                    }
                }
            }
        }


        //LOL, the player fucked up, we should humiliate him
        public void ProcessGameOverEvent () {

            Helpers.Callback(OnGameOver);
            currentState = GameState.GameOver;
        }

        void OnApplicationPause (bool pauseStatus) {
            if (pauseStatus) {
                PauseGame();
            }
            else {
                ContinueGame();
            }
        }

        public void StopAutoPlay () {
            ChangeStatusToGameOver();
            ProcessGameOverEvent();
        }

        public void PrepareNewGame () {
            ResetGame();

            //spawn 12 rows of tiles
            for (int i = 0; i < 12; i++) {
                GenerateNextTile();
            }

            Timing.RunCoroutine(WaitForLoadingAds());

        }

        /// <summary>
        /// Slow down crazy child, let the ads load, then you can play
        /// </summary>
        /// <returns></returns>
        IEnumerator<float> WaitForLoadingAds () {
            noteStart.SetLoading(true);
            //float elapsedTime = 0;
            yield return Timing.WaitForSeconds(0.5f);
           
                yield return Timing.WaitForSeconds(0.5f);

            noteStart.SetLoading(false);
        }

        public void PauseGame () {
            currentState = GameState.Paused;
        }

        public bool CanContinue () {
            if (CurrentGameStage != GameState.AutoPlay) {
                return numContinue < GameManager.Instance.GameConfigs.maxContinuePerGame;
            }
            else {
                return false;
            }
        }

        public int GetPriceForContinuePlaying () {
            return (int)(Mathf.Pow(GameManager.Instance.GameConfigs.startingPriceToContinue, numContinue + 1));
        }

        /// <summary>
        /// Continue playing the game
        /// </summary>
        /// <param name="afterPausing">Is this continue comes after pausing the game? If not, the game will increase continue counter</param>
        public void ContinueGame (bool afterPausing = true) {
            if (!afterPausing) {
                ++numContinue;
                //Debug.Log("Increasing speed after continue game");
                if (currentRunSpeed > speedAfterContinue) {
                    IncreaseGameSpeed(speedAfterContinue, this.targetSpeed, 5);
                }
            }

            //Continue game after some delay to make sure any UI animation has been completed
            Timing.RunCoroutine(C_RoutineContinueGame());
        }
        private IEnumerator<float> C_RoutineContinueGame () {
            yield return Timing.WaitForSeconds(0.5f);
            currentState = GameState.Continue;
        }

        #region Function for new UI gameplay
        public void StartGame () {
            if (isListenThisSong) {                
                SetAutoPlayUI();
                currentRunSpeed = targetSpeed;
                currentState = GameState.AutoPlay;
            }
            else {
                currentState = GameState.Playing;
                InGameUIController.Instance.gameStarted = true;
            }
            tilesContainer.gameObject.SetActive(false);
        }
        public GameState CurrentGameStage {
            get {
                return currentState;
            }
        }
        public void ChangeStatusToGameOver () {
            currentState = GameState.GameOver;
        }

        #endregion

        public int GetTotalTitles()
        {
            return listTilesData != null ? listTilesData.Count : 0;
        }
    }
}