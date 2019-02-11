using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom.BusinessObject
{
    public class UserBO
    {
        private int _user_id;
        private string _username;
        private string _password;
        private string _email;
        private DateTime _created;
        private DateTime? _closed;

        public UserBO(Domain.User user)
        {
            _user_id = user.user_id;
            _username = user.username;
            _password = user.password;
            _email = user.email;
            _created = user.created;
            _closed = user.closed;
            
        }

        public int UserId
        {
            get { return _user_id; }
        }

        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;
            }
        }

        public string Password
        {
            set
            {
                _password = value;
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                _email = value;
            }
        }

        public DateTime Created
        {
            get
            {
                return _created;
            }
            set
            {
                _created = value;
            }
        }


        public DateTime? Closed
        {
            get
            {
                return _closed;
            }
            set
            {
                _closed = value;
            }
        }
    }
}