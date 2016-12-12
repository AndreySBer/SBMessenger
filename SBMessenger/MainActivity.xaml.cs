using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            this.Show();
            showDialog();
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
