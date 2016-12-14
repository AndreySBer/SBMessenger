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
                
                UsersMessages = SQLiteConnector.GetSavedMessages(MessengerInterop.UserName);
                Users = new Dictionary<string, User>();
                if (UsersMessages == null)
                {
                    UsersMessages = new Dictionary<string, List<Message>>();
                }
                foreach (string i in UsersMessages.Keys)
                {
                    Users.Add(i, new SBMessenger.User(i));
                }
                foreach (string i in users)
                {
                    if (!UsersMessages.ContainsKey(i))
                    {
                        User temp = new SBMessenger.User(i);
                        Users.Add(temp.UserID, temp);
                        UsersMessages.Add(temp.UserID, new List<Message>());
                    }
                }
                UsersChangedEvent?.Invoke();
                
            }
        }

        public static Dictionary<string, List<Message>> UsersMessages { get; set; }
        public static Dictionary<string,User> Users { get; set; }
        

        public static Message findMessageById(string id)
        {
            foreach (var i in UsersMessages)
            {
                foreach(var j in i.Value)
                {
                    if (j.MessageId == id)
                        return j;
                }
            }
            return null;
        }
    }
}
