using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatRoom.BusinessObject;
using ChatRoom.Service;

namespace ChatRoom.ViewModel
{
    public class FriendWindowVM : BaseVM
    { 
        private int _lastRowId;
        private List<FriendshipMessageBO> _msgsBo;
        private FriendshipMessageBO _lastMsgBo;
        private UserBO _user;
        private UserBO _friend;
        private int _friendship_id1;
        private int _friendship_id2;

        public FriendWindowVM(UserBO user, UserBO friend)
        {
            _msgsBo = new List<FriendshipMessageBO>();
            _lastRowId = 0;
            _user = user;
            _friend = friend;
            _friendship_id1 = FriendshipService.FindFriendship(_user.UserId, _friend.UserId).Friendship_id;
            _friendship_id2 = FriendshipService.FindFriendship(_friend.UserId, _user.UserId).Friendship_id;
        }

        public List<FriendshipMessageBO> MsgsBo
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

        public void LoadMsgs()
        {
            if (_lastRowId <= 0)
            {
                MsgsBo.AddRange(FriendshipMessageService.GetMessage(5, out _lastRowId, _friendship_id1, _friendship_id2));
            }
            else
            {
                MsgsBo.AddRange(FriendshipMessageService.GetMessageAfter(_lastRowId, out _lastRowId, _friendship_id1, _friendship_id2));
            }
        }

        public  bool CheckMsgs()
        {
            if (FriendshipMessageService.CheckNewMsg(_lastRowId, _friendship_id1, _friendship_id2, out _lastMsgBo))
            {
                return true;
            }
            return false;
        }

        public bool CheckFriendship()
        {
            if (FriendshipService.FindFriendship(_user.UserId, _friend.UserId) != null)
            {
                return true;
            }
            return false;
        }

        public bool LastMsgIsSeen()
        {
            if (_lastMsgBo != null && _lastMsgBo.Is_seen == "1" && _lastMsgBo.User_id == _user.UserId)
            {
                return true;
            }
            return false;
        }

        public void AddMsg(String msgText)
        {
            FriendshipMessageService.AddMessage(_user.UserId, _friendship_id1, msgText);
        }
    }
}
