using ChatRoom.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ChatRoom.BusinessObject;

namespace ChatRoom.Service
{
    /// <summary>
    /// ChatroomMessage class. 
    /// Põhiliselt kasutatakse selle classi meetodeid jututoa sõnumite ohjamiseks.
    /// </summary>
    public class ChatroomMessageService
    {
        
        /// <summary>
        /// AddMessage meetod.
        /// Lisab andmebaasi kasutaja poolt saadetud sõnumi.
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="message"></param>
        public static void AddMessage(int user_id, string message)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                Chatroom_message chatMessage = new Chatroom_message
                {
                    user_id = user_id,
                    text = message,
                    sent = DateTime.Now
                };
                db.Chatroom_message.Add(chatMessage);
                db.SaveChanges();
            }
        }



        /// <summary>
        /// GetMessage meetod.
        /// Pärib andmebaasist jututoa hiljutised sõnumid. Sõnumite kogus sõltub parameetri sisendist. 
        /// Antud meetod kasutab out parameetrit selleks, et anda tagasi infot, milline oli viimase sõnumi indeks andmebaasis. See on eelkõige oluline antud projekti tehnlises lahenduses.
        /// </summary>
        /// <param name="messageCount"></param>
        /// <param name="lastRowId"></param>
        /// <returns>Tagastab jututoa sõnumite objektide listi.</returns>
        public static List<ChatMessageBO> GetMessage(int messageCount, out int lastRowId)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                lastRowId = 0;
                var m_list = ((from x in db.Chatroom_message
                               orderby x.Chatroom_message_Id descending
                               select x).Take(messageCount)).OrderBy(x => x.Chatroom_message_Id).ToList();

                List<ChatMessageBO> msgs = new List<ChatMessageBO>();
                foreach (var item in m_list)
                {
                    msgs.Add(new ChatMessageBO(item));
                    lastRowId = item.Chatroom_message_Id;
                }
                return msgs;
            }
        }


        /// <summary>
        /// GetMessageAfter meetod.
        /// Pärib andmebaasist hiljutisemad sõnumid peale parameetri sisestatud indeksit. 
        /// Antud meetod kasutab out parameetrit selleks, et anda tagasi infot, milline oli viimase sõnumi indeks andmebaasis. See on eelkõige oluline antud projekti tehnlises lahenduses.
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="lastRowId"></param>
        /// <returns>Tagastab jututoa sõnumite objektide listi.</returns>
        public static List<ChatMessageBO> GetMessageAfter(int rowId, out int lastRowId)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                lastRowId = 0;
                var m_list = ((from x in db.Chatroom_message
                               orderby x.Chatroom_message_Id descending
                               where x.Chatroom_message_Id > rowId
                               select x).OrderBy(x => x.Chatroom_message_Id)).ToList();

                List<ChatMessageBO> msgs = new List<ChatMessageBO>();
                foreach (var item in m_list)
                {
                    msgs.Add(new ChatMessageBO(item));
                    lastRowId = item.Chatroom_message_Id;
                }
                return msgs;
            }
        }


        /// <summary>
        /// checkNewMsg meetod.
        /// Antud meetod kontrollib kas on laekunud uusi sõnumeid andmebaasi peale sisestatud parameetri indeksit.
        /// </summary>
        /// <param name="lastRow_id"></param>
        /// <returns>Tagastab tõeväärtuse kas on laekunud uusi sõnumeid</returns>
        public static bool checkNewMsg(int lastRow_id)
        {
            using (chatdbEntities db = new chatdbEntities())
            {

                int msgId = (from x in db.Chatroom_message
                             orderby x.Chatroom_message_Id descending
                             select x.Chatroom_message_Id).FirstOrDefault();

                if (msgId == lastRow_id)
                {

                    return false;
                }
                return true;
            }
        }


        /// <summary>
        /// FormatMsgs meetod.
        /// </summary>
        /// <param name="msgs"></param>
        /// <returns>Tagastab listi soovitud vorminguga sõnumitest</returns>
        public static List<String> FormatMsgs(List<ChatMessageBO> msgs)
        {
            List<String> formatedMsgs = new List<String>();
            foreach (var item in msgs)
            {
                formatedMsgs.Add(UserService.FindUserById(item.User_id).Username + "  [" + item.Sent.ToString("yyyy-MM-dd HH:mm") + "] : " + item.Text);
            }
            return formatedMsgs;
        }
    }
}
