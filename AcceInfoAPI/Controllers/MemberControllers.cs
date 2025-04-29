using Common.Models;
using Common.Models.Request;
using Common.Models.Response;
using Common.Query;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Principal;
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
        private Common.Query.Customer _customer;

        public MemberControllers(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _auth = new Common.Query.Auth();
            _customer = new Common.Query.Customer();
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
                string ContactRoleJnId = string.Empty, ReceipientId = string.Empty;
                try
                {
                    ReceipientId = await conn.ExecuteScalarAsync<string>(_masterList.insertRecipientSql, new
                    {
                        Name = memberRequest.Name,
                        Email = memberRequest.Email,
                        ContactNumber = memberRequest.ContactNumber,
                        IstransferByEmail = memberRequest.IstransferByEmail,
                        IstransferByMobile = memberRequest.IstransferByMobile,
                        PrefLanguage = memberRequest.PrefLanguage,
                        NickName = memberRequest.NickName,
                        Contact = contactId
                    }, trx);


                    ContactRoleJnId = await conn.ExecuteScalarAsync<string>(_masterList.insertContactRoleJnSql, new
                    {
                        ContactId = contactId,
                        RoleId = Constants.ROLE_MEMBER_VALUE,
                        MemberId = ReceipientId
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
                    Status = Constants.FAILED_STATUS,
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
                    Name = (string)x.Name,
                    IstransferByEmail = (bool)x.IstransferByEmail,
                    IstransferByMobile = (bool)x.IstransferByMobile,
                    Email = (string)x.Email,
                    PreferredLanguage = (string)x.PrefLanguage,
                    NickName = (string)x.NickName,
                    MobileNumber = (string)x.ContactNumber,
                    AccountNumber = (string)x.AccountNumber,
                    MemberId = (string)x.RecipientId,
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
        }
        [Authorize]
        [HttpGet("get-payeecategories")]
        public async Task<IActionResult> GetPayeeCategories()
        {
            var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);
            var GetPayeeCategoryQuery = (await db.QueryAsync<dynamic>(_masterList.GetPayeeCategoriesQuery)).ToList();
            var PayeeCategoryListQueryList = GetPayeeCategoryQuery.Select(x => new
            {
                Name = (string)x.Name,
                PayeeTypeId = (string)x.PayeeTypeId
            });

            return Ok(
                new Common.Models.ResponseModel
                {
                    statusCode = HttpStatusCode.OK,
                    Status = Constants.SUCCESS_STATUS,
                    Data = PayeeCategoryListQueryList,
                    Message = Constants.DATA_FOUND_SUCCESSFULLY
                });
        }
        [Authorize]
        [HttpPost("payee-add")]
        public async Task<IActionResult> AddPayee([FromBody] Common.Models.Request.AddPayeeRequest payeeRequest)
        {
            var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);
            using var conn = await db.GetOpenConnectionAsync();
            using var trx = conn.BeginTransaction();
            var contactId = _httpContextAccessor.HttpContext?.User?.FindFirst("contactId")?.Value;
            var ContactRoleJnId = await conn.ExecuteScalarAsync<string>(_masterList.AddPayeeQuery, new
            {
                PayeeName = payeeRequest.PayeeName,
                PayeeNumber = payeeRequest.PayeeNumber,
                PayeeType = payeeRequest.PayeeType,
                ContactId = contactId
            }, trx);

            await trx.CommitAsync();

            return Ok(
                new Common.Models.ResponseModel
                {
                    statusCode = HttpStatusCode.OK,
                    Status = Constants.SUCCESS_STATUS,
                    Data = ContactRoleJnId,
                    Message = Constants.DATA_FOUND_SUCCESSFULLY
                });
        }
        [Authorize]
        [HttpGet("getpayeelist")]
        public async Task<IActionResult> GetPayeeByContact()
        {
            try
            {
                var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);
                var contactId = _httpContextAccessor.HttpContext?.User?.FindFirst("contactId")?.Value;
                var GetPayeeListQuery = (await db.QueryAsync<dynamic>(_masterList.GetPayeeByContact, new { ContactId = contactId })).ToList();
                var PayeeListQueryList = GetPayeeListQuery.Select(x => new
                {
                    PayeeName = (string)x.PayeeName,
                    PayeeNumber = (string)x.PayeeNumber,
                    PayeeType = (string)x.PayeeType,
                    PayeeId = (string)x.PayeeId,
                    PayeeTypeName = (string)x.PayeeTypeName
                });
                if (PayeeListQueryList != null && PayeeListQueryList.Count() > 0)
                {
                    return Ok(
                    new Common.Models.ResponseModel
                    {
                        statusCode = HttpStatusCode.OK,
                        Status = Constants.SUCCESS_STATUS,
                        Data = PayeeListQueryList,
                        Message = Constants.DATA_FOUND_SUCCESSFULLY
                    });
                }
                else
                {
                    return Ok(
                    new Common.Models.ResponseModel
                    {
                        statusCode = HttpStatusCode.OK,
                        Status = Constants.SUCCESS_STATUS,
                        Message = Constants.DATA_NOT_FOUND
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new Common.Models.ResponseModel
                    {
                        statusCode = HttpStatusCode.InternalServerError,
                        Status = Constants.FAILED_STATUS,
                        Message = Constants.ERROR_STATUS
                    });
            }
        }
        [Authorize]
        [HttpPost("AddCustomer")]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerRequestModel request)
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
            using var conn = await db.GetOpenConnectionAsync();
            using var trx = conn.BeginTransaction();

            try
            {

                var UserId = _httpContextAccessor.HttpContext?.User?.FindFirst("contactId")?.Value;
                string CardNumber = Common.Helper.AuthHelper.Generate16DigitCardNumber();
                var Customer = await conn.ExecuteScalarAsync<string>(_customer.insertCustomerQuery, new
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Password = request.Password,
                    UserName = request.UserName,
                    Email = request.Email,
                    MobileNumber = request.MobileNumber,
                    CreatedBy = UserId,
                    CardNumber = CardNumber
                }, trx);

                var CustomerRoleJn = await conn.ExecuteScalarAsync<string>(_masterList.insertCustomerRoleJnSql, new
                {
                    ContactId = Customer,
                    RoleId = Constants.CUSTOMER_TYPE_ID
                }, trx);


                // Generate account number and IDs
                string accountNumber = Common.Helper.AuthHelper.Generate13DigitAccountNumber();
                
                // Insert account
                var newAccountId = await conn.ExecuteScalarAsync<string>(_masterList.insertAccountSql, new
                {
                    AccountNumber = accountNumber,
                    Balance = 50000.00,
                    Status = true,
                    AccountCategory = Constants.ACCOUNT_CHEQUING_ID,
                    Name = request.FirstName + " Primary Account"
                }, trx);

                // Link customer to account
                var contactAccountJoinId = await conn.ExecuteScalarAsync<string>(_masterList.insertCustomerAccountSql, new
                {
                    AccountId = newAccountId,
                    CustomerId = Customer,
                    Status = true
                }, trx);

                await trx.CommitAsync();

                return Ok(new
                {
                    status = "success",
                    message = "Customer and Account created successfully.",
                    contactId = Customer,
                    accountId = newAccountId
                });
            }
            catch (Exception ex)
            {
                await trx.RollbackAsync();
                return StatusCode(500, new
                {
                    status = Constants.ERROR_STATUS,
                    message = Constants.CUSTOMER_NOT_CREATED,
                    error = ex.Message
                });
            }
        }

        [Authorize]
        [HttpGet("GetCustomer")]
        public async Task<IActionResult> GetCustomerList()
        {
            try
            {
                var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);

                var AccountTypeListQuery = (await db.QueryAsync<dynamic>(_customer.GetCustomer, new { RoleId = Constants.CUSTOMER_TYPE_ID }));

                var AccountTypeListQueryList = AccountTypeListQuery.Select(x => new Common.Models.Response.ContactResponseModel
                {
                    FirstName = (string)x.FirstName,
                    LastName = (string)x.LastName,
                    MobileNumber = (string)x.MobileNumber,
                    Email = (string)x.Email,
                    UserName = (string)x.UserName,
                    CardNumber = (string)x.CardNumber,
                    ContactId = (string)x.ContactId

                });

                if (AccountTypeListQuery != null)
                {
                    return Ok(new
                    {
                        Status = Constants.SUCCESS_STATUS,
                        Data = AccountTypeListQuery
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Status = Constants.FAILED_STATUS,
                        Message = "Customer Not Found"
                    });
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    status = Constants.ERROR_STATUS,
                    message = Constants.DATA_NOT_FOUND,
                    error = ex.Message
                });
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
