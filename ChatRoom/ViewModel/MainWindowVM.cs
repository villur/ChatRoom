using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatRoom.BusinessObject;
using ChatRoom.Service;

namespace ChatRoom.ViewModel
{
    public class MainWindowVM : BaseVM
    {
        private int _lastRowId;
        private List<ChatMessageBO> _msgsBo;
        private List<UserBO> _friends;
        private UserBO _user;
        private int _currentFriendlistRecords;
        private int _newFriendlistRecords;

        public MainWindowVM(UserBO user)
        {
            _user = user;
            _lastRowId = 0;
            this._msgsBo = new List<ChatMessageBO>();
        }


        public List<ChatMessageBO> MsgsBo
        {
            get
            {
                return _msgsBo;
            }

            private set
            {
                _msgsBo = value;
            }
        }

        public List<UserBO> Friends
        {
            get
            {
                return _friends;
            }

            private set
            {
                _friends = value;
                base.NotifyPropertyChanged("Friends");
            }
        }

        public void LoadChatMsgs()
        {
            //if last row id has changed then fetches new message
            if (_lastRowId <= 0)
            {
                MsgsBo.AddRange(ChatroomMessageService.GetMessage(5, out _lastRowId));
            }
            else
            {
                MsgsBo.AddRange(ChatroomMessageService.GetMessageAfter(_lastRowId, out _lastRowId));
            }
        }

        public void LoadFriendList()
        {
            Friends = new List<UserBO>(FriendshipService.FindFriends(_user.UserId));
        }

        public bool CheckMsgs()
        {
            if (ChatroomMessageService.checkNewMsg(_lastRowId))
            {
                LoadChatMsgs();
                return true;
            }
            return false;
        }

        public bool CheckFriendList()
        {
            if (_friends != null)
            {
                _currentFriendlistRecords = _friends.Count();
            }

            if (_friends != null)
            {
                _newFriendlistRecords = FriendshipService.FindFriends(_user.UserId).Count();
            }

            if (_newFriendlistRecords != _currentFriendlistRecords)
            {
                LoadFriendList();
                return true;
            }
            return false;
        }

        public void AddMsg(String msgText)
        {
            ChatroomMessageService.AddMessage(_user.UserId, msgText);
        }

        public void RemoveFriend(UserBO friend)
        {
            FriendshipBO friendship1 = FriendshipService.FindFriendship(_user.UserId, friend.UserId);
            FriendshipBO friendship2 = FriendshipService.FindFriendship(friend.UserId, _user.UserId);
            FriendshipService.EndFriendship(friendship1.Friendship_id, friendship2.Friendship_id);
        }

        public void DeclineFriendship(UserBO declinedUser)
        {
            FriendshipBO declinedFriendship = FriendshipService.FindFriendship(declinedUser.UserId, _user.UserId);
            FriendshipService.EndOneWayFriendship(declinedFriendship.Friendship_id);
        }
    }
}
