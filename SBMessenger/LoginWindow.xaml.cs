using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace SBMessenger
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            //Environment.Exit(0);
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            const int pw_min_length= 6;
            string errors = "";
            IPAddress URL;
            ushort Port;
            try
            {
                if (login.Text.Length > 0)
                {
                    var eMailValidator = new System.Net.Mail.MailAddress(login.Text);
                }
                else
                {
                    errors += "\nНеправильный логин";
                }
                
            }
            catch (FormatException)
            {
                errors += "\nНеправильный логин";
            }
            if (password.Text.Length < pw_min_length)
            {
                errors += "\nСлишком короткий пароль";
            }
            if (!IPAddress.TryParse(url.Text,out URL))
            {
                errors += "\nНеправильный URL";
            }
            if (!ushort.TryParse(port.Text, out Port))
            {
                errors += "\nНеправильный порт";
            }
            if (errors == "")
            {
                MessengerInterop.Init(URL.ToString(), Port);
                Task<OperationResult> task = MessengerInterop.Login(login.Text, password.Text);
                
                switch (task.Result)
                {
                    case OperationResult.Ok:
                        SuccessToaster.Toast(message: "Успех", animation: netoaster.ToasterAnimation.FadeIn);
                        MessengerInterop.RegisterObserver();
                        MessengerInterop.RequestActiveUsers();
                        this.Close();
                        break;
                    case OperationResult.AuthError: ErrorToaster.Toast(message: "AuthError"); break;
                    case OperationResult.NetworkError: ErrorToaster.Toast(message: "NetworkError"); break;
                    case OperationResult.InternalError: ErrorToaster.Toast(message: "InternalError"); break;
                }
                
            }
            else
            {
                errors = "Возникли ошибки:" + errors;
                ErrorToaster.Toast(message: errors);
            }
        }
    }
}
