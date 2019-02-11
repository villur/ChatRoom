using ChatRoom.BusinessObject;
using ChatRoom.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatRoom.Service;

namespace ChatRoom.Service
{
    /// <summary>
    /// AdminService class. 
    /// Põhiliselt kasutatakse selle meetodeid AdminView jaoks.
    /// </summary>
    public class AdminService
    {
        /// <summary>
        /// GetUsers meetod.
        /// Eelkõige vajalik kasutajate kuvamisel listis (AdminView aknas).
        /// </summary>
        /// <returns>
        /// Tagastab listi UserBO objektidest.
        /// </returns>
        public static List<UserBO> GetUsers()
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var u_list = (from x in db.User
                              select x).ToList();

                List<UserBO> user_list = new List<UserBO>();
                foreach (var item in u_list)
                {
                    user_list.Add(new UserBO(item));
                }
                return user_list;
            }
        }


        /// <summary>
        /// CloseUser meetod.
        /// See meetod lukustab soovitud kasutaja. Lukustamine toimub ajatempli alusel. Kasutatakse AdminView aknas kasutajate lukustamiseks.
        /// </summary>
        public static void CloseUser(string selected_username)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var closeIt =
                (from x in db.User
                 where x.username == selected_username
                 select x).SingleOrDefault();

                closeIt.closed = DateTime.Now;
                db.SaveChanges();
            }

        }


        /// <summary>
        /// ActivateUser meetod.
        /// See meetod aktiveerib soovitud kasutaja kasutajatunnuse alusel. Aktiveerimise käigus muudetakse ära andmebaasis kasutaja sulgemiskuupäev (null). Kasutatakse AdminView aknas kasutajate aktiveerimiseks.
        /// </summary>
        public static void ActivateUser(string selected_username)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var closeIt =
                (from x in db.User
                 where x.username == selected_username
                 select x).SingleOrDefault();

                closeIt.closed = null;

                db.SaveChanges();
            }

        }


        /// <summary>
        /// HasBeenClosed meetod.
        /// Kontrollib andmebaasist aja järgi kas kasutaja on lukustatud või mitte.
        /// </summary>
        /// <param name="selected_username"></param>
        /// <returns>tagastab tõeväärtuse</returns>
        public static bool HasBeenClosed(string selected_username)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var info =
                (from x in db.User
                 where x.username == selected_username && x.closed != null
                 select x).SingleOrDefault();

                if (info != null)
                {
                    return true;
                }
                return false;
            }
        }


        /// <summary>
        /// ChangeUserPass meetod.
        /// Muudab soovitud kasutaja parooli ära. Kasutusel AdminView vaate jaoks. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public static void ChangeUserPass(UserBO user, string password)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                String hashedPassword = UserService.HashPass(password);
                var change =
                (from x in db.User
                 where x.username == user.Username
                 select x).SingleOrDefault();
                change.password = hashedPassword;
                db.SaveChanges();
            }
        }


        /// <summary>
        /// CalcPosts meetod.
        /// Pärib andmebaasist nii jututoa kui ka privaatvestluste postitused ja arvutab need kokku.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Tagastab kasutaja kõikide postituste summa.</returns>
        public static int CalcPosts(int userId)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                int total = 0;
                var chatroomPosts = (from x in db.Chatroom_message
                                     where x.user_id.Equals(userId)
                                     select x);
                if (chatroomPosts != null)
                {
                    total = chatroomPosts.Count();
                }
                var friendPosts = (from x in db.Friendship_message
                                   where x.user_id.Equals(userId)
                                   select x);
                if (friendPosts != null)
                {
                    total = total + friendPosts.Count();
                }
                return total;
            }
        }

        /// <summary>
        /// BroadcastMsg meetod.
        /// Antud meetod edastab programmi administraatori sõnumi kõikidele kasutajatele. Kasutusel AdminView aknas, et saaks edastada globaalsõnumit.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msg"></param>
        public static void BroadcastMsg(int userId, String msg)
        {
            ChatroomMessageService.AddMessage(userId, msg);
        }
    }
}
