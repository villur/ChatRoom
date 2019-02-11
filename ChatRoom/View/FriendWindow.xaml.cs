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
using System.Timers;
using ChatRoom.ViewModel;

namespace ChatRoom.View
{
    /// <summary>
    /// Interaction logic for FriendWindow.xaml
    /// </summary>
    /// 
    public partial class FriendWindow
        
    {
        private UserBO _user;
        private UserBO _friend;
        private FriendWindowVM _vm;

        public FriendWindow(UserBO user, UserBO friend)
        {
            InitializeComponent();
            _user = user;
            _friend = friend;
            this.Title = "Chat with " + _friend.Username;

            _vm = new FriendWindowVM(_user, _friend);
            _vm.LoadMsgs();
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
                if (_vm.CheckMsgs())
                {
                    _vm.LoadMsgs();
                    chatBoxFriend.Items.Refresh();

                    //scroll down
                    chatBoxFriend.Items.MoveCurrentToLast();
                    chatBoxFriend.ScrollIntoView(chatBoxFriend.Items.CurrentItem);
                }
                
                if (!_vm.CheckFriendship())
                {
                    this.Close();
                }

                if (_vm.LastMsgIsSeen())
                {
                    txtIsSeen.Text = _friend.Username + " has seen your last message.";
                }
                else
                {
                    txtIsSeen.Text = "";
                }
                
            });
        }

        private void btnSendFriend_Click(object sender, RoutedEventArgs e)
        {
            _vm.AddMsg(txtBoxFriend.Text);
            txtBoxFriend.Text = "";
        }
    }
}
