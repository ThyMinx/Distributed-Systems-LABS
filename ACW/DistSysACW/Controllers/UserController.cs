using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistSysACW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DistSysACW.Controllers
{
    public class UserController : BaseController
    {

        /// <summary>
        /// A very good example of using CRUD to Create, Read, Update and Delete
        /// </summary>
        /// <param name="ctx"></param>

        public UserController(Models.UserContext ctx) : base(ctx) { }

        [HttpGet]
        [ActionName("new")]
        public IActionResult GetUser([FromQuery] string uname)
        {
            bool userExists = UserDatabaseAccess.userCheck(uname);

            return userExists ? Ok("This user exists.") : Ok("This user does not exist.");
        }

        [HttpPost]
        [ActionName("new")]
        public IActionResult CreateNewUser([FromBody]string user)
        {
            if (!UserDatabaseAccess.userCheck(user))
                return Ok(UserDatabaseAccess.NewUser(user));
            else if (UserDatabaseAccess.userCheck(user))
                return StatusCode(StatusCodes.Status403Forbidden, "This user name has been taken. Try something else!");
            else
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        [HttpDelete]
        [ActionName("RemoveUser")]
        [Authorize(Roles = "Admin,User")]
        public ActionResult RemoveUser(string uname, [FromHeader]string key)
        {
            if (UserDatabaseAccess.userCheck(uname))
            {
                if (UserDatabaseAccess.userApi(key, uname))
                {
                    if (UserDatabaseAccess.deleteUser(uname))
                        return Ok(true);
                    else
                        return Ok(false);
                }
                else
                {
                    return Ok("You cannot delete a user that is not yourself.");
                }
            }
            else
            {
                return Ok("There is no user with that username.");
            }
        }

        [HttpPost]
        [ActionName("ChangeRole")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateUser([FromHeader]string key, [FromBody] User user)
        {
            var values = new Dictionary<string, string>
            {
                {user.uname,user.role}
            };

            var userN = _context.Users.Where(x => x.uname == user.uname).FirstOrDefault();

            string msg = "";

            if (UserDatabaseAccess.keyCheck(key))
            {
                if (UserDatabaseAccess.userCheck(user.uname))
                {
                    if (userN.role.Equals("Admin") || (userN.role.Equals("User")))
                    {
                        if (userN.role != user.role)
                        {
                            userN.role = user.role;
                            _context.SaveChanges();
                            return Ok("DONE ");
                        }
                        else
                        {
                            msg = "This user cannot change role to be the same role";
                        }
                    }
                    else
                    {
                        msg = "Role doesn't exist";
                    }

                }
                else
                {
                    msg = "User doesn't exist";
                }
            }
            else
            {
                msg = "An unkown error occured";
            }

            return BadRequest(msg);

        }
    }
}