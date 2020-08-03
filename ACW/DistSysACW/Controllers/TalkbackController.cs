using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DistSysACW.Controllers
{
    public class TalkBackController : BaseController
    {
        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public TalkBackController(Models.UserContext context) : base(context) { }


        [ActionName("Hello")]
        public ActionResult Get()
        {
            #region TASK1
            // TODO: add api/talkback/hello response

            return Ok("Hello World");
            #endregion
        }

        [ActionName("Sort")]
        public ActionResult getInts([FromQuery]int[] integers)
        {
            #region TASK1

            // TODO: 
            // sort the integers into ascending order
            // send the integers back as the api/talkback/sort response
            try
            {
                Array.Sort(integers);
                return Ok(integers);

            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            #endregion
        }
    }
}
