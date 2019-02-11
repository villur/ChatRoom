using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatRoom.Domain;
using ChatRoom.BusinessObject;

namespace ChatRoom.Service
{
    /// <summary>
    /// FriendshipMessageService class. 
    /// Põhiliselt kasutatakse selle classi meetodeid privaatsõnumite ohjamiseks.
    /// </summary>
    public class FriendshipMessageService
    {
        /// <summary>
        /// FriendHasSeenMsg meetod.
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="friendshipId"></param>
        /// <returns>Tagastab tõeväärtuse kas sõber on näinud sõnumit</returns>
        public static bool FriendHasSeenMsg(int msgId, int friendshipId)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var friendMessage = (from x in db.Friendship_message
                                     where x.friendship_message_id.Equals(msgId) &&
                                     x.friendship_id.Equals(friendshipId)
                                     select x).FirstOrDefault();
                if (friendMessage != null)
                {
                    return true;
                }
                return false;
            }
        }

        
        /// <summary>
        /// friendSawMsg meetod.
        /// See meetod lisab andmebaasi märke selle kohta, et sõnumit on nähtud.
        /// </summary>
        /// <param name="msgId"></param>
        public static void friendSawMsg(int msgId)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var friendMessage = (from x in db.Friendship_message
                                     where x.friendship_message_id.Equals(msgId)
                                     select x).FirstOrDefault();
                if (friendMessage.is_seen != "1")
                {
                    friendMessage.is_seen = "1";
                    db.SaveChanges();
                }
            }
        }


        /// <summary>
        /// AddMessage meetod.
        /// Antud meetod lisab uue privaatvestluse sõnumi andmebaasi.
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="friendship_id"></param>
        /// <param name="message"></param>
        public static void AddMessage(int user_id, int friendship_id, string message)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                Friendship_message friendMessage = new Friendship_message
                {
                    user_id = user_id,
                    friendship_id = friendship_id,
                    text = message,
                    sent = DateTime.Now,
                    is_seen = ""

                };
                db.Friendship_message.Add(friendMessage);
                db.SaveChanges();
            }
        }


        /// <summary>
        /// GetMessage meetod.
        /// Pärib andmebaasist jututoa hiljutised sõnumid. Sõnumite kogus sõltub parameetri sisendist. 
        /// Antud meetod kasutab out parameetrit selleks, et anda tagasi infot, milline oli viimase sõnumi indeks andmebaasis. See on eelkõige oluline antud projekti tehnilises lahenduses.
        /// </summary>
        /// <param name="messageCount"></param>
        /// <param name="lastMsgId"></param>
        /// <param name="friendship1"></param>
        /// <param name="friendship2"></param>
        /// <returns>Tagastab listi privaatvestluse sõnumi objektidest</returns>
        public static List<FriendshipMessageBO> GetMessage(int messageCount, out int lastMsgId, int friendship1, int friendship2)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                lastMsgId = 0;
                var m_list = ((from x in db.Friendship_message
                               where x.friendship_id.Equals(friendship1) || x.friendship_id.Equals(friendship2)
                               orderby x.friendship_message_id descending
                               select x).Take(messageCount)).OrderBy(x => x.sent).ToList();

                List<FriendshipMessageBO> message_list = new List<FriendshipMessageBO>();
                foreach (var item in m_list)
                {
                    if (item.friendship_id == friendship2) // sonumid mis on sobralt
                    {
                        friendSawMsg(item.friendship_message_id);
                    }
                    message_list.Add(new FriendshipMessageBO(item));
                    lastMsgId = item.friendship_message_id;
                }
                return message_list;
            }
        }


        /// <summary>
        /// Pärib andmebaasist hiljutisemad sõnumid peale parameetri sisestatud indeksit. 
        /// Antud meetod kasutab out parameetrit selleks, et anda tagasi infot, milline oli viimase sõnumi indeks andmebaasis. See on eelkõige oluline antud projekti tehnlises lahenduses.
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="lastMsgId"></param>
        /// <param name="friendship1"></param>
        /// <param name="friendship2"></param>
        /// <returns>Tagastab listi privaatvestluse sõnumite objektidena</returns>
        public static List<FriendshipMessageBO> GetMessageAfter(int msgId, out int lastMsgId, int friendship1, int friendship2)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                lastMsgId = 0;
                var m_list = ((from x in db.Friendship_message
                               where x.friendship_id.Equals(friendship1) || x.friendship_id.Equals(friendship2)
                               orderby x.friendship_message_id descending
                               where x.friendship_message_id > msgId
                               select x).OrderBy(x => x.sent)).ToList();

                List<FriendshipMessageBO> message_list = new List<FriendshipMessageBO>();
                foreach (var item in m_list)
                {
                    if (item.friendship_id == friendship2) // sonumid mis on sobralt
                    {
                        friendSawMsg(item.friendship_message_id);
                    }
                    message_list.Add(new FriendshipMessageBO(item));
                    lastMsgId = item.friendship_message_id;
                }
                return message_list;
            }
        }


        /// <summary>
        /// CheckNewMsg meetod.
        /// Antud meetod kontrollib kas on laekunud uusi sõnumeid andmebaasi peale sisestatud parameetri indeksit.
        /// Juhul kui out parameeter tagastab sõnumi objekti, siis on leakunud uus sõnum. See on eelkõige oluline antud projekti tehnlises lahenduses.
        /// </summary>
        /// <param name="lastMsgId"></param>
        /// <param name="friendship1"></param>
        /// <param name="friendship2"></param>
        /// <param name="lastMsg"></param>
        /// <returns>Tagastab tõeväärtuse uue sõnumi laekumise kohta</returns>
        public static bool CheckNewMsg(int lastMsgId, int friendship1, int friendship2, out FriendshipMessageBO lastMsg)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var msg = (from x in db.Friendship_message
                             where x.friendship_id.Equals(friendship1) || x.friendship_id.Equals(friendship2)
                             orderby x.friendship_message_id descending
                             select x).FirstOrDefault();

                if (msg == null)
                {
                    lastMsg = null;
                    return false;
                }

                lastMsg = new FriendshipMessageBO(msg);

                if (msg.friendship_message_id == lastMsgId)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
