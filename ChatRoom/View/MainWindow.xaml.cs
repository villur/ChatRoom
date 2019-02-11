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
using System.Collections.ObjectModel;
using ChatRoom.Domain;
using ChatRoom.BusinessObject;
using ChatRoom.Service;
using System.Threading;
using System.Timers;
using ChatRoom.ViewModel;

namespace ChatRoom.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private UserBO _user;
        private MainWindowVM _vm;
        private bool _friendRequestDialogOpened;

        public MainWindow(UserBO user)
        {
            InitializeComponent();
            txtMessage.Focus();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            _user = user;
            txtLoggedInAs.Text = txtLoggedInAs.Text + _user.Username;
            _friendRequestDialogOpened = false;

            _vm = new ViewModel.MainWindowVM(_user);
            _vm.LoadFriendList();
            _vm.LoadChatMsgs();
            this.DataContext = _vm;

            //Timer
            System.Timers.Timer msgTimer = new System.Timers.Timer();
            msgTimer.Elapsed += new ElapsedEventHandler(DoUpdate);
            msgTimer.Interval = 1000;
            msgTimer.Enabled = true;
        }

        private void DoUpdate(object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (_friendRequestDialogOpened == false)
                {
                    ShowFriendRequests();
                    _friendRequestDialogOpened = false;
                }
                
                _vm.CheckFriendList();

                if (_vm.CheckMsgs())
                {
                    msgBox.Items.Refresh();

                    //scroll down
                    msgBox.Items.MoveCurrentToLast();
                    msgBox.ScrollIntoView(msgBox.Items.CurrentItem);
                }
                
            });
        }

        private void btnAddFriend_Click(object sender, RoutedEventArgs e)
        {
            int result = FriendshipService.AddFriend(_user.UserId, txtAddFriend.Text);
            switch (result)
            {
                case 0:
                    textBlockAddFriend.Text = "Friend request sent!";
                    break;
                case -1:
                    textBlockAddFriend.Text = "User does not exist!";
                    break;
                case -2:
                    textBlockAddFriend.Text = "Friend request has already sent!";
                    break;
            }
        }

        public void ShowFriendRequests()
        {
            _friendRequestDialogOpened = true;
            List<UserBO> pendingFriend = new List<UserBO>(FriendshipService.FindFriendRequests(_user.UserId));
            if (pendingFriend == null)
            {
                return;
            }
            foreach (var friend in pendingFriend)
            {
                PendingFriendshipDialog(friend.Username);
            }
        }

        public void PendingFriendshipDialog(string user)
        {
            MessageBoxResult result = MessageBox.Show(user + " would like to be your friend.", "Friend request", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Service.FriendshipService.AddFriend(_user.UserId, user);
                    break;
                case MessageBoxResult.No:
                    _vm.DeclineFriendship(UserService.FindUserByUsername(user));
                    break;
            }   
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            _vm.AddMsg(txtMessage.Text);
            txtMessage.Text = "";
        }

        private void msgBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {       
            ChatMessageBO msgUser = (ChatMessageBO)(msgBox.SelectedValue);
            if (msgUser.Username != _user.Username)
            {
                txtAddFriend.Text = msgUser.Username;
            }
            else
            {
                txtAddFriend.Text = "";
            }   
        }

        private void friendBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UserBO chatFriend = (UserBO)(friendBox.SelectedValue);
            FriendWindow friendChat = new FriendWindow(_user, chatFriend);
            friendChat.Show();
        }

        private void FriendBoxMenu_Click(object sender, RoutedEventArgs e)
        {
            UserBO selectedFriend = (UserBO)(friendBox.SelectedValue);
            _vm.RemoveFriend(selectedFriend);
        }
    }
}
