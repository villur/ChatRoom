using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatRoom.BusinessObject;
using ChatRoom.Service;

namespace ChatRoom.ViewModel
{
    public class AdminWindowVM : BaseVM
    {
        private List<UserBO> _users;

        public List<UserBO> Users
        {
            get
            {
                return _users;
            }

            private set
            {
                _users = value;
                base.NotifyPropertyChanged("Users");
            }
        }

        public void LoadUsers()
        {
            Users = new List<UserBO> (AdminService.GetUsers());
        }

        public int GetPostsCount(int userId)
        {
            return AdminService.CalcPosts(userId);
        }

        public void SendGlobalMsg(int userId, String text)
        {
            AdminService.BroadcastMsg(userId, text);
        }
    }


}
