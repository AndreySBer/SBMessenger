using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SBMessenger
{
    public static partial class MessengerInterop
    {
        public class UsersListResult
        {
            public event MessageResultHandler UsersChangedEvent;

            public void UsersLoaded(string[] users, int length)
            {
                UsersMessenges = new Dictionary<string, List<Message>>();
                Users = new Dictionary<string, User>();
                foreach (string i in users)
                {
                    User temp = new SBMessenger.User(i);
                    UsersMessenges.Add(temp.UserID, new List<Message>());
                    Users.Add(temp.UserID, temp);
                }
                UsersChangedEvent?.Invoke();
            }
        }

        public static Dictionary<string, List<Message>> UsersMessenges { get; set; }
        public static Dictionary<string,User> Users { get; set; }
    }
}
