using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SBMessenger
{
    //Delegates-callbacks
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LoginCallback(OperationResult result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UsersResultCallback([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)][In, Out] string[] users, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void StatusChanged([MarshalAs(UnmanagedType.LPStr)] string MessageId, MessageStatus status);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void MessageReceived([MarshalAs(UnmanagedType.LPStr)] string UserId,
        [MarshalAs(UnmanagedType.LPStr)]string MessageId,
        long time,
        /*MessageContent*/
        MessageContentType type,
        bool encrypted,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)][In, Out] byte[] Message,
        int mesLen);



    public static partial class MessengerInterop
    {
        public delegate void MessageResultHandler();
        public static void Init()
        {
            Init("127.0.0.1", 5222);
        }

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init([MarshalAs(UnmanagedType.LPStr)] string url, ushort port);

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Login([MarshalAs(UnmanagedType.LPStr)] string login, [MarshalAs(UnmanagedType.LPStr)] string password, IntPtr loginCallback);

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterObserver(IntPtr statusChanged, IntPtr messageReceived);

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Disconnect();


        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMessage([MarshalAs(UnmanagedType.LPStr)] string recepientId, [MarshalAs(UnmanagedType.LPStr)] string msg, int msg_len);
        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendComplexMessage([MarshalAs(UnmanagedType.LPStr)] string recepientId,
        MessageContentType type,
        bool encrypted,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)][In, Out] byte[] msg,
        int msg_len);

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMessageSeen([MarshalAs(UnmanagedType.LPStr)] string userId, [MarshalAs(UnmanagedType.LPStr)] string msgId);

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RequestActiveUsers(IntPtr reqUserCallback);

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern encryption_algorithm_type GetUserEncryption([MarshalAs(UnmanagedType.LPStr)]string userId);

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte[] GetPublicKey(string userId);

        public static Task<OperationResult> Login(string login, string password)
        {
            var task = new TaskCompletionSource<OperationResult>();
            var loginCallback = Marshal.GetFunctionPointerForDelegate(
            new LoginCallback(task.SetResult));
            Login(login, password, loginCallback);
            UserName = login;
            return task.Task;
        }
        public static MessageReceivedResult mRres = new MessageReceivedResult();
        public static StatusChangedResult stCres = new StatusChangedResult();
        private static StatusChanged StChDeleg;
        private static MessageReceived MesRecdeleg;
        public static void RegisterObserver()
        {
            StChDeleg = new StatusChanged(stCres.StatusChanged);
            IntPtr statusCallback = Marshal.GetFunctionPointerForDelegate(StChDeleg);
            MesRecdeleg = new MessageReceived(mRres.MessageReceived);
            IntPtr messageCallback = Marshal.GetFunctionPointerForDelegate(MesRecdeleg);
            RegisterObserver(statusCallback, messageCallback);
        }
        private static UsersResultCallback usersResultCallback;
        public static UsersListResult urh = new UsersListResult();
        public static void RequestActiveUsers()
        {
            usersResultCallback = new UsersResultCallback(urh.UsersLoaded);
            IntPtr usResCb = Marshal.GetFunctionPointerForDelegate(usersResultCallback);
            RequestActiveUsers(usResCb);
        }
        public static string UserName { get; private set; }
    }
}
