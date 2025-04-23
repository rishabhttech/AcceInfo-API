using Common.Models;
using Common.Query;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Xml.Linq;

namespace AcceInfoAPI.Controllers
{

    [Route("api/member")]
    public class MemberControllers : Controller
    {
        private readonly IConfiguration _configuration;
        private Common.Query.Auth _auth;
        private Common.Query.Account _masterList;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MemberControllers(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _auth = new Common.Query.Auth();
            _masterList = new Common.Query.Account();
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddMember([FromBody] Common.Models.Request.AddMemberRequest memberRequest)
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
            var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);
            var Member = await db.QuerySingleAsync<dynamic>(_masterList.CheckIfMemberExistbyEmail, new
            {
                Email = memberRequest.Email,
            });
            string? MemberId = Member?.ContactId?.ToString();

            if (!string.IsNullOrEmpty(MemberId))
            {
                using var conn = await db.GetOpenConnectionAsync();
                using var trx = conn.BeginTransaction();
                var contactId = _httpContextAccessor.HttpContext?.User?.FindFirst("contactId")?.Value;
                string ContactRoleJnId = string.Empty;
                try
                {
                    ContactRoleJnId = await conn.ExecuteScalarAsync<string>(_masterList.insertContactRoleJnSql, new
                    {
                        ContactId = contactId,
                        RoleId = Constants.ROLE_MEMBER_VALUE,
                        MemberId = MemberId
                    }, trx);

                    await trx.CommitAsync();

                    return Ok(new ResponseModel
                    {
                        statusCode = HttpStatusCode.OK,
                        Status = Constants.SUCCESS_STATUS,
                        Message = Constants.MEMBER_ADDED_SUCCESSFULLY
                    });
                }
                catch (Exception ex)
                {
                    await trx.RollbackAsync();
                    return BadRequest(new Common.Models.ResponseModel
                    {
                        Status = Constants.FAILED_STATUS,
                        statusCode = HttpStatusCode.BadRequest,
                        Message = Constants.MEMBER_NOT_CREATED
                    });
                }
            }
            else
            {
                return Ok(new Common.Models.ResponseModel
                {
                    statusCode = HttpStatusCode.OK,
                    Status = Constants.SUCCESS_STATUS,
                    Message = Constants.ACCOUNT_NOT_FOUND
                });
            }
        }
        [Authorize]
        [HttpGet("get-list")]
        public async Task<IActionResult> GetMemberList()
        {
            var contactId = _httpContextAccessor.HttpContext?.User?.FindFirst("contactId")?.Value;
            if (!string.IsNullOrEmpty(contactId))
            {
                var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);
                var AccountTypeListQuery = (await db.QueryAsync<dynamic>(_masterList.GetMemberListOfContact, new { ContactId = contactId })).ToList();
                var AccountTypeListQueryList = AccountTypeListQuery.Select(x => new 
                {
                    FirstName = (string)x.FirstName,
                    LastName = (string)x.LastName,
                    Email = (string)x.Email,
                    PreferredLanguage = (string)x.Language,
                    NickName = (string)x.NickName,
                    SendTransferedBy = (string)x.SendTransferBy,
                    MobileNumber = (string)x.MobileNumber,
                    AccountNumber = (string)x.AccountNumber,
                    MemberId = (string)x.MemberId,
                    AccountId = (string)x.AccountId,
                    AccountName = (string)x.AccountName
                });
                return Ok(new Common.Models.ResponseModel {
                    statusCode = HttpStatusCode.OK,
                    Status = Constants.SUCCESS_STATUS,
                    Data = AccountTypeListQueryList,
                    Message = Constants.DATA_FOUND_SUCCESSFULLY
                });
            }
            else
            {
                return Ok(new Common.Models.ResponseModel
                {
                    statusCode = HttpStatusCode.BadRequest,
                    Status = Constants.FAILED_STATUS,
                    Message = Constants.ENTER_VALID_MEMBERID
                });
            }
                return Ok();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
