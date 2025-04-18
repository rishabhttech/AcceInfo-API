using Common.Helper;
using Common.Models;
using Common.Models.Request;
using Common.Query;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AcceInfoAPI.Controllers
{
    [Route("api/accounts")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private Common.Query.Account _account;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
            _account = new Common.Query.Account();
        }

        [HttpPost("add")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
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
                string accountNumber = Common.Helper.AuthHelper.Generate13DigitAccountNumber();
                string AccountId = Guid.NewGuid().ToString();
                string CustomerAccountJnId = Guid.NewGuid().ToString();
                

                var accountId = await conn.ExecuteScalarAsync<string>(_account.insertAccountSql, new
                {
                    AccountNumber = accountNumber,
                    Balance = request.Amount,
                    Status = true,
                    AccountCategory = request.AccountType
                }, trx);
                var contactAccountJnId = await conn.ExecuteScalarAsync<Guid>(_account.insertCustomerAccountSql, new
                {
                    AccountId = accountId,
                    CustomerId = request.ContactId,
                    Status = true,
                }, trx);

                await trx.CommitAsync();
                return Ok(new Common.Models.Response.AccountResponse
                {
                    statusCode = System.Net.HttpStatusCode.OK,
                    Status = Constants.SUCCESS_STATUS,
                    Message = Constants.LOGIN_SUCCESSFULLY
                });
            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    await trx.RollbackAsync();
                }
                return Unauthorized(new Common.Models.Response.LoginResponse
                {
                    Status = Constants.FAILED_STATUS,
                    statusCode = System.Net.HttpStatusCode.Unauthorized,
                    Message = Constants.LOGIN_FAILED
                });
            }

            



            var newAccountId = Guid.NewGuid().ToString();

            return Ok(new
            {
                status = "success",
                message = "Account created successfully.",
                accountId = newAccountId
            });
        }

        [HttpGet("cust-account")]
        public async Task<IActionResult> GetCustomerAccounts([FromQuery] string ContactId)
        {
            if(!string.IsNullOrEmpty(ContactId))
            {
                var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);

                var AccountTypeListQuery = (await db.QueryAsync<dynamic>(_account.getCustomerAccounts, new { CustomerId = ContactId })).ToList();
                var AccountTypeListQueryList = AccountTypeListQuery.Select(x => new
                {
                    AccountId = (string)x.AccountId,
                    AccountNumber = (string)x.AccountNumber,
                    AccountCategoryName = (string)x.AccountCategoryName,
                    AccountCategoryId = (string)x.AccountCategoryId,
                    Balance = (long)x.Balance
                });


                return Ok(new Common.Models.ResponseModel
                {
                    Status = Constants.SUCCESS_STATUS,
                    Data = AccountTypeListQueryList,
                    statusCode = HttpStatusCode.OK
                });
            }
            else
            {
                return BadRequest(new Common.Models.Response.LoginResponse
                {
                    Status = Constants.FAILED_STATUS,
                    statusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = Constants.LOGIN_FAILED
                });
            }
                
        }

        [HttpGet("acc-detail")]
        public async Task<IActionResult> GetAccountDetail([FromQuery] string AccountId)
        {
            if (!string.IsNullOrEmpty(AccountId))
            {
                var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);

                var AccountTypeListQuery = (await db.QueryAsync<dynamic>(_account.getAccountById, new { AccountId = AccountId })).ToList();
                var AccountTypeListQueryList = AccountTypeListQuery.Select(x => new
                {
                    AccountId = (string)x.AccountId,
                    AccountNumber = (string)x.AccountNumber,
                    AccountCategoryName = (string)x.AccountCategoryName,
                    AccountCategoryId = (string)x.AccountCategoryId,
                    Balance = (long)x.Balance
                });


                return Ok(new Common.Models.ResponseModel
                {
                    Status = Constants.SUCCESS_STATUS,
                    Data = AccountTypeListQueryList,
                    statusCode = HttpStatusCode.OK
                });
            }
            else
            {
                return BadRequest(new Common.Models.Response.LoginResponse
                {
                    Status = Constants.FAILED_STATUS,
                    statusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = Constants.LOGIN_FAILED
                });
            }

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
