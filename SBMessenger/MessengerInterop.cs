using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SBMessenger
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LoginCallback(OperationResult result);
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
    [StructLayout(LayoutKind.Sequential)]
    public struct UserFull
    {
        [MarshalAs(UnmanagedType.LPStr)] string userID;
        encryption_algorithm_type encryptionAlgo;
        [MarshalAs(UnmanagedType.SafeArray)] byte[] SecPublicKey;
        int KeyLength;
    };

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UsersResultCallback(OperationResult result, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)][In, Out] UserFull[] users, int length);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void StatusChanged([MarshalAs(UnmanagedType.LPStr)] string MessageId, MessageStatus status);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl )]
    public delegate void MessageReceived([MarshalAs(UnmanagedType.LPStr)] string UserId,
        [MarshalAs(UnmanagedType.LPStr)]string MessageId,
        /*MessageContent*/
        MessageContentType type,
        bool encrypted,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)][In,Out] byte[] Message, 
        int mesLen);
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


    public static class MessengerInterop
    {
        public class MessageResult
        {
            public delegate void MessageResultHandler();
            public event MessageResultHandler StatusChangedEvent;
            public event MessageResultHandler MessageReceivedEvent;

            public string MessageId { get; private set; }
            public MessageStatus Status { get; private set; }
            public string UserId { get; private set; }
            public byte[] Message { get; private set; }
            public int MessageLength { get; private set; }
            public string ReceivedMessageId { get; private set; }
            public bool ReceivedMessageEncrypted { get; private set; }
            public MessageContentType ReceivedMesType { get; private set; }

            public void StatusChanged(string MessageId, MessageStatus status)
            {
                this.MessageId = MessageId;
                this.Status = status;
                StatusChangedEvent();
            }
            public void MessageReceived(string UserId, string MessageId,
            /*MessageContent*/
            MessageContentType type,
            bool encrypted, byte[] Message, int mesLen)
            {
                this.UserId = UserId;
                this.Message = Message;
                this.MessageLength = mesLen;
                ReceivedMessageId = MessageId;
                ReceivedMessageEncrypted = encrypted;
                ReceivedMesType = type;
                MessageReceivedEvent();
                SendMessageSeen(UserId, ReceivedMessageId);
            }
        }

        public class UsersListHandler
        {
            public void UsersLoaded(OperationResult result, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)][In, Out] UserFull[] users, int length)
            {

            }
        }

        public static void Init()
        {
            Init("127.0.0.1", 5222);
        }

        [DllImport("NativeLinker.dll" , CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init([MarshalAs(UnmanagedType.LPStr)] string url, ushort port);

        [DllImport("NativeLinker.dll" , CallingConvention = CallingConvention.Cdecl)]
        public static extern void Login([MarshalAs(UnmanagedType.LPStr)] string login, [MarshalAs(UnmanagedType.LPStr)] string password, IntPtr loginCallback);

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterObserver(IntPtr statusChanged, IntPtr messageReceived);

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Disconnect();


        [DllImport("NativeLinker.dll" , CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMessage([MarshalAs(UnmanagedType.LPStr)] string recepientId, [MarshalAs(UnmanagedType.LPStr)] string msg, int msg_len);
        [DllImport("NativeLinker.dll" , CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendComplexMessage([MarshalAs(UnmanagedType.LPStr)] string recepientId,
        MessageContentType type,
        bool encrypted,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)][In,Out] byte[] msg,
        int msg_len);

        [DllImport("NativeLinker.dll" , CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMessageSeen([MarshalAs(UnmanagedType.LPStr)] string userId, [MarshalAs(UnmanagedType.LPStr)] string msgId);

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RequestActiveUsers(IntPtr reqUserCallback);

        public static Task<OperationResult> Login(string login, string password)
        {
            var task = new TaskCompletionSource<OperationResult>();
            var loginCallback = Marshal.GetFunctionPointerForDelegate(
            new LoginCallback(task.SetResult));
            Login(login, password, loginCallback);
            return task.Task;
        }
        public static MessageResult res = new MessageResult();
        private static StatusChanged StChDeleg;
        private static MessageReceived MesRecdeleg;
        public static void RegisterObserver()
        {
            StChDeleg = new StatusChanged(res.StatusChanged);
            IntPtr statusCallback = Marshal.GetFunctionPointerForDelegate(StChDeleg);
            MesRecdeleg = new MessageReceived(res.MessageReceived);
            IntPtr messageCallback = Marshal.GetFunctionPointerForDelegate(MesRecdeleg);
            RegisterObserver(statusCallback, messageCallback);
        }
        private static UsersResultCallback usersResultCallback;
        public static UsersListHandler urh = new UsersListHandler();
        public static void RequestActiveUsers()
        {
            usersResultCallback = new UsersResultCallback(urh.UsersLoaded);
            IntPtr usResCb = Marshal.GetFunctionPointerForDelegate(usersResultCallback);
            RequestActiveUsers(usResCb);
        }
    }
}
