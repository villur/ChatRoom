using System;
using System.Collections.Generic;
using System.Linq;
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
using ChatRoom.BusinessObject;

namespace ChatRoom.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            txtUsername.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            UserBO user;
            bool loginProcessed = Service.UserService.VerifyLogin(txtUsername.Text, pwdBox.Password, out user);
            

            if (loginProcessed)
            {
                bool loginClosed = Service.AdminService.HasBeenClosed(user.Username);

                if (loginClosed && loginProcessed)
                {
                    txtErrMessage.Text = "Your account has been closed!";
                }
                else
                {

                    if (loginProcessed == true)
                    {
                        //staatiline admini sisse logimine
                        if (Service.UserService.AdminValidation(txtUsername.Text) == true)
                        {
                            AdminWindow adminWindow = new AdminWindow(user);
                            Close();
                            adminWindow.Show();
                        }
                        else
                        {
                            MainWindow mainWindow = new MainWindow(user);
                            Close();
                            mainWindow.Show();
                        }
                    }
                }
            }
            else
            {
                txtErrMessage.Text = "Wrong username or password!";
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow regWindow = new RegisterWindow();
            regWindow.Show();
        }
    }
}
