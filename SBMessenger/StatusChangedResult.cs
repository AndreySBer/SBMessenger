namespace SBMessenger
{
    public static partial class MessengerInterop
    {
        public class StatusChangedResult
        {
            public event MessageResultHandler StatusChangedEvent;

            public string MessageId { get; private set; }
            public MessageStatus Status { get; private set; }

            public void StatusChanged(string MessageId, MessageStatus status)
            {
                this.MessageId = MessageId;
                this.Status = status;



                if (Status == MessageStatus.Sending)
                {
                    CurrentMessage.MessageId = MessageId;
                }
                else
                {
                    Message message = MessengerInterop.findMessageById(MessageId);

                    if (message != null)
                    {
                        switch (Status)
                        {
                            case MessageStatus.Delivered: message.State = "Доставлено"; break;
                            case MessageStatus.Seen: message.State = message.time.ToShortTimeString(); break;
                            case MessageStatus.FailedToSend: message.State = "Не отправлено"; break;
                        }
                        SQLiteConnector.EditMessageStatus(MessageId, message.State);
                    }
                }
                StatusChangedEvent();
            }

        }
        public static Message CurrentMessage;
    }
}
