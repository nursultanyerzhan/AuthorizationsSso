using Microsoft.AspNetCore.Mvc;
using AuthorizationsSso.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using AuthorizationsSso.Helpers;

namespace AuthorizationsSso.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GetIIN : ControllerBase
    {
        private IConfiguration _config;
        public GetIIN(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                using (var db = new AuthDbContext())
                {
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    
                    var authorization = db.Authorizations.SingleOrDefault(r => r.AccessToken == accessToken);

                    if (authorization == null)
                        return BadRequest(new { error = "Row not found, token: " + accessToken });

                    LogHelper.WriteToLogFile("api/GetIIN: iin=" + authorization.Iin);

                    return Ok(new { iin = authorization.Iin });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "У вас отсутствует ИИН В базе" });
            }
        }
    }
}