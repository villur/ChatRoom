using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatRoom.BusinessObject;
using ChatRoom.Domain;

namespace ChatRoom.Service
{
    /// <summary>
    /// FriendshipService class. 
    /// Põhiliselt kasutatakse selle classi meetodeid selleks, et kuvada kasutajaliideses sõbralist ja luua seoseid kasutajate vahel.
    /// </summary>
    public class FriendshipService
    {
        /// <summary>
        /// FindFriendship meetod.
        /// Antud meetod on kasulik, et teada saada kas kahe kasutaja vahel eksisteerib "ühepoolne" sõprus.
        /// Näiteks kas üks kasutaja on hiljuti kedagi oma sõbraks lisanud.
        /// </summary>
        /// <param name="user1Id"></param>
        /// <param name="user2Id"></param>
        /// <returns>Tagastab objekti kahe kasutaja sõpruse kohta. Kui seda ei eksisteeri, siis tagastakse null.</returns>
        public static FriendshipBO FindFriendship(int user1Id, int user2Id)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var friendship = (from x in db.Friendship
                                  where x.user1_id.Equals(user1Id) &&
                                  x.user2_id.Equals(user2Id) &&
                                 (x.ended.Equals(null) || x.ended > DateTime.Now)
                                  select x).FirstOrDefault();
                if (friendship == null)
                {
                    return null;
                }
                return new FriendshipBO(friendship);
            }
        }

        /// <summary>
        /// FindFriends meetod.
        /// Antud meetod leiab vastavale kasutajale kõik sõbrad. See meetod on kasulik näiteks sõbralisti kuvamisel.
        /// Tegu on projekti ühe keerukama meetodiga. Suur abi selle meetodi loomiseks oli siit: http://www.codeproject.com/Questions/280296/query-to-display-mutual-friends .
        /// </summary>
        /// <param name="current_user_id"></param>
        /// <returns>Tagastab listi kasutajate objektidest.</returns>
        public static List<UserBO> FindFriends(int current_user_id)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var friends = (from x in db.Friendship
                               join z in db.User on x.user2_id equals z.user_id
                               where x.user1_id.Equals(current_user_id) &&
                               (x.ended.Equals(null) || x.ended > DateTime.Now) &&
                               (from y in db.Friendship
                                where y.user1_id.Equals(x.user2_id) && y.user2_id.Equals(x.user1_id) &&
       (y.ended.Equals(null) || y.ended > DateTime.Now)
                                select y.user1_id).ToList().Contains(x.user2_id)
                                select z).ToList();

                List<UserBO> friendsList = new List<UserBO>();

                foreach (var friend in friends)
                {
                    UserBO user = new UserBO(friend);
                    friendsList.Add(user);
                }
                return friendsList;
            }
        }

        /// <summary>
        /// UserHasOneWayFriendShip meetod.
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="friend_id"></param>
        /// <returns>Tagastab tõeväärtuse kas kasutajal eksisteerib ühepoolne sõprus.</returns>
        public static bool UserHasOneWayFriendShip(int currentUserId, int friend_id)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var friendship = (from x in db.Friendship
                                  where x.user1_id.Equals(currentUserId) &&
                                  x.user2_id.Equals(friend_id) &&
                                  (x.ended.Equals(null) || x.ended > DateTime.Now)
                                  select x).FirstOrDefault();

                if (friendship == null)
                {
                    return false;
                }
                var test = friendship.User;
                return true;
            }
        }


        /// <summary>
        /// FindFriendRequests meetod.
        /// Antud meetod pärib andmebaasist kõik kasutajad, kes soovivad luua sõprussuhet.
        /// </summary>
        /// <param name="current_user_id"></param>
        /// <returns>Tagastab listi kasutajatest</returns>
        public static List<UserBO> FindFriendRequests(int current_user_id)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var friends = (from x in db.Friendship
                               join z in db.User on x.user1_id equals z.user_id
                               where x.user2_id.Equals(current_user_id) &&
                               (x.ended.Equals(null) || x.ended > DateTime.Now) &&
                               !(from y in db.Friendship
                                 where y.user2_id.Equals(x.user1_id) && y.user1_id.Equals(x.user2_id) &&
       (y.ended.Equals(null) || y.ended > DateTime.Now)
                                 select y.user2_id).ToList().Contains(x.user1_id)
                               select z).ToList();

                List<UserBO> pendingList = new List<UserBO>();

                foreach (var friend in friends)
                {
                    UserBO user = new UserBO(friend);
                    pendingList.Add(user);
                }
                return pendingList;
            }
        }


        /// <summary>
        /// AddFriend meetod.
        /// Antud meetod üritab luua uue sõprussuhte ja tagastab numbrilise väärtuse kasutaja sõbraks lisamise õnnestumise kohta.
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="friend_username"></param>
        /// <returns>0 lisamine õnnestus, -1 kasutajat ei leitud, -2 sõbrakutse on juba saadetud</returns>
        public static int AddFriend(int currentUserId, string friend_username)
        {
            UserBO friend = UserService.FindUserByUsername(friend_username);
            if (friend == null)
            {
                return -1; // kasutajat ei leitud!
            }

            if (UserHasOneWayFriendShip(currentUserId, friend.UserId))
            {
                return -2; // s6brakutse on juba saadetud!
            }
            using (chatdbEntities db = new chatdbEntities())
            {
                Friendship oneWayHandshake = new Friendship
                {
                    user1_id = currentUserId,
                    user2_id = friend.UserId,
                    started = DateTime.Now
                };
                db.Friendship.Add(oneWayHandshake);
                db.SaveChanges();
            }
            return 0; // ok
        }

        /// <summary>
        /// EndFriendship meetod.
        /// Meetod, mis lõpetab kasutajatevahelise sõpruse. Sõpruse eemaldamine on andmebaasis "kahepoolne".
        /// Leiab rakendust näiteks, siis kui lõppkasutaja soovib eemalda sõbra enda sõbranimekirjast.
        /// </summary>
        /// <param name="friendshipId1"></param>
        /// <param name="friendshipId2"></param>
        public static void EndFriendship(int friendshipId1, int friendshipId2)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                Friendship friendshipRecord1 = (from x in db.Friendship
                                                where x.friendship_id.Equals(friendshipId1)
                                                select x).FirstOrDefault();

                Friendship friendshipRecord2 = (from x in db.Friendship
                                                where x.friendship_id.Equals(friendshipId2)
                                                select x).FirstOrDefault();
                if (friendshipRecord1 != null)
                {
                    friendshipRecord1.ended = DateTime.Now;
                    db.SaveChanges();
                }
                if (friendshipRecord2 != null)
                {
                    friendshipRecord2.ended = DateTime.Now;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// EndOneWayFriendship meetod.
        /// Meetod, mis lõpetab kasutajate vahelise sõpruse ainult ühepoolselt.
        /// Meetod leiab rakendust näiteks, siis kui kasutaja vastab uuele sõbrakutsele eitavalt.
        /// </summary>
        /// <param name="friendshipId"></param>
        public static void EndOneWayFriendship(int friendshipId)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                Friendship friendshipRecord = (from x in db.Friendship
                                               where x.friendship_id.Equals(friendshipId)
                                               select x).FirstOrDefault();
                if (friendshipRecord != null)
                {
                    friendshipRecord.ended = DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
    }
}
