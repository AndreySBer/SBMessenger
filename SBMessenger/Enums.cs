namespace SBMessenger
{
    public enum OperationResult : int
    {
        Ok,
        AuthError,
        NetworkError,
        InternalError
    }
    public enum encryption_algorithm_type
    {
        None,
        RSA_1024
    }
    public enum MessageStatus
    {
        Sending,
        Sent,
        FailedToSend,
        Delivered,
        Seen
    };

    public enum MessageContentType
    {
        Text,
        Image,
        Video
    };
}
