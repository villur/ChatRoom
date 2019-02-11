using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom.BusinessObject
{
    public class FriendshipMessageBO
    {
        private int _friendship_message_id;
        private int _user_id;
        private int _friendship_id;
        private string _text;
        private DateTime _sent;
        private string _is_seen;
        private string _username;
        private String _formatedMsg;

        public FriendshipMessageBO(Domain.Friendship_message Friendship_message)
        {
            _friendship_message_id = Friendship_message.friendship_message_id;
            _user_id = Friendship_message.user_id;
            _friendship_id = Friendship_message.friendship_id;
            _text = Friendship_message.text;
            _sent = Friendship_message.sent;
            _is_seen = Friendship_message.is_seen;
            _username = Service.UserService.FindUserById(_user_id).Username;
            _formatedMsg = _username + "  [" + _sent.ToString("yyyy-MM-dd HH:mm") + "] : " + _text;
        }

        public int Friendship_message_id
        {
            get
            {
                return _friendship_message_id;
            }
        }

        public int User_id
        {
            get
            {
                return _user_id;
            }

            set
            {
                _user_id = value;
            }
        }

        public int Friendship_id
        {
            get
            {
                return _friendship_id;
            }

            set
            {
                _friendship_id = value;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
            }
        }

        public DateTime Sent
        {
            get
            {
                return _sent;
            }

            set
            {
                _sent = value;
            }
        }

        public string Is_seen
        {
            get
            {
                return _is_seen;
            }

            set
            {
                _is_seen = value;
            }
        }

        public String FormatedMsg
        {
            get
            {

                return _formatedMsg;
            }
        }
    }
}
