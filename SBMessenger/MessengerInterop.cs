using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SBMessenger
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LoginCallback(OperationResult result);
    //public delegate void ReqUsersCallback(OperationResult result, )
    public enum OperationResult : int
    {
        Ok,
        AuthError,
        NetworkError,
        InternalError
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void StatusChanged([MarshalAs(UnmanagedType.LPStr)] string MessageId, MessageStatus status);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void MessageReceived([MarshalAs(UnmanagedType.LPStr)] string UserId, [MarshalAs(UnmanagedType.LPStr)] string Message);
    public enum MessageStatus
    {
        Sending,
        Sent,
        FailedToSend,
        Delivered,
        Seen
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
        public string Message { get; private set; }

        public void StatusChanged(string MessageId, MessageStatus status)
        {
            this.MessageId = MessageId;
            this.Status = status;
            StatusChangedEvent();
        }
        public void MessageReceived(string UserId, string Message) {
            this.UserId = UserId;
            this.Message = Message;
            MessageReceivedEvent();
            //MessengerInterop.SendMessageSeen(UserId, Message.MessageID);
        }
    }

        /*[DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init();*/

        public static void Init()
        {
            Init("127.0.0.1", 5222);
        }

        [DllImport("NativeLinker.dll",
        CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(
        [MarshalAs(UnmanagedType.LPStr)] string url, ushort port);

        /*[DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Login();*/

        [DllImport("NativeLinker.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Login([MarshalAs(UnmanagedType.LPStr)] string login, [MarshalAs(UnmanagedType.LPStr)] string password, IntPtr loginCallback);

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterObserver(IntPtr statusChanged, IntPtr messageReceived);

        [DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Disconnect();

        /*[DllImport("NativeLinker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RequestActiveUsers(IntPtr reqUserCallback);*/

        /*[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]

        public struct Message
        {
            [MarshalAs(UnmanagedType.LPStr)]
            string identifier;
            long time;
            MessageContent content;
        };
        public enum MessageContentType : int
        {
            Text,
            Image,
            Video
        }

        public struct MessageContent
        {
            MessageContentType type;
            bool encrypted;

            IntPtr data;
            int data_length;
        };

        [DllImport("NativeLinker.dll",
    CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Message SendMessage([MarshalAs(UnmanagedType.LPStr)] string recepientId, ref MessageContent msgData);*/

        [DllImport("NativeLinker.dll",CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMessage([MarshalAs(UnmanagedType.LPStr)] string recepientId, [MarshalAs(UnmanagedType.LPStr)] string msg, int msg_len);
        [DllImport("NativeLinker.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMessageSeen([MarshalAs(UnmanagedType.LPStr)] string userId, [MarshalAs(UnmanagedType.LPStr)] string msgId);

        public static Task<OperationResult> Login(string login, string password)
        {
            var task = new TaskCompletionSource<OperationResult>();
            var loginCallback = Marshal.GetFunctionPointerForDelegate(
            new LoginCallback(task.SetResult));
            MessengerInterop.Login(login, password,loginCallback);
            return task.Task;
        }
        public static MessageResult res = new MessageResult();
        public static Task<MessageResult> RegisterObserver()
        {
            var task = new TaskCompletionSource<MessageResult>();
            //MessageResult res = new MessageResult();
            var statusCallback = Marshal.GetFunctionPointerForDelegate(new StatusChanged(res.StatusChanged));
            var messageCallback= Marshal.GetFunctionPointerForDelegate(new MessageReceived(res.MessageReceived));
            MessengerInterop.RegisterObserver(statusCallback, messageCallback);
            task.SetResult(res);
            return task.Task;
        }
    }
}
