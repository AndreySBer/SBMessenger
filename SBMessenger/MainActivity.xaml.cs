using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SBMessenger
{
    /// <summary>
    /// Interaction logic for MainActivity.xaml
    /// </summary>
    public partial class MainActivity : Window
    {
        List<Messenge> items;
        public MainActivity()
        {
            InitializeComponent();
            this.DataContext = MessengerInterop.Users;
            this.Show();
           

            MessengerInterop.MessageResultHandler messageReceivedHandler = delegate ()
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    string mes = Encoding.UTF8.GetString(MessengerInterop.mRres.Message);
                    mes = mes.Remove(mes.Length - 1);
                    SuccessToaster.Toast(message: mes, animation: netoaster.ToasterAnimation.FadeIn);
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
                    UsersList.ItemsSource = MessengerInterop.Users.Select<User, String>(u => u.UserID);
                    CurrentUser = MessengerInterop.Users[0].UserID;
                });
            };
            MessengerInterop.mRres.MessageReceivedEvent += messageReceivedHandler;
            MessengerInterop.stCres.StatusChangedEvent += changedStatusHandler;
            MessengerInterop.urh.UsersChangedEvent += usersRequestHandler;
            showDialog();

            items = new List<Messenge>();
            
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
            CurrentUser= (string)e.AddedItems[0];
            UserName.Content = CurrentUser;
            items = new List<Messenge>();
            items.Add(new Messenge() { UserName = "UserName", Text = "Text", Time = "Time" });
            MessagesLV.ItemsSource = items;
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string msg = MessageText.Text;
            if (msg.Length > 0)
            {
                byte[] mesg = Encoding.UTF8.GetBytes(msg + '\0');
                MessengerInterop.SendComplexMessage(CurrentUser, MessageContentType.Text, false, mesg, mesg.Length);
                items.Add(new Messenge() { UserName = MessengerInterop.UserName, Text = msg, Time = DateTime.Now.ToShortTimeString() });
                ICollectionView view = CollectionViewSource.GetDefaultView(items);
                view.Refresh();
            }
            else
            {
                ErrorToaster.Toast(message: "Нельзя отправлять пустые сообщения", animation: netoaster.ToasterAnimation.FadeIn);
            }
        }

        public class Messenge
        {
            public string UserName { get; set; }
            public string Time { get; set; }
            public string Text { get; set; }
            
        }
    }
}
