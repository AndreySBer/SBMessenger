using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace SBMessenger
{
    /// <summary>
    /// Interaction logic for MainActivity.xaml
    /// </summary>
    public partial class MainActivity : Window
    {
        public MainActivity()
        {
            InitializeComponent();
            this.DataContext = MessengerInterop.UsersMessages;
            this.Show();


            MessengerInterop.MessageResultHandler messageReceivedHandler = delegate ()
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    string user = MessengerInterop.mRres.UserId;
                    MessengerInterop.Users[user].unreadMesages += 1;
                    ICollectionView view = CollectionViewSource.GetDefaultView(MessengerInterop.Users.Values);
                    view.Refresh();
                });
            };

            MessengerInterop.MessageResultHandler changedStatusHandler = delegate ()
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    ICollectionView view = CollectionViewSource.GetDefaultView(MessengerInterop.UsersMessages[CurrentUser]);
                    view.Refresh();
                });
            };

            MessengerInterop.MessageResultHandler usersRequestHandler = delegate ()
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    UsersList.ItemsSource = MessengerInterop.Users.Values;
                    usersCounter.Text = MessengerInterop.Users.Count + " пользователей онлайн";
                    CurrentUser = MessengerInterop.UserName;
                });
            };
            MessengerInterop.mRres.MessageReceivedEvent += messageReceivedHandler;
            MessengerInterop.stCres.StatusChangedEvent += changedStatusHandler;
            MessengerInterop.urh.UsersChangedEvent += usersRequestHandler;
            showDialog();



        }
        void showDialog()
        {
            LoginWindow aboutWindow = new LoginWindow();
            aboutWindow.Owner = this;
            aboutWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            aboutWindow.ShowDialog();
        }
        string CurrentUser = "";
        //Message CurrentMessage;

        private void UsersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                CurrentUser = ((User)e.AddedItems[0]).UserID;
            }
            UserName.Content = CurrentUser;
            for (int i = 0; i < MessengerInterop.Users[CurrentUser].unreadMesages; i++)
            {
                int count = MessengerInterop.UsersMessages[CurrentUser].Count;
                string ReceivedMessageId = MessengerInterop.UsersMessages[CurrentUser][count - i - 1].MessageId;
                MessengerInterop.SendMessageSeen(CurrentUser, ReceivedMessageId);
            }
            MessengerInterop.Users[CurrentUser].unreadMesages = 0;
            ICollectionView view = CollectionViewSource.GetDefaultView(MessengerInterop.Users.Values);
            view.Refresh();
            MessagesLV.ItemsSource = MessengerInterop.UsersMessages[CurrentUser];
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string msg = MessageText.Text;
            if (msg.Length > 0)
            {
                byte[] mesg = Encoding.UTF8.GetBytes(msg + '\0');
                MessengerInterop.CurrentMessage = new Message(MessengerInterop.UserName, msg, DateTime.Now);
                MessengerInterop.SendComplexMessage(CurrentUser, MessageContentType.Text, false, mesg, mesg.Length);
                
                MessengerInterop.UsersMessages[CurrentUser].Add(MessengerInterop.CurrentMessage);
                ICollectionView view = CollectionViewSource.GetDefaultView(MessengerInterop.UsersMessages[CurrentUser]);
                view.Refresh();
            }
            else
            {
                ErrorToaster.Toast(message: "Нельзя отправлять пустые сообщения", animation: netoaster.ToasterAnimation.FadeIn);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            MessengerInterop.Disconnect();
            base.OnClosed(e);
        }

        private void ResreshButton_Click(object sender, RoutedEventArgs e)
        {
            MessengerInterop.RequestActiveUsers();
        }
    }
}
