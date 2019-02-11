using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatRoom.Domain;

namespace ChatRoom.BusinessObject
{
    public class FriendshipBO
    {
        private int _friendship_id;
        private int _user1_id;
        private int _user2_id;
        private DateTime _started;
        private DateTime? _ended;

        public FriendshipBO(Domain.Friendship friendship)
        {
            _friendship_id = friendship.friendship_id;
            _user1_id = friendship.user1_id;
            _user2_id = friendship.user2_id;
            _started = friendship.started;
            _ended = friendship.ended;
        }

        public int Friendship_id
        {
            get
            {
                return _friendship_id;
            }
        }

        public int User1_id
        {
            get
            {
                return _user1_id;
            }

            set
            {
                _user1_id = value;
            }
        }

        public int User2_id
        {
            get
            {
                return _user2_id;
            }

            set
            {
                _user2_id = value;
            }
        }

        public DateTime Started
        {
            get
            {
                return _started;
            }

            set
            {
                _started = value;
            }
        }

        public DateTime? Ended
        {
            get
            {
                return _ended;
            }

            set
            {
                _ended = value;
            }
        }
    }
}
