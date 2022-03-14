using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Mio.Utils {
    //attempt to replace action with callback, then remove system.dll
    public delegate void Callback();

    public class TimeCounter : MonoSingleton<TimeCounter> {
        private bool isInitialized = false;

        private List<int> listDuration;
        //private Dictionary<int, List<Action>> dictCallbacks;
        //private Dictionary<int, int> dictTimeElapsed;
        private Dictionary<int, List<TimeCounterItem>> subscribers;

        private float startTime;
        private float elapsedTime;
        //private float lastUpdateTime;
        private int elapsedSeconds, elapsedMinutes, elapsedHour;


        public void Initialize() {
            if (isInitialized) return;

            startTime = Time.realtimeSinceStartup;
            elapsedTime = 0;
            listDuration = new List<int>(10);
            subscribers = new Dictionary<int, List<TimeCounterItem>>(10);

            isInitialized = true;
        }

        void Update() {
            elapsedTime = Time.realtimeSinceStartup - startTime;
            //no need to calculate elapsed time in every frame ;v
            if (elapsedTime - elapsedSeconds < 1) {
                return;
            }
            //lastUpdateTime = elapsedTime;

            //calculate clock time
            elapsedSeconds = Mathf.RoundToInt(elapsedTime);
            //elapsedMinutes = elapsedSeconds / 60;
            //elapsedHour = elapsedSeconds / 3600;

            //called each seconds
            if (listDuration.Count > 0) {
                for (int i = 0; i < listDuration.Count; i++) {
                    UpdateSubscriber(listDuration[i]);                    
                }
            }
        }

        /// <summary>
        /// Increase time counter and callback if needed for subscriber at specified duration
        /// </summary>
        private void UpdateSubscriber(int duration) {
            if (subscribers.ContainsKey(duration)) {
                TimeCounterItem item;
                for (int i = 0; i < subscribers[duration].Count; i++) {
                    item = subscribers[duration][i];
                    if (++item.elapsedTime >= duration) {
                        DoCallbacks(item.listLoopCallbacks);
                        DoCallbacks(item.listOneTimeCallbacks);
                        item.listOneTimeCallbacks.Clear();
                        item.elapsedTime = 0;
                        if(item.listLoopCallbacks.Count <= 0) {
                            subscribers[duration].RemoveAt(i--);
                        }
                    }
                }

                if(subscribers[duration].Count <= 0) {
                    subscribers.Remove(duration);
                }
            }
        }

        /// <summary>
        /// Register a method, so that it will be run each time the counter lasted for specified duration
        /// </summary>
        /// <param name="callback">The method to call, can't be null</param>
        /// <param name="duration">How long before the callback is call</param>
        /// <param name="loop">Default is true, if false, the callback will be removed after 1 run</param>
        public void RegisterForDuration(Callback callback, int duration, bool loop = true) {
            if (!isInitialized) { Initialize(); }

            if (callback == null) {
                Debug.LogWarning("Trying to add null as callback for TimeCounter, will ignore this call");
                return;
            }

            TimeCounterItem item = null;
            //if the system has contain list of callback at this duration
            if (subscribers.ContainsKey(duration)) {
                //try to find a counter that hasn't start counting yet. Why?
                //Because if it's counting, the duration will became not accuracy anymore
                for(int i = 0; i < subscribers[duration].Count; i++) {
                    if(subscribers[duration][i].elapsedTime == 0) {
                        item = subscribers[duration][i];
                    }
                }

                //if there is non to be found, create new one
                if(item == null) {
                    item = new TimeCounterItem();
                    subscribers[duration].Add(item);
                }
            }
            else {
                var list = new List<TimeCounterItem>(10);
                item = new TimeCounterItem();
                list.Add(item);
                subscribers.Add(duration, list);
            }

            if (loop) {
                item.listLoopCallbacks.Add(callback);
            }
            else {
                item.listOneTimeCallbacks.Add(callback);
            }
        }

        public void UnregisterForDuration(Callback callback, int duration, bool isLoop = true) {
            if (callback == null) {
                Debug.LogWarning("Trying to remove null as callback for TimeCounter, will ignore this call");
                return;
            }

            if (subscribers.ContainsKey(duration)) {
                for (int j = 0; j < subscribers[duration].Count; j++) {
                    TimeCounterItem item = subscribers[duration][j];
                    List<Callback> list = isLoop ? item.listLoopCallbacks : item.listOneTimeCallbacks;
                    for (int i = 0; i < list.Count; i++) {
                        if (list[i] == callback) {
                            list.RemoveAt(i--);
                        }
                    }
                }
            }
        }

        private void DoCallbacks(List<Callback> callbacks) {
            for (int i = 0; i < callbacks.Count; i++) {
                if (callbacks[i] != null) {
                    callbacks[i]();
                }
            }
        }

        private class TimeCounterItem {
            public int elapsedTime;
            public List<Callback> listLoopCallbacks;
            public List<Callback> listOneTimeCallbacks;

            public TimeCounterItem() {
                elapsedTime = 0;
                listLoopCallbacks = new List<Callback>(5);
                listOneTimeCallbacks = new List<Callback>(5);
            }
        }
    }
}