using Microsoft.AspNetCore.Mvc;
using AuthorizationsSso.Models;
using Newtonsoft.Json;
using AuthorizationsSso.Helpers;

namespace AuthorizationsSso.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authentication : ControllerBase
    {
        private IConfiguration _config;
        public Authentication(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Get(Guid client_id, string redirect_uri, string state, Guid authUnique, string response_type, string approval_prompt)
        {
            LogHelper.WriteToLogFile($"api/Authentication: client_id={client_id} redirect_uri={redirect_uri} state={state} authUnique={authUnique}");
            try
            {
                using (var db = new AuthDbContext())
                {
                    if (!db.AuthorizationAppClients.Any(r => r.ClientId == client_id && redirect_uri.Contains(r.RedirectUri)))
                        return BadRequest(new { state });

                    var authorization = db.Authorizations.SingleOrDefault(r => r.Id == authUnique);

                    if (authorization == null)
                        return BadRequest("В базе данных отсутствует запись с authUnique = " + authUnique);

                    var accessCode = AccessCodeHelper.GenerateAccessCode();

                    authorization.AccessCode = accessCode;

                    AccessToken accessToken = new AccessToken
                    {
                        state = state,
                        code = accessCode
                    };

                    db.SaveChanges();

                    string url = "";
                    if (!redirect_uri.Contains("http"))
                        url = "http://";

                    url += redirect_uri.Contains("back") ? redirect_uri + "&state=" + state + "&code=" + accessCode : redirect_uri + "?state=" + state + "&code=" + accessCode;
                    
                    LogHelper.WriteToLogFile(url);

                    return Redirect(url);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, JsonConvert.SerializeObject(ex));
            }
        }
    }

    public class UserCredentials
    {
        public Guid client_id { get; set; }
        public string state { get; set; }
        public string response_type { get; set; }
        public string approval_prompt { get; set; }
        public string redirect_uri { get; set; }
        public Guid authUnique { get; set; }
    }

    public class AccessToken
    {
        public string state { get; set; }
        public string code { get; set; }
    }
}