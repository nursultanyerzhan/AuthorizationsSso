using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using AuthorizationsSso.Models;
using Newtonsoft.Json;
using AuthorizationsSso.Helpers;

namespace AuthorizationsSso.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authorization : ControllerBase
    {
        private IConfiguration _config;
        public Authorization(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                using (var db = new AuthDbContext())
                {
                    IFormCollection form = await Request.ReadFormAsync();
                    string code = form["code"];

                    LogHelper.WriteToLogFile("api/Authorization: code=" + code);

                    if (string.IsNullOrEmpty(code))
                        return BadRequest(new { error = "code пустой" });

                    var authorization = db.Authorizations.SingleOrDefault(r => r.AccessCode == code);

                    if (authorization == null)
                        return BadRequest(new { error = "code неправильный" + code });

                    // if (authorization.CreatedDate.AddMinutes(5) < DateTime.Now)
                    //     return BadRequest(new { error = "Срок действии code истек" });

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
                      _config["Jwt:Issuer"],
                      null,
                      expires: DateTime.Now.AddMinutes(1),
                      signingCredentials: credentials);

                    var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
                    authorization.AccessToken = token;
                    authorization.RefreshToken = AccessCodeHelper.GenerateAccessCode();

                    db.SaveChanges();

                    var dtoToken = new DtoToken
                    {
                        access_token = authorization.AccessToken,
                        refresh_token = authorization.RefreshToken,
                        expires_in = TimeHelper.GetTimeStamp(DateTime.Now.AddMinutes(5)),
                        expires = TimeHelper.GetTimeStamp(DateTime.Now.AddHours(3))
                    };

                    return Ok(dtoToken);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, JsonConvert.SerializeObject(ex));
            }
        }

        [HttpPost]
        [Route("GetNewAccessToken")]
        public async Task<IActionResult> GetNewAccessToken()
        {
            IFormCollection form = await Request.ReadFormAsync();
            string? refreshToken = form["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("refresh_token пустой!");

            using (var db = new AuthDbContext())
            {
                if (!db.Authorizations.Any(r => r.RefreshToken == refreshToken))
                    return Ok("refresh_token token неактуальный!");

                var authorization = db.Authorizations.Single(r => r.RefreshToken == refreshToken);
                if (authorization.CreatedDate.AddHours(2) < DateTime.Now)
                    return Ok("refresh_token token просрочен");

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
                  _config["Jwt:Issuer"],
                  null,
                  expires: DateTime.Now.AddMinutes(1),
                  signingCredentials: credentials);

                var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

                authorization.CreatedDate = DateTime.Now;
                authorization.AccessToken = token;
                authorization.AccessCode = null;
                authorization.RefreshToken = AccessCodeHelper.GenerateAccessCode();

                db.SaveChanges();

                var dtoToken = new DtoToken
                {
                    access_token = authorization.AccessToken,
                    refresh_token = authorization.RefreshToken,
                    expires_in = TimeHelper.GetTimeStamp(DateTime.Now.AddMinutes(5)),
                    expires = TimeHelper.GetTimeStamp(DateTime.Now.AddHours(3))
                };

                return Ok(dtoToken);
            }
        }
    }

    public class DtoToken
    {
        public string? access_token { get; set; }
        public string? resource_owner_id { get; set; }
        public string? refresh_token { get; set; }
        public long expires_in { get; set; }
        public long expires { get; set; }
    }

    public class DtoAuthorization
    {
        public string code { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string redirect_uri { get; set; }
        public string grant_type { get; set; }
    }
}