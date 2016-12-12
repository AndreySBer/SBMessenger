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
                Users = new List<User>();
                foreach (string i in users)
                {
                    Users.Add(new SBMessenger.User(i));
                }
                UsersChangedEvent?.Invoke();
            }
        }
        public static List<User> Users
        {
            get;
            set;
        }
    }
}
