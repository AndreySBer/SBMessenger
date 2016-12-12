using System;

namespace SBMessenger
{
    public static partial class MessengerInterop
    {
        
        public class MessageReceivedResult
        {
            public event MessageResultHandler MessageReceivedEvent;

            public string UserId { get; private set; }
            public byte[] Message { get; private set; }
            public int MessageLength { get; private set; }
            public string ReceivedMessageId { get; private set; }
            public bool ReceivedMessageEncrypted { get; private set; }
            public MessageContentType ReceivedMesType { get; private set; }
            public DateTime Time { get; private set; }

            
            public void MessageReceived(string UserId,
                string MessageId,
                long time,
            /*MessageContent*/
            MessageContentType type,
            bool encrypted,
            byte[] Message,
            int mesLen)
            {
                this.UserId = UserId;
                this.Message = Message;
                this.MessageLength = mesLen;
                ReceivedMessageId = MessageId;
                ReceivedMessageEncrypted = encrypted;
                ReceivedMesType = type;
                Time = DateTimeConversion.UnixTimeToDateTime(time);

                MessageReceivedEvent();
                SendMessageSeen(UserId, ReceivedMessageId);
            }
        }
    }
}
