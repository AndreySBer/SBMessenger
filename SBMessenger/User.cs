using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SBMessenger
{
    public class User
    {
        public string UserID { get; private set; }
        private bool isSet = false;//for EncrAlgorithm
        public encryption_algorithm_type EncrAlgorithm
        {
            get
            {
                if (!isSet)
                {
                    this.EncrAlgorithm = MessengerInterop.GetUserEncryption(UserID);
                    isSet = true;
                }
                return this.EncrAlgorithm;
            }
            private set { }
        }
        public byte[] SecPublicKey
        {
            get
            {
                if (this.SecPublicKey == null)
                {
                    this.SecPublicKey = MessengerInterop.GetPublicKey(UserID);
                }
                return this.SecPublicKey;
            }
            private set { }
        }
        int KeyLength { get; }

        public int unreadMesages { get; set; }
        public bool hasUnreadMesages { get { return unreadMesages > 0; } private set { } }

        public User(string id)
        {
            UserID = id;
            unreadMesages = 0;
        }
    }
}
