using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SBMessenger
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            MessengerInterop.MessageResult.MessageResultHandler handler = delegate ()
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    SuccessToaster.Toast(message: MessengerInterop.res.Message, animation: netoaster.ToasterAnimation.FadeIn);
                });
            };
            MessengerInterop.res.StatusChangedEvent += delegate () { };
            MessengerInterop.res.MessageReceivedEvent += handler;
            /*delegate ()
        {
            //throw new ApplicationException(MessengerInterop.res.Message);
            //SuccessToaster.Toast(message: MessengerInterop.res.Message, animation: netoaster.ToasterAnimation.FadeIn);
        };*/
        }



        ////////////////////////
        ////////Buttons/////////
        ////////////////////////
        private string receiver_id;
        private void button_Click_Login(object sender, RoutedEventArgs e)
        {
            MessengerInterop.Init();

            Task<OperationResult> task = MessengerInterop.Login("user_one@sb", "pass");
            MessengerInterop.RegisterObserver();
            receiver_id = "user_two@sb";
            switch (task.Result)
            {
                case OperationResult.Ok: SuccessToaster.Toast(message: "Loged in", animation: netoaster.ToasterAnimation.FadeIn); break;
                case OperationResult.AuthError: ErrorToaster.Toast(message: "AuthError"); break;
                case OperationResult.NetworkError: ErrorToaster.Toast(message: "NetworkError"); break;
                case OperationResult.InternalError: ErrorToaster.Toast(message: "InternalError"); break;
            }
        }
        private void button_Click_Disconnect(object sender, RoutedEventArgs e)
        {


            MessengerInterop.Init();

            Task<OperationResult> task = MessengerInterop.Login("user_two@sb", "pass");
            receiver_id = "user_one@sb";
            switch (task.Result)
            {
                case OperationResult.Ok: SuccessToaster.Toast(message: "Loged in", animation: netoaster.ToasterAnimation.FadeIn); break;
                case OperationResult.AuthError: ErrorToaster.Toast(message: "AuthError"); break;
                case OperationResult.NetworkError: ErrorToaster.Toast(message: "NetworkError"); break;
                case OperationResult.InternalError: ErrorToaster.Toast(message: "InternalError"); break;
            }

        }

        private void button_Click_ShowActiveUses(object sender, RoutedEventArgs e)
        {
            string msg = "Hello world";
            MessengerInterop.SendMessage(receiver_id, msg, msg.Length);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MessengerInterop.Disconnect();
        }
    }


}

