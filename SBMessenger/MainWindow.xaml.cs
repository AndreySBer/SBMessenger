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
            this.DataContext = MessengerInterop.UsersMessenges;
            this.Show();


            MessengerInterop.MessageResultHandler messageReceivedHandler = delegate ()
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
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
                    string mes = "Message is " + Enum.GetName(typeof(MessageStatus), MessengerInterop.stCres.Status);
                    ErrorToaster.Toast(message: mes, animation: netoaster.ToasterAnimation.FadeIn);

                });
            };

            MessengerInterop.MessageResultHandler usersRequestHandler = delegate ()
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    UsersList.ItemsSource = MessengerInterop.UsersMessenges.Keys;
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
            CurrentUser = (string)e.AddedItems[0];
            UserName.Content = CurrentUser;
            MessagesLV.ItemsSource = MessengerInterop.UsersMessenges[CurrentUser];
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string msg = MessageText.Text;
            if (msg.Length > 0)
            {
                byte[] mesg = Encoding.UTF8.GetBytes(msg + '\0');
                MessengerInterop.SendComplexMessage(CurrentUser, MessageContentType.Text, false, mesg, mesg.Length);
                MessengerInterop.UsersMessenges[CurrentUser].Add(new Message(MessengerInterop.UserName, msg, DateTime.Now));
                ICollectionView view = CollectionViewSource.GetDefaultView(MessengerInterop.UsersMessenges[CurrentUser]);
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
    }
}
