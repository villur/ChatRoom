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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatRoom.Service;
using System.Text.RegularExpressions;



namespace ChatRoom.View
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void btnregister_Click(object sender, RoutedEventArgs e)
        {
            if (UserService.FindUserByUsername(txtUsername.Text) != null)
            {
                txtblockReg.Text = "Username already exists!";
            }
            else
            {
                if (legitPass() && hasValue() && emailIsValid(txtEmail.Text))
                {
                    Service.UserService.RegisterUser(txtUsername.Text, pwdBox.Password, txtEmail.Text);
                    txtblockReg.Text = "Great success! User created.";
                    this.Close();
                }
                else if (legitPass() == false)
                {
                    txtblockReg.Text = "Passwords do not match.";
                }
                else if (emailIsValid(txtEmail.Text) == false)
                {
                    txtblockReg.Text = "Email is invalid";
                }
                else
                {
                    txtblockReg.Text = "Username or email is invalid. Maybe password is also missing";
                }

            }
        }

        private bool legitPass()
        {
            if (pwdBox.Password == pwdBox2.Password)
            {
                return true;
            }
            return false;
        }
        private bool hasValue()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(pwdBox.Password))
            {
                return false;
            }
            return true;
        }

        public bool emailIsValid(string email)
        {
            string expression;
            expression = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expression))
            {
                if (Regex.Replace(email, expression, string.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }
}
