using UnityEngine;
using MovementEffects;
using System.Collections.Generic;
using Mio.TileMaster;
using System;
using Mio.Utils.MessageBus;

namespace Mio.Utils {
    public class LivesManager : MonoSingleton<LivesManager> {

        //private Message msgLifeChanges = new Message(MessageBusType.LifeChanged);
        //private MessageBus.Message msgUserDataChanges = new MessageBus.Message(MessageBusType.UserDataChanged);

        private MessageBoxDataModel msgbxRechargeLives;
        private Timing timingInstance;
        //private static Timing
        private bool isCountingDown = false;
        private int secondsRemaining = 0;
        //private float timeStartCountDown = 0;
        private Message timeCountDownMessage = new Message(MessageBusType.TimeLifeCountDown);
        //public const string KEY_LIFE_TIMER = "life_timer";

        //Dictionary<string, Action>
        //[SerializeField]
        //private int currentLife;
        //private int oldLife;
        //public int CurrentLife {
        //    get { return currentLife; }
        //    private set {
        //        //oldLife = currentLife;
        //        currentLife = value;

        //    }
        //}       

        public int CurrentLife {
            get { return ProfileHelper.Instance.CurrentLife; }
        }

        public int MaxLife {
            get { return ProfileHelper.Instance.MaxLife; }
        }

        public void Initialize() {
            //msgbxRechargeLives = new MessageBoxDataModel();
            //msgbxRechargeLives.message = "Look like you're out of lives. Do you want to receive 20 more lives for free?";
            //msgbxRechargeLives.messageNo = "No";
            //msgbxRechargeLives.messageYes = "Free lives!!!";
            //msgbxRechargeLives.OnNoButtonClicked = OnUserRefuseToRechargeLife;
            //msgbxRechargeLives.OnYesButtonClicked = OnUserChoseToRechargeLife;

            //if (CurrentLife <= 0) {
            //    SetLife(MaxLife);
            //}

            //initialize time counter
            timingInstance = gameObject.AddComponent<Timing>();
            timingInstance.TimeBetweenSlowUpdateCalls = 1f;

            SetupCountdown();

            //register with message bus
            MessageBus.MessageBus.Instance.Subscribe(MessageBusType.LifeChanged, OnUserLifeChanged);
        }


        private void SetupCountdown() {
            isCountingDown = false;
            secondsRemaining = 0;

            //default value to support user who has just updated to this version from older one
            if(GameManager.Instance.GameConfigs.secondsPerLife <= 0) {
                GameManager.Instance.GameConfigs.secondsPerLife = 300;
            }

            //get last count down timestamp from user data
            double lastCountdownUnix = ProfileHelper.Instance.LastLifeCountdownUnixTimeStamp;
            DateTime dtLastCountdown = TimeHelper.UnixTimeStampToDateTime(lastCountdownUnix);
            TimeSpan diff = DateTime.UtcNow - dtLastCountdown;
            double secondsElapsed = diff.TotalSeconds;

            //print(string.Format("Seconds elapsed from last count down to increase lives: {0}", secondsElapsed));
            //check how many time have passed?
            if(secondsElapsed <= 0) {
                //maybe our users has cheat here. Do nothing about it ;)
                secondsElapsed = 0;
            }

            //calculate and add generated life
            if(secondsElapsed > 0) {
                int lifeAdded = Mathf.FloorToInt((float)secondsElapsed / GameManager.Instance.GameConfigs.secondsPerLife);
                if(lifeAdded > 0) {
                    if (lifeAdded + ProfileHelper.Instance.CurrentLife <= ProfileHelper.Instance.MaxLife) {
                        ProfileHelper.Instance.CurrentLife += (lifeAdded);
                    }else {
                        if(ProfileHelper.Instance.CurrentLife < ProfileHelper.Instance.MaxLife) {
                            ProfileHelper.Instance.CurrentLife = ProfileHelper.Instance.MaxLife;
                        }
                    }
                    secondsElapsed -= (lifeAdded * GameManager.Instance.GameConfigs.secondsPerLife);
                }
            }

            //check if we need to generate more life or not
            if(CurrentLife < MaxLife) {
                StartCountDown(GameManager.Instance.GameConfigs.secondsPerLife - (int)secondsElapsed);
            }
        }

        private void OnUserLifeChanged(Message msg) {
            if(CurrentLife < MaxLife) {
                if (!isCountingDown) {
                    StartCountDown(GameManager.Instance.GameConfigs.secondsPerLife);
                }
            }
        }

        private void StartCountDown(int totalSeconds) {
            //timingInstance.
            isCountingDown = true;
            secondsRemaining = totalSeconds;
            
            //only update user data if we start counting down full duration. Why? Because that way, we can reduce read/write to storage, and always make sure that we only need to calculate remaining times with full countdown duration
            if (totalSeconds == GameManager.Instance.GameConfigs.secondsPerLife) {
                ProfileHelper.Instance.LastLifeCountdownUnixTimeStamp = TimeHelper.DateTimeToUnixTimeStamp(DateTime.UtcNow.AddSeconds(GameManager.Instance.GameConfigs.secondsPerLife - totalSeconds));
                //MessageBus.MessageBus.Annouce(msgUserDataChanges);
            }

            timingInstance.RunCoroutineOnInstance(CountingDown(), Segment.SlowUpdate);
        }

        private IEnumerator<float> CountingDown() {
            while (isCountingDown) {
                //print("Current time: " + Time.realtimeSinceStartup);               
                --secondsRemaining;
                timeCountDownMessage.data = secondsRemaining;               

                if(secondsRemaining <= 0) {
                    isCountingDown = false;
                    secondsRemaining = 0;
                    IncreaseLife(1);
                }
                MessageBus.MessageBus.Annouce(timeCountDownMessage);
                yield return 0f;
            }
        }


        private void OnUserRefuseToRechargeLife() {
            //SceneManager.Instance.CloseScene();
        }

        private void OnUserChoseToRechargeLife() {
            LivesManager.Instance.IncreaseLife(20);
            //SceneManager.Instance.CloseScene();
        }

        /// <summary>
        /// Short hand for ChangeLife(-1)
        /// </summary>
        public bool ReduceLife() {
            return IncreaseLife(-1);
        }

        /// <summary>
        /// Short hand for ChangeLife(1)
        /// </summary>
        public bool AddLife() {
            return IncreaseLife(1);
        }

        /// <summary>
        /// Add more life to current value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IncreaseLife(int value, bool limitbreak = false) {
            //if (ProfileHelper.Instance.CurrentLife + value >= 0) {
            //    if (limitbreak || ProfileHelper.Instance.CurrentLife + value <= MaxLife) {
            //        ProfileHelper.Instance.CurrentLife += value;
            //    }

            //    return true;
            //}
            ProfileHelper.Instance.CurrentLife += value;
            return true;
        }

        //public bool isTest = false;
        //void Update() {
        //    if (isTest) {
        //        isTest = false;
        //        StartCountDown(10);
        //    }
        //}

        

        public bool CanPlaySong() {
            return (ProfileHelper.Instance.CurrentLife >= 1);
        }

        public void SetLife(int value) {
            ProfileHelper.Instance.CurrentLife = value;
        }

    }
}