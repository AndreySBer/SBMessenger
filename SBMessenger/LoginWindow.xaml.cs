using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//using static System.Windows.Controls.BooleanToVisibilityConverter;
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

        private void Login(object sender, RoutedEventArgs e)
        {
            const int pw_min_length= 6;
            string errors = "";
            IPAddress URL;
            ushort Port;
            try
            {
                var eMailValidator = new System.Net.Mail.MailAddress(login.Text);
                
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
