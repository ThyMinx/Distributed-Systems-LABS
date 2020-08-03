using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DistSysACW.Models
{
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key 
        [Key]
        public string key { get; set; }
        public string uname { get; set; }
        public string role { get; set; }
        
        public User()
        {

        }
        //public ICollection<Log> Logs { get; set; }
        #endregion
    }

    #region Task13?
    // TODO: You may find it useful to add code here for Logging
    #endregion

    public static class UserDatabaseAccess
    {
        #region Task3 
        // TODO: Make methods which allow us to read from/write to the database 

        public static string NewUser(string puser)
        {
            var ctx = new UserContext();
            if (ctx.Users.Any(x => x.uname == puser)) //Check if username is in use
                return "User Exists";
            else
            {
                //If there is no admin user the next user registered will become the admin
                if (ctx.Users.Any(x => x.role == "Admin"))
                {
                    User newUser = new User()
                    {
                        uname = puser,
                        key = Guid.NewGuid().ToString(),
                        role = "User"
                    };
                    ctx.Users.Add(newUser);

                    ctx.SaveChanges(); //ALWAYS REMEMBER THIS FFS
                    return newUser.key;
                }
                else
                {
                    User newUser = new User()
                    {
                        uname = puser,
                        key = Guid.NewGuid().ToString(),
                        role = "Admin"
                    };
                    ctx.Users.Add(newUser);
                    ctx.SaveChanges(); //ALWAYS REMEMBER THIS FFS
                    return newUser.key;
                }
            }

        }

        public static bool userCheck(string puser)
        {
            var ctx = new UserContext();
            return ctx.Users.Any(x => x.uname == puser) ? true : false;
        }

        public static bool keyCheck(string api)
        {
            var ctx = new UserContext();
            return ctx.Users.Any(x => x.key == api) ? true : false;
        }

        public static bool userApi(string api, string puser)
        {
            var ctx = new UserContext();
            var u = ctx.Users.Where(x => x.key == api).FirstOrDefault();
            return u.uname == puser ? true : false;
        }

        public static bool deleteUser(string puser)
        {
            var ctx = new UserContext();
            var u = ctx.Users
                .Where(x => x.uname == puser)
                .FirstOrDefault();
            ctx.Remove(u);
            ctx.SaveChanges();
            return !userCheck(puser) ? true : false;
        }
        #endregion
    }


}