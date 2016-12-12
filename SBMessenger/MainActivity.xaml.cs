using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public MainActivity()
        {
            InitializeComponent();
            this.DataContext = MessengerInterop.Users;
            this.Show();
            showDialog();

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
                });
            };
            MessengerInterop.mRres.MessageReceivedEvent += messageReceivedHandler;
            MessengerInterop.stCres.StatusChangedEvent += changedStatusHandler;
            MessengerInterop.urh.UsersChangedEvent += usersRequestHandler;

        }
        void showDialog()
        {
            LoginWindow aboutWindow = new LoginWindow();
            aboutWindow.Owner = this;
            aboutWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            aboutWindow.ShowDialog();
        }

        
    }
}
