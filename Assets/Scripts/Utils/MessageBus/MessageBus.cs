using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MovementEffects;

namespace Mio.Utils.MessageBus {
    public class MessageBus : MonoSingleton<MessageBus> {
        private Dictionary<MessageBusType, List<Action<Message>>> listeners = new Dictionary<MessageBusType, List<Action<Message>>>(50);
        private Timing timer;
        private Queue<Message> normalMSGs;
        private float lastBreakTime = 0;
        private float MAX_FRAME_TIME = 1f/60f;

        public void Initialize () {
            timer = gameObject.AddComponent<Timing>();
            normalMSGs = new Queue<Message>(25);
            timer.RunCoroutineOnInstance(ProcessMessage(), Segment.Update);
        }

        IEnumerator<float> ProcessMessage () {
            while (true) {
                yield return 0f;
                lastBreakTime = Time.realtimeSinceStartup;
                while (normalMSGs.Count > 0) {
                    if (Time.realtimeSinceStartup - lastBreakTime >= MAX_FRAME_TIME) break;

                    //print("Dispatching message " + normalMSGs.Peek().messageType);
                    DispatchMessage(normalMSGs.Dequeue());
                }
            }
        }

        public void Subscribe(MessageBusType type, Action<Message> handler) {
            if (listeners.ContainsKey(type)) {
                if(listeners[type] == null) {
                    listeners[type] = new List<Action<Message>>(20);
                }
            }else {
                listeners.Add(type, new List<Action<Message>>(20));
            }

            //print("Added handler for message " + type);
            listeners[type].Add(handler);
        }

        public void Unsubscribe(MessageBusType type, Action<Message> handler) {
            if (listeners.ContainsKey(type)) {
                if (listeners[type] != null) {
                    listeners[type].Remove(handler);
                }
            }
        }

        /// <summary>
        /// Send a message to all subscribers listening to this certain message type
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(Message message, bool immidiately = false) {
            if (immidiately) {
                DispatchMessage(message);
            }else {
                normalMSGs.Enqueue(message);
            }
            ////print("Sending message " + message.messageType);
            //if (listeners.ContainsKey(message.messageType)) {
            //    var listHandlers = listeners[message.messageType];
            //    //print("--List handler contains " + listHandlers.Count);
            //    for (int i = 0; i < listHandlers.Count; i++) {
            //        if(listHandlers[i] != null) {
            //            //print("--Message sent");
            //            listHandlers[i](message);
            //        }
            //    }
            //}
        }

        private void DispatchMessage(Message message) {
            if (listeners.ContainsKey(message.messageType)) {
                var listHandlers = listeners[message.messageType];
                //print("--List handler contains " + listHandlers.Count);
                for (int i = 0; i < listHandlers.Count; i++) {
                    //print("--Message sent");
                    if (listHandlers[i] != null) {
                        listHandlers[i](message);
                    }
                }
            }
        }


        /// <summary>
        /// Push a message directly into processing. Note that this call is synchronous and is not in framerate control
        /// </summary>
        /// <param name="message"></param>
        public static void AnnouceHighPriorityMessage (Message message) {
            Instance.DispatchMessage(message);
        }

        /// <summary>
        /// Short hand for calling send message with normal priority (will be delay if the target framerate is not reached)
        /// </summary>
        public static void Annouce(Message message) {
            Instance.SendMessage(message);
        }
        //Dictionary<MessageBusType, List<MessageSubscriber>> subscriberLists =
        //new Dictionary<MessageBusType, List<MessageSubscriber>>();

        //public void AddSubscriber(MessageSubscriber subscriber) {
        //    MessageBusType[] messageTypes = subscriber.MessageTypes;
        //    for (int i = 0; i < messageTypes.Length; i++)
        //        AddSubscriberToMessage(messageTypes[i], subscriber);
        //}

        //void AddSubscriberToMessage(MessageBusType messageType,
        //                             MessageSubscriber subscriber) {
        //    if (!subscriberLists.ContainsKey(messageType))
        //        subscriberLists[messageType] =
        //            new List<MessageSubscriber>();

        //    subscriberLists[messageType].Add(subscriber);
        //}

        //public void SendMessage(Message message) {
        //    if (!subscriberLists.ContainsKey(message.messageType))
        //        return;

        //    List<MessageSubscriber> subscriberList =
        //        subscriberLists[message.messageType];

        //    for (int i = 0; i < subscriberList.Count; i++)
        //        SendMessageToSubscriber(message, subscriberList[i]);
        //}

        //void SendMessageToSubscriber(Message message,
        //                             MessageSubscriber subscriber) {
        //    //subscriber.Handler.HandleMessage(message);
        //}

        ///* Singleton */
        //static MessageBus instance;

        //public static MessageBus Instance {
        //    get {
        //        if (instance == null)
        //            instance = new MessageBus();

        //        return instance;
        //    }
        //}

        //private MessageBus() { }
    }
}