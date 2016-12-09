using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

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
            MessengerInterop.res.StatusChangedEvent += delegate(){ };
            MessengerInterop.res.MessageReceivedEvent += delegate () { SuccessToaster.Toast(message: MessengerInterop.res.Message, animation: netoaster.ToasterAnimation.FadeIn); };
        }



        ////////////////////////
        ////////Buttons/////////
        ////////////////////////
        private string receiver_id;
        private void button_Click_Login(object sender, RoutedEventArgs e)
        {
            MessengerInterop.Init();

            Task<OperationResult> task = MessengerInterop.Login("User_One@sb","pass");
            receiver_id = "User_Two@sb";
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

            //MessengerInterop.Disconnect();
            MessengerInterop.Init();

            Task<OperationResult> task = MessengerInterop.Login("User_Two@sb", "pass");
            receiver_id = "User_One@sb";
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
    }


}

