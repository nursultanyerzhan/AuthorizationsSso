using AuthorizationsSso.Helpers;
using AuthorizationsSso.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using System.Text;


namespace AuthorizationsSso.Controllers
{
    [ApiController]
    // [Authorize]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private IConfiguration _config;
        public TestController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("GetArchives")]
        public async Task<IActionResult> GetArchives()
        {
            // string code = ""; 
            // for(int i =0; i < 10; i++)
            // {
            //     code += Guid.NewGuid().ToString();
            // }
            // code = code.Replace("-", "");

            var code = AccessCodeHelper.GenerateAccessCode();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return Ok(code);
        }

        [HttpGet]
        [Route("GetHash")]
        public IActionResult GetHash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                string hashedValue = builder.ToString();

                return Ok(hashedValue);
            }
        }

        [HttpGet]
        [Route("SendMessage")]
        public IActionResult SendMessage(string message)
        {
            return Redirect("http://192.168.60.31/Testers/AcceptMessage?message=" + message);
        }
        
        [HttpGet]
        [Route("GetJson")]
        public IActionResult GetJson(string message)
        {
            return Ok(message + "15515");
        }
        
        [HttpGet]
        [Route("WriteToTable")]
        public IActionResult WriteToTable(string message)
        {
            LogHelper.WriteToLogFile(message);
            return Ok(message);
        }
        
    }
}
