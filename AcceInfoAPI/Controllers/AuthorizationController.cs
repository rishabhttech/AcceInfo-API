using Common.Helper;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AcceInfoAPI.Controllers
{
    [Route("api")]
    public class AuthorizationController : Controller
    {
        private readonly IConfiguration _configuration;
        private Common.Query.Auth _auth;
        private Common.Query.Account _masterList;
        public AuthorizationController(IConfiguration configuration)
        {
            _configuration = configuration;
            _auth = new Common.Query.Auth();
            _masterList = new Common.Query.Account();
        }

        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] Common.Models.Request.LoginRequest loginRequest)
        {
            AuthHelper authHelper = new AuthHelper(_configuration);
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                           .SelectMany(v => v.Errors)
                           .Select(e => e.ErrorMessage)
                           .ToList();
                    return BadRequest(new Common.Models.ResponseModel
                    {
                        Status = Common.Models.Constants.ERROR_STATUS,
                        Message = Common.Models.Constants.MISSING_FIELDS,
                        statusCode = HttpStatusCode.BadRequest,
                        ErrorMessages = errors
                    });
                }
                if(!string.IsNullOrEmpty(loginRequest.RefreshToken))
                {
                    var refreshtokenresponse = authHelper.RefreshJwtToken(loginRequest.RefreshToken, loginRequest.Username, _configuration["Jwt:Key"], _configuration["Jwt:Issuer"], _configuration["Jwt:Audience"]);

                    return Ok(new Common.Models.Response.LoginResponse
                    {
                        statusCode = System.Net.HttpStatusCode.OK,
                        Status = refreshtokenresponse.Status,
                        Message = Constants.LOGIN_SUCCESSFULLY,
                        Name = loginRequest.Username,
                        Token = (string)refreshtokenresponse.Token,
                        RefreshToken = refreshtokenresponse.RefreshToken,
                        ExpiresIn = refreshtokenresponse.ExpiresIn
                    });
                }
                var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);
                
                var employee = await db.QuerySingleAsync<dynamic>(_auth.DoLogin, new
                {
                    UserName = loginRequest.Username,
                    Password = loginRequest.Password,
                    Role = loginRequest.Type.ToLower()
                });

                if (employee != null)
                {
                    
                    var tokenresponse = authHelper.GenerateJwtToken(loginRequest.Username, _configuration["Jwt:Key"], _configuration["Jwt:Issuer"], _configuration["Jwt:Audience"]);

                    return Ok(new Common.Models.Response.LoginResponse
                    {
                        statusCode = System.Net.HttpStatusCode.OK,
                        Status = tokenresponse.Status,
                        Message = Constants.LOGIN_SUCCESSFULLY,
                        Name = employee.UserName,
                        Token = (string)tokenresponse.Token,
                        RefreshToken = tokenresponse.RefreshToken,
                        ExpiresIn = tokenresponse.ExpiresIn
                    });
                }
                return Unauthorized(new Common.Models.Response.LoginResponse
                {
                    Status = Constants.FAILED_STATUS,
                    statusCode = System.Net.HttpStatusCode.Unauthorized,
                    Message = Constants.LOGIN_FAILED
                });
            }
            catch(Exception ex)
            {
                return Unauthorized(new Common.Models.Response.LoginResponse
                {
                    Status = Constants.FAILED_STATUS,
                    statusCode = System.Net.HttpStatusCode.Unauthorized,
                    Message = Constants.LOGIN_FAILED
                });
            }
           
            
        }

        
        [HttpGet("accounts/master")]
        public async Task<IActionResult> GetAccountMasterList()
        {
            var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);

            var AccountTypeListQuery = (await db.QueryAsync<dynamic>(_masterList.AccountTypeList)).ToList();
            var AccountTypeListQueryList = AccountTypeListQuery.Select(x => new Common.Models.Response.AccountCategoriesResponse
            {
                Name = (string)x.Name,
                AccountCategoryId = (string)x.AccountCategoryId
            });


            return Ok(new
            {
                Status = Constants.SUCCESS_STATUS,
                Data = AccountTypeListQueryList
            });
        }

        [HttpPost("auth/otp-verify")]
        public IActionResult VerifyOtp([FromBody] Common.Models.Request.OtpRequest otpRequest)
        {
            if (otpRequest.otp == "123456")
            {
                return Ok(new
                {
                    status = "success",
                    message = "OTP verified successfully."
                });
            }
            else
            {
                return Ok(new
                {
                    status = "failed",
                    message = "Invalid OTP."
                });
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
