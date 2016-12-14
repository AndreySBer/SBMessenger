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
                if (!UsersMessages.ContainsKey(UserId))
                {
                    UsersMessages.Add(UserId, new List<Message>());
                    Users.Add(UserId, new User(UserId));
                }
                if (UserId != UserName)
                {

                    UsersMessages[UserId].Add(new Message(MessageId, UserId, Time, type, encrypted, Message) { State = Time.ToShortTimeString() });

                }
                this.UserId = UserId;
                MessageReceivedEvent?.Invoke();

            }
        }
    }
}
