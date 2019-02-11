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
using ChatRoom.Service;
using ChatRoom.BusinessObject;
using ChatRoom.ViewModel;

namespace ChatRoom.View
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private UserBO _admin;
        private AdminWindowVM _vm;
        private UserBO _listUser;

        public AdminWindow(UserBO admin)
        {
            InitializeComponent();
            _admin = admin;
            _vm = new AdminWindowVM();
            this.DataContext = _vm;
            _vm.LoadUsers();
        }

        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            if (AdminService.HasBeenClosed(_listUser.Username))
            {

                txtInfo.Text = _listUser.Username + " is already closed!";
            }
            else
            {
                AdminService.CloseUser(_listUser.Username);
                txtInfo.Text = _listUser.Username + " was closed successfully!";
            }
        }

        private void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            if (AdminService.HasBeenClosed(_listUser.Username))
            {
                AdminService.ActivateUser(_listUser.Username);
                txtInfo.Text = _listUser.Username + " was activated again!";
            }
            else
            {
                txtInfo.Text = _listUser.Username + " is already active!";
            }
        }

        private void listUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _listUser = (UserBO)(listUsers.SelectedValue);
            if (_listUser != null)
            {
                txtInfo.Text = "username: " + _listUser.Username + "\nemail: " + _listUser.Email + "\nuser created: " + _listUser.Created + "\nuser closed: " + _listUser.Closed + "\nposts: " + _vm.GetPostsCount(_listUser.UserId);
            }
            else
            {
                txtInfo.Text = "";
            }
        }

        private void btnChangePass_Click(object sender, RoutedEventArgs e)
        {
            AdminService.ChangeUserPass(_listUser, txtPass.Password);
            txtInfo.Text = _listUser.Username + " password was changed!";
            txtPass.Password = "";
        }

        private void btnSendAll_Click(object sender, RoutedEventArgs e)
        {
            _vm.SendGlobalMsg(_admin.UserId, txtGlobal.Text);
            txtInfo.Text = "Global message was sent!";
            txtGlobal.Text = "";
        }

        private void btnUserView_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(_admin);
            mainWindow.Show();
        }

        private void btnLoadUsers_Click(object sender, RoutedEventArgs e)
        {
            listUsers.UnselectAll();
            _vm.LoadUsers();
        }
    }
}
