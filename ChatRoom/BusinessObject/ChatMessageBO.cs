using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatRoom.Service;


namespace ChatRoom.BusinessObject
{
    public class ChatMessageBO
    {
        private int _message_id;
        private int _user_id;
        private string _text;
        private DateTime _sent;
        private String _username;
        private String _formatedMsg;

        public ChatMessageBO(Domain.Chatroom_message chatMessage)
        {
            _message_id = chatMessage.Chatroom_message_Id;
            _user_id = chatMessage.user_id;
            _text = chatMessage.text;
            _sent = chatMessage.sent;
            _username = Service.UserService.FindUserById(_user_id).Username;
            _formatedMsg = _username + "  [" + _sent.ToString("yyyy-MM-dd HH:mm") + "] : " + _text;
        }

        public int Message_id
        {
            get
            {
                return _message_id;
            }

            set
            {
                _message_id = value;
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

        public String Username
        {
            get
            {   
                return _username;
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
