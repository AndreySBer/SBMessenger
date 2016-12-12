using System;
using System.Text;
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
            MessengerInterop.MessageResultHandler handler = delegate ()
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    string mes = Encoding.UTF8.GetString(MessengerInterop.mRres.Message);
                    mes = mes.Remove(mes.Length - 1);
                    SuccessToaster.Toast(message: mes + " " + receiver_id, animation: netoaster.ToasterAnimation.FadeIn);
                });
            };

            MessengerInterop.MessageResultHandler handler1 = delegate ()
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    string mes = "Message is " + Enum.GetName(typeof(MessageStatus), MessengerInterop.stCres.Status);
                    ErrorToaster.Toast(message: mes, animation: netoaster.ToasterAnimation.FadeIn);

                });


            };
            MessengerInterop.stCres.StatusChangedEvent += handler1;
            MessengerInterop.mRres.MessageReceivedEvent += handler;
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
            MessengerInterop.RequestActiveUsers();
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
            string msg = "Please help me";
            byte[] mesg = Encoding.UTF8.GetBytes(msg + '\0');
            MessengerInterop.SendComplexMessage(receiver_id, MessageContentType.Text, false, mesg, mesg.Length);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MessengerInterop.Disconnect();
        }
    }


}

