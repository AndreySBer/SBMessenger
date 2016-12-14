
using System;
using System.Text;

namespace SBMessenger
{
    public class Message
    {

        public string MessageId { get; set; }
        public string UserName { get; private set; }
        public DateTime time { get; private set; }
        public string State { get; set; }
        public string Text { get; private set; }
        public MessageContentType Type { get; private set; }
        public bool Encrypted { get; private set; }
        public byte[] Data { get; private set; }
        public Message() { }

        public Message(string messageId, string userId, DateTime time, MessageContentType type, bool encrypted, byte[] message)
        {
            MessageId = messageId;
            UserName = userId;
            this.time = time;
            Type = type;
            Encrypted = encrypted;
            Data = message;
            Text = (Type == MessageContentType.Text && !Encrypted) ?
                Encoding.UTF8.GetString(message) :
                Type.ToString() + (Encrypted ? "Encrypted" : "Not encrypted");
            if (Type == MessageContentType.Text && !Encrypted && Text.Length>0&& Text.EndsWith("\0"))
                Text = Text.Remove(Text.Length - 1);
        }

        //temporary constructor for add messages when sending
        public Message(string userId, string text, DateTime time)
        {
            UserName = userId;
            this.time = time;
            Type = MessageContentType.Text;
            Encrypted = false;
            Data = Encoding.UTF8.GetBytes(text + '\0');
            Text = text;
            State = "Отправка";
        }
    }
}
