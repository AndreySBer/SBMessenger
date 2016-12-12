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
                StatusChangedEvent();
            }
        }
    }
}
