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
        //List<Messenge> items;
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
                    //string mes = Encoding.UTF8.GetString(MessengerInterop.mRres.Message);
                    //mes = mes.Remove(mes.Length - 1);

                    //SuccessToaster.Toast(message: mes, animation: netoaster.ToasterAnimation.FadeIn);
                });
            };

            MessengerInterop.MessageResultHandler changedStatusHandler = delegate ()
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    //string mes = "Message is " + Enum.GetName(typeof(MessageStatus), MessengerInterop.stCres.Status);
                    //ErrorToaster.Toast(message: mes, animation: netoaster.ToasterAnimation.FadeIn);

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

        private void UsersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentUser = ((User)e.AddedItems[0]).UserID;
            UserName.Content = CurrentUser;
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
                MessengerInterop.SendComplexMessage(CurrentUser, MessageContentType.Text, false, mesg, mesg.Length);
                MessengerInterop.UsersMessages[CurrentUser].Add(new Message(MessengerInterop.UserName, msg, DateTime.Now));
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResreshButton_Click(object sender, RoutedEventArgs e)
        {
            MessengerInterop.RequestActiveUsers();
        }
    }
}
