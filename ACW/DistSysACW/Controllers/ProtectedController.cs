using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoreExtensions;
using DistSysACW.CoreExtensions;
using DistSysACW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DistSysACW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProtectedController : BaseController
    {
        public ProtectedController(Models.UserContext ctx) : base(ctx) { }

        [HttpGet]
        [ActionName("Hello")]
        public ActionResult protectedHello([FromHeader] string key)
        {
            try
            {
                if (UserDatabaseAccess.keyCheck(key))
                    return Ok("Hello " + User.Identity.Name);
                else
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }

        [HttpGet]
        [ActionName("sha1")]
        public ActionResult protectedSHA1([FromHeader]string key, string message)
        {
            try
            {
                if (String.IsNullOrEmpty(message))
                {
                    if (UserDatabaseAccess.keyCheck(key))
                    {
                        byte[] buffer = Encoding.ASCII.GetBytes(message);
                        var sha1 = SHA1.Create();
                        var hash = sha1.ComputeHash(buffer);
                        return Ok(hash);
                    }
                    else
                        return Ok();
                }
                else
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }


        }

        [HttpGet]
        [ActionName(("sha256"))]
        public ActionResult protectedSHA256([FromHeader]string key, string message)
        {
            try
            {
                if (String.IsNullOrEmpty(message))
                {
                    if (UserDatabaseAccess.keyCheck(key))
                    {
                        byte[] buffer = Encoding.ASCII.GetBytes(message);
                        var sha = SHA256.Create();
                        var hash = sha.ComputeHash(buffer);
                        return Ok(hash);
                    }
                    else
                        return Ok();
                }
                else
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }

        [HttpGet]
        [ActionName("getpublickey")]
        public ActionResult publicKey([FromHeader]string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("Couldn't get the public key");
            if (UserDatabaseAccess.keyCheck(key))
            {

                byte[] dataToChange = Encoding.ASCII.GetBytes(key);
                byte[] encrpyt;
                byte[] decryptData;
                RSAParameters publicKey;
                RSAParameters privateKey;
                string str;
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.PersistKeyInCsp = true;
                    publicKey = rsa.ExportParameters(false);
                    privateKey = rsa.ExportParameters(true);

                    encrpyt = RSAInternal.RSAEncrypt(dataToChange, publicKey);
                    str = RSACryptoExtensions.ToXmlStringCore22(rsa, false);
                    decryptData = RSAInternal.RSADecrypt(encrpyt, privateKey);
                    RSACryptoExtensions.FromXmlStringCore22(rsa, str);
                }
                return Ok(str);
            }
            else
            {
                return Ok("ApiKey invalid");
            }
        }
    }
}