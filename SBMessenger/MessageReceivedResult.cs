using System;
using System.Collections.Generic;

namespace SBMessenger
{
    public static partial class MessengerInterop
    {
        
        public class MessageReceivedResult
        {
            public event MessageResultHandler MessageReceivedEvent;

            public string UserId { get; private set; }
            //public byte[] Message { get; private set; }
            //public int MessageLength { get; private set; }
            //public string ReceivedMessageId { get; private set; }
            //public bool ReceivedMessageEncrypted { get; private set; }
            //public MessageContentType ReceivedMesType { get; private set; }
            //public DateTime Time { get; private set; }

            
            public void MessageReceived(string UserId,
                string MessageId,
                long time,
            /*MessageContent*/
            MessageContentType type,
            bool encrypted,
            byte[] Message,
            int mesLen)
            {
                DateTime Time = DateTimeConversion.UnixTimeToDateTime(time);
                //костыль   
                if (!UsersMessages.ContainsKey(UserId))
                {
                    UsersMessages.Add(UserId, new List<Message>());
                    Users.Add(UserId, new User(UserId));
                }
                UsersMessages[UserId].Add(new Message(UserId, Time, type, encrypted, Message));
                this.UserId = UserId;
                MessageReceivedEvent?.Invoke();
                //SendMessageSeen(UserId, ReceivedMessageId);
            }
        }
    }
}
