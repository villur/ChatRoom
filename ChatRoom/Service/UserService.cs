using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using ChatRoom.Domain;
using ChatRoom.BusinessObject;
using ChatRoom.View;


namespace ChatRoom.Service
{
    /// <summary>
    /// UserServie class. 
    /// Siin klassis asuvad kõik meetodid, mis on seotud kasutajatega.
    /// </summary>
    public class UserService
    {

        /// <summary>
        /// RegisterUser meetod.
        /// Antud meetod registreerib andmebaasi uue kasutaja vastavate parameetrite alusel.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        public static void RegisterUser(string username, string password, string email)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                User registeredUser = new User
                {
                    username = username,
                    password = HashPass(password),
                    email = email,
                    created = DateTime.Now
                };
                db.User.Add(registeredUser);
                db.SaveChanges();
            }
        }


        /// <summary>
        /// HashPass meetod.
        /// Antud meetodi eesmärgiks on luua vastavast sisendist räsi.
        /// Rakendust leiab see meetod näiteks kasutaja parooli salvestusel andmebaasi.
        /// </summary>
        /// <param name="pass"></param>
        /// <returns>tagastab SHA1 algoritmi vorminguga räsi</returns>
        public static string HashPass(string pass)
        {
            string salt = "3.14piper";
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Encoding.Unicode.GetBytes(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            byte[] inarray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inarray);
        }


        /// <summary>
        /// VerifyLogin meetod.
        /// Antud meetod verifitseerib kasutajatunnuse ja parooli.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="userbo"></param>
        /// <returns>Tagastab tõeväärtuse kasutaja ja parooli kohta</returns>
        public static bool VerifyLogin(string user, string password, out UserBO userbo)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                String hashedPassword = HashPass(password);
                var userdata = (from x in db.User
                                where x.username.Equals(user) && x.password.Equals(hashedPassword)
                                select x).FirstOrDefault();

                if (userdata == null)
                {
                    userbo = null;
                    return false;
                }

                userbo = new UserBO(userdata);
                return true;
            }
        }

        /// <summary>
        /// AdminValidation meetod.
        /// Antud meetod verifitseerib kas kasutaja kuulub Admin kasutajagruppi.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Tagastab tõeväärtuse kas vastav kasutajatunnus kuulub Admin kasutajagruppi</returns>
        public static bool AdminValidation(string username)
        {
            using (chatdbEntities db = new chatdbEntities())
            {

                var user_group_data = (from x in db.User
                                       join y in db.User_group on x.user_id equals y.user_id
                                       join z in db.User_group_right on y.user_group_right_id equals z.user_group_right_id
                                       where x.username.Equals(username) && y.user_group_right_id.Value.ToString() == "1"
                                       select x).FirstOrDefault();
                if (user_group_data == null)
                {
                    return false;
                }
                else
                    return true;
            }
        }

        
        /// <summary>
        /// FindUserByUsername meetod.
        /// Universaalne meetod, mis otsib kasutajaobjekti kasutajatunnuse alusel andmebaasist.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Kasutajatunnuse leidmise korral tagastab kasutaja objekti, kui ei leia siis null</returns>
        public static UserBO FindUserByUsername(string username)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var user = (from x in db.User
                            where x.username.Equals(username)
                            select x).FirstOrDefault();
                if (user == null)
                {
                    return null;
                }
                UserBO foundUser = new UserBO(user);
                return foundUser;
            }
        }

        /// <summary>
        /// FindUserById meetod.
        /// Universaalne meetod, mis otsib kasutajaobjekti kasutaja indeksi alusel andmebaasist.
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns>Kasutajatunnuse leidmise korral tagastab kasutaja objekti, kui ei leia siis null</returns>
        public static UserBO FindUserById(int user_id)
        {
            using (chatdbEntities db = new chatdbEntities())
            {
                var user = (from x in db.User
                            where x.user_id.Equals(user_id)
                            select x).FirstOrDefault();
                if (user == null)
                {
                    return null;
                }
                UserBO foundUser = new UserBO(user);
                return foundUser;
            }
        }
    }
}
