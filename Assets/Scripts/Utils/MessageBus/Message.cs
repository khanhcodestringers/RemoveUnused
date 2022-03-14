using UnityEngine;

namespace Mio.Utils.MessageBus {
    public class Message {
        public MessageBusType messageType;
        //public GameObject sender;
        public object data;
        public Message() { }

        public Message(MessageBusType type) {
            messageType = type;
        }

        public Message(MessageBusType type, object data) {
            messageType = type;
            this.data = data;
        }
    }

    //public class GenericMessage<T> : Message {
    //    public T data;

    //    public GenericMessage(MessageBusType type, T data){
    //        this.messageType = type;
    //        this.data = data;
    //    }
    //}
}