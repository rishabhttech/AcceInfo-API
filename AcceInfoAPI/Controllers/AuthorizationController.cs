using Common.Helper;
using Common.Models;
using Common.Models.Request;
using Common.Models.Response.Common.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthorizationController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _auth = new Common.Query.Auth();
            _masterList = new Common.Query.Account();
            _httpContextAccessor = httpContextAccessor;
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
                    var contactId = _httpContextAccessor.HttpContext?.User?.FindFirst("contactId")?.Value;
                    var refreshtokenresponse = authHelper.RefreshJwtToken(contactId, loginRequest.RefreshToken, loginRequest.Username, _configuration["Jwt:Key"], _configuration["Jwt:Issuer"], _configuration["Jwt:Audience"]);

                    return Ok(new Common.Models.Response.LoginResponse
                    {
                        statusCode = System.Net.HttpStatusCode.OK,
                        Status = refreshtokenresponse.Status,
                        Message = refreshtokenresponse.Message,
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
                    
                    var tokenresponse = authHelper.GenerateJwtToken(employee.UserName, employee.ContactId, _configuration["Jwt:Key"], _configuration["Jwt:Issuer"], _configuration["Jwt:Audience"]);

                    return Ok(new Common.Models.Response.LoginResponse
                    {
                        statusCode = System.Net.HttpStatusCode.OK,
                        Status = tokenresponse.Status,
                        Message = Constants.LOGIN_SUCCESSFULLY,
                        Name = employee.UserName,
                        Token = (string)tokenresponse.Token,
                        RefreshToken = tokenresponse.RefreshToken,
                        ExpiresIn = tokenresponse.ExpiresIn,
                        ContactId = (string)employee?.ContactId,
                        FirstName = (string)employee?.FirstName,
                        LastName = (string)employee?.LastName,
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
        public async Task<IActionResult> VerifyOtp([FromBody] Common.Models.Request.OtpRequest otpRequest)
        {
            if (otpRequest.otp == "123456")
            {
                var contactId = _httpContextAccessor.HttpContext?.User?.FindFirst("contactId")?.Value;
                var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);

                var employee = await db.QuerySingleAsync<dynamic>(_auth.ContactDetails, new
                {
                    CustomerId = contactId,
                });

                return Ok(new
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    MobileNumber = employee.MobileNumber,
                    DOB = employee.DOB,
                    Status = Constants.SUCCESS_STATUS,
                    Statuscode = HttpStatusCode.OK
                });
            }
            else
            {
                return Ok(new
                {
                    status = Constants.FAILED_STATUS,
                    message = Constants.TOKEN_GENERATED_FAILED
                });
            }
        }

        [Authorize]
        [HttpPost("user/details")]
        public async Task<IActionResult> GetUserDetails()
        {
            try
            {
                var contactId = _httpContextAccessor.HttpContext?.User?.FindFirst("contactId")?.Value;
                var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);

                var employee = await db.QuerySingleAsync<dynamic>(_auth.ContactDetails, new
                {
                    CustomerId = contactId,
                });

                return Ok(new
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    MobileNumber = employee.MobileNumber,
                    DOB = employee.DOB,
                    Status = Constants.SUCCESS_STATUS,
                    Statuscode = HttpStatusCode.OK
                });
            }
            catch(Exception ex)
            {
                return Unauthorized(new { Status = Constants.FAILED_STATUS, Statuscode = HttpStatusCode.Unauthorized });
            }
            

        }

        [Authorize]
        [HttpPost("user/account-transaction-history")]
        public async Task<IActionResult> GetTransactionHistoryByAccountId([FromBody] TransactionHistoryRequest request)
        {
            try
            {
                //var username = User.Identity?.Name;
                //if (string.IsNullOrEmpty(username))
                //{
                //    return Unauthorized(new
                //    {
                //        Status = Constants.FAILED_STATUS,
                //        Message = "Unauthorized access"
                //    });
                //}

                if (string.IsNullOrEmpty(request.AccountId))
                {
                    return BadRequest(new
                    {
                        Status = Constants.ERROR_STATUS,
                        Message = "AccountId is required."
                    });
                }

                var db = new DBConnectionHelper(_configuration, _configuration[Constants.DB_CONNECTIONSTRING]);

                var transactions = (await db.QueryAsync<dynamic>(_masterList.GetTransactionHistoryByAccountId, new
                {
                    AccountId = request.AccountId
                })).ToList();

                var result = transactions.Select(t => new TransactionHistoryResponse
                {
                    TransactionId = (string)t.TransactionId,
                    TransactionFrom = (int)t.TransactionFrom,
                    TransactionTo = (int)t.TransactionTo,
                    CreatedOn = (DateTime)t.CreatedOn,
                    Amount = (int)t.Amount,
                    Note = (string)t.Note,
                    TransactionType = (string)t.TransactionType,
                    IsSelfTransfer = (bool)t.IsSelfTransfer
                });

                return Ok(new
                {
                    Status = Constants.SUCCESS_STATUS,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = Constants.FAILED_STATUS,
                    Message = "An error occurred while retrieving transaction history"
                });
            }
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
