using DistSysACW.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DistSysACW.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, Models.UserContext dbContext)
        {
            #region Task5
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then set the correct roles for the User, using claims

            var key = context.Request.Headers["ApiKey"];
            if (UserDatabaseAccess.keyCheck(key))
            {
                Models.UserContext userCtx = new Models.UserContext();

                ///op = operator
                ///des = designation
                string op = null;
                string des = null;
                var _users = userCtx.Users;

                foreach (Models.User user in _users)
                {
                    op = user.uname;
                    des = user.role;
                }

                var vals = dbContext.Users.First(x => x.key == key);
                var name = vals.uname;

                List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, vals.uname),
                new Claim(ClaimTypes.NameIdentifier, key),
                new Claim(ClaimTypes.Role, vals.role),
                new Claim(ClaimTypes.Authentication,key)
                };
                
                var claim_identity = new ClaimsIdentity(claims);
                var claim_principals = new ClaimsPrincipal(claim_identity);
                context.User = claim_principals;
            }

            #endregion

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

    }
}
